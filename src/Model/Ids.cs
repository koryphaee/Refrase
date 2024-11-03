using Strongly;

[assembly: StronglyDefaults(
	backingType: StronglyType.Long,
	converters: StronglyConverter.SystemTextJson | StronglyConverter.EfValueConverter | StronglyConverter.SwaggerSchemaFilter,
	implementations: StronglyImplementations.IComparable | StronglyImplementations.IEquatable | StronglyImplementations.IFormattable | StronglyImplementations.Parsable,
	cast: StronglyCast.None,
	math: StronglyMath.All)]

namespace Refrase.Model;

[Strongly]
public partial struct VideoId;

[Strongly]
public partial struct FrameId;
