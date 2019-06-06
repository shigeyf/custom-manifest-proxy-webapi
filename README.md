# A simple Web API implementation of Custom Manifest for Adaptive Media Streaming

This is a simple sample Azure Function implementation for manifest proxy Web API.

## How to deploy this sample

1. Open Visual Studio solution file in this repository.
1. Right-click on "custom-manifest-proxy" project to select "Publish..." to publish this Azure Function application.
1. Select "Create New" to create a new Azure Function app.
1. Create a new Azure Function - App Services
    - Input your Azure Function name
    - Select appropriate Subscription and Resource Group
    - Select Y1 as Hosting Plan
    - Create a new Storage account for this Azure Function app
1. Click "Create" button
1. After deployment of the new Azure Function, open Azure Portal
1. Open the new Azure Function app
1. Navigate to "Platform features" -> "CORS"
1. Remove all entries of "Allowed Origins", and add "*", then click "Save" button

## Get function URLs

1. Open the new Azure Function app
1. Navigate to Functions menu
1. Open each deployed function to get the Function URL by clicking "</> Get function URL", which includes an auth code to be able to access the function.

## How to use functions

1. Add your manifest URL as a query string (*manifest=*) to your function URL
1. Pass the function URL to your player

## Samples functions in this project repository

- HlsCustomManifestProxy
  - This sample function allows to add Auth Token query string parameter (*?token=xxx*) to key acquisition URL in second level HLS m3u8 playlist for the authorization of key delivery services.
  - This is because Safari (with native HLS player stack) does not support handling "**Authorization:**" HTTP request header, which is the generic way for key acquisition authorization defined in OAuth2.
  - Please see more in detail at [this article](https://azure.microsoft.com/en-us/blog/how-to-make-token-authorized-aes-encrypted-hls-stream-working-in-safari/)

- MpegDashCustomManifestProxy
  - This sample function gives a very simple template with adding BaseURL in MPEG-DASH MPD manifest (the BaseURL directive is required in the modified MPD manifest, because the origin of MPD manifest is now this function host origin and MPEG-DASH player needs to retrieve media fragments from the original origin, which is different from the function host origin.
  - You can feel free to add any code to this template in order to modify the original MPD manifest file.
