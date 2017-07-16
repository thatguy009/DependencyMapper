using DependencyMapper.Models;
using System.Data.Entity;
using System.Net;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace DependencyMapper.Controllers
{
    public class SystemsController : Controller
    {
        private DependencyMapperContext db = new DependencyMapperContext();

        private async Task SetSystemList(int[] dependants = null, int[] dependancies = null)
        {
            var sysList = await db.Systems.Select(s => new { s.ID, s.Name }).ToListAsync();

            ViewBag.Dependants =
                new MultiSelectList(
                    sysList.OrderByDescending(s => s.Name),
                    "ID",
                    "Name",
                    dependants != null ? sysList.Where(s => dependants.Contains(s.ID)) : null
                    );

            ViewBag.Dependancies =
                new MultiSelectList(
                    sysList.OrderByDescending(s => s.Name),
                    "ID",
                    "Name",
                    dependancies != null ? sysList.Where(s => dependancies.Contains(s.ID)) : null
                    );
        }

        private async Task<Models.System> GetSystem(int id)
        {
            return await db.Systems
            .Include(s => s.Dependants)
            .Include(s => s.Dependancies)
            .Where(s => s.ID == id).SingleOrDefaultAsync();

        }

        // GET: Systems
        public async Task<ActionResult> Index()
        {
            return View(await db.Systems.ToListAsync());
        }

        // GET: Systems/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Models.System system = await GetSystem(id.Value);

            if (system == null)
            {
                return HttpNotFound();
            }

            return View(system);
        }

        // GET: Systems/Create
        public async Task<ActionResult> Create()
        {
            await SetSystemList();
            return View();
        }

        // POST: Systems/Create
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Name,Description,Tags")] Models.System system, int[] dependants, int[] dependancies)
        {
            if (ModelState.IsValid)
            {
                if (dependants != null)
                {
                    system.Dependants = await db.Systems.Where(s => dependants.Contains(s.ID)).ToListAsync();
                }

                if (dependancies != null)
                {
                    system.Dependancies = await db.Systems.Where(s => dependancies.Contains(s.ID)).ToListAsync();
                }

                db.Systems.Add(system);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            await SetSystemList(dependants, dependancies);

            return View(system);
        }

        // GET: Systems/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Models.System system = await GetSystem(id.Value);
            if (system == null)
            {
                return HttpNotFound();
            }

            await SetSystemList(
                system.Dependants?.Select(d => d.ID).ToArray(),
                system.Dependancies?.Select(d => d.ID).ToArray()
                );
            return View(system);
        }

        // POST: Systems/Edit/5
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "ID,Name,Description,Tags")] Models.System system, int[] dependants, int[] dependancies)
        {
            if (ModelState.IsValid)
            {
                if (dependants != null)
                {
                    system.Dependants = await db.Systems.Where(s => dependants.Contains(s.ID)).ToListAsync();
                }

                if (dependancies != null)
                {
                    system.Dependancies = await db.Systems.Where(s => dependancies.Contains(s.ID)).ToListAsync();
                }

                db.Entry(system).State = EntityState.Modified;
                await db.SaveChangesAsync();

                if (dependants != null)
                {
                    system.Dependants.RemoveAll(d => !dependants.Contains(d.ID));
                }

                if (dependancies != null)
                {
                    system.Dependancies.RemoveAll(d => !dependancies.Contains(d.ID));
                }
                await db.SaveChangesAsync();

                return RedirectToAction("Index");
            }
            await SetSystemList(dependants, dependancies);

            return View(system);
        }

        // GET: Systems/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DependencyMapper.Models.System system = await db.Systems.FindAsync(id);
            if (system == null)
            {
                return HttpNotFound();
            }
            return View(system);
        }

        // POST: Systems/Delete/5
        [HttpPost, ActionName("Delete"), ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            var system = await GetSystem(id);
            if (system.Dependants != null)
            {
                foreach (var item in system.Dependants)
                {
                    item.Dependancies.Remove(system);
                }
            }

            if (system.Dependancies != null)
            {
                foreach (var item in system.Dependancies)
                {
                    item.Dependants.Remove(system);
                }
            }
            await db.SaveChangesAsync();
            db.Systems.Remove(system);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
