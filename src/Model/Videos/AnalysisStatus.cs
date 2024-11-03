namespace Refrase.Model.Videos;

public enum AnalysisStatus
{
	Scheduled = 0,
	MetadataImported = 1,
	FramesCreated = 2,
	FramesHashed = 3,
	Completed = 4
}
