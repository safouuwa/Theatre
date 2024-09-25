using System.Data.Common;
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
}