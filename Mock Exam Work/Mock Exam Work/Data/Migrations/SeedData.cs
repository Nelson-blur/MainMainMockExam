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
                var rooms = new List<Rooms>
                {
                    new Rooms
                    { RoomsName = "Meeting Room",
                      RoomsDescription = "A spacious conference room.",
                      Capacity = 20,
                      HourlyRate = 50.0f,
                      City = "New York",
                      IsAvailable = true
                    },
                    new Rooms
                    { RoomsName = "Conference Room",
                      RoomsDescription = "A cozy meeting room.",
                      Capacity = 10,
                      HourlyRate = 30.0f,
                      City = "Los Angeles",
                      IsAvailable = true
                    },
                    // Add more rooms as needed
                };
                await context.Rooms.AddRangeAsync(rooms);
                await context.SaveChangesAsync();
            }
        }
        public static async Task SeedBookingsAsync(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            if (!await context.Bookings.AnyAsync())
            {
                var users = await userManager.Users.Take(2).ToListAsync();
                var rooms = await context.Rooms.Take(2).ToListAsync();
                if (users.Count >= 2 && rooms.Count >= 2)
                {
                    var bookings = new List<Bookings>
                    {
                        new Bookings
                        {
                            UserId = users[0].Id,
                            RoomsId = rooms[0].RoomsId,
                            CheckInDate = DateTime.Now.AddDays(1),
                            CheckOutDate = DateTime.Now.AddDays(1),
                            Status = "Pending",
                            BookingCreatedAt = DateTime.Now,
                            SpecialRequest = "None",
                            IsPayed = false,
                            PayedAt = DateTime.MinValue

                        },
                        new Bookings
                        {
                            UserId = users[1].Id,
                            RoomsId = rooms[1].RoomsId,
                            CheckInDate = DateTime.Now.AddDays(2),
                            CheckOutDate = DateTime.Now.AddDays(2),
                            Status = "Confirmed",
                            BookingCreatedAt = DateTime.Now,
                            SpecialRequest = "Confirmed",
                            IsPayed = true,
                            PayedAt = DateTime.Now


                        }
                    };
                    await context.Bookings.AddRangeAsync(bookings);
                    await context.SaveChangesAsync();
                }
            }
        }
        public static async Task SeedStaffAsync(IServiceProvider serviceProvider)
        {
            using var context = new ApplicationDbContext(serviceProvider.GetRequiredService<DbContextOptions<ApplicationDbContext>>());


            if (!await context.Staff.AnyAsync())
            {
                await context.Staff.AddRangeAsync(
                    new Staff
                    {
                        JobTitle = "Manager",
                        StaffFullName = "John Doe",
                        Bio = "Experienced manager with a background in hospitality."
                    },
                    new Staff
                    {
                        JobTitle = "Receptionist",
                        StaffFullName = "Jane Smith",
                        Bio = "Friendly receptionist with excellent customer service skills."
                    }
                    );
                await context.SaveChangesAsync();


            }
        }
        public static async Task SeedUsersAsync(UserManager<IdentityUser> userManager)
        {
            var usersToCreate = new List<(string Email, string Password, string Role)>
            {
                ("customer1@example.com", "Customer@123", "User"),
                ("customer2@example.com", "Customer@123", "User"),
                ("manager1@example.com", "Manager@123", "Manager"),
                ("manager2@example.com", "Manager@123", "Manager")
            };
            foreach (var (email, password, role) in usersToCreate)
            {
                if (await userManager.FindByEmailAsync(email) == null)
                {
                    var user = new IdentityUser { UserName = email, Email = email, EmailConfirmed = true };
                    var result = await userManager.CreateAsync(user, password);
                    if (result.Succeeded)
                    {
                        await userManager.AddToRoleAsync(user, role);
                        Console.WriteLine($"Created user {email} with role {role}");
                    }
                }
            }
        }

        public static async Task SeedRoles(IServiceProvider serviceProvider, UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            //Create Roles if they dont exist
            string[] roleNames = { "Admin", "Manager", "User" };
            foreach (var rolename in roleNames)
            {
                var roleExist = await roleManager.RoleExistsAsync(rolename);
                if (!roleExist)
                {
                    await roleManager.CreateAsync(new IdentityRole(rolename));
                }
            }

            //Craate an Admin user if it doesnt exist
            var adminUser = await userManager.FindByEmailAsync("admin@example.com");
            if (adminUser == null)
            {
                adminUser = new IdentityUser { UserName = "admin@example.com", Email = "admin@example.com", EmailConfirmed = true };
                await userManager.CreateAsync(adminUser, "Admin@123");

            }
            //Add admin role if not already assigned
            if(!await userManager.IsInRoleAsync(adminUser, "Admin"))
            {
                await userManager.AddToRoleAsync(adminUser, "Admin");
            }
        }
        public static async Task SeedAllAsync(IServiceProvider serviceProvider,UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            var context = serviceProvider.GetRequiredService<ApplicationDbContext>();
            
            await SeedRoles(serviceProvider, userManager, roleManager);
            await SeedUsersAsync(userManager);
            await SeedRoomsAsync(context);
            await SeedBookingsAsync(context, userManager);
            await SeedStaffAsync(serviceProvider);
        }
    }
}
        
           
