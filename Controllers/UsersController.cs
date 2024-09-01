using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using UserService.DTO;
using UserService.Models;

namespace UserService.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class UsersController : ControllerBase
	{
		private readonly UserService.Services.UserService _userService;
		private readonly IConfiguration _configuration;

		public UsersController(UserService.Services.UserService userService, IConfiguration configuration)
		{
			_userService = userService;
			_configuration = configuration;
		}

		[Authorize]
		[HttpGet]
		public ActionResult<List<User>> Get() =>
				_userService.Get();

		[HttpGet("{id:length(24)}", Name = "GetUser")]
		public ActionResult<User> Get(string id)
		{
			var user = _userService.Get(id);

			if (user == null)
			{
				return NotFound();
			}

			return user;
		}

		[Authorize]
		[HttpPost]
		public ActionResult<User> Create(UserCreateDto userDto)
		{
			var user = new User
			{
				Username = userDto.Username,
				PasswordHash = BCrypt.Net.BCrypt.HashPassword(userDto.PasswordHash),
				Email = userDto.Email
			};

			_userService.Create(user);

			return CreatedAtRoute("GetUser", new { id = user.Id.ToString() }, user);
		}

		[Authorize]
		[HttpPut("{id:length(24)}")]
		public IActionResult Update(string id, UserCreateDto userDto)
		{
			var user = _userService.Get(id);

			if (user == null)
			{
				return NotFound();
			}

			var userIn = new User
			{
				Id=user.Id,
				Username = userDto.Username,
				PasswordHash = BCrypt.Net.BCrypt.HashPassword(userDto.PasswordHash),
				Email = userDto.Email
			};

			_userService.Update(id, userIn);

			return NoContent();
		}

		[Authorize]
		[HttpDelete("{id:length(24)}")]
		public IActionResult Delete(string id)
		{
			var user = _userService.Get(id);

			if (user == null)
			{
				return NotFound();
			}

			_userService.Remove(user.Id);

			return NoContent();
		}

		[HttpPost("login")]
		public IActionResult Login([FromBody] UserLoginDto loginDto)
		{
			var user = _userService.Authenticate(loginDto.Username, loginDto.Password);

			if (user == null)
				return Unauthorized("Invalid username or password.");

			var tokenHandler = new JwtSecurityTokenHandler();
			var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Key"]);
			var tokenDescriptor = new SecurityTokenDescriptor
			{
				Subject = new ClaimsIdentity(new[] { new Claim("id", user.Id.ToString()) }),
				Expires = DateTime.UtcNow.AddHours(1),
				SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
				Issuer = _configuration["Jwt:Issuer"],
				Audience = _configuration["Jwt:Audience"]
			};
			var token = tokenHandler.CreateToken(tokenDescriptor);
			var tokenString = tokenHandler.WriteToken(token);

			return Ok(new { Token = tokenString });
		}
	}
}
