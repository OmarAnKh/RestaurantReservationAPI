using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using RestaurantReservation.Db.Repositories;
using RestaurantReservation.Domain;

namespace RestaurantReservationAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerController:ControllerBase
    {
        private readonly CustomerRepository _customerRepository;
        const int maxCustomersPageSize = 20;
        public CustomerController(CustomerRepository customerRepository)
        {
            _customerRepository = customerRepository;
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
        public async Task<ActionResult<IEnumerable<Customer>>> GetCustomers( string? email,string? searchQuery,int pageNumber=1,int pageSize=10)
        {
            if (pageSize > maxCustomersPageSize)
            {
                pageSize = maxCustomersPageSize;
            }
            var (customerEntities,paginationMetaData)=await _customerRepository.GetAllAsync(email, searchQuery, pageNumber, pageSize);
            Response.Headers.Append("X-Pagination",JsonSerializer.Serialize(paginationMetaData));
            return Ok(customerEntities);
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
    }
}