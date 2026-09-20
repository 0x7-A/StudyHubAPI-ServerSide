using Microsoft.IdentityModel.Tokens;
using StudyHubAPI.Models.DTOs.Auth;
using StudyHubAPI.Models.Entities;
using StudyHubAPI.Models.Enums;
using StudyHubAPI.Repositories;
using StudyHubAPI.Utils;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace StudyHubAPI.Services
{
    public class AuthService
    {
        private readonly PersonRepository _personRepository;
        private readonly IConfiguration _configuration;

        public AuthService(PersonRepository personRepository, IConfiguration configuration)
        {
            _personRepository = personRepository;
            _configuration = configuration;
        }

        private static string GenerateRefreshToken()
        {
            var bytes = new byte[64];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(bytes);
            return Convert.ToBase64String(bytes);
        }

        private static string HashToken(string token)
        {
            using var sha256 = SHA256.Create();
            var hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(token));
            return Convert.ToBase64String(hashBytes);
        }

        private string GenerateJwtToken(int id, string email, PersonRole role)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, id.ToString()),
                new Claim(ClaimTypes.Email, email),
                new Claim(ClaimTypes.Role, role.ToString())
            };

            var secretKey = Environment.GetEnvironmentVariable("MY_API_TOKEN");
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"] ?? "StudyHubApi",
                audience: _configuration["Jwt:Audience"] ?? "StudyHubApiUsers",
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(15),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public async Task<LoginResponseDto?> Login(LoginRequestDto request)
        {
            var person = await _personRepository.GetPersonByEmail(request.Email);
            if (person == null || !PasswordHasher.VerifyPassword(request.Password, person.PasswordHash))
            {
                return null;
            }

         
            var accessToken = GenerateJwtToken(person.PersonID, person.Email, person.Role);
            var rawRefreshToken = GenerateRefreshToken();


            var refreshTokenEntity = new RefreshToken
            {
                TokenHash = HashToken(rawRefreshToken),
                ExpiresAt = DateTime.UtcNow.AddDays(7),
                CreatedAt = DateTime.UtcNow,
                RevokedAt = null,
                PersonID = person.PersonID
            };

         
            await _personRepository.AddRefreshTokenAsync(refreshTokenEntity);


            return new LoginResponseDto
            {
                PersonID = person.PersonID,
                FullName = $"{person.FirstName} {person.LastName}",
                Role = person.Role,
                AccessToken = accessToken,
                RefreshToken = rawRefreshToken
            };
        }


        public async Task<TokenResponse?> RefreshTokenAsync(RefreshRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.RefreshToken))
            {
                return null;
            }

            var tokenHash = HashToken(request.RefreshToken);
            var storedToken = await _personRepository.GetRefreshTokenByHashAsync(tokenHash);

            // Check if token exists and is valid (not expired, not revoked)
            if (storedToken == null || !storedToken.IsActive)
            {
                return null;
            }

            // Revoke old token
            await _personRepository.UpdateRefreshTokenAsync(storedToken);

            // Generate new Access and Refresh tokens
            var person = storedToken.Person;
            var newAccessToken = GenerateJwtToken(person.PersonID, person.Email, person.Role);
            var newRawRefreshToken = GenerateRefreshToken();

            var newRefreshTokenEntity = new RefreshToken
            {
                TokenHash = HashToken(newRawRefreshToken),
                ExpiresAt = DateTime.UtcNow.AddDays(7),
                CreatedAt = DateTime.UtcNow,
                RevokedAt = null,
                PersonID = person.PersonID
            };

            await _personRepository.AddRefreshTokenAsync(newRefreshTokenEntity);

            return new TokenResponse
            {
                AccessToken = newAccessToken,
                RefreshToken = newRawRefreshToken
            };
        }

        public async Task<bool> LogoutAsync(LogoutRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.RefreshToken))
            {
                return false;
            }

            var tokenHash = HashToken(request.RefreshToken);
            var storedToken = await _personRepository.GetRefreshTokenByHashAsync(tokenHash);

            if (storedToken == null || !storedToken.IsActive)
            {
                return false;
            }

     
            storedToken.RevokedAt = DateTime.UtcNow;
            await _personRepository.UpdateRefreshTokenAsync(storedToken);

            return true;
        }

    }
}