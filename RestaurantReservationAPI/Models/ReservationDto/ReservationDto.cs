using System.ComponentModel.DataAnnotations;
using RestaurantReservation.Domain;

namespace RestaurantReservationAPI.Models.ReservationDto;

public class ReservationDto
{
    public int ReservationId { get; set; }
    public int RestaurantId { get; set; }
    public int CustomerId { get; set; }
    public IEnumerable<ReservationTable> ReservationTables { get; set; } = new List<ReservationTable>();
    public DateTime ReservationDate { get; set; }
    public int PartySize { get; set; }

    public IEnumerable<Order> Orders { get; set; } = new List<Order>();
}