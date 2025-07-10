using System.ComponentModel.DataAnnotations;

namespace RestaurantReservationAPI.Models.Customer;

public class CustomerCreationDto
{
    public int CustomerId { get; set; }

    [Required]
    [MaxLength(50)]
    public string FirstName { get; set; }
    [Required]
    [MaxLength(50)]
    public string LastName { get; set; }
    [Required]
    [MaxLength(255)]
    public string Email { get; set; }
    [Required] 
    [MaxLength(10)]
    public string PhoneNumber { get; set; }
}