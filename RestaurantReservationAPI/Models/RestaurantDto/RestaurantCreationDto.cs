using System.ComponentModel.DataAnnotations;

namespace RestaurantReservationAPI.Models.Restaurant;

public class RestaurantCreationDto
{
    [Required] [MaxLength(50)] public string Name { get; set; }

    [Required] [MaxLength(100)] public string Address { get; set; }

    [Required] [MaxLength(15)] public string PhoneNumber { get; set; }

    public decimal OpeningHours { get; set; }
}