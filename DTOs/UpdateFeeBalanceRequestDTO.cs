namespace Project_StudentERP.DTOs
{
    public class UpdateFeeBalanceRequestDTO
    {
        public int AdmissionId { get; set; }
        public string PaymentType { get; set; }
        public DateTime PaymentDate { get; set; }
        public string? Remarks { get; set; }
        public string TransactionId { get; set; }

        public List<FeePaidList> PaidFeeList { get; set; }
    }

    public class FeePaidList
    {
        public int AdmissionFeeId { get; set; }
        public decimal AmountPaid { get; set; }
    }
}
