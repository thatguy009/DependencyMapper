using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using DependencyMapper.Models;

namespace DependencyMapper.Controllers
{
    public class LinksController : ApiController
    {
        private DependencyMapperContext db = new DependencyMapperContext();

        // GET: api/Links
        [ResponseType(typeof(List<Link>))]
        public async Task<IHttpActionResult> GetAllLinks()
        {
            var systems = await db.Systems
            .Include(s => s.Dependants)
            .Include(s => s.Dependancies)
            .ToListAsync();

            var ret = new List<Link>();

            foreach (var system in systems)
            {
                var links = GetLinkList(system);

                if (links != null && links.Count > 0)
                {
                    ret.AddRange(links);
                }
            }

            return Ok(ret);
        }

        // GET: api/Links/5
        [ResponseType(typeof(List<Link>))]
        public async Task<IHttpActionResult> GetLinks(int id)
        {
            Models.System system = await GetSystem(id);

            if (system == null)
            {
                return NotFound();
            }

            var links = GetLinkList(system);
            

            return Ok(links);
        }

        private List<Link> GetLinkList(Models.System system)
        {
            List<Link> links = new List<Link>();

            if (system.Dependants != null)
            {
                links.AddRange(
                    system.Dependants?
                    .Select(s =>
                    new Link()
                    {
                        Source = s.Name,
                        Target = system.Name,
                        Type = "Dependant"
                    })
                    );
            }

            if (system.Dependancies != null)
            {
                links.AddRange(
                    system.Dependancies?
                    .Select(s =>
                    new Link()
                    {
                        Source = system.Name,
                        Target = s.Name,
                        Type = "Dependancy"
                    })
                    );
            }
            return links;
        }

        private async Task<Models.System> GetSystem(int id)
        {
            return await db.Systems
            .Include(s => s.Dependants)
            .Include(s => s.Dependancies)
            .Where(s => s.ID == id).SingleOrDefaultAsync();

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