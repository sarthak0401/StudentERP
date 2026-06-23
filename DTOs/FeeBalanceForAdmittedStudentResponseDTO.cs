using Microsoft.AspNetCore.DataProtection.KeyManagement;

namespace Project_StudentERP.DTOs
{
    public class FeeBalanceForAdmittedStudentResponseDTO
    {
        public string? Message { get; set; }
        public bool Success { get; set; }
        public int Status { get; set; }
        public List<FeeBalanceForAdmittedStudent> Data { get; set; } = new();
    }

    public class FeeBalanceForAdmittedStudent
    {
        public string FeeTypeName { get; set; }
        public string ClassName { get; set; }
        public string SectionName { get; set; }
        public int AdmissionFeeId { get; set; }
        public int AdmissionId { get; set; }
        public decimal TotalAmt { get; set; }

        public decimal TotalPaid { get; set; }

        public decimal BalanceAmt { get; set; }
    }
}
