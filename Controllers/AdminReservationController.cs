using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StarterKit.Services;
using StarterKit.Filters;


namespace StarterKit.Controllers
{
    [Route("api/v1/admin/reservations")]
    [ApiController]
    public class AdminReservationController : ControllerBase
    {
        private readonly IReservationService _reservationService;
        private readonly ITheatreShowService _theatreShowService;

        public AdminReservationController(IReservationService reservationService, ITheatreShowService theatreShowService)
        {
            _reservationService = reservationService;
            _theatreShowService = theatreShowService;
        }
        [AdminOnly]
        [HttpGet]
        public IActionResult GetReservations([FromQuery] string? show, [FromQuery] DateTime? date)
        {
            var reservations = _reservationService.GetAllReservations();

            if (!string.IsNullOrEmpty(show))
            {
                reservations = reservations.Where(r => r.TheatreShowDate.TheatreShow.Title.Equals(show, StringComparison.OrdinalIgnoreCase)).ToList();
            }

            if (date.HasValue)
            {
                reservations = reservations.Where(r => r.TheatreShowDate.DateAndTime.Date == date.Value.Date).ToList();
            }

            if(reservations is null)
            {
                return NotFound("No reservations found for this date or show");
            }

            return Ok(reservations);
        }
        [AdminOnly]
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
                int resnumber;
                bool correctParse = int.TryParse(reservationNumber, out resnumber);
                if(correctParse == true)
                {
                    reservations = reservations.Where(r => r.ReservationId ==  resnumber).ToList();
                }
            }

            if (!reservations.Any())
            {
                return NotFound("No reservations found for the provided criteria.");
            }

            return Ok(reservations);
        }
        [AdminOnly]
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

        [AdminOnly]
        [HttpDelete("{id}")]
        public IActionResult DeleteReservation(int id)
        {
            var reservation = _reservationService.GetReservationById(id);
            if (reservation == null)
            {
                return NotFound("Reservation not found.");
            }

            bool IsDeleted = _reservationService.DeleteReservation(reservation);
            if(IsDeleted)
            {
                return Ok("Reservation deleted successfully.");
            }
            return BadRequest();
        }
    }
}
