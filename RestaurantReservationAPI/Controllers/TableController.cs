using System.Text.Json;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using RestaurantReservation.Db.Repositories.Interfaces;
using RestaurantReservation.Domain;
using RestaurantReservationAPI.Models.TableDto;

namespace RestaurantReservationAPI.Controllers;

/// <summary>
/// Table Controller with their restaurants
/// </summary>
[Route("api/[controller]")]
[ApiController]
[Authorize]
public class TableController : ControllerBase
{
    private readonly IMapper _mapper;
    private readonly ITableRepository _tableRepository;
    private readonly IRestaurantRepository _restaurantRepository;
    private const int MaxPageSize = 20;
    /// <summary>
    /// The Table Controller Constructor
    /// </summary>
    /// <param name="tableRepository">The Table repository you want to use (inherit the class and implement it if you want the method to work in different way)</param>
    /// <param name="restaurantRepository">The restaurant repository we want to use </param>
    /// <param name="mapper">The mapper to map the Dto</param>
    public TableController(ITableRepository tableRepository, IRestaurantRepository restaurantRepository, IMapper mapper)
    {
        _tableRepository = tableRepository;
        _mapper = mapper;
        _restaurantRepository = restaurantRepository;
    }
    /// <summary>
    /// Get tables
    /// </summary>
    /// <param name="page">the page number you want (default is 1)</param>
    /// <param name="pageSize">the page size you want (default is 20 and max is 20)</param>
    /// <returns>Tables in the range you gave (page number and size)</returns>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<TableWithoutRelationsDto>>> GetTablesAsync(int page = 1, int pageSize = 20)
    {
        if (pageSize > MaxPageSize)
        {
            pageSize = MaxPageSize;
        }
        var (tableEntities, paginationMetaData) = await _tableRepository.GetAllAsync(string.Empty, string.Empty, page, pageSize);
        Response.Headers.Append("X-Pagination",
            JsonSerializer.Serialize(paginationMetaData));
        return Ok(_mapper.Map<IEnumerable<TableWithoutRelationsDto>>(tableEntities));
    }

    /// <summary>
    /// Get a specific table via the table id
    /// </summary>
    /// <param name="id">Table id</param>
    /// <returns>not found if there is no table with that id, the table if exists</returns>
    [HttpGet("{id}")]
    public async Task<ActionResult<TableWithoutRelationsDto>> GetTableByIdAsync(int id)
    {
        var tableEntity = await _tableRepository.GetByIdAsync(id);
        if (tableEntity == null)
        {
            return NotFound();
        }
        return Ok(_mapper.Map<TableWithoutRelationsDto>(tableEntity));
    }
    /// <summary>
    /// Create A Table 
    /// </summary>
    /// <param name="tableCreationDto"> The table info to be created</param>
    /// <returns>NoFound if there is no restaurant with the provided Id, the table if created successfully</returns>
    [HttpPost]
    public async Task<ActionResult<TableDto>> PostTableAsync(TableCreationDto tableCreationDto)
    {
        var restaurant = await _restaurantRepository.GetByIdAsync(tableCreationDto.RestaurantId);
        if (restaurant == null)
        {
            return NotFound();
        }
        var tableEntity = _mapper.Map<Table>(tableCreationDto);
        await _tableRepository.AddAsync(tableEntity);
        return Ok(_mapper.Map<TableDto>(tableEntity));
    }
    /// <summary>
    /// Update table info
    /// </summary>
    /// <param name="id">The table id you want to update</param>
    /// <param name="patchDocument">The new value (for now it's only for the table capacity )</param>
    /// <returns>
    /// Not found if couldn't find the table
    /// bad request if failed
    /// no content if succeed 
    /// </returns>
    [HttpPatch("{id}")]
    public async Task<ActionResult> PatchTableAsync(int id, JsonPatchDocument<TableUpdateDto> patchDocument)
    {
        var table = await _tableRepository.GetByIdAsync(id);
        if (table == null)
        {
            return NotFound();
        }
        var tableEntity = _mapper.Map<TableUpdateDto>(table);
        patchDocument.ApplyTo(tableEntity, ModelState);
        if (!TryValidateModel(tableEntity))
        {
            return BadRequest(ModelState);
        }
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        _mapper.Map(tableEntity, table);
        await _tableRepository.SaveChangesAsync();
        return NoContent();
    }

    /// <summary>
    /// Delete a table
    /// </summary>
    /// <param name="id"> the id of the table you want to delete</param>
    /// <returns>
    /// not found, when could not find the Table   
    /// no content, if succeed
    /// </returns>
    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteTableAsync(int id)
    {
        var table = await _tableRepository.GetByIdAsync(id);
        if (table == null)
        {
            return NotFound();
        }
        await _tableRepository.DeleteAsync(id);
        return NoContent();
    }
}