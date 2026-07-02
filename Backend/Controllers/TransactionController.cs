using System.Security.Claims;
using Backend.DTOs;
using Backend.DTOs.TransactionDTOs;
using Backend.Exceptions;
using Backend.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TransactionController : ControllerBase
    {
        private readonly ITransactionRepo _transactionRepo;
        private readonly IUserRepo _userRepo;
        private readonly IReceiptPdfService _receiptPdfService;
        private readonly IReportService _reportService;

        public TransactionController(ITransactionRepo transactionRepo, IUserRepo userRepo,IReceiptPdfService receiptPdfService, IReportService reportService)
        {
            _transactionRepo = transactionRepo;
            _userRepo = userRepo;
            _receiptPdfService = receiptPdfService;
            _reportService = reportService;
        }


        private Guid ValidateIdString(string? idString)
        {
            if (string.IsNullOrEmpty(idString) || !Guid.TryParse(idString, out Guid id))
            {
                throw new InvalidException($"Invalid `{idString}` id string");
            }
            return id;
        }


        [Authorize(Roles = "Admin")]
        [HttpGet("getAllTransactions")]
        public async Task<IActionResult> GetAllTransactions()
        {
            var allTransactions = await _transactionRepo.GetAllTransactionsAync();

            return Ok(allTransactions);
        }

        [Authorize]
        [HttpGet("getTransactionById")]
        public async Task<IActionResult> GetTransactionById([FromQuery] string transactionIdString)
        {
            Guid transactionId = ValidateIdString(transactionIdString);

            var transaction = await _transactionRepo.GetTransactionByIdAsync(transactionId);

            return Ok(transaction);
        }


        [Authorize(Roles = "Dealer")]
        [HttpGet("getDealerTransactions")]
        public async Task<IActionResult> GetDealerTransactions()
        {
            Guid dealerId = ValidateIdString(User.FindFirstValue(ClaimTypes.NameIdentifier));

            var dealerTransactions = await _transactionRepo.GetTransactionsByDealerIdAsync(dealerId);

            return Ok(dealerTransactions);
        }


        [Authorize(Roles = "Admin")]
        [HttpGet("generateReport")]
        public async Task<IActionResult> GenerateReport([FromQuery] string userIdString)
        {
            Guid userId = ValidateIdString(userIdString);

            var user = await _userRepo.GetUserByIdAsync(userId);

            var transactions = await _transactionRepo.GetTransactionsByUserIdAsync(userId);

            if (!transactions.Any())
                return NotFound(new { message = "No transactions found for this dealer." });

            var excelBytes =  _reportService.GenerateExcel(transactions, user);

            return File(excelBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"{user.FullName}_{userId}_Transactions_Report.xlsx");
        }


        [Authorize(Roles = "Farmer")]
        [HttpGet("getFarmerTransactions")]
        public async Task<IActionResult> GetFarmerTransactions()
        {
            Guid farmerId = ValidateIdString(User.FindFirstValue(ClaimTypes.NameIdentifier));

            var farmerTransactions = await _transactionRepo.GetTransactionsByFarmerIdAsync(farmerId);

            return Ok(farmerTransactions);
        }


        [Authorize(Roles = "Dealer")]
        [HttpPost("initiateTransaction")]
        public async Task<IActionResult> InitiateTransaction([FromBody] TransactionDto transactionDto, [FromQuery] string CropListingIdString)
        {
            Guid dealerId = ValidateIdString(User.FindFirstValue(ClaimTypes.NameIdentifier));

            Guid cropListingId = ValidateIdString(CropListingIdString);

            if (await _transactionRepo.InitiateTransactionAsync(transactionDto, dealerId, cropListingId))
                return Ok(new { message = "transaction initiated successfully" });

            else throw new UnableException("Unable to initiate Transaction");
        }


        [Authorize(Roles = "Farmer")]
        [HttpPatch("acceptTransaction")]
        public async Task<IActionResult> AcceptTransaction([FromQuery] string transactionIdString)
        {
            Guid transactionId = ValidateIdString(transactionIdString);

            if (await _transactionRepo.AcceptTransactionAsync(transactionId))
                return Ok(new { message = "transaction accepted successfully" });

            else throw new UnableException("Unable to accept transaction");
        }


        [Authorize(Roles = "Farmer, Dealer")]
        [HttpPatch("rejectTransaction")]
        public async Task<IActionResult> RejectTransaction([FromQuery] string transactionIdString)
        {
            string? role = User.FindFirstValue(ClaimTypes.Role);

            Guid transactionId = ValidateIdString(transactionIdString);

            if (role != null)
            {
                if (await _transactionRepo.RejectTransactionAsync(transactionId, role))
                    return Ok(new { message = "transaction rejected successfully", role });

                else throw new UnableException("Unable to reject transaction");
            }
            else throw new NotFoundException("Role of user not found");
        }


        [Authorize(Roles = "Dealer")]
        [HttpPatch("makePayment")]
        public async Task<IActionResult> MakePayment([FromQuery] string transactionIdString)
        {
            Guid transactionId = ValidateIdString(transactionIdString);

            if (await _transactionRepo.MakePaymentAsync(transactionId))
                return Ok(new { message = "payment done successfully" });

            else throw new UnableException("Unable to make payment");
        }
        
        [Authorize(Roles = "Dealer, Farmer")]
        [HttpGet("receipt/pdf")]
        public async Task<IActionResult> GetReceiptPdf([FromQuery] string transactionIdString)
        {
            Guid transactionId = ValidateIdString(transactionIdString);

            var transaction = await _transactionRepo.GetTransactionByIdAsync(transactionId);

            if (transaction == null || transaction.TransactionStatus != "Completed")
                throw new  NotFoundException("Transaction not found or not Completed");

            
            ReceiptDto dto = new ReceiptDto
            {
                TransactionNumber = transaction.Id,

                DealerName = transaction.Dealer.FullName,
                DealerEmail = transaction.Dealer.Email,
                DealerPhone = transaction.Dealer.PhoneNumber,

                FarmerName = transaction.Listing.Farmer.FullName,
                FarmerEmail = transaction.Listing.Farmer.Email,
                FarmerPhone = transaction.Listing.Farmer.PhoneNumber,

                CropName = transaction.Listing.Crop.Name,
                Description = transaction.Listing.Description,
                QuantityInKg = transaction.QuantityInKg,
                PricePerKg = transaction.Listing.PricePerKg,
                TotalPrice = transaction.QuantityInKg * transaction.Listing.PricePerKg,

                Discount = (transaction.QuantityInKg * transaction.Listing.PricePerKg) - transaction.TotalPrice,
                AmountPaid = transaction.TotalPrice,
                CreatedAt = transaction.CreatedAt
            };

            var pdfBytes = _receiptPdfService.GenerateReceiptPdf(dto);
            return File(pdfBytes, "application/pdf", $"Receipt_{transactionId}.pdf");
        }
    }
}