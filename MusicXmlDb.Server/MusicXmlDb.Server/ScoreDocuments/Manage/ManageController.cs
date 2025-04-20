using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MusicXmlDb.Server.MusicXmlDocuments;
using MusicXmlDb.Server.Users;

namespace MusicXmlDb.Server.ScoreDocuments.Manage
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ManageController : ControllerBase
    {
        private readonly IMusicXmlValidator musicXmlValidator;
        private readonly IPrivateScoreDocumentRepository scoreDocumentRepository;

        public ManageController(IMusicXmlValidator musicXmlValidator, PrivateScoreDocumentRepository scoreDocumentRepository)
        {
            this.musicXmlValidator = musicXmlValidator;
            this.scoreDocumentRepository = scoreDocumentRepository;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ScoreDocument>>> GetScoreDocuments()
        {
            var user = ApplicationUser.CreateLoggedInUser(User);
            if (user == null)
            {
                return Unauthorized();
            }

            var scoreDocumentModels = await scoreDocumentRepository.GetScoreDocumentsWithHistoriesAsync(user.Id);
            return scoreDocumentModels;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ScoreDocument>> GetScoreDocument(Guid id)
        {
            var user = ApplicationUser.CreateLoggedInUser(User);
            if (user == null)
            {
                return Unauthorized();
            }

            var scoreDocumentModel = await scoreDocumentRepository.GetScoreDocumentWithHistoriesAsync(user.Id, id);
            if (scoreDocumentModel is null)
            {
                return NotFound();
            }

            return scoreDocumentModel;
        }

        [HttpGet("{scoreDocumentId}/{historyId}")]
        public async Task<ActionResult<MusicXmlDocument>> GetScoreDocument(Guid scoreDocumentId, Guid historyId)
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

            return xmlDocument;
        }

        [HttpGet("{scoreDocumentId:guid}/{historyId:guid}/download")]
        public async Task<IActionResult> DownloadScoreDocument(Guid scoreDocumentId, Guid historyId)
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

        [HttpPost]
        public async Task<ActionResult<PostScoreDocumentResponse>> PostScoreDocument([FromForm] string name, [FromForm] bool isPublic, IFormFile formFile)
        {
            var user = ApplicationUser.CreateLoggedInUser(User);
            if (user == null)
            {
                return Unauthorized();
            }

            string xmlContent;
            try
            {
                xmlContent = musicXmlValidator.Validate(formFile);
            }
            catch (MusicXmlValidationException ex)
            {
                return Problem(ex.Message);
            }
            catch (Exception ex)
            {
                return Problem(ex.Message);
            }

            var body = new PostScoreDocumentBody()
            {
                DocumentName = name,
                XmlString = xmlContent,
                IsPublic = isPublic,
            };

            try
            {
                var response = await scoreDocumentRepository.InsertScoreDocumentAsync(user.Id, body);
                return response;
            }
            catch (Exception ex)
            {
                return Problem(ex.Message);
            }
        }

        [HttpPost("{scoreDocumentId}")]
        public async Task<ActionResult<PostScoreDocumentHistoryReponse>> PostScoreDocument(Guid scoreDocumentId, IFormFile formFile)
        {
            var user = ApplicationUser.CreateLoggedInUser(User);
            if (user == null)
            {
                return Unauthorized();
            }

            string xmlContent;
            try
            {
                xmlContent = musicXmlValidator.Validate(formFile);
            }
            catch (MusicXmlValidationException ex)
            {
                return Problem(ex.Message);
            }
            catch (Exception ex)
            {
                return Problem(ex.Message);
            }

            var body = new PostMusicXmlBody()
            {
                ScoreDocumentId = scoreDocumentId,
                XmlString = xmlContent,
            };

            try
            {
                var response = await scoreDocumentRepository.AddScoreDocumentHistoryAsync(user.Id, body);
                return response;
            }
            catch (Exception ex)
            {
                return Problem(ex.Message);
            }
        }

        [HttpPut("{scoreDocumentId}")]
        public async Task<IActionResult> PutScoreDocument(Guid scoreDocumentId, PutScoreDocumentBody scoreDocumentModel)
        {
            if (scoreDocumentModel.Id != scoreDocumentId)
            {
                return BadRequest();
            }

            var user = ApplicationUser.CreateLoggedInUser(User);
            if (user == null)
            {
                return Unauthorized();
            }

            try
            {
                await scoreDocumentRepository.UpdateScoreDocumentAsync(user.Id, scoreDocumentModel);
            }
            catch (Exception ex)
            {
                return Problem(ex.Message);
            }

            return NoContent();
        }

        [HttpDelete("{scoreDocumentId}/{scoreDocumentHistoryId}")]
        public async Task<IActionResult> DeleteScoreDocumentHistory(Guid scoreDocumentId, Guid scoreDocumentHistoryId)
        {
            var user = ApplicationUser.CreateLoggedInUser(User);
            if (user == null)
            {
                return Unauthorized();
            }

            try
            {
                if (!await scoreDocumentRepository.DeleteScoreDocumentHistoryAsync(user.Id, scoreDocumentId, scoreDocumentHistoryId))
                {
                    return BadRequest("Cannot delete a history if there less than 2 histories to a score document.");
                }
            }
            catch (Exception ex)
            {
                return Problem(ex.Message);
            }

            return NoContent();
        }

        [HttpDelete("{scoreDocumentId}")]
        public async Task<IActionResult> DeleteScoreDocument(Guid scoreDocumentId)
        {
            var user = ApplicationUser.CreateLoggedInUser(User);
            if (user == null)
            {
                return Unauthorized();
            }

            try
            {
                if (!await scoreDocumentRepository.DeleteScoreDocumentAsync(user.Id, scoreDocumentId))
                {
                    return BadRequest("Cannot delete a history if there less than 2 histories to a score document.");
                }
            }
            catch (Exception ex)
            {
                return Problem(ex.Message);
            }

            return NoContent();
        }
    }
}
