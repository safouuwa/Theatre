using StarterKit.Models;
using Microsoft.EntityFrameworkCore;

namespace StarterKit.Services;

public class VenueService : IVenueService
{
    private readonly DatabaseContext _context;

    public VenueService(DatabaseContext context)
    {
        _context = context;
    }

    public List<VenueDisplayModel> RetrieveAllVenues()
    {
        var venues = _context.Venue.ToList();
        return venues.Select(venue => new VenueDisplayModel
        {
            VenueId = (int)venue.VenueId,
            Name = venue.Name,
            Capacity = (int)venue.Capacity
        }).ToList();
    }
}

