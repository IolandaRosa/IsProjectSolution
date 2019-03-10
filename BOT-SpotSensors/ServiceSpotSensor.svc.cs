using System;
using System.Collections.Generic;
using System.Globalization;
using System.Xml;

namespace BOT_SpotSensors
{
    public class ServiceSpotSensor : IServiceSpotSensor
    {
        string filePath = AppDomain.CurrentDomain.BaseDirectory.ToString() + @"App_Data\XMLPark_B_Spots.xml";
        Random random = new Random();

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
                //spot.BatteryStatus = int.Parse(item["batteryStatus"].InnerText);
                double value = random.NextDouble();

                XmlNode statusNode = item.SelectSingleNode("status");

                if (statusNode != null)
                {
                    ValueType status;

                    if (value < 0.8)
                    {
                        spot.BatteryStatus = 0;
                        status = ValueType.OCCUPIED;
                    }
                    else
                    {
                        spot.BatteryStatus = 1;
                        status = ValueType.FREE;
                    }

                    String timeStamp = GetTimestamp(DateTime.Now);

                    spot.Status = new Status
                    {
                        //Value= (ValueType)Enum.Parse(typeof(ValueType), statusNode["value"].InnerText.ToUpper()),
                        Value = status,
                        Timestamp = DateTime.ParseExact(timeStamp, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture)
                        //Timestamp = DateTime.ParseExact(statusNode["timestamp"].InnerText, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture)
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

            //Vai buscar os nos de parking spots para colocar os novos valores nos campos devidos
            XmlNodeList xmlSpots = doc.SelectNodes("/parkSpots/parkingSpot");

            foreach (XmlNode item in xmlSpots)
            {
                item["batteryStatus"].InnerText = random.Next(0, 2).ToString();

                string status;
                if (random.Next(0, 2) == 0)
                {
                    status = "free";
                }
                else
                {
                    status = "occupied";
                }

                XmlNode value = item.SelectSingleNode("status/value");
                value.InnerText = status;

                string timeStamp = GetTimestamp(DateTime.Now);
                XmlNode time = item.SelectSingleNode("status/timestamp");
                time.InnerText = timeStamp;
            }

            doc.Save(filePath);

            return doc.DocumentElement;
        }

        private String GetTimestamp(DateTime value)
        {
            return value.ToString("yyyy-MM-dd HH:mm:ss");
        }
    }
}
