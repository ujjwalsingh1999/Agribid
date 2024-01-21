using Agribid.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Routing;
using System.ComponentModel;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860


namespace Agribid.Controllers
{
    [Route("api/customers")]
    [ApiController]
    public class CustomersController : ControllerBase
    {
        private readonly AgribidCustomerContext _agribidCustomerContext;
        public CustomersController(AgribidCustomerContext customerContext)
        {
            _agribidCustomerContext = customerContext;
        }
        [AllowAnonymous]
        [HttpGet("GetCustomers")]
        public async Task<ActionResult<IEnumerable<Customer>>> GetCustomers()
        {
            try
            {
                var customers = await _agribidCustomerContext.Customers.ToListAsync();

                if (customers.Count == 0)
                {
                    return NotFound();
                }

                return customers;
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred while retrieving customers.");
            }
        }

        [AllowAnonymous]
        [HttpGet("GetCustomerById/{id}")]  // Explicitly specifying the HTTP verb
        public async Task<ActionResult<Customer>> GetCustomerById(int id)
        {
            // ... (your existing code)
            try
            {
                var customer = await _agribidCustomerContext.Customers.FindAsync(id);

                if (customer == null)
                {
                    return NotFound();
                }

                return customer;
            }
            catch (Exception ex)
            {
                // Error handling without logging
                return StatusCode(500, "An error occurred while retrieving customer.");
            }

        }

        [AllowAnonymous]
        [HttpGet("FindByEmail/{email}")]  // Explicitly specifying the HTTP verb
        public async Task<ActionResult<Customer>> FindCustomerByEmail(string email)
        {
            // ... (your existing code)
            try
            {
                if (string.IsNullOrEmpty(email))
                {
                    return BadRequest("Email can not be empty.");
                }
                var customer = await _agribidCustomerContext.Customers.SingleOrDefaultAsync(c => c.Email == email);

                if (customer == null)
                {
                    return NotFound("Error: User not found");
                }
                return customer;
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An internal server has occured");
            }
        }

        [AllowAnonymous]
        [HttpPost("AddCustomer")]
        public async Task<IActionResult> AddCustomer(Customer customer)
        {
            try
            {
                if (customer == null)
                {
                    return BadRequest("Customer cannot be null.");
                }

                var checkExisitingEmail = await _agribidCustomerContext.Customers.Where(c => c.Email == customer.Email).FirstOrDefaultAsync();

                var checkExisitingMobile = await _agribidCustomerContext.Customers.Where(c => c.PhoneNumber == customer.PhoneNumber).FirstOrDefaultAsync();
                if (checkExisitingEmail != null)
                {
                    return BadRequest("Email alredy in use.");
                }
                else if (checkExisitingMobile != null)
                {
                    return BadRequest("Mobile Number already exsist.");
                }

                customer.CreatedAt = DateTime.UtcNow;
                customer.UpdatedAt = DateTime.UtcNow;

                // Add customer to database
                _agribidCustomerContext.Customers.Add(customer);
                await _agribidCustomerContext.SaveChangesAsync();

                return CreatedAtAction("GetCustomerById", new { id = customer.CustomerId }, customer);
            }
            catch (DbUpdateException ex)
            {
                return StatusCode(500, "Error saving customer to database.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred while adding customer.");
            }
        }

        [Authorize]
        [HttpDelete("DeleteCustomer/{id}")]
        public async Task<ActionResult> DeleteCustomer(int id)
        {
            try
            {
                // Validate customer ID
                if (id <= 0)
                {
                    return BadRequest("Invalid Customer ID.");
                }

                var customer = await _agribidCustomerContext.Customers.FindAsync(id);

                if (customer == null)
                {
                    return NotFound($"Customer with CustomerID {id} not found.");
                }

                _agribidCustomerContext.Customers.Remove(customer);
                await _agribidCustomerContext.SaveChangesAsync();

                return Ok($"Customer with ID {id} has been deleted.");
            }
            catch (DbUpdateException ex)
            {
                return StatusCode(500, "Error deleting customer. Please check for dependencies.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An unexpected error occurred while deleting customer.");
            }
        }

        [AllowAnonymous]
        [HttpPut("UpdateCustomer/{id}")]
        public async Task<ActionResult<Customer>> UpdateCustomer(int id, Customer customer)
        {

            try
            {
                var customerObj = await _agribidCustomerContext.Customers.FindAsync(id);
                if (customerObj == null)
                {
                    return NotFound();
                }

                var checkExisitingEmail = await _agribidCustomerContext.Customers.Where
                    (c => c.Email == customer.Email).FirstOrDefaultAsync();

                var checkExisitingMobile = await _agribidCustomerContext.Customers.Where(c => c.PhoneNumber == customer.PhoneNumber).FirstOrDefaultAsync();
                if (checkExisitingEmail != null)
                {
                    return BadRequest("Email alredy in use.");
                }
                else if (checkExisitingMobile != null)
                {
                    return BadRequest("Mobile Number already exsist.");
                }
                else
                {
                    customerObj.FirstName = customer.FirstName;
                    customerObj.LastName = customer.LastName;
                    customerObj.Email = customer.Email;
                    customerObj.Address = customer.Address;
                    customerObj.PhoneNumber = customer.PhoneNumber;
                    customerObj.CreatedAt = customer.CreatedAt;
                    customerObj.UpdatedAt = DateTime.UtcNow;
                }
                _agribidCustomerContext.Update(customerObj);
                await _agribidCustomerContext.SaveChangesAsync();
                return customerObj;
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred while retrieving customer.");
            }
        }

    }
}