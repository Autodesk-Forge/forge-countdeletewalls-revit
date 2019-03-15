# CountIt Sample

![Platforms](https://img.shields.io/badge/Plugins-Windows-lightgray.svg)
![.NET](https://img.shields.io/badge/.NET%20Framework-4.7-blue.svg)
[![Revit](https://img.shields.io/badge/Revit-2019-lightblue.svg)](http://developer.autodesk.com/)

![Basic](https://img.shields.io/badge/Level-Basic-blue.svg)

## Description

CountIt is an application that counts walls, floors, doors and windows in a rvt file and its rvt links. It takes a JSON file that specifies which categories of elements will be counted. The output of this application is a text file which contains the element counts.

# Setup

## Prerequisites

1. **Visual Studio** 2017
2. **Revit** 2019 required to compile changes into the plugin
3. **7z zip** requires to create the bundle ZIP, [download here](https://www.7-zip.org/)

## Building CountIt.csproj

Right-click on References, then Add Reference and Browse for **RevitAPI.dll** (by default under `C:\Program Files\Autodesk\Revit 201x\ folder`). Then right-click on this **RevitAPI** reference, go to Properties, then set Copy Local to False.

Then right-click on the project, go to Manage NuGet Packages..., under **Browser** you can search for _DesignAutomation.Revit_ and install `Autodesk.Forge.DesignAutomation.Revit` (choose the appropriate Revit version you need). Then search and install `Newtonsoft.Json` (which is used to parse input data in JSON format).

Build `CountIt.csproj` in `Release` or `Debug` configuration.

## Build

Under **Properties**, at **Build Event** page, the following `Post-build event command line` will copy the DLL into the `\CountIt.bundle/Content/` folder, create a `.ZIP` (using [7z](https://www.7-zip.org/)) and copy to the Webapp folder.

```
xcopy /Y /F "$(TargetDir)*.dll" "$(ProjectDir)CountIt.bundle\Contents\"
del /F "$(ProjectDir)..\WebApp\wwwroot\bundles\CountIt.zip"
"C:\Program Files\7-Zip\7z.exe" a -tzip "$(ProjectDir)../WebApp/wwwroot/bundles/CountIt.zip" "$(ProjectDir)CountIt.bundle\" -xr0!*.pdb
```

# Further Reading

- [My First Revit Plugin](https://knowledge.autodesk.com/support/revit-products/learn-explore/caas/simplecontent/content/my-first-revit-plug-overview.html)
- [Revit Developer Center](https://www.autodesk.com/developer-network/platform-technologies/revit)

## License

This sample is licensed under the terms of the [MIT License](http://opensource.org/licenses/MIT). Please see the [LICENSE](LICENSE) file for full details.

## Written by

Zhong Wu, Forge Partner Development team.
