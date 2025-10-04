namespace Refrase.Core.Videos;

public class ReEncodingProgress
{
	public int CurrentFrame { get; private set; }

	public int? TotalFrames { get; private set; }

	public void Prepare(int? totalFrames)
	{
		CurrentFrame = 0;
		TotalFrames = totalFrames;
	}

	public void Report(string output)
	{
		KeyValuePair<string, string>[] lines = output
			.Split(Environment.NewLine)
			.Select(l =>
			{
				string[] parts = l.Split('=');
				return new KeyValuePair<string, string>(parts.First(), parts.Last());
			})
			.ToArray();

		if (lines.FirstOrDefault() is ("frame", string value) && int.TryParse(value, out int frame))
			CurrentFrame = frame;

		if (lines.LastOrDefault() is ("progress", "end"))
			CurrentFrame = 0;
	}
}
