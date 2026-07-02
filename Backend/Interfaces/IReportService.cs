using Backend.Models;

namespace Backend.Interfaces
{
    public interface IReportService
    {
        byte[] GenerateExcel(List<Transaction> transactions, User user);
    }
}