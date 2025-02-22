using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MusicXmlDb.Server.ScoreDocuments;
using MusicXmlDb.Server.Users;

namespace MusicXmlDb.Server.MusicXmlDocuments;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class MusicXmlDocumentsController : ControllerBase
{
    private readonly UserManager<ApplicationUser> userManager;
    private readonly ScoreDocumentContext scoreDocumentContext;
    private readonly MusicXmlDocumentContext musicXmlDocumentContext;

    public MusicXmlDocumentsController(UserManager<ApplicationUser> userManager, ScoreDocumentContext scoreDocumentContext, MusicXmlDocumentContext musicXmlDocumentContext)
    {
        this.userManager = userManager;
        this.scoreDocumentContext = scoreDocumentContext;
        this.musicXmlDocumentContext = musicXmlDocumentContext;
    }

    [HttpGet("{scoreDocumentId}")]
    public async Task<IActionResult> GetScoreDocument(Guid scoreDocumentId)
    {
        var user = await userManager.GetUserAsync(User);
        if (user == null)
        {
            return Unauthorized();
        }

        var scoreDocument = scoreDocumentContext.ScoreDocuments
            .Where(e => e.UserId == user.Id)
            .Include(e => e.History)
            .FirstOrDefault(e => e.Id == scoreDocumentId);
        if (scoreDocument == null)
        {
            return NotFound();
        }

        var scoreDocumentHistory = scoreDocument.History
            .OrderByDescending(e => e.Created)
            .FirstOrDefault();
        if (scoreDocumentHistory == null)
        {
            return NotFound();
        }

        var xmlDocument = await musicXmlDocumentContext.MusicXmlDocuments
            .FirstOrDefaultAsync(e => e.Id == scoreDocumentHistory.MusicXmlId);
        if (xmlDocument == null)
        {
            return NotFound();
        }

        scoreDocument.Views++;
        scoreDocumentContext.ScoreDocuments.Update(scoreDocument);
        await scoreDocumentContext.SaveChangesAsync();

        return Content(xmlDocument.Content, "application/xml");
    }

    [HttpGet("{scoreDocumentId}/{historyId}")]
    public async Task<IActionResult> GetScoreDocument(Guid scoreDocumentId, Guid historyId)
    {
        var user = await userManager.GetUserAsync(User);
        if (user == null)
        {
            return Unauthorized();
        }

        var scoreDocument = scoreDocumentContext.ScoreDocuments
            .Where(e => e.UserId == user.Id)
            .Include(e => e.History)
            .FirstOrDefault(e => e.Id == scoreDocumentId);
        if (scoreDocument == null)
        {
            return NotFound();
        }

        var scoreDocumentHistory = scoreDocument.History
            .FirstOrDefault(e => e.Id == historyId);
        if (scoreDocumentHistory == null)
        {
            return NotFound();
        }

        var xmlDocument = await musicXmlDocumentContext.MusicXmlDocuments
            .FirstOrDefaultAsync(e => e.Id == scoreDocumentHistory.MusicXmlId);
        if (xmlDocument == null)
        {
            return NotFound();
        }

        scoreDocument.Views++;
        scoreDocumentContext.ScoreDocuments.Update(scoreDocument);
        await scoreDocumentContext.SaveChangesAsync();

        return Content(xmlDocument.Content, "application/xml");
    }
}


