using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MusicXmlDb.Server.MusicXmlDocuments;
using MusicXmlDb.Server.Users;

namespace MusicXmlDb.Server.ScoreDocuments.Profile
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProfileController : ControllerBase
    {
        private readonly IProfileScoreDocumentRepository _scoreDocumentRepository;

        public ProfileController(ProfileScoreDocumentRepository scoreDocumentRepository)
        {
            this._scoreDocumentRepository = scoreDocumentRepository;
        }

        [HttpGet("{userId}")]
        public async Task<ActionResult<IEnumerable<ScoreDocument>>> GetScoreDocuments(string userId)
        {
            var scoreDocumentModels = await _scoreDocumentRepository.GetPublicScoreDocumentsForProfileAsync(userId);

            return scoreDocumentModels;
        }
    }
}
