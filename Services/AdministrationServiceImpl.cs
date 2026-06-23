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

        public GetFeeForAlreadyAdmittedStudentResponseDTO GetAllFeeInfoForAlreadyAdmittedStudent(
            int id
        )
        {
            try
            {
                using SqlConnection conn = new SqlConnection(
                    _configuration.GetConnectionString("DefaultConnection")
                );

                List<FeeForAdmittedStudent> fmList = conn.Query<FeeForAdmittedStudent>(
                        "sp_getAllSelectedFeeTypesForParticularStudent",
                        new { SId = id },
                        commandType: CommandType.StoredProcedure
                    )
                    .ToList();

                return new GetFeeForAlreadyAdmittedStudentResponseDTO
                {
                    Success = true,
                    Status = 200,
                    FeeLst = fmList,
                    Message = "Successful operation",
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return new GetFeeForAlreadyAdmittedStudentResponseDTO
                {
                    Status = 500,
                    Success = false,
                    Message = "Internal server error",
                };
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

        public FeeBalanceForAdmittedStudentResponseDTO GetBalanceFeeForAdmittedStudent(int id)
        {
            try
            {
                using SqlConnection conn = new SqlConnection(
                    _configuration.GetConnectionString("DefaultConnection")
                );
                List<FeeBalanceForAdmittedStudent> feeList =
                    conn.Query<FeeBalanceForAdmittedStudent>(
                            "sp_getFeeBalanceForAdmittedStudent",
                            new { SId = id },
                            commandType: CommandType.StoredProcedure
                        )
                        .ToList();

                return new FeeBalanceForAdmittedStudentResponseDTO
                {
                    Success = true,
                    Status = 200,
                    Message = "Successfully fetched fee balance",
                    Data = feeList,
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return new FeeBalanceForAdmittedStudentResponseDTO
                {
                    Success = false,
                    Status = 500,
                    Message = "Internal server error",
                };
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
                if (dto.SelectedFeesList == null || !dto.SelectedFeesList.Any())
                {
                    return new AdmissionStudentResponseDTO
                    {
                        Success = false,
                        Status = 400,
                        Message = "At least one fee must be selected.",
                    };
                }

                using SqlConnection conn = new SqlConnection(
                    _configuration.GetConnectionString("DefaultConnection")
                );

                DataTable dt = new DataTable();
                dt.Columns.Add("FtId", typeof(int));
                dt.Columns.Add("FixedAmt", typeof(int));
                dt.Columns.Add("Discount", typeof(float));
                dt.Columns.Add("FinalAmount", typeof(float));

                foreach (var x in dto.SelectedFeesList)
                {
                    dt.Rows.Add(x.FtId, x.FixedAmt, x.Discount, x.FinalAmount);
                    //Console.WriteLine(x.FtId);
                    //Console.WriteLine(x.FixedAmt);
                    //Console.WriteLine(x.Discount);
                    //Console.WriteLine(x.FinalAmount);
                }

                var param = new DynamicParameters();
                param.Add("@StdId", dto.StdId);
                param.Add("@CSId", dto.CSId);
                param.Add("@SelectedFees", dt.AsTableValuedParameter("SelectedFeesType"));

                int AdmissionId = conn.ExecuteScalar<int>(
                    "sp_studentAdmission",
                    param,
                    commandType: CommandType.StoredProcedure
                );

                return new AdmissionStudentResponseDTO
                {
                    Success = true,
                    Status = 200,
                    Message = "Student admitted successfully",
                    AdmissionId = AdmissionId,
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

        public async Task<UpdateFeeBalanceResponseDTO> UpdateFeeBalanceForAdmittedStudent(
            UpdateFeeBalanceRequestDTO dto
        )
        {
            try
            {
                /*
                Console.WriteLine(dto.Remarks);
                Console.WriteLine(dto.TransactionId);
                Console.WriteLine(dto.AdmissionId);
                Console.WriteLine(dto.PaymentType);
                Console.WriteLine(dto.PaymentDate);

                foreach (var x in dto.PaidFeeList)
                {
                    Console.WriteLine(x.AdmissionFeeId + ": " + x.AmountPaid);
                }
                */

                DataTable dt = new DataTable();
                dt.Columns.Add("AdmissionFeeId", typeof(int));
                dt.Columns.Add("AmountPaid", typeof(decimal));

                foreach (var x in dto.PaidFeeList)
                {
                    dt.Rows.Add(x.AdmissionFeeId, x.AmountPaid);
                }

                using SqlConnection conn = new SqlConnection(
                    _configuration.GetConnectionString("DefaultConnection")
                );

                var param = new DynamicParameters();
                param.Add("@AdmissionId", dto.AdmissionId);
                param.Add("@PaymentType", dto.PaymentType);
                param.Add("@PaymentDate", dto.PaymentDate);
                param.Add("@Remark", dto.Remarks);
                param.Add("@TransactionId", dto.TransactionId);
                param.Add("@FeeList", dt.AsTableValuedParameter("FeeBalanceType"));

                await conn.ExecuteAsync(
                    "sp_updateBalanceFeeAmtForAdmittedStudent",
                    param,
                    commandType: CommandType.StoredProcedure
                );

                return new UpdateFeeBalanceResponseDTO
                {
                    Success = true,
                    Status = 200,
                    Message = "Successfully updated the Fee Balance",
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return new UpdateFeeBalanceResponseDTO
                {
                    Status = 500,
                    Success = false,
                    Message = "Internal Server Error",
                };
            }
        }

        public UpdateFeeDetailsStudentResponseDTO UpdateFeeDetailsOfAlreadyAdmittedStudent(
            UpdateFeeDetailsStudentRequestDTO dto
        )
        {
            try
            {
                if (dto.SelectedFees == null || !dto.SelectedFees.Any())
                {
                    return new UpdateFeeDetailsStudentResponseDTO
                    {
                        Success = false,
                        Status = 400,
                        Message = "At least one fee must be selected.",
                    };
                }

                using SqlConnection conn = new SqlConnection(
                    _configuration.GetConnectionString("DefaultConnection")
                );

                DataTable dt = new DataTable();
                dt.Columns.Add("FtId", typeof(int));
                dt.Columns.Add("FixedAmt", typeof(int));
                dt.Columns.Add("Discount", typeof(float));
                dt.Columns.Add("FinalAmount", typeof(float));

                foreach (var x in dto.SelectedFees)
                {
                    dt.Rows.Add(x.FtId, x.FixedAmt, x.Discount, x.FinalAmount);
                    //Console.WriteLine(x.FtId);
                    //Console.WriteLine(x.FixedAmt);
                    //Console.WriteLine(x.Discount);
                    //Console.WriteLine(x.FinalAmount);
                }

                var param = new DynamicParameters();
                param.Add("@AdmId", dto.AdmissionId);
                param.Add("@SelectedFees", dt.AsTableValuedParameter("SelectedFeesType"));

                conn.Execute(
                    "sp_updateAdmittedStudentFee",
                    param,
                    commandType: CommandType.StoredProcedure
                );

                return new UpdateFeeDetailsStudentResponseDTO
                {
                    Success = true,
                    Status = 200,
                    Message = "Student admitted successfully",
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return new UpdateFeeDetailsStudentResponseDTO
                {
                    Status = 500,
                    Message = "Internal Server Error",
                    Success = false,
                };
            }
        }
    }
}
