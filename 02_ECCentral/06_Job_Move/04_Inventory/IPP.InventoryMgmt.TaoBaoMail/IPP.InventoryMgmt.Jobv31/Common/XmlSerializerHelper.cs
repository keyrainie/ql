using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.IO;
using System.Xml;

namespace IPP.InventoryMgmt.JobV31.Common
{
    public class XmlSerializerHelper
    {
        public static T Deserializer<T>(string response, Encoding encoding)
        {
            XmlSerializer ser = new XmlSerializer(typeof(T));
            object obj = null;
            byte[] buffer = encoding.GetBytes(response);
            using (MemoryStream stream = new MemoryStream(buffer))
            {
                obj = ser.Deserialize(stream);
            }
            return (T)obj;
        }
    }
}
