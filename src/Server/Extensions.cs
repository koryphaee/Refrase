using Microsoft.EntityFrameworkCore;
using Refrase.Core.Paths;
using Refrase.Model;

namespace Refrase.Server;

public static class Extensions
{
	public static async Task Migrate(this IHost host)
	{
		await using RefraseContext context = await host.Services
			.GetRequiredService<IDbContextFactory<RefraseContext>>()
			.CreateDbContextAsync();
		await context.Database.MigrateAsync();
	}

	public static void AddModel(this IServiceCollection services)
	{
		services.AddDbContextFactory<RefraseContext>(ConfigureDbContext);
	}

	private static void ConfigureDbContext(IServiceProvider serviceProvider, DbContextOptionsBuilder dbContextOptionsBuilder)
	{
		DataPaths dataPaths = serviceProvider.GetRequiredService<DataPaths>();
		string connectionString = "Data Source = " + dataPaths.Database;
		dbContextOptionsBuilder.UseSqlite(connectionString);
	}
}
