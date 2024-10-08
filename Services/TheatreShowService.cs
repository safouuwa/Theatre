using System.Data.Common;
using Microsoft.AspNetCore.Mvc;
using StarterKit.Controllers;
using StarterKit.Models;
using StarterKit.Utils;
using Microsoft.EntityFrameworkCore;


namespace StarterKit.Services;

public class TheatreShowService : ITheatreShowService
{

    private readonly DatabaseContext _context;

    public TheatreShowService(DatabaseContext context)
    {
        _context = context;
    }

    public List<TheatreShow> RetrieveAll()
    {
        List<TheatreShow> mainlist = _context.TheatreShow.ToList().Select(show => new TheatreShow
        {
            TheatreShowId = show.TheatreShowId,
            Title = show.Title,
            Description = show.Description,
            Price = show.Price,
            theatreShowDates = show.theatreShowDates,
            Venue = show.Venue
        }).ToList();
        foreach (TheatreShow t in mainlist)
        {
            t.theatreShowDates = _context.TheatreShowDate.Where(x => x.TheatreShow == t).ToList();
            foreach (TheatreShowDate ts in t.theatreShowDates) ts.TheatreShow = null;
            t.Venue = _context.Venue.FirstOrDefault(x => x.TheatreShows.Contains(t));
            t.Venue.TheatreShows = null;
        }
        return mainlist;
    }

    public TheatreShow PostTheatreShow(TheatreShow theatreShow)
    {
        if (LoginController.LoggedIn != LoginStatus.Success) return null;
        var existingVenue = _context.Venue.FirstOrDefault(x => x.VenueId == theatreShow.Venue.VenueId);
        if (existingVenue == null)
        {
            _context.Venue.Add(theatreShow.Venue);
        }
        else
        {
            theatreShow.Venue = existingVenue;
        }
        _context.TheatreShow.Add(theatreShow);
        foreach (TheatreShowDate t in theatreShow.theatreShowDates) _context.TheatreShowDate.Add(t);
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

    public TheatreShowDate GetShowDateById(int showDateId)
    {
        return _context.TheatreShowDate
                    .Include(sd => sd.TheatreShow)
                    .ThenInclude(ts => ts.Venue)
                    .FirstOrDefault(sd => sd.TheatreShowDateId == showDateId);
    }

}