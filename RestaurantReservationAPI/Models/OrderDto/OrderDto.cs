namespace RestaurantReservationAPI.Models.OrderDto;

public class OrderDto
{
    public int OrderId { get; set; }
    public int ReservationId { get; set; }
    public int EmployeeId { get; set; }
    public DateTime OrderDate { get; set; }
    public int TotalAmount { get; set; }
    public IEnumerable<OrderItemDto.OrderItemDto> OrderItems { get; set; } = new List<OrderItemDto.OrderItemDto>();
}