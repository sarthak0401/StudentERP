using Dapper;
using Microsoft.Data.SqlClient;
using Project_StudentERP.DTOs;
using Project_StudentERP.Interfaces;
using Project_StudentERP.Models;

namespace Project_StudentERP.Services
{
    public class AdminSectionServiceImpl : IAdminSectionService
    {
        public readonly IConfiguration _configuration;
        public readonly ILogger<AdminSectionServiceImpl> _logger;

        public AdminSectionServiceImpl(
            IConfiguration configuration,
            ILogger<AdminSectionServiceImpl> logger
        )
        {
            _configuration = configuration;
            _logger = logger;
        }

        public ClassAddResponseDTO AddClass(ClassAddRequestDTO dto)
        {
            try
            {
                using SqlConnection conn = new SqlConnection(
                    _configuration.GetConnectionString("DefaultConnection")
                );

                var param = new
                {
                    cname = dto.ClassN,
                    section = dto.Section,
                    capacity = dto.Capacity,
                    status = dto.Status,
                };
                int rowAffected = conn.Execute(
                    "insert into Classes(CNAME,SECTION,CAPACITY,STATUS) values(@cname, @section, @capacity, @status)",
                    param
                );

                if (rowAffected == 1)
                {
                    return new ClassAddResponseDTO
                    {
                        Status = 200,
                        Message = "Insertion successfull",
                        Success = true,
                    };
                }
                else
                {
                    return new ClassAddResponseDTO
                    {
                        Status = 500,
                        Message = "Internal server error",
                        Success = false,
                    };
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return new ClassAddResponseDTO
                {
                    Status = 500,
                    Message = "Internal server error",
                    Success = false,
                };
            }
        }

        public List<ClassModel> GetAllClasses()
        {
            try
            {
                using SqlConnection conn = new SqlConnection(
                    _configuration.GetConnectionString("DefaultConnection")
                );

                List<ClassModel> allClasses = conn.Query<ClassModel>("select * from Classes")
                    .ToList();

                return allClasses;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return null;
            }
        }
    }
}
