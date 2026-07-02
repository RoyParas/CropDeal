using System.ComponentModel.DataAnnotations;

namespace Backend.DTOs.ReviewDTOs
{
    public class AddReviewDto
    {
        [Range(1, 5, ErrorMessage = "Rating must be between 1 and 5.")]
        public int Rating { get; set; } = 0;

        [Required(ErrorMessage = "Provide Comment")]
        public string Comment { get; set; } = string.Empty;
    }
}