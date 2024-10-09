using StarterKit.Models;
namespace StarterKit.Services;

public interface ITheatreShowService
{
    public List<TheatreShowDisplayModel> RetrieveAll();
    public TheatreShowDisplayModel PostTheatreShow(TheatreShow theatreShow);
    public int UpdateTheatreShow(TheatreShow theatreShow);
    public KeyValuePair<TheatreShow, int> DeleteTheatreShow(int showid);
    
    public TheatreShowDisplayModel RetrieveById(int id); 
     // New method for filtering and sorting theatre shows
    public List<TheatreShowDisplayModel> GetTheatreShows(string sortBy, string sortOrder,
            int? id = null,
            string title = null,    
            string description = null,
            string location = null,
            DateTime? startDate = null,
            DateTime? endDate = null);

    public List<TheatreShowDisplayModel> GetTheatreShowRange(string startdate, string enddate);
    
    TheatreShowDate GetShowDateById(int showDateId);
}