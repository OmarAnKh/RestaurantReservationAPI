namespace RestaurantReservationAPI.Models.Restaurant;

public class RestaurantWithoutRelations
{
    public int RestaurantId { get; set; }
    public string Name { get; set; }
    public string Address { get; set; }
    public string PhoneNumber { get; set; }
    public decimal OpeningHours { get; set; }
}