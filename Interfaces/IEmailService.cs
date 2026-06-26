namespace Project_StudentERP.Interfaces
{
    public interface IEmailService
    {
        Task SendReceipt(
            string toEmail,
            string toParentEmail,
            string studentName,
            string receiptNo,
            decimal amountPaid,
            DateTime paymentDate,
            byte[] pdf
        );
    }
}
