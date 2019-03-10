using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParkSS
{
    class Park
    {
        public String Id { get; set; }
        public String Description { get; set; }
        public int NumberOfSpots { get; set; }
        public String OperatingHours { get; set; }
        public int NumberOfSpecialSpots { get; set; }
        public DateTime UpdateGeolocationData { get; set; }

        public Park(string id, string description, int numberOfSpots, string operatingHours, int numberOfSpecialSpots, DateTime updateGeolocationData)
        {
            Id = id;
            Description = description;
            NumberOfSpots = numberOfSpots;
            OperatingHours = operatingHours;
            NumberOfSpecialSpots = numberOfSpecialSpots;
            UpdateGeolocationData = updateGeolocationData;
        }
    }
}
