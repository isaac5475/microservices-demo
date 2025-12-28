using UserManagementAPI.Contracts;
using UserManagementAPI.Entities;
using UserManagementAPI.Modells;
using UserManagementAPI.Repositories;

namespace UserManagementAPI.Services
{
    public class UsersService
    {
        private readonly IPasswordHasher _passwordHasher;
        private readonly IUserRepository _userRepository;
        private readonly IJwtProvider _jwtProvider;
        private readonly IRefreshTokenRepository _refreshTokenRepository;

        public UsersService(IPasswordHasher passwordHasher, IUserRepository userRepository, IJwtProvider jwtProvider, IRefreshTokenRepository refreshTokenRepository)
        {
            _passwordHasher = passwordHasher;
            _userRepository = userRepository;
            _jwtProvider = jwtProvider;
            _refreshTokenRepository = refreshTokenRepository;
        }

        public async Task Register(string email, string password, string username)
        {
            var existingUser = await _userRepository.GetByUsername(username);
            if (existingUser != null)
            {
                throw new Exception("Username is already used");
            }
            existingUser = await _userRepository.GetByEmail(email);
            if (existingUser != null)
            {
                throw new Exception("Email is already used");
            }
            var hashedPassword = _passwordHasher.Generate(password);

            var user = User.Create(Guid.NewGuid(), hashedPassword, email, username);

            await _userRepository.Add(user);
        }

        public async Task<LoginUserResponse> Login(string username, string password)
        {
            var user = await _userRepository.GetByUsername(username);
            if(user == null)
            {
                throw new UnauthorizedAccessException("User not found");
            }
            if (!_passwordHasher.Verify(password, user.PasswordHash))
            {
                throw new UnauthorizedAccessException("Invalid password");
            }
            var accessToken = _jwtProvider.GenerateToken(user);

            var refreshToken = _jwtProvider.GenerateRefreshToken(user);
            await _refreshTokenRepository.Add(refreshToken);
            
            return new LoginUserResponse
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken.Token.ToString(),
                User = new UserDTO(user.Username, user.Email)
            };
        }

        public async Task<User> Get(string userId)
        {
            var user = await _userRepository.GetByUserId(userId);
            if (user == null)
            {
                throw new Exception("User not found");
            }
            return user;
        }
        
        public async Task<UserDTO> GetDTO(string userId)
        {
            var user = await _userRepository.GetByUserId(userId);
            if (user == null)
            {
                throw new Exception("User not found");
            }
            var userDTO = new UserDTO(user.Username, user.Email);
            return userDTO;
        }
    }
}
