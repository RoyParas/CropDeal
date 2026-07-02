using Backend.DTOs;

namespace Backend.Interfaces
{
    public interface IReceiptPdfService
    {
        byte[] GenerateReceiptPdf(ReceiptDto receipt);
    }
}
