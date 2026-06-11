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

        [HttpPost("login")]
        public IActionResult UserLogin([FromBody] LoginRequestDTO dto)
        {
            try
            {
                var response = _authService.UserLogin(dto);
                if (response.Success)
                {
                    // Setting the cookie here, since the user credentials are validated
                    Response.Cookies.Append(
                        "accessToken",
                        response.AccessToken,
                        new CookieOptions
                        {
                            HttpOnly = true,
                            Secure = true, 
                            SameSite = SameSiteMode.Strict,
                            Expires = DateTime.UtcNow.AddMinutes(15)
                        }
                    );


                   return StatusCode(
                       response.StatusCode,
                       new { AccessToken = response.AccessToken, Success = response.Success, Message = response.Message }
                   );
                }

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
