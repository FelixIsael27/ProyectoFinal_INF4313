using AgenciadeTours.Data;
using AgenciadeTours.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Text;

namespace AgenciadeTours.Controllers
{
    public class ToursController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ToursController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Lista()
        {
            return View(await _context.Tours.Include(t => t.Pais).Include(t => t.Destino).ToListAsync());
        }

        public IActionResult Crear()
        {
            ViewBag.PaisID = new SelectList(_context.Paises, "PaisID", "Nombre");
            ViewBag.DestinoID = new SelectList(_context.Destinos, "DestinoID", "Nombre");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Crear(Tour tour)
        {
            if (_context.Tours.Any(t => t.TourID == tour.TourID))
            {
                ModelState.AddModelError("TourID", "El ID del tour ya existe.");
            }

            if (!_context.Paises.Any(p => p.PaisID == tour.PaisID))
            {
                ModelState.AddModelError("PaisID", "El país seleccionado no existe.");
            }

            var destino = _context.Destinos.FirstOrDefault(d => d.DestinoID == tour.DestinoID);
            if (destino == null)
            {
                ModelState.AddModelError("DestinoID", "El destino seleccionado no existe.");
            }

            if (tour.Fecha.Date < DateTime.Now.Date)
            {
                ModelState.AddModelError("Fecha", "La fecha no puede ser una fecha pasada.");
            }

            if (destino != null)
            {
                tour.Destino = destino;
                var pais = _context.Paises.Find(tour.PaisID);
                if (pais == null)
                {
                    ModelState.AddModelError("PaisID", "El país seleccionado no existe.");
                }
                else
                {
                    tour.Pais = pais;
                }
            }

            if (!ModelState.IsValid)
            {
                ViewBag.PaisID = new SelectList(_context.Paises, "PaisID", "Nombre", tour.PaisID);
                ViewBag.DestinoID = new SelectList(_context.Destinos, "DestinoID", "Nombre", tour.DestinoID);
                return View(tour);
            }

            _context.Add(tour);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Lista));
        }

        public async Task<IActionResult> Editar(int? id)
        {
            if (id == null) return NotFound("ID no proporcionado.");

            var tour = await _context.Tours.FindAsync(id);
            if (tour == null) return NotFound("El tour no existe.");

            ViewBag.PaisID = new SelectList(_context.Paises, "PaisID", "Nombre", tour.PaisID);
            ViewBag.DestinoID = new SelectList(_context.Destinos, "DestinoID", "Nombre", tour.DestinoID);
            return View(tour);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Editar(int? id, Tour tour)
        {
            if (id != tour.TourID)
            {
                return NotFound("El ID del tour no coincide.");
            }

            var tourDb = await _context.Tours.AsNoTracking().FirstOrDefaultAsync(t => t.TourID == id);
            if (tourDb == null)
            {
                ModelState.AddModelError("", "El Tour no existe.");
            }

            var destino = _context.Destinos.FirstOrDefault(d => d.DestinoID == tour.DestinoID);
            if (destino == null)
            {
                ModelState.AddModelError("DestinoID", "El destino seleccionado no existe.");
            }

            if (tour.Fecha.Date < DateTime.Now.Date)
            {
                ModelState.AddModelError("Fecha", "La fecha no puede ser pasada.");
            }

            if (destino != null)
            {
                tour.Destino = destino;
                var pais = _context.Paises.Find(tour.PaisID);
                if (pais == null)
                {
                    ModelState.AddModelError("PaisID", "El país seleccionado no existe.");
                }
                else
                {
                    tour.Pais = pais;
                }
            }

            if (!ModelState.IsValid)
            {
                ViewBag.PaisID = new SelectList(_context.Paises, "PaisID", "Nombre", tour.PaisID);
                ViewBag.DestinoID = new SelectList(_context.Destinos, "DestinoID", "Nombre", tour.DestinoID);
                return View(tour);
            }

            _context.Update(tour);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Lista));
        }

        public async Task<IActionResult> Eliminar(int? id)
        {
            if (id == null) return NotFound("ID no proporcionado");
            var tour = await _context.Tours.Include(t => t.Pais).Include(t => t.Destino).FirstOrDefaultAsync(m => m.TourID == id);
            if (tour == null) return NotFound("El tour no existe.");
            return View(tour);
        }

        [HttpPost, ActionName("Eliminar")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ConfirmarEliminacion(int id)
        {
            try
            {
                var tour = await _context.Tours.FindAsync(id);

                if (tour == null)
                {
                    return NotFound("El tour ya no existe.");
                }

                _context.Tours.Remove(tour);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Lista));
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Error al eliminar el tour: " + ex.Message;
                return View();
            }
        }

        public async Task<IActionResult> ExportarCSV()
        {
            try
            {
                var tours = await _context.Tours
                    .Include(t => t.Pais)
                    .Include(t => t.Destino)
                    .ToListAsync();

                var sb = new StringBuilder();
                sb.AppendLine("TourID,Nombre,Pais,Destino,Fecha,Hora,Precio,ITBIS,FechaHoraInicio,FechaHoraFin,Estado");

                foreach (var t in tours)
                {
                    sb.AppendLine(
                        $"{t.TourID}," +
                        $"{t.Nombre}," +
                        $"{t.Pais?.Nombre}," +
                        $"{t.Destino?.Nombre}," +
                        $"{t.Fecha:yyyy-MM-dd}," +
                        $"{t.Hora}," +
                        $"{t.Precio}," +
                        $"{t.ITBIS}," +
                        $"{t.FechaHoraInicio}," +
                        $"{t.FechaHoraFin}," +
                        $"{t.Estado}"
                    );
                }

                return File(Encoding.UTF8.GetBytes(sb.ToString()), "text/csv", "tours.csv");
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error al exportar el archivo: " + ex.Message;
                return RedirectToAction(nameof(Lista));
            }
        }
    }
}