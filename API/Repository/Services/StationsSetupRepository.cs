using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GeoLabAPI.Exceptions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace GeoLabAPI
{
    public class GPSTime
    {
        public int week { get; }
        public double t { get; }

        public GPSTime(int week, double t)
        {
            this.week = week;
            this.t = t;
        }
        
        public GPSTime()
        {
        }
    }

    public class RaspberryHealth
    {
        public int healthCode { get; set; }
    }

    public class StationsSetupRepository : IStationsSetupRepository
    {
        private geolabContext db;
        private bool disposed = false;
        const decimal NULL_decimal = default(decimal);
        const int NULL_int = default(int);
        DateTime NULL_dateTime = default(DateTime);

        public StationsSetupRepository(geolabContext context) => db = context;

        public bool Delete(StationsSetup station)
        {
            if (!IsExist(station))
                throw new NotFoundException();

            db.Stations.Remove(station);
            DeleteDataTableAsync(station.TableName);

            return true;
        }

        private async Task DeleteDataTableAsync(string tableName)
        {
            var datas = new StationDataRepository(db);
            var table = await datas.GetAllAsync(tableName.ToLower());
            foreach (var data in table)
            {
                datas.DeleteAsync(tableName, data.WEEK, data.T);
            }
        }

        public async Task<bool> DeleteAsync(int stationID)
        {
            var station = await GetByIdAsync(stationID);
            return Delete(station);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposed)
                return; 

            if (disposing) 
                db.Dispose();

            disposed = true;
        }    

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public async Task<IEnumerable<StationsSetup>> GetAllAsync()
        {
            return await db.Stations.ToListAsync();
        }

        public async Task<StationsSetup> GetByIdAsync(int stationID)
        {
            if (!IsExist(stationID))
                throw new NotFoundException();

            return await db.Stations
                .FirstAsync(m => m.Id == stationID);
        }

        public StationsSetup GetByTableNameAsync(string tableName)
        {
            if (!IsExist(tableName))
                throw new NotFoundException();

            return db.Stations.First(m => m.TableName == tableName.ToUpper());
        }

        public async Task<bool> InsertAsync(StationsSetup station)
        {
            if (IsExist(station))
                throw new DuplicateException();

            if (station.OperatorId == NULL_int)
                throw new NotFoundException();

            if (station.SensorType == null)
                throw new NotFoundException();

            if (station.Latitude == NULL_decimal)
                throw new NotFoundException();

            if (station.Longitude == NULL_decimal)
                throw new NotFoundException();

            if (station.Address == null)
                throw new NotFoundException();        

            if (station.Date == NULL_dateTime)
                throw new NotFoundException();

            if (station.StationId == null)
                throw new NotFoundException();

            if (station.Owner == null)
                throw new NotFoundException();

            station.Id = 0;
            station.Health = 0;
            station.Status = true;
            station.TableName = null;

            await db.Stations.AddAsync(station);
            return true;
        }

        public bool IsExist(StationsSetup station)
        {
            return IsExist(station.Id) 
                || IsExist(station.Latitude, station.Latitude, station.SensorType);
        }
        
        public bool IsExist(int stationID)
        {
            return db.Stations.Any(x => x.Id == stationID);
        }

        public bool IsExist(decimal lat, decimal lon, string sensorType)
        {
            return db.Stations.Any(s => 
                s.SensorType == sensorType 
                && s.Latitude.CompareTo(lat) == 0
                && s.Longitude.CompareTo(lon) == 0);
        }

        public bool IsExist(string tableName)
        {
            return db.Stations.Any(x => 
                x.TableName.ToLower() == tableName.ToLower());
        }

        public async Task<bool> saveAsync()
        {
            await db.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdateAsync(StationsSetup station)
        {
            StationsSetup s = await GetByIdAsync(station.Id);

            s.OperatorId = station.OperatorId == NULL_int ? s.OperatorId : station.OperatorId;
            s.SensorType = station.SensorType ?? s.SensorType;
            s.Status = station.Status ?? s.Status;
            s.Health = station.Health ?? s.Health;
            s.Latitude = station.Latitude == NULL_decimal ? s.Latitude : station.Latitude;
            s.Longitude = station.Longitude == NULL_decimal ? s.Longitude : station.Longitude;
            s.Date = station.Date == NULL_dateTime ? s.Date : station.Date;
            s.HealthTime = station.HealthTime ?? s.HealthTime;
            s.Address = station.Address ?? s.Address;
            s.City = station.City ?? s.City;
            s.Owner = station.Owner ?? s.Owner;
            s.StationId = station.StationId ?? s.StationId;
            
            return true;
        }
    }
}