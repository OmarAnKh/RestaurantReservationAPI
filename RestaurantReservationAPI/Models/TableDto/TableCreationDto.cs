using System.ComponentModel.DataAnnotations;

namespace RestaurantReservationAPI.Models.TableDto;

public class TableCreationDto
{

    [Required]
    public int RestaurantId { get; set; }
    [Required]
    public int Capacity { get; set; }
}