using System.Text.Json;
using AutoMapper;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using RestaurantReservation.Db.Repositories.Interfaces;
using RestaurantReservation.Domain;
using RestaurantReservationAPI.Models.OrderDto;

namespace RestaurantReservationAPI.Controllers;

/// <summary>
/// Orders controller
/// </summary>
[Route("api/[controller]")]
[ApiController]
public class OrderController : ControllerBase
{
    private readonly IOrderRepository _orderRepository;
    private readonly IReservationRepository _reservationRepository;
    private readonly IEmployeeRepository _employeeRepository;
    private readonly IMapper _mapper;
    private const int MaxPageSize = 20;
    /// <summary>
    /// Order controller constructor 
    /// </summary>
    /// <param name="orderRepository">The orderRepository you want to use </param>
    /// <param name="reservationRepository">The reservationRepository you want to use </param>
    /// <param name="employeeRepository">The employeeRepository you want to use </param>
    /// <param name="mapper">mapper to map the data between Dtos</param>
    public OrderController(IOrderRepository orderRepository, IReservationRepository reservationRepository, IEmployeeRepository employeeRepository, IMapper mapper)
    {
        _orderRepository = orderRepository;
        _reservationRepository = reservationRepository;
        _employeeRepository = employeeRepository;
        _mapper = mapper;
    }
    /// <summary>
    /// Get a list of orders if you want you can get it for a specific date
    /// </summary>
    /// <param name="orderData">The data of the orders you want (Optional)</param>
    /// <param name="pageNumber">the page number you want (default is 1)</param>
    /// <param name="pageSize">The Page size you want (default is 10 and max is 20)</param>
    /// <returns>
    /// IEnumerable of OrderDto
    /// </returns>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<OrderDto>>> GetOrders(DateTime? orderData, int pageNumber = 1, int pageSize = 10)
    {
        if (pageSize is < 1 or > MaxPageSize)
        {
            pageSize = MaxPageSize;
        }
        var (orderEntities, paginationMetaData) = await _orderRepository.GetAllAsync(orderData.ToString(), string.Empty, pageNumber, pageSize);
        var orders = _mapper.Map<IEnumerable<OrderDto>>(orderEntities);
        Response.Headers.Append("X-pagination", JsonSerializer.Serialize(paginationMetaData));
        return Ok(orders);
    }
    /// <summary>
    /// Get a specific order using the order id
    /// </summary>
    /// <param name="id">The id for the order you want</param>
    /// <returns>
    /// not found, when the order is not found
    /// OrderDto, When the order is found
    /// </returns>
    [HttpGet("{id}")]
    public async Task<ActionResult<OrderDto>> GetOrder(int id)
    {
        var order = await _orderRepository.GetByIdAsync(id);
        if (order == null)
        {
            return NotFound();
        }
        return Ok(_mapper.Map<OrderDto>(order));
    }
    
    /// <summary>
    /// Create an order
    /// </summary>
    /// <param name="order"> the Order info</param>
    /// <returns>
    /// not found, when the reservation or the employee is not found
    /// OrderDto, when succeed
    /// </returns>
    [HttpPost]
    public async Task<ActionResult<OrderDto>> PostOrder(OrderCreationDto order)
    {
        var reservation = await _reservationRepository.GetByIdAsync(order.ReservationId);
        if (reservation == null)
        {
            return NotFound();
        }
        var employee = await _employeeRepository.GetByIdAsync(order.EmployeeId);
        if (employee == null)
        {
            return NotFound();
        }
        await _orderRepository.AddAsync(_mapper.Map<Order>(order));
        return Ok(order);
    }
    
    /// <summary>
    /// Update order 
    /// </summary>
    /// <param name="id">the id of the order you want to update</param>
    /// <param name="patchDoc">the new values</param>
    /// <returns></returns>
    [HttpPatch("{id}")]
    public async Task<ActionResult> PatchOrder(int id,JsonPatchDocument<OrderUpdateDto> patchDoc)
    {
        var order = await _orderRepository.GetByIdAsync(id);
        if (order == null)
        {
            return NotFound();
        }
        var orderUpdateDto = _mapper.Map<OrderUpdateDto>(order);
        patchDoc.ApplyTo(orderUpdateDto, ModelState);
        if (!TryValidateModel(orderUpdateDto))
        {
            return BadRequest();
        }
        if (!ModelState.IsValid)
        {
            return BadRequest();
        }
        _mapper.Map(orderUpdateDto, order);
        await _orderRepository.SaveChangesAsync();
        return NoContent();
    }
    
    /// <summary>
    /// Delete an order
    /// </summary>
    /// <param name="id">the order id you want to delete</param>
    /// <returns></returns>
    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteOrder(int id)
    {
        var order = await _orderRepository.GetByIdAsync(id);
        if (order is null)
        {
            return NotFound();
        }
        await _orderRepository.DeleteAsync(id);
        return NoContent();
    }

}