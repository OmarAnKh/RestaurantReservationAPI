using AutoMapper;
using RestaurantReservation.Domain;
using RestaurantReservationAPI.Models.EmployeeDto;

namespace RestaurantReservationAPI.Profiles;

public class EmployeeProfile : Profile
{
    public EmployeeProfile()
    {
        CreateMap<Employee, EmployeeDto>();
        CreateMap<EmployeeDto, Employee>();
        CreateMap<EmployeeCreationDto, Employee>();
        CreateMap<Employee, EmployeeUpdateDto>();
        CreateMap<EmployeeUpdateDto, Employee>();
    }
}