namespace Refrase.Core;

public class RefraseOptions
{
	public const string Section = "Refrase";

	public string? DataDirectory { get; set; }

	public string? PushoverUser { get; set; }

	public string? PushoverToken { get; set; }
}
