namespace RestaurantReservationAPI.Models.TableDto;
using RestaurantReservation.Domain;

public class TableDto
{
    public int TableId { get; set; }

    public int RestaurantId { get; set; }
    public int Capacity { get; set; }
    public IEnumerable<ReservationTable> ReservationTables { get; set; } = new List<ReservationTable>();
}