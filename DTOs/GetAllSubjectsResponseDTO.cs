using Project_StudentERP.Models;

namespace Project_StudentERP.DTOs
{
    public class GetAllSubjectsResponseDTO
    {
        public string? Message { get; set; }
        public bool Success { get; set; }
        public int Status { get; set; }
        public List<SubjectModel> Subjects { get; set; } = new();
    }
}
