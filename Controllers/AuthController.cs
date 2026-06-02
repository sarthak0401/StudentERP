using Microsoft.AspNetCore.Mvc;
using Project_StudentERP.DTOs;
using Project_StudentERP.Interfaces;

namespace Project_StudentERP.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        public readonly IAuthService _authService;
        public readonly ILogger<AuthController> _logger;

        public AuthController(IAuthService service, ILogger<AuthController> logger)
        {
            _authService = service;
            _logger = logger;
        }

        [HttpGet("/login")]
        public IActionResult UserLogin([FromBody] LoginRequestDTO dto)
        {
            try
            {
                var response = _authService.UserLogin(dto);
                return StatusCode(
                    response.StatusCode,
                    new { Success = response.Success, Message = response.Message }
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return StatusCode(500, new { Success = false, Message = "Internal Server error" });
            }
        }
    }
}
