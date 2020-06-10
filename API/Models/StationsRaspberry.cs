using System;
using System.Collections.Generic;

namespace GeoLabAPI
{
    public partial class Raspberry
    {
        public int Id { get; set; }
        public int RaspberryId { get; set; }

        public Raspberry()
        {
            
        }

        public Raspberry(int raspberryID)
        {
            RaspberryId = raspberryID;
        }
    }
}
