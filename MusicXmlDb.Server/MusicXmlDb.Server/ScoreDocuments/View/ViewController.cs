using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MusicXmlDb.Server.MusicXmlDocuments;
using MusicXmlDb.Server.Users;

namespace MusicXmlDb.Server.ScoreDocuments.View
{
    [Route("api/[controller]")]
    [ApiController]
    public class ViewController : ControllerBase
    {
        private readonly IMusicXmlValidator musicXmlValidator;
        private readonly PublicScoreDocumentRepository scoreDocumentRepository;

        public ViewController(IMusicXmlValidator musicXmlValidator, PublicScoreDocumentRepository scoreDocumentRepository)
        {
            this.musicXmlValidator = musicXmlValidator;
            this.scoreDocumentRepository = scoreDocumentRepository;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ScoreDocument>> GetScoreDocument(Guid id)
        {
            var user = ApplicationUser.CreateLoggedInUser(User);

            var scoreDocumentModel = await scoreDocumentRepository.GetScoreDocumentWithHistoriesAsync(id);
            if (scoreDocumentModel is null)
            {
                return NotFound();
            }

            if (!scoreDocumentModel.IsPublic)
            {
                if (user?.Id != scoreDocumentModel.UserId)
                {
                    return Unauthorized();
                }
            }

            return scoreDocumentModel;
        }

        [HttpGet("{scoreDocumentId}/{historyId}")]
        public async Task<IActionResult> GetScoreDocument(Guid scoreDocumentId, Guid historyId)
        {
            var user = ApplicationUser.CreateLoggedInUser(User);
            if (user == null)
            {
                return Unauthorized();
            }

            var xmlDocument = await scoreDocumentRepository.GetMusicXmlDocumentAsync(scoreDocumentId, historyId);
            if (xmlDocument == null)
            {
                return NotFound();
            }

            return Content(xmlDocument.Content, "application/xml");
        }
    }
}
