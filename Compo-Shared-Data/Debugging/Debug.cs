using System;
using System.Collections.Generic;
using System.Text;

namespace Compo_Shared_Data.Debugging
{
    public class Debug
    {
        public static void LogError(object DataObject)
        {
            Console.WriteLine("[ERROR] " + Convert.ToString(DataObject));
        }
    }
}
