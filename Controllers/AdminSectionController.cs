using Microsoft.AspNetCore.Mvc;
using Project_StudentERP.DTOs;
using Project_StudentERP.Interfaces;

namespace Project_StudentERP.Controllers
{
    [ApiController]
    [Route("api/admin")]
    public class AdminSectionController : Controller
    {
        public readonly IAdminSectionService _adminSectionService;
        public readonly ILogger<AdminSectionController> _logger;

        public AdminSectionController(
            IAdminSectionService adminSectionService,
            ILogger<AdminSectionController> logger
        )
        {
            _adminSectionService = adminSectionService;
            _logger = logger;
        }

        [HttpPost("add/class")]
        public IActionResult AddClass([FromBody] ClassAddRequestDTO dto)
        {
            try
            {
                var res = _adminSectionService.AddClass(dto);
                return StatusCode(res.Status, res);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while adding class.");
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        [HttpGet("get/classes")]
        public IActionResult GetAllClasses()
        {
            try
            {
                var res = _adminSectionService.GetAllClasses();
                return Ok(res);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return StatusCode(500, "Internal Server Error");
            }
        }
    }
}
