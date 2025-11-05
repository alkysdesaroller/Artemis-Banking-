using ArtemisBanking.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace ArtemisBanking.Infrastructure.Persistence
{
    public class ArtemisContextFactory : IDesignTimeDbContextFactory<ArtemisContext>
    {
        public ArtemisContext CreateDbContext(string[] args)
        {
            // Buscar el appsettings.json del proyecto WebApp (1 nivel arriba)
            var basePath = Path.Combine(Directory.GetCurrentDirectory(), "../ArtemisBanking");

            var configuration = new ConfigurationBuilder()
                .SetBasePath(basePath)
                .AddJsonFile("appsettings.json", optional: false)
                .Build();

            var connectionString = configuration.GetConnectionString("DefaultConnection");

            var optionsBuilder = new DbContextOptionsBuilder<ArtemisContext>();
            optionsBuilder.UseSqlServer(connectionString);

            return new ArtemisContext(optionsBuilder.Options);
        }
    }
}
