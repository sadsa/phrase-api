using Microsoft.AspNetCore.Mvc;
using PhraseApi.Core.Entities;
using PhraseApi.Infrastructure.Data;

namespace PhraseApi.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PhrasesController : ControllerBase
    {
        private readonly PhrasesDbContext _context;

        public PhrasesController(PhrasesDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Phrase phrase)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            phrase.CreatedAt = DateTime.UtcNow;
            phrase.IsActive = true;

            _context.Phrases.Add(phrase);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(Create), new { id = phrase.Id }, phrase);
        }
    }
}