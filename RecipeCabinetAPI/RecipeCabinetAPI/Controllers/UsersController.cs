using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RecipeCabinetAPI.Data;
using RecipeCabinetAPI.Models;
using RecipeCabinetAPI.Services;
using System.Security.Claims;

namespace RecipeCabinetAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly TokenService _tokenService;

        public UsersController(ApplicationDbContext context, TokenService tokenService)
        {
            _context = context;
            _tokenService = tokenService;
        }

        // POST: api/Users/register
        [HttpPost("register")]
        public async Task<ActionResult<User>> RegisterUser(User user)
        {
            if (_context.Users.Any(u => u.Email == user.Email))
            {
                return Conflict("Email already in use");
            }
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetUser), new { email = user.Email }, user);
        }

        // POST: api/Users/login
        [HttpPost("login")]
        public async Task<ActionResult<string>> Login(UserLoginDto userLogin)
        {
            var user = await _context.Users.SingleOrDefaultAsync(u => u.Email == userLogin.Email);
            if (user != null && BCrypt.Net.BCrypt.Verify(userLogin.Password, user.Password))
            {
                var token = _tokenService.GenerateJwtToken(user);
                return Ok(token);
            }
            return Unauthorized("Invalid credentials");
        }

        [HttpGet("{email}")]
        public async Task<ActionResult<User>> GetUser(string email)
        {
            var user = await _context.Users.FindAsync(email);
            if (user == null) return NotFound();
            return user;
        }

        // GET: api/Users/validate-token
        [HttpGet("validate-token")]
        [Authorize]
        public IActionResult ValidateToken()
        {
            return Ok(new { message = "Token is valid" });
        }

        [HttpGet("{email}/username")]
        public async Task<ActionResult<string>> GetUsernameByEmail(string email)
        {
            var user = await _context.Users.FindAsync(email);
            if (user == null)
            {
                return NotFound("User not found");
            }

            return Ok(user.Username);
        }

        [HttpPost("change-password")]
        [Authorize]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto changePasswordDto)
        {
            var userEmail = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userEmail == null)
            {
                return Unauthorized("User is not authenticated");
            }

            var user = await _context.Users.SingleOrDefaultAsync(u => u.Email == userEmail);
            if (user == null)
            {
                return NotFound("User not found");
            }

            if (!BCrypt.Net.BCrypt.Verify(changePasswordDto.CurrentPassword, user.Password))
            {
                return BadRequest("Current password is incorrect");
            }

            user.Password = BCrypt.Net.BCrypt.HashPassword(changePasswordDto.NewPassword);
            _context.Users.Update(user);
            await _context.SaveChangesAsync();

            return Ok("Password changed successfully");
        }

        [HttpPost("change-username")]
        [Authorize]
        public async Task<IActionResult> ChangeUsername([FromBody] string changeUsernameRequest)
        {
            var userEmail = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userEmail == null)
            {
                return Unauthorized("User is not authenticated");
            }

            var user = await _context.Users.SingleOrDefaultAsync(u => u.Email == userEmail);
            if (user == null)
            {
                return NotFound("User not found");
            }

            if (user.Username == changeUsernameRequest)
            {
                return BadRequest("New username cannot be the same as the current username");
            }

            user.Username = changeUsernameRequest;
            _context.Users.Update(user);
            await _context.SaveChangesAsync();

            return Ok("Username changed successfully");
        }
    }
}
