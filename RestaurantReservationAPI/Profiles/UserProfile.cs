using AutoMapper;
using RestaurantReservation.Domain;
using RestaurantReservationAPI.Models.UserDto;

namespace RestaurantReservationAPI.Profiles;

public class UserProfile : Profile
{

    public UserProfile()
    {
        CreateMap<UserLoginDto, User>();
        CreateMap<User, UserLoginDto>();
        CreateMap<UserRegisterDto, User>();
        CreateMap<User, UserRegisterDto>();
    }
}