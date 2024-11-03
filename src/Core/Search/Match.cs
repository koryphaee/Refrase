using Refrase.Core.Frames;
using Refrase.Model.Videos;

namespace Refrase.Core.Search;

public record Match(
	Video Video,
	FrameInfo Frame,
	double Similarity);
