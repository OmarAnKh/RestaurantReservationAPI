using System.ComponentModel.DataAnnotations;

namespace RestaurantReservationAPI.Models.EmployeeDto;

public class EmployeeCreationDto
{
    [Required] public int RestaurantId { get; set; }
    [Required] [MaxLength(20)] public string FirstName { get; set; }
    [Required] [MaxLength(20)] public string LastName { get; set; }
    [Required] [MaxLength(20)] public string Position { get; set; }
}