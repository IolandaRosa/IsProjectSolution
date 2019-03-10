using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParkSS
{
    class Spot
    {
        public String Id { get; set; }
        public String Type { get; set; }
        public String Name { get; set; }
        public String Location { get; set; }
        public String StatusValue { get; set; }
        public DateTime StatusTimestamp { get; set; }
        public int BatteryLevel { get; set; }

        public Spot(string id, string type, string name, string location, string statusValue, DateTime statusTimestamp, int batteryLevel)
        {
            Id = id;
            Type = type;
            Name = name;
            Location = location;
            StatusValue = statusValue;
            StatusTimestamp = statusTimestamp;
            BatteryLevel = batteryLevel;
        }
    }
}
