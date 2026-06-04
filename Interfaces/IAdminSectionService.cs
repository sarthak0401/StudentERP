using Project_StudentERP.DTOs;
using Project_StudentERP.Models;

namespace Project_StudentERP.Interfaces
{
    public interface IAdminSectionService
    {
        ClassAddResponseDTO AddClass(ClassAddRequestDTO dto);
        List<ClassModel> GetAllClasses();
    }
}
