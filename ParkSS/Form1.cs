using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;

namespace ParkSS
{
    public partial class Form1 : Form
    {
        MqttClient client = null;
        SqlConnection connection = null;

        //TO change topics and endPoint
        string[] topics = { "parks", "spots" };
        string endPoint = "127.0.0.1";

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            client = new MqttClient(endPoint);
        }

        private void btnLaunchParkSS_Click(object sender, EventArgs e)
        {
            //Ligar ao serviço
            client.Connect(Guid.NewGuid().ToString());

            if (!client.IsConnected)
            {
                this.BeginInvoke((MethodInvoker)delegate
                {
                    MessageBox.Show("Unnable to connect with broker");
                    return;
                });
            }

            //Ficar à escuta
            client.MqttMsgPublishReceived += Client_MqttMsgPublishReceived;

            //Colocar os tópicos / canais onde cliente fica à escuta
            byte[] qosLevels = { MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE, MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE };

            client.Subscribe(topics, qosLevels);

            //Deve-se ter algo caso este cliente deseje dessubscrever o serviço???
        }

        //Quando recebe a mensagem
        private void Client_MqttMsgPublishReceived(object sender, MqttMsgPublishEventArgs e)
        {
            this.BeginInvoke((MethodInvoker)delegate
            {
                //Criar a conexão com a BD
                SqlConnection connection = new SqlConnection(Properties.Settings.Default.ConnectionStr);

                try
                {
                    //Abrir a conexão com a BD
                    connection.Open();

                    if (e.Topic == "parks")
                    { 
                        Park park = GetDataFromPark(e);

                        Int32 res = RegisterExistOnTable(e, "PARK", connection, park.Id);

                        if (res == 0)
                        {
                            InsertParkDataIntoTable(park, connection); 
                        }
                        else
                        {
                            UpdateParkDataIntoTable(park, connection);
                        }
                    }
                    else if (e.Topic == "spots")
                    {
                        Spot spot=GetDataFromSpot(e);

                        Int32 res = RegisterExistOnTable(e, "SPOTS", connection, spot.Name);

                        //richTextBoxSpots.AppendText(res.ToString());

                        if (res == 0)
                        {
                            InsertSpotDataIntoTable(spot, connection); 
                        }
                        else
                        {
                            UpdateSpotDataIntoTable(spot, connection);
                        }
                    }

                    connection.Close();

                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);

                    if (connection.State == System.Data.ConnectionState.Open)
                    {
                        connection.Close();
                    }
                }
            });
        }

        #region Parks Auxiliary Functions
        private Park GetDataFromPark(MqttMsgPublishEventArgs e)
        {
            XmlDocument doc = new XmlDocument();
            MemoryStream ms = new MemoryStream(e.Message);
            doc.Load(ms);

            //extrair a informação do parque e colocar na textBox
            string id = doc.SelectSingleNode("park/id").InnerText;
            richTextBoxParks.AppendText($"Id: {id} ");
            string description = doc.SelectSingleNode("park/description").InnerText;
            richTextBoxParks.AppendText($"Description: {description} ");
            int numberOfSpots = int.Parse(doc.SelectSingleNode("park/numberOfSpots").InnerText);
            richTextBoxParks.AppendText($"Number of Spots: {numberOfSpots} ");
            string operatingHours = doc.SelectSingleNode("park/operatingHours").InnerText;
            richTextBoxParks.AppendText($"Operating Hours: {operatingHours} ");
            int numberOfSpecialSpots = int.Parse(doc.SelectSingleNode("park/numberOfSpecialSpots").InnerText);
            richTextBoxParks.AppendText($"Number Special Spots: {numberOfSpecialSpots} ");
            string date = doc.SelectSingleNode("park/updatedGeolocationData").InnerText;
            DateTime updateGeolocationData = DateTime.ParseExact(date, "dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture);
            richTextBoxParks.AppendText($"Date Update Geolocation: {updateGeolocationData}\n");

            return new Park(id, description, numberOfSpots, operatingHours, numberOfSpecialSpots, updateGeolocationData);
        }

        private void InsertParkDataIntoTable(Park value, SqlConnection connection)
        {
            SqlCommand cmd = new SqlCommand("INSERT INTO PARK VALUES(@id, @description,@numberOfSpots,@operatingHours,@numberOfSpecialSpots,@updateGeolocationData)", connection);

            AddParkParameters(value, cmd);

            cmd.ExecuteScalar();

            richTextBoxParks.AppendText($"Inserted {value.Id} on table Park\n\n");
        }

        private void UpdateParkDataIntoTable(Park value, SqlConnection connection)
        {
            SqlCommand cmd = new SqlCommand("UPDATE PARK SET Description=@description,NumberOfSpots=@numberOfSpots,OperatingHours=@operatingHours,NumberOfSpecialSpots=@numberOfSpecialSpots,UpdatedGeolocationData=@updateGeolocationData WHERE Id=@id", connection);

            AddParkParameters(value, cmd);

            int rows = cmd.ExecuteNonQuery();

            richTextBoxParks.AppendText($"Updated {rows} rows on table Park\n\n");
        }

        private void AddParkParameters(Park value, SqlCommand cmd)
        {
            cmd.Parameters.AddWithValue("@id", value.Id);
            cmd.Parameters.AddWithValue("@description", value.Description);
            cmd.Parameters.AddWithValue("@numberOfSpots", value.NumberOfSpots);
            cmd.Parameters.AddWithValue("@operatingHours", value.OperatingHours);
            cmd.Parameters.AddWithValue("@numberOfSpecialSpots", value.NumberOfSpecialSpots);
            cmd.Parameters.AddWithValue("@updateGeolocationData", value.UpdateGeolocationData);
        }
        #endregion

        #region Spot Auxiliary Functions
        private Spot GetDataFromSpot(MqttMsgPublishEventArgs e)
        {
            XmlDocument doc = new XmlDocument();
            MemoryStream ms = new MemoryStream(e.Message);
            doc.Load(ms);

            string id = doc.SelectSingleNode("parkingSpot/id").InnerText;
            richTextBoxSpots.AppendText($"{id} | ");
            string type = doc.SelectSingleNode("parkingSpot/type").InnerText;
            richTextBoxSpots.AppendText($"{type} | ");
            string name = doc.SelectSingleNode("parkingSpot/name").InnerText;
            richTextBoxSpots.AppendText($"{name} | ");
            string location = doc.SelectSingleNode("parkingSpot/location").InnerText;
            richTextBoxSpots.AppendText($"GeoLoc: {location} | ");
            string value = doc.SelectSingleNode("parkingSpot/status/value").InnerText;
            richTextBoxSpots.AppendText($"Status: {value} - ");
            string date = doc.SelectSingleNode("parkingSpot/status/timestamp").InnerText;
            DateTime timestamp = DateTime.ParseExact(date, "dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture);
            richTextBoxSpots.AppendText($"Timestamp: {date} | ");
            int batery = int.Parse(doc.SelectSingleNode("parkingSpot/batteryStatus").InnerText);
            richTextBoxSpots.AppendText($"Batery Status:{getBatteryString(batery)}\n");

            return new Spot(id, type, name, location, value, timestamp, batery);
        }

        private string getBatteryString(int batery)
        {
            return batery == 0 ? "Good" : "Low";
        }

        private void InsertSpotDataIntoTable(Spot value, SqlConnection connection)
        {
            SqlCommand cmd = new SqlCommand("INSERT INTO SPOTS VALUES(@id, @type,@name,@location,@value,@timestamp,@battery)", connection);

            AddSpotParameters(value, cmd);

            cmd.ExecuteScalar();

            richTextBoxSpots.AppendText($"Inserted {value.Name} on table Spots\n\n");
        }

        private void UpdateSpotDataIntoTable(Spot value, SqlConnection connection)
        {
            SqlCommand cmd = new SqlCommand("UPDATE SPOTS SET Id=@id,Type=@type,Location=@location,StatusValue=@value,StatusTimestamp=@timestamp,BatteryLevel=@battery WHERE Name=@name", connection);
            
            AddSpotParameters(value, cmd);

            int rows = cmd.ExecuteNonQuery();

            richTextBoxSpots.AppendText($"Updated {rows} rows on table Spots\n\n");
        }

        private void AddSpotParameters(Spot value, SqlCommand cmd)
        {
            //@id, @type,@name,@location,@value,@timestamp,@battery
            cmd.Parameters.AddWithValue("@id", value.Id);
            cmd.Parameters.AddWithValue("@type", value.Type);
            cmd.Parameters.AddWithValue("@name", value.Name);
            cmd.Parameters.AddWithValue("@location", value.Location);
            cmd.Parameters.AddWithValue("@value", value.StatusValue);
            cmd.Parameters.AddWithValue("@timestamp", value.StatusTimestamp);
            cmd.Parameters.AddWithValue("@battery", value.BatteryLevel);
        }
        #endregion

        private Int32 RegisterExistOnTable(MqttMsgPublishEventArgs e, String tableName, SqlConnection connection, String id)
        {
            SqlCommand cmd;

            if (tableName == "PARK")
            {
                cmd = new SqlCommand($"SELECT COUNT(*) FROM {tableName} WHERE Id=@id", connection);
            }
            else
            {
                cmd = new SqlCommand($"SELECT COUNT(*) FROM {tableName} WHERE NAME=@id", connection);
            }

            cmd.Parameters.AddWithValue("@id", id);
            var res = Convert.ToInt32(cmd.ExecuteScalar());

            return res;
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (client!=null && client.IsConnected)
            {
                client.Disconnect();
            }

            if (connection!=null && connection.State == System.Data.ConnectionState.Open)
            {
                connection.Close();
            }
        }
    }
}
