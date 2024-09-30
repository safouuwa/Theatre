using StarterKit.Models;
namespace StarterKit.Services;

public interface ITheatreShowService
{
    public List<TheatreShow> RetrieveAll();
    public TheatreShow PostTheatreShow(TheatreShow theatreShow);
    public int UpdateTheatreShow(TheatreShow theatreShow);
    public KeyValuePair<TheatreShow, int> DeleteTheatreShow(int showid);
    public TheatreShow RetrieveById(int id); 
}