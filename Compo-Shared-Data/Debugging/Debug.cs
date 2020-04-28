using System;
using System.Collections.Generic;
using System.Text;

namespace Compo_Shared_Data.Debugging
{
    public class Debug
    {
        public static void Log(object DataObject)
        {
            Console.WriteLine("[LOG] " + Convert.ToString(DataObject));
        }

        public static void LogWarning(object DataObject)
        {
            Console.WriteLine("[WARNING] " + Convert.ToString(DataObject));
        }

        public static void LogError(object DataObject)
        {
            Console.WriteLine("[ERROR] " + Convert.ToString(DataObject));
        }
    }
}
