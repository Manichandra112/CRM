//using CRM_Backend.Data;
//using CRM_Backend.Domain.Entities;
//using CRM_Backend.Entities;
//using Microsoft.AspNetCore.Identity;
//using Microsoft.EntityFrameworkCore;
//using System;

//namespace CRM_Backend.Data.Seed;

//public static class AdminSeeder
//{
//    public static async Task SeedAsync(AppDbContext context)
//    {
//        if (await context.Users.AnyAsync(u => u.Username == "admin"))
//            return; // already seeded

//        var passwordHasher = new PasswordHasher<User>();

//        var admin = new User
//        {
//            Username = "admin",
//            Email = "admin@gmail.com",
//            IsActive = true,
//            CreatedAt = DateTime.UtcNow,
//            PasswordHash = "", // will set below
//            Profile = new UserProfile
//            {
//                FirstName = "admin",
//                LastName = "admin",
//                MobileNumber = "9000001111",
//                Department = "admin",
//                Designation = "admin"
//            },
//            UserRoles = new List<UserRole>()
//        };

//        admin.PasswordHash = passwordHasher.HashPassword(admin, "Temp@123");

//        var adminRole = await context.Roles.FirstAsync(r => r.Code == "Admin");

//        admin.UserRoles.Add(new UserRole
//        {
//            RoleId = adminRole.Id,
//            User = admin
//        });

//        context.Users.Add(admin);
//        await context.SaveChangesAsync();
//    }
//}
