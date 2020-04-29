using System;
using System.Collections.Generic;
using System.Text;

namespace Compo_Shared_Data.Debugging
{
    public class Debug
    {
        public static void Log(object DataObject, ConsoleColor color = ConsoleColor.White)
        {
            Console.ForegroundColor = color;
            Console.WriteLine("[LOG] " + Convert.ToString(DataObject), Console.ForegroundColor);
        }

        public static void LogWarning(object DataObject)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("[WARNING] " + Convert.ToString(DataObject), Console.ForegroundColor);
        }

        public static void LogError(object DataObject)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("[ERROR] " + Convert.ToString(DataObject), Console.ForegroundColor);
        }
    }
}
