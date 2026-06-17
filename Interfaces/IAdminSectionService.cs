using System.Data;
using Microsoft.AspNetCore.Mvc;
using Project_StudentERP.DTOs;
using Project_StudentERP.DTOs.DTOs_new;
using Project_StudentERP.Models;

namespace Project_StudentERP.Interfaces
{
    public interface IAdminSectionService
    {
        ClassAddResponseDTO AddClass(ClassAddRequestDTO dto);
        Task<AddFeeStructureAmountResponseDTO> AddFeeStructure(AddFeeStructureAmountRequestDTO dto);
        AddFeeTypeResponseDTO AddFeeType(AddFeeTypeRequestDTO dto);
        AddClassFeeTypeMappingResponseDTO AddClassSectionFeeMapping(
            AddClassFeeTypeMappingRequestDTO dto
        );
        SubjectAddResponseDTO AddSubject(SubjectAddRequestDTO dto);
        AssignSubjectResponseDTO AddSubjectsToClass(AssignSubjectDTO dto);
        Task<bool> DeleteClassById(int id);
        Task<DeleteFeeTypeResponseDTO> DeleteFeeType(int id);
        List<ClassModel> GetAllClasses();
        List<FeeType> GetAllFeeTypes(int? csid);
        List<FeeTypeForParticularStd> GetAllFeeTypesForParticularStdId(int id);
        GetSectionsDTO GetAllSections();
        List<StandardResponseDTO> GetAllStandards();
        List<SubjectModel> GetAllSubjects();
    }
}
