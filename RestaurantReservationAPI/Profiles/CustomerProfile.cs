using AutoMapper;
using RestaurantReservation.Domain;
using RestaurantReservationAPI.Models.Customer;

namespace RestaurantReservationAPI.Profiles;

public class CustomerProfile : Profile
{
    public CustomerProfile()
    {
        CreateMap<CustomerCreationDto, Customer>();
        CreateMap<Customer,CustomerWithoutReservationsDto>();
        CreateMap<Customer, CustomerUpdateDto>();
        CreateMap<CustomerUpdateDto, Customer>();
    }
}