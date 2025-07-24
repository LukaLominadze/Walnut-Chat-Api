using ENDE.Models.User;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using System.Text;
using ENDE.Data.Repositories;

namespace ENDE.Api.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly PasswordHasher<User> _passwordHasher;
        private readonly IConfiguration _configuration;

        public AuthService(IUserRepository userRepository,
                           PasswordHasher<User> passwordHasher,
                           IConfiguration configuration)
        {
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
            _configuration = configuration;
        }

        public string CreateRefreshToken()
        {
            var randomNumber = new byte[32];
            var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }

        public string CreateToken(User user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.MobilePhone, user.PhoneNumber),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Role, user.Role)
            };

            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_configuration.GetValue<string>("AppSettings:Token")!));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var tokenDescriptor = new JwtSecurityToken(
                issuer: _configuration.GetValue<string>("AppSettings:Issuer"),
                audience: _configuration.GetValue<string>("AppSettings:Audience"),
                claims: claims,
                signingCredentials: creds,
                expires: DateTime.UtcNow
                    .AddMinutes(_configuration.GetValue<double>(
                        "AppSettings:TokenExpiryTimeMinutes")
                    )
                );

            var token = new JwtSecurityTokenHandler().WriteToken(tokenDescriptor);

            return token;
        }

        public async Task<string> GetGuidFromTokenAsync(string token)
        {
            var handler = new JwtSecurityTokenHandler();
            var jwtSecurityToken = handler.ReadJwtToken(token);
            string? response = await ValidateTokenDataAsync(token);
            if (response == null)
            {
                return string.Empty;
            }
            Claim userClaim = jwtSecurityToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)!;
            if (userClaim == null)
            {
                return string.Empty;
            }
            return userClaim.Value;
        }

        public bool IsPasswordValid(string passwordValidation)
        {
            return passwordValidation.ToLower().Equals("success");
        }

        public string LoginUser(User user, UserLoginDto userDto)
        {
            var verifyPassword = _passwordHasher.VerifyHashedPassword(user, user.HashedPassword, userDto.Password);
            if (verifyPassword == PasswordVerificationResult.Failed ||
                userDto.Email != user.Email)
            {
                return string.Empty; 
            }

            if (ValidateUser(user) == null)
            {
                RefreshUser(user);
            }

            return CreateToken(user);
        }

        public string RefreshUser(User user)
        {
            user.RefreshToken = CreateRefreshToken();
            user.RefreshTokenExpiryData = DateTime.UtcNow.AddMinutes(
                _configuration.GetValue<double>("AppSettings:RefreshTokenExpiryTimeMinutes")
                );
            return user.RefreshToken;
        }

        public User? RegisterUser(UserDto userDto)
        {
            var user = new User();
            user.Username = userDto.Username;

            bool validateEmail = Regex.IsMatch(userDto.Email, @"^[^@\s]+@[^@\s]+\.(com|ge)$");
            if (!validateEmail)
            {
                return null;
            }

            bool validateNumber = userDto.PhoneNumber.Length == 9 &&
                                  Regex.IsMatch(userDto.PhoneNumber, @"^\d+$");
            if (!validateNumber)
            {
                return null;
            }

            user.Email = userDto.Email;
            user.PhoneNumber = userDto.PhoneNumber;
            user.HashedPassword = _passwordHasher.HashPassword(user, userDto.Password);

            return user;
        }

        public string ValidatePassword(string password)
        {
            if (password.Length < 10)
            {
                return "Password must be at least 10 characters!";
            }
            else if (password.ToLower().Equals(password))
            {
                return "Password must have at least one uppercase letter!";
            }
            else if (!Regex.IsMatch(password, @"\d"))
            {
                return "Password must have at least one number!";
            }
            else if (!Regex.IsMatch(password, @"[!@#$%^&*(),.?""{}|<>_\-+=\[\]\\\/~`']"))
            {
                return "Password must have at least one special character!";
            }
            return "Success";
        }

        public User? ValidateUser(User user)
        {
            
            if (user.RefreshToken == null ||
                user.RefreshTokenExpiryData < DateTime.UtcNow)
            {
                return null;
            }
            return user;
        }

        private async Task<string?> ValidateTokenDataAsync(string token)
        {
            var handler = new JwtSecurityTokenHandler();
            var jwtSecurityToken = handler.ReadJwtToken(token);
            string issuer = _configuration.GetValue<string>("AppSettings:Issuer")!;
            if (jwtSecurityToken.Issuer != issuer)
            {
                return null;
            }
            string audience = _configuration.GetValue<string>("AppSettings:Audience")!;
            if (jwtSecurityToken.Audiences.First() != audience)
            {
                return null;
            }
            string id = jwtSecurityToken.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value;
            var user = await _userRepository.GetUserByGuidAsync(id);
            if (user == null)
            {
                return null;
            }
            return token;
        }
    }
}
