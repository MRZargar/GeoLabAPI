using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GeoLabAPI
{
    public interface IStationsSetupRepository
    {
        Task<IEnumerable<StationsSetup>> GetAllAsync();
        Task<StationsSetup> GetByIdAsync(int stationID);
        StationsSetup GetByTableNameAsync(string tableName);
        Task<bool> InsertAsync(StationsSetup station);
        Task<bool> UpdateAsync(StationsSetup station);
        bool Delete(StationsSetup station);
        Task<bool> DeleteAsync(int stationID);
        bool IsExist(StationsSetup station);
        bool IsExist(int stationID);
        bool IsExist(string tableName);
        bool IsExist(decimal x, decimal y, string sensorType);
        Task<bool> saveAsync();
        
        void Dispose();
    }
}