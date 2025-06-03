using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore;

namespace ECOSOL.API.Data
{
    public class EcosolDbContextFactory : IDesignTimeDbContextFactory<EcosolDbContext>
    {
        public EcosolDbContext CreateDbContext(string[] args)
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.Development.json", optional: true)
                .AddJsonFile("appsettings.json", optional: true)
                .AddEnvironmentVariables()
                .Build();

            var optionsBuilder = new DbContextOptionsBuilder<EcosolDbContext>();

            var connectionString = configuration.GetConnectionString("DefaultConnection");

            optionsBuilder.UseNpgsql(connectionString); // Altere se usar outro banco, ex: UseSqlServer

            return new EcosolDbContext(optionsBuilder.Options);
        }
    }
}
