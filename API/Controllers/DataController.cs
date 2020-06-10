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
    public class DataController : ControllerBase
    {
        private IStationDataRepository datas;
        private IStationsSetupRepository stations;

        public DataController(geolabContext context)
        {
            datas = new StationDataRepository(context);
            stations = new StationsSetupRepository(context);
        }

        // GET: api/Data
        [HttpGet("{tableName}")]
        public async Task<ActionResult<IEnumerable<StationData>>> GetDatas(string tableName)
        {
            try
            {
                return (await datas.GetAllAsync(tableName)).ToList();
            }
            catch (NotFoundException)
            {
                return NotFound();
            }
            catch(Exception ex)
            {
                return StatusCode(501);
            }
        }

        // GET: api/Data/5
        [HttpGet("{tableName}/{id}")]
        public async Task<ActionResult<StationData>> GetStationData(string tableName, double id)
        {
            try
            {
                return await datas.GetByIdAsync(tableName, id);
            }
            catch (NotFoundException)
            {
                return NotFound();
            }
            catch(Exception)
            {
                return StatusCode(501);
            }
        }

        // PUT: api/Data/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("{tableName}/{id}")]
        public async Task<IActionResult> PutStationData(string tableName, double id, StationData data)
        {
            if (id != data.T)
            {
                return BadRequest();
            }

            try
            {
                await datas.UpdateAsync(tableName, data);
                await datas.saveAsync();
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
                return StatusCode(501);
            }

            return RedirectToAction("GetStationData", new { tableName = tableName, id = id });
        }

        // POST: api/Data
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost("{tableName}")]
        public async Task<ActionResult<StationData>> PostStationData(string tableName, StationData[] data)
        {
            foreach (var item in data)
            {
                try
                {
                    datas.InsertAsync(tableName, item);
                }
                catch(DuplicateException)
                {
                    continue;
                }
                catch (NotFoundException)
                {
                    return NotFound();
                }
                catch(Exception)
                {
                    return StatusCode(501);
                }
            }
    
            try
            {
                await datas.saveAsync();  
            }
            catch (DbUpdateConcurrencyException)
            {
                return BadRequest();
            }
            
            return NoContent();
        }

        // DELETE: api/Data/5
        [HttpDelete("{tableName}/{id}")]
        public async Task<ActionResult<StationData>> DeleteStationData(string tableName, double id)
        {
            try
            {
                await datas.DeleteAsync(tableName, id);
                await datas.saveAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                return BadRequest();
            }
            catch(NotFoundException)
            {
                return NotFound();
            }
            catch (Exception)
            {
                return StatusCode(501);
            }

            return NoContent();
        }

        // POST: api/Data/{RaspberryID}/313
        [HttpPost("{RaspberryID}/313")]
        public async Task<ActionResult<StationData>> PostRaspberry(int RaspberryID)
        {
            try
            {
                await datas.InsertRaspberryAsync(new Raspberry(RaspberryID));
                await datas.saveAsync();
            }
            catch(DuplicateException)
            {
                return NoContent();
            }
            catch (DbUpdateConcurrencyException)
            {
                return BadRequest();
            }
            catch(Exception)
            {
                return StatusCode(501);
            }
            
            return NoContent();
        }

        // POST: api/Data/{RaspberryID}/313
        [HttpGet("{RaspberryID}/313")]
        public ActionResult<string> GetRaspberry(int RaspberryID)
        {
            try
            {
                return datas.GetTableNameByRaspberryId(RaspberryID);
            }
            catch(NotFoundException)
            {
                return NotFound();
            }
            catch(Exception)
            {
                return StatusCode(501);
            }
        }

        [HttpPut("{tableName}")]
        public async Task<IActionResult> PutStatus(string tableName, int healthCode)
        {
            try
            {
                var station = stations.GetByTableNameAsync(tableName);
                station.Health = healthCode;

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
            catch(Exception ex)
            {
                return StatusCode(501);
            }

            return NoContent();
        }
    }
}
