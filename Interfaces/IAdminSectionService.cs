using System.Data;
using Microsoft.AspNetCore.Mvc;
using Project_StudentERP.DTOs;
using Project_StudentERP.Models;

namespace Project_StudentERP.Interfaces
{
    public interface IAdminSectionService
    {
        ClassAddResponseDTO AddClass(ClassAddRequestDTO dto);
        Task<AddFeeStructureAmountResponseDTO> AddFeeStructure(AddFeeStructureAmountRequestDTO dto);
        AddFeeTypeResponseDTO AddFeeType(AddFeeTypeRequestDTO dto);
        AddStdFeeTypeMappingResponseDTO AddStdFeeMapping(AddStdFeeTypeMappingRequestDTO dto);
        SubjectAddResponseDTO AddSubject(SubjectAddRequestDTO dto);
        AssignSubjectResponseDTO AddSubjectsToClass(AssignSubjectDTO dto);
        Task<bool> DeleteClassById(int id);
        Task<DeleteFeeTypeResponseDTO> DeleteFeeType(int id);
        List<ClassModel> GetAllClasses();
        List<FeeType> GetAllFeeTypes();
        List<FeeTypeForParticularStd> GetAllFeeTypesForParticularStdId(int id);
        List<StandardResponseDTO> GetAllStandards();
        List<SubjectModel> GetAllSubjects();
    }
}
