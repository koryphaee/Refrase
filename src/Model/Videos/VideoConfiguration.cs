using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Refrase.Model.Videos;

internal class VideoConfiguration : IEntityTypeConfiguration<Video>
{
	public void Configure(EntityTypeBuilder<Video> builder)
	{
		builder
			.HasIndex(f => f.Name)
			.IsUnique();
	}
}
