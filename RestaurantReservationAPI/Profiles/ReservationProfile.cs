using AutoMapper;
using RestaurantReservation.Domain;
using RestaurantReservationAPI.Models.ReservationDto;

namespace RestaurantReservationAPI.Profiles;

public class ReservationProfile : Profile
{
    public ReservationProfile()
    {
        CreateMap<Reservation, ReservationDto>();
        CreateMap<ReservationDto, Reservation>();
        CreateMap<ReservationCreationDto, Reservation>();
        CreateMap<Reservation, ReservationUpdateDto>();
        CreateMap<ReservationUpdateDto, Reservation>();
    }

}