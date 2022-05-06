using Microsoft.AspNetCore.StaticFiles;
using MimeTypes;
using Suka.Models;

namespace Suka.Services;

public class SukaService
{
	private readonly List<HostedFile> _hostedFiles = new();
	
	// public IEnumerable<HostedFile> Dump() => _hostedFiles;

	/// <summary>
	/// Store a new file
	/// </summary>
	public async Task<HostedFile> ProcessFileAsync(HttpRequest request, string? @catch = null)
	{
		var temporaryFile = Path.GetTempFileName();
		
		// Store the bytes to a new temporary file
		var streamContent = new StreamContent(request.Body);
		var bytes = await streamContent.ReadAsByteArrayAsync();
		await File.WriteAllBytesAsync(temporaryFile, bytes);
		
		var contentType = GetContentType(request, @catch);
		
		// Create `HostedFile` model
		var hostedFile = new HostedFile
		{
			Uid = Guid.NewGuid(),
			CreationDateTime = DateTime.UtcNow,
			TemporaryFile = temporaryFile,
			ContentType = contentType,
			Extension = MimeTypeMap.GetExtension(contentType)
		};

		// Add the new file into the list
		_hostedFiles.Add(hostedFile);

		return hostedFile;
	}

	/// <summary>
	/// Retrive a file with his UID or a filename
	/// </summary>
	public HostedFile? RetrieveFile(string filename)
	{
		var uid = Path.GetFileNameWithoutExtension(filename);
		return _hostedFiles.FirstOrDefault(x => x.Uid.ToString().StartsWith(uid));
	}

	private static string? GetContentType(HttpRequest request, string? @catch)
	{
		switch (request.Method.ToUpper())
		{
			default:
				return request.Headers["Content-Type"];
			
			case "PUT":
				var contentType = string.Empty;
				if (@catch != null) new FileExtensionContentTypeProvider().TryGetContentType(@catch, out contentType);
				return contentType;
		}
	}
}