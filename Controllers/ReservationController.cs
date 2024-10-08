using Microsoft.AspNetCore.Mvc;
using StarterKit.Models;
using StarterKit.Services;
using System.Collections.Generic;
using System.Linq;

namespace StarterKit.Controllers
{
    [Route("api/v1/Reservation")]
    public class ReservationController : Controller
    {
        private readonly IReservationService _reservationService;
        private readonly ITheatreShowService _theatreShowService;

        public ReservationController(IReservationService reservationService, ITheatreShowService theatreShowService)
        {
            _reservationService = reservationService;
            _theatreShowService = theatreShowService;
        }

        [HttpPost]
        public IActionResult MakeReservation([FromBody] List<ReservationRequest> reservationRequests)
        {
            if (reservationRequests == null || !reservationRequests.Any())
            {
                return BadRequest("Reservation requests cannot be empty.");
            }

            double totalPrice = 0;

            foreach (var request in reservationRequests)
            {
                if (string.IsNullOrWhiteSpace(request.Email))
                {
                    return BadRequest("Email cannot be null or empty.");
                }
                var showDate = _theatreShowService.GetShowDateById(request.TheatreShowDateId);

                if (showDate == null)
                {
                    return NotFound($"Show date with ID {request.TheatreShowDateId} not found.");
                }
                if (showDate.TheatreShow == null)
                {
                    return BadRequest("Theatre show information is missing.");
                }

                var venue = showDate.TheatreShow.Venue;
                if (venue == null)
                {
                    return BadRequest("Venue information is missing.");
                }
                int reservedTickets = showDate.Reservations?.Sum(r => r.AmountOfTickets) ?? 0;

                if (reservedTickets + request.NumberOfTickets > venue.Capacity)
                {
                    return BadRequest($"Not enough tickets available for show date ID {request.TheatreShowDateId}. Requested: {request.NumberOfTickets}, Available: {venue.Capacity - reservedTickets}");
                }

                if (reservedTickets + request.NumberOfTickets > venue.Capacity)
                {
                    return BadRequest($"Not enough tickets available for show date ID {request.TheatreShowDateId}. Requested: {request.NumberOfTickets}, Available: {venue.Capacity - reservedTickets}");
                }

                totalPrice += showDate.TheatreShow.Price * request.NumberOfTickets;

                var customer = _reservationService.GetCustomerByEmail(request.Email);
                if (customer == null)
                {
                    customer = new Customer
                    {
                        FirstName = request.FirstName,
                        LastName = request.LastName,
                        Email = request.Email
                    };
                    _reservationService.AddCustomer(customer);
                }



                var reservation = new Reservation
                {
                    AmountOfTickets = request.NumberOfTickets,
                    Used = false,
                    Customer = customer,
                    TheatreShowDate = showDate
                };

                _reservationService.AddReservation(reservation);
            }

            return Ok(new { TotalPrice = totalPrice });
        }
    }

    public class ReservationRequest
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Email { get; set; }
        public int TheatreShowDateId { get; set; }
        public int NumberOfTickets { get; set; }
    }
}
