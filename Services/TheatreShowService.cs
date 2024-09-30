using System.Data.Common;
using Microsoft.AspNetCore.Mvc;
using StarterKit.Controllers;
using StarterKit.Models;
using StarterKit.Utils;
using System.Collections.Generic;
using System.Globalization;

namespace StarterKit.Services;

public class TheatreShowService : ControllerBase, ITheatreShowService
{

    private readonly DatabaseContext _context;

    public TheatreShowService(DatabaseContext context)
    {
        _context = context;
    }

    public List<TheatreShow> RetrieveAll()
        {
            var theatreShows = _context.TheatreShow
                .Select(show => new TheatreShow
                {
                    TheatreShowId = show.TheatreShowId,
                    Title = show.Title,
                    Description = show.Description,
                    Price = show.Price,
                    theatreShowDates = show.theatreShowDates,
                    Venue = show.Venue
                }).ToList();

            foreach (var show in theatreShows)
            {
                show.theatreShowDates = _context.TheatreShowDate
                    .Where(date => date.TheatreShow == show)
                    .ToList();
            }

            return theatreShows;
        }

    public TheatreShow RetrieveById(int id)
    {
        return _context.TheatreShow.FirstOrDefault(show => show.TheatreShowId == id);
    }

    public TheatreShow PostTheatreShow(TheatreShow theatreShow)
    {
        var existingVenue = _context.Venue.FirstOrDefault(v => v.VenueId == theatreShow.Venue.VenueId);
        if (existingVenue == null)
        {
            _context.Venue.Add(theatreShow.Venue);
        }
        else
        {
            theatreShow.Venue = existingVenue;
        }
    
        _context.TheatreShow.Add(theatreShow);
        foreach (TheatreShowDate t in theatreShow.theatreShowDates)
        {
            _context.TheatreShowDate.Add(t);
        }
    
        _context.SaveChanges();
        return theatreShow;
    }
    
    public int UpdateTheatreShow(TheatreShow theatreShow)
    {
        if (LoginController.LoggedIn != LoginStatus.Success) return 1;
        if (!_context.TheatreShow.Any(x => x.TheatreShowId == theatreShow.TheatreShowId)) return 2;
        _context.TheatreShow.Remove(_context.TheatreShow.FirstOrDefault(x => x.TheatreShowId == theatreShow.TheatreShowId));
        foreach (TheatreShowDate t in _context.TheatreShowDate.Where(x => x.TheatreShow.TheatreShowId == theatreShow.TheatreShowId)) _context.TheatreShowDate.Remove(t);
        PostTheatreShow(theatreShow);
        return 0;
    }
    
     public KeyValuePair<TheatreShow,int> DeleteTheatreShow(int showid)
    {
        if (LoginController.LoggedIn != LoginStatus.Success) return new KeyValuePair<TheatreShow, int>(null, 1);
        if (!_context.TheatreShow.Any(x => x.TheatreShowId == showid)) return new KeyValuePair<TheatreShow, int>(null, 2);
        foreach (TheatreShowDate t in _context.TheatreShowDate.Where(x => x.TheatreShow.TheatreShowId == showid)) _context.TheatreShowDate.Remove(t);
        TheatreShow show = _context.TheatreShow.FirstOrDefault(x => x.TheatreShowId == showid);
        _context.TheatreShow.Remove(show);
        _context.SaveChanges();
        return new KeyValuePair<TheatreShow, int>(show, 0);
    }

    [HttpGet]
    public List<TheatreShow> GetTheatreShows(
        int? id = null,
        string title = null,
        string description = null,
        string location = null,
        DateTime? startDate = null,
        DateTime? endDate = null,
        string sortBy = "title",
        string sortOrder = "asc")
    {
        var theatreShows = _context.TheatreShow.AsQueryable();
    
        if (id.HasValue)
        {
            theatreShows = theatreShows.Where(s => s.TheatreShowId == id.Value);
        }
    
        if (!string.IsNullOrEmpty(title))
        {
            theatreShows = theatreShows.Where(s => s.Title.Contains(title));
        }
    
        if (!string.IsNullOrEmpty(description))
        {
            theatreShows = theatreShows.Where(s => s.Description.Contains(description));
        }
    
        if (!string.IsNullOrEmpty(location))
        {
            theatreShows = theatreShows.Where(s => s.Venue.Name.Contains(location));
        }
    
        if (startDate.HasValue)
        {
            theatreShows = theatreShows.Where(s => s.theatreShowDates.Any(d => d.DateAndTime >= startDate.Value));
        }
    
        if (endDate.HasValue)
        {
            theatreShows = theatreShows.Where(s => s.theatreShowDates.Any(d => d.DateAndTime <= endDate.Value));
        }
    
        switch (sortBy.ToLower())
        {
            case "title":
                theatreShows = sortOrder.ToLower() == "desc" ? theatreShows.OrderByDescending(s => s.Title) : theatreShows.OrderBy(s => s.Title);
                break;
            case "price":
                theatreShows = sortOrder.ToLower() == "desc" ? theatreShows.OrderByDescending(s => s.Price) : theatreShows.OrderBy(s => s.Price);
                break;
            case "date":
                theatreShows = sortOrder.ToLower() == "desc" ? theatreShows.OrderByDescending(s => s.theatreShowDates.Min(d => d.DateAndTime)) : theatreShows.OrderBy(s => s.theatreShowDates.Min(d => d.DateAndTime));
                break;
            case "location":
                theatreShows = sortOrder.ToLower() == "desc" ? theatreShows.OrderByDescending(s => s.Venue.VenueId) : theatreShows.OrderBy(s => s.Venue);
                break;
            default:
                theatreShows = theatreShows.OrderBy(s => s.Title);
                break;
        }
    
        return theatreShows.ToList();
    }

    public List<TheatreShow> GetTheatreShowRange(string startdate, string enddate)
    {
        string format = "MM-dd-yyyy";
        Console.WriteLine($"Received Start Date: {startdate}");
        Console.WriteLine($"Received End Date: {enddate}");

        // Convert string dates to DateTime
        if (!DateTime.TryParseExact(startdate, format, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime startDate) ||
            !DateTime.TryParseExact(enddate, format, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime endDate))
        {
            throw new ArgumentException("Invalid date format. Please use 'MM-dd-yyyy'.");
        }

        // Ensure end date is greater than or equal to start date
        if (endDate < startDate)
        {
            throw new ArgumentException("End date must be greater than or equal to start date.");
        }
        Console.WriteLine($"{startDate}");
        Console.WriteLine($"{endDate}");
        Console.WriteLine($"{_context.TheatreShowDate.First().DateAndTime.GetType()}");

        if (_context.TheatreShowDate.First().DateAndTime >= startDate && _context.TheatreShowDate.First().DateAndTime <= endDate) Console.WriteLine("Cool");
        // Retrieve TheatreShows within the date range
        var theatreShowsdates = _context.TheatreShowDate.Where(showDate => showDate.DateAndTime == startDate).ToList();
        var theatreShows = theatreShowsdates.Select(showDate => showDate.TheatreShow).ToList();

        return theatreShows;
    }
}
    
