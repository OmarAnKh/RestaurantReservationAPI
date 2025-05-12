using AutoMapper;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using RestaurantReservation.Db.Repositories.Interfaces;
using RestaurantReservation.Domain;
using RestaurantReservationAPI.Models.EmployeeDto;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace RestaurantReservationAPI.Controllers;

/// <summary>
/// The Employee controller
/// </summary>
[Route("api/[controller]")]
[ApiController]
public class EmployeeController : ControllerBase
{
    private readonly IEmployeeRepository _employeeRepository;
    private readonly IRestaurantRepository _restaurantRepository;
    private readonly IOrderRepository _orderRepository;
    private readonly IMapper _mapper;
    private const int MaxPageSize = 20;
    /// <summary>
    /// The Employee Controller constructor
    /// </summary>
    /// <param name="employeeRepository">The Employee repository you want to use</param>
    /// <param name="restaurantRepository">The restaurant repository you want to use</param>
    /// <param name="mapper">the mapper to map data between Dtos</param>
    public EmployeeController(IEmployeeRepository employeeRepository, IRestaurantRepository restaurantRepository, IMapper mapper, IOrderRepository orderRepository)
    {
        _employeeRepository = employeeRepository;
        _restaurantRepository = restaurantRepository;
        _mapper = mapper;
        _orderRepository = orderRepository;
    }
    /// <summary>
    /// Get List Of employees that is paginated
    /// </summary>
    /// <param name="name">search for an employee using the fist name</param>
    /// <param name="searchQuery"> search for employees </param>
    /// <param name="pageNumber">The page number you want (default is 1)</param>
    /// <param name="pageSize">The page size you want (default is 10 and max is 20)</param>
    /// <returns></returns>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<EmployeeDto>>> GetEmployees(string? name, string? searchQuery, int pageNumber = 1, int pageSize = 10)
    {
        if (pageSize is < 1 or > MaxPageSize)
        {
            pageSize = MaxPageSize;
        }
        var (employeeEntities, paginationMetaData) = await _employeeRepository.GetAllAsync(name, searchQuery, pageNumber, pageSize);
        var employees = _mapper.Map<IEnumerable<EmployeeDto>>(employeeEntities);
        Response.Headers.Append("X-pagination", JsonSerializer.Serialize(paginationMetaData));
        return Ok(employees);
    }
    /// <summary>
    /// Get a specific Employee
    /// </summary>
    /// <param name="id">the employee id</param>
    /// <returns>
    ///Not found when the employee does not exist
    /// an EmployeeDto when found
    /// </returns>
    [HttpGet("{id}")]
    public async Task<ActionResult<EmployeeDto>> GetEmployee(int id)
    {
        var employee = await _employeeRepository.GetByIdAsync(id);
        if (employee == null)
        {
            return NotFound();
        }
        var employeeDto = _mapper.Map<EmployeeDto>(employee);
        return Ok(employeeDto);
    }
    /// <summary>
    /// Add a new Employee
    /// </summary>
    /// <param name="employeeCreationDto">The EmployeeCreationDto info</param>
    /// <returns>
    /// not found, When the restaurant is not found
    /// EmployeeDto, When the employee is added
    /// </returns>
    [HttpPost]
    public async Task<ActionResult<EmployeeDto>> PostEmployee(EmployeeCreationDto employeeCreationDto)
    {
        var restaurant = await _restaurantRepository.GetByIdAsync(employeeCreationDto.RestaurantId);
        if (restaurant is null)
        {
            return NotFound();
        }
        var employee = _mapper.Map<Employee>(employeeCreationDto);
        await _employeeRepository.AddAsync(employee);
        return Ok(_mapper.Map<EmployeeDto>(employee));
    }
    /// <summary>
    /// Update an employee
    /// </summary>
    /// <param name="id">The id of the Employee you want to update</param>
    /// <param name="employeeUpdateDto">The new values for the employee</param>
    /// <returns>
    /// Not Found when the Employee does not exist
    /// BadRequest when validation is not working or the ModelState is not valid 
    /// No Content When succeed
    /// </returns>
    [HttpPatch("{id}")]
    public async Task<ActionResult> PatchEmployee(int id, JsonPatchDocument<EmployeeUpdateDto> employeeUpdateDto)
    {
        var employee = await _employeeRepository.GetByIdAsync(id);
        if (employee is null)
        {
            return NotFound();
        }
        var employeeToPatch = _mapper.Map<EmployeeUpdateDto>(employee);
        employeeUpdateDto.ApplyTo(employeeToPatch, ModelState);
        if (!TryValidateModel(employeeUpdateDto))
        {
            return BadRequest();
        }
        if (!ModelState.IsValid)
        {
            return BadRequest();
        }
        _mapper.Map(employeeToPatch, employee);
        await _employeeRepository.SaveChangesAsync();
        return NoContent();
    }

    /// <summary>
    /// Delete an employee 
    /// </summary>
    /// <param name="id">the id of the Employee you want to delete</param>
    /// <returns>
    /// not found when could not find the employee   
    /// no content if succeed
    /// </returns>
    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteEmployee(int id)
    {
        var employee = await _employeeRepository.GetByIdAsync(id);
        if (employee is null)
        {
            return NotFound();
        }
        await _employeeRepository.DeleteAsync(id);
        return NoContent();
    }
    /// <summary>
    /// Return a list of all managers
    /// </summary>
    /// <returns></returns>
    [HttpGet("managers")]
    public async Task<ActionResult<IEnumerable<EmployeeDto>>> GetManagers()
    {
        var managers = await _employeeRepository.GetManagersAsync();
        var managersDto = _mapper.Map<IEnumerable<EmployeeDto>>(managers);
        return Ok(managersDto);
    }

    [HttpGet("{employeeId}/average-order-amount")]
    public async Task<ActionResult<double>> GetAverageOrderAmount(int employeeId)
    {
        var employee = await _employeeRepository.GetByIdAsync(employeeId);
        if (employee is null)
        {
            return NotFound();
        }
        var avgOrderAmountForAnEmployee = await _orderRepository.CalculateAverageOrderAmountAsync(employeeId);
        return Ok(avgOrderAmountForAnEmployee);
    }
}