using Microsoft.AspNetCore.Mvc;
using StarterKit.Models;
using StarterKit.Services;
using StarterKit.Filters;


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
        [GuestOnly]
        [HttpPost]
        public IActionResult MakeReservation([FromBody] NonLoggedReservationRequest reservationrequest)
        {
            if (reservationrequest == null)
            {
                return BadRequest("Reservation requests cannot be empty.");
            }
            if (string.IsNullOrWhiteSpace(reservationrequest.Email))
            {
                return BadRequest("Email cannot be null or empty.\nAll reservation requests have been canceled. Please make sure all orders are correct to confirm your reservation(s)");
            }

            double totalPrice = 0;
            int points = 0;
            bool isSpecialOccasion = false;
            RewardDetails rewardDetails = null;
            var reservationRequests = reservationrequest.Requests;

            foreach (var request in reservationRequests)
            {
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

            }

            foreach (var request in reservationRequests)
            {
                var showDate = _theatreShowService.GetShowDateById(request.TheatreShowDateId);
                
                if (_rewardService.IsSpecialOccasion(showDate.DateAndTime))
                {
                    isSpecialOccasion = true;
                    rewardDetails = _rewardService.ApplySpecialOccasionRewards(reservationrequest.Email, showDate.DateAndTime);
                }

                float timeBonus = _reservationService.CalculateTimeBonus(showDate.DateAndTime);
                totalPrice += showDate.TheatreShow.Price * request.NumberOfTickets;

                var customer = _reservationService.GetCustomerByEmail(reservationrequest.Email);
                if (customer == null)
                {
                    customer = new Customer
                    {
                        FirstName = reservationrequest.FirstName,
                        LastName = reservationrequest.LastName,
                        Email = reservationrequest.Email,
                        Password = reservationrequest.Password == null ? "password" : reservationrequest.Password,
                        Points = 0,
                        Tier = "Standard"
                    };
                    _reservationService.AddCustomer(customer);
                }

                int reservationsThisYear = _reservationService.GetReservations()
                    .Count(r => r.Customer.Email == customer.Email && r.TheatreShowDate.DateAndTime.Year == DateTime.Now.Year);
                customer.UpdateTier(reservationsThisYear);
                float tierMultiplier = _reservationService.CalculateTierMultiplier(customer.Tier == "Bronze" ? 1 : customer.Tier == "Silver" ? 2 : customer.Tier == "Gold" ? 3 : 0);
                int totalPoints = (int)Math.Round(totalPrice * timeBonus * tierMultiplier);
                customer.Points += totalPoints;
                points += totalPoints;

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
                return Ok(new
                {
                    TotalPrice = totalPrice,
                    SpecialOccasion = true,
                    BonusPoints = rewardDetails.BonusPoints,
                    Discounts = rewardDetails.Discounts,
                    SpecialPerks = rewardDetails.SpecialPerks,
                    PointsReceived = points, 
                    Login = $"If you wish to log in with your emailaddress as username, use your password to get access granted. If you did not enter a password, you may use 'password' to log in."
                });
            }

            return Ok(new { TotalPrice = totalPrice, PointsReceived = points, Login = $"If you wish to log in with your emailaddress as username, use your password to get access granted. If you did not enter a password, you may use 'password' to log in."});
        }


        [UserOnly]
        [HttpPost("account")]
        public IActionResult MakeReservation([FromBody] List<LoggedReservationRequest> reservationrequest)
        {
            if (reservationrequest == null)
            {
                return BadRequest("Reservation requests cannot be empty.");
            }

            double totalPrice = 0;
            int points = 0;
            bool isSpecialOccasion = false;
            RewardDetails rewardDetails = null;
            var reservationRequests = reservationrequest;

            foreach (var request in reservationRequests)
            {
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

            }

            var customer = _reservationService.GetCustomerByEmail(LoginController.CurrentLoggedIn.Email);

            foreach (var request in reservationRequests)
            {
                var showDate = _theatreShowService.GetShowDateById(request.TheatreShowDateId);
                
                if (_rewardService.IsSpecialOccasion(showDate.DateAndTime))
                {
                    isSpecialOccasion = true;
                    rewardDetails = _rewardService.ApplySpecialOccasionRewards(customer.Email, showDate.DateAndTime);
                }

                float timeBonus = _reservationService.CalculateTimeBonus(showDate.DateAndTime);
                totalPrice += showDate.TheatreShow.Price * request.NumberOfTickets;

                int reservationsThisYear = _reservationService.GetReservations()
                    .Count(r => r.Customer.Email == customer.Email && r.TheatreShowDate.DateAndTime.Year == DateTime.Now.Year);
                customer.UpdateTier(reservationsThisYear);
                float tierMultiplier = _reservationService.CalculateTierMultiplier(customer.Tier == "Bronze" ? 1 : customer.Tier == "Silver" ? 2 : customer.Tier == "Gold" ? 3 : 0);
                int totalPoints = (int)Math.Round(totalPrice * timeBonus * tierMultiplier);
                customer.Points += totalPoints;
                points += totalPoints;

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
                return Ok(new
                {
                    TotalPrice = totalPrice,
                    SpecialOccasion = true,
                    BonusPoints = rewardDetails.BonusPoints,
                    Discounts = rewardDetails.Discounts,
                    SpecialPerks = rewardDetails.SpecialPerks,
                    PointsReceived = points, 
                });
            }
            if (customer.Discount)
            {
                _reservationService.HandleDiscount(customer);
                return Ok(new { TotalPrice = totalPrice / 2, PointsReceived = points, Discount = "50%"});
            }
            return Ok(new { TotalPrice = totalPrice, PointsReceived = points});
        }
    }

    // public class NonLoggedReservationRequest
    // {
    //     public string? FirstName { get; set; }
    //     public string? LastName { get; set; }
    //     public string? Email { get; set; }
    //     public int TheatreShowDateId { get; set; }
    //     public int NumberOfTickets { get; set; }
    // }

    public class LoggedReservationRequest
    {
        public int TheatreShowDateId { get; set; }
        public int NumberOfTickets { get; set; }
    }

    public class NonLoggedReservationRequest
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string Email { get; set; }
        public string? Password { get; set; }
        public List<LoggedReservationRequest> Requests { get; set; }
    }
}
