using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Backend.Data;
using Backend.Models;
using Microsoft.AspNetCore.Authorization;


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


        // GET: api/Privacy/gettext
        [HttpGet("gettext")]
        public async Task<IActionResult> GetPrivacy([FromQuery] string type)
        {
            try
            {
                var privacy = await _privacyDbContext.Texts
                    .Where(p => p.Type == type)
                    .Select(p => new
                    {
                        type = p.Type,
                        content = p.Content
                    })
                    .FirstOrDefaultAsync();

                if (privacy == null)
                {
                    return NotFound($"No privacy text found for type: {type}");
                }

                return Ok(privacy.content);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // POST: api/Privacy/updatetext
        [HttpPost("updatetext")]
        [Authorize(Policy= "BackOffice")]
        public async Task<IActionResult> PostPrivacy([FromBody] TextModelUpdate text)
        {
            // Check if the request body is empty
            if (text == null || string.IsNullOrWhiteSpace(text.Type) || string.IsNullOrWhiteSpace(text.Content))
            {
                return BadRequest("Both 'Type' and 'Content' fields are required.");
            }

            try
            {
                // Check if the text type exists in the database
                var existingText = await _privacyDbContext.Texts
                    .Where(p => p.Type == text.Type)
                    .FirstOrDefaultAsync();

                if (existingText == null)
                {
                    return NotFound($"No entry found for type: {text.Type}");
                }

                // Update the existing text content
                existingText.Content = text.Content;
                await _privacyDbContext.SaveChangesAsync();

                return Ok("Privacy text updated successfully.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // POST: api/Privacy/addtext
        // [HttpPost("addtext")]
        // public async Task<IActionResult> AddPrivacyText([FromBody] TextModelUpdate text)
        // {
        //     if (text == null || string.IsNullOrWhiteSpace(text.Type) || string.IsNullOrWhiteSpace(text.Content))
        //     {
        //         return BadRequest("Both 'Type' and 'Content' fields are required.");
        //     }

        //     try
        //     {
        //         var existingText = await _privacyDbContext.Texts
        //             .Where(p => p.Type == text.Type)
        //             .FirstOrDefaultAsync();

        //         if (existingText != null)
        //         {
        //             return BadRequest($"Text with type '{text.Type}' already exists.");
        //         }

        //         var newText = new Entities.Text
        //         {
        //             Type = text.Type,
        //             Content = text.Content
        //         };

        //         await _privacyDbContext.Texts.AddAsync(newText);
        //         await _privacyDbContext.SaveChangesAsync();

        //         return Ok("Privacy text added successfully.");
        //     }
        //     catch (Exception ex)
        //     {
        //         // Log the exception (optional: using a logging library)
        //         // _logger.LogError(ex, "Error adding privacy text.");
        //         return StatusCode(500, $"Internal server error: {ex.Message}");
        //     }
        // }
    }
}