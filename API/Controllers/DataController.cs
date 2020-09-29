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
        const int oneHourAsSeconds = 1 * 60 * 60;
        const int dataCountPerHour = oneHourAsSeconds * 100;
        const int oneWeekAsSeconds = 7 * 24 * oneHourAsSeconds;
        private IStationDataRepository datas;
        private IStationsSetupRepository stations;

        public DataController(geolabContext context)
        {
            datas = new StationDataRepository(context);
            stations = new StationsSetupRepository(context);
        }

        private void nextHour(ref int week, ref double t)
        {
            t += oneHourAsSeconds;
            if (t >= oneWeekAsSeconds)
            {
                t -= oneWeekAsSeconds;
                week++;
            }
        }

        [HttpGet("Histogram/{tableName}")]
        public async Task<ActionResult<IEnumerable<double>>> GetCount(string tableName, int? week, double? t)
        {
            if (week == null || t == null)
                return NotFound();

            try
            {
                int currentWeek = week.Value;
                double currentT = t.Value;
                var HistData = new List<double>();
                for (int i = 1; i <= 24; i++)
                {
                    var from = new GPSTime(currentWeek, currentT);
                    nextHour(ref currentWeek, ref currentT);
                    var to = new GPSTime(currentWeek, currentT - 0.001);

                    var count = datas.GetCount(tableName, from, to);
                    double percent = (double)count / (double)dataCountPerHour * 100.0;

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
        public async Task<ActionResult<IEnumerable<IEnumerable<double>>>> GetDatas(string tableName, int? fromWeek, int? toWeek, double? fromT, double? toT)
        {
            try
            {
                GPSTime from = null, to = null;
                if (fromWeek != null || fromT != null)
                    from = new GPSTime(fromWeek.Value, fromT.Value);
                if (toWeek != null || toT != null)
                    to = new GPSTime(toWeek.Value, toT.Value);

                var Datas = (await datas.GetAllAsync(tableName, from, to)).ToList();
                var All = new List<List<double>>();
                foreach (var data in Datas)
                {
                    All.Add(new List<double>() {data.WEEK, data.T, data.AX, data.AY, data.AZ, data.Temp.Value} );
                }
                return All;
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
        public async Task<ActionResult<IEnumerable<double>>> GetStationData(string tableName, int week, double id)
        {
            try
            {
                var data = await datas.GetByIdAsync(tableName, week, id);
                return new List<double>() {data.WEEK, data.T, data.AX, data.AY, data.AZ, data.Temp.Value};
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

            return RedirectToAction("GetStationData", new { tableName = tableName, id = id, week = week });
        }

        // POST: api/Data
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost("{tableName}")]
        public async Task<ActionResult<StationData>> PostStationData(string tableName, double[][] data)
        {
            foreach (var item in data)
            {
                try
                {
                    var newData = new StationData() 
                    {
                        WEEK = (int)item[0],
                        T = item[1],
                        AX = item[2],
                        AY = item[3],
                        AZ = item[4],
                        Temp = item[5]
                    };

                    datas.InsertAsync(tableName, newData);
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
        public async Task<IActionResult> PutStatus(string tableName, RaspberryHealth h)
        {
            try
            {
                var station = stations.GetByTableNameAsync(tableName);
                station.Health = h.healthCode;
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
