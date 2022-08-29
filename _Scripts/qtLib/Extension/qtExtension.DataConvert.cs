using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using _Scripts.Helper;

namespace _Scripts.qtLib.Extension
{
    public static partial class qtExtension
    {
        public static bool IsSerializable<T>(this T target)
        {
            return ((target is ISerializable) || (Attribute.IsDefined(typeof (T), typeof (SerializableAttribute))));
        }
        
        public static byte[] ConvertToByteArray(this object target)
        {
            if (!target.IsSerializable())
            {
                qtLogging.LogError($"{target.GetType()} is NOT serializable object, can NOT convert to byte[]!");
                return new byte[] { };
            }
            BinaryFormatter bf = new BinaryFormatter();
            MemoryStream ms = new MemoryStream();
            bf.Serialize(ms, target);

            return ms.ToArray();
        }

        public static T ConvertToObject<T>(this byte[] data)
        {
            MemoryStream memStream = new MemoryStream();
            BinaryFormatter binForm = new BinaryFormatter();
            memStream.Write(data, 0, data.Length);
            memStream.Seek(0, SeekOrigin.Begin);
            T result = (T)binForm.Deserialize(memStream);

            return result;
        }
    }
}