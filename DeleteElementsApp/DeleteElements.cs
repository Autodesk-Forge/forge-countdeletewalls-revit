using System;
using System.IO;
using System.Collections.Generic;

using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.DB;
using DesignAutomationFramework;

namespace DeleteElements
{
    /// <summary>
    /// Delete elements depends on the input parameters
    /// </summary>
   [Autodesk.Revit.Attributes.Regeneration(Autodesk.Revit.Attributes.RegenerationOption.Manual)]
   [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
   public class DeleteElementsApp : IExternalDBApplication
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
         DeleteAllElements(e.DesignAutomationData);
      }

        /// <summary>
        /// delete elements depends on the input params.json file
        /// </summary>
        /// <param name="data"></param>
        public static void DeleteAllElements(DesignAutomationData data)
      {
         if (data == null) throw new ArgumentNullException(nameof(data));

         Application rvtApp = data.RevitApp;
         if (rvtApp == null) throw new InvalidDataException(nameof(rvtApp));

         string modelPath = data.FilePath;
         if (String.IsNullOrWhiteSpace(modelPath)) throw new InvalidDataException(nameof(modelPath));

         Document doc = data.RevitDoc;
         if (doc == null) throw new InvalidOperationException("Could not open document.");


        // For CountIt workItem: If RvtParameters is null, count all types
        DeleteElementsParams deleteElementsParams = DeleteElementsParams.Parse("params.json");

        using (Transaction transaction = new Transaction(doc))
         {
                transaction.Start("Delete Elements");
                if (deleteElementsParams.walls)
                {
                    FilteredElementCollector col = new FilteredElementCollector(doc).OfClass(typeof(Wall));
                    doc.Delete(col.ToElementIds());

                }
                if (deleteElementsParams.floors)
                {
                    FilteredElementCollector col = new FilteredElementCollector(doc).OfClass(typeof(Floor));
                    doc.Delete(col.ToElementIds());
                }
                if (deleteElementsParams.doors)
                {
                    FilteredElementCollector collector = new FilteredElementCollector(doc);
                    ICollection<ElementId> collection = collector.OfClass(typeof(FamilyInstance))
                                                       .OfCategory(BuiltInCategory.OST_Doors)
                                                       .ToElementIds();
                    doc.Delete(collection);
                }
                if (deleteElementsParams.windows)
                {
                    FilteredElementCollector collector = new FilteredElementCollector(doc);
                    ICollection<ElementId> collection = collector.OfClass(typeof(FamilyInstance))
                                                       .OfCategory(BuiltInCategory.OST_Windows)
                                                       .ToElementIds();
                    doc.Delete(collection);
                }
                transaction.Commit();
         }

         ModelPath path = ModelPathUtils.ConvertUserVisiblePathToModelPath("result.rvt");
         doc.SaveAs(path, new SaveAsOptions());
      }
   }
}
