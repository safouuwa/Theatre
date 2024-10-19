using Microsoft.EntityFrameworkCore;
using StarterKit.Models;
using System.Collections.Generic;
using System.Linq;
using System;

namespace StarterKit.Services
{
    public class ReservationService : IReservationService
    {
        private readonly DatabaseContext _context;

        public ReservationService(DatabaseContext context)
        {
            _context = context;
        }

        public float CalculateTimeBonus(DateTime ReservationTime)
        {
            TimeSpan currTime = ReservationTime.TimeOfDay;

            TimeSpan morningStart  = new TimeSpan(9, 0, 0);
            TimeSpan noonStart = new TimeSpan(12, 0, 0);
            TimeSpan afternoonStart = new TimeSpan(14, 0, 0);
            TimeSpan eveningStart = new TimeSpan(19, 0, 0);
            TimeSpan midnight = new TimeSpan(0, 0, 0);
            if (currTime >= morningStart && currTime < noonStart)
            {
                return 2.0f; // 9:00 AM - 12:00 PM -> 2x points
            }
            else if (currTime >= noonStart && currTime < afternoonStart)
            {
                return 1.5f; // 12:00 PM - 2:00 PM -> 1.5x points
            }
            else if (currTime >= afternoonStart && currTime < eveningStart)
            {
                return 1.0f; // 2:00 PM - 7:00 PM -> 1x points
            }
            else if (currTime >= eveningStart || currTime < midnight)
            {
                return 1.25f; // 7:00 PM - 12:00 AM -> 1.25x points
            }

            // Default multiplier (if time doesn't fit into defined ranges)
            return 1.0f;
        }

        public ReservationDisplayModel ConvertToDisplayModel(Reservation r)
        {
            return new ReservationDisplayModel
            {
                ReservationId = r.ReservationId,
                AmountOfTickets = r.AmountOfTickets,
                Used = r.Used,
                Customer = new CustomerDisplayModel
                {
                    CustomerId = r.Customer.CustomerId,
                    FirstName = r.Customer.FirstName,
                    LastName = r.Customer.LastName,
                    Email = r.Customer.Email,
                },
                TheatreShowDate = new TheatreShowDateDisplayModel
                {
                    TheatreShowDateId = r.TheatreShowDate.TheatreShowDateId,
                    DateAndTime = r.TheatreShowDate.DateAndTime,
                    TheatreShow = new TheatreShowDisplayModelForField
                    {
                        TheatreShowId = r.TheatreShowDate.TheatreShow.TheatreShowId,
                        Title = r.TheatreShowDate.TheatreShow.Title,
                        Description = r.TheatreShowDate.TheatreShow.Description,
                        Price = r.TheatreShowDate.TheatreShow.Price,
                        Venue = new VenueDisplayModel
                        {
                            VenueId = r.TheatreShowDate.TheatreShow.Venue?.VenueId ?? 0,
                            Name = r.TheatreShowDate.TheatreShow.Venue?.Name,
                            Capacity = r.TheatreShowDate.TheatreShow.Venue.Capacity
                        }
                    }
                }
            };
        }

        public void AddReservation(Reservation reservation)
        {
            if (reservation.Customer != null)
            {
                var existingCustomer = _context.Customer
                    .FirstOrDefault(c => c.Email == reservation.Customer.Email);

                if (existingCustomer != null)
                {
                    reservation.Customer = existingCustomer;
                }
                else
                {
                    _context.Customer.Add(reservation.Customer);
                }
            }
            else
            {
                throw new ArgumentException("Customer cannot be null");
            }

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
                    reservation.TheatreShowDate = showDate;
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
        public void UpdateReservation(Reservation reservation)
        {
            var existingReservation = GetReservationById(reservation.ReservationId);
            if (existingReservation != null)
            {
                existingReservation.Used = reservation.Used;
                _context.Reservation.Update(existingReservation);
                _context.SaveChanges();
            }
            else
            {
                throw new ArgumentException("Reservation does not exist");
            }
        }

        public bool DeleteReservation(Reservation reservation)
        {
            if(reservation != null)
            {
                _context.Reservation.Remove(reservation);
                _context.SaveChanges();
                return true;
            }
            return false;

        }

        public List<ReservationDisplayModel> GetAllReservations()
        {
            var all = _context.Reservation
                .Include(x => x.Customer)
                .Include(x => x.TheatreShowDate.TheatreShow.Venue)
                .ToList();
            return all.Select(x => ConvertToDisplayModel(x)).ToList();
        }
    }
}

