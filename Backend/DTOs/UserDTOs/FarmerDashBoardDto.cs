namespace Backend.DTOs.UserDTOs
{
    public class FarmerDashBoardDto
    {
        public int TotalListedCrops {get; set;}
        public int CompletedDeals {get; set;}
        public float AverageRating { get; set; } = 0;
        public float TotalEarnings {get; set;}
    }
}