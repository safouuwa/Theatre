using Microsoft.AspNetCore.Mvc;
using StarterKit.Services;
using StarterKit.Filters;

namespace StarterKit.Controllers;

[Route("api/v1/Venue")]
public class VenueController : Controller
{
    private readonly IVenueService _venueService;

    public VenueController(IVenueService service)
    {
        _venueService = service;
    }

    [AdminOnly]
    [HttpGet]
    public IActionResult GetAllVenues()
    {
        return Ok(_venueService.RetrieveAllVenues());
    }
}

