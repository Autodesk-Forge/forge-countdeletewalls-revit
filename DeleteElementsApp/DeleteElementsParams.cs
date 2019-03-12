using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;


namespace DeleteElements
{
    /// <summary>
    /// DeleteElementsParams is used to parse the input json parameters
    /// </summary>
    internal class DeleteElementsParams
    {

        public bool walls { get; set; } = false;
        public bool floors { get; set; } = false;
        public bool doors { get; set; } = false;
        public bool windows { get; set; } = false;

        static public DeleteElementsParams Parse(string jsonPath)
        {
            try
            {
                if (!File.Exists(jsonPath))
                    return new DeleteElementsParams { walls = true, floors = true, doors = true, windows = true };

                string jsonContents = File.ReadAllText(jsonPath);
                return JsonConvert.DeserializeObject<DeleteElementsParams>(jsonContents);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception when parsing json file: " + ex);
                return null;
            }
        }


    }
}
