using System.ComponentModel.DataAnnotations;

namespace MeetingRoomBooking.Models
{
    public class RoomViewModel
    {
        public int Room_Id { get; set; }

        [Required, StringLength(100)]
        public string Room_Name { get; set; } = string.Empty;

        [StringLength(255)]
        public string? Room_Location { get; set; }

        [Range(1, 500)]
        public int? Room_Capacity { get; set; }

        [StringLength(500)]
        public string? Room_Amenities { get; set; }

        public string? Room_Company { get; set; }
    }
}