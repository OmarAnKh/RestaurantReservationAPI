using System.ComponentModel.DataAnnotations;
using RestaurantReservation.Domain;

namespace RestaurantReservationAPI.Models.OrderDto;

public class OrderCreationDto
{
    public int ReservationId { get; set; }
    public int EmployeeId { get; set; }
    public DateTime OrderDate { get; set; } = DateTime.Now;
    public int TotalAmount { get; set; }
}