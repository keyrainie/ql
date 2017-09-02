using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Web.Script.Serialization;

namespace ECommerce.Facade.Common.RestClient
{
    public interface ISerializer
    {
        string Serialization(object target, Type type);
        object Deserialize(Stream stream, Type type);
    }

    public class JsonSerializer : ISerializer
    {
        #region ISerializer Members

        public string Serialization(object target, Type type)
        {
            //if (target == null)
            //{
            //    return "";
            //}

            //using (MemoryStream stream = new MemoryStream())
            //{
            //    DataContractJsonSerializer serializer = new DataContractJsonSerializer(type);
            //    serializer.WriteObject(stream, target);
            //    byte[] s = stream.ToArray();
            //    return Encoding.UTF8.GetString(s, 0, s.Length);
            //}
            return new JavaScriptSerializer().Serialize(target);
        }


        public object Deserialize(Stream stream, Type type)
        {
            using (StreamReader reader = new StreamReader(stream))
            {
                var value = reader.ReadToEnd();
                //var obj=new JavaScriptSerializer().Deserialize<>()
                return new JavaScriptSerializer().Deserialize(value, type);
            }
            //DataContractJsonSerializer serializer = new DataContractJsonSerializer(type);
            //return serializer.ReadObject(stream);
        }

        #endregion
    }

    public class ObjXmlSerializer : ISerializer
    {
        #region ISerializer Members
        public string Serialization(object target, Type type)
        {
            //if (target == null)
            //{
            //    return "";
            //}
            //using (MemoryStream stream = new MemoryStream())
            //{
            //    DataContractSerializer serializer = new DataContractSerializer(type);
            //    serializer.WriteObject(stream, target);

            //    byte[] s = stream.ToArray();
            //    return Encoding.UTF8.GetString(s, 0, s.Length);
            //}
            throw new NotImplementedException();
        }

        public object Deserialize(Stream stream, Type type)
        {
            //DataContractSerializer serializer = new DataContractSerializer(type);
            //return serializer.ReadObject(stream);
            throw new NotImplementedException();
        }

        #endregion
    }

    internal static class SerializerFactory
    {
        private static readonly Dictionary<string, ISerializer> Items;

        static SerializerFactory()
        {
            Items = new Dictionary<string, ISerializer>();
            Items.Add(ContentTypes.Json, new JsonSerializer());
            Items.Add(ContentTypes.Xml, new ObjXmlSerializer());
        }

        public static ISerializer GetSerializer(string serializerName)
        {
            ISerializer serializer = null;
            if (!string.IsNullOrEmpty(serializerName))
            {
                Items.TryGetValue(serializerName, out serializer);
            }
            return serializer;
        }

        public static void Register(string serializerName, ISerializer serializer)
        {
            Items.Add(serializerName, serializer);
        }
    }
}
