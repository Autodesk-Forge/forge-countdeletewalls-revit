# design.automation-csharp-revit.count.delete

![Platforms](https://img.shields.io/badge/Web-Windows|MacOS-lightgray.svg)
![.NET](https://img.shields.io/badge/.NET%20Core-2.1-blue.svg)
[![oAuth2](https://img.shields.io/badge/oAuth2-v1-green.svg)](http://developer.autodesk.com/)
[![Data-Management](https://img.shields.io/badge/Data%20Management-v1-green.svg)](http://developer.autodesk.com/)
[![Model-Derivative](https://img.shields.io/badge/Model%20Derivative-v1-green.svg)](http://developer.autodesk.com/)
[![Design-Automation](https://img.shields.io/badge/Design%20Automation-v3-green.svg)](http://developer.autodesk.com/)

![Platforms](https://img.shields.io/badge/Plugins-Windows-lightgray.svg)
![.NET](https://img.shields.io/badge/.NET%20Framework-4.7-blue.svg)
[![Revit](https://img.shields.io/badge/Revit-2019-lightblue.svg)](http://developer.autodesk.com/)

![Advanced](https://img.shields.io/badge/Level-Advanced-red.svg)
[![License](http://img.shields.io/:license-MIT-blue.svg)](http://opensource.org/licenses/MIT)

# Description

This sample is based on Learn Forge [Design Automation Sample](http://learnforge.autodesk.io/#/tutorials/modifymodels), the workflow is pretty similar, please make sure to go through that sample first, or you are already familiar with that. 

This sample includes 2 Revit plugin projects, `CountIt` and `DeleteElement`, which are migrated from the `CountIt` & `DeleteWalls` from [Autodesk Forge official document](https://forge.autodesk.com/en/docs/design-automation/v3/tutorials/revit-samples/), I just improved a little to make the 2 plugin project can Count/Delete elements(Walls, Floors, Doors, Windows) based on the input json file. 

The sample also integrates the UI from [learn.forge.viewmodels](https://github.com/Autodesk-Forge/learn.forge.viewmodels) to get the input revit file from the bucket, and also put the result file back to the same bucket. The result files are different, a .txt file for `CountIt`, and an modified .rvt file for `DeleteElement`.

To use the sample, the workflow should be:
1. Build the solution to create 2 AppBundle under `wwwroot/bundles`;
2. Create/Update AppBundle & Activity in Configure dialog;
3. Create a bucket and upload a Revit project file;
4. Select a Revit project file, translate it to view;
5. Select different element you want to Count/Delete, select activity, click `Start workitem` to post the workitem.
6. The result file(.txt or .rvt) will be put in the same bucket, you can translate and view it, or you can download the file to check out.


## Thumbnail

![thumbnail](thumbnail.png)


## Live version

Work in progress.

# Setup

## Prerequisites

1. **Forge Account**: Learn how to create a Forge Account, activate subscription and create an app at [this tutorial](http://learnforge.autodesk.io/#/account/). 
2. **Visual Studio**: Either Community (Windows) or Code (Windows, MacOS).
3. **.NET Core** basic knowledge with C#
4. **ngrok**: Routing tool, [download here](https://ngrok.com/)
7. **Revit** 2019: required to compile changes into the plugin

## Running locally

Clone this project or download it. It's recommended to install [GitHub desktop](https://desktop.github.com/). To clone it via command line, use the following (**Terminal** on MacOSX/Linux, **Git Shell** on Windows):

    git clone https://github.com/johnonsoftware/design.automation-csharp-revit.count.delete


**Visual Studio** (Windows):

Right-click on the project, then go to **Debug**. Adjust the settings as shown below. 

![](visual_studio_settings.png) 

**ngrok**

Run `ngrok http 3000 -host-header="localhost:3000"` to create a tunnel to your local machine, then copy the address into the `FORGE_WEBHOOK_URL` environment variable.

**Environment variables**

At the `.vscode\launch.json`, find the env vars and add your Forge Client ID, Secret and callback URL. Also define the `ASPNETCORE_URLS` variable. The end result should be as shown below:

```json
"env": {
    "ASPNETCORE_ENVIRONMENT": "Development",
    "ASPNETCORE_URLS" : "http://localhost:3000",
    "FORGE_CLIENT_ID": "your id here",
    "FORGE_CLIENT_SECRET": "your secret here",
    "FORGE_WEBHOOK_URL": "your ngrok address here: e.g. http://abcd1234.ngrok.io",
    "FORGE_DESIGN_AUTOMATION_NICKNAME": "your design automation nickname if you already set: e.g. revitiomycompanyname"
},
```

**Revit plugin**

A compiled version of the `Revit` plugin (.bundles) is included on the `web` module, under `wwwroot/bundles` folder. Any changes on these plugins will require to create a new .bundle, the **Post-build** event should create it. Please review the readme for [CountItApp](https://github.com/Autodesk-Forge/design.automation-csharp-revit.count.delete/tree/master/CountItApp) & [DeleteElementsApp](https://github.com/Autodesk-Forge/design.automation-csharp-revit.count.delete/tree/master/DeleteElementsApp)

Start the app.

Open `http://localhost:3000` to start the app, follow the workflow I mentioned before.

## Deployment

To deploy this application to Heroku, the **Callback URL** for Forge must use your `.herokuapp.com` address. After clicking on the button below, at the Heroku Create New App page, set your Client ID, Secret, NickName and Callback URL for Forge.

[![Deploy](https://www.herokucdn.com/deploy/button.svg)](https://heroku.com/deploy)


# Further Reading

Documentation:


- [Data Management API](https://developer.autodesk.com/en/docs/data/v2/overview/)
- [Model Derivative API](https://developer.autodesk.com/en/docs/data/v2/overview/)
- [Webhook](https://forge.autodesk.com/en/docs/webhooks/v1)
- [Design Automation](https://forge.autodesk.com/en/docs/design-automation/v3/developers_guide/overview/)

Desktop APIs:

- [Revit](https://knowledge.autodesk.com/support/revit-products/learn-explore/caas/simplecontent/content/my-first-revit-plug-overview.html)


### Known Issues

- Sometimes you may fail to delete some Revit element depends on the Revit project file, will check that later. 
- Upload file to bucket is still under migration.

### Tips & Tricks

This sample uses .NET Core and should work fine on both Windows and MacOS, did not verify on MacOS yet, but you can see [this tutorial for MacOS](https://github.com/augustogoncalves/dotnetcoreheroku) if you want to try.

### Troubleshooting


## License

This sample is licensed under the terms of the [MIT License](http://opensource.org/licenses/MIT). Please see the [LICENSE](LICENSE) file for full details.

## Written by

Zhong Wu, [Forge Partner Development](http://forge.autodesk.com)
