using RestaurantReservation.Domain;

namespace RestaurantReservationAPI.Models.Customer;

public class CustomersDto
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public string PhoneNumber { get; set; }

    public IEnumerable<Reservation> Reservations { get; set; }
        = new List<Reservation>();
}