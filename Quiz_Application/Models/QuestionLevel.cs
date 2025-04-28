using System.ComponentModel.DataAnnotations;

namespace Quiz_Application.Models
{
    public class QuestionLevel
    {
        public int? QuestionLevelID { get; set; }

        [Required(ErrorMessage = "Question level is required")]
        [StringLength(100, ErrorMessage = "Question level cannot exceed 100 characters")]
        public string QuestionLevelName { get; set; } // Renamed for clarity

        [Required(ErrorMessage = "User ID is required")]
        public int UserID { get; set; } // Foreign Key to MST_User
 
    }


    public class Level
    {
        public int? QuestionLevelID { get; set; }

        
        public string QuestionLevelName { get; set; }
    }

    public class AppUser
    {
        public int UserID { get; set; }
        public string Username { get; set; }
    }

}
