using Project_StudentERP.Models;

namespace Project_StudentERP.DTOs
{
    public class GetAllClassesDTO
    {
        public int STATUS { get; set; }
        public bool Success { get; set; }

        public List<ClassModel> classes { get; set; } = new();
    }
}
