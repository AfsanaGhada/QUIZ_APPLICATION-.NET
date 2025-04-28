
using System.ComponentModel.DataAnnotations;

namespace Quiz_Application.Models
{
    public class User
    {
        
        public int? UserID { get; set; }

        [Required(ErrorMessage = "Username is required")]
        [StringLength(100, ErrorMessage = "Username cannot exceed 100 characters")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [DataType(DataType.Password)]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Password must be at least 6 characters")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        [StringLength(100)]
        public string Email { get; set; }

        [Required(ErrorMessage = "Mobile number is required")]
        [Phone(ErrorMessage = "Invalid mobile number format")]
        [StringLength(15, MinimumLength = 10, ErrorMessage = "Mobile number cannot exceed 15 digits")]
        public string Mobile { get; set; }

        public bool IsActive { get; set; }=true;
        public bool IsAdmin { get; set; } = false;
    }
}
