using StarterKit.Models;
using System.Collections.Generic;
using System.Linq;

namespace StarterKit.Services
{
    public class ReservationService : IReservationService
    {
        private readonly DatabaseContext _context;

        public ReservationService(DatabaseContext context)
        {
            _context = context;
        }

        public void AddReservation(Reservation reservation)
        {
            // Ensure that the customer is attached to the context
            if (reservation.Customer != null)
            {
                var existingCustomer = _context.Customer
                    .FirstOrDefault(c => c.Email == reservation.Customer.Email);

                if (existingCustomer != null)
                {
                    reservation.Customer = existingCustomer; // Use existing customer
                }
                else
                {
                    _context.Customer.Add(reservation.Customer); // Add new customer
                }
            }
            else
            {
                throw new ArgumentException("Customer cannot be null");
            }

            // Ensure that the TheatreShowDate is correctly set
            if (reservation.TheatreShowDate != null)
            {
                var showDate = _context.TheatreShowDate
                    .FirstOrDefault(sd => sd.TheatreShowDateId == reservation.TheatreShowDate.TheatreShowDateId);

                if (showDate == null)
                {
                    throw new ArgumentException("TheatreShowDate does not exist");
                }
                else
                {
                    reservation.TheatreShowDate = showDate; // Use existing show date
                }
            }
            else
            {
                throw new ArgumentException("TheatreShowDate cannot be null");
            }

            // Add reservation to the context
            _context.Reservation.Add(reservation);
            _context.SaveChanges();
        }


        public IEnumerable<Reservation> GetReservations()
        {
            return _context.Reservation
                .ToList();
        }

        public Reservation? GetReservationById(int reservationId)
        {
            return _context.Reservation
                .FirstOrDefault(r => r.ReservationId == reservationId);
        }

        public Customer? GetCustomerByEmail(string email)
        {
            return _context.Customer.FirstOrDefault(c => c.Email == email);
        }

        public void AddCustomer(Customer customer)
        {
            _context.Customer.Add(customer);
            _context.SaveChanges();
        }


    }
}
