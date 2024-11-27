using Refrase.Model.Frames;

namespace Refrase.Model.Videos;

public class Video
{
	public long Id { get; set; }

	public required string Name { get; set; }

	public required string Category { get; set; }

	public required DateTime Imported { get; set; }

	public DateTime? Analyzed { get; set; }

	public string? Url { get; set; }

	public AnalysisStatus Status { get; set; }

	public int Width { get; set; }

	public int Height { get; set; }

	public int FrameCount { get; set; }

	public float FrameRate { get; set; }

	public TimeSpan Duration { get; set; }

	public ICollection<Frame> Frames { get; set; } = [];
}
