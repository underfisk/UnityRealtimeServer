using System;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace UNetwork
{
    class PacketBuilder : SerializationBinder
    {
        /// <summary>
        /// Serializes an object to byte array
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static byte[] Serialize(object obj)
        {
            if (obj == null)
                return null;
            using (MemoryStream ms = new MemoryStream())
            {
                var bf = new BinaryFormatter();
                bf.Serialize(ms, obj);
                return ms.ToArray();
            }
        }

        /// <summary>
        /// Deserializes an object from byte to array
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static object Deserialize(byte[] data)
        {
            //invalid data
            if (data.Length <= 0) return null;

            using (MemoryStream ms = new MemoryStream())
            {
                var bf = new BinaryFormatter();
               

                ms.Write(data, 0, data.Length);
                ms.Seek(0, SeekOrigin.Begin);
                return (object)bf.Deserialize(ms);
            }
        }

        /// <summary>
        /// Workaround to fix the cross assembly problem
        /// </summary>
        /// <param name="assemblyName"></param>
        /// <param name="typeName"></param>
        /// <returns></returns>
        public override Type BindToType(string assemblyName, string typeName)
        {
            Console.WriteLine("I'm being called at ASM Name: " + assemblyName);
            Type tyType = null;
            string sShortAssemblyName = assemblyName.Split(',')[0];

            Assembly[] ayAssemblies = AppDomain.CurrentDomain.GetAssemblies();

            foreach (Assembly ayAssembly in ayAssemblies)
            {
                if (sShortAssemblyName == ayAssembly.FullName.Split(',')[0])
                {
                    tyType = ayAssembly.GetType(typeName);
                    break;
                }
            }
            return tyType;
        }
    }
}
