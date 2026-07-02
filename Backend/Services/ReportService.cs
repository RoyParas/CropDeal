using System.Drawing;
using Backend.Data;
using Backend.Exceptions;
using Backend.Interfaces;
using Backend.Models;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using OfficeOpenXml.Style;

namespace Backend.Services
{
    public class ReportService : IReportService
    {
        private readonly AppDbContext _context;
        public ReportService(AppDbContext context)
        {
            _context = context;
        }
        public byte[] GenerateExcel(List<Transaction> transactions, User user)
        {
            using var package = new ExcelPackage();
            var worksheet = package.Workbook.Worksheets.Add("Transactions");


            worksheet.Cells[1, 1].Value = user.Role == "Farmer" ? "Farmer Name": "Dealer Name";
            worksheet.Cells[1, 2].Value = user.FullName;
            worksheet.Cells[2, 1].Value = "Phone Number";
            worksheet.Cells[2, 2].Value = user.PhoneNumber;
            worksheet.Cells[3, 1].Value = "Email";
            worksheet.Cells[3, 2].Value = user.Email;
            worksheet.Cells[4, 1].Value = "Is Active ?";
            worksheet.Cells[4, 2].Value = user.IsActive.ToString();
            if (user.Role == "Farmer")
            {
                worksheet.Cells[5, 1].Value = "Average Rating";
                worksheet.Cells[5, 2].Value = user.AverageRating + "/5 ⭐";
            }

            worksheet.Cells[1, 1, 5, 1].Style.Font.Bold = true;
            worksheet.Cells[1, 1, 5, 1].Style.Fill.PatternType = ExcelFillStyle.Solid;
            worksheet.Cells[1, 1, 5, 1].Style.Fill.BackgroundColor.SetColor(Color.LightYellow);
            using (var range = worksheet.Cells[1, 1, 5, 2])
            {
                range.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                range.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                range.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                range.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                range.Style.Border.Top.Color.SetColor(Color.Gray);
                range.Style.Border.Bottom.Color.SetColor(Color.Gray);
                range.Style.Border.Left.Color.SetColor(Color.Gray);
                range.Style.Border.Right.Color.SetColor(Color.Gray);
            }

            // Headers
            int headerRow = 7;
            int totalRow = transactions.Count;
            worksheet.Cells[headerRow, 1].Value = "Transaction Id";
            worksheet.Cells[headerRow, 2].Value = "Listing Id";
            worksheet.Cells[headerRow, 3].Value = user.Role == "Farmer" ? "Dealer Id" : "Farmer Id";
            worksheet.Cells[headerRow, 4].Value = user.Role == "Farmer" ? "Dealer Name" : "Farmer Name";
            worksheet.Cells[headerRow, 5].Value = "Crop Name";
            worksheet.Cells[headerRow, 6].Value = "QuantityBought(in Kg)";
            worksheet.Cells[headerRow, 7].Value = "Dealing Price Per Kg";
            worksheet.Cells[headerRow, 8].Value = "Total Price";
            worksheet.Cells[headerRow, 9].Value = "Transaction Status";
            worksheet.Cells[headerRow, 10].Value = "Transaction Date";

            // Apply bold, border and background color to header row
            using (var range = worksheet.Cells[headerRow, 1, headerRow, 10])
            {
                range.Style.Font.Bold = true;
                range.Style.Fill.PatternType = ExcelFillStyle.Solid;
                range.Style.Fill.BackgroundColor.SetColor(Color.LightSteelBlue);
                range.Style.Font.Color.SetColor(Color.DarkBlue);
                range.Style.Border.Top.Style = ExcelBorderStyle.Thick;
                range.Style.Border.Bottom.Style = ExcelBorderStyle.Thick;
                range.Style.Border.Left.Style = ExcelBorderStyle.Thick;
                range.Style.Border.Right.Style = ExcelBorderStyle.Thick;
                range.Style.Border.Top.Color.SetColor(Color.Gray);
                range.Style.Border.Bottom.Color.SetColor(Color.Gray);
                range.Style.Border.Left.Color.SetColor(Color.Gray);
                range.Style.Border.Right.Color.SetColor(Color.Gray);
                range.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            }

            // Data
            for (int i = 0; i < transactions.Count; i++)
            {
                var t = transactions[i];
                worksheet.Cells[i + 8, 1].Value = t.Id;
                worksheet.Cells[i + 8, 2].Value = t.ListingId;
                worksheet.Cells[i + 8, 3].Value = user.Role == "Farmer" ? t.DealerId : t.Listing.FarmerId;
                worksheet.Cells[i + 8, 4].Value = user.Role == "Farmer" ? t.Dealer.FullName : t.Listing.Farmer.FullName;
                worksheet.Cells[i + 8, 5].Value = t.Listing.Crop.Name;
                worksheet.Cells[i + 8, 6].Value = t.QuantityInKg;
                worksheet.Cells[i + 8, 7].Value = t.FinalPricePerKg;
                worksheet.Cells[i + 8, 8].Value = t.TotalPrice;
                worksheet.Cells[i + 8, 9].Value = t.TransactionStatus;
                worksheet.Cells[i + 8, 10].Value = t.UpdatedAt.ToString("yyyy-MM-dd");
            }

            using (var range = worksheet.Cells[headerRow+1, 1, 7 + totalRow, 10])
            {
                // range.Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Gray);
                range.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                range.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                range.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                range.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                range.Style.Border.Top.Color.SetColor(Color.Gray);
                range.Style.Border.Bottom.Color.SetColor(Color.Gray);
                range.Style.Border.Left.Color.SetColor(Color.Gray);
                range.Style.Border.Right.Color.SetColor(Color.Gray);
                range.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            }

            worksheet.Cells.AutoFitColumns();
            return package.GetAsByteArray();
        }

    }
}