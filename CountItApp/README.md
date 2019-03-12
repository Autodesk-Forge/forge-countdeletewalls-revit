# CountIt Sample

[![.net](https://img.shields.io/badge/.net-4.7-green.svg)](http://www.microsoft.com/en-us/download/details.aspx?id=30653)
[![Design Automation](https://img.shields.io/badge/Design%20Automation-v3-green.svg)](http://developer.autodesk.com/)
[![Visual Studio](https://img.shields.io/badge/Visual%20Studio-2017-green.svg)](https://www.visualstudio.com/)

## Description

CountIt is an application that counts walls, floors, doors and windows in a rvt file and its rvt links. It takes a JSON file that specifies which categories of elements will be counted. The output of this application is a text file which contains the element counts.

## Building CountIt.csproj

Right-click on References, then Add Reference and Browse for **RevitAPI.dll** (by default under `C:\Program Files\Autodesk\Revit 201x\ folder`). Then right-click on this **RevitAPI** reference, go to Properties, then set Copy Local to False.

Then right-click on the project, go to Manage NuGet Packages..., under **Browser** you can search for `DesignAutomation.Revit` and install `Autodesk.Forge.DesignAutomation.Revit` (choose the appropriate Revit version you need). Then search and install `Newtonsoft.Json` (which is used to parse input data in JSON format).

Build `CountIt.csproj` in `Release` or `Debug` configuration.

## Creating and Publishing the Appbundle

Create an `appbundle` zip package from the build outputs and publish the `appbundle` to Design Automation.  

The `JSON` in your `appbundle` POST should look like this:
```json
{
  "id": "CountItApp",
  "engine": "Autodesk.Revit+2019",
  "description": "CountIt appbundle based on Revit 2019"
}
```
Notes:
* `engine` = `Autodesk.Revit+2019` - A list of engine versions can be found [here](https://forge.autodesk.com/en/docs/design-automation/v3/reference/http/engines-GET/).

After you upload the `appbundle` zip package, you should create an alias for this appbundle. The `JSON` in the POST should look like this:
```json
{
  "version": 1,
  "id": "dev"
}
```

> **The instructions for these steps and more about `appbundle` are [here](https://forge.autodesk.com/en/docs/design-automation/v3/tutorials/revit/step4-publish-appbundle/)**.


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
         "localName": "Params.json"
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
   "engine": "Autodesk.Revit+2019",
   "appbundles": [ "YourNickname.CountItApp+dev" ],
   "description": "Count and output elements from Revit file."
}
```
Notes:
* `engine` = `Autodesk.Revit+2019` - A list of engine versions can be found [here](https://forge.autodesk.com/en/docs/design-automation/v3/reference/http/engines-GET/).
* `YourNickname` - The owner of appbundle `CountItApp`. More information can be found [here](https://forge.autodesk.com/en/docs/design-automation/v3/tutorials/revit/step3-create-nickname/).

Then you should create an alias for this activity. The `JSON` in the POST should look like this:
```json
{
  "version": 1,
  "id": "dev"
}
```

> **The instructions for these steps and more about `activity` are [here](https://forge.autodesk.com/en/docs/design-automation/v3/tutorials/revit/step5-publish-activity/)**.

## POST a WorkItem

Now POST a `workitem` against the `activity` to run a job on your `appbundle`.

The `JSON` that accompanies the `workitem` POST will look like this:

```json
{
  "activityId": "YourNickname.CountItActivity+dev",
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
* `YourNickname` - The owner of activity `CountItActivity`. More information can be found [here](https://forge.autodesk.com/en/docs/design-automation/v3/tutorials/revit/step3-create-nickname/).

> **The instructions for this step and more about `workitem` are [here](https://forge.autodesk.com/en/docs/design-automation/v3/tutorials/revit/step6-post-workitem/)**.

`CountItActivity` expects an input file `Params.json`. The contents of the embedded JSON will be stored in a file named `Params.json`, as specified by the `parameters` of `countItParams` in the activity `CountItActivity`. The CountIt application reads this file from current working folder and parses the JSON to determine which element categories should be counted. The counting result is saved to `result.txt` which will be uploaded to `url` you provide in the workitem.

The function `CountElementsInModel` in [Main.cs](CountItApp/Main.cs) performs these operations.
