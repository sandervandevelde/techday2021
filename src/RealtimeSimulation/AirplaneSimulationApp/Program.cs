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
                var csvLines = File.ReadAllLines("BA874-v2.csv");

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
                        var timeSpanToWait = measurement.DateTimeUtc - previousMessageUTC;

                        Console.Write($"Sending next message in {timeSpanToWait.TotalSeconds} seconds");

                        sendNextMessage = sendNextMessage.AddSeconds(timeSpanToWait.TotalSeconds);

                        while (DateTime.Now < sendNextMessage)
                        {
                            await Task.Delay(1000);

                            Console.Write(".");
                        }

                        Console.WriteLine();
                    }

                    previousMessageUTC = measurement.DateTimeUtc;

                    // send

                    measurement.DateTimeUtc = DateTime.UtcNow;

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

                Console.WriteLine($"Loop reset. (waiting ten minutes before next flight)");

                await Task.Delay(600000);

                Console.WriteLine($"Starting next flight");

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

            // Callsign,Date,Time,Position,Latitude,Longditude,Altitude,V/S FPM,Seconds Last report,Speed,Direction,OutSide Air Temp,Wind Direction,Wind Speed

            measurement.Callsign = values[0];  // 0

            var dateParts = values[1].Split('/');

            var dateTimeString = $"{dateParts[2]}-{dateParts[1]}-{dateParts[0]}T{values[2]}Z";

            measurement.DateTimeUtc = Convert.ToDateTime(dateTimeString);  // 1 + 2
            measurement.Position = values[3];  //3
            measurement.Latitude = Convert.ToDouble(values[4]); // 4
            measurement.Longitude = Convert.ToDouble(values[5]); // 5
            measurement.Altitude = Convert.ToInt32(values[6]);  //6
            measurement.VSFPM = Convert.ToInt32(values[7]);  // 7
            measurement.SecondsLastReport = Convert.ToInt32(values[8]);  // 8
            measurement.Speed = Convert.ToInt32(values[9]); // 9
            measurement.Direction = Convert.ToInt32(values[10]); // 10
            measurement.OutsideAirTemp = Convert.ToDouble(values[11]); // 11
            measurement.WindDirection = Convert.ToDouble(values[12]); // 12
            measurement.WindSpeed = Convert.ToInt32(values[13]); // 13

            return measurement;
        }
    }

    /// <summary>
    /// Callsign,Date,Time,Position,Latitude,Longditude,Altitude,V/S FPM,Seconds Last report,Speed,Direction,OutSide Air Temp,Wind Direction,Wind Speed
    /// </summary>
    internal class Measurement
    {
        public string Callsign { get; set; }
        public DateTime DateTimeUtc { get; set; }
        public string Position { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public int Altitude { get; set; }
        public int VSFPM { get; set; }
        public int SecondsLastReport { get; set; }
        public int Speed { get; set; }
        public int Direction { get; set; }
        public double OutsideAirTemp { get; set; }
        public double WindDirection { get; set; }
        public int WindSpeed { get; set; }
    }
}