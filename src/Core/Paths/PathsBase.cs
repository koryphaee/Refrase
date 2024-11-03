namespace Refrase.Core.Paths;

public abstract class PathsBase
{
	protected PathsBase(string parent, string id)
	{
		Id = id;
		Directory = Path.Combine(parent, id);
		System.IO.Directory.CreateDirectory(Directory);
	}

	public string Id { get; set; }

	public string Directory { get; }

	protected string Nested(string name)
	{
		return Path.Combine(Directory, name);
	}
}
