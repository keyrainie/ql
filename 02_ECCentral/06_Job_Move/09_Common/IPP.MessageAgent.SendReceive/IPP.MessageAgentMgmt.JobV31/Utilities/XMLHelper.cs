using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.IO;

namespace IPP.MessageAgent.SendReceive.JobV31.Utilities
{
    public class XMLHelper
    {
        public static string SelectSingleNode(string xpath, string xmlMessage)
        {
            XmlDocument xmlDoc = new XmlDocument();
            TextReader reader = new StringReader(xmlMessage);
            xmlDoc.Load(reader);

            XmlNode node = xmlDoc.SelectSingleNode(xpath);
            if (node != null)
            {
                return node.Value;
            }
            return string.Empty;
        }
    }
}
