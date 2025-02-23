using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MusicXmlDb.Server.MusicXmlDocuments;
using MusicXmlDb.Server.Users;

namespace MusicXmlDb.Server.ScoreDocuments
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ScoreDocumentsController : ControllerBase
    {
        private readonly IMusicXmlValidator musicXmlValidator;
        private readonly ScoreDocumentContext scoreDocumentContext;
        private readonly MusicXmlDocumentContext musicXmlDocumentContext;

        public ScoreDocumentsController(IMusicXmlValidator musicXmlValidator, ScoreDocumentContext scoreDocumentContext, MusicXmlDocumentContext musicXmlDocumentContext)
        {
            this.musicXmlValidator = musicXmlValidator;
            this.scoreDocumentContext = scoreDocumentContext;
            this.musicXmlDocumentContext = musicXmlDocumentContext;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ScoreDocumentModel>>> GetScoreDocuments()
        {
            var user = ApplicationUser.CreateLoggedInUser(User);
            if (user == null)
            {
                return Unauthorized();
            }

            var scoreDocument = await scoreDocumentContext.ScoreDocuments
                .Where(e => e.UserId == user.Id)
                .ToListAsync();
            var scoreDocumentModels = scoreDocument.Select(e => ScoreDocumentModel.Create(e, user)).ToList();
            return scoreDocumentModels;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ScoreDocumentModel>> GetScoreDocument(Guid id)
        {
            var user = ApplicationUser.CreateLoggedInUser(User);
            if (user == null)
            {
                return Unauthorized();
            }

            var scoreDocument = await scoreDocumentContext.ScoreDocuments
                .Where(e => e.UserId == user.Id)
                .Include(e => e.History)
                .FirstOrDefaultAsync(e => e.Id == id);

            if (scoreDocument == null)
            {
                return NotFound();
            }

            var scoreDocumentModel = ScoreDocumentModel.Create(scoreDocument, user);
            return scoreDocumentModel;
        }

        [HttpPost]
        public async Task<ActionResult<ScoreDocumentModel>> PostScoreDocument([FromForm] string name, IFormFile formFile)
        {
            var user = ApplicationUser.CreateLoggedInUser(User);
            if (user == null)
            {
                return Unauthorized();
            }

            var scoreDocumentId = Guid.NewGuid();

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

            var xmlDocument = new MusicXmlDocument()
            {
                Id = Guid.NewGuid(),
                Content = xmlContent,
            };

            var scoreDocumentHistory = new ScoreDocumentHistory()
            {
                ScoreDocumentId = scoreDocumentId,
                Created = DateTime.UtcNow,
                Id = Guid.NewGuid(),
                UserId = user.Id,
                MusicXmlId = xmlDocument.Id
            };

            var scoreDocument = new ScoreDocument()
            {
                Id = scoreDocumentId,
                UserId = user.Id,
                Name = name,
                History = [scoreDocumentHistory],
                IsPublic = false,
                Created = DateTime.Now,
                Modified = DateTime.Now,
                Views = 0
            };

            await musicXmlDocumentContext.AddAsync(xmlDocument);
            await musicXmlDocumentContext.SaveChangesAsync();

            await scoreDocumentContext.ScoreDocuments.AddAsync(scoreDocument);
            await scoreDocumentContext.ScoreDocumentHistories.AddAsync(scoreDocumentHistory);
            await scoreDocumentContext.SaveChangesAsync();

            var scoreDocumentModel = ScoreDocumentModel.Create(scoreDocument, user);
            return scoreDocumentModel;
        }

        [HttpPost("{id}")]
        public async Task<ActionResult<ScoreDocumentHistoryModel>> PostScoreDocument(Guid id, IFormFile formFile)
        {
            var user = ApplicationUser.CreateLoggedInUser(User);
            if (user == null)
            {
                return Unauthorized();
            }

            var scoreDocument = await scoreDocumentContext.ScoreDocuments
                .Where(e => e.UserId == user.Id)
                .Include(e => e.History)
                .FirstOrDefaultAsync(e => e.Id == id);

            if (scoreDocument == null)
            {
                return NotFound();
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

            var xmlDocument = new MusicXmlDocument()
            {
                Id = Guid.NewGuid(),
                Content = xmlContent,
            };

            var scoreDocumentHistory = new ScoreDocumentHistory()
            {
                ScoreDocumentId = id,
                Created = DateTime.UtcNow,
                Id = Guid.NewGuid(),
                UserId = user.Id,
                MusicXmlId = xmlDocument.Id
            };

            await musicXmlDocumentContext.AddAsync(xmlDocument);
            await musicXmlDocumentContext.SaveChangesAsync();

            await scoreDocumentContext.ScoreDocumentHistories.AddAsync(scoreDocumentHistory);
            await scoreDocumentContext.SaveChangesAsync();

            var scoreDocumentHistoryModel = ScoreDocumentHistoryModel.Create(scoreDocumentHistory);
            return scoreDocumentHistoryModel;
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutScoreDocument(Guid id, PutScoreDocumentBody scoreDocumentModel)
        {
            if (scoreDocumentModel.Id != id)
            {
                return BadRequest();
            }

            var user = ApplicationUser.CreateLoggedInUser(User);
            if (user == null)
            {
                return Unauthorized();
            }

            var scoreDocument = await scoreDocumentContext.ScoreDocuments
                .Where(e => e.UserId == user.Id)
                .Include(e => e.History)
                .FirstOrDefaultAsync(e => e.Id == id);

            if (scoreDocument == null)
            {
                return NotFound();
            }

            scoreDocument.Name = scoreDocumentModel.Name;
            scoreDocument.IsPublic = scoreDocumentModel.IsPublic;
            scoreDocument.Modified = DateTime.UtcNow;

            scoreDocumentContext.ScoreDocuments.Update(scoreDocument);
            await scoreDocumentContext.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteScoreDocument(Guid id)
        {
            var user = ApplicationUser.CreateLoggedInUser(User);
            if (user == null)
            {
                return Unauthorized();
            }

            var scoreDocument = await scoreDocumentContext.ScoreDocuments
                .Where(e => e.UserId == user.Id)
                .Include(e => e.History)
                .FirstOrDefaultAsync(e => e.Id == id);
            if (scoreDocument == null)
            {
                return NotFound();
            }

            foreach (var version in scoreDocument.History)
            {
                var versionEntity = new MusicXmlDocument()
                {
                    Id = version.MusicXmlId,
                };
                musicXmlDocumentContext.MusicXmlDocuments.Attach(versionEntity);
                musicXmlDocumentContext.MusicXmlDocuments.Remove(versionEntity);
            }

            scoreDocumentContext.ScoreDocuments.Remove(scoreDocument);

            await musicXmlDocumentContext.SaveChangesAsync();
            await scoreDocumentContext.SaveChangesAsync();

            return NoContent();
        }
    }
}
