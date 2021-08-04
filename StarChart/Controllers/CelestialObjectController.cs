using System.Linq;
using Microsoft.AspNetCore.Mvc;
using StarChart.Data;
using StarChart.Models;

namespace StarChart.Controllers
{
    [Route("")]
    [ApiController]
    public class CelestialObjectController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public CelestialObjectController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("{id:int}", Name = "GetById")]
        public IActionResult GetById(int id)
        {
            var celestialObject = _context.CelestialObjects.Find(id);
            if (celestialObject == null) return NotFound();

            celestialObject.Satellites = _context.CelestialObjects.Where(obj => obj.OrbitedObjectId == id).ToList();

            return Ok(celestialObject);
        }

        [HttpGet("{name}")]
        public IActionResult GetByName(string name)
        {
            var celestialObjects = _context.CelestialObjects.Where(e => e.Name == name);
            if (!celestialObjects.Any()) return NotFound();

            foreach (var celestialObject in celestialObjects)
                celestialObject.Satellites = _context.CelestialObjects
                    .Where(obj => obj.OrbitedObjectId == celestialObject.Id).ToList();

            return Ok(celestialObjects.ToList());
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            var celestialObjects = _context.CelestialObjects;

            foreach (var celestialObject in celestialObjects)
                celestialObject.Satellites = _context.CelestialObjects
                    .Where(obj => obj.OrbitedObjectId == celestialObject.Id).ToList();

            return Ok(celestialObjects.ToList());
        }

        [HttpPost]
        public IActionResult Create([FromBody] CelestialObject celestialObject)
        {
            _context.CelestialObjects.Add(celestialObject);
            return CreatedAtRoute("GetById", new {celestialObject.Id}, celestialObject);
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, CelestialObject celestialObject)
        {
            var celestial = _context.CelestialObjects.Find(id);
            if (celestial == null) return NotFound();

            celestial.Name = celestialObject.Name;
            celestial.OrbitalPeriod = celestialObject.OrbitalPeriod;
            celestial.OrbitedObjectId = celestialObject.OrbitedObjectId;
            _context.CelestialObjects.Update(celestial);
            _context.SaveChanges();
            return NoContent();
        }

        [HttpPatch("{id}/{name}")]
        public IActionResult RenameObject(int id, string name)
        {
            var celestial = _context.CelestialObjects.Find(id);
            if (celestial == null) return NotFound();
            celestial.Name = name;
            _context.CelestialObjects.Update(celestial);
            _context.SaveChanges();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var celestialsToDelete = _context.CelestialObjects.Where(cob => cob.Id == id || cob.OrbitedObjectId == id)
                .ToList();
            if (!celestialsToDelete.Any()) return NotFound();
            _context.CelestialObjects.RemoveRange(celestialsToDelete);
            _context.SaveChanges();
            return NoContent();
        }
    }
}
