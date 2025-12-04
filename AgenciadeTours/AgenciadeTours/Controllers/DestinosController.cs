using AgenciadeTours.Data;
using AgenciadeTours.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Text;

namespace AgenciadeTours.Controllers
{
    public class DestinosController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DestinosController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Lista()
        {
            return View(await _context.Destinos.OrderBy(p => p.DestinoID).Include(x => x.Pais).ToListAsync());
        }

        public IActionResult Crear()
        {
            ViewBag.PaisID = new SelectList(_context.Paises, "PaisID", "Nombre");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Crear(Destino destino)
        {

            if (await _context.Destinos.AnyAsync(d => d.Nombre == destino.Nombre && d.PaisID == destino.PaisID))
            {
                ModelState.AddModelError("Nombre", "Ya existe un destino con este nombre en este país.");
            }

            if (ModelState.IsValid)
            {
                ViewBag.PaisID = new SelectList(_context.Paises, "PaisID", "Nombre", destino.PaisID);
                return View(destino);
            }

            _context.Add(destino);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Lista));
        }

        public async Task<IActionResult> Editar(int? id)
        {
            if (id == null) return NotFound();
            var destino = await _context.Destinos.FindAsync(id);
            if (destino == null) return NotFound();

            ViewBag.PaisID = new SelectList(_context.Paises, "PaisID", "Nombre", destino.PaisID);
            return View(destino);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Editar(int id, Destino destino)
        {
            if (id != destino.DestinoID) return NotFound();

            if (await _context.Destinos.AnyAsync(d => d.Nombre == destino.Nombre && d.PaisID == destino.PaisID && d.DestinoID != destino.DestinoID))
            {
                ModelState.AddModelError("Nombre", "Ya existe otro destino con ese nombre en este país.");
            }

            if (ModelState.IsValid)
            {
                ViewBag.PaisID = new SelectList(_context.Paises, "PaisID", "Nombre", destino.PaisID);
                return View(destino);
            }

            _context.Update(destino);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Lista));
        }

        public async Task<IActionResult> Eliminar(int? id)
        {
            if (id == null) return NotFound();
            var destino = await _context.Destinos.Include(a => a.Pais).FirstOrDefaultAsync(a => a.DestinoID == id);
            if (destino == null) return NotFound();
            return View(destino);
        }

        [HttpPost, ActionName("Eliminar")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ConfirmarEliminacion(int id)
        {
            try
            {
                var destino = await _context.Destinos.FindAsync(id);

                if (destino == null) return NotFound();

                _context.Destinos.Remove(destino);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Error al eliminar el destino: " + ex.Message);
                var destino = await _context.Destinos.FindAsync(id);
                return View(destino);
            }

            return RedirectToAction(nameof(Lista));
        }

        public async Task<IActionResult> ExportarCSV()
        {
            var lista = await _context.Destinos.Include(x => x.Pais).ToListAsync();
            var sb = new StringBuilder();

            sb.AppendLine("DestinoID,Nombre,Pais,Dias_Duracion,Horas_Duracion");

            foreach (var d in lista)
            {
                sb.AppendLine($"{d.DestinoID},{d.Nombre},{d.Pais.Nombre},{d.Dias_Duracion},{d.Horas_Duracion}");
            }

            return File(Encoding.UTF8.GetBytes(sb.ToString()), "text/csv", "Destinos.csv");
        }
    }
}