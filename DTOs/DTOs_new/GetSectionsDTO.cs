using Project_StudentERP.Models;

namespace Project_StudentERP.DTOs.DTOs_new
{
    public class GetSectionsDTO
    {
        public int Status { get; set; }
        public bool Success { get; set; }
        public string? Message { get; set; }
        public List<Sections> sections { get; set; } = new List<Sections>();
    }
}
