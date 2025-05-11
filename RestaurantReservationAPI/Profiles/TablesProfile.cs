using AutoMapper;
using RestaurantReservation.Domain;
using RestaurantReservationAPI.Models.TableDto;

namespace RestaurantReservationAPI.Profiles;

public class TablesProfile : Profile
{
    public TablesProfile()
    {
        CreateMap<Table, TableWithoutRelationsDto>();
        CreateMap<TableCreationDto, Table>();
        CreateMap<TableDto, Table>();
        CreateMap<Table, TableDto>();
        CreateMap<TableUpdateDto, Table>();
        CreateMap<Table, TableUpdateDto>();
    }
}