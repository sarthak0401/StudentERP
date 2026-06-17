using System.Data;
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

        [HttpPost("add/feeType")]
        public IActionResult AddFeeType([FromBody] AddFeeTypeRequestDTO dto)
        {
            try
            {
                var res = _adminSectionService.AddFeeType(dto);
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

        [HttpGet("get/feeType/{csid?}")]
        public IActionResult GetAllFeeTypes(int? csid)
        {
            try
            {
                var res = _adminSectionService.GetAllFeeTypes(csid);
                return StatusCode(
                    200,
                    new
                    {
                        success = true,
                        status = 200,
                        FeeTypes = res,
                    }
                );
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

        [HttpDelete("del/feeType/{id}")]
        public async Task<IActionResult> DeleteFeeType(int id)
        {
            try
            {
                var res = await _adminSectionService.DeleteFeeType(id);
                return StatusCode(
                    res.Status,
                    new
                    {
                        success = res.Success,
                        status = res.Status,
                        Message = res.Message,
                    }
                );
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
                        message = "Internal Server Error",
                    }
                );
            }
        }

        [HttpGet("get/standards")]
        public IActionResult GetAllStandards()
        {
            var res = _adminSectionService.GetAllStandards();
            return StatusCode(
                200,
                new
                {
                    Status = 200,
                    Success = true,
                    Standards = res,
                }
            );
        }

        [HttpPost("add/classSectionFeeMap")]
        public IActionResult AddStdFeeMapping([FromBody] AddClassFeeTypeMappingRequestDTO dto)
        {
            try
            {
                var res = _adminSectionService.AddClassSectionFeeMapping(dto);
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
                        Message = "Internal server error",
                    }
                );
            }
        }

        [HttpGet("get/feeTypes/StdId/{id}")]
        public IActionResult GetAllFeeTypesForParticularStdId(int id)
        {
            try
            {
                var res = _adminSectionService.GetAllFeeTypesForParticularStdId(id);
                return Ok(
                    new
                    {
                        Status = 200,
                        Success = true,
                        FeeTypes = res,
                    }
                );
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
                        Message = "Internal server error",
                    }
                );
            }
        }

        [HttpPost("add/feeStructure")]
        public async Task<IActionResult> AddFeeStructure(
            [FromBody] AddFeeStructureAmountRequestDTO dto
        )
        {
            try
            {
                var res = await _adminSectionService.AddFeeStructure(dto);
                return StatusCode((int)res.Status, res);
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
                        Message = "Internal Server error",
                    }
                );
            }
        }

        [HttpGet("get/sections")]
        public IActionResult GetAllSections()
        {
            var res = _adminSectionService.GetAllSections();
            return StatusCode(res.Status, res);
        }
    }
}
