using AgenciadeTours.Data;
using AgenciadeTours.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text;

namespace AgenciadeTours.Controllers
{
    public class PaisesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PaisesController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Lista()
        {
            return View(await _context.Paises.OrderBy(p => p.Nombre).ToListAsync());
        }

        public IActionResult Crear()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Crear(Pais pais)
        {
            if (await _context.Paises.AnyAsync(p => p.Nombre == pais.Nombre))
            {
                ModelState.AddModelError("Nombre", "Ya existe un país con ese nombre.");
            }

            if (ModelState.IsValid)
            {
                return View(pais);
            }

            _context.Add(pais);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Lista));
        }

        public async Task<IActionResult> Editar(int? id)
        {
            if (id == null) return NotFound();
            var pais = await _context.Paises.FindAsync(id);
            if (pais == null) return NotFound();
            return View(pais);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Editar(int id, Pais pais)
        {
            if (id != pais.PaisID) return NotFound();

            if (await _context.Paises.AnyAsync(p => p.Nombre == pais.Nombre && p.PaisID != id))
                ModelState.AddModelError("Nombre", "Ya existe otro país con ese nombre.");

            if (ModelState.IsValid)
            {
                return View(pais);
            }

            try
            {
                _context.Update(pais);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PaisExiste(pais.PaisID))
                    return NotFound();
                else
                    throw;
            }

            return RedirectToAction(nameof(Lista));
        }

        public async Task<IActionResult> Eliminar(int? id)
        {
            if (id == null) return NotFound();
            var pais = await _context.Paises.FirstOrDefaultAsync(m => m.PaisID == id);
            if (pais == null) return NotFound();
            return View(pais);
        }

        [HttpPost, ActionName("Eliminar")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ConfirmarEliminacion(int id)
        {
            try
            {
                var pais = await _context.Paises
                    .Include(p => p.Destinos)
                    .FirstOrDefaultAsync(p => p.PaisID == id);

                if (pais == null) return NotFound();

                if (pais.Destinos.Any())
                {
                    ModelState.AddModelError("", "No se puede eliminar un país con uno o varios destinos asociados.");
                    return View(pais);
                }

                _context.Paises.Remove(pais);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Error al eliminar el país: " + ex.Message);
                var pais = await _context.Paises.FindAsync(id);
                return View(pais);
            }

            return RedirectToAction(nameof(Lista));
        }

        public async Task<IActionResult> ExportarCSV()
        {
            var lista = await _context.Paises.ToListAsync();
            var sb = new StringBuilder();

            sb.AppendLine("PaisID,Nombre");

            foreach (var p in lista)
            {
                sb.AppendLine($"{p.PaisID},{p.Nombre}");
            }

            return File(Encoding.UTF8.GetBytes(sb.ToString()), "text/csv", "Paises.csv");
        }
        private bool PaisExiste(int id)
        {
            return _context.Paises.Any(e => e.PaisID == id);
        }
    }
}