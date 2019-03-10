using ParkDACE.ServiceSpotSensor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace ParkDACE
{
    class Program
    {
        static void Main(string[] args)
        {
            ServiceSpotSensorClient client = new ServiceSpotSensorClient();

            List<Spot> spots=client.GetAllSpotsData().ToList();

            Console.WriteLine(spots[1].Type);
            Console.ReadKey();
        }
    }
}
