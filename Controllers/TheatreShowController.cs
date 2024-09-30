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

    [HttpGet("id/{id}")]
    public IActionResult Get(int id)
    {
        if (id <= 0)
        {
            return BadRequest("Invalid ID provided");
        }
    
        try
        {
            Console.WriteLine($"Retrieving show with ID: {id}");
            var show = _theatreShowService?.RetrieveById(id);
            if (show == null)
            {
                Console.WriteLine("Theatre show not found");
                return NotFound("Theatre show not found in the database");
            }
            return Ok(show);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Exception: {ex.Message}");
            return StatusCode(500, "An error occurred while processing your request");
        }
    }

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
}

