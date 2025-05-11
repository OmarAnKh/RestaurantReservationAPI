namespace RestaurantReservationAPI.Models.TableDto;

public class TableWithoutRelationsDto
{
    public int TableId { get; set; }
    public int Capacity { get; set; }
    public int RestaurantId { get; set; }
}