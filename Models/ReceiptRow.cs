namespace Project_StudentERP.Models
{
    public class ReceiptRow
    {
        public DateTime GeneratedDate { get; set; }

        public string SName { get; set; }
        public string ReceiptNo { get; set; }

        public string ClassName { get; set; }

        public string SectionName { get; set; }

        public string PaymentType { get; set; }

        public DateTime PaymentDate { get; set; }

        public string Remarks { get; set; }

        public string TransactionNo { get; set; }

        public string FeeTypeName { get; set; }

        public decimal TotalAmt { get; set; }

        public decimal TotalPaid { get; set; }

        public decimal BalanceAmt { get; set; }
        public decimal AmountPaid { get; set; }
    }
}
