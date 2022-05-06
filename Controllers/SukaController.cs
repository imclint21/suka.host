using Microsoft.AspNetCore.Mvc;
using Suka.Services;

namespace Suka.Controllers;

[Route("")]
public class SukaController : Controller
{
	private readonly SukaService _sukaService;

	public SukaController(SukaService sukaService) => _sukaService = sukaService;

	[HttpGet("")]
	public ActionResult Index() => View();

	[HttpGet("{filename}")]
	public ActionResult HttpGet(string filename)
	{
		var retrievedFile = _sukaService.RetrieveFile(filename);
		if (retrievedFile?.TemporaryFile == null) return NotFound();
		var bytes = System.IO.File.ReadAllBytes(retrievedFile.TemporaryFile);
		return File(bytes, retrievedFile.ContentType ?? "application/octet-stream",  string.Concat(retrievedFile.Uid.ToString()[..8], retrievedFile.Extension));
	}

	[HttpPost]
	[Route("")]
	public async Task<ActionResult> UploadCatcher()
	{
		var hostedFile = await _sukaService.ProcessFileAsync(Request);
		return Ok(Url.Action("HttpGet", "Suka", new { filename = string.Concat(hostedFile.Uid.ToString()[..8], hostedFile.Extension) }, "https"));
	}

	[HttpPut]
	[Route("{**catch}")]
	public async Task<ActionResult> UploadCatcher(string @catch)
	{
		var hostedFile = await _sukaService.ProcessFileAsync(Request, @catch);
		return Ok(Url.Action("HttpGet", "Suka", new { filename = string.Concat(hostedFile.Uid.ToString()[..8], hostedFile.Extension) }, "https"));
	}
}