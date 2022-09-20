using AdtFunctions;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Text.Json.Serialization;

namespace EvetngridMessageTestApp
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            var json = File.ReadAllText("eventgrid.json");

            var deviceMessage = JsonConvert.DeserializeObject<EventGridMessage>(json);


            var message = new EventGridMessage();

            message.body = new Body();

            message.body.messageId = 1;
            message.body.deviceId = "aaa";
            message.body.outsideAirTemp = 42;

            message.properties = new UserProperties();
            message.properties.messageType = "xxx";
            message.properties.messageCount = "32";

            message.systemProperties = new SystemProperties();
            message.systemProperties.iothubenqueuedtime = "aaaa";
            message.systemProperties.iothubmessagesource = "bbbb";
            message.systemProperties.iothubcontenttype = "cccc";
            message.systemProperties.iothubcontentencoding = "dddd";
            message.systemProperties.iothubconnectiondeviceid = "device";

            var js = JsonConvert.SerializeObject(message);




        }
    }
}
