using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Mock_Exam_Work.Models;

namespace Mock_Exam_Work.Data.Migrations
{
    public class SeedData
    {
        public static async Task SeedRoomsAsync(ApplicationDbContext context)
        {
            if (!await context.Rooms.AnyAsync())
            {
                var Rooms = new List<Rooms>
                {
                    new Rooms
                    { RoomsName = "Room 1",
                      RoomsDescription = "A spacious conference room.",
                      Capacity = 20,
                      HourlyRate = 50.0f,
                      City = "New York",
                      IsAvailable = true
                    },
                    new Rooms
                    { RoomsName = "Room 2",
                      RoomsDescription = "A cozy meeting room.",
                      Capacity = 10,
                      HourlyRate = 30.0f,
                      City = "Los Angeles",
                      IsAvailable = true
                    },
                    // Add more rooms as needed
                };
                await context.Rooms.AddRangeAsync(Rooms);
                await context.SaveChangesAsync();
            }
        }
        public static async Task SeedBookingsAsync(ApplicationDbContext context)
        {
            if (!await context.Bookings.AnyAsync())
            {
                var Room1 = await context.Rooms.FirstOrDefaultAsync(r => r.RoomsName == "Room 1");
                if (Room1 == null)
                    return;

                var booking = new Bookings
                {
                    RoomsId = Room1.RoomsId,
                    BookingCreatedAt = DateTime.Now,
                    CheckInDate = DateTime.Now.AddHours(1),
                    CheckOutDate = DateTime.Now.AddHours(2),
                    Status = "Pending",
                    SpecialRequest = "None",
                    IsPayed = false,
                    PayedAt = DateTime.MinValue,
                    UserId = "sample-user-id" // Replace with actual user ID
                };
                await context.Bookings.AddAsync(booking);
                await context.SaveChangesAsync();
            }
        }
        public static async Task SeedInstallationAsync(IServiceProvider serviceProvider, UserManager<IdentityUser> userManager)
        {
            using var scope = serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            if (!context.Installations.Any())
            {
                var user1 = await userManager.FindByEmailAsync("user1@example.com");
                var user2 = await userManager.FindByEmailAsync("user2@example.com");

                if (user1 == null || user2 == null)
                {
                    var installations = new List<Installations>

            {
                new Installations
                {
                    Name = "Site Alpha",
                    Email = "sitealpha@example.com",
                    Desciption = "Test install 1",
                    BindingAddress = "123 Main st",
                    City = "London",
                    UserId = user1.Id,
                },
                new Installations
                {
                    Name = "Site Beta",
                    Email = "sitebeta@example.com",
                    Desciption = "Test install 2",
                    BindingAddress = "678 State Rd",
                    City = "Birmingham",
                    UserId = user2.Id,
                }
            };
                    context.Installations.AddRangeAsync(installations);
                    await context.SaveChangesAsync();
                }
            }

        }
    }
}
    
