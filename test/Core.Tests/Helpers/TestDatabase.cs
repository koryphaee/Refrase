using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Refrase.Model;

namespace Refrase.Core.Tests.Helpers;

public sealed class TestDatabase : IDbContextFactory<RefraseContext>, IAsyncDisposable
{
	private readonly SqliteConnection connection;

	private TestDatabase(SqliteConnection connection)
	{
		this.connection = connection;
	}

	public static async Task<TestDatabase> Create(string dataSource = ":memory:")
	{
		string connectionString = $"Data Source = {dataSource}";
		var connection = new SqliteConnection(connectionString);
		await connection.OpenAsync();
		return new TestDatabase(connection);
	}

	public RefraseContext CreateDbContext()
	{
		var builder = new DbContextOptionsBuilder<RefraseContext>();
		builder.UseSqlite(connection);

		var context = new RefraseContext(builder.Options);
		context.Database.EnsureCreated();
		return context;
	}

	public async ValueTask DisposeAsync()
	{
		await connection.DisposeAsync();
	}
}
