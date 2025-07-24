using ENDE.Api.Services;
using ENDE.Data.Repositories;
using ENDE.Models.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ENDE.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly IAuthService _authService;

        public AuthController(IUserRepository userRepository, IAuthService authService)
        {
            _userRepository = userRepository;
            _authService = authService;
        }

        [Authorize]
        [HttpGet("validate")]
        public IActionResult ValidateSession()
        {
            return Ok();
        }

        [HttpPost("register")]
        public async Task<IActionResult> RegisterUser(UserDto userDto)
        {
            if (userDto == null)
            {
                return BadRequest("Invalid request. user field is null");
            }

            if (string.IsNullOrEmpty(userDto.Username) ||
                string.IsNullOrEmpty(userDto.Email) ||
                string.IsNullOrEmpty(userDto.Password))
            {
                return BadRequest("No field can be null");
            }

            var passwordValidation = _authService.ValidatePassword(userDto.Password);
            if (!_authService.IsPasswordValid(passwordValidation))
            {
                return BadRequest(passwordValidation);
            }

            var user = _authService.RegisterUser(userDto);
            if (user == null)
            {
                return BadRequest("Invalid username, email, number or password");
            }

            await _userRepository.AddUserAsync(user);

            return Ok(user);
        }

        [HttpPost("login")]
        public async Task<IActionResult> LoginUser(UserLoginDto loginDto)
        {
            if (loginDto == null)
            {
                return BadRequest("Invalid request. user is null");
            }

            var user = await _userRepository.GetUserByEmailAsync(loginDto.Email);
            if (user == null)
            {
                return BadRequest("Incorrect email or password");
            }

            var authToken = _authService.LoginUser(user, loginDto);
            if (authToken == string.Empty)
            {
                return BadRequest("Incorrect email or password");
            }

            await _userRepository.UpdateUserAsync(user);

            var data = new 
            {
                Username = (await _userRepository.GetUserByEmailAsync(loginDto.Email))!.Username,
                AuthToken = authToken
            };

            return Ok(data);
        }

        [HttpPut("refresh")]
        public async Task<IActionResult> RefreshUser(RefreshTokenRequestDto request)
        {
            if (request == null)
            {
                return BadRequest("Invalid data");
            }

            string id = await _authService.GetGuidFromTokenAsync(request.AuthToken);
            if (id == string.Empty)
            {
                return BadRequest("Invalid token");
            }

            var user = await _userRepository.GetUserByGuidAsync(id);

            if (user == null)
            {
                return BadRequest("User doesn't exist");
            }

            var validate = _authService.ValidateUser(user);
            if (validate == null)
            {
                return Unauthorized("Refresh token expired");
            }

            _authService.RefreshUser(user);
            var authToken = _authService.CreateToken(user);

            await _userRepository.UpdateUserAsync(user);

            return Ok(authToken);
        }
    }
}
