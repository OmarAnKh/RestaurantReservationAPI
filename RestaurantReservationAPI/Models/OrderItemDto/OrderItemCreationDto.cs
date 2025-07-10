namespace RestaurantReservationAPI.Models.OrderItemDto;

public class OrderItemCreationDto
{
    public int OrderId { get; set; }
    public int MenuItemId { get; set; }
    public int Quantity { get; set; }
}