using AutoMapper;
using RestaurantReservation.Domain;
using RestaurantReservationAPI.Models.OrderItemDto;

namespace RestaurantReservationAPI.Profiles;

public class OrderItemProfile : Profile
{
    public OrderItemProfile()
    {
        CreateMap<OrderItem, OrderItemDto>();
        CreateMap<OrderItemCreationDto, OrderItem>();
        CreateMap<OrderItemUpdateDto, OrderItem>();
        CreateMap<OrderItem, OrderItemUpdateDto>();
        CreateMap<OrderItemCreationDto,OrderItemDto>();
    }
}