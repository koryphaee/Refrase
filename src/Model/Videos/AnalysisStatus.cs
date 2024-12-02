namespace Refrase.Model.Videos;

public enum AnalysisStatus
{
	Scheduled = 0,
	ReEncoded = 1,
	MetadataImported = 2,
	FramesCreated = 3,
	FramesHashed = 4,
	Completed = 5
}
