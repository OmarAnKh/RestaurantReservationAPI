using System.ComponentModel.DataAnnotations;

namespace RestaurantReservationAPI.Models.ReservationDto;

public class ReservationCreationDto
{
    public int RestaurantId { get; set; }
    public int CustomerId { get; set; }
    public DateTime ReservationDate { get; set; } = DateTime.Now;
    [Range(1, int.MaxValue, ErrorMessage = "Party size must be greater than zero.")]
    public int PartySize { get; set; }
}