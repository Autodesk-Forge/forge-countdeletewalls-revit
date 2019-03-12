# DeleteWalls Sample

[![.net](https://img.shields.io/badge/.net-4.5-green.svg)](http://www.microsoft.com/en-us/download/details.aspx?id=30653)
[![Design Automation](https://img.shields.io/badge/Design%20Automation-v3-green.svg)](http://developer.autodesk.com/)
[![visual studio](https://img.shields.io/badge/Visual%20Studio-2017-green.svg)](https://www.visualstudio.com/)

## Description

DeleteWalls is an application that takes in a rvt file and outputs another rvt file with all of the walls removed.

## Dependencies

This project was built in Visual Studio 2017. Download it [here](https://www.visualstudio.com/).

This sample references Revit 2018's `RevitAPI.dll` and [DesignAutomationBridge.dll](https://revitio.s3.amazonaws.com/documentation/DesignAutomationBridge.dll) for Revit 2018.

In order to POST appbundles, activities, and workitems you must have credentials for [Forge](../Docs/Forge.md).

## Building DeleteWalls.sln

Download [DesignAutomationBridge.dll](https://revitio.s3.amazonaws.com/documentation/DesignAutomationBridge.dll) for Revit 2018. DesignAutomationBridge.dlls for other Revit versions can be found [here](../Docs/AppBundle.md#engine-version-aliases).

Find `RevitAPI.dll` in your Revit 2018 install location and note its location. 

Clone this repository and open `DeleteWalls.sln` in Visual Studio.  

In the DeleteWalls C# project, repair the references to `DesignAutomationBridge` and `RevitAPI`.  You can do this by removing and re-adding the references, or by opening the `DeleteWalls.csproj` for edit and manually updating the reference paths.

Build `DeleteWalls.sln` in `Release` or `Debug` configuration.

## Creating and Publishing the Appbundle

Create an `appbundle` zip package from the build outputs and publish the `appbundle` to Design Automation.

The `JSON` in your appbundle POST should look like this:
```json
{
  "id": "DeleteWallsApp",
  "engine": "Autodesk.Revit+2018",
  "description": "DeleteWalls appbundle based on Revit 2018"
}
```
Notes:
* `engine` = `Autodesk.Revit+2018` - A list of engine versions can be found [here](../Docs/AppBundle.md#engine-version-aliases).

After you upload the `appbundle` zip package, you should create an alias for this appbundle. The `JSON` in the POST should look like this:
```json
{
  "version": 1,
  "id": "test"
}
```

> **The instructions for these steps and more about `appbundle` are [here](../Docs/AppBundle.md)**.

## Creating the Activity

Define an `activity` to run against the `appbundle`.

The `JSON` that accompanies the `activity` POST will look like this:
```json
{
   "id": "DeleteWallsActivity",
   "commandLine": [ "$(engine.path)\\\\revitcoreconsole.exe /i $(args[rvtFile].path) /al $(appbundles[DeleteWallsApp].path)" ],
   "parameters": {
      "rvtFile": {
         "zip": false,
         "ondemand": false,
         "verb": "get",
         "description": "Input Revit model",
         "required": true,
         "localName": "$(rvtFile)"
      },
      "result": {
         "zip": false,
         "ondemand": false,
         "verb": "put",
         "description": "Results",
         "required": true,
         "localName": "result.rvt"
      }
   },
   "engine": "Autodesk.Revit+2018",
   "appbundles": [ "YourNickname.DeleteWallsApp+test" ],
   "description": "Delete walls from Revit file."
}
```
Notes:
* `engine` = `Autodesk.Revit+2018` - A list of engine versions can be found [here](../Docs/AppBundle.md#engine-version-aliases).
* `YourNickname` - The owner of appbundle `DeleteWallsApp`. More information can be found [here](../Docs/Nickname.md).

Then you should create an alias for this activity. The `JSON` in the POST should look like this:
```json
{
  "version": 1,
  "id": "test"
}
```

> **The instructions for these steps and more about `activity` are [here](../Docs/Activity.md)**.


## POST a WorkItem

Now POST a `workitem` against the `activity` to run a job on your `appbundle`.

The `JSON` that accompanies the `WorkItem` POST will look like this:
```json
{
  "activityId": "YourNickname.DeleteWallsActivity+test",
  "arguments": {
    "rvtFile": {
      "url": "https://myWebsite/DeleteWalls.rvt"
    },
    "result": {
      "verb": "put",
      "url": "https://myWebsite/signed/url/to/result.rvt"
    }
  }
}
```
Notes:
* `YourNickname` - The owner of activity `DeleteWallsActivity`. More information can be found [here](../Docs/Nickname.md).

> **The instructions for this step and more about `workitem` are [here](../Docs/WorkItem.md)**.

The value of the `rvtFile` parameter is the URL to the input file `DeleteWalls.rvt`. DeleteWalls application opens `DeleteWalls.rvt`, deletes the walls in it and saves it as `result.rvt`. The output file `result.rvt` will be uploaded to `url` you provide in the workitem.  

The function `DeleteAllWalls` in [DeleteWalls.cs](DeleteWallsApp/DeleteWalls.cs) performs these operations.
