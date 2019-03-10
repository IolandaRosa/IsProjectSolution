using ExcelLib;
using Park_DACE.ServiceSpotSensor;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Timers;
using System.Windows.Forms;
using System.Xml;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;

namespace Park_DACE
{
    public partial class ParkDACE : Form
    {
        private ParkingSensorNodeDll.ParkingSensorNodeDll dll;

        //tudo o que está static é por causa do set timer
        private static ServiceSpotSensorClient client = null;
        private static XmlNode parkInfo = null;

        private int refreshRate = 0;
        private string temporalUnit = null;

        private static char[] charSeparators = new char[] { ';' }; //para o split
        private String[] values = null;

        private static XmlDocument outputDocument = null;
        private Dictionary<string, string> coordenatesDll = null;
        private static System.Timers.Timer aTimer;

        private string endPoint = "127.0.0.1";
        private static MqttClient mClient = null;
        string[] topics = { "parks", "spots" };

        //private BackgroundWorker bw = new BackgroundWorker();
        public ParkDACE()
        {
            InitializeComponent();
            //bw.DoWork += new DoWorkEventHandler(DoWork);

        }
        public void DoWork(object sender, DoWorkEventArgs e)
        {
            dll.Initialize(NewSensorValueFunction, 1000);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            mClient = new MqttClient(endPoint);
            mClient.Connect(Guid.NewGuid().ToString());

            if (!mClient.IsConnected)
            {
                MessageBox.Show("Error conecting to message broker");
            }

            mClient.MqttMsgPublishReceived += Client_MqttMsgPublishReceived;

            //para receber feedBack do parkSS caso seja pertinente
            Byte[] qosLevels = { MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE, MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE };
            mClient.Subscribe(topics, qosLevels);
        }

        private void Client_MqttMsgPublishReceived(object sender, MqttMsgPublishEventArgs e)
        {
            this.BeginInvoke((MethodInvoker)delegate
            {
                // listBoxSpots.Items.Add($"Received on  topic: {e.Topic} the message {Encoding.UTF8.GetString(e.Message)} \n");
            });

        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            if (dll == null)
            {
                MessageBox.Show("The service was not initiated");
                return;
            }

            dll.Stop();
            this.BeginInvoke((MethodInvoker)delegate
            {
                dll.Stop();
            });
        }
        private void btnLaunch_Click(object sender, EventArgs e)
        {
            //---------------LER O CONFIG FILE E EXTRAIR OS DADOS---------------
            listBoxSpots.Items.Add("Reading config file...");
            listBoxSpots.Items.Add("");

            //readConfigFile
            XmlDocument doc = new XmlDocument();
            doc.Load("configurationFile.xml");

            //tempo de reload
            listBoxSpots.Items.Add("Geting reload time..");
            XmlNode filtro = doc.SelectSingleNode("//@refreshRate");
            refreshRate = int.Parse(filtro.InnerText);
            listBoxSpots.Items.Add("Reload time: " + refreshRate);

            filtro = doc.SelectSingleNode("//@units");
            temporalUnit = filtro.InnerText;
            listBoxSpots.Items.Add("Temporal unit: " + temporalUnit);

            XmlNodeList providers = doc.SelectNodes("//provider");

            XmlElement park = null;
            DateTime updatedGeolocationData = new DateTime();

            outputDocument = new XmlDocument();
            XmlDeclaration dec = outputDocument.CreateXmlDeclaration("1.0", null, null);
            outputDocument.AppendChild(dec);

            foreach (XmlNode provider in providers)
            {
                parkInfo = provider["parkInfo"];
                if (provider["connectionType"].InnerText == "DLL")
                {
                    coordenatesDll = ReadCoordenatesFromExcel(parkInfo["geoLocationFile"].InnerText, int.Parse(parkInfo["numberOfSpots"].InnerText));
                    //sendParkAInfo();

                    updatedGeolocationData = ExcelHandler.ReadUpdatedGeolocationData(parkInfo["geoLocationFile"].InnerText);
                    park = createPark(outputDocument, parkInfo["id"].InnerText, parkInfo["description"].InnerText,
                        parkInfo["numberOfSpots"].InnerText, parkInfo["operatingHours"].InnerText, parkInfo["numberOfSpecialSpots"].InnerText, updatedGeolocationData.ToString());


                    outputDocument.AppendChild(park);
                    //----------ENVIAR O PARK NO PUB/SUB----------
                    listBoxSpots.Items.Add("Park do A a enviar: " + outputDocument.InnerXml);
                    sendInfo("parks", outputDocument.InnerXml);
                    outputDocument.RemoveChild(park);


                    dll = new ParkingSensorNodeDll.ParkingSensorNodeDll();

                    dll.Initialize(NewSensorValueFunction, 3000);
                    //É o initialize debaixo que depois deve ser usado mas para agora ir desenvolvendo usa-se a de cima 
                    //basicamnete ao dividir o refhresh rate pelo numero de spots faz com que sempre que volta ao primeiro spot coincida com o refresh rate
                    //mesmo que haja um que "avarie" e nao seja enviado não há problema os outros sao executados
                    //dll.Initialize(NewSensorValueFunction, (int)(GetCorrectTime(refreshRate, temporalUnit)/ int.Parse(parkInfo["numberOfSpots"].InnerText)));


                    //só para ver na listbox as coordenadas lidas do excel
                    foreach (KeyValuePair<string, string> item in coordenatesDll)
                    {
                        listBoxSpots.Items.Add(item);
                    }

                }
                else if (provider["connectionType"].InnerText == "SOAP")
                {
                    client = new ServiceSpotSensorClient();
                    //sendParkBInfo();
                    client = new ServiceSpotSensorClient();
                    // adicionar o endpoit do soap dinamicamente(ainda falta fazer)
                    // ServiceHost host = new ServiceHost(typeof(CalculatorService), new Uri(baseAddress));


                    //criar o park
                    updatedGeolocationData = ExcelHandler.ReadUpdatedGeolocationData(parkInfo["geoLocationFile"].InnerText);
                    park = createPark(outputDocument, parkInfo["id"].InnerText, parkInfo["description"].InnerText,
                        parkInfo["numberOfSpots"].InnerText, parkInfo["operatingHours"].InnerText, parkInfo["numberOfSpecialSpots"].InnerText, updatedGeolocationData.ToString());


                    outputDocument.AppendChild(park);
                    //----------ENVIAR O PARK NO PUB/SUB----------
                    listBoxSpots.Items.Add("Park do B a enviar: " + outputDocument.InnerXml);

                    sendInfo("parks", outputDocument.InnerXml);
                    outputDocument.RemoveChild(park);
                    //SetTimer(GetCorrectTime(refreshRate, temporalUnit)); //é para usar desta maneira mas para development usar o debaixo
                    SetTimer(5000);
                }
                else
                {
                    listBoxSpots.Items.Add("Connection type not recognized");
                }
            }
        }

        private static void sendInfo(string topic, string content)
        {
            if (mClient.IsConnected)
            {
                byte[] msg = Encoding.UTF8.GetBytes(content);
                mClient.Publish(topic, msg);
            }
        }
        //a partir dos valores do ficheiro de configuração devolve o tempo na unicade correta usada pelo timer
        private int GetCorrectTime(int refreshRate, string temporalUnit)
        {
            switch (temporalUnit)
            {
                case "minutes": return refreshRate * 60 * 1000;
                case "seconds": return refreshRate * 1000;
                default: return refreshRate;
            }
        }

        private static void SetTimer(int time)
        {
            aTimer = new System.Timers.Timer(time);
            aTimer.Elapsed += OnTimedEvent;
            aTimer.AutoReset = true;
            aTimer.Enabled = true;
        }
        private static void OnTimedEvent(Object source, ElapsedEventArgs e)
        {
            XmlElement spotElement = null;
            Dictionary<string, string> coordenatesSoap = ReadCoordenatesFromExcel(parkInfo["geoLocationFile"].InnerText, int.Parse(parkInfo["numberOfSpots"].InnerText));
            List<Spot> spotsSoap = client.GetAllSpotsData().ToList();

            foreach (Spot spot in spotsSoap)
            {

                foreach (KeyValuePair<string, string> item in coordenatesSoap)
                {
                    if (item.Key == spot.Name)
                    {
                        spotElement = createSpot(outputDocument, spot.Name, spot.Status.Timestamp.ToString(), spot.Status.Value.ToString(), spot.BatteryStatus.ToString(), item.Value, spot.Type, spot.Id);
                        outputDocument.AppendChild(spotElement);

                        //----------ENVIAR O SPOT NO PUB/SUB----------
                        //Nao consigo escrever na lsitbox com este método por isso escrevo na consola(mas tambem so serve para ver se a estrutura está bem)
                        Console.WriteLine("Spot do B a enviar: " + outputDocument.InnerXml);
                        sendInfo("spots", outputDocument.InnerXml);
                        outputDocument.RemoveChild(spotElement);

                    }
                }
            }
        }

        public void NewSensorValueFunction(string str)
        {
            XmlElement spot = null;
            try
            {
                this.BeginInvoke((MethodInvoker)delegate
                {
                    values = str.Split(charSeparators);
                    string coordenate = getCoordenate(coordenatesDll, values[1]);
                    String parkingStatus = values[3] == "1" ? "FREE" : "OCCUPIED";
                    spot = createSpot(outputDocument, values[1], values[2], parkingStatus, values[4], coordenate, "ParkingSpot", values[0]);
                    outputDocument.AppendChild(spot);

                    //----------ENVIAR O SPOT NO PUB/SUB----------
                    listBoxSpots.Items.Add("Spot do A a enviar: " + outputDocument.InnerXml);
                    sendInfo("spots", outputDocument.InnerXml);
                    outputDocument.RemoveChild(spot);
                });
            }
            catch (Exception)
            {
                throw;
            }
        }

        private string getCoordenate(Dictionary<string, string> coordenates, string spotId)
        {
            string coordenate = null;
            foreach (KeyValuePair<string, string> item in coordenates)
            {
                if (item.Key == spotId)
                {
                    coordenate = item.Value;
                    return coordenate;
                }
            }

            return coordenate;
        }

        public static XmlElement createSpot(XmlDocument doc, string spotName, string timestamp, string parkingStatus,
            string batteryStatus, string location, string parkingType, string parkingId)
        {
            XmlElement spot = doc.CreateElement("parkingSpot");

            XmlElement id = doc.CreateElement("id");
            id.InnerText = parkingId;

            XmlElement type = doc.CreateElement("type");
            type.InnerText = parkingType;

            XmlElement name = doc.CreateElement("name");
            name.InnerText = spotName;

            XmlElement locationSpot = doc.CreateElement("location");
            locationSpot.InnerText = location;

            XmlElement status = doc.CreateElement("status");
            //value e timestamp sao filhos do value 
            XmlElement spotParkingStatus = doc.CreateElement("value");
            spotParkingStatus.InnerText = parkingStatus;

            XmlElement spotTimestamp = doc.CreateElement("timestamp");
            spotTimestamp.InnerText = timestamp;

            status.AppendChild(spotParkingStatus);
            status.AppendChild(spotTimestamp);

            XmlElement spotBatteryStatus = doc.CreateElement("batteryStatus");
            spotBatteryStatus.InnerText = batteryStatus;

            spot.AppendChild(id);
            spot.AppendChild(type);
            spot.AppendChild(name);
            spot.AppendChild(locationSpot);
            spot.AppendChild(status);
            spot.AppendChild(spotBatteryStatus);

            return spot;
        }


        public XmlElement createPark(XmlDocument doc, string id, string description, string numberOfSpots, string operatingHours, string numberOfSpecialSpots, string updateGeolocationDate)
        {
            XmlElement park = doc.CreateElement("park");

            XmlElement parkId = doc.CreateElement("id");
            parkId.InnerText = id;

            XmlElement parkDescription = doc.CreateElement("description");
            parkDescription.InnerText = description;

            XmlElement parkNumberOfSpots = doc.CreateElement("numberOfSpots");
            parkNumberOfSpots.InnerText = numberOfSpots;

            XmlElement parkOperatingHours = doc.CreateElement("operatingHours");
            parkOperatingHours.InnerText = operatingHours;

            XmlElement parkNumberOfSpecialSpots = doc.CreateElement("numberOfSpecialSpots");
            parkNumberOfSpecialSpots.InnerText = numberOfSpecialSpots;

            XmlElement updatedGeolocationDatePark = doc.CreateElement("updatedGeolocationData");
            updatedGeolocationDatePark.InnerText = updateGeolocationDate;

            park.AppendChild(parkId);
            park.AppendChild(parkDescription);
            park.AppendChild(parkNumberOfSpots);
            park.AppendChild(parkOperatingHours);
            park.AppendChild(parkNumberOfSpecialSpots);
            park.AppendChild(updatedGeolocationDatePark);
            return park;
        }


        private static Dictionary<string, string> ReadCoordenatesFromExcel(string filename, int numSpots)
        {
            //começa no A6 ate A6+numspots-1
            Dictionary<string, string> coordenates = new Dictionary<string, string>();
            int lastpos = 5 + numSpots;

            string firstRangeId = "A6";
            string lastRangeId = "A" + lastpos;

            string firstRangeCoordenates = "B6";
            string lastoordenates = "B" + lastpos;

            var ids = ExcelHandler.ReadNxMCellsFromFirstWorksheet(filename, firstRangeId, lastRangeId);
            var coordenatesExcel = ExcelHandler.ReadNxMCellsFromFirstWorksheet(filename, firstRangeCoordenates, lastoordenates);

            var idsSplit = ids.Split(charSeparators);
            var coordenatesSplit = coordenatesExcel.Split(charSeparators);
            for (int i = 0; i < idsSplit.Length - 1; i++)
            {
                coordenates.Add(idsSplit[i], coordenatesSplit[i]);
            }

            return coordenates;
        }

        private void btnSendParkInfoManual_Click(object sender, EventArgs e)
        {
            btnLaunch.PerformClick();
            listBoxSpots.Items.Add("Sending Park info manually...");
        }
    }
}
