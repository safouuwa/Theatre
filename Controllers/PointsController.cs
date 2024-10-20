using Microsoft.AspNetCore.Mvc;
using StarterKit.Models;
using StarterKit.Filters;
using StarterKit.Services;

namespace StarterKit.Controllers
{
    [Route("api/v1/points")]
    public class PointsController : Controller
    {
        private Customer customer = LoginController.CurrentLoggedIn;
        public readonly IPointService service;
        public PointsController(IPointService _service)
        {
            service = _service;
        }
        [UserOnly]
        [HttpPost("gift")]
        public IActionResult Post([FromBody] GiftBody gift)
        {
            customer = service.RefreshCustomer(customer.Email, customer.Password);
            if (gift == null) return BadRequest("");
            if (customer.Email == gift.Email) return BadRequest("Cannot gift points to your own account.");
            if (gift.PointAmount > customer.Points) return BadRequest("Request to gift more points than your account currently owns.");
            bool check = service.GiftPoints(customer.Email, gift.Email, gift.PointAmount);
            if (check is false) return BadRequest("No customer with given email.");
            return Ok($"{gift.PointAmount} points gifted to {gift.Email}!\n Your Points: {customer.Points}");
        }

    }

    public class GiftBody
    {
        public string Email { get; set; }
        public int PointAmount { get; set; }
    }
}