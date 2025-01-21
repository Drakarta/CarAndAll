using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Backend.Data;
using Backend.Models;


namespace Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PrivacyController : ControllerBase
    {
        private readonly ApplicationDbContext _privacyDbContext;

        public PrivacyController(ApplicationDbContext context)
        {
            _privacyDbContext = context;
        }

        [HttpGet("gettext")]
        public async Task<IActionResult> GetPrivacy()
        {
            try
            {
                var privacy = await _privacyDbContext.Text.Select(p => new
                {
                    type = p.type,
                    content = p.content
                })
                .ToListAsync();

                return Ok(privacy);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost("updatetext")]
        public async Task<IActionResult> PostPrivacy([FromBody] TextModelUpdate text)
        {
            try
            {
                var Text = await _privacyDbContext.Text
                    .Where(p => p.type == text.Type).FirstOrDefaultAsync();
                    
                if (Text == null)
                {
                    return NotFound("type not found");
                }

                Text.content = text.Content;
                await _privacyDbContext.SaveChangesAsync();
                
                return Ok("Privacy text updated");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}