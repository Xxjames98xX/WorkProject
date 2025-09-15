using Microsoft.AspNetCore.Mvc;
using ASPWebAPI.Models;
using ASPWebAPI.Database;
using Serilog;

//Ideally the password should be hashed and salted before storing in the database
//This is a simple example for demonstration purposes only
namespace ASPWebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")] // Route: "api/Login" [controller] is replaced by the controller name without "Controller" suffix
    public class LoginController : ControllerBase
    {
        // --------------------------------  Dependency Injection  -----------------------------------
        //For logging
        //Read only means can only be set in the constructor/assigned once
        private readonly ILogger<LoginController> _logger;

        //Dependency Injection of the AccountsDBContext, Can refer to builder services in Program.cs
        private readonly AccountsDBContext _context;

        // When the class is created by the framework, it will automatically provide the required dependencies
        // It will see that the constructor requires an ILogger and AccountsDBContext, and will try to provide them, if found.
        // This is done by the built-in dependency injection system in ASP.NET Core
        public LoginController(ILogger<LoginController> logger, AccountsDBContext context)
        {
            _logger = logger;
            _context = context;
        }


        // ------------------------------  Handle HTTP Request (Action Method)  -----------------------------------


        // IActionResult is a common return type for controller actions/response results. Interface ActionResult
        // It allows for flexibility in returning different types of responses, such as Ok(), NotFound
        // [FromBody] indicates that the request body should be deserialized (json response) into the LoginRequest object

        [HttpPost("authenticate")] // POST api/Login/authenticate
        public IActionResult Login([FromBody] LoginRequest request)
        {
            var isValid = _context.Accounts.Any(row => row.Username == request.Username && row.Password == request.Password);
            if (isValid)
            {
                _logger.LogInformation("User {Username} logged in successfully", request.Username);
                return Ok(new { token = "dummy-jwt-token" });
            }

            _logger.LogInformation("User {Username} tried to login, but invalid credentials", request.Username);
            return Unauthorized(new { message = "Invalid credentials" });
        }


        // Adds accounts into the MS SQL database
        [HttpPost("register")] // POST api/Login/register
        public IActionResult Register([FromBody] LoginRequest request)
        {
            // Dummy registration logic for demonstration purposes
            if (!string.IsNullOrEmpty(request.Username) && !string.IsNullOrEmpty(request.Password))
            {
                // Based of this declaration in the EntityFramework.cs -> public DbSet<LoginRequest> Accounts { get; set; }
                // Specify the table to add to
                _context.Accounts.Add(request);
                _context.SaveChanges();
                return Ok(new { message = "User registered successfully" });
            }
            return BadRequest(new { message = "Invalid registration data" });
        }
    }
}