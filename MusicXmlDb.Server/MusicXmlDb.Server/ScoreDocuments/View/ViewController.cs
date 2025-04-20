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
        private readonly IPublicScoreDocumentRepository _scoreDocumentRepository;

        public ViewController(PublicScoreDocumentRepository scoreDocumentRepository)
        {
            this._scoreDocumentRepository = scoreDocumentRepository;
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<ScoreDocument>> GetScoreDocument(Guid id)
        {
            var user = ApplicationUser.CreateLoggedInUser(User);

            var scoreDocumentModel = await _scoreDocumentRepository.GetScoreDocumentWithHistoriesAsync(user?.Id, id);
            if (scoreDocumentModel is null)
            {
                return NotFound();
            }

            return scoreDocumentModel;
        }

        [HttpGet("{scoreDocumentId:guid}/{historyId:guid}")]
        public async Task<IActionResult> GetScoreDocument(Guid scoreDocumentId, Guid historyId)
        {
            var user = ApplicationUser.CreateLoggedInUser(User);

            var xmlDocument = await _scoreDocumentRepository.GetMusicXmlDocumentAsync(user?.Id, scoreDocumentId, historyId);
            if (xmlDocument == null)
            {
                return NotFound();
            }

            return Content(xmlDocument.Content, "application/xml");
        }
    }
}
