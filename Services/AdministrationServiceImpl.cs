using System.Data;
using Dapper;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Data.SqlClient;
using Project_StudentERP.DTOs;
using Project_StudentERP.Interfaces;
using Project_StudentERP.Models;

namespace Project_StudentERP.Services
{
    public class AdministrationServiceImpl : IAdministrationService
    {
        public readonly ILogger<AdministrationServiceImpl> _logger;
        public readonly IConfiguration _configuration;

        public AdministrationServiceImpl(
            ILogger<AdministrationServiceImpl> logger,
            IConfiguration configuration
        )
        {
            _logger = logger;
            _configuration = configuration;
        }

        public async Task AddStudent(StudentDTO dto, string filename, int tId)
        {
            try
            {
                using var connection = new SqlConnection(
                    _configuration.GetConnectionString("DefaultConnection")
                );

                _logger.LogInformation("Student Id: {Id}", dto.SId);

                var dt = new DataTable();
                dt.Columns.Add("ContactNo");
                foreach (var contact in dto.ContactNos)
                {
                    _logger.LogInformation("Contact: {Contact}", contact.ContactNo);
                }

                foreach (var item in dto.ContactNos)
                {
                    dt.Rows.Add(item.ContactNo);
                }

                var parameters = new DynamicParameters();

                parameters.Add("@ContactNos", dt.AsTableValuedParameter("StudentContactType"));
                parameters.Add("@SId", dto.SId);
                parameters.Add("@SName", dto.SName);
                parameters.Add("@SAge", dto.SAge);
                parameters.Add("@SDOB", dto.SDOB);
                parameters.Add("@SGender", dto.SGender);
                parameters.Add("@SBloodGrp", dto.SBloodGrp);

                parameters.Add("@SImg", filename);

                parameters.Add("@SFatherName", dto.SFatherName);
                parameters.Add("@SMotherName", dto.SMotherName);

                parameters.Add("@SGuardianContact", dto.SGuardianContact);
                parameters.Add("@SGuardianAltContact", dto.SGuardianAltContact);

                parameters.Add("@SAddress", dto.SAddress);
                parameters.Add("@SAddressCity", dto.SAddressCity);
                parameters.Add("@SAddressState", dto.SAddressState);
                parameters.Add("@SAddressPincode", dto.SAddressPincode);

                parameters.Add("@S10thPctg", dto.S10thPctg);
                parameters.Add("@S12thPctg", dto.S12thPctg);

                parameters.Add("@SJeeRank", dto.SJeeRank);

                parameters.Add("@SJuniorClgName", dto.SJuniorClgName);

                parameters.Add("@SGapYears", dto.SGapYears);
                parameters.Add("@SGapJustification", dto.SGapJustification);
                parameters.Add("@TId", tId);

                await connection.ExecuteAsync(
                    "sp_addOrEditStudent",
                    parameters,
                    commandType: CommandType.StoredProcedure
                );
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public List<string> getAllContacts(int id)
        {
            try
            {
                using SqlConnection conn = new SqlConnection(
                    _configuration.GetConnectionString("DefaultConnection")
                );

                var contacts = conn.Query<string>(
                        "select ContactNo from StudentContact where StudentId = @id",
                        new { id = id }
                    )
                    .ToList();

                return contacts;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return new();
            }
        }

        public GetAllFeeTypesForParticularClassSectionResponseDTO GetAllFeeTypesForParticularClassSection(
            int id
        )
        {
            try
            {
                using SqlConnection conn = new SqlConnection(
                    _configuration.GetConnectionString("DefaultConnection")
                );

                List<RowTemplateForFeeTypesOfParticularClassSection> fmList =
                    conn.Query<RowTemplateForFeeTypesOfParticularClassSection>(
                            "sp_getAllFeeTypesForParticularClassSection",
                            new { CSId = id },
                            commandType: CommandType.StoredProcedure
                        )
                        .ToList();

                return new GetAllFeeTypesForParticularClassSectionResponseDTO
                {
                    Success = true,
                    Status = 200,
                    fmLists = fmList,
                    Message = "Successful operation",
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return new GetAllFeeTypesForParticularClassSectionResponseDTO
                {
                    Status = 500,
                    Success = false,
                    Message = "Internal server error",
                };
            }
        }

        public List<Student>? GetAllStudents()
        {
            try
            {
                using SqlConnection conn = new SqlConnection(
                    _configuration.GetConnectionString("DefaultConnection")
                );

                List<Student> students = conn.Query<Student>("select * from StudentDetails")
                    .ToList();

                return students;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return null;
            }
        }

        public Student GetStudentById(int id)
        {
            try
            {
                using SqlConnection conn = new SqlConnection(
                    _configuration.GetConnectionString("DefaultConnection")
                );
                var param = new { Id = id };
                var student = conn.QuerySingle<Student>("sp_getUserById", param);
                return student;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return null;
            }
        }

        public List<SearchStudentResponseDTO> GetStudentsOnSearch(StudentSearchDTO studentSearchDTO)
        {
            try
            {
                using SqlConnection conn = new SqlConnection(
                    _configuration.GetConnectionString("DefaultConnection")
                );

                var param = new { SearchText = studentSearchDTO.SearchText };

                var students = conn.Query<SearchStudentResponseDTO>(
                        "sp_searchStudent",
                        param,
                        commandType: CommandType.StoredProcedure
                    )
                    .ToList();

                return students;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return null;
            }
        }

        public AdmissionStudentResponseDTO StudentAdmission(AdmissionStudentRequestDTO dto)
        {
            try
            {
                using SqlConnection conn = new SqlConnection(
                    _configuration.GetConnectionString("DefaultConnection")
                );

                return new AdmissionStudentResponseDTO
                {
                    Success = true,
                    Status = 200,
                    Message = "Student admitted successfully",
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return new AdmissionStudentResponseDTO
                {
                    Status = 500,
                    Message = "Internal Server Error",
                    Success = false,
                };
            }
        }
    }
}
