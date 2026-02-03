using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Mock_Exam_Work.Models;

namespace Mock_Exam_Work.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<Mock_Exam_Work.Models.Bookings> Bookings { get; set; } = default!;
        public DbSet<Mock_Exam_Work.Models.Rooms> Rooms { get; set; } = default!;
        public DbSet<Mock_Exam_Work.Models.Staff> Staff { get; set; } = default!;
    }
}
