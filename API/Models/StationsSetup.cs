using System;
using System.Collections.Generic;

namespace GeoLabAPI
{
    public partial class StationsSetup
    {
        public int Id { get; set; }
        public string TableName { get; set; }
        public string City { get; set; }
        public string StationId { get; set; }
        public string Address { get; set; }
        public string Owner { get; set; }
        public DateTime Date { get; set; }
        public bool? Status { get; set; }
        public string SensorType { get; set; }
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }
        public int? Health { get; set; }
        public int RaspberryId { get; set; }
        public DateTime? HealthTime { get; set; }
        public int OperatorId { get; set; }
    }
}
