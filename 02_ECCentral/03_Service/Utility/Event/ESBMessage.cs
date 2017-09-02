using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECCentral.Service.Utility
{
    public class ESBMessage
    {
        public string MessageID { get; set; }

        public string Subject { get; set; }

        public string SubscriberID { get; set; }

        public string Content { get; set; }

        public T GetData<T>(ESBMessageSerializerType serializerType = ESBMessageSerializerType.Xml)
        {
            if (Content == null)
            {
                return default(T);
            }
            if (serializerType == ESBMessageSerializerType.BinaryBase64)
            {
                return SerializationUtility.BinaryDeserialize<T>(Content);
            }
            if (serializerType == ESBMessageSerializerType.Json)
            {
                return SerializationUtility.JsonDeserialize<T>(Content);
            }
            return SerializationUtility.XmlDeserialize<T>(Content);
        }
    }

    public enum ESBMessageSerializerType
    {
        Xml = 0,
        Json = 1,
        BinaryBase64 = 2
    }
}
