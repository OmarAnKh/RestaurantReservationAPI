using AutoMapper;
using RestaurantReservation.Domain;
using RestaurantReservationAPI.Models.Restaurant;

namespace RestaurantReservationAPI.Profiles;

public class RestaurantProfile:Profile
{
    public RestaurantProfile()
    {
        CreateMap<Restaurant, RestaurantWithoutRelations>();
        CreateMap<Restaurant, RestaurantDto>();
        CreateMap<RestaurantDto, Restaurant>();
        CreateMap<Restaurant, RestaurantCreationDto>();
        CreateMap<RestaurantCreationDto, Restaurant>();
        CreateMap<Restaurant, RestaurantUpdateDto>();
        CreateMap<RestaurantUpdateDto, Restaurant>();
    }
}