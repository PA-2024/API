﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API_GesSIgn.Models;

namespace API_GesSIgn.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SubjectsHourController : ControllerBase
    {
        private readonly MonDbContext _context;

        public SubjectsHourController(MonDbContext context)
        {
            _context = context;
        }

        // GET: api/SubjectsHour
        [HttpGet]
        public async Task<ActionResult<IEnumerable<SubjectsHour>>> GetSubjectsHours()
        {
            return await _context.SubjectsHour
                .Include(s => s.SubjectsHour_Sectors)
                .ToListAsync();
        }

        // GET: api/SubjectsHour/5
        [HttpGet("{id}")]
        public async Task<ActionResult<SubjectsHour>> GetSubjectsHour(int id)
        {
            var subjectsHour = await _context.SubjectsHour
                .Include(s => s.SubjectsHour_Sectors)
                .FirstOrDefaultAsync(s => s.SubjectsHour_Id == id);

            if (subjectsHour == null)
            {
                return NotFound();
            }

            return subjectsHour;
        }

        // POST: api/SubjectsHour
        [HttpPost]
        public async Task<ActionResult<SubjectsHour>> PostSubjectsHour(SubjectsHour subjectsHour)
        {
            _context.SubjectsHour.Add(subjectsHour);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetSubjectsHour", new { id = subjectsHour.SubjectsHour_Id }, subjectsHour);
        }

        // PUT: api/SubjectsHour/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutSubjectsHour(int id, SubjectsHour subjectsHour)
        {
            if (id != subjectsHour.SubjectsHour_Id)
            {
                return BadRequest();
            }

            _context.Entry(subjectsHour).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SubjectsHourExists(id))
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

        // DELETE: api/SubjectsHour/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSubjectsHour(int id)
        {
            var subjectsHour = await _context.SubjectsHour.FindAsync(id);
            if (subjectsHour == null)
            {
                return NotFound();
            }

            _context.SubjectsHour.Remove(subjectsHour);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool SubjectsHourExists(int id)
        {
            return _context.SubjectsHour.Any(e => e.SubjectsHour_Id == id);
        }
    }
}