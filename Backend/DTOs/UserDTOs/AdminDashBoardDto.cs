namespace Backend.DTOs.UserDTOs
{
    public class AdminDashBoardDto
    {
        public int TotalDealers {get; set;}
        public int TotalFarmers {get; set;}
        public int TotalAllowedCrops {get; set;}
        public int TotalListedCrops {get; set;} 
        public int CompletedDeals {get; set;} 
        public float TotalDealValue { get; set; }
    }
}