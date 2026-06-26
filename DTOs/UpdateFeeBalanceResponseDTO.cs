namespace Project_StudentERP.DTOs
{
    public class UpdateFeeBalanceResponseDTO
    {
        public int Status { get; set; }
        public bool Success { get; set; }
        public string? Message { get; set; }
        public int? ReceiptId { get; set; }
        public string? ReceiptNumber { get; set; }
    }
}
