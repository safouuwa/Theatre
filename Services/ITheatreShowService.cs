using StarterKit.Models;
namespace StarterKit.Services;

public interface ITheatreShowService
{
    public List<TheatreShowDisplayModel> RetrieveAll();
    public TheatreShow PostTheatreShow(TheatreShow theatreShow);
    public int UpdateTheatreShow(TheatreShow theatreShow);
    public KeyValuePair<TheatreShow, int> DeleteTheatreShow(int showid);
    
    public TheatreShowDisplayModel RetrieveById(int id); 
     // New method for filtering and sorting theatre shows
    public List<TheatreShowDisplayModel> GetTheatreShows(
            int? id = null,
            string title = null,
            string description = null,
            string location = null,
            DateTime? startDate = null,
            DateTime? endDate = null,
            string sortBy = "title",
            string sortOrder = "asc");

    public List<TheatreShow> GetTheatreShowRange(string startdate, string enddate);
    
}