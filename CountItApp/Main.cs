using System.Collections.Generic;
using System.IO;

using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.DB;
using DesignAutomationFramework;
using Newtonsoft.Json;


namespace CountIt
{
    [Autodesk.Revit.Attributes.Regeneration(Autodesk.Revit.Attributes.RegenerationOption.Manual)]
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    public class CountIt : IExternalDBApplication
    {
        public ExternalDBApplicationResult OnStartup(ControlledApplication app)
        {
            DesignAutomationBridge.DesignAutomationReadyEvent += HandleDesignAutomationReadyEvent;
            return ExternalDBApplicationResult.Succeeded;
        }

         public ExternalDBApplicationResult OnShutdown(ControlledApplication app)
         {
            return ExternalDBApplicationResult.Succeeded;
         }

         public void HandleDesignAutomationReadyEvent(object sender, DesignAutomationReadyEventArgs e)
         {  
            e.Succeeded = CountElementsInModel(e.DesignAutomationData.RevitApp, e.DesignAutomationData.FilePath, e.DesignAutomationData.RevitDoc);
      }

        internal static List<Document> GetHostAndLinkDocuments(Document revitDoc)
        {
            List<Document> docList = new List<Document>();
            docList.Add(revitDoc);

            // Find RevitLinkInstance documents
            FilteredElementCollector elemCollector = new FilteredElementCollector(revitDoc);
            elemCollector.OfClass(typeof(RevitLinkInstance));
            foreach (Element curElem in elemCollector)
            {
                RevitLinkInstance revitLinkInstance = curElem as RevitLinkInstance;
                if (null == revitLinkInstance)
                   continue;

                Document curDoc = revitLinkInstance.GetLinkDocument();
                if (null == curDoc) // Link is unloaded.
                   continue;
                
                // When one linked document has more than one RevitLinkInstance in the
                // host document, then 'docList' will contain the linked document multiple times.

                docList.Add(curDoc);
            }

            return docList;
        }

        /// <summary>
        /// Count the element in each file
        /// </summary>
        /// <param name="revitDoc"></param>
        /// <param name="countItParams"></param>
        /// <param name="results"></param>
        internal static void CountElements(Document revitDoc, CountItParams countItParams, ref CountItResults results)
        {
            if (countItParams.walls)
            {
                FilteredElementCollector elemCollector = new FilteredElementCollector(revitDoc);
                elemCollector.OfClass(typeof(Wall));
                int count = elemCollector.ToElementIds().Count;
                results.walls += count;
                results.total += count;
            }

            if (countItParams.floors)
            {
                FilteredElementCollector elemCollector = new FilteredElementCollector(revitDoc);
                elemCollector.OfClass(typeof(Floor));
                int count = elemCollector.ToElementIds().Count;
                results.floors += count;
                results.total += count;
            }

            if (countItParams.doors)
            {
                FilteredElementCollector collector = new FilteredElementCollector(revitDoc);
                ICollection<Element> collection = collector.OfClass(typeof(FamilyInstance))
                                                   .OfCategory(BuiltInCategory.OST_Doors)
                                                   .ToElements();

                int count = collection.Count;
                results.doors += count;
                results.total += count;
            }

            if (countItParams.windows)
            {
                FilteredElementCollector collector = new FilteredElementCollector(revitDoc);
                ICollection<Element> collection = collector.OfClass(typeof(FamilyInstance))
                                                   .OfCategory(BuiltInCategory.OST_Windows)
                                                   .ToElements();

                int count = collection.Count;
                results.windows += count;
                results.total += count;
            }
        }

        /// <summary>
        /// count the elements depends on the input parameter in params.json
        /// </summary>
        /// <param name="rvtApp"></param>
        /// <param name="inputModelPath"></param>
        /// <param name="doc"></param>
        /// <returns></returns>
        public static bool CountElementsInModel(Application rvtApp, string inputModelPath, Document doc)
        {
            if (rvtApp == null)
               return false;

            if (!File.Exists(inputModelPath))
               return false;
            
            if (doc == null)
               return false;
            
            // For CountIt workItem: If RvtParameters is null, count all types
            CountItParams countItParams = CountItParams.Parse("params.json");
            CountItResults results = new CountItResults();

            List<Document> allDocs = GetHostAndLinkDocuments(doc);
            foreach(Document curDoc in allDocs)
            {
               CountElements(curDoc, countItParams, ref results);
            }
            
            using (StreamWriter sw = File.CreateText("result.txt"))
            {
               sw.WriteLine(JsonConvert.SerializeObject(results));
               sw.Close();
            }

            return true;
        }    
    }
}
