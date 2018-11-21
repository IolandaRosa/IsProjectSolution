using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using System.Xml;

namespace BOT_SpotSensors
{
    [ServiceContract]
    public interface IServiceSpotSensor
    {

        [OperationContract]
        List<Spot> GetAllSpotsData();

        [OperationContract]
        XmlElement GetAllSpotsIDataXml();

    }

    [DataContract]
    public class Spot
    {
        [DataMember]
        public String Id { get; set; }

        [DataMember]
        public String Type { get; set; }

        [DataMember]
        public String Name { get; set; }

        [DataMember]
        public String Location { get; set; }

        [DataMember]
        public Status Status { get; set; }

        [DataMember]
        public int BatteryStatus { get; set; }
    }

    [DataContract]
    public class Status
    {
        [DataMember]
        public ValueType Value { get; set; }

        [DataMember]
        public DateTime Timestamp { get; set; }
    }

    public enum ValueType
    {
        FREE, OCCUPIED
    }
}
