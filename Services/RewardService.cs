using StarterKit.Models;
using System;
using System.Collections.Generic;

namespace StarterKit.Services
{
    public class RewardService : IRewardService
    {
        private readonly IReservationService _reservationService;

        public RewardService(IReservationService reservationService)
        {
            _reservationService = reservationService;
        }

        public bool IsSpecialOccasion(DateTime reservationDate)
        {
            DateTime[] specialDates = new DateTime[]
            {
                new DateTime(DateTime.Now.Year, 12, 25),
                // Christmas If needed you can add more just type the month then the date. beacuse the year is already set to DateTime.Now.Year.
            };

            foreach (var date in specialDates)
            {
                if (reservationDate.Date == date.Date) return true;
            }

            return false;
        }

        public RewardDetails ApplySpecialOccasionRewards(string customerEmail, DateTime reservationDate)
        {
            var customer = _reservationService.GetCustomerByEmail(customerEmail);

            if (customer == null)
                throw new Exception("Customer not found.");

            var rewardDetails = new RewardDetails
            {
                BonusPoints = 100,
                Discounts = 15.0m,
                SpecialPerks = "Free Refreshments and Premium Seating"
            };

            customer.Points += rewardDetails.BonusPoints;

            _reservationService.UpdateReservation(new Reservation
            {
                Customer = customer
            });

            return rewardDetails;
        }

        public Customer RefreshCustomer(string email, string password)
        {
            return new Customer
            {
                Email = email,
                Password = password
            };
        }
    }

    public class RewardDetails
    {
        public int BonusPoints { get; set; }
        public decimal Discounts { get; set; }
        public string SpecialPerks { get; set; }
    }
}
