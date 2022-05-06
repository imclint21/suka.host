namespace Suka.Models;

public class HostedFile
{
	public Guid Uid { get; set; }
	public DateTime CreationDateTime { get; set; }
	public string? TemporaryFile { get; set; }
	public string? ContentType { get; set; }
	public string? Extension { get; set; }
}