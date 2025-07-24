using ENDE.Models.User;

namespace ENDE.Api.Services
{
    public interface IAuthService
    {
        public User? ValidateUser(User user);
        public string ValidatePassword(string password);
        public User? RegisterUser(UserDto userDto);
        public string LoginUser(User user, UserLoginDto userDto);
        public Task<string> GetGuidFromTokenAsync(string token);
        public string CreateToken(User user);
        public string CreateRefreshToken();
        public string RefreshUser(User user);
        public bool IsPasswordValid(string validationResult);
    }
}
