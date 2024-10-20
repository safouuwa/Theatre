using StarterKit.Models;
using System.Collections.Generic;

namespace StarterKit.Services
{
    public interface IReservationService
    {
        public float CalculateTimeBonus(DateTime ReservationTime);
        void AddReservation(Reservation reservation);
        IEnumerable<Reservation> GetReservations();
        Reservation GetReservationById(int reservationId);
        public Customer GetCustomerByEmail(string email);
        public void AddCustomer(Customer customer);

        void UpdateReservation(Reservation reservation);
        bool DeleteReservation(Reservation reservation);

        List<ReservationDisplayModel> GetAllReservations();

        float CalculateTierMultiplier(int tier);
    }
}
