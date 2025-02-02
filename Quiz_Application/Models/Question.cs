using System.ComponentModel.DataAnnotations;

namespace Quiz_Application.Models
{
    public class Question
    {
        [Required(ErrorMessage = "Question text is required")]
        public string QuestionText { get; set; }

        [Required(ErrorMessage = "Question level is required")]
        public int QuestionLevelID { get; set; } // Foreign Key to MST_QuestionLevel

        [Required(ErrorMessage = "Option A is required")]
        [StringLength(100, ErrorMessage = "Option A cannot exceed 100 characters")]
        public string OptionA { get; set; }

        [Required(ErrorMessage = "Option B is required")]
        [StringLength(100, ErrorMessage = "Option B cannot exceed 100 characters")]
        public string OptionB { get; set; }

        [Required(ErrorMessage = "Option C is required")]
        [StringLength(100, ErrorMessage = "Option C cannot exceed 100 characters")]
        public string OptionC { get; set; }

        [Required(ErrorMessage = "Option D is required")]
        [StringLength(100, ErrorMessage = "Option D cannot exceed 100 characters")]
        public string OptionD { get; set; }

        [Required(ErrorMessage = "Correct option is required")]
        [RegularExpression("^(A|B|C|D)$", ErrorMessage = "Correct option must be A, B, C, or D")]
        public string CorrectOption { get; set; }

        [Required(ErrorMessage = "Question marks are required")]
        [Range(1, int.MaxValue, ErrorMessage = "Marks must be at least 1")]
        public int QuestionMarks { get; set; }
        public bool IsActive { get; set; } = true;


        [Required(ErrorMessage = "User ID is required")]
        public int UserID { get; set; } // Foreign Key to MST_User

    }
}
