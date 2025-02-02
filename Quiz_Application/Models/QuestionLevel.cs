using System.ComponentModel.DataAnnotations;

namespace Quiz_Application.Models
{
    public class QuestionLevel
    {
        [Required(ErrorMessage = "Question level is required")]
        [StringLength(100, ErrorMessage = "Question level cannot exceed 100 characters")]
        public string QuestionLevelName { get; set; } // Renamed for clarity

        [Required(ErrorMessage = "User ID is required")]
        public int UserID { get; set; } // Foreign Key to MST_User
    }
}
