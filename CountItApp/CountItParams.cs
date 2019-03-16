/////////////////////////////////////////////////////////////////////
// Copyright (c) Autodesk, Inc. All rights reserved
// Written by Forge Partner Development
//
// Permission to use, copy, modify, and distribute this software in
// object code form for any purpose and without fee is hereby granted,
// provided that the above copyright notice appears in all copies and
// that both that copyright notice and the limited warranty and
// restricted rights notice below appear in all supporting
// documentation.
//
// AUTODESK PROVIDES THIS PROGRAM "AS IS" AND WITH ALL FAULTS.
// AUTODESK SPECIFICALLY DISCLAIMS ANY IMPLIED WARRANTY OF
// MERCHANTABILITY OR FITNESS FOR A PARTICULAR USE.  AUTODESK, INC.
// DOES NOT WARRANT THAT THE OPERATION OF THE PROGRAM WILL BE
// UNINTERRUPTED OR ERROR FREE.
/////////////////////////////////////////////////////////////////////
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
