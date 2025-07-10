using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using sistema_padron_electoral.Data;
using sistema_padron_electoral.Models;
using System.IO;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authorization;

namespace sistema_padron_electoral.Controllers
{
    [Authorize(Policy = "AdminPadronOnly")]
    [ApiController]
    [Route("api/[controller]")]
    public class Votantes1Controller : ControllerBase
    {
        private readonly sistema_padron_electoralContext _context;

        public Votantes1Controller(sistema_padron_electoralContext context)
        {
            _context = context;
        }

        // GET: api/Votantes1
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Votante>>> GetVotante()
        {
            try
            {
                return await _context.Votante.ToListAsync();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al obtener votantes: {ex.Message}");
            }
        }

        // GET: api/Votantes1/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Votante>> GetVotante(Guid id)
        {
            try
            {
                var votante = await _context.Votante.FindAsync(id);
                if (votante == null)
                {
                    return NotFound();
                }
                return votante;
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al obtener votante: {ex.Message}");
            }
        }

        // PUT: api/Votantes1/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutVotante(
            Guid id,
            [FromForm] string? CI,
            [FromForm] string? NombreCompleto,
            [FromForm] string? Direccion,
            [FromForm] int? RecintoId,
            [FromForm] string? RecintoNombre,
            IFormFile fotoAnverso,
            IFormFile fotoReverso,
            IFormFile fotoVotante)
        {
            try
            {
                var votante = await _context.Votante.FindAsync(id);
                if (votante == null) return NotFound();

                if (CI != null) votante.CI = CI;
                if (NombreCompleto != null) votante.NombreCompleto = NombreCompleto;
                if (Direccion != null) votante.Direccion = Direccion;
                if (RecintoId.HasValue) votante.RecintoId = RecintoId.Value;
                if (RecintoNombre != null) votante.RecintoNombre = RecintoNombre;

                if (fotoAnverso != null) votante.FotoCarnetAnverso = await SaveFileAsync(fotoAnverso);
                if (fotoReverso != null) votante.FotoCarnetReverso = await SaveFileAsync(fotoReverso);
                if (fotoVotante != null) votante.FotoVotante = await SaveFileAsync(fotoVotante);

                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al actualizar votante: {ex.Message}");
            }
        }

        // POST: api/Votantes1
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Votante>> PostVotante(
            [FromForm] string CI,
            [FromForm] string NombreCompleto,
            [FromForm] string Direccion,
            [FromForm] int RecintoId,
            [FromForm] string RecintoNombre,
            IFormFile fotoAnverso,
            IFormFile fotoReverso,
            IFormFile fotoVotante)
        {
            try
            {
                var votante = new Votante
                {
                    Id = Guid.NewGuid(),
                    CI = CI,
                    NombreCompleto = NombreCompleto,
                    Direccion = Direccion,
                    RecintoId = RecintoId,
                    RecintoNombre = RecintoNombre,
                    FotoCarnetAnverso = await SaveFileAsync(fotoAnverso),
                    FotoCarnetReverso = await SaveFileAsync(fotoReverso),
                    FotoVotante = await SaveFileAsync(fotoVotante)
                };

                _context.Votante.Add(votante);
                await _context.SaveChangesAsync();

                return CreatedAtAction("GetVotante", new { id = votante.Id }, votante);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al crear votante: {ex.Message}");
            }
        }

        // DELETE: api/Votantes1/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteVotante(Guid id)
        {
            try
            {
                var votante = await _context.Votante.FindAsync(id);
                if (votante == null)
                {
                    return NotFound();
                }

                DeleteFileIfExists(votante.FotoCarnetAnverso);
                DeleteFileIfExists(votante.FotoCarnetReverso);
                DeleteFileIfExists(votante.FotoVotante);

                _context.Votante.Remove(votante);
                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al eliminar votante: {ex.Message}");
            }
        }

        [AllowAnonymous]
        [HttpGet("consulta-publica/{ci}")]
        public async Task<ActionResult<object>> ConsultaPublicaPorCI(string ci)
        {
            try
            {
                var votante = await _context.Votante
                    .Where(v => v.CI == ci)
                    .Select(v => new {
                        v.NombreCompleto,
                        v.RecintoNombre,
                        v.RecintoId,
                        v.FotoVotante
                    })
                    .FirstOrDefaultAsync();
                if (votante == null)
                {
                    return NotFound(new { mensaje = "No se encontró un votante con ese CI." });
                }
                return Ok(votante);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al consultar el padrón: {ex.Message}");
            }
        }

        [AllowAnonymous]
        [HttpGet("por-recinto/{recintoId}")]
        public async Task<ActionResult<IEnumerable<object>>> GetVotantesPorRecinto(int recintoId)
        {
            try
            {
                var votantes = await _context.Votante
                    .Where(v => v.RecintoId == recintoId)
                    .Select(v => new {
                        v.Id,
                        v.NombreCompleto,
                        v.CI
                    })
                    .OrderBy(v => v.NombreCompleto)
                    .ToListAsync();
                
                return Ok(votantes);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al obtener votantes del recinto: {ex.Message}");
            }
        }

        private bool VotanteExists(Guid id)
        {
            return _context.Votante.Any(e => e.Id == id);
        }

        private async Task<string> SaveFileAsync(IFormFile file)
        {
            try
            {
                if (file == null)
                {
                    return null;
                }
                var uploadsPath = Path.Combine(Directory.GetCurrentDirectory(), "uploads", "votantes");
                if (!Directory.Exists(uploadsPath))
                {
                    Directory.CreateDirectory(uploadsPath);
                }

                var fileName = Guid.NewGuid() + Path.GetExtension(file.FileName);
                var filePath = Path.Combine(uploadsPath, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }
                return $"uploads/votantes/{fileName}";
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al guardar archivo: {ex.Message}", ex);
            }
        }
        private void DeleteFileIfExists(string relativePath)
        {
            if (string.IsNullOrEmpty(relativePath)) return;
            var fullPath = Path.Combine(Directory.GetCurrentDirectory(), relativePath.Replace("/", Path.DirectorySeparatorChar.ToString()));
            if (System.IO.File.Exists(fullPath))
            {
                System.IO.File.Delete(fullPath);
            }
        }
    }
}
