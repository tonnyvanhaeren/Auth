using IdentityServer4.EntityFramework.DbContexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Threading.Tasks;
using web.Data;

namespace web.StartupHelpers
{
    internal static class DatabaseExtensions
    {
        internal static async Task EnsureDbUpToDateAsync(this IHost host)
        {
            using (var scope = host.Services.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<AuthDbContext>();
                await context.Database.MigrateAsync();

                var grantDbContext = scope.ServiceProvider.GetRequiredService<PersistedGrantDbContext>();
                await grantDbContext.Database.MigrateAsync();
            }
        }
    }
}
