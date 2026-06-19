using Project_StudentERP.Models;

namespace Project_StudentERP.DTOs
{
    public class GetFeeForAlreadyAdmittedStudentResponseDTO
    {
        public int Status { get; set; }
        public string? Message { get; set; }
        public bool Success { get; set; }

        public List<FeeForAdmittedStudent> FeeLst { get; set; } = new();
    }
}
