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

        [UserOnly]
        [HttpGet("shop")]
        public IActionResult GetShopInfo()
        {
            customer = service.RefreshCustomer(customer.Email, customer.Password);
            return Ok($"Your Points: {customer.Points}\nAvailable Items:\n50% Discount off your next order: 200 Points\nUse shop/discount to purchase!"); //more rewards added in the future
        }

        [UserOnly]
        [HttpGet("shop/discount")]
        public IActionResult GetDiscount()
        {
            customer = service.RefreshCustomer(customer.Email, customer.Password);
            var prepoints = customer.Points;
            bool check = service.BuyDiscount(customer);
            if (check is false) return BadRequest("Purchase failed; Insufficient amount of Points");
            return Ok($"50% Discount Bought!\nYour Points: {prepoints} -> {customer.Points}\nDiscount will be applied at checkout of your next reservation!");
        }

    }

    public class GiftBody
    {
        public string Email { get; set; }
        public int PointAmount { get; set; }
    }
}