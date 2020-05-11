using System;
using System.Collections.Generic;
using System.Text;

namespace Compo_Shared_Data.Debugging
{
    public class Debug
    {
        public static void Log(object DataObject, ConsoleColor color = ConsoleColor.White)
        {
            string Message = (DataObject != null) ? DataObject.ToString() : "NULL";

            try
            {
                Console.ForegroundColor = color;
                Console.WriteLine("[LOG] " + Message, Console.ForegroundColor);
            }
            catch
            {
                Console.WriteLine("[LOG] " + Message);
            }
        }

        public static void LogWarning(object DataObject)
        {
            string Message = (DataObject != null) ? DataObject.ToString() : "NULL";

            try
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("[WARNING] " + Message, Console.ForegroundColor);
            }
            catch
            {
                Console.WriteLine("[WARNING] " + Message);
            }
        }

        public static void LogError(object DataObject)
        {
            string Message = (DataObject != null) ? DataObject.ToString() : "NULL";

            try
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("[ERROR] " + Message, Console.ForegroundColor);
            }
            catch
            {
                Console.WriteLine("[ERROR] " + Message);
            }
        }
    }
}
