using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GeoLabAPI.Exceptions;

namespace GeoLabAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StationsController : ControllerBase
    {
        private IStationsSetupRepository stations;

        public StationsController(geolabContext context)
        {
            stations = new StationsSetupRepository(context);
        }

        // GET: api/Stations
        [HttpGet]
        public async Task<ActionResult<IEnumerable<StationsSetup>>> GetStationsSetup()
        {
            return (await stations.GetAllAsync()).ToList();
        }

        // GET: api/Stations/5
        [HttpGet("{id}")]
        public async Task<ActionResult<StationsSetup>> GetStationsSetup(int id)
        {
            try
            {
                return await stations.GetByIdAsync(id);
            }
            catch (NotFoundException)
            {
                return NotFound();
            }
            catch(Exception)
            {
                return BadRequest();
            }
        }

        // PUT: api/Stations/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutStationsSetup(int id, StationsSetup station)
        {
            if (id != station.Id)
            {
                return BadRequest();
            }

            try
            {
                await stations.UpdateAsync(station);
                await stations.saveAsync();
            }
            catch(NotFoundException)
            {
                return NotFound();
            }
            catch (DbUpdateConcurrencyException)
            {
                return BadRequest();
            }
            catch(Exception)
            {
                return BadRequest();
            }

            return RedirectToAction("GetStationsSetup", new { id = station.Id });
        }

        // POST: api/Stations
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        public async Task<ActionResult<StationsSetup>> PostStationsSetup(StationsSetup stationsSetup)
        {
            try
            {
                await stations.InsertAsync(stationsSetup);
                await stations.saveAsync();    
            }
            catch(DuplicateException)
            {
                return Conflict();
            }
            catch (DbUpdateConcurrencyException)
            {
                return BadRequest();
            }
            catch (NotFoundException)
            {
                return NotFound();
            }
            catch(Exception)
            {
                return BadRequest();
            }
            
            return CreatedAtAction("GetStationsSetup", new { id = stationsSetup.Id }, stationsSetup);
        }

        // DELETE: api/Stations/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<StationsSetup>> DeleteStationsSetup(int id)
        {            
            try
            {
                await stations.DeleteAsync(id);
                await stations.saveAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                return BadRequest();
            }
            catch (NotFoundException)
            {
                return NotFound();
            }
            catch(Exception)
            {
                return BadRequest();
            }

            return NoContent();
        }
    }
}
