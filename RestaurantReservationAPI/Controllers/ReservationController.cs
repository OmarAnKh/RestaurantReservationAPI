using System.Text.Json;
using AutoMapper;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using RestaurantReservation.Db.Repositories.Interfaces;
using RestaurantReservation.Domain;
using RestaurantReservationAPI.Models.ReservationDto;

namespace RestaurantReservationAPI.Controllers;

/// <summary>
/// The reservation controller
/// </summary>
[Route("api/[controller]")]
[ApiController]
public class ReservationController : ControllerBase
{
    private readonly IReservationRepository _reservationRepository;
    private readonly IRestaurantRepository _restaurantRepository;
    private readonly ICustomerRepository _customerRepository;
    private readonly IMapper _mapper;
    private const int _maxPageSize = 20;
    /// <summary>
    /// the reservation controller constructor
    /// </summary>
    /// <param name="reservationRepository">The reservation repository you want to use (it takes an interface) </param>
    /// <param name="mapper"> IMapper to make the Dtos</param>
    /// <param name="restaurantRepository">The restaurant repository you want to use</param>
    /// <param name="customerRepository">The customer repository you want to use </param>
    public ReservationController(IReservationRepository reservationRepository, IMapper mapper, IRestaurantRepository restaurantRepository, ICustomerRepository customerRepository)
    {
        _reservationRepository = reservationRepository;
        _mapper = mapper;
        _restaurantRepository = restaurantRepository;
        _customerRepository = customerRepository;
    }
    /// <summary>
    /// Get Reservations paginated
    /// </summary>
    /// <param name="pageNumber">the number of the page you want (default is 1)</param>
    /// <param name="pageSize">the page size you want (default is 10 and max is 20)</param>
    /// <returns>an IEnumerable of ReservationDto </returns>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ReservationDto>>> GetReservations(int pageNumber = 1, int pageSize = 10)
    {
        if (pageSize is > _maxPageSize or < 1)
        {
            pageSize = _maxPageSize;
        }
        var (reservationEntities, paginationMetaData) = await _reservationRepository.GetAllAsync(string.Empty, string.Empty, pageNumber, pageSize);
        Response.Headers.Append("X-Pagination",
            JsonSerializer.Serialize(paginationMetaData));
        return Ok(_mapper.Map<IEnumerable<ReservationDto>>(reservationEntities));
    }
    /// <summary>
    /// Get a specific reservation via id
    /// </summary>
    /// <param name="id"> the id of the reservation you want </param>
    /// <returns>
    /// not found if there is no reservation with the provided id
    /// A reservationDto if the reservation is found
    /// </returns>
    [HttpGet("{id}")]
    public async Task<ActionResult<ReservationDto>> GetReservation(int id)
    {
        var reservationEntity = await _reservationRepository.GetByIdAsync(id);
        if (reservationEntity == null)
        {
            return NotFound();
        }
        return Ok(_mapper.Map<ReservationDto>(reservationEntity));
    }
    /// <summary>
    /// Create a reservation 
    /// </summary>
    /// <param name="reservationDto">The reservation info</param>
    /// <returns>
    /// not found if the restaurant or the customer is not found in the database
    /// a bad request if the party size is less than one
    /// the reservationDto if succeed 
    /// </returns>
    [HttpPost]
    public async Task<ActionResult<ReservationDto>> PostReservation(ReservationCreationDto reservationDto)
    {
        var restaurant = await _restaurantRepository.GetByIdAsync(reservationDto.RestaurantId);
        if (restaurant == null)
        {
            return NotFound();
        }
        var customer = await _customerRepository.GetByIdAsync(reservationDto.CustomerId);
        if (customer == null)
        {
            return NotFound();
        }
        if (reservationDto.PartySize < 1)
        {
            return BadRequest();
        }
        var reservationEntity = _mapper.Map<Reservation>(reservationDto);
        await _reservationRepository.AddAsync(reservationEntity);
        return Ok(_mapper.Map<ReservationDto>(reservationEntity));
    }
    
    /// <summary>
    /// Update reservation  
    /// </summary>
    /// <param name="id">the id of the reservation you want to update</param>
    /// <param name="patchDocument">The new reservation info</param>
    /// <returns></returns>
    [HttpPatch("{id}")]
    public async Task<ActionResult> PatchReservation(int id, JsonPatchDocument<ReservationUpdateDto> patchDocument)
    {
        var reservationEntity = await _reservationRepository.GetByIdAsync(id);
        if (reservationEntity == null)
        {
            return NotFound();
        }
        var reservationUpdatedEntity = _mapper.Map<ReservationUpdateDto>(reservationEntity);
        patchDocument.ApplyTo(reservationUpdatedEntity, ModelState);
        if (!TryValidateModel(reservationUpdatedEntity))
        {
            return BadRequest();
        }
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        _mapper.Map(reservationUpdatedEntity, reservationEntity);
        await _reservationRepository.SaveChangesAsync();
        return NoContent();
    }

    /// <summary>
    /// Delete a reservation
    /// </summary>
    /// <param name="id">The id of the reservation you want to update</param>
    /// <returns></returns>
    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteReservation(int id)
    {
        var reservationEntity = await _reservationRepository.GetByIdAsync(id);
        if (reservationEntity == null)
        {
            return NotFound();
        }
        await _reservationRepository.DeleteAsync(id);
        return NoContent();
    }
}