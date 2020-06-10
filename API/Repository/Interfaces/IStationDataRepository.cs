using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GeoLabAPI
{
    public interface IStationDataRepository
    {
        Task<IEnumerable<StationData>> GetAllAsync(string tableName);
        Task<StationData> GetByIdAsync(string tableName, double T);
        Task<bool> InsertAsync(string tableName, StationData data);
        Task<bool> UpdateAsync(string tableName, StationData data);
        bool Delete(string tableName, StationData data);
        Task<bool> DeleteAsync(string tableName, double T);
        bool IsExist(string tableName, StationData data);
        bool IsExist(string tableName, double T);
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