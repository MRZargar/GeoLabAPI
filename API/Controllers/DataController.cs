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

        [HttpGet("Histogram/{tableName}")]
        public async Task<ActionResult<IEnumerable<double>>> GetDatas(string tableName, int? week, double? t)
        {
            if (week == null || t == null)
                return NotFound();
            
            int oneHourAsSeconds = 1 * 60 * 60;
            int dataCountPerHour = oneHourAsSeconds * 100;

            try
            {
                var HistData = new List<double>();
                for (int i = 0; i < 24; i++)
                {
                    var from = new GPSTime(i * week.Value, t.Value + (i - 1) * oneHourAsSeconds);
                    var to = new GPSTime((i + 1) * week.Value, t.Value + i * oneHourAsSeconds);

                    int count = datas.GetCount(tableName, from, to);
                    double percent = count / dataCountPerHour;

                    HistData.Add(percent);
                }

                return HistData;
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


        // GET: api/Data
        [HttpGet("{tableName}")]
        public async Task<ActionResult<IEnumerable<StationData>>> GetDatas(string tableName, int? fromWeek, int? toWeek, double? fromT, double? toT)
        {
            try
            {
                GPSTime from = null, to = null;
                if (fromWeek != null || fromT != null)
                    from = new GPSTime(fromWeek.Value, fromT.Value);
                if (toWeek != null || toT != null)
                    to = new GPSTime(toWeek.Value, toT.Value);

                return (await datas.GetAllAsync(tableName, from, to)).ToList();
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
        [HttpGet("{tableName}/{week}/{id}")]
        public async Task<ActionResult<StationData>> GetStationData(string tableName, int week, double id)
        {
            try
            {
                return await datas.GetByIdAsync(tableName, week, id);
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
        [HttpPut("{tableName}/{week}/{id}")]
        public async Task<IActionResult> PutStationData(string tableName, int week, double id, StationData data)
        {
            if (id != data.T && week != data.WEEK)
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
        [HttpDelete("{tableName}/{week}/{id}")]
        public async Task<ActionResult<StationData>> DeleteStationData(string tableName, int week, double id)
        {
            try
            {
                await datas.DeleteAsync(tableName, week, id);
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
                station.HealthTime = DateTime.Now;

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
