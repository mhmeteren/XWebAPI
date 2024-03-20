using Entities.CacheModels;
using Entities.DataTransferObjects.BaseUser;
using Entities.Exceptions.BaseUser;
using Entities.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Services.Contracts;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Services
{
    public class AuthenticationManager(
        ICacheService cacheService,
        UserManager<BaseUser> baseUser,
        IConfiguration config) : IAuthenticationService
    {



        private readonly UserManager<BaseUser> _baseUserManager = baseUser;
        private readonly IConfiguration _config = config;
        private readonly ICacheService _cacheService = cacheService;

        protected BaseUser? _baseUser;







        #region JWT Token Modules
        public async Task<bool> ValidateUser(BaseUserDtoForLogin baseUserDtoForLogin)
        {
            _baseUser = await _baseUserManager.FindByNameAsync(baseUserDtoForLogin.Username);
            var result = (
                _baseUser != null 
                && await _baseUserManager.CheckPasswordAsync(_baseUser, baseUserDtoForLogin.Password) 
                && await _baseUserManager.IsEmailConfirmedAsync(_baseUser));
            return result;
        }


        public async Task<TokenDto> CreateToken(bool populateExp)
        {
            var signinCredentials = GetSigninCredentials();
            var claims = await GetClaims();
            var tokenOptions = GenerateTokenOptions(signinCredentials, claims);

            var refreshToken = GenerateRefreshToken();
            TimeSpan? remainingTime = null;

            string cacheKey = string.Format(CacheSettings.UserRefreshToken.Key, _baseUser.UserName);

            if (!populateExp)
            {
                remainingTime = await _cacheService.GetRemainingTime(cacheKey);
            }

            await _cacheService.SetCachedData(cacheKey, refreshToken, remainingTime ?? CacheSettings.UserRefreshToken.ExpirationTime);

            var accessToken = new JwtSecurityTokenHandler().WriteToken(tokenOptions);
            return new TokenDto
            (
                AccessToken : accessToken,
                RefreshToken : refreshToken
            );

        }


        private SigningCredentials GetSigninCredentials()
        {
            var jwtSettings = _config.GetSection("JwtSettings");
            var key = Encoding.UTF8.GetBytes(jwtSettings["secretKey"]);
            var secret = new SymmetricSecurityKey(key);
            return new SigningCredentials(secret, SecurityAlgorithms.HmacSha256);
        }


        private async Task<List<Claim>> GetClaims()
        {
            var claims = new List<Claim>()
            {
                new(ClaimTypes.Name,_baseUser.UserName)
            };

            var roles = await _baseUserManager.GetRolesAsync(_baseUser);

            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }
            return claims;
        }


        private JwtSecurityToken GenerateTokenOptions(SigningCredentials signinCredentials, List<Claim> claims)
        {
            var jwtSettings = _config.GetSection("JwtSettings");

            var tokenOptions = new JwtSecurityToken(
                issuer: jwtSettings["validIssuer"],
                audience: jwtSettings["validAudience"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(Convert.ToDouble(jwtSettings["expires"])),
                signingCredentials: signinCredentials);

            return tokenOptions;

        }


        private string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);

        }


        public async Task<TokenDto> RefreshToken(TokenDto tokenDto)
        {
            var principal = GetPrincipalFromExpiredToken(tokenDto.AccessToken);
            var user = await _baseUserManager.FindByNameAsync(principal.Identity.Name);


            if (user is null || 
                await _cacheService.GetCachedData<string>(string.Format(CacheSettings.UserRefreshToken.Key, user.UserName)) != tokenDto.RefreshToken)
                throw new RefreshTokenBadRequestException();

            _baseUser = user;
            return await CreateToken(populateExp: false);
        }


        private ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
        {
            var jwtSettings = _config.GetSection("JwtSettings");
            var secretKey = jwtSettings["secretKey"];

            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = jwtSettings["validIssuer"],
                ValidAudience = jwtSettings["validAudience"],
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey))

            };

            var tokenHandler = new JwtSecurityTokenHandler();

            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken securityToken);

            var jwtSecurityToken = securityToken as JwtSecurityToken;
            if (jwtSecurityToken is null ||
                !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
            {
                throw new SecurityTokenException("Invalid token.");
            }

            return principal;
        }

        #endregion


        public Task<IdentityResult> ConfirmEmail(string Token, string Id)
        {
            throw new NotImplementedException();
        }

    }
}
