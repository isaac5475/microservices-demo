using Microsoft.AspNetCore.Mvc;
using UserManagementAPI.Contracts;
using UserManagementAPI.Services;

namespace UserManagementAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TokensController : ControllerBase
    {
        private readonly RefreshTokenService _refreshTokenService;
        private readonly UsersService _usersService;
        private readonly IJwtProvider _jwtProvider;

        public TokensController(RefreshTokenService refreshTokenService, UsersService usersService, IJwtProvider jwtProvider)
        {
            _refreshTokenService = refreshTokenService;
            _usersService = usersService;
            _jwtProvider = jwtProvider;
        }

        // POST: api/Tokens/refreshToken
        [HttpPost("refreshToken")]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest refreshToken)
        {
            var token = refreshToken.RefreshToken;

            var storedToken = await _refreshTokenService.GetByToken(token);

            if (storedToken == null || storedToken.Expires < DateTime.UtcNow)
            {
                return Unauthorized("Invalid or expired refresh token.");
            }

            var userId = storedToken.UserId.ToString();

            await _refreshTokenService.Delete(storedToken);

            // Generate new JWT and refresh token (save refresh to db)
            var user = await _usersService.Get(userId);
            if (user == null)
            {
                return Unauthorized("User not found.");
            }

            var newAccessToken = _jwtProvider.GenerateToken(user);
            var newRefreshToken = _jwtProvider.GenerateRefreshToken(user);

            await _refreshTokenService.Save(newRefreshToken);

            //Return tokens
            return Ok(new { AccessToken = newAccessToken, RefreshToken = newRefreshToken.Token });
        }
    }
}
