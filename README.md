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
1. Open each deployed function to get the Function URL by clicking "</> Get function URL"

## How to use functions

1. Add your manifest URL as a query string (manifest=) to your function URL
1. Pass the function URL to your player
