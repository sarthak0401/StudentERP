using System.Data;
using System.Data.Common;
using System.Transactions;
using System.Xml;
using System.Xml.Linq;
using Dapper;
using Microsoft.Data.SqlClient;
using Project_StudentERP.DTOs;
using Project_StudentERP.DTOs.DTOs_new;
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

                DataTable dt = new DataTable();
                dt.Columns.Add("SectionId", typeof(int));

                foreach (var x in dto.SectionIds)
                {
                    dt.Rows.Add(x);
                }

                var param = new DynamicParameters();
                param.Add("@ClassName", dto.ClassName);
                param.Add("@SectionIds", dt.AsTableValuedParameter("SectionIdsType"));
                param.Add("@Capacity", dto.Capacity);
                param.Add("@IsActive", dto.Status);

                conn.Execute("sp_addClass", param, commandType: CommandType.StoredProcedure);

                return new ClassAddResponseDTO
                {
                    Status = 200,
                    Message = "Insertion successfull",
                    Success = true,
                };
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

        public async Task<AddFeeStructureAmountResponseDTO> AddFeeStructure(
            AddFeeStructureAmountRequestDTO dto
        )
        {
            if (dto.FeeStructures == null || !dto.FeeStructures.Any())
            {
                return new AddFeeStructureAmountResponseDTO
                {
                    Success = false,
                    Status = 400,
                    Message = "No fee structures provided",
                };
            }

            DataTable dt = new DataTable();
            dt.Columns.Add("StandardFeeMapId", typeof(int));
            dt.Columns.Add("Amount", typeof(decimal));

            foreach (var x in dto.FeeStructures)
            {
                dt.Rows.Add(x.StandardFeeMapId, x.Amount);
            }

            try
            {
                using SqlConnection conn = new SqlConnection(
                    _configuration.GetConnectionString("DefaultConnection")
                );
                await conn.OpenAsync();

                using SqlTransaction transaction = conn.BeginTransaction();
                try
                {
                    var param = new DynamicParameters();
                    param.Add("@FeeStructures", dt.AsTableValuedParameter("FeeStructureType"));

                    await conn.ExecuteAsync(
                        "sp_SaveFeeStructure",
                        param,
                        transaction,
                        commandType: CommandType.StoredProcedure
                    );

                    await transaction.CommitAsync();

                    return new AddFeeStructureAmountResponseDTO
                    {
                        Success = true,
                        Status = 200,
                        Message = "Fee structure added successfully",
                    };
                }
                catch
                {
                    transaction.Rollback();
                    throw;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);

                return new AddFeeStructureAmountResponseDTO
                {
                    Success = false,
                    Status = 500,
                    Message = "Internal server error",
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

        public AddClassFeeTypeMappingResponseDTO AddClassSectionFeeMapping(
            AddClassFeeTypeMappingRequestDTO dto
        )
        {
            using SqlConnection conn = new SqlConnection(
                _configuration.GetConnectionString("DefaultConnection")
            );

            conn.Open();

            using var transaction = conn.BeginTransaction();
            try
            {
                // See in the schema we have on delete cascade for FeeStructureCls so when the entry in parent table i.e, ClassFeeType gets deleted, automatically that linked entries in child table will get deleted as well

                conn.Execute(
                    "DELETE FROM ClassSectionFeeType WHERE CSId = @ClassSectionId",
                    new { dto.ClassSectionId },
                    transaction
                );

                foreach (var x in dto.feeMappings)
                {
                    Console.WriteLine("ClassId " + dto.ClassSectionId);
                    Console.WriteLine("FeeId " + x.feeTypeId);
                    Console.WriteLine("FeeAmount " + x.amount);
                    Console.WriteLine("---------------");

                    conn.Execute(
                        "sp_addClassSectionFeeTypes",
                        new
                        {
                            csid = dto.ClassSectionId,
                            ftid = x.feeTypeId,
                            amt = x.amount,
                        },
                        transaction,
                        commandType: CommandType.StoredProcedure
                    );
                }

                transaction.Commit();

                return new AddClassFeeTypeMappingResponseDTO
                {
                    Status = 200,
                    Success = true,
                    Message = "Mapping done successfully",
                };
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                _logger.LogError(ex, ex.Message);
                return new AddClassFeeTypeMappingResponseDTO
                {
                    Status = 500,
                    Success = false,
                    Message = "Internal server error",
                };
            }
            //try
            //{
            //    using SqlConnection conn = new SqlConnection(
            //        _configuration.GetConnectionString("DefaultConnection")
            //    );

            //    var param = new DynamicParameters();
            //    param.Add("@StdId", dto.StdId);
            //    param.Add("@FeeTypes", dt.AsTableValuedParameter("StdFeeTypes"));

            //    int rowsAffected = conn.Execute(
            //        "sp_addStdFeeTypes",
            //        param,
            //        commandType: CommandType.StoredProcedure
            //    );

            //    if (rowsAffected > 0)
            //    {
            //        return new AddStdFeeTypeMappingResponseDTO
            //        {
            //            Status = 200,
            //            Success = true,
            //            Message = "Mapping done successfully",
            //        };
            //    }
            //    else
            //    {
            //        return new AddStdFeeTypeMappingResponseDTO
            //        {
            //            Status = 400,
            //            Success = false,
            //            Message = "Mapping failed!",
            //        };
            //    }
            //}
            //catch (Exception ex)
            //{
            //    _logger.LogError(ex, ex.Message);
            //    return new AddStdFeeTypeMappingResponseDTO
            //    {
            //        Status = 500,
            //        Message = "Internal Server error",
            //        Success = false,
            //    };
            //}
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
                    "sp_addSubject",
                    param,
                    commandType: CommandType.StoredProcedure
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

            parameters.Add("@ClassSectionId", dto.classSectionId);

            parameters.Add("@Subjects", dt.AsTableValuedParameter("SubjectIdTableType"));

            try
            {
                using SqlConnection conn = new SqlConnection(
                    _configuration.GetConnectionString("DefaultConnection")
                );

                conn.Execute(
                    "sp_AssignSubjectstToClassSection",
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
                    "sp_deleteFeeType",
                    new { Id = id },
                    commandType: CommandType.StoredProcedure
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

                List<ClassModel> allClasses = conn.Query<ClassModel>(
                        "sp_getAllClasses",
                        commandType: CommandType.StoredProcedure
                    )
                    .ToList();

                return allClasses;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return new List<ClassModel>();
            }
        }

        public List<FeeType> GetAllFeeTypes(int? csid)
        {
            try
            {
                using SqlConnection conn = new SqlConnection(
                    _configuration.GetConnectionString("DefaultConnection")
                );

                var res = conn.Query<FeeType>(
                        "sp_getAllFeeTypesSelectedForAParticularClsSection",
                        new { csid = csid },
                        commandType: CommandType.StoredProcedure
                    )
                    .ToList();

                return res;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return new List<FeeType>();
            }
        }

        public List<FeeTypeForParticularStd> GetAllFeeTypesForParticularStdId(int id)
        {
            try
            {
                using SqlConnection conn = new SqlConnection(
                    _configuration.GetConnectionString("DefaultConnection")
                );

                var res = conn.Query<FeeTypeForParticularStd>(
                        "sp_getFeeTypesForParticularStd",
                        new { StdId = id },
                        commandType: CommandType.StoredProcedure
                    )
                    .ToList();

                return res;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return new List<FeeTypeForParticularStd>();
            }
        }

        public GetSectionsDTO GetAllSections()
        {
            try
            {
                using SqlConnection conn = new SqlConnection(
                    _configuration.GetConnectionString("DefaultConnection")
                );

                var res = conn.Query<Sections>("select * from SectionMaster").ToList();

                return new GetSectionsDTO
                {
                    Status = 200,
                    Success = true,
                    sections = res,
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return new GetSectionsDTO
                {
                    Status = 500,
                    Success = false,
                    Message = "Internal server error",
                };
            }
        }

        public List<StandardResponseDTO> GetAllStandards()
        {
            try
            {
                using SqlConnection conn = new SqlConnection(
                    _configuration.GetConnectionString("DefaultConnection")
                );

                var res = conn.Query<StandardResponseDTO>(
                        "select * from Standards order by StandardId"
                    )
                    .ToList();

                return res;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return new List<StandardResponseDTO>();
            }
        }

        public GetAllSubjectsResponseDTO GetAllSubjects(int? csid)
        {
            try
            {
                using SqlConnection conn = new SqlConnection(
                    _configuration.GetConnectionString("DefaultConnection")
                );

                var res = conn.Query<SubjectModel>(
                        "sp_getAllSubjects",
                        new { csid = csid == null ? null : csid },
                        commandType: CommandType.StoredProcedure
                    )
                    .ToList();

                return new GetAllSubjectsResponseDTO
                {
                    Success = true,
                    Status = 200,
                    Message = "Get subjects successfull",
                    Subjects = res,
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return new GetAllSubjectsResponseDTO
                {
                    Status = 500,
                    Success = false,
                    Message = "Internal Server error",
                };
            }
        }

        public DeleteSubjectResponseDTO DeleteSubject(int id)
        {
            try
            {
                using SqlConnection conn = new SqlConnection(
                    _configuration.GetConnectionString("DefaultConnection")
                );

                int rowAffected = conn.Execute(
                    "sp_deleteSubject",
                    new { Id = id },
                    commandType: CommandType.StoredProcedure
                );

                if (rowAffected > 0)
                {
                    return new DeleteSubjectResponseDTO
                    {
                        Success = true,
                        Status = 200,
                        Message = "Deletion successfull",
                    };
                }
                else
                {
                    return new DeleteSubjectResponseDTO
                    {
                        Success = false,
                        Status = 400,
                        Message = "Deletion unsuccessfull, bad request",
                    };
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return new DeleteSubjectResponseDTO
                {
                    Success = false,
                    Status = 500,
                    Message = "Internal server error",
                };
            }
        }
    }
}
