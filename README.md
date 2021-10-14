# techday2021

This repository hosts some configuration and coding projects related to the Tech Days 2021 event.

## Flight data

### V1

Flight data is available at [drop box](https://www.dropbox.com/s/pj2ey7yfyjfifk3/BA874_29622329.csv?dl=0).

*Note*: This data is not finalized.

*Note*: To read this data using String.Split(','), you need to remove the double quotes first (") due to the comma inside the the position (10-13-2021).  

## V2

Flight data is now available at [GitHub](https://github.com/CliffAgius/TechDays2021/tree/main/FlightData).

## Device connection string

A device connection string should be added to the USER environment variables on your development machine.

The name of the variable is:

    AzureIoTDeviceConnectionString

You an alter the 'target' as needed

    var connectionString = Environment.GetEnvironmentVariable("AzureIoTDeviceConnectionString", EnvironmentVariableTarget.User);

## Loop

This application loops. After running through all the lines, it just starts again.

## Sample outcome

### V1

This is a sample of the Time Series Insights where you can see the real time simulation loops through the incoming lines:

![image](https://user-images.githubusercontent.com/694737/137129971-9d008a29-e30a-4fd3-a2b2-ce7c75590236.png)

## Links

## Tech Days 2021 Event

Registration:
    https://mktoevents.com/Microsoft+Event/302272/157-GQE-382

### TimeSeries Insights

MS Learn:
    https://docs.microsoft.com/en-us/learn/modules/explore-analyze-time-series-insights/

IoT Show:
    https://channel9.msdn.com/Shows/Internet-of-Things-Show/Using-Azure-Time-Series-Insights-to-create-an-Industrial-IoT-analytics-platform

    