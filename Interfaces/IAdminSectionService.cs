using Project_StudentERP.DTOs;
using Project_StudentERP.Models;

namespace Project_StudentERP.Interfaces
{
    public interface IAdminSectionService
    {
        ClassAddResponseDTO AddClass(ClassAddRequestDTO dto);
        SubjectAddResponseDTO AddSubject(SubjectAddRequestDTO dto);
        AssignSubjectResponseDTO AddSubjectsToClass(AssignSubjectDTO dto);
        Task<bool> DeleteClassById(int id);
        List<ClassModel> GetAllClasses();
        List<SubjectModel> GetAllSubjects();
    }
}
