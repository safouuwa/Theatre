using System.Text;
using Microsoft.AspNetCore.Mvc;
using StarterKit.Services;
using StarterKit.Models;

namespace StarterKit.Controllers;


[Route("api/v1/TheatreShow")]
public class TheatreShowController : Controller
{
    private readonly ITheatreShowService _theatreShowService;
    public TheatreShowController(ITheatreShowService service) => _theatreShowService = service;

    [HttpGet()]
    public IActionResult ShowAll() => Ok(_theatreShowService.RetrieveAll());

    [HttpPost()]
    public IActionResult PostTheatreShow([FromBody]TheatreShow theatreShow)
    {
        TheatreShow show = _theatreShowService.PostTheatreShow(theatreShow);
        if (show is null) return Unauthorized("Admin is not logged in; no access to this feature");
        return Ok($" {show.Title} added to the database!");
    }

    [HttpPut("Update")]
    public IActionResult UpdateTheatreShow([FromBody] TheatreShow theatreShow)
    {
        int show = _theatreShowService.UpdateTheatreShow(theatreShow);
        if (show is 1) return Unauthorized("Admin is not logged in; no access to this feature");
        if (show is 2) return Unauthorized("Given data does not exist in database; nothing to update");
        return Ok($" {theatreShow.Title} updated in the database!");
    }

    [HttpDelete("{showid}")]
    public IActionResult DeleteTheatreShow([FromRoute] int showid)
    {
        Console.WriteLine(showid);
        KeyValuePair<TheatreShow,int> show = _theatreShowService.DeleteTheatreShow(showid);
        if (show.Value is 1) return Unauthorized("Admin is not logged in; no access to this feature");
        if (show.Value is 2) return Unauthorized("Given data does not exist in database; nothing to delete");
        return Ok($" {show.Key.Title} deleted from the database!");
    }

    [HttpGet]
        public IActionResult GetTheatreShows(
            [FromQuery] int? id = null,
            [FromQuery] string title = null,
            [FromQuery] string description = null,
            [FromQuery] string venue = null,
            [FromQuery] DateTime? startDate = null,
            [FromQuery] DateTime? endDate = null,
            [FromQuery] string sortBy = "title",
            [FromQuery] bool ascending = true)
        {
            if (id.HasValue)
            {
                var show = _theatreShowService.GetTheatreShowById(id.Value);
                if (show == null) return NotFound("Show not found");
                return Ok(show);
            }

            var shows = _theatreShowService.GetAllTheatreShows();

            if (!string.IsNullOrEmpty(title))
            {
                shows = shows.Where(s => s.Title.Contains(title, StringComparison.OrdinalIgnoreCase)).ToList();
            }

            if (!string.IsNullOrEmpty(description))
            {
                shows = shows.Where(s => s.Description.Contains(description, StringComparison.OrdinalIgnoreCase)).ToList();
            }
            
            if (!string.IsNullOrEmpty(venue))
            {
                shows = shows.Where(s => s.Venue.Name.Contains(venue, StringComparison.OrdinalIgnoreCase)).ToList();
            }

            if (startDate.HasValue)
            {
                shows = shows.Where(s => s.Date >= startDate.Value).ToList();
            }

            if (endDate.HasValue)
            {
                shows = shows.Where(s => s.Date <= endDate.Value).ToList();
            }

            shows = sortBy.ToLower() switch
            {
                "price" => ascending ? shows.OrderBy(s => s.Price).ToList() : shows.OrderByDescending(s => s.Price).ToList(),
                "date" => ascending ? shows.OrderBy(s => s.Date).ToList() : shows.OrderByDescending(s => s.Date).ToList(),
                _ => ascending ? shows.OrderBy(s => s.Title).ToList() : shows.OrderByDescending(s => s.Title).ToList(),
            };

            return Ok(shows);
        }
}

