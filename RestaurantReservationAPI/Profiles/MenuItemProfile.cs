using AutoMapper;
using RestaurantReservation.Domain;
using RestaurantReservationAPI.Models;
using RestaurantReservationAPI.Models.MenuItemDto;

namespace RestaurantReservationAPI.Profiles;

public class MenuItemProfile : Profile
{
    public MenuItemProfile()
    {
        CreateMap<MenuItem, MenuItemDto>();
        CreateMap<MenuItemDto, MenuItem>();
        CreateMap<MenuItemCreationDto, MenuItem>();
        CreateMap<MenuItemUpdateDto, MenuItem>();
        CreateMap<MenuItem, MenuItemUpdateDto>();
    }
}