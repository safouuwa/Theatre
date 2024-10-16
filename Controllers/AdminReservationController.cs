using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StarterKit.Models;
using StarterKit.Services;
using System.Linq;

namespace StarterKit.Controllers
{
    [Route("api/v1/admin/reservations")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class AdminReservationController : ControllerBase
    {
        private readonly IReservationService _reservationService;
        private readonly ITheatreShowService _theatreShowService;

        public AdminReservationController(IReservationService reservationService, ITheatreShowService theatreShowService)
        {
            _reservationService = reservationService;
            _theatreShowService = theatreShowService;
        }

        [HttpGet]
        public IActionResult GetReservations([FromQuery] string? show, [FromQuery] DateTime? date)
        {
            var reservations = _reservationService.GetAllReservations();

            if (!string.IsNullOrEmpty(show))
            {
                reservations = reservations.Where(r => r.TheatreShowDate.TheatreShow.Name.Equals(show, StringComparison.OrdinalIgnoreCase)).ToList();
            }

            if (date.HasValue)
            {
                reservations = reservations.Where(r => r.TheatreShowDate.DateAndTime.Date == date.Value.Date).ToList();
            }

            return Ok(reservations);
        }

        [HttpGet("search")]
        public IActionResult SearchReservation([FromQuery] string? email, [FromQuery] string? reservationNumber)
        {
            if (string.IsNullOrEmpty(email) && string.IsNullOrEmpty(reservationNumber))
            {
                return BadRequest("You must provide either an email or a reservation number to search.");
            }

            var reservations = _reservationService.GetAllReservations();

            if (!string.IsNullOrEmpty(email))
            {
                reservations = reservations.Where(r => r.Customer.Email.Equals(email, StringComparison.OrdinalIgnoreCase)).ToList();
            }

            if (!string.IsNullOrEmpty(reservationNumber))
            {
                reservations = reservations.Where(r => r.ReservationNumber == reservationNumber).ToList();
            }

            if (!reservations.Any())
            {
                return NotFound("No reservations found for the provided criteria.");
            }

            return Ok(reservations);
        }

        [HttpPatch("{id}/mark-used")]
        public IActionResult MarkReservationAsUsed(int id)
        {
            var reservation = _reservationService.GetReservationById(id);
            if (reservation == null)
            {
                return NotFound("Reservation not found.");
            }

            if (reservation.Used)
            {
                return BadRequest("Reservation has already been marked as used.");
            }

            reservation.Used = true;
            _reservationService.UpdateReservation(reservation);

            return Ok("Reservation marked as used.");
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteReservation(int id)
        {
            var reservation = _reservationService.GetReservationById(id);
            if (reservation == null)
            {
                return NotFound("Reservation not found.");
            }

            _reservationService.DeleteReservation(reservation);

            return Ok("Reservation deleted successfully.");
        }
    }
}
