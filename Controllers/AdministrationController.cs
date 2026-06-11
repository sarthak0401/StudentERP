using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Project_StudentERP.DTOs;
using Project_StudentERP.Interfaces;

namespace Project_StudentERP.Controllers
{
    [ApiController]
    [Route("api/adm")]
    public class AdministrationController : Controller
    {
        public readonly IAdministrationService _administrationService;
        public readonly ILogger<AdministrationController> _logger;

        public AdministrationController(
            IAdministrationService administrationService,
            ILogger<AdministrationController> logger
        )
        {
            _administrationService = administrationService;
            _logger = logger;
        }

        [HttpPost("addStudent")]
        //[Authorize(Roles = "TEACHER")]
        public async Task<IActionResult> AddStudent([FromForm] StudentDTO dto)
        {
            try
            {
                // debugging the list of contacts
                foreach (var x in dto.ContactNos)
                {
                    Console.WriteLine(x.ContactNo);
                }

                var filename = dto.ExistingImage;
                if (dto.SImg != null)
                {
                    var folderPath = Path.Combine(
                        Directory.GetCurrentDirectory(),
                        "wwwroot",
                        "studentImages"
                    );

                    // This will create this folder if it doesnt exist, and if it does exist, it will do nothing
                    Directory.CreateDirectory(folderPath);

                    filename = Guid.NewGuid().ToString() + Path.GetExtension(dto.SImg.FileName);

                    var path = Path.Combine(folderPath, filename);

                    using (var stream = new FileStream(path, FileMode.Create))
                    {
                        await dto.SImg.CopyToAsync(stream);
                    }
                }

                //int tId = Convert.ToInt32(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
                int tId = 1;

                await _administrationService.AddStudent(dto, filename, tId);
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return StatusCode(500, new { success = false, message = "Internal server error" });
            }
        }

        [HttpGet("students")]
        public IActionResult getAllStudents()
        {
            try
            {
                var students = _administrationService.GetAllStudents();
                return Ok(students);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return StatusCode(500, new { success = false, message = "Internal server error" });
            }
        }

        [HttpGet("student/{id}")]
        public IActionResult GetStudentById(int id)
        {
            try
            {
                var student = _administrationService.GetStudentById(id);
                return Ok(student);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return StatusCode(500, new { success = false, message = "Internal server error" });
            }
        }

        [HttpPost("student/search")]
        public IActionResult GetStudentsOnSearch([FromBody] StudentSearchDTO studentSearchDto)
        {
            try
            {
                var students = _administrationService.GetStudentsOnSearch(studentSearchDto);
                return Ok(students);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return StatusCode(500, new { success = false, message = "Internal server error" });
            }
        }
    }
}
