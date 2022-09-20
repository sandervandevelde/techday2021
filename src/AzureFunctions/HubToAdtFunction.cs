// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.EventGrid.Models;
using Microsoft.Azure.WebJobs.Extensions.EventGrid;
using Microsoft.Extensions.Logging;
using Azure.DigitalTwins.Core;
using Azure.Identity;
using System.Net.Http;
using Azure.Core.Pipeline;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Text;
using System.Threading.Tasks;
using AdtFunctions;

namespace Contoso.AdtFunctions
{
    public static class HubToAdtFunction
    {
        private static readonly string adtInstanceUrl = Environment.GetEnvironmentVariable("ADT_SERVICE_URL");
        private static readonly HttpClient httpClient = new HttpClient();

        [FunctionName("HubToAdtFunction")]
        public async static Task Run(
            [EventGridTrigger] EventGridEvent eventGridEvent,
            ILogger log)
        {
            log.LogInformation($"*** incoming message: '{eventGridEvent.Data}'");

            if (adtInstanceUrl == null)
            {
                log.LogError("Application setting \"ADT_SERVICE_URL\" not set");
                return;
            }

            try
            {
                ManagedIdentityCredential cred =
                    new ManagedIdentityCredential("https://digitaltwins.azure.net");

                DigitalTwinsClient client =
                    new DigitalTwinsClient(
                        new Uri(adtInstanceUrl),
                        cred,
                        new DigitalTwinsClientOptions { Transport = new HttpClientTransport(httpClient) });

                log.LogInformation($"Azure digital twins service client connection created.");

                if (eventGridEvent != null 
                        && eventGridEvent.Data != null)
                {
                    var deviceMessage = JsonConvert.DeserializeObject<EventGridMessage>(eventGridEvent.Data.ToString());

                    var callsign = deviceMessage.body.callsign;
                    var latitude = deviceMessage.body.latitude;
                    var longitude = deviceMessage.body.longitude;
                    var altitude = deviceMessage.body.altitude;
                    var vSFPM = deviceMessage.body.vSFPM;
                    var secondsLastReport = deviceMessage.body.secondsLastReport;
                    var speed = deviceMessage.body.speed;
                    var direction = deviceMessage.body.direction;
                    var outsideAirTemp = deviceMessage.body.outsideAirTemp;
                    var windDirection = deviceMessage.body.windDirection;
                    var windSpeed = deviceMessage.body.windSpeed;
                                       
                    var patch = new Azure.JsonPatchDocument();
                    //patch.AppendReplace("/lastCallsign", callsign);
                    //patch.AppendReplace("/lastLatitude", latitude);
                    //patch.AppendReplace("/lastLongitude", longitude);
                    //patch.AppendReplace("/lastAltitude", altitude);
                    //patch.AppendReplace("/lastVSFPM", vSFPM);
                    //patch.AppendReplace("/lastSecondsLastReport", secondsLastReport);
                    //patch.AppendReplace("/lastSpeed", speed);
                    //patch.AppendReplace("/lastDirection", direction);
                    //patch.AppendReplace("/lastOutsideAirTemp", outsideAirTemp);
                    //patch.AppendReplace("/lastWindDirection", windDirection);
                    //patch.AppendReplace("/lastWindSpeed", windSpeed);
                    //patch.AppendReplace("/abbreviation", "test");

                    var deviceId = deviceMessage.systemProperties.iothubconnectiondeviceid;

                    log.LogInformation($"PATCHING {deviceId}: '{patch}'");                        

                    //await client.UpdateDigitalTwinAsync(deviceId, patch);

                    var bodyJson = JsonConvert.SerializeObject(deviceMessage.body);

                    await client.PublishTelemetryAsync(deviceId, null, bodyJson);
                }
            }
            catch (Exception e)
            {
                log.LogError(e.Message);
            }
        }
    }
}