using System.ComponentModel.DataAnnotations;

namespace RestaurantReservationAPI.Models.MenuItemDto;

public class MenuItemUpdateDto
{
    [Required]
    [MaxLength(50)]
    public string Name { get; set; }
    [Required]
    [MaxLength(200)]
    public string Description { get; set; }
    public decimal Price { get; set; }
}