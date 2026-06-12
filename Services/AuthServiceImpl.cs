using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography.Xml;
using System.Text;
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.IdentityModel.Tokens;
using Project_StudentERP.DTOs;
using Project_StudentERP.Interfaces;

namespace Project_StudentERP.Services
{
    public class AuthServiceImpl : IAuthService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<AuthServiceImpl> _logger;

        public AuthServiceImpl(IConfiguration configuration, ILogger<AuthServiceImpl> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public string GenerateJwtToken(string userId, string userRole)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, userId),
                new Claim(ClaimTypes.Role, userRole),
            };

            var token = new JwtSecurityToken(
                claims: claims,
                signingCredentials: creds,
                expires: DateTime.Now.AddMinutes(15)
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public LoginResponseDTO UserLogin(LoginRequestDTO dto)
        {
            try
            {
                Console.WriteLine("Reached auth service layer ");
                using SqlConnection conn = new SqlConnection(
                    _configuration.GetConnectionString("DefaultConnection")
                );
                var resp = conn.QueryFirst(
                    "select PKID, ROLE from UserLogin where USERNAME=@username AND PSWD=@password",
                    new { username = dto.USERNAME, password = dto.PSWD }
                );

                Console.WriteLine(resp.PKID);
                Console.WriteLine(resp.ROLE);

                if (resp != null)
                {
                    return new LoginResponseDTO
                    {
                        StatusCode = 200,
                        Success = true,
                        AccessToken = GenerateJwtToken(
                            Convert.ToString(resp.PKID),
                            Convert.ToString(resp.ROLE)
                        ),
                        Message = "User Logged in successfully",
                    };
                }
                return new LoginResponseDTO
                {
                    StatusCode = 401,
                    Success = false,
                    Message = "Bad Credentials",
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return new LoginResponseDTO
                {
                    StatusCode = 500,
                    Success = false,
                    Message = "Internal server error",
                };
            }
        }
    }
}
