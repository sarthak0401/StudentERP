using Dapper;
using Microsoft.Data.SqlClient;
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

        public LoginResponseDTO UserLogin(LoginRequestDTO dto)
        {
            try
            {
                using SqlConnection conn = new SqlConnection(
                    _configuration.GetConnectionString("DefaultConnection")
                );
                int resp = conn.ExecuteScalar<int>(
                    "select count(1) from UserLogin where USERNAME=@username AND PSWD=@password",
                    new { username = dto.USERNAME, password = dto.PSWD }
                );

                if (resp > 0)
                {
                    return new LoginResponseDTO
                    {
                        StatusCode = 200,
                        Success = true,
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
