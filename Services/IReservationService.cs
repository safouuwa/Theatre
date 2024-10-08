using StarterKit.Models;
using System.Collections.Generic;

namespace StarterKit.Services
{
    public interface IReservationService
    {
        void AddReservation(Reservation reservation);
        IEnumerable<Reservation> GetReservations();
        Reservation GetReservationById(int reservationId);
        public Customer GetCustomerByEmail(string email);
        public void AddCustomer(Customer customer);

    }
}
