# CountIt Sample

[![.net](https://img.shields.io/badge/.net-4.5-green.svg)](http://www.microsoft.com/en-us/download/details.aspx?id=30653)
[![Design Automation](https://img.shields.io/badge/Design%20Automation-v3-green.svg)](http://developer.autodesk.com/)
[![Visual Studio](https://img.shields.io/badge/Visual%20Studio-2017-green.svg)](https://www.visualstudio.com/)

## Description

CountIt is an application that counts walls, floors, doors and windows in a rvt file and its rvt links. It takes a JSON file that specifies which categories of elements will be counted. The output of this application is a text file which contains the element counts.

## Dependencies 

This project was built in Visual Studio 2017. Download it [here](https://www.visualstudio.com/).

This sample references Revit 2018's `RevitAPI.dll`, [DesignAutomationBridge.dll](https://revitio.s3.amazonaws.com/documentation/DesignAutomationBridge.dll) for Revit 2018 and [Newtonsoft JSON framework](https://www.newtonsoft.com/json).

In order to POST appbundles, activities, and workitems you must have credentials for [Forge](../Docs/Forge.md).

## Building CountIt.sln

Download [DesignAutomationBridge.dll](https://revitio.s3.amazonaws.com/documentation/DesignAutomationBridge.dll) for Revit 2018 and [Newtonsoft JSON framework](https://www.newtonsoft.com/json).  DesignAutomationBridge.dlls for other Revit versions can be found [here](../Docs/AppBundle.md#engine-version-aliases).

Find `RevitAPI.dll` in your Revit 2018 install location and note its location. 

Clone this repository and open `CountIt.sln` in Visual Studio.  

In the CountIt C# project, repair the references to `DesignAutomationBridge`, `Newtonsoft JSON framework` and `RevitAPI`.  You can do this by removing and re-adding the references, or by opening the `CountIt.csproj` for edit and manually updating the reference paths.

Build `CountIt.sln` in `Release` or `Debug` configuration.

## Creating and Publishing the Appbundle

Create an `appbundle` zip package from the build outputs and publish the `appbundle` to Design Automation.  

The `JSON` in your `appbundle` POST should look like this:
```json
{
  "id": "CountItApp",
  "engine": "Autodesk.Revit+2018",
  "description": "CountIt appbundle based on Revit 2018"
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
   "id": "CountItActivity",
   "commandLine": [ "$(engine.path)\\\\revitcoreconsole.exe /i $(args[rvtFile].path) /al $(appbundles[CountItApp].path)" ],
   "parameters": {
      "rvtFile": {
         "zip": false,
         "ondemand": false,
         "verb": "get",
         "description": "Input Revit model",
         "required": true,
         "localName": "$(rvtFile)"
      },
      "countItParams": {
         "zip": false,
         "ondemand": false,
         "verb": "get",
         "description": "CountIt parameters",
         "required": false,
         "localName": "CountItParams.json"
      },
      "result": {
         "zip": false,
         "ondemand": false,
         "verb": "put",
         "description": "Results",
         "required": true,
         "localName": "result.txt"
      }
   },
   "engine": "Autodesk.Revit+2018",
   "appbundles": [ "YourNickname.CountItApp+test" ],
   "description": "Count and output elements from Revit file."
}
```
Notes:
* `engine` = `Autodesk.Revit+2018` - A list of engine versions can be found [here](../Docs/AppBundle.md#engine-version-aliases).
* `YourNickname` - The owner of appbundle `CountItApp`. More information can be found [here](../Docs/Nickname.md).

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

The `JSON` that accompanies the `workitem` POST will look like this:

```json
{
  "activityId": "YourNickname.CountItActivity+test",
  "arguments": {
    "rvtFile": {
      "url": "https://myWebsite/CountIt.rvt"
    },
    "countItParams": {
      "url": "data:application/json,{'walls': false, 'floors': true, 'doors': true, 'windows': true}"
    },
    "result": {
      "verb": "put",
      "url": "https://myWebsite/signed/url/to/result.txt"
    }
  }
}
```
Notes:
* `YourNickname` - The owner of activity `CountItActivity`. More information can be found [here](../Docs/Nickname.md).

> **The instructions for this step and more about `workitem` are [here](../Docs/WorkItem.md)**.

`CountItActivity` expects an input file `CountItParams.json`. The contents of the embedded JSON will be stored in a file named `CountItParams.json`, as specified by the `parameters` of `countItParams` in the activity `CountItActivity`. The CountIt application reads this file from current working folder and parses the JSON to determine which element categories should be counted. The counting result is saved to `result.txt` which will be uploaded to `url` you provide in the workitem.

The function `CountElementsInModel` in [Main.cs](CountItApp/Main.cs) performs these operations.
