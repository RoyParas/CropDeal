using Backend.DTOs;
using Backend.Interfaces;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace Backend.Services;

public class ReceiptPdfService : IReceiptPdfService
{
    public byte[] GenerateReceiptPdf(ReceiptDto receipt)
    {
        var document = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4.Landscape());
                page.Margin(2, Unit.Centimetre);
                page.DefaultTextStyle(x => x.FontSize(11).FontFamily("Arial"));

                // ==== Header with branding ====
                page.Header().Padding(15).Row(row =>
                {
                    // Logo
                    row.ConstantItem(180).Height(150).Image(Image.FromFile("wwwroot/images/logo-for-receipt.png")).FitHeight();

                    // Title & Info
                    row.RelativeItem().AlignRight().Column(col =>
                    {
                        col.Item().AlignRight().Text("RECEIPT")
                            .FontSize(28).Bold().FontColor(Colors.Green.Darken2);

                        col.Item().AlignRight().Text($"Transaction ID: {receipt.TransactionNumber}")
                            .FontSize(12).FontColor(Colors.Grey.Darken2);

                        col.Item().AlignRight().Text($"Date: {receipt.CreatedAt:dd MMM yyyy}")
                            .FontSize(11).FontColor(Colors.Grey.Darken1);
                    });
                });

                // ==== Billing Section ====
                page.Content().PaddingTop(20).Column(col =>
                {
                    col.Spacing(15);

                    col.Item().Row(row =>
                    {
                        row.RelativeItem().Column(left =>
                        {
                            left.Item().Text("From:").Bold().FontColor(Colors.Black);
                            left.Item().Text(receipt.FarmerName);
                            left.Item().Text($"Email: {receipt.FarmerEmail}");
                            left.Item().Text($"Phone: {receipt.FarmerPhone}");
                        });

                        row.RelativeItem().AlignRight().Column(right =>
                        {
                            right.Item().Text("To:").Bold().FontColor(Colors.Black);
                            right.Item().Text(receipt.DealerName);
                            right.Item().Text($"Email: {receipt.DealerEmail}");
                            right.Item().Text($"Phone: {receipt.DealerPhone}");
                        });
                    });

                    // ==== Item Table ====
                    col.Item().PaddingTop(25).Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.RelativeColumn(2); // Item
                            columns.RelativeColumn(3); // Description
                            columns.ConstantColumn(80); // Quantity
                            columns.ConstantColumn(100); // Unit Price
                            columns.ConstantColumn(100); // Total
                        });

                        table.Header(header =>
                        {
                            string[] headers = { "Item", "Description", "Qty", "Unit Price", "Total" };
                            foreach (var h in headers)
                            {
                                header.Cell().Element(CellHeaderStyle).Text(h).Bold().FontColor(Colors.White);
                            }

                            static IContainer CellHeaderStyle(IContainer container) =>
                                container.Background(Colors.Green.Darken2).PaddingVertical(5).PaddingHorizontal(8);
                        });

                        table.Cell().Element(CellStyle).Text(receipt.CropName);
                        table.Cell().Element(CellStyle).Text(receipt.Description);
                        table.Cell().Element(CellStyle).Text($"{receipt.QuantityInKg:N2} kg");
                        table.Cell().Element(CellStyle).Text($"₹ {receipt.PricePerKg:N2}");
                        table.Cell().Element(CellStyle).Text($"₹ {receipt.TotalPrice:N2}");

                        static IContainer CellStyle(IContainer container) =>
                            container.BorderBottom(1).BorderColor(Colors.Grey.Lighten2).PaddingVertical(4).PaddingHorizontal(8);
                    });

                    // ==== Summary ====
                    col.Item().AlignRight().PaddingTop(30).Width(130).Table(table =>
                    {
                        table.ColumnsDefinition(cols =>
                        {
                            cols.RelativeColumn();
                            cols.ConstantColumn(60);
                        });

                        void AddRow(string label, string value, bool bold = false)
                        {
                            table.Cell().Element(SummaryCellStyle).Text(label).Style(bold ? TextStyle.Default.Bold() : TextStyle.Default);
                            table.Cell().Element(SummaryCellStyle).AlignRight().Text(value).Style(bold ? TextStyle.Default.Bold() : TextStyle.Default);
                        }

                        AddRow("Subtotal:", $"₹ {receipt.TotalPrice:N2}");
                        AddRow("Discount:", $"- ₹ {receipt.Discount:N2}");
                        AddRow("Total Paid:", $"₹ {receipt.AmountPaid:N2}", true);

                        static IContainer SummaryCellStyle(IContainer c) =>
                            c.PaddingVertical(4);
                    });

                    // ==== Footer ====
                    page.Footer().Column(footer =>
                    {
                        footer.Item().PaddingVertical(5).LineHorizontal(1).LineColor(Colors.Grey.Lighten1);
                        footer.Item().AlignCenter().Text("Thank you for doing business with CropDeal.")
                            .FontSize(10).Italic().FontColor(Colors.Grey.Darken1);
                    });
                });
            });
        });

        return document.GeneratePdf();
    }

}