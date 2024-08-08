using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;
using WebApplication1.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication1.Controllers
{
    [Route("api/items")]
    [ApiController]
    public class ItemsApiController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<ItemsApiController> _logger;

        public ItemsApiController(ApplicationDbContext context, ILogger<ItemsApiController> logger)
        {
            _context = context;
            _logger = logger;
        }
        [HttpGet("test")]
        public IActionResult Test()
        {
            return Ok("API is working!");
        }
        // GET: api/items
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Item>>> GetItems()
        {
            _logger.LogInformation("GetItems called.");
            var items = await _context.Items.ToListAsync();
            _logger.LogInformation($"Retrieved {items.Count} items.");
            return items;
        }
        [HttpPost("upload")]
        public async Task<IActionResult> UploadItemImage(IFormFile image, [FromBody] Item item)
        {
            if (image == null || image.Length == 0)
            {
                return BadRequest("Image is not selected");
            }

            using (var memoryStream = new MemoryStream())
            {
                await image.CopyToAsync(memoryStream);
                item.ItemImage = memoryStream.ToArray();
            }

            _context.Items.Add(item);
            await _context.SaveChangesAsync();

            return Ok(new { Message = "Image uploaded successfully" });
        }

        // GET: api/items/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetItem(int id)
        {
            var item = await _context.Items.FindAsync(id);
            if (item == null)
            {
                return NotFound();
            }

            var itemDto = new
            {
                item.Id,
                item.Name,
                item.Brand,
                item.Price,
                item.Category,
                item.SubCategory,
                item.Description,
                item.Condition,
                item.Latitude,
                item.Longitude,
                item.Status,
                item.ViewCount,
                item.LikeCount,
                item.UserId,

                ItemImage = item.ItemImage != null ? Convert.ToBase64String(item.ItemImage) : null
            };

            return Ok(itemDto);
        }
        [HttpPost] 
        public async Task<ActionResult<Item>> PostItem(Item item)
        {
            _context.Items.Add(item);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetItem", new { id = item.Id }, item);
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> PutItem(int id, Item item)
        {
            if (id != item.Id)
            {
                return BadRequest();
            }

            _context.Entry(item).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
               
            }
            return NoContent();
        }
        [HttpDelete("{id}")]
        public IActionResult DeleteItem(int id)
        {
            var item = _context.Items.Find(id);
            if (item == null)
            {
                return NotFound();
            }

            _context.Items.Remove(item);
            _context.SaveChanges();

            return NoContent();
        }
    }
}
