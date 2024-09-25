using System.Data.Common;
using Microsoft.AspNetCore.Mvc;
using StarterKit.Controllers;
using StarterKit.Models;
using StarterKit.Utils;

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
        if (theatreShow == null) Console.WriteLine("bruh");
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
}