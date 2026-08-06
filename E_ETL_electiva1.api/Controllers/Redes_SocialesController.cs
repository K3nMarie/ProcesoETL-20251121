using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using E_ETL_electiva1.api.Models;
using E_ETL_electiva1.api.context;

namespace E_ETL_electiva1.api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class Redes_SocialesController : ControllerBase
    {
        private readonly opiniones_de_clientesDBContext _context;

        public Redes_SocialesController(opiniones_de_clientesDBContext context)
        {
            _context = context;
        }

        // GET: api/Redes_Sociales
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Redes_Sociales>>> GetRedes_Sociales()
        {
            return await _context.Redes_Sociales.ToListAsync();
        }

        // GET: api/Redes_Sociales/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Redes_Sociales>> GetRedes_Sociales(int id)
        {
            var redes_Sociales = await _context.Redes_Sociales.FindAsync(id);

            if (redes_Sociales == null)
            {
                return NotFound();
            }

            return redes_Sociales;
        }

        // PUT: api/Redes_Sociales/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutRedes_Sociales(int id, Redes_Sociales redes_Sociales)
        {
            if (id != redes_Sociales.IdRedSocial)
            {
                return BadRequest();
            }

            _context.Entry(redes_Sociales).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!Redes_SocialesExists(id))
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

        // POST: api/Redes_Sociales
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Redes_Sociales>> PostRedes_Sociales(Redes_Sociales redes_Sociales)
        {
            _context.Redes_Sociales.Add(redes_Sociales);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetRedes_Sociales", new { id = redes_Sociales.IdRedSocial }, redes_Sociales);
        }

        // DELETE: api/Redes_Sociales/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRedes_Sociales(int id)
        {
            var redes_Sociales = await _context.Redes_Sociales.FindAsync(id);
            if (redes_Sociales == null)
            {
                return NotFound();
            }

            _context.Redes_Sociales.Remove(redes_Sociales);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool Redes_SocialesExists(int id)
        {
            return _context.Redes_Sociales.Any(e => e.IdRedSocial == id);
        }
    }
}
