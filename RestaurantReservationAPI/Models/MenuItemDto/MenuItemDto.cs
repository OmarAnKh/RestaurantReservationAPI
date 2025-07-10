using System.ComponentModel.DataAnnotations;

namespace RestaurantReservationAPI.Models.MenuItemDto;

public class MenuItemDto
{
    public int MenuItemId { get; set; }
    public int RestaurantId { get; set; }
    [Required] [MaxLength(50)] public string Name { get; set; }
    [Required] [MaxLength(200)] public string Description { get; set; }
    public decimal Price { get; set; }
}