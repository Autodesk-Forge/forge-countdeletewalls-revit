
namespace CountIt
{
    /// <summary>
    /// CountItResults is used to save the count result into Json file
    /// </summary>
    internal class CountItResults
   {
      public int walls { get; set; } = 0;
      public int floors { get; set; } = 0;
      public int doors { get; set; } = 0;
      public int windows { get; set; } = 0;
      public int total { get; set; } = 0;
   }
}
