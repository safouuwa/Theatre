using StarterKit.Models;

namespace StarterKit.Services;

public interface IVenueService
{
    public List<VenueDisplayModel> RetrieveAllVenues();
}

