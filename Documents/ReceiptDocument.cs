namespace Project_StudentERP.Documents
{
    using global::Project_StudentERP.Models;
    using QuestPDF.Fluent;
    using QuestPDF.Helpers;
    using QuestPDF.Infrastructure;

    namespace Project_StudentERP.Documents
    {
        public class ReceiptDocument : IDocument
        {
            private readonly List<ReceiptRow> _rows;

            public ReceiptDocument(List<ReceiptRow> rows)
            {
                _rows = rows;
            }

            public DocumentMetadata GetMetadata() => DocumentMetadata.Default;

            public void Compose(IDocumentContainer container)
            {
                var header = _rows.First();

                decimal totalPaidInReceipt = _rows.Sum(x => x.AmountPaid);

                container.Page(page =>
                {
                    page.Margin(20);

                    //-------------------------------------
                    // HEADER
                    //-------------------------------------

                    page.Header()
                        .Background(Colors.Blue.Medium)
                        .Padding(10)
                        .AlignCenter()
                        .Text("STUDENT FEE RECEIPT")
                        .FontColor(Colors.White)
                        .FontSize(20)
                        .Bold();

                    //-------------------------------------
                    // CONTENT
                    //-------------------------------------

                    page.Content()
                        .PaddingVertical(15)
                        .Column(col =>
                        {
                            //---------------------------------
                            // Receipt Info
                            //---------------------------------

                            col.Item()
                                .Row(row =>
                                {
                                    row.RelativeItem()
                                        .Text(text =>
                                        {
                                            text.Span("Receipt No : ");
                                            text.Span(header.ReceiptNo).Bold();
                                        });

                                    row.RelativeItem()
                                        .AlignRight()
                                        .Text(text =>
                                        {
                                            text.Span("Date : ");
                                            text.Span($"{header.GeneratedDate:dd-MMM-yyyy}").Bold();
                                        });
                                });

                            col.Item().PaddingBottom(10);

                            //---------------------------------
                            // Student Details Card
                            //---------------------------------

                            col.Item()
                                .Border(1)
                                .Padding(18)
                                .Row(row =>
                                {
                                    row.RelativeItem()
                                        .Column(left =>
                                        {
                                            left.Item()
                                                .Text("Student Information")
                                                .Bold()
                                                .FontSize(13);

                                            left.Item().PaddingTop(8);

                                            left.Item()
                                                .Text(text =>
                                                {
                                                    text.Span("Student Name : ").SemiBold();
                                                    text.Span(header.SName);
                                                });

                                            left.Item().PaddingTop(3);

                                            left.Item()
                                                .Text(text =>
                                                {
                                                    text.Span("Class : ").SemiBold();
                                                    text.Span(
                                                        $"{header.ClassName} - {header.SectionName}"
                                                    );
                                                });
                                        });

                                    row.ConstantItem(35);
                                    row.RelativeItem()
                                        .Column(right =>
                                        {
                                            right
                                                .Item()
                                                .Text("Payment Information")
                                                .Bold()
                                                .FontSize(13);

                                            right.Item().PaddingTop(8);

                                            right
                                                .Item()
                                                .Text(text =>
                                                {
                                                    text.Span("Payment Type : ").SemiBold();
                                                    text.Span(header.PaymentType.ToUpper());
                                                });

                                            right.Item().PaddingTop(3);

                                            right
                                                .Item()
                                                .Text(text =>
                                                {
                                                    text.Span("Payment Date : ").SemiBold();
                                                    text.Span($"{header.PaymentDate:dd-MMM-yyyy}");
                                                });

                                            right.Item().PaddingTop(3);

                                            string txnNo = header.PaymentType.Equals(
                                                "cash",
                                                StringComparison.OrdinalIgnoreCase
                                            )
                                                ? "-"
                                                : header.TransactionNo.ToUpper();

                                            right
                                                .Item()
                                                .Text(text =>
                                                {
                                                    text.Span("Transaction No : ").SemiBold();
                                                    text.Span(txnNo);
                                                });

                                            if (!string.IsNullOrWhiteSpace(header.Remarks))
                                            {
                                                right.Item().Text($"Remarks : {header.Remarks}");
                                            }
                                        });
                                });

                            col.Item().PaddingVertical(15);

                            //---------------------------------
                            // Fee Details Heading
                            //---------------------------------

                            col.Item().Text("Fee Details").FontSize(15).Bold();

                            col.Item().PaddingBottom(8);

                            //---------------------------------
                            // Fee Table
                            //---------------------------------

                            col.Item()
                                .Table(table =>
                                {
                                    table.ColumnsDefinition(columns =>
                                    {
                                        columns.RelativeColumn(3);
                                        columns.RelativeColumn();
                                        columns.RelativeColumn();
                                        columns.RelativeColumn();
                                        columns.RelativeColumn();
                                    });

                                    //---------------------------------
                                    // Table Header
                                    //---------------------------------

                                    table.Header(header =>
                                    {
                                        header
                                            .Cell()
                                            .Border(1)
                                            .Background(Colors.Grey.Lighten1)
                                            .PaddingVertical(6)
                                            .PaddingLeft(12)
                                            .Text("Fee Type")
                                            .Bold()
                                            .AlignLeft();

                                        header
                                            .Cell()
                                            .Border(1)
                                            .Background(Colors.Grey.Lighten1)
                                            .Padding(5)
                                            .Text("Total Fee")
                                            .Bold()
                                            .AlignCenter();

                                        header
                                            .Cell()
                                            .Border(1)
                                            .Background(Colors.Grey.Lighten1)
                                            .Padding(5)
                                            .Text("Paid Now")
                                            .Bold()
                                            .AlignCenter();

                                        header
                                            .Cell()
                                            .Border(1)
                                            .Background(Colors.Grey.Lighten1)
                                            .Padding(5)
                                            .Text("Total Paid")
                                            .Bold()
                                            .AlignCenter();

                                        header
                                            .Cell()
                                            .Border(1)
                                            .Background(Colors.Grey.Lighten1)
                                            .Padding(5)
                                            .Text("Balance")
                                            .Bold()
                                            .AlignCenter();
                                    });

                                    //---------------------------------
                                    // Table Rows
                                    //---------------------------------

                                    foreach (var row in _rows)
                                    {
                                        table
                                            .Cell()
                                            .Border(1)
                                            .PaddingVertical(6)
                                            .PaddingLeft(12)
                                            .AlignLeft()
                                            .Text(row.FeeTypeName);

                                        table
                                            .Cell()
                                            .Border(1)
                                            .Padding(5)
                                            .AlignCenter()
                                            .Text(row.TotalAmt.ToString("N2"));

                                        table
                                            .Cell()
                                            .Border(1)
                                            .Padding(5)
                                            .AlignCenter()
                                            .Text(row.AmountPaid.ToString("N2"));

                                        table
                                            .Cell()
                                            .Border(1)
                                            .Padding(5)
                                            .AlignCenter()
                                            .Text(row.TotalPaid.ToString("N2"));

                                        table
                                            .Cell()
                                            .Border(1)
                                            .Padding(5)
                                            .AlignCenter()
                                            .Text(row.BalanceAmt.ToString("N2"));
                                    }
                                });

                            //---------------------------------
                            // Total Paid Box
                            //---------------------------------

                            col.Item().PaddingTop(15);

                            col.Item()
                                .Background(Colors.Green.Lighten3)
                                .Border(1)
                                .PaddingVertical(12)
                                .PaddingHorizontal(15)
                                .Column(total =>
                                {
                                    total
                                        .Item()
                                        .AlignRight()
                                        .Text("Total Paid In This Receipt")
                                        .SemiBold()
                                        .FontSize(13);

                                    total
                                        .Item()
                                        .AlignRight()
                                        .Text($"₹ {totalPaidInReceipt:N2}")
                                        .Bold()
                                        .FontSize(20);
                                });
                        });

                    //-------------------------------------
                    // FOOTER
                    //-------------------------------------

                    page.Footer()
                        .BorderTop(1)
                        .PaddingTop(8)
                        .AlignCenter()
                        .Column(col =>
                        {
                            col.Item().Text("Thank You For Your Payment").SemiBold().Italic();
                            col.Item()
                                .Text("This is a computer generated receipt.")
                                .FontSize(9)
                                .FontColor(Colors.Grey.Darken1);
                            col.Item()
                                .Text("Generated by Student ERP")
                                .FontSize(8)
                                .FontColor(Colors.Grey.Medium);
                        });
                });
            }
        }
    }
}
