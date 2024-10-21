using System.Data.Common;
using Microsoft.AspNetCore.Mvc;
using StarterKit.Controllers;
using StarterKit.Models;
using StarterKit.Utils;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Components.Web;

namespace StarterKit.Services;

public class TheatreShowService : ControllerBase, ITheatreShowService
{

    private readonly DatabaseContext _context;

    public TheatreShowService(DatabaseContext context)
    {
        _context = context;
    }

    private static TheatreShowDisplayModel ConvertToTheatreShowDisplayModel(TheatreShow show)
    {
        if (show is null) return null;
        return new TheatreShowDisplayModel
        {
            TheatreShowId = show.TheatreShowId,
            Title = show.Title,
            Description = show.Description,
            Price = show.Price,
            Venue = new VenueDisplayModel
            {
                VenueId = show.Venue?.VenueId ?? 0,
                Name = show.Venue?.Name,
                Capacity = show.Venue.Capacity
            },
            TheatreShowDates = show.theatreShowDates?.Select(date => new TheatreShowDateDisplayModelForField
            {
                TheatreShowDateId = date.TheatreShowDateId,
                DateAndTime = date.DateAndTime,
                TheatreShowId = show.TheatreShowId
            }).ToList()
        };
    }

    private static TheatreShowDateDisplayModel ConvertToTheatreShowDateDisplayModel(TheatreShowDate show)
    {
        if (show is null) return null;
        return new TheatreShowDateDisplayModel
        {
            TheatreShowDateId = show.TheatreShowDateId,
            DateAndTime = show.DateAndTime,
            TheatreShow = new TheatreShowDisplayModelForField
            {
                TheatreShowId = show.TheatreShow.TheatreShowId,
                Title = show.TheatreShow.Title,
                Description = show.TheatreShow.Description,
                Price = show.TheatreShow.Price,
                Venue = new VenueDisplayModel
                {
                    VenueId = show.TheatreShow.Venue?.VenueId ?? 0,
                    Name = show.TheatreShow.Venue?.Name,
                    Capacity = show.TheatreShow.Venue.Capacity
                },
            }
        };
    }


    public List<TheatreShowDisplayModel> RetrieveAll()
{
    var theatreShows = _context.TheatreShow
        .Include(show => show.Venue)
        .Include(show => show.theatreShowDates)
        .ToList();

    return theatreShows.Select(show => ConvertToTheatreShowDisplayModel(show)).ToList();
}

public TheatreShowDisplayModel RetrieveById(int id)
{
    var show = _context.TheatreShow
        .Include(s => s.Venue)
        .Include(s => s.theatreShowDates)
        .FirstOrDefault(s => s.TheatreShowId == id);

    return show == null ? null : ConvertToTheatreShowDisplayModel(show);
    }

    public TheatreShowDisplayModel PostTheatreShow(TheatreShow theatreShow)
    {
        if (_context.TheatreShow.Any(x => x.TheatreShowId == theatreShow.TheatreShowId)) return null;
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
        return ConvertToTheatreShowDisplayModel(theatreShow);
    }
    
    public int UpdateTheatreShow(TheatreShow theatreShow)
    {
        if (!_context.TheatreShow.Any(x => x.TheatreShowId == theatreShow.TheatreShowId)) return 2;
        _context.TheatreShow.Remove(_context.TheatreShow.FirstOrDefault(x => x.TheatreShowId == theatreShow.TheatreShowId));
        foreach (TheatreShowDate t in _context.TheatreShowDate.Where(x => x.TheatreShow.TheatreShowId == theatreShow.TheatreShowId)) _context.TheatreShowDate.Remove(t);
        _context.SaveChanges();
        PostTheatreShow(theatreShow);
        return 0;
    }
    
     public KeyValuePair<TheatreShow,int> DeleteTheatreShow(int showid)
    {
        if (!_context.TheatreShow.Any(x => x.TheatreShowId == showid)) return new KeyValuePair<TheatreShow, int>(null, 2);
        foreach (TheatreShowDate t in _context.TheatreShowDate.Where(x => x.TheatreShow.TheatreShowId == showid)) _context.TheatreShowDate.Remove(t);
        TheatreShow show = _context.TheatreShow.FirstOrDefault(x => x.TheatreShowId == showid);
        _context.TheatreShow.Remove(show);
        _context.SaveChanges();
        return new KeyValuePair<TheatreShow, int>(show, 0);
    }

    public TheatreShowDate GetShowDateById(int showDateId)
    {
        TheatreShowDate date = _context.TheatreShowDate
                    .Include(x => x.Reservations)
                    .Include(sd => sd.TheatreShow)
                    .ThenInclude(ts => ts.Venue)
                    .FirstOrDefault(sd => sd.TheatreShowDateId == showDateId);
        return date;
    }



    public List<TheatreShowDisplayModel> GetTheatreShows(string sortBy, string sortOrder,
        int? id = null,
        string title = null,
        string description = null,
        string location = null,
        DateTime? startDate = null,
        DateTime? endDate = null)
    {
        if (sortOrder.ToLower() != "desc" && sortOrder.ToLower() != "asc") throw new ArgumentException("No valid sort order given; please enter 'asc' for ascending order of 'desc' for descending order.");
        var theatreShows = _context.TheatreShow.Include(show => show.Venue).Include(show => show.theatreShowDates).AsQueryable();
    
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
            case "description":
                theatreShows = sortOrder.ToLower() == "desc" ? theatreShows.OrderByDescending(s => s.Description) : theatreShows.OrderBy(s => s.Description);
                break;
            case "price":
                theatreShows = sortOrder.ToLower() == "desc" ? theatreShows.OrderByDescending(s => s.Price) : theatreShows.OrderBy(s => s.Price);
                break;
            case "location":
                theatreShows = sortOrder.ToLower() == "desc" ? theatreShows.OrderByDescending(s => s.Venue.VenueId) : theatreShows.OrderBy(s => s.Venue);
                break;
            default:
                theatreShows = null;
                break;
        }
        if (theatreShows is null) throw new ArgumentException("No valid sort criteria given");;
        var theatreShows1 = theatreShows.ToList();
        return theatreShows1.Select(show => ConvertToTheatreShowDisplayModel(show)).ToList();
    }

    public List<TheatreShowDisplayModel> GetTheatreShowRange(string startdate, string enddate)
    {
        string format = "MM-dd-yyyy";

        // Convert string dates to DateTime
        if (!DateTime.TryParseExact(startdate, format, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime startDate) ||
            !DateTime.TryParseExact(enddate, format, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime endDate))
        {
            throw new ArgumentException("Invalid date format. Please use 'MM-dd-yyyy'.");
        }

        if (endDate < startDate)
        {
            throw new ArgumentException("End date must be greater than or equal to start date.");
        }

        var dates = _context.TheatreShowDate
        .Include(s => s.TheatreShow)
        .ToList();

        var theatreShows = dates.Where(showDate => showDate.DateAndTime >= startDate && showDate.DateAndTime <= endDate).ToList();
        var newtheatreShows = theatreShows.Select(x => x.TheatreShow.TheatreShowId).ToList();

        var shows = newtheatreShows.Select(id => _context.TheatreShow.Include(x => x.Venue).First(x => x.TheatreShowId == id)).ToList();

        // Now you can convert to the display model
        return shows.Select(show => ConvertToTheatreShowDisplayModel(show)).ToList();

    }
}

