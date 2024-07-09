using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Moq;
using RecipeCabinetAPI.Controllers;
using RecipeCabinetAPI.Data;
using RecipeCabinetAPI.Models;
using RecipeCabinetAPI.Services;
using System.Security.Claims;

namespace RecipeCabinet.Tests
{
    public class UsersControllerTests : IDisposable
    {
        private readonly ApplicationDbContext _context;
        private readonly Mock<TokenService> _mockTokenService;
        private readonly UsersController _controller;

        public UsersControllerTests()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "RecipeCabinetTest")
                .Options;
            _context = new ApplicationDbContext(options);

            var mockConfiguration = new Mock<IConfiguration>();
            mockConfiguration.SetupGet(x => x["Jwt:Key"]).Returns("your-256-bit-secret");
            mockConfiguration.SetupGet(x => x["Jwt:Issuer"]).Returns("issuer");
            mockConfiguration.SetupGet(x => x["Jwt:Audience"]).Returns("audience");

            _mockTokenService = new Mock<TokenService>(mockConfiguration.Object);
            _controller = new UsersController(_context, _mockTokenService.Object);

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext()
            };
        }

        [Fact]
        public async Task RegisterUser_ReturnsConflict_WhenEmailAlreadyInUse()
        {
            var user = new User { Email = "test@example.com", Password = "password", Username = "testuser" };
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            var result = await _controller.RegisterUser(user);

            Assert.IsType<ConflictObjectResult>(result.Result);
        }

        [Fact]
        public async Task RegisterUser_ReturnsCreatedAtActionResult_ForValidUser()
        {
            var user = new User { Email = "newuser@example.com", Password = "password", Username = "newuser" };

            var result = await _controller.RegisterUser(user);

            Assert.IsType<CreatedAtActionResult>(result.Result);
        }

        [Fact]
        public async Task Login_ReturnsUnauthorized_WhenCredentialsAreInvalid()
        {
            var userLogin = new UserLoginDto { Email = "nonexistent@example.com", Password = "wrongpassword" };

            var result = await _controller.Login(userLogin);

            Assert.IsType<UnauthorizedObjectResult>(result.Result);
        }

        [Fact]
        public async Task GetUser_ReturnsNotFound_WhenUserDoesNotExist()
        {
            var result = await _controller.GetUser("nonexistent@example.com");

            Assert.IsType<NotFoundResult>(result.Result);
        }

        [Fact]
        public async Task GetUser_ReturnsUser_ForExistingUser()
        {
            var user = new User { Email = "test@example.com", Password = "password", Username = "testuser" };
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            var result = await _controller.GetUser(user.Email);

            Assert.IsType<ActionResult<User>>(result);
            var returnedUser = result.Value;
            Assert.Equal(user.Email, returnedUser.Email);
        }

        [Fact]
        public void ValidateToken_ReturnsOk_WhenTokenIsValid()
        {
            _controller.ControllerContext.HttpContext.User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.NameIdentifier, "test@example.com")
            }, "mock"));

            var result = _controller.ValidateToken();

            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task ChangePassword_ReturnsUnauthorized_WhenUserIsNotAuthenticated()
        {
            var changePasswordDto = new ChangePasswordDto { CurrentPassword = "current", NewPassword = "new" };

            var result = await _controller.ChangePassword(changePasswordDto);

            Assert.IsType<UnauthorizedObjectResult>(result);
        }

        [Fact]
        public async Task ChangePassword_ReturnsBadRequest_WhenCurrentPasswordIsIncorrect()
        {
            var user = new User { Email = "test@example.com", Password = BCrypt.Net.BCrypt.HashPassword("password"), Username = "testuser" };
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            _controller.ControllerContext.HttpContext.User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Email)
            }, "mock"));

            var changePasswordDto = new ChangePasswordDto { CurrentPassword = "wrongpassword", NewPassword = "newpassword" };

            var result = await _controller.ChangePassword(changePasswordDto);

            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task ChangePassword_ReturnsOk_WhenPasswordIsSuccessfullyChanged()
        {
            var user = new User { Email = "test@example.com", Password = BCrypt.Net.BCrypt.HashPassword("password"), Username = "testuser" };
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            _controller.ControllerContext.HttpContext.User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Email)
            }, "mock"));

            var changePasswordDto = new ChangePasswordDto { CurrentPassword = "password", NewPassword = "newpassword" };

            var result = await _controller.ChangePassword(changePasswordDto);

            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task ChangeUsername_ReturnsUnauthorized_WhenUserIsNotAuthenticated()
        {
            var changeUsernameRequest = "newusername";

            var result = await _controller.ChangeUsername(changeUsernameRequest);

            Assert.IsType<UnauthorizedObjectResult>(result);
        }

        [Fact]
        public async Task ChangeUsername_ReturnsBadRequest_WhenNewUsernameIsSameAsCurrent()
        {
            var user = new User { Email = "test@example.com", Username = "currentusername", Password = "password" };
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            _controller.ControllerContext.HttpContext.User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Email)
            }, "mock"));

            var changeUsernameRequest = "currentusername";

            var result = await _controller.ChangeUsername(changeUsernameRequest);

            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task ChangeUsername_ReturnsOk_WhenUsernameIsSuccessfullyChanged()
        {
            var user = new User { Email = "test@example.com", Username = "oldusername", Password = "password" };
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            _controller.ControllerContext.HttpContext.User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Email)
            }, "mock"));

            var changeUsernameRequest = "newusername";

            var result = await _controller.ChangeUsername(changeUsernameRequest);

            Assert.IsType<OkObjectResult>(result);
        }


        public void Dispose()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }
    }
}
