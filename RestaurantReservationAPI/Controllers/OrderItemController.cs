using System.Text.Json;
using AutoMapper;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using RestaurantReservation.Db.Repositories.Interfaces;
using RestaurantReservation.Domain;
using RestaurantReservationAPI.Models.OrderItemDto;

namespace RestaurantReservationAPI.Controllers;

/// <summary>
/// OrderItem Controller
/// </summary>
[Route("api/[controller]")]
[ApiController]
public class OrderItemController : ControllerBase
{
    private readonly IOrderItemRepository _orderItemRepository;
    private readonly IOrderRepository _orderRepository;
    private readonly IMenuItemRepository _menuItemRepository;
    private readonly IMapper _mapper;
    private const int MaxPageSize = 20;
    /// <summary>
    /// OrderItem Constructor 
    /// </summary>
    /// <param name="orderItemRepository">The OrderItem Repository you want to use</param>
    /// <param name="orderRepository">The order Repository you want to use</param>
    /// <param name="menuItemRepository">The menuItem Repository you want to use</param>
    /// <param name="mapper">Mapper to map data between Dto</param>
    public OrderItemController(IOrderItemRepository orderItemRepository, IOrderRepository orderRepository, IMenuItemRepository menuItemRepository, IMapper mapper)
    {
        _orderItemRepository = orderItemRepository;
        _orderRepository = orderRepository;
        _menuItemRepository = menuItemRepository;
        _mapper = mapper;
    }

    /// <summary>
    /// Get a list of OrderItemDto paginated into pages 
    /// </summary>
    /// <param name="pageNumber">the number of page you want (default is 1)</param>
    /// <param name="pageSize">The page size you want (default is 10 and max is 20)</param>
    /// <returns>
    /// and IEnumerable of OrderItemDto
    /// </returns>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<OrderItemDto>>> GetOrderItemsAsync(int pageNumber = 1, int pageSize = 10)
    {
        if (pageSize is < 1 or > MaxPageSize)
        {
            pageSize = MaxPageSize;
        }
        var (orderItemEntities, paginationMetaData) = await _orderItemRepository.GetAllAsync(string.Empty, string.Empty, pageNumber, pageSize);
        var orderItems = _mapper.Map<IEnumerable<OrderItemDto>>(orderItemEntities);
        Response.Headers.Append("X-Pagination", JsonSerializer.Serialize(paginationMetaData));
        return Ok(orderItems);

    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpGet("{id}")]
    public async Task<ActionResult<OrderItemDto>> GetOrderItemAsync(int id)
    {
        var orderItem = await _orderItemRepository.GetByIdAsync(id);
        if (orderItem == null)
        {
            return NotFound();
        }
        return Ok(_mapper.Map<OrderItemDto>(orderItem));
    }
    /// <summary>
    /// Create OrderItem
    /// </summary>
    /// <param name="orderItem">the order item info</param>
    /// <returns></returns>
    [HttpPost]
    public async Task<ActionResult<OrderItemDto>> PostOrderItemAsync(OrderItemCreationDto orderItem)
    {
        var orderItemEntity = _mapper.Map<OrderItem>(orderItem);
        var menuItem = await _menuItemRepository.GetByIdAsync(orderItemEntity.MenuItemId);
        if (menuItem == null)
        {
            return NotFound();
        }
        var order = await _orderRepository.GetByIdAsync(orderItemEntity.OrderId);
        if (order == null)
        {
            return NotFound();
        }
        await _orderItemRepository.AddAsync(orderItemEntity);
        return Ok(_mapper.Map<OrderItemDto>(orderItem));
    }
    /// <summary>
    /// Update OrderItem
    /// </summary>
    /// <param name="id">the order item id you want to update</param>
    /// <param name="patchDocument">the new values you want</param>
    /// <returns></returns>
    [HttpPatch("{id}")]
    public async Task<ActionResult> PatchOrderItemAsync(int id, JsonPatchDocument<OrderItemUpdateDto> patchDocument)
    {
        var orderItem = await _orderItemRepository.GetByIdAsync(id);
        if (orderItem is null)
        {
            return NotFound();
        }
        var orderItemUpdateDto = _mapper.Map<OrderItemUpdateDto>(orderItem);
        patchDocument.ApplyTo(orderItemUpdateDto, ModelState);
        if (!TryValidateModel(orderItemUpdateDto))
        {
            return BadRequest();
        }
        if (!ModelState.IsValid)
        {
            return BadRequest();
        }
        _mapper.Map(orderItemUpdateDto, orderItem);
        await _orderItemRepository.SaveChangesAsync();
        return NoContent();
    }
    /// <summary>
    /// Delete order item
    /// </summary>
    /// <param name="id">the id of the order item you want to delete</param>
    /// <returns></returns>
    [HttpDelete("{id}")]
    public async Task<ActionResult<OrderItemDto>> DeleteOrderItemAsync(int id)
    {
        var orderItem = await _orderItemRepository.GetByIdAsync(id);
        if (orderItem is null)
        {
            return NotFound();
        }
        await _orderItemRepository.DeleteAsync(id);
        return NoContent();
    }
}