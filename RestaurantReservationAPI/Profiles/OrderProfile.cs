using AutoMapper;
using RestaurantReservation.Domain;
using RestaurantReservationAPI.Models.OrderDto;

namespace RestaurantReservationAPI.Profiles;

public class OrderProfile : Profile
{
    public OrderProfile()
    {
        CreateMap<Order, OrderDto>();
        CreateMap<OrderDto, Order>();
        CreateMap<OrderCreationDto, Order>();
        CreateMap<Order, OrderCreationDto>();
        CreateMap<Order, OrderUpdateDto>();
        CreateMap<OrderUpdateDto, Order>();
    }
}