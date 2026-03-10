using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SalterEFModels.EFModels;

namespace SalterWebApi.Areas.Forum.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ForumBoardsController : ControllerBase
    {
        private readonly SalterDbContext _context;

        public ForumBoardsController(SalterDbContext context)
        {
            _context = context;
        }

        // GET: api/ForumBoards
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ForumBoardCategory>>> GetForumBoardCategories()
        {
            return await _context.ForumBoardCategories.ToListAsync();
        }

        // GET: api/ForumBoards/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ForumBoardCategory>> GetForumBoardCategory(int id)
        {
            var forumBoardCategory = await _context.ForumBoardCategories.FindAsync(id);

            if (forumBoardCategory == null)
            {
                return NotFound();
            }

            return forumBoardCategory;
        }

        // PUT: api/ForumBoards/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutForumBoardCategory(int id, ForumBoardCategory forumBoardCategory)
        {
            if (id != forumBoardCategory.BoardId)
            {
                return BadRequest();
            }

            _context.Entry(forumBoardCategory).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ForumBoardCategoryExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/ForumBoards
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<ForumBoardCategory>> PostForumBoardCategory(ForumBoardCategory forumBoardCategory)
        {
            _context.ForumBoardCategories.Add(forumBoardCategory);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetForumBoardCategory", new { id = forumBoardCategory.BoardId }, forumBoardCategory);
        }

        // DELETE: api/ForumBoards/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteForumBoardCategory(int id)
        {
            var forumBoardCategory = await _context.ForumBoardCategories.FindAsync(id);
            if (forumBoardCategory == null)
            {
                return NotFound();
            }

            _context.ForumBoardCategories.Remove(forumBoardCategory);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ForumBoardCategoryExists(int id)
        {
            return _context.ForumBoardCategories.Any(e => e.BoardId == id);
        }
    }
}
