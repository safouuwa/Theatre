using System.Text;
using Microsoft.AspNetCore.Mvc;
using StarterKit.Services;
using StarterKit.Models;
using StarterKit.Filters;

    namespace StarterKit.Controllers;


[Route("api/v1/TheatreShow")]
public class TheatreShowController : Controller
{
    private readonly ITheatreShowService _theatreShowService;
    public TheatreShowController(ITheatreShowService service) => _theatreShowService = service;

    [HttpGet()]
    public IActionResult ShowAll() => Ok(_theatreShowService.RetrieveAll());

    [HttpGet("id/{id}")]
    public IActionResult Get(int id)
    {
        var show = _theatreShowService.RetrieveById(id);
        if (show == null)
        {
            return NotFound("Theatre show not found in the database");
        }
        return Ok(show);
    }
    [AdminOnly]
    [HttpPost()]
    public IActionResult PostTheatreShow([FromBody]TheatreShow theatreShow)
    {
        if (theatreShow == null) return BadRequest("Data in body not sufficient or correct for a Theatre Show");
        if (theatreShow.TheatreShowId != null || theatreShow.theatreShowDates.Any(x => x.TheatreShowDateId != null) || theatreShow.theatreShowDates.Any(x => x.TheatreShow != null)) return BadRequest("ID fields (except for VenueId) are auto incremented, thus should not be filled.");
        if (theatreShow.Venue.VenueId == 0 && (theatreShow.Venue.Name == null || theatreShow.Venue.Capacity == 0)) return BadRequest("For the venue, please either only enter the ID of an existing one, or register a new one by only providing a Name and Capacity.");
        TheatreShowDisplayModel show = _theatreShowService.PostTheatreShow(theatreShow);
        if (show == null) return NotFound("ID for Venue not found.");
        return Ok($" {show.Title} added to the database!");
    }
    [AdminOnly]
    [HttpPut("{showid}")]
    public IActionResult UpdateTheatreShow([FromRoute] int showid, [FromBody] TheatreShow theatreShow)
    {
        if (theatreShow == null) return BadRequest("Data in body not sufficient or correct for a Theatre Show");
        if (theatreShow.TheatreShowId != null || theatreShow.theatreShowDates.Any(x => x.TheatreShowDateId != null) || theatreShow.theatreShowDates.Any(x => x.TheatreShow != null)) return BadRequest("ID fields (except for VenueId) are auto incremented, thus should not be filled.");
        if (theatreShow.Venue.VenueId == 0 && (theatreShow.Venue.Name == null || theatreShow.Venue.Capacity == 0)) return BadRequest("For the venue, please either only enter the ID of an existing one, or register a new one by only providing a Name and Capacity.");
        theatreShow.TheatreShowId = showid;
        int show = _theatreShowService.UpdateTheatreShow(theatreShow);
        if (show is 2) return Unauthorized("Given data does not exist in database; nothing to update");
        return Ok($" {theatreShow.Title} updated in the database!");
    }
    [AdminOnly]
    [HttpDelete("{showid}")]
    public IActionResult DeleteTheatreShow([FromRoute] int showid)
    {
        KeyValuePair<TheatreShow,int> show = _theatreShowService.DeleteTheatreShow(showid);
        if (show.Value is 2) return Unauthorized("Given data does not exist in database; nothing to delete");
        return Ok($" {show.Key.Title} deleted from the database!");
    }

    [HttpGet("filter/{sortBy}/{sortOrder}")]
    public IActionResult GetTheatreShows([FromRoute]string sortBy, [FromRoute]string sortOrder,
        int? id = null,
        string title = null,
        string description = null,
        string location = null,
        DateTime? startDate = null,
        DateTime? endDate = null)
    {
        var shows = _theatreShowService.GetTheatreShows(sortBy, sortOrder, id, title, description, location, startDate, endDate);
        if (shows is null) return NotFound("No correct sort criteria given");
        return Ok(shows);
    }
    [HttpGet("filter/date/{startdate}/{enddate}")]
    public IActionResult GetTheatreShowsRange([FromRoute]string startdate, [FromRoute]string enddate)
    {
        var shows = _theatreShowService.GetTheatreShowRange(startdate, enddate);
        return Ok(shows);
    }
}

