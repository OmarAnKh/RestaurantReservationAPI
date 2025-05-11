using System.ComponentModel.DataAnnotations;
using RestaurantReservation.Domain;

namespace RestaurantReservationAPI.Models.Restaurant;

public class RestaurantDto
{
    public int RestaurantId { get; set; }

    [Required] public string Name { get; set; }
    [Required] public string Address { get; set; }
    [Required] public string PhoneNumber { get; set; }
    public decimal OpeningHours { get; set; }

    public List<Reservation> Reservations { get; set; } = new List<Reservation>();
    public List<Employee> Employees { get; set; } = new List<Employee>();
    public List<Table> Tables { get; set; } = new List<Table>();
    public List<MenuItem> MenuItems { get; set; } = new List<MenuItem>();
}