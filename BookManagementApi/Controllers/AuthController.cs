
using DataAccess.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using Microsoft.IdentityModel.Tokens;
using Models.Entities;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace BookManagementApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly BookDB _db;
		private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
		private readonly SignInManager<User> _signInManager;
		private readonly IConfiguration _configuration;

		public AuthController(BookDB db, UserManager<User> userManager, RoleManager<IdentityRole> roleManager, SignInManager<User> signInManager, IConfiguration configuration)
		{
			_db = db;
			_userManager = userManager;
			_roleManager = roleManager;
			_signInManager = signInManager;
			_configuration = configuration;
		}

		[HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto request)
        {
			var existingUser = await _userManager.FindByEmailAsync(request.Email);
			if (existingUser != null)
			{
				return Conflict(new { message = "Email is already registered." });
			}

			var user = new User
			{
				UserName = request.Email,
				Email = request.Email,
				FullName = request.FullName,
			};

			var validationContext = new ValidationContext(user, serviceProvider: null, items: null);
			var validationResults = new List<ValidationResult>();
			var isValid = Validator.TryValidateObject(user, validationContext, validationResults, validateAllProperties: true);

			if (!isValid)
			{
				var errors = validationResults.Select(r => r.ErrorMessage).ToList();
				return BadRequest(new { message = errors });
			}

			var result = await _userManager.CreateAsync(user, request.Password);
			if (result.Succeeded)
			{
				await _userManager.AddToRoleAsync(user, "user");
				return Created(string.Empty, new { message = "User registered successfully" });

			}
			return BadRequest(new { message = "Error creating user", errors = result.Errors });
		}

		[HttpPost("login")]
		public async Task<IActionResult> Login([FromBody] LoginDto request)
		{
			if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
				return BadRequest(new { message = "Email and password are required." });

			var user = await _userManager.FindByEmailAsync(request.Email);

			if (user == null)
				return Unauthorized(new { message = "Invalid credentials." });

			var result = await _signInManager.PasswordSignInAsync(user.UserName, request.Password, false, false);
			if (!result.Succeeded)
				return Unauthorized(new { message = "Invalid credentials." });

			try
			{
				var claims = new List<Claim>
				{
					new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
					new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
					new Claim("email", user.Email),
					new Claim("fullName", user.FullName)
				};

				var roles = _userManager.GetRolesAsync(user).Result; 
				foreach (var role in roles)
				{
					claims.Add(new Claim(ClaimTypes.Role, role));
				}

				var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Secret"]));
				var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

				var token = new JwtSecurityToken(
					issuer: _configuration["Jwt:Issuer"],
					audience: _configuration["Jwt:Audience"],
					claims: claims,
					expires: DateTime.Now.AddDays(1),
					signingCredentials: creds
				);
				var tokenString = new JwtSecurityTokenHandler().WriteToken(token);
				return Ok(new
				{
					message = "Login successful",
					token = tokenString
				});
			}
			catch (Exception ex)
			{
				// Log the exception for debugging
				Console.WriteLine(ex.Message);
				return StatusCode(500, new { message = "An error occurred while generating the token." });
			}
		}

		[Authorize(Roles = "admin")]
		[HttpPost("make-admin")]
		public async Task<IActionResult> MakeAdmin([FromBody] MakeAdminDto request)
		{
			if (string.IsNullOrWhiteSpace(request.Email))
			{
				return BadRequest(new { message = "Email is required." });
			}

			var user = await _userManager.FindByEmailAsync(request.Email);
			if (user == null)
			{
				return NotFound(new { message = "User not found." });
			}

			if (await _userManager.IsInRoleAsync(user, "admin"))
			{
				return Conflict(new { message = "User is already an admin." });
			}

			var result = await _userManager.AddToRoleAsync(user, "admin");
			if (!result.Succeeded)
			{
				return BadRequest(new { message = "Error promoting user to admin", errors = result.Errors });
			}

			return Ok(new { message = "User has been promoted to admin successfully." });
		}

	}

	public record RegisterDto (string FullName, string Email, string Password);
    public record LoginDto (string Email, string Password);
	public record MakeAdminDto(string Email);

}
