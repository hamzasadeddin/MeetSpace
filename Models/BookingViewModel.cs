using System.ComponentModel.DataAnnotations;

namespace MeetingRoomBooking.Models
{
    public class BookingViewModel
    {
        [Required]
        public int RoomId { get; set; }

        [Required, StringLength(255)]
        public string Subject { get; set; } = string.Empty;

        [Required, StringLength(384)]
        public string OrganizerName { get; set; } = string.Empty;

        [Required]
        public DateTime StartDateTime { get; set; }

        [Required]
        public DateTime EndDateTime { get; set; }

        public string? Description { get; set; }
        public string? Location { get; set; }
        public int? AttendeesCount { get; set; }

        public bool IsRecurring { get; set; }
        public string? RecurrenceType { get; set; }
        public DateTime? RecurrenceEndDate { get; set; }

        public MeetingRoom? Room { get; set; }
        public List<MeetingRoom> AvailableRooms { get; set; } = new();
    }
}