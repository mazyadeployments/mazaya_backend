using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using MMA.WebApi.DataAccess.Models;
using System.IO;

namespace MMA.WebApi
{
    public class DbContextFactory : IDesignTimeDbContextFactory<MMADbContext>
    {
        public MMADbContext CreateDbContext(string[] args)
        {
            IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

            var builder = new DbContextOptionsBuilder<MMADbContext>();

            var connectionString = configuration.GetConnectionString("Database");

            builder.UseSqlServer(connectionString, b => b.MigrationsAssembly("MMA.WebApi.DataAccess"));

            return new MMADbContext(builder.Options);
        }
    }
}
