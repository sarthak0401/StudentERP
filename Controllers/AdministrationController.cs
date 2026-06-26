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

        [HttpGet("student/contacts/{id}")]
        public IActionResult GetAllContactsOfStudent(int id)
        {
            var contacts = _administrationService.getAllContacts(id);
            return StatusCode(200, new { Success = true, Contacts = contacts });
        }

        [HttpGet("get/classSectionFeeType/{id}")]
        public IActionResult GetAllFeeTypesForParticularClassSection(int id)
        {
            try
            {
                var res = _administrationService.GetAllFeeTypesForParticularClassSection(id);
                return StatusCode(res.Status, res);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return StatusCode(
                    500,
                    new
                    {
                        Success = false,
                        Status = 500,
                        Message = "Internal Server error",
                    }
                );
            }
        }

        [HttpPost("student/addmission")]
        public IActionResult StudentAdmission([FromBody] AdmissionStudentRequestDTO dto)
        {
            try
            {
                var res = _administrationService.StudentAdmission(dto);
                return StatusCode(res.Status, res);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return StatusCode(
                    500,
                    new
                    {
                        Status = 500,
                        Success = false,
                        Message = "Internal Server Error",
                    }
                );
            }
        }

        [HttpGet("get/fee/student/{id}")]
        public IActionResult GetAllFeeForAlreadyAdmittedStudent(int id)
        {
            try
            {
                var res = _administrationService.GetAllFeeInfoForAlreadyAdmittedStudent(id);
                return StatusCode(res.Status, res);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return StatusCode(
                    500,
                    new
                    {
                        Status = 500,
                        Success = false,
                        Message = "Internal Server Error",
                    }
                );
            }
        }

        [HttpPost("update/feeDetails/student")]
        public IActionResult UpdateFeeDetailsOfAlreadyAdmittedStudent(
            [FromBody] UpdateFeeDetailsStudentRequestDTO dto
        )
        {
            try
            {
                var res = _administrationService.UpdateFeeDetailsOfAlreadyAdmittedStudent(dto);
                return StatusCode(res.Status, res);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return StatusCode(
                    500,
                    new
                    {
                        Status = 500,
                        Success = false,
                        Message = "Internal Server Error",
                    }
                );
            }
        }

        [HttpGet("get/balanceFee/student/{id}")]
        public IActionResult GetBalanceFeeForAdmittedStudent(int id)
        {
            try
            {
                var res = _administrationService.GetBalanceFeeForAdmittedStudent(id);
                return StatusCode(res.Status, res);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return StatusCode(
                    500,
                    new
                    {
                        Status = 500,
                        Success = false,
                        Message = "Internal Server Error",
                    }
                );
            }
        }

        [HttpPost("update/balance/fee/student")]
        public async Task<IActionResult> UpdateFeeBalanceForAdmittedStudent(
            [FromBody] UpdateFeeBalanceRequestDTO dto
        )
        {
            try
            {
                var res = await _administrationService.UpdateFeeBalanceForAdmittedStudent(dto);
                return StatusCode(res.Status, res);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return StatusCode(
                    500,
                    new
                    {
                        Success = false,
                        Status = 500,
                        Message = "Internal Server Error",
                    }
                );
            }
        }

        [HttpGet("download/receipt/{receiptId}")]
        public async Task<IActionResult> DownloadReceipt(int receiptId)
        {
            try
            {
                var pdfBytes = await _administrationService.GenerateReceipt(receiptId);
                return File(pdfBytes, "application/pdf", $"receipt_{receiptId}.pdf");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return StatusCode(
                    500,
                    new
                    {
                        Success = false,
                        Status = 500,
                        Message = "Internal Server Error",
                    }
                );
            }
        }

        [HttpGet("get/receipts/student/{id}")]
        public IActionResult GetAllReceipts(int id)
        {
            try
            {
                var res = _administrationService.GetAllReceipts(id);
                return StatusCode(res.Status, res);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return StatusCode(
                    500,
                    new
                    {
                        Success = false,
                        Status = 500,
                        Message = "Internal Server Error",
                    }
                );
            }
        }
    }
}
