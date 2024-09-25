using StarterKit.Models;
namespace StarterKit.Services;

public interface ITheatreShowService
{
    public List<TheatreShow> RetrieveAll();
    public TheatreShow PostTheatreShow(TheatreShow theatreShow);
}