using Project_StudentERP.DTOs;
using Project_StudentERP.Models;

namespace Project_StudentERP.Interfaces
{
    public interface IAdministrationService
    {
        Task AddStudent(StudentDTO dto, string filename, int tId);
        List<string> getAllContacts(int id);
        List<Student> GetAllStudents();
        Student GetStudentById(int id);
        List<SearchStudentResponseDTO> GetStudentsOnSearch(StudentSearchDTO studentSearchDTO);
    }
}
