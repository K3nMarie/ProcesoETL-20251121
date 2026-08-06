
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
    public class Fuente_DatosController : ControllerBase
    {
        private readonly opiniones_de_clientesDBContext _context;

        public Fuente_DatosController(opiniones_de_clientesDBContext context)
        {
            _context = context;
        }

        // GET: api/Fuente_Datos
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Fuente_Datos>>> GetFuente_Datos()
        {
            return await _context.Fuente_Datos.ToListAsync();
        }

        // GET: api/Fuente_Datos/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Fuente_Datos>> GetFuente_Datos(string id)
        {
            var fuente_Datos = await _context.Fuente_Datos.FindAsync(id);

            if (fuente_Datos == null)
            {
                return NotFound();
            }

            return fuente_Datos;
        }

        // PUT: api/Fuente_Datos/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutFuente_Datos(string id, Fuente_Datos fuente_Datos)
        {
            if (id != fuente_Datos.IdFuente)
            {
                return BadRequest();
            }

            _context.Entry(fuente_Datos).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!Fuente_DatosExists(id))
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

        // POST: api/Fuente_Datos
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Fuente_Datos>> PostFuente_Datos(Fuente_Datos fuente_Datos)
        {
            _context.Fuente_Datos.Add(fuente_Datos);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (Fuente_DatosExists(fuente_Datos.IdFuente))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetFuente_Datos", new { id = fuente_Datos.IdFuente }, fuente_Datos);
        }

        // DELETE: api/Fuente_Datos/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteFuente_Datos(string id)
        {
            var fuente_Datos = await _context.Fuente_Datos.FindAsync(id);
            if (fuente_Datos == null)
            {
                return NotFound();
            }

            _context.Fuente_Datos.Remove(fuente_Datos);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool Fuente_DatosExists(string id)
        {
            return _context.Fuente_Datos.Any(e => e.IdFuente == id);
        }
    }
}
