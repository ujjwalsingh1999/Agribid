using Agribid.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Agribid.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AuthorizationController: ControllerBase
    {

        public AuthorizationController(AgribidCustomerContext customerContext)
        {
            _customerContext = customerContext;
        }

        private readonly AgribidCustomerContext _customerContext;
        [HttpGet]
        public async Task <ActionResult> Get(string email)
        {
            // Access the user's claims, including the JWT token
            //var userId = User.FindFirst(ClaimTypes.Name)?.Value;

            var user = await _customerContext.Customers.SingleOrDefaultAsync(c => c.Email == email);

            // Log the token
            var jwtToken = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "token");
            Console.WriteLine($"JWT Token for User {user}: {jwtToken}");

            // Your other action logic here

            return Ok("Data returned from your action");
        }
    }
}
