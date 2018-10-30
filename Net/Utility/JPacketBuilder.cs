using Newtonsoft.Json;
using System;

namespace Networking.Utility
{
    /// <summary>
    /// JSON Packet Serializator is a utility class to pack/unpack
    /// packets data
    /// </summary>
    public static class JPacketBuilder
    {
        /// <summary>
        /// Accepts only packet objects and serializes it to a string
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string Serialize(Packet packet)
        {
            try
            {
                return JsonConvert.SerializeObject(packet);
            }
            catch(JsonException je)
            {
                Debug.Log(je.ToString());
            }

            return String.Empty;
        }

        /// <summary>
        /// Accepts only objects and serializes to JSON
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string Serialize(object obj)
        {
            try
            {
                return JsonConvert.SerializeObject(obj);
            }
            catch(JsonException je)
            {
                Debug.Log(je.ToString());
            }

            return String.Empty;
        }

        /// <summary>
        /// Deserializes a json data and returns an Packet Object
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="json"></param>
        /// <returns></returns>
        public static Packet Deserialize(string json)
        {
            try
            {
                return JsonConvert.DeserializeObject<Packet>(json);
            }
            catch(JsonException je)
            {
                Debug.Log(je.ToString());
            }

            return default(Packet);
        }

        /// <summary>
        /// Deserializes the data to a given type
        /// Be sure the given type matches the other side
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="json"></param>
        /// <returns></returns>
        public static T Deserialize<T>(string json)
        {
            try
            {
                return JsonConvert.DeserializeObject<T>(json);
            }
            catch(JsonException je)
            {
                Debug.Log(je.ToString());
            }

            return default(T);
        }
    }
}
