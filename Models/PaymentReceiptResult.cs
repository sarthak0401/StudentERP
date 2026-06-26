namespace Project_StudentERP.Models
{
    public class PaymentReceiptResult
    {
        public int PaymentId { get; set; }

        public int ReceiptId { get; set; }

        public string ReceiptNo { get; set; } = string.Empty;

        public string ParentEmail { get; set; }
        public string SEmail { get; set; }
        public DateTime PaymentDate { get; set; }
        public decimal TotalAmtPaid { get; set; }
        public string SName { get; set; }
    }
}
