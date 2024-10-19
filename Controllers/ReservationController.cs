using Microsoft.AspNetCore.Mvc;
using StarterKit.Models;
using StarterKit.Services;


namespace StarterKit.Controllers
{
    [Route("api/v1/Reservation")]
    public class ReservationController : Controller
    {
        private readonly IReservationService _reservationService;
        private readonly ITheatreShowService _theatreShowService;
        private readonly IRewardService _rewardService;

        public ReservationController(IReservationService reservationService, ITheatreShowService theatreShowService, IRewardService rewardService)
        {
            _reservationService = reservationService;
            _theatreShowService = theatreShowService;
            _rewardService = rewardService;
        }

        [HttpPost]
        public IActionResult MakeReservation([FromBody] List<ReservationRequest> reservationRequests)
        {
            if (reservationRequests == null || !reservationRequests.Any())
            {
                return BadRequest("Reservation requests cannot be empty.");
            }

            double totalPrice = 0;
            bool isSpecialOccasion = false;
            RewardDetails rewardDetails = null;

            foreach (var request in reservationRequests)
            {
                if (string.IsNullOrWhiteSpace(request.Email))
                {
                    return BadRequest("Email cannot be null or empty.\nAll reservation requests have been canceled. Please make sure all orders are correct to confirm your reservation(s)");
                }
                var showDate = _theatreShowService.GetShowDateById(request.TheatreShowDateId);

                if (showDate == null)
                {
                    return NotFound($"Show date with ID {request.TheatreShowDateId} not found.\nAll reservation requests have been canceled. Please make sure all orders are correct to confirm your reservation(s)");
                }
                if (showDate.TheatreShow == null)
                {
                    return BadRequest("Theatre show information is missing.\nAll reservation requests have been canceled. Please make sure all orders are correct to confirm your reservation(s)");
                }

                if (showDate.DateAndTime < DateTime.Now)
                {
                    return BadRequest($"Show date {showDate.DateAndTime} is in the past.\nAll reservation requests have been canceled. Please make sure all orders are correct to confirm your reservation(s)");
                }

                var venue = showDate.TheatreShow.Venue;
                if (venue == null)
                {
                    return BadRequest("Venue information is missing.\nAll reservation requests have been canceled. Please make sure all orders are correct to confirm your reservation(s)");
                }
                int reservedTickets = showDate.Reservations?.Sum(r => r.AmountOfTickets) ?? 0;

                if (reservedTickets + request.NumberOfTickets > venue.Capacity)
                {
                    return BadRequest($"Not enough tickets available for show date ID {request.TheatreShowDateId}. Requested: {request.NumberOfTickets}, Available: {venue.Capacity - reservedTickets}\nAll reservation requests have been canceled. Please make sure all orders are correct to confirm your reservation(s)");
                }
                if (_rewardService.IsSpecialOccasion(showDate.DateAndTime))
                {
                    isSpecialOccasion = true;
                    rewardDetails = _rewardService.ApplySpecialOccasionRewards(request.Email, showDate.DateAndTime);
                }
            }

            foreach (var request in reservationRequests)
            {
                var showDate = _theatreShowService.GetShowDateById(request.TheatreShowDateId);
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

            if (isSpecialOccasion && rewardDetails != null)
            {
                return Ok(new { TotalPrice = totalPrice,
                                SpecialOccasion = true,
                                BonusPoints = rewardDetails.BonusPoints,
                                Discounts = rewardDetails.Discounts,
                                SpecialPerks = rewardDetails.SpecialPerks 
                                });   
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
