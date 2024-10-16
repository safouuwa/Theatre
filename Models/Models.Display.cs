namespace StarterKit.Models
{
    public class CustomerDisplayModel
    {
        public int CustomerId { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Email { get; set; }
        // You can choose to include a simplified view of Reservations if neede
    }

    public class ReservationDisplayModel
    {
        public int ReservationId { get; set; }
        public int AmountOfTickets { get; set; }
        public bool Used { get; set; }
        // Optionally include the TheatreShowDate details

        public CustomerDisplayModel Customer { get; set; }
        public TheatreShowDateDisplayModel? TheatreShowDate { get; set; }
    }

    public class TheatreShowDateDisplayModel
    {
        public int TheatreShowDateId { get; set; }
        public DateTime DateAndTime { get; set; } // Format can be handled at the display layer
        // Optionally include TheatreShow details
        public TheatreShowDisplayModelForField TheatreShow { get; set; } // Just reference ID
    }

    public class TheatreShowDisplayModel
    {
        public int TheatreShowId { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public double Price { get; set; }
        // You might want to include just the dates or a simplified list
        public List<TheatreShowDateDisplayModelForField>? TheatreShowDates { get; set; }
        public VenueDisplayModel? Venue { get; set; }
    }

    public class VenueDisplayModel
    {
        public int VenueId { get; set; }
        public string? Name { get; set; }
        public int Capacity { get; set; }
    }






    public class TheatreShowDisplayModelForField
    {
        public int TheatreShowId { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public double Price { get; set; }
        // You might want to include just the dates or a simplified list
        public VenueDisplayModel? Venue { get; set; }
    }

    public class TheatreShowDateDisplayModelForField
    {
        public int TheatreShowDateId { get; set; }
        public DateTime DateAndTime { get; set; } // Format can be handled at the display layer
        // Optionally include TheatreShow details
        public int TheatreShowId { get; set; } // Just reference ID
    }

    public class CustomerDisplayModelForField
    {
        public int CustomerId { get; set; }

        public string? FirstName { get; set; }

        public string? LastName { get; set; }

        public string? Email { get; set; }
    }
}
