using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using RestaurantReservation.Db.Repositories.Interfaces;
using RestaurantReservation.Domain;
using RestaurantReservationAPI.Models.UserDto;

namespace RestaurantReservationAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UserController : ControllerBase
{
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;
    private readonly string? _secretKey = Environment.GetEnvironmentVariable("SECRET_KEY");
    private readonly string? _issuer = Environment.GetEnvironmentVariable("ISSUER");
    private readonly string? _audience = Environment.GetEnvironmentVariable("AUDIENCE");

    public UserController(IUserRepository userRepository, IMapper mapper)
    {
        _userRepository = userRepository;
        _mapper = mapper;
    }

    [HttpPost("register")]
    public async Task<ActionResult> Register([FromBody] UserRegisterDto dto)
    {
        var user = _mapper.Map<User>(dto);
        var result = await _userRepository.CreateUserAsync(user);
        return result switch
        {
            1 => Ok("User created successfully."),
            2 => Conflict("User already exists."),
            _ => StatusCode(500, "Internal server error.")
        };
    }

    [HttpPost("login")]
    public async Task<ActionResult<string>> Login([FromBody] UserLoginDto dto)
    {
        var user = await _userRepository.ValidateUserAsync(dto.UserName, dto.Password);
        if (user == null)
        {
            return Unauthorized("Invalid username or password.");
        }

        if (string.IsNullOrEmpty(_secretKey) || string.IsNullOrEmpty(_issuer) || string.IsNullOrEmpty(_audience))
        {
            return StatusCode(500, "JWT configuration is missing.");
        }

        var key = new SymmetricSecurityKey(Convert.FromBase64String(_secretKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, user.UserName),
            new Claim("UserId", user.UserId.ToString())
        };

        var token = new JwtSecurityToken(
            issuer: _issuer,
            audience: _audience,
            claims: claims,
            expires: DateTime.UtcNow.AddHours(1),
            signingCredentials: creds
        );

        var tokenString = new JwtSecurityTokenHandler().WriteToken(token);
        return Ok(tokenString);
    }
}