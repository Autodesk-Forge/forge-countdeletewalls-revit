using System;
using System.IO;

using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.DB;
using DesignAutomationFramework;

namespace DeleteWalls
{
   [Autodesk.Revit.Attributes.Regeneration(Autodesk.Revit.Attributes.RegenerationOption.Manual)]
   [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
   public class DeleteWallsApp : IExternalDBApplication
   {
      public ExternalDBApplicationResult OnStartup(Autodesk.Revit.ApplicationServices.ControlledApplication app)
      {
         DesignAutomationBridge.DesignAutomationReadyEvent += HandleDesignAutomationReadyEvent;
         return ExternalDBApplicationResult.Succeeded;
      }

      public ExternalDBApplicationResult OnShutdown(Autodesk.Revit.ApplicationServices.ControlledApplication app)
      {
         return ExternalDBApplicationResult.Succeeded;
      }

      public void HandleDesignAutomationReadyEvent(object sender, DesignAutomationReadyEventArgs e)
      {
         e.Succeeded = true;
         DeleteAllWalls(e.DesignAutomationData);
      }

      public static void DeleteAllWalls(DesignAutomationData data)
      {
         if (data == null) throw new ArgumentNullException(nameof(data));

         Application rvtApp = data.RevitApp;
         if (rvtApp == null) throw new InvalidDataException(nameof(rvtApp));

         string modelPath = data.FilePath;
         if (String.IsNullOrWhiteSpace(modelPath)) throw new InvalidDataException(nameof(modelPath));

         Document doc = data.RevitDoc;
         if (doc == null) throw new InvalidOperationException("Could not open document.");

         using (Transaction transaction = new Transaction(doc))
         {
            FilteredElementCollector col = new FilteredElementCollector(doc).OfClass(typeof(Wall));
            transaction.Start("Delete All Walls");
            doc.Delete(col.ToElementIds());
            transaction.Commit();
         }

         ModelPath path = ModelPathUtils.ConvertUserVisiblePathToModelPath("result.rvt");
         doc.SaveAs(path, new SaveAsOptions());
      }
   }
}
