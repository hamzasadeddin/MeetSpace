using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MeetingRoomBooking.Models
{
    [Table("AppUser")]
    public class AppUser
    {
        [Key]
        public int User_Id { get; set; }

        [Required]
        public string User_FName { get; set; } = string.Empty;

        [Required]
        public string User_LName { get; set; } = string.Empty;

        [Required]
        public string User_Logon { get; set; } = string.Empty;

        [Required]
        public string User_Password { get; set; } = string.Empty;

        [Required]
        public string User_Company { get; set; } = string.Empty;

        public byte User_IsActive { get; set; } = 1;

        public DateTime? User_CreatedDate { get; set; }

        public string User_Role { get; set; } = "User";
        public string? User_Phone { get; set; }
        public string? User_Email { get; set; }
        public string? User_Department { get; set; }
    }
}