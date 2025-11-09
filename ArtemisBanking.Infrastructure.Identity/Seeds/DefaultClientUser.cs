using ArtemisBanking.Core.Domain.Common.Enums;
using ArtemisBanking.Infrastructure.Identity.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace ArtemisBanking.Infrastructure.Identity.Seeds
{
    public static class DefaultClientUser
    {
        public static async Task SeedAsync(UserManager<AppUser> userManager)
        {
            AppUser user = new()
            {
                IdentityCardNumber = "00000000004",
                FirstName = "Juan",
                LastName = "Client",
                Email = "Client@email.com",
                EmailConfirmed = true,
                PhoneNumberConfirmed = true,
                UserName = "client"
            };

            if (await userManager.Users.AllAsync(u => u.Id != user.Id))
            {
                var entityUser = await userManager.FindByEmailAsync(user.Email);
                if(entityUser == null)
                {
                    await userManager.CreateAsync(user, "123Pa$$word!");
                    await userManager.AddToRoleAsync(user, nameof(Roles.Client));
                }
            }
       
        }
    }
}
