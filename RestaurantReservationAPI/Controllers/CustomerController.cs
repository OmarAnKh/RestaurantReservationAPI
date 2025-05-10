using System.Text.Json;
using AutoMapper;
using Azure;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using RestaurantReservation.Db.Repositories;
using RestaurantReservation.Domain;
using RestaurantReservationAPI.Models.Customer;

namespace RestaurantReservationAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerController:ControllerBase
    {
        private readonly CustomerRepository _customerRepository;
        private readonly IMapper _mapper;
        const int maxCustomersPageSize = 20;
        public CustomerController(CustomerRepository customerRepository ,IMapper mapper)
        {
            _customerRepository = customerRepository;
            _mapper = mapper;
        }
        
        /// <summary>
        /// Get all Customers, split into pages
        /// </summary>
        /// <param email="email">email to find</param>
        /// <param email="searchQuery">something to search for in the firstname lastname or email</param>
        /// <param email="pageNumber">the page number you want</param>
        /// <param email="pageSize">the page size you want (the max page size is 20 and the default is 10)</param>
        /// <returns>a list of customers and a pagination info in the header</returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CustomerWithoutReservationsDto>>> GetCustomers( string? email,string? searchQuery,int pageNumber=1,int pageSize=10)
        {
            if (pageSize > maxCustomersPageSize)
            {
                pageSize = maxCustomersPageSize;
            }
            var (customerEntities,paginationMetaData)=await _customerRepository.GetAllAsync(email, searchQuery, pageNumber, pageSize);
            Response.Headers.Append("X-Pagination",JsonSerializer.Serialize(paginationMetaData));
            return Ok(_mapper.Map<IEnumerable<CustomerWithoutReservationsDto>>(customerEntities));
        }
        /// <summary>
        /// Get a customer via customer id
        /// </summary>
        /// <param name="id"> the id of the customer</param>
        /// <returns>A customer</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<Customer>> GetCustomer(int id)
        {
            var customer = await _customerRepository.GetByIdAsync(id);
            if (customer == null)
            {
                return NotFound();
            }
            return Ok(customer);
        }
        /// <summary>
        /// Create A Customer
        /// </summary>
        /// <param name="customer"> Your Customer Info</param>
        /// <returns>The Created Customer</returns>
        [HttpPost]
        public async Task<ActionResult<Customer>> PostCustomer(CustomerCreationDto customer)
        {
            
            var finalCustomer=_mapper.Map<Customer>(customer);
            await _customerRepository.AddAsync(finalCustomer);
            var customerCreated= _mapper.Map<Customer>(finalCustomer);
            return CreatedAtAction(nameof(GetCustomer), new { id = finalCustomer.CustomerId }, customerCreated);
        }
        /// <summary>
        /// Update the customer first or last name
        /// </summary>
        /// <param name="id">The costumer id</param>
        /// <param name="patchCustomer">The UpdateDoc</param>
        /// <returns>doesnt return anything</returns>
        [HttpPatch("{id}")]
        public async Task<ActionResult> PatchCustomer(int id,JsonPatchDocument<CustomerUpdateDto> patchCustomer)
        {
            var customer = await _customerRepository.GetByIdAsync(id);
            if (customer == null)
            {
                return NotFound();
            }
            var updatedCustomer = _mapper.Map<CustomerUpdateDto>(customer);
            patchCustomer.ApplyTo(updatedCustomer);
            if (!TryValidateModel(updatedCustomer))
            {
                return BadRequest();
            }
            _mapper.Map(updatedCustomer,customer);
            await _customerRepository.SaveChangesAsync();
            return NoContent();
        }
        /// <summary>
        /// Delete a customer
        /// </summary>
        /// <param name="id">the Customer id</param>
        /// <returns>Nothing</returns>
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteCustomer(int id)
        {
            var customer = await _customerRepository.GetByIdAsync(id);
            if (customer == null)
            {
                return NotFound();
            }
            await _customerRepository.DeleteAsync(id);
            return NoContent();
        }
    }
}