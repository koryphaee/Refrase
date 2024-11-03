using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Refrase.Model;

internal class DesignTimeContextFactory : IDesignTimeDbContextFactory<RefraseContext>
{
	public RefraseContext CreateDbContext(string[] args)
	{
		DbContextOptionsBuilder<RefraseContext> builder = new();
		builder.UseSqlite("Data Source = :memory:");
		return new RefraseContext(builder.Options);
	}
}