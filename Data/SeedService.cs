using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WmsCore.Models;

namespace WmsCore.Data
{
    public class SeedService
    {
        public static async Task SeedDatabase(IServiceProvider serviceProvider) {
            using var scope = serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<WmsCoreContext>();
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();

                //Sprawdzenie czy baza jest gotowa
                await context.Database.EnsureCreatedAsync();

                //Dodanie ról
                await AddRoleAsync(roleManager, "Admin");
                await AddRoleAsync(roleManager, "User");
                await AddRoleAsync(roleManager, "Manager");
                


            //Dodanie administratora
            string adminLogin = "admin";
                var adminUser = await userManager.FindByNameAsync(adminLogin);
                if (adminUser == null) {
                    var newAdmin = new User
                    {
                        UserName = adminLogin,
                        FullName = "Administrator",
                        Email = "admin@tentative.pl"
                    };
                    var result = await userManager.CreateAsync(newAdmin, "admin");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(newAdmin, "Admin");
                }
                else {
                    throw new Exception($"Błąd kreacji bazy danych - nie udało sie utworzyć administratora systemu");
                }

                }
            //Seed domyślnej kategorii
                await AddCategoryIfNotExistsAsync(context, "Inne");
        }

        private static async Task AddRoleAsync(RoleManager<IdentityRole> roleManager, string roleName) {
            if (!await roleManager.RoleExistsAsync(roleName)) {

                var result = await roleManager.CreateAsync(new IdentityRole(roleName));
                if (!result.Succeeded) {
                    throw new Exception($"Błąd kreacji ról w bazie danych");
                }
            }
        
        }

        private static async Task AddCategoryIfNotExistsAsync(WmsCoreContext context, string categoryName)
        {
            // Sprawdzamy czy kategoria o takiej nazwie już istnieje
            var exists = await context.Category.AnyAsync(c => c.Name == categoryName);

            if (!exists)
            {
                var category = new Category
                {
                    Name = categoryName,
                };

                await context.Category.AddAsync(category);
                await context.SaveChangesAsync(); 
            }
        }
    }
}
