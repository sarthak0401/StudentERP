namespace Project_StudentERP.DTOs
{
    public class UpdateFeeDetailsStudentRequestDTO
    {
        public int AdmissionId { get; set; }
        public List<FeeDetails> SelectedFees { get; set; } = new();
    }

    public class FeeDetails
    {
        public int FtId { get; set; }
        public decimal FixedAmt { get; set; }
        public decimal Discount { get; set; }
        public decimal FinalAmount { get; set; }
    }
}
