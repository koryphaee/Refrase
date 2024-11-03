using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Refrase.Model.Frames;

internal class FrameConfiguration : IEntityTypeConfiguration<Frame>
{
	public void Configure(EntityTypeBuilder<Frame> builder)
	{
		builder
			.HasIndex(f => new { f.VideoId, f.Index })
			.IsUnique();
	}
}
