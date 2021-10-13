using Microsoft.Azure.Devices.Client;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace AirplaneSimulationApp
{
    internal class Program
    {
        private static async Task Main(string[] args)
        {
            Console.WriteLine("BA874 Fligh simulation!");

            var connectionString = Environment.GetEnvironmentVariable("AzureIoTDeviceConnectionString", EnvironmentVariableTarget.User);

            using var client = DeviceClient.CreateFromConnectionString(connectionString);

            while (true)
            {
                var csvLines = File.ReadAllLines("BA874_29622329.csv");

                var firstLine = true;

                var i = 0;

                var previousMessageUTC = DateTime.MinValue;

                var sendNextMessage = DateTime.MinValue;

                foreach (var csvLine in csvLines)
                {
                    if (firstLine)
                    {
                        firstLine = false;
                        continue;
                    }

                    var measurement = ConvertToMeasurement(csvLine);

                    // stall as in realtime

                    if (previousMessageUTC != DateTime.MinValue)
                    {
                        var timeSpanToWait = measurement.UTC - previousMessageUTC;

                        Console.Write($"Sending next message in {timeSpanToWait.TotalSeconds} seconds");

                        sendNextMessage = sendNextMessage.AddSeconds(timeSpanToWait.TotalSeconds);

                        while (DateTime.Now < sendNextMessage)
                        {
                            await Task.Delay(1000);

                            Console.Write(".");
                        }

                        Console.WriteLine();
                    }

                    previousMessageUTC = measurement.UTC;

                    // send

                    string jsonData = JsonConvert.SerializeObject(measurement);

                    using var message = new Message(Encoding.UTF8.GetBytes(jsonData));

                    message.ContentEncoding = "utf-8";
                    message.ContentType = "application/json";

                    message.Properties.Add("messagetype", "simulation");

                    await client.SendEventAsync(message);

                    sendNextMessage = DateTime.Now;

                    i++;

                    Console.WriteLine($"Message sent at {sendNextMessage} ({i}/{csvLines.Length})");

                    await Task.Delay(100);
                }

                Console.WriteLine($"Loop reset.");

                firstLine = true;

                i = 0;

                previousMessageUTC = DateTime.MinValue;

                sendNextMessage = DateTime.MinValue;
            }
        }

        private static Measurement ConvertToMeasurement(string csvLine)
        {
            if (string.IsNullOrEmpty(csvLine))
            {
                return null;
            }

            var measurement = new Measurement();

            var values = csvLine.Split(',');

            measurement.Timestamp = Convert.ToUInt32(values[0]);
            measurement.UTC = Convert.ToDateTime(values[1]);
            measurement.Callsign = values[2];
            measurement.Latitude = Convert.ToDouble(values[3]);
            measurement.Longitude = Convert.ToDouble(values[4]);
            measurement.Altitude = Convert.ToInt32(values[5]);
            measurement.Speed = Convert.ToInt32(values[6]);
            measurement.Direction = Convert.ToInt32(values[7]);

            return measurement;
        }
    }

    internal class Measurement
    {
        public UInt32 Timestamp { get; set; }
        public DateTime UTC { get; set; }
        public string Callsign { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public int Altitude { get; set; }
        public int Speed { get; set; }
        public int Direction { get; set; }
    }
}