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
    public class StationDataRepository : IStationDataRepository, IDisposable
    {
        private geolabContext db;
        private bool disposed = false;
        const double double_NULL = default(double);
        public StationDataRepository(geolabContext context) => db = context;

        public bool IsExistStation(string tableName)
        {
            return db.Stations
                .Any(m => m.TableName.ToLower() == tableName.ToLower());
        }

        public bool Delete(string tableName, StationData data)
        {
            if (!IsExist(tableName, data))
                throw new NotFoundException();

            db.Datas.Remove(data);
            return true;
        }

        public async Task<bool> DeleteAsync(string tableName, double T)
        {
            db.tableName = tableName.ToLower();

            if (!IsExistStation(tableName))
                throw new NotFoundException();

            var data = await GetByIdAsync(tableName, T);
            return Delete(tableName, data);
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

        public async Task<IEnumerable<StationData>> GetAllAsync(string tableName)
        {
            db.tableName = tableName.ToLower();

            if (!IsExistStation(tableName))
                throw new NotFoundException();

            return await db.Datas.ToListAsync();
        }

        public async Task<StationData> GetByIdAsync(string tableName, double T)
        {
            db.tableName = tableName.ToLower();

            if (!IsExistStation(tableName))
                throw new NotFoundException();         
            
            if (!IsExist(tableName, T))
                throw new NotFoundException();

            return await db.Datas
                .FirstAsync(m => m.T == T);
        }

        public async Task<bool> InsertAsync(string tableName, StationData data)
        {     
            if (data.T == double_NULL)
                throw new NotFoundException();

            if (data.AX == double_NULL)
                throw new NotFoundException();
            
            if (data.AY == double_NULL)
                throw new NotFoundException();

            if (data.AZ == double_NULL)
                throw new NotFoundException();

            if (IsExist(tableName, data))
                throw new DuplicateException();

            await db.Datas.AddAsync(data);
            return true;
        }

        public bool IsExist(string tableName, StationData data)
        {
            return IsExist(tableName, data.T);
        }

        public bool IsExist(string tableName, double T)
        {
            db.tableName = tableName.ToLower();

            if (!IsExistStation(tableName))
                throw new NotFoundException();
            
            return db.Datas.Any(x => x.T == T);
        }

        public async Task<bool> saveAsync()
        {
            await db.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdateAsync(string tableName, StationData data)
        {
            StationData d = await GetByIdAsync(tableName, data.T);

            d.AX = data.AX == double_NULL ? d.AX : data.AX;
            d.AY = data.AY == double_NULL ? d.AY : data.AY;
            d.AZ = data.AZ == double_NULL ? d.AZ : data.AZ;
            d.Temp = data.Temp == double_NULL ? d.Temp : data.Temp;
            
            return true;
        }

        public bool IsExistRaspberry(int RaspberryID)
        {
            return db.Raspberries
                .Any(x => x.RaspberryId == RaspberryID);
        }

        public bool IsExistRaspberry(Raspberry raspberry)
        {
            return IsExistRaspberry(raspberry.RaspberryId);
        }

        public async Task<IEnumerable<Raspberry>> GetAllRaspberriesAsync()
        {
            return await db.Raspberries.ToListAsync();
        }

        public async Task<Raspberry> GetByIdRaspberryAsync(int raspberryID)
        {
            if (!IsExistRaspberry(raspberryID))
                throw new NotFoundException();

            return await db.Raspberries
                .FirstAsync(x => x.RaspberryId == raspberryID);
        }

        public async Task<bool> InsertRaspberryAsync(Raspberry raspberry)
        {
            if (IsExistRaspberry(raspberry))
                throw new DuplicateException();

            await db.Raspberries.AddAsync(raspberry);
            return true;
        }

        public bool DeleteRaspberry(Raspberry raspberry)
        {
            if (!IsExistRaspberry(raspberry))
                throw new NotFoundException();

            db.Raspberries.Remove(raspberry);

            return true;
        }

        public async Task<bool> DeleteRaspberryAsync(int raspberryID)
        {
            var ras = await GetByIdRaspberryAsync(raspberryID);
            return DeleteRaspberry(ras);
        }

        private bool IsExistStation(int stationId)
        {
            return db.Stations.Any(x => x.Id == stationId);
        }

        public string GetTableNameByStationId(int id)
        {
            if (!IsExistStation(id))
                throw new NotFoundException();

            var station = db.Stations.First(x => x.Id == id);

            return station.TableName;
        }

        public string GetTableNameByRaspberryId(int id)
        {
            if (!IsExistRaspberry(id))
                throw new NotFoundException();

            var station = db.Stations.First(x => x.RaspberryId == id);

            return station.TableName;
        }
    }
}