using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Refrase.Model.Frames;
using Refrase.Model.Videos;

namespace Refrase.Model;

public class RefraseContext(DbContextOptions<RefraseContext> options) : DbContext(options)
{
	public DbSet<Video> Videos => Set<Video>();

	public DbSet<Frame> Frames => Set<Frame>();

	protected override void OnConfiguring(DbContextOptionsBuilder builder)
	{
		builder
			.UseSnakeCaseNamingConvention();
	}

	protected override void ConfigureConventions(ModelConfigurationBuilder builder)
	{
		Type[] idTypes = GetType().Assembly.ExportedTypes
			.Where(t => t.IsValueType && !t.IsEnum)
			.ToArray();

		foreach (Type idType in idTypes)
		{
			Type conversionType = idType.GetNestedTypes().Single(t => t.IsAssignableTo(typeof(ValueConverter)));
			builder.Properties(idType).HaveConversion(conversionType);
		}
	}

	protected override void OnModelCreating(ModelBuilder builder)
	{
		foreach (IMutableEntityType entityType in builder.Model.GetEntityTypes())
			foreach (IMutableForeignKey foreignKey in entityType.GetForeignKeys())
				foreignKey.DeleteBehavior = DeleteBehavior.Restrict;

		builder.ApplyConfigurationsFromAssembly(GetType().Assembly);
	}
}
