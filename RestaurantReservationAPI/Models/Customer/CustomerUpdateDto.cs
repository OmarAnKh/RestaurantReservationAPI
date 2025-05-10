using System.ComponentModel.DataAnnotations;

namespace RestaurantReservationAPI.Models.Customer
{
    public class CustomerUpdateDto
    {
        [MaxLength(50)]
        public string FirstName { get; set; } = string.Empty;
        [MaxLength(50)]
        public string LastName { get; set; } = string.Empty;
    }
}