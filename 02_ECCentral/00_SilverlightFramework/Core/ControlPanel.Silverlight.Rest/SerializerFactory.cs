using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Runtime.Serialization;
using System.Collections.Generic;
using Newegg.Oversea.Silverlight.Rest;
using System.Text;

namespace Newegg.Oversea.Silverlight.ControlPanel.Rest
{
    public interface ISerializer
    {
        string Serialization(object target,Type type);
        object Deserialize(Stream stream,Type type);
    }

    public class JsonSerializer:ISerializer
    {
        #region ISerializer Members

        public string Serialization(object target, Type type)
        {
            if (target == null)
            {
                return "";
            }

            using (MemoryStream stream = new MemoryStream())
            {
                DataContractJsonSerializer serializer = new DataContractJsonSerializer(type);
                serializer.WriteObject(stream, target);
                byte[] s = stream.ToArray();
                return Encoding.UTF8.GetString(s, 0, s.Length);
            }
        }


        public object Deserialize(Stream stream, Type type)
        {
            DataContractJsonSerializer serializer = new DataContractJsonSerializer(type);
            return serializer.ReadObject(stream);
        }

        #endregion
    }

    public class XmlSerializer : ISerializer
    {
        #region ISerializer Members
        public string Serialization(object target, Type type)
        {
            if (target == null)
            {
                return "";
            }
            using (MemoryStream stream = new MemoryStream())
            {
                DataContractSerializer serializer = new DataContractSerializer(type);
                serializer.WriteObject(stream, target);

                byte[] s = stream.ToArray();
                return Encoding.UTF8.GetString(s, 0, s.Length);
            }
        }

        public object Deserialize(Stream stream, Type type)
        {
            DataContractSerializer serializer = new DataContractSerializer(type);
            return serializer.ReadObject(stream);
        }

        #endregion
    }

    internal static class SerializerFactory
    {
        private static readonly Dictionary<string, ISerializer> Items;

        static SerializerFactory()
        {
            Items = new Dictionary<string, ISerializer>();
            Items.Add(ContentTypes.Json,new JsonSerializer());
            Items.Add(ContentTypes.Xml, new XmlSerializer());
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
