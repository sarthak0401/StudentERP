using System.Transactions;
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

        [HttpDelete("del/class/{id}")]
        public async Task<IActionResult> DeleteClass(int id)
        {
            try
            {
                bool ans = await _adminSectionService.DeleteClassById(id);
                if (ans)
                {
                    return Ok(new { success = true, message = "Deleted successfully!" });
                }
                else
                {
                    return StatusCode(
                        404,
                        new { success = false, message = "Resource with this Id not found" }
                    );
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return StatusCode(500, new { success = false, message = "Internal Server error" });
            }
        }

        [HttpPost("add/subject")]
        public IActionResult AddSubject([FromBody] SubjectAddRequestDTO dto)
        {
            try
            {
                var response = _adminSectionService.AddSubject(dto);
                if (response.Success)
                {
                    return Ok(response);
                }
                else
                {
                    return BadRequest(response);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return StatusCode(
                    500,
                    new
                    {
                        status = 500,
                        success = false,
                        message = "Internal server error",
                    }
                );
            }
        }

        [HttpGet("get/subjects")]
        public IActionResult GetAllSubjects()
        {
            try
            {
                var res = _adminSectionService.GetAllSubjects();
                return Ok(res);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return StatusCode(
                    500,
                    new
                    {
                        message = "Internal Server Error",
                        success = false,
                        status = 500,
                    }
                );
            }
        }

        [HttpPost("add/subjectClass")]
        public IActionResult AddSubjectsToClass([FromBody] AssignSubjectDTO dto)
        {
            var res = _adminSectionService.AddSubjectsToClass(dto);

            return Ok(res);
        }
    }
}
