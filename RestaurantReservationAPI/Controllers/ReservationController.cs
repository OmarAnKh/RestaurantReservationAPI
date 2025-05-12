using System.Text.Json;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using RestaurantReservation.Db.Repositories.Interfaces;
using RestaurantReservation.Domain;
using RestaurantReservationAPI.Models.MenuItemDto;
using RestaurantReservationAPI.Models.OrderDto;
using RestaurantReservationAPI.Models.ReservationDto;

namespace RestaurantReservationAPI.Controllers;

/// <summary>
/// The reservation controller
/// </summary>
[Route("api/[controller]")]
[ApiController]
[Authorize]

public class ReservationController : ControllerBase
{
    private readonly IReservationRepository _reservationRepository;
    private readonly IRestaurantRepository _restaurantRepository;
    private readonly ICustomerRepository _customerRepository;
    private readonly IOrderRepository _orderRepository;
    private readonly IMapper _mapper;
    private const int MaxPageSize = 20;
    /// <summary>
    /// the reservation controller constructor
    /// </summary>
    /// <param name="reservationRepository">The reservation repository you want to use (it takes an interface) </param>
    /// <param name="mapper"> IMapper to make the Dtos</param>
    /// <param name="restaurantRepository">The restaurant repository you want to use</param>
    /// <param name="customerRepository">The customer repository you want to use </param>
    public ReservationController(IReservationRepository reservationRepository, IMapper mapper, IRestaurantRepository restaurantRepository, ICustomerRepository customerRepository, IOrderRepository orderRepository)
    {
        _reservationRepository = reservationRepository;
        _mapper = mapper;
        _restaurantRepository = restaurantRepository;
        _customerRepository = customerRepository;
        _orderRepository = orderRepository;
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
        if (pageSize is > MaxPageSize or < 1)
        {
            pageSize = MaxPageSize;
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


    /// <summary>
    /// Get the reservation fo a customer
    /// </summary>
    /// <param name="customerId">the customer id</param>
    /// <returns></returns>
    [HttpGet("customer/{customerId}")]
    public async Task<IActionResult> GetReservationsByCustomer(int customerId)
    {
        var reservations = await _reservationRepository.GetReservationsByCustomerAsync(customerId);

        if (reservations == null || reservations.Count == 0)
        {
            return NotFound($"No reservations found for customer with ID {customerId}.");
        }

        return Ok(_mapper.Map<IEnumerable<ReservationDto>>(reservations));
    }


    /// <summary>
    /// Retrieves all reservations along with their associated customer and restaurant details.
    /// </summary>
    /// <returns>A list of reservations including customer and restaurant information.</returns>
    /// <response code="200">Returns the list of reservations.</response>
    [HttpGet("with-customer-and-restaurant")]
    public async Task<ActionResult<List<ReservationsWithCustomerAndRestaurant>>> GetReservationsWithCustomerAndRestaurant()
    {
        var result = await _reservationRepository.ListReservationsWithCustomerAsync();
        return Ok(result);
    }

    /// <summary>
    /// Retrieves a list of customers who had reservations with a party size greater than the specified value.
    /// </summary>
    /// <param name="partySize">The minimum party size to filter customers.</param>
    /// <returns>A list of customers with reservations larger than the given party size.</returns>
    /// <response code="200">Returns the list of customers.</response>
    /// <response code="400">If the party size is less than or equal to zero.</response>
    [HttpGet("customers-by-party-size/{partySize}")]
    public async Task<ActionResult<List<ReservationCustomerDto>>> GetReservationCustomersByPartySize(int partySize)
    {
        if (partySize <= 0)
        {
            return BadRequest("Party size must be greater than zero.");
        }

        var result = await _reservationRepository.ListReservationCustomersAsync(partySize);
        return Ok(result);
    }
    /// <summary>
    /// The orders with their menu items for a specific reservation
    /// </summary>
    /// <param name="reservationId">The id of the reservation you want</param>
    /// <returns></returns>
    [HttpGet("{reservationId}/orders")]
    public async Task<ActionResult<IEnumerable<OrderDto>>> GetReservationOrders(int reservationId)
    {
        var OrdersAndMenuItem = await _orderRepository.ListOrdersAndMenuItemsAsync(reservationId);
        return Ok(_mapper.Map<IEnumerable<OrderDto>>(OrdersAndMenuItem));
    }
    /// <summary>
    /// get the menu items for a specific reservation
    /// </summary>
    /// <param name="reservationId">the id for the reservation you want</param>
    /// <returns></returns>
    [HttpGet("{reservationId}/menu-items")]
    public async Task<ActionResult<IEnumerable<MenuItemDto>>> GetReservationMenuItems(int reservationId)
    {
        var menuItem = await _orderRepository.ListOrderedMenuItemsAsync(reservationId);
        return Ok(_mapper.Map<IEnumerable<MenuItemDto>>(menuItem));
    }
}