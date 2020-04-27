using Compo_Shared_Data.Debugging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

namespace Compo_Shared_Data.Network
{
    public class Package
    {
        public static byte[] Packaging(object Data)
        {
            try
            {
                BinaryFormatter Formatter = new BinaryFormatter();
                using (MemoryStream Stream = new MemoryStream())
                {
                    Formatter.Serialize(Stream, Data);
                    return Stream.ToArray();
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"An exception while packing the object data.\nError code:\n" + ex);
            }
            return null;
        }

        public static T Unpacking<T>(byte[] Data)
        {
            try
            {
                BinaryFormatter Formatter = new BinaryFormatter();
                MemoryStream Stream = new MemoryStream(Data);

                Stream.Seek(0, SeekOrigin.Begin);
                object Component = Formatter.Deserialize(Stream);
                Stream.Position = 0;
                Stream.Close();

                return (T)Component;
            }
            catch (Exception ex)
            {
                Debug.LogError("An exception while trying to unpack the object data.\nError code:\n" + ex);
            }
            return default;
        }
    }
}
