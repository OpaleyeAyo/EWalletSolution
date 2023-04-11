using Microsoft.AspNetCore.Identity;
using System;
using System.Threading.Tasks;

namespace EWallet.DataLayer.Data
{
    public static class SeedIdentityRoles
    {
        public static async Task SeedRoles(RoleManager<IdentityRole> roleManager)
        {
            Console.WriteLine("Seeding roles database");

            string[] userRoles = new string[] { "Customer", "Admin", "SuperAdmin"};

            foreach (var userRole in userRoles)
            {
                //check if the role exist
                bool roleExist = await roleManager.RoleExistsAsync(userRole);

                if (!roleExist)
                {
                    await roleManager.CreateAsync(new IdentityRole(userRole));
                    Console.WriteLine($"Role: {userRole} added");
                }
            }

            Console.WriteLine("Seeding roles database completed...");
        }

        public static async Task SeedSuperAdmin(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            Console.WriteLine("Begin Seeding SuperAdmin...");

            //Check if the record exist
            var existingSuperAdmin = await userManager.FindByEmailAsync("heryourbabs@gmail.com");

            //check if the role exist
            bool roleExist = await roleManager.RoleExistsAsync("SuperAdmin");

            if (existingSuperAdmin == null && roleExist)
            {
                Console.WriteLine("Creating Super admin account...");

                IdentityUser SuperAdmin = new IdentityUser()
                {
                    Email = "heryourbabs@gmail.com",
                    UserName = "heryourbabs@gmail.com",
                    EmailConfirmed = true
                };

                var newSuperAdmin = await userManager.CreateAsync(SuperAdmin, "Pa55w0rd@123");

                if (newSuperAdmin.Succeeded)
                {
                    Console.WriteLine("Super admin account created.");

                    //Add role
                    var addRole = await userManager.AddToRoleAsync(SuperAdmin, "SuperAdmin");

                    if (addRole.Succeeded)
                    {
                        Console.WriteLine("Role added to Super admin.");
                    }
                }  
            }

            Console.WriteLine("Seeding SuperAdmin completed...");
        }
    }
}
