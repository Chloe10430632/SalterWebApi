using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SalterEFModels.EFModels;

namespace SalterWebApi.Areas.Experience
{
    [Route("api/[controller]")]
    [ApiController]
    public class ExpCoachesController : ControllerBase
    {
        private readonly SalterDbContext _context;

        public ExpCoachesController(SalterDbContext context)
        {
            _context = context;
        }

        // GET: api/ExpCoaches
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ExpCoach>>> GetExpCoaches()
        {
            return await _context.ExpCoaches.ToListAsync();
        }

        // GET: api/ExpCoaches/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ExpCoach>> GetExpCoach(int id)
        {
            var expCoach = await _context.ExpCoaches.FindAsync(id);

            if (expCoach == null)
            {
                return NotFound();
            }

            return expCoach;
        }

        // PUT: api/ExpCoaches/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutExpCoach(int id, ExpCoach expCoach)
        {
            if (id != expCoach.Id)
            {
                return BadRequest();
            }

            _context.Entry(expCoach).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ExpCoachExists(id))
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

        // POST: api/ExpCoaches
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<ExpCoach>> PostExpCoach(ExpCoach expCoach)
        {
            _context.ExpCoaches.Add(expCoach);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetExpCoach", new { id = expCoach.Id }, expCoach);
        }

        // DELETE: api/ExpCoaches/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteExpCoach(int id)
        {
            var expCoach = await _context.ExpCoaches.FindAsync(id);
            if (expCoach == null)
            {
                return NotFound();
            }

            _context.ExpCoaches.Remove(expCoach);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ExpCoachExists(int id)
        {
            return _context.ExpCoaches.Any(e => e.Id == id);
        }
    }
}
