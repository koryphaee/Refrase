namespace Refrase.Core;

public class RefraseException : Exception
{
	public RefraseException(string? message) : base(message)
	{
	}

	public RefraseException(string? message, Exception? innerException) : base(message, innerException)
	{
	}
}
