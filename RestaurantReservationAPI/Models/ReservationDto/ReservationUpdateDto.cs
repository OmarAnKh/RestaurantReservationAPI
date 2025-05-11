using System.ComponentModel.DataAnnotations;

namespace RestaurantReservationAPI.Models.ReservationDto;

public class ReservationUpdateDto
{
    public DateTime ReservationDate { get; set; }
    [Range(1, int.MaxValue, ErrorMessage = "Party size must be greater than zero.")]
    public int PartySize { get; set; }    
}