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
    public class OpinionesController : ControllerBase
    {
        private readonly opiniones_de_clientesDBContext _context;

        public OpinionesController(opiniones_de_clientesDBContext context)
        {
            _context = context;
        }

        // GET: api/Opiniones
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Opiniones>>> GetOpiniones()
        {
            return await _context.Opiniones.ToListAsync();
        }

        // GET: api/Opiniones/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Opiniones>> GetOpiniones(int id)
        {
            var opiniones = await _context.Opiniones.FindAsync(id);

            if (opiniones == null)
            {
                return NotFound();
            }

            return opiniones;
        }

        // PUT: api/Opiniones/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutOpiniones(int id, Opiniones opiniones)
        {
            if (id != opiniones.IdOpinion)
            {
                return BadRequest();
            }

            _context.Entry(opiniones).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!OpinionesExists(id))
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

        // POST: api/Opiniones
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Opiniones>> PostOpiniones(Opiniones opiniones)
        {
            _context.Opiniones.Add(opiniones);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetOpiniones", new { id = opiniones.IdOpinion }, opiniones);
        }

        // DELETE: api/Opiniones/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOpiniones(int id)
        {
            var opiniones = await _context.Opiniones.FindAsync(id);
            if (opiniones == null)
            {
                return NotFound();
            }

            _context.Opiniones.Remove(opiniones);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool OpinionesExists(int id)
        {
            return _context.Opiniones.Any(e => e.IdOpinion == id);
        }
    }
}
