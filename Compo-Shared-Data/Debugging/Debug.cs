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
                Console.WriteLine($"[{DateTime.Now}][LOG] " + Message, Console.ForegroundColor);
            }
            catch
            {
                Console.WriteLine($"[{DateTime.Now}][LOG] " + Message);
            }
        }

        public static void LogWarning(object DataObject)
        {
            string Message = (DataObject != null) ? DataObject.ToString() : "NULL";

            try
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine($"[{DateTime.Now}][WARNING] " + Message, Console.ForegroundColor);
            }
            catch
            {
                Console.WriteLine($"[{DateTime.Now}][WARNING] " + Message);
            }
        }

        public static void LogError(object DataObject)
        {
            string Message = (DataObject != null) ? DataObject.ToString() : "NULL";

            try
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"[{DateTime.Now}][ERROR] " + Message, Console.ForegroundColor);
            }
            catch
            {
                Console.WriteLine($"[{DateTime.Now}][ERROR] " + Message);
            }
        }
    }
}
