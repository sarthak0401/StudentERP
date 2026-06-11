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

                var dt = new DataTable();
                dt.Columns.Add("ContactNo");
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

        public List<Student> GetStudentsOnSearch(StudentSearchDTO studentSearchDTO)
        {
            try
            {
                using SqlConnection conn = new SqlConnection(
                    _configuration.GetConnectionString("DefaultConnection")
                );

                var param = new
                {
                    FDate = studentSearchDTO.FDate,
                    EDate = studentSearchDTO.EDate,
                    SearchText = studentSearchDTO.SearchText,
                };

                var students = conn.Query<Student>(
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
    }
}
