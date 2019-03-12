using System;
using System.IO;
using Newtonsoft.Json;

namespace CountIt
{
    /// <summary>
    /// CountItParams is used to parse the input Json parameters
    /// </summary>
    internal class CountItParams
   {
      public bool walls { get; set; } = false;
      public bool floors { get; set; } = false;
      public bool doors { get; set; } = false;
      public bool windows { get; set; } = false;

      static public CountItParams Parse(string jsonPath)
      {
         try
         {
            if (!File.Exists(jsonPath))
               return new CountItParams { walls = true, floors = true, doors = true, windows = true };

            string jsonContents = File.ReadAllText(jsonPath);
            return JsonConvert.DeserializeObject<CountItParams>(jsonContents);
         }
         catch (Exception ex)
         {
            Console.WriteLine("Exception when parsing json file: " + ex);
            return null;
         }
      }
   }
}
