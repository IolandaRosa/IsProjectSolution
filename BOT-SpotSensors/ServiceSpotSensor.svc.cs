using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using System.Xml;

namespace BOT_SpotSensors
{
    public class ServiceSpotSensor : IServiceSpotSensor
    {
        string filePath = AppDomain.CurrentDomain.BaseDirectory.ToString() + @"App_Data\XMLPark_B_Spots.xml";

        public List<Spot> GetAllSpotsData()
        {
            List<Spot> spots = new List<Spot>();

            XmlDocument doc = new XmlDocument();
            doc.Load(filePath);

            XmlNodeList xmlSpots = doc.SelectNodes("/parkSpots/parkingSpot");

            foreach (XmlNode item in xmlSpots)
            {
                Spot spot = new Spot();

                spot.Id = item["id"].InnerText;
                spot.Type = item["type"].InnerText;
                spot.Location = item["location"].InnerText;
                spot.Name = item["name"].InnerText;
                spot.BatteryStatus = int.Parse(item["batteryStatus"].InnerText);

                XmlNode statusNode = item.SelectSingleNode("status");

                if (statusNode != null)
                {
                    spot.Status = new Status
                    {
                        Value= (ValueType)Enum.Parse(typeof(ValueType), statusNode["value"].InnerText.ToUpper()),
                        Timestamp = DateTime.ParseExact(statusNode["timestamp"].InnerText, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture)
                    };
                }

                spots.Add(spot);
            }

            return spots;
        }

        public XmlElement GetAllSpotsIDataXml()
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(filePath);

            return doc.DocumentElement;
        }
    }
}
