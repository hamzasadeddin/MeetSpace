using Microsoft.EntityFrameworkCore;
using MeetingRoomBooking.Models;

namespace MeetingRoomBooking.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Communication> Communications { get; set; }
        public DbSet<MeetingRoom> MeetingRooms { get; set; }
        public DbSet<AppUser> AppUsers { get; set; }
    }
}