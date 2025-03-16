using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MusicXmlDb.Server.ScoreDocuments;
using MusicXmlDb.Server.Users;

namespace MusicXmlDb.Server.MusicXmlDocuments;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class MusicXmlDocumentsController : ControllerBase
{
    private readonly ScoreDocumentRepository scoreDocumentRepository;

    public MusicXmlDocumentsController(ScoreDocumentRepository scoreDocumentRepository)
    {
        this.scoreDocumentRepository = scoreDocumentRepository;
    }

    [HttpGet("{scoreDocumentId}")]
    public async Task<IActionResult> GetScoreDocument(Guid scoreDocumentId)
    {
        var user = ApplicationUser.CreateLoggedInUser(User);
        if (user == null)
        {
            return Unauthorized();
        }

        var xmlDocument = await scoreDocumentRepository.GetLatestMusicXmlDocumentAsync(user.Id, scoreDocumentId);
        if (xmlDocument == null)
        {
            return NotFound();
        }

        return Content(xmlDocument.Content, "application/xml");
    }

    [HttpGet("{scoreDocumentId}/{historyId}")]
    public async Task<IActionResult> GetScoreDocument(Guid scoreDocumentId, Guid historyId)
    {
        var user = ApplicationUser.CreateLoggedInUser(User);
        if (user == null)
        {
            return Unauthorized();
        }

        var xmlDocument = await scoreDocumentRepository.GetMusicXmlDocumentAsync(user.Id, scoreDocumentId, historyId);
        if (xmlDocument == null)
        {
            return NotFound();
        }

        return Content(xmlDocument.Content, "application/xml");
    }
}


