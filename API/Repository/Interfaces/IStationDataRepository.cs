using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GeoLabAPI
{
    public interface IStationDataRepository
    {
        int GetCount(string tableName, GPSTime from, GPSTime to);
        Task<IEnumerable<StationData>> GetAllAsync(string tableName, GPSTime from, GPSTime to);
        Task<StationData> GetByIdAsync(string tableName, int week, double T);
        Task<bool> InsertAsync(string tableName, StationData data);
        Task<bool> UpdateAsync(string tableName, StationData data);
        bool Delete(string tableName, StationData data);
        Task<bool> DeleteAsync(string tableName, int week, double T);
        bool IsExist(string tableName, StationData data);
        bool IsExist(string tableName, int week, double T);
        string GetTableNameByStationId(int id);
        string GetTableNameByRaspberryId(int id);
        

        Task<IEnumerable<Raspberry>> GetAllRaspberriesAsync();
        Task<Raspberry> GetByIdRaspberryAsync(int raspberryID);
        Task<bool> InsertRaspberryAsync(Raspberry raspberry);
        bool DeleteRaspberry(Raspberry raspberry);
        Task<bool> DeleteRaspberryAsync(int raspberryID);
        bool IsExistRaspberry(int RaspberryID);
        bool IsExistRaspberry(Raspberry raspberry);


        Task<bool> saveAsync();
        void Dispose();
    }
}