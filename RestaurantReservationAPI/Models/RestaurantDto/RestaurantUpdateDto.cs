namespace RestaurantReservationAPI.Models.Restaurant;

public class RestaurantUpdateDto
{
    public string PhoneNumber { get; set; }
    public string Name { get; set; }
    public decimal OpeningHours { get; set; }
}