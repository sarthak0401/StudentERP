using System.Data;
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

        public AddFeeTypeResponseDTO AddFeeType(AddFeeTypeRequestDTO dto)
        {
            try
            {
                using SqlConnection conn = new SqlConnection(
                    _configuration.GetConnectionString("DefaultConnection")
                );

                int res = conn.Execute(
                    "sp_addOrEditFeeType",
                    new
                    {
                        @Id = dto.FeeTypeId == null ? null : dto.FeeTypeId,
                        @Name = dto.FeeTypeName,
                    },
                    commandType: CommandType.StoredProcedure
                );

                if (res > 0)
                {
                    return new AddFeeTypeResponseDTO
                    {
                        Status = 200,
                        Success = true,
                        Message = "Operation Successfull!",
                    };
                }
                return new AddFeeTypeResponseDTO
                {
                    Status = 400,
                    Success = false,
                    Message = "Operation Unsuccessfull!",
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return new AddFeeTypeResponseDTO
                {
                    Status = 500,
                    Success = false,
                    Message = "Internal Server Error",
                };
            }
        }

        public SubjectAddResponseDTO AddSubject(SubjectAddRequestDTO dto)
        {
            try
            {
                using SqlConnection conn = new SqlConnection(
                    _configuration.GetConnectionString("DefaultConnection")
                );

                var param = new
                {
                    sname = dto.SubjectName,
                    scode = dto.SubjectCode,
                    scredits = dto.SubjectCredits,
                };

                int rowsAffected = conn.Execute(
                    "insert into Subjects(SNAME, SCODE, SCREDITS) values (@sname, @scode, @scredits)",
                    param
                );

                if (rowsAffected > 0)
                {
                    return new SubjectAddResponseDTO
                    {
                        Message = "Subject added successfully",
                        Status = 200,
                        Success = true,
                    };
                }

                return new SubjectAddResponseDTO
                {
                    Message = "Failed to add subject",
                    Status = 404,
                    Success = false,
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return new SubjectAddResponseDTO
                {
                    Message = "Internal server error",
                    Status = 500,
                    Success = false,
                };
            }
        }

        public AssignSubjectResponseDTO AddSubjectsToClass(AssignSubjectDTO dto)
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("SubjectId", typeof(int));

            foreach (var item in dto.subjectIds)
            {
                dt.Rows.Add(item);
            }

            var parameters = new DynamicParameters();

            parameters.Add("@ClassId", dto.classId);

            parameters.Add("@Subjects", dt.AsTableValuedParameter("SubjectIdTableType"));

            try
            {
                using SqlConnection conn = new SqlConnection(
                    _configuration.GetConnectionString("DefaultConnection")
                );

                conn.Execute(
                    "sp_AssignSubjectstToClass",
                    parameters,
                    commandType: CommandType.StoredProcedure
                );

                return new AssignSubjectResponseDTO
                {
                    Status = 200,
                    Message = "Subjects mapped to class successfully",
                    Success = true,
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return new AssignSubjectResponseDTO
                {
                    Status = 500,
                    Message = "Internal server error",
                    Success = false,
                };
            }
        }

        public async Task<bool> DeleteClassById(int id)
        {
            try
            {
                using SqlConnection conn = new SqlConnection(
                    _configuration.GetConnectionString("DefaultConnection")
                );

                int rowAffected = await conn.ExecuteAsync(
                    "delete from Classes where CID = @id",
                    new { id = id }
                );

                return rowAffected > 0;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task<DeleteFeeTypeResponseDTO> DeleteFeeType(int id)
        {
            try
            {
                using SqlConnection conn = new SqlConnection(
                    _configuration.GetConnectionString("DefaultConnection")
                );

                int rowAffected = await conn.ExecuteAsync(
                    "delete from FeeType where FeeTypeId = @Id",
                    new { Id = id }
                );

                if (rowAffected > 0)
                {
                    return new DeleteFeeTypeResponseDTO
                    {
                        Status = 200,
                        Message = "Deletion successfull",
                        Success = true,
                    };
                }
                return new DeleteFeeTypeResponseDTO
                {
                    Status = 400,
                    Message = "Bad request!",
                    Success = false,
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return new DeleteFeeTypeResponseDTO
                {
                    Status = 500,
                    Message = "Internal Server Error",
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
                return new List<ClassModel>();
            }
        }

        public List<FeeType> GetAllFeeTypes()
        {
            try
            {
                using SqlConnection conn = new SqlConnection(
                    _configuration.GetConnectionString("DefaultConnection")
                );

                var res = conn.Query<FeeType>("select * from FeeType").ToList();

                return res;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return new List<FeeType>();
            }
        }

        public List<SubjectModel> GetAllSubjects()
        {
            try
            {
                using SqlConnection conn = new SqlConnection(
                    _configuration.GetConnectionString("DefaultConnection")
                );

                var res = conn.Query<SubjectModel>("select * from Subjects").ToList();

                return res;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return new List<SubjectModel>();
            }
        }
    }
}
