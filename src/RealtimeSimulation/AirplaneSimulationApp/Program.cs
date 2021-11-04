using Microsoft.Azure.Devices.Client;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;

namespace AirplaneSimulationApp
{
    internal class Program
    {
        private static async Task Main(string[] args)
        {
            Console.WriteLine("AAW874 Fligh simulation!");

            var connectionString = Environment.GetEnvironmentVariable("AzureIoTDeviceConnectionString", EnvironmentVariableTarget.User);

            using var client = DeviceClient.CreateFromConnectionString(connectionString);

            var deviceId = "AAW874";  // HARDCODED

            while (true)
            {
                var csvLines = File.ReadAllLines("AAW874-v2.csv");

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

                    var measurement = ConvertToMeasurement(csvLine, deviceId);

                    // stall as in realtime

                    if (previousMessageUTC != DateTime.MinValue)
                    {
                        var timeSpanToWait = measurement.dateTimeUtc - previousMessageUTC;

                        Console.Write($"Sending next message in {timeSpanToWait.TotalSeconds} seconds");

                        sendNextMessage = sendNextMessage.AddSeconds(timeSpanToWait.TotalSeconds);

                        while (DateTime.Now < sendNextMessage)
                        {
                            await Task.Delay(1000);

                            Console.Write(".");
                        }

                        Console.WriteLine();
                    }

                    previousMessageUTC = measurement.dateTimeUtc;

                    // send

                    measurement.dateTimeUtc = DateTime.UtcNow;

                    string jsonData = JsonConvert.SerializeObject(measurement);

                    using var message = new Message(Encoding.UTF8.GetBytes(jsonData));

                    message.ContentEncoding = "utf-8";
                    message.ContentType = "application/json";

                    message.Properties.Add("messageType", "simulation");
                    message.Properties.Add("messageCount", i.ToString());

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

        private static int _messageId = 0;

        private static Measurement ConvertToMeasurement(string csvLine, string deviceId)
        {
            if (string.IsNullOrEmpty(csvLine))
            {
                return null;
            }

            var provider = new NumberFormatInfo();
            provider.NumberDecimalSeparator = ".";
            provider.NumberGroupSeparator = ",";

            var measurement = new Measurement();

            var values = csvLine.Split(',');

            // Callsign,Date,Time,Position,Latitude,Longditude,Altitude,V/S FPM,Seconds Last report,Speed,Direction,OutSide Air Temp,Wind Direction,Wind Speed

            _messageId++;
            measurement.messageId = _messageId;

            measurement.deviceId = deviceId;

            measurement.callsign = values[0];  // 0

            var dateParts = values[1].Split('/');

            var dateTimeString = $"{dateParts[2]}-{dateParts[1]}-{dateParts[0]}T{values[2]}Z";

            measurement.date = values[1];
            measurement.time = values[2];

            measurement.dateTimeUtc = Convert.ToDateTime(dateTimeString);  // 1 + 2
            measurement.position = values[3];  //3
            measurement.latitude = Convert.ToDouble(values[4], provider); // 4
            measurement.longitude = Convert.ToDouble(values[5], provider); // 5
            measurement.altitude = Convert.ToInt32(values[6]);  //6
            measurement.vSFPM = Convert.ToInt32(values[7]);  // 7
            measurement.secondsLastReport = Convert.ToInt32(values[8]);  // 8
            measurement.speed = Convert.ToInt32(values[9]); // 9
            measurement.direction = Convert.ToInt32(values[10]); // 10
            measurement.outsideAirTemp = Convert.ToDouble(values[11], provider); // 11  
            measurement.windDirection = Convert.ToDouble(values[12], provider); // 12
            measurement.windSpeed = Convert.ToInt32(values[13]); // 13

            return measurement;
        }
    }

    /// <summary>
    /// Callsign,Date,Time,Position,Latitude,Longditude,Altitude,V/S FPM,Seconds Last report,Speed,Direction,OutSide Air Temp,Wind Direction,Wind Speed
    /// </summary>
internal class Measurement
{
    public int messageId {get;set;}
    public string deviceId {get;set;}
    public string callsign { get; set; }
    public string date { get; set; }
    public string time { get; set; }
    public DateTime dateTimeUtc { get; set; }
    public string position { get; set; }
    public double latitude { get; set; }
    public double longitude { get; set; }
    public int altitude { get; set; }
    public int vSFPM { get; set; }
    public int secondsLastReport { get; set; }
    public int speed { get; set; }
    public int direction { get; set; }
    public double outsideAirTemp { get; set; }
    public double windDirection { get; set; }
    public int windSpeed { get; set; }
}
}