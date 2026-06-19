using Project_StudentERP.DTOs;
using Project_StudentERP.Models;

namespace Project_StudentERP.Interfaces
{
    public interface IAdministrationService
    {
        Task AddStudent(StudentDTO dto, string filename, int tId);
        List<string> getAllContacts(int id);
        GetFeeForAlreadyAdmittedStudentResponseDTO GetAllFeeInfoForAlreadyAdmittedStudent(int id);
        GetAllFeeTypesForParticularClassSectionResponseDTO GetAllFeeTypesForParticularClassSection(
            int id
        );
        List<Student> GetAllStudents();
        Student GetStudentById(int id);
        List<SearchStudentResponseDTO> GetStudentsOnSearch(StudentSearchDTO studentSearchDTO);
        AdmissionStudentResponseDTO StudentAdmission(AdmissionStudentRequestDTO dto);
        UpdateFeeDetailsStudentResponseDTO UpdateFeeDetailsOfAlreadyAdmittedStudent(
            UpdateFeeDetailsStudentRequestDTO dto
        );
    }
}
