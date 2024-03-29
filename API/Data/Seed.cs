using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using API.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    public class Seed
    {
      public static async Task ClearConnection(DataContext context)
      {
        context.Connections.RemoveRange(context.Connections);
        await context.SaveChangesAsync();
      }
        public static async Task SeedUsers(UserManager<AppUser> userManager, RoleManager<AppRole> roleManager)
        {
          if(await userManager.Users.AnyAsync()) return;   

          var userData = await System.IO.File.ReadAllTextAsync("Data/UserSeedData.json");
          var users = JsonSerializer.Deserialize<List<AppUser>>(userData);
          var roles = new List<AppRole> 
          {
            new AppRole{Name = "Member"},
            new AppRole{Name = "Admin"},
            new AppRole{Name = "Moderator"}
          };
          foreach(var role in roles)
          {
            await roleManager.CreateAsync(role);
          }
          foreach(var user in users)
          {
            user.UserName = user.UserName.ToLower();
            user.Created = DateTime.SpecifyKind(user.Created, DateTimeKind.Utc);
            user.LastActive = DateTime.SpecifyKind(user.LastActive, DateTimeKind.Utc);
            user.DateOfBirth = DateTime.SpecifyKind(user.DateOfBirth, DateTimeKind.Utc);
            await userManager.CreateAsync(user, "Password1");
            await userManager.AddToRoleAsync(user, "Member");
          }
          var admin = new AppUser{
            UserName = "admin"
          };
          await userManager.CreateAsync(admin,"Password1");
          await userManager.AddToRolesAsync(admin, new[] {"Admin", "Moderator"});
        }
    }
}