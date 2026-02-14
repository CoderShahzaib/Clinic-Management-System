using ClinicManagementSystem.Core.Domain.IdentityEntities;
using Microsoft.AspNetCore.Identity;

namespace ClinicManagementSystem.Infrastructure.Data.Seed
{
    public static class IdentitySeeder
    {
        public static async Task SeedAdminAsync(
            UserManager<ApplicationUser> userManager,
            RoleManager<ApplicationRole> roleManager)
        {
            string adminEmail = "admin@clinic.com";
            string adminPassword = "Admin@123";

            if (!await roleManager.RoleExistsAsync("Admin"))
            {
                await roleManager.CreateAsync(new ApplicationRole { Name = "Admin" });
            }

            var admin = await userManager.FindByEmailAsync(adminEmail);
            if (admin == null)
            {
                admin = new ApplicationUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    PersonName = "System Admin"
                };

                await userManager.CreateAsync(admin, adminPassword);
                await userManager.AddToRoleAsync(admin, "Admin");
            }
        }

        public static async Task SeedDoctorRoleAsync(
            RoleManager<ApplicationRole> roleManager)
        {
            if (!await roleManager.RoleExistsAsync("Doctor"))
            {
                await roleManager.CreateAsync(new ApplicationRole
                {
                    Name = "Doctor"
                });
            }
        }
    }
}
