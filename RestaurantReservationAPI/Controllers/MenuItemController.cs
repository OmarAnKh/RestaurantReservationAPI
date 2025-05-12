using System.Text.Json;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using RestaurantReservation.Db.Repositories.Interfaces;
using RestaurantReservation.Domain;
using RestaurantReservationAPI.Models.MenuItemDto;

namespace RestaurantReservationAPI.Controllers;

/// <summary>
/// MenuItem controller
/// </summary>
[Route("api/[controller]")]
[ApiController]
[Authorize]

public class MenuItemController : ControllerBase
{
    private readonly IMenuItemRepository _menuItemRepository;
    private readonly IRestaurantRepository _restaurantRepository;
    private readonly IMapper _mapper;
    private const int MaxPageSize = 20;
    /// <summary>
    /// Menu item controller constructor
    /// </summary>
    /// <param name="menuItemRepository">The MenuItem repository you want to use (you can implement it your self since its an interface</param>
    /// <param name="restaurantRepository">The restaurant repository you want to use (you can implement it your self since its an interface</param>
    /// <param name="mapper">the mapper to map the data between Dtos</param>
    public MenuItemController(IMenuItemRepository menuItemRepository, IRestaurantRepository restaurantRepository, IMapper mapper)
    {
        _menuItemRepository = menuItemRepository;
        _restaurantRepository = restaurantRepository;
        _mapper = mapper;
    }
    /// <summary>
    /// Get MenuItem Paginated into pages and search for an item by name or search by search query
    /// </summary>
    /// <param name="name">to search for an item using its name only</param>
    /// <param name="searchQuery">to search for an item</param>
    /// <param name="pageNumber">the page number you want (default is 1)</param>
    /// <param name="pageSize">the page size you want (default is 10 and max is 20)</param>
    /// <returns>
    /// A IEnumerable of MenuItemDto
    /// </returns>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<MenuItemDto>>> GetMenuItems(string? name, string? searchQuery, int pageNumber = 1, int pageSize = 10)
    {
        if (pageSize is < 1 or > MaxPageSize)
        {
            pageSize = MaxPageSize;
        }
        var (menuItemEntities, paginationMetaData) = await _menuItemRepository.GetAllAsync(name, searchQuery, pageNumber, pageSize);
        Response.Headers.Append("X-Pagination", JsonSerializer.Serialize(paginationMetaData));
        return Ok(_mapper.Map<IEnumerable<MenuItemDto>>(menuItemEntities));
    }
    /// <summary>
    /// Get a specific item using MenuItem id
    /// </summary>
    /// <param name="id">The id of the Item you want to search for </param>
    /// <returns>A MenuItem if exists</returns>
    [HttpGet("{id}")]
    public async Task<ActionResult<MenuItemDto>> GetMenuItem(int id)
    {
        var menuItem = await _menuItemRepository.GetByIdAsync(id);
        if (menuItem == null)
        {
            return NotFound();
        }
        return Ok(_mapper.Map<MenuItemDto>(menuItem));
    }
    /// <summary>
    /// Create a MenuItem
    /// </summary>
    /// <param name="menuItemCreationDto">The Item info you want to add</param>
    /// <returns>
    /// not found if couldn't find the restaurant
    /// a menuItemDto if succeed
    /// </returns>
    [HttpPost]
    public async Task<ActionResult<MenuItemDto>> PostMenuItem(MenuItemCreationDto menuItemCreationDto)
    {
        var restaurant = await _restaurantRepository.GetByIdAsync(menuItemCreationDto.RestaurantId);
        if (restaurant == null)
        {
            return NotFound();
        }
        var menuItemEntity = _mapper.Map<MenuItem>(menuItemCreationDto);
        await _menuItemRepository.AddAsync(menuItemEntity);
        return Ok(_mapper.Map<MenuItemDto>(menuItemEntity));
    }
    /// <summary>
    /// Update a menu item
    /// </summary>
    /// <param name="id">the id of the menu item you want to update</param>
    /// <param name="patchDocument">the new values you want to save</param>
    /// <returns></returns>
    [HttpPatch("{id}")]
    public async Task<ActionResult> PatchMenuItem(int id, JsonPatchDocument<MenuItemUpdateDto> patchDocument)
    {
        var menuItem = await _menuItemRepository.GetByIdAsync(id);
        if (menuItem == null)
        {
            return NotFound();
        }
        var menuItemToPatch = _mapper.Map<MenuItemUpdateDto>(menuItem);
        patchDocument.ApplyTo(menuItemToPatch, ModelState);
        if (!TryValidateModel(menuItemToPatch))
        {
            return BadRequest();
        }
        if (!ModelState.IsValid)
        {
            return BadRequest();
        }
        _mapper.Map(menuItemToPatch, menuItem);
        await _menuItemRepository.SaveChangesAsync();
        return NoContent();
    }
    /// <summary>
    /// Delete A menu item
    /// </summary>
    /// <param name="id"> The id of the item you want to delete</param>
    /// <returns>
    /// not found when could not find the Item   
    /// no content if succeed
    /// </returns>
    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteMenuItem(int id)
    {
        var menuItem = await _menuItemRepository.GetByIdAsync(id);
        if (menuItem == null)
        {
            return NotFound();
        }
        await _menuItemRepository.DeleteAsync(id);
        return NoContent();
    }

}