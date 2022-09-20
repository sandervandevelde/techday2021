using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace AdtFunctions
{
    internal class EventGridMessage
    {
        public UserProperties properties {get; set;}
        public SystemProperties systemProperties { get; set; }
        public Body body { get; set; }
    }

    /// <summary>
    ///"body": {
    ///    "messageId": 3,
    ///    "deviceId": "AAW874",
    ///    "callsign": "AAW874",
    ///    "date": "04/10/2021",
    ///    "time": "11:21:42",
    ///    "dateTimeUtc": "2022-09-20T19:56:33.9696079Z",
    ///    "position": "\"51.473167;-0.485792\"",
    ///    "latitude": 51.473167,
    ///    "longitude": -0.485792,
    ///    "altitude": 0,
    ///    "vSFPM": 0,
    ///    "secondsLastReport": 13,
    ///    "speed": 2,
    ///    "direction": 270,
    ///    "outsideAirTemp": 15.0,
    ///    "windDirection": 250.0,
    ///    "windSpeed": 6
    ///}
    /// </summary>
    public class Body
    {
        public int messageId { get; set; }
        public string deviceId { get; set; }
        public string callsign { get; set; }
        public string date { get; set; }
        public string time { get; set; }
        public string dateTimeUtc { get; set; }
        public string position { get; set; }
        public double latitude { get; set; }
        public double longitude { get; set; }
        public double altitude { get; set; }
        public double vSFPM { get; set; }
        public double secondsLastReport { get; set; }
        public double speed { get; set; }
        public double direction { get; set; }
        public double outsideAirTemp { get; set; }
        public double windDirection { get; set; }
        public double windSpeed { get; set; }
    }

    /// <summary>
    /// "systemProperties": {
    ///    "iothub-content-type": "application/json",
    ///    "iothub-content-encoding": "utf-8",
    ///    "iothub-connection-device-id": "AAW874sim",
    ///    "iothub-connection-auth-method": "{\"scope\":\"device\",\"type\":\"sas\",\"issuer\":\"iothub\",\"acceptingIpFilterRule\":null}",
    ///    "iothub-connection-auth-generation-id": "637984218972246327",
    ///    "iothub-enqueuedtime": "2022-09-20T19:56:34.013Z",
    ///    "iothub-message-source": "Telemetry"
    /// },
    /// </summary>
    public class SystemProperties
    {
        [JsonProperty("iothub-content-type")]
        public string iothubcontenttype {get; set;}

        [JsonProperty("iothub-content-encoding")]
        public string iothubcontentencoding {get; set;}

        [JsonProperty("iothub-connection-device-id")]
        public string iothubconnectiondeviceid { get; set; }

        [JsonProperty("iothub-connection-auth-method")]
        public string iothubconnectionauthmethod { get; set; }

        [JsonProperty("iothub-connection-auth-generation-id")]
        public string iothubconnectionauthgenerationid { get; set; }

        [JsonProperty("iothub-enqueuedtime")]
        public string iothubenqueuedtime { get; set; }

        [JsonProperty("iothub-message-source")]
        public string iothubmessagesource { get; set; }
    }

    /// <summary>
    /// "properties": {
    ///    "messageType": "simulation",
    ///    "messageCount": "2"
    /// },
    /// </summary>
    public class UserProperties
    {
        public string messageType { get; set; }
        public string messageCount { get; set; }
    }
}
