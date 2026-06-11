using Project_StudentERP.DTOs;
using Project_StudentERP.Models;

namespace Project_StudentERP.Interfaces
{
    public interface IAdministrationService
    {
        Task AddStudent(StudentDTO dto, string filename, int tId);
        List<Student> GetAllStudents();
        Student GetStudentById(int id);
        List<Student> GetStudentsOnSearch(StudentSearchDTO studentSearchDTO);
    }
}
