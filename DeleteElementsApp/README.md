# DeleteElements Sample

[![.net](https://img.shields.io/badge/.net-4.7-green.svg)](http://www.microsoft.com/en-us/download/details.aspx?id=30653)
[![Design Automation](https://img.shields.io/badge/Design%20Automation-v3-green.svg)](http://developer.autodesk.com/)
[![visual studio](https://img.shields.io/badge/Visual%20Studio-2017-green.svg)](https://www.visualstudio.com/)

## Description

DeleteElements is an application that takes in a rvt file and outputs another rvt file with all of the specified element types removed.

## Building DeleteElements.csproj

Right-click on References, then Add Reference and Browse for **RevitAPI.dll** (by default under `C:\Program Files\Autodesk\Revit 201x\ folder`). Then right-click on this **RevitAPI** reference, go to Properties, then set Copy Local to False.

Then right-click on the project, go to Manage NuGet Packages..., under **Browser** you can search for `DesignAutomation.Revit` and install `Autodesk.Forge.DesignAutomation.Revit` (choose the appropriate Revit version you need). Then search and install `Newtonsoft.Json` (which is used to parse input data in JSON format).

Build `DeleteElements.csproj` in `Release` or `Debug` configuration.

## Creating and Publishing the Appbundle

Create an `appbundle` zip package from the build outputs and publish the `appbundle` to Design Automation.

The `JSON` in your appbundle POST should look like this:
```json
{
  "id": "DeleteElementsApp",
  "engine": "Autodesk.Revit+2019",
  "description": "DeleteElements appbundle based on Revit 2019"
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
   "id": "DeleteElementsActivity",
   "commandLine": [ "$(engine.path)\\\\revitcoreconsole.exe /i $(args[rvtFile].path) /al $(appbundles[DeleteElementsApp].path)" ],
   "parameters": {
      "rvtFile": {
         "zip": false,
         "ondemand": false,
         "verb": "get",
         "description": "Input Revit model",
         "required": true,
         "localName": "$(rvtFile)"
      },
      "deleteElementsParams": {
         "zip": false,
         "ondemand": false,
         "verb": "get",
         "description": "DeleteElements parameters",
         "required": false,
         "localName": "Params.json"
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
   "engine": "Autodesk.Revit+2019",
   "appbundles": [ "YourNickname.DeleteElementsApp+dev" ],
   "description": "Delete elements from Revit file."
}
```
Notes:
* `engine` = `Autodesk.Revit+2019` - A list of engine versions can be found [here](https://forge.autodesk.com/en/docs/design-automation/v3/reference/http/engines-GET/).
* `YourNickname` - The owner of appbundle `DeleteElementsApp`. More information can be found [here](https://forge.autodesk.com/en/docs/design-automation/v3/tutorials/revit/step3-create-nickname/).

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

The `JSON` that accompanies the `WorkItem` POST will look like this:
```json
{
  "activityId": "YourNickname.DeleteElementsActivity+dev",
  "arguments": {
    "rvtFile": {
      "url": "https://myWebsite/DeleteElements.rvt"
    },
    "deleteElementsParams": {
      "url": "data:application/json,{'walls': false, 'floors': true, 'doors': true, 'windows': true}"
    },   
     "result": {
      "verb": "put",
      "url": "https://myWebsite/signed/url/to/result.rvt"
    }
  }
}
```
Notes:
* `YourNickname` - The owner of activity `DeleteElementsActivity`. More information can be found [here](.https://forge.autodesk.com/en/docs/design-automation/v3/tutorials/revit/step3-create-nickname/).

> **The instructions for this step and more about `workitem` are [here](https://forge.autodesk.com/en/docs/design-automation/v3/tutorials/revit/step6-post-workitem/)**.

The value of the `rvtFile` parameter is the URL to the input file `DeleteElements.rvt`. DeleteElements application opens `DeleteElements.rvt`, deletes the specified element types in it and saves it as `result.rvt`. The output file `result.rvt` will be uploaded to `url` you provide in the workitem.  

The function `DeleteAllElements` in [DeleteElements.cs](DeleteElementsApp/DeleteElements.cs) performs these operations.
