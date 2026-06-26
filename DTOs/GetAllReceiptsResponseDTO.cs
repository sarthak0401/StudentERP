namespace Project_StudentERP.DTOs
{
    public class GetAllReceiptsResponseDTO
    {
        public int Status { get; set; }
        public bool Success { get; set; }
        public string Message { get; set; }
        public List<ReceiptDTO> Receipts { get; set; } = new();
    }

    public class ReceiptDTO
    {
        public int ReceiptId { get; set; }
        public string ReceiptNo { get; set; }
        public string PaymentType { get; set; }
        public string TransactionNo { get; set; }
        public DateTime PaymentDate { get; set; }
        public string ClassName { get; set; }
        public string SectionName { get; set; }
    }
}
