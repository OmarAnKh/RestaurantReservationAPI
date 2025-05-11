using System.Text.Json;
using AutoMapper;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using RestaurantReservation.Db.Repositories.Interfaces;
using RestaurantReservation.Domain;
using RestaurantReservationAPI.Models.Restaurant;
using RestaurantReservationAPI.Services;

namespace RestaurantReservationAPI.Controllers;

/// <summary>
///     Restaurant Controller
/// </summary>
[Route("api/[controller]")]
[ApiController]
public class RestaurantController : ControllerBase
{
    private readonly IRestaurantRepository _restaurantRepository;
    private readonly IMapper _mapper;
    private readonly int _maxPageSize = 20;


    /// <summary>
    ///     Restaurant Controller constructor
    /// </summary>
    /// <param name="restaurantRepository"></param>
    /// <param name="mapper"></param>
    public RestaurantController(IRestaurantRepository restaurantRepository, IMapper mapper)
    {
        _restaurantRepository = restaurantRepository;
        _mapper = mapper;
    }
    /// <summary>
    ///     Return list of restaurants you can return all the restaurants or search
    /// </summary>
    /// <param name="name"> a name of the restaurant you want (optional)</param>
    /// <param name="searchQuery">something to search for like the address or phone number (optional)</param>
    /// <param name="pageNumber">the page number you want (default is 1)</param>
    /// <param name="pageSize">the page size you want (default is 10 and max is 20)</param>
    /// <returns></returns>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<RestaurantWithoutRelations>>> GetRestaurants(string? name, string? searchQuery, int pageNumber = 1, int pageSize = 10)
    {
        if (pageSize > _maxPageSize)
        {
            pageSize = _maxPageSize;
        }
        (IEnumerable<Restaurant>? restaurants, PaginationMetaData? paginationMetaData) = await _restaurantRepository.GetAllAsync(name, searchQuery, pageNumber, pageSize);
        Response.Headers.Append("X-Pagination",
            JsonSerializer.Serialize(paginationMetaData));
        return Ok(_mapper.Map<IEnumerable<RestaurantWithoutRelations>>(restaurants));
    }
    /// <summary>
    ///     Get a restaurant via id
    /// </summary>
    /// <param name="id">The id of the restaurant you want</param>
    /// <returns>The Restaurant</returns>
    [HttpGet("{id}")]
    public async Task<ActionResult<Restaurant>> GetRestaurant(int id)
    {
        Restaurant? restaurant = await _restaurantRepository.GetByIdAsync(id);
        if (restaurant == null)
        {
            return NotFound();
        }
        return Ok(_mapper.Map<RestaurantDto>(restaurant));
    }

    /// <summary>
    ///     Create Restaurant
    /// </summary>
    /// <param name="restaurant">The restaurant info that you want to add</param>
    /// <returns></returns>
    [HttpPost]
    public async Task<ActionResult<Restaurant>> PostRestaurant(RestaurantCreationDto restaurant)
    {
        Restaurant? restaurantEntity = _mapper.Map<Restaurant>(restaurant);
        await _restaurantRepository.AddAsync(restaurantEntity);
        RestaurantDto? restaurantDto = _mapper.Map<RestaurantDto>(restaurantEntity);
        return CreatedAtAction("GetRestaurant", new { id = restaurantDto.RestaurantId }, restaurantDto);
    }
    /// <summary>
    ///     Update a restaurant info
    /// </summary>
    /// <param name="id">The id of the restaurant</param>
    /// <param name="patchDocument">The new values</param>
    /// <returns></returns>
    [HttpPatch("{id}")]
    public async Task<ActionResult> PatchRestaurant(int id, JsonPatchDocument<RestaurantUpdateDto> patchDocument)
    {
        Restaurant? restaurant = await _restaurantRepository.GetByIdAsync(id);
        if (restaurant == null)
        {
            return NotFound();
        }
        RestaurantUpdateDto updateRestaurantDto = _mapper.Map<RestaurantUpdateDto>(restaurant);
        patchDocument.ApplyTo(updateRestaurantDto, ModelState);
        if (!ModelState.IsValid)
        {
            return BadRequest();
        }
        if (!TryValidateModel(updateRestaurantDto))
        {
            return BadRequest();
        }
        _mapper.Map(updateRestaurantDto, restaurant);
        await _restaurantRepository.SaveChangesAsync();
        return NoContent();
    }
    /// <summary>
    /// Delete a restaurant
    /// </summary>
    /// <param name="id"> the id of the restaurant you want to delete</param>
    /// not found when could not find the restaurant   
    /// no content if succeed
    /// <returns></returns>
    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteRestaurant(int id)
    {
        Restaurant? restaurant = await _restaurantRepository.GetByIdAsync(id);
        if (restaurant == null)
        {
            return NotFound();
        }
        await _restaurantRepository.DeleteAsync(id);
        return NoContent();
    }
}