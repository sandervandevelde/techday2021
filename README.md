# techday2021

This repository hosts some configuration and coding projects related to the Tech Days 2021 event.

## Flight data

Flight data is available at [drop box](https://www.dropbox.com/s/pj2ey7yfyjfifk3/BA874_29622329.csv?dl=0).

*Note*: This data is not finalized.

*Note*: To read this data using String.Split(','), you need to remove the double quotes first (") due to the comma inside the the position (10-13-2021).  

## Device connection string

A device connection string should be added to the USER environment variables on your development machine.

The name of the variable is:

    AzureIoTDeviceConnectionString

You an alter the 'target' as needed

    var connectionString = Environment.GetEnvironmentVariable("AzureIoTDeviceConnectionString", EnvironmentVariableTarget.User);

## Loop

This application loops. After running through all the lines, it just starts again.

