using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MeetingRoomBooking.Models
{
    [Table("MeetingRoom")]
    public class MeetingRoom
    {
        [Key]
        public int Room_Id { get; set; }
        public string Room_Name { get; set; } = string.Empty;
        public string? Room_Location { get; set; }
        public int? Room_Capacity { get; set; }
        public string? Room_Amenities { get; set; }
        public byte? Room_IsActive { get; set; }
        public string? Room_ImageUrl { get; set; }
        public string? Room_Company { get; set; }
    }
}