using Microsoft.AspNetCore.Identity;
using WebApiDotNet.Data.Entities;

namespace WebApiDotNet.Data
{
    public class Seeder
    {
        public async Task SeedAsync(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<UserEntity>>();

            var email = "elcin.b200488@gmail.com";
            var user = await userManager.FindByEmailAsync(email);

            if (user == null)
            {
                user = new UserEntity
                {
                    UserName = email,
                    Email = email,
                    EmailConfirmed = true
                };

                // Хешує пароль "123456" за всіма стандартами Identity
                await userManager.CreateAsync(user, "123456");
            }
        }
    }
}
