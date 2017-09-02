using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Specialized;
using System.Xml;
using System.Configuration;

namespace Nesoft.Utility.DataAccess.RealTime
{
    public class RealTimeSection : IConfigurationSectionHandler
    {
        private static RealTimeSetting s_PersisterSetting = null;
        private static bool s_HasInit = false;
        private static object s_SyncObj = new object();
        public static RealTimeSetting GetSetting()
        {
            if (s_HasInit == false)
            {
                lock (s_SyncObj)
                {
                    if (s_HasInit == false)
                    {                       
                        s_PersisterSetting = ConfigurationManager.GetSection("realTime") as RealTimeSetting;
                        s_HasInit = true;
                    }
                }
            }
            return s_PersisterSetting;
        }

        public object Create(object parent, object configContext, XmlNode section)
        {
            RealTimeSetting setting = new RealTimeSetting();
            if (section != null)
            {
                string tmp = GetNodeAttribute(section, "default");
                setting.DefaultPersisteName = (tmp != null && tmp.Length > 0) ? tmp : null;
                XmlNode[] nodeList = GetChildrenNodes(section, "item");
                foreach (XmlNode node in nodeList)
                {
                    string name = GetNodeAttribute(node, "name");
                    if (name == null || name.Length <= 0)
                    {
                        throw new ConfigurationErrorsException("The attribute 'name' of the xml node 'persister/item' cannot be empty in config file.");
                    }
                    if (setting.ContainsKey(name))
                    {
                        throw new ConfigurationErrorsException("Duplicated name '" + name + "' of the xml node 'persister/item' in config file.");
                    }
                    string type = GetNodeAttribute(node, "type");
                    if (type == null || type.Length <= 0)
                    {
                        throw new ConfigurationErrorsException("The attribute 'type' of the xml node 'persister/item' cannot be empty in config file.");
                    }
                    NameValueCollection parms = new NameValueCollection();
                    XmlNode[] paramList = GetChildrenNodes(node.SelectSingleNode("parameters"), "add");
                    foreach (XmlNode pa in paramList)
                    {
                        parms.Add(GetNodeAttribute(pa, "key"), GetNodeAttribute(pa, "value"));
                    }
                    setting.Add(name, new PersisteItemConfig
                    {
                        Name = name,
                        Type = type,
                        Parameters = parms
                    });
                }
            }
            return setting;
        }

        private XmlNode[] GetChildrenNodes(XmlNode node, string nodeName)
        {
            if (node == null || node.ChildNodes == null || node.ChildNodes.Count <= 0)
            {
                return new XmlNode[0];
            }
            List<XmlNode> nodeList = new List<XmlNode>(node.ChildNodes.Count);
            foreach (XmlNode child in node.ChildNodes)
            {
                if (child.Name == nodeName)
                {
                    nodeList.Add(child);
                }
            }
            return nodeList.ToArray();
        }

        private string GetNodeAttribute(XmlNode node, string attributeName)
        {
            if (node.Attributes == null
                        || node.Attributes[attributeName] == null
                        || node.Attributes[attributeName].Value == null
                        || node.Attributes[attributeName].Value.Trim() == string.Empty)
            {
                return string.Empty;
            }
            return node.Attributes[attributeName].Value.Trim();
        }
    }

    public class RealTimeSetting : Dictionary<string, PersisteItemConfig>
    {
        public string DefaultPersisteName
        {
            get;
            set;
        }
    }

    public class PersisteItemConfig
    {
        public string Name
        {
            get;
            set;
        }

        public string Type
        {
            get;
            set;
        }

        public NameValueCollection Parameters
        {
            get;
            set;
        }
    }  
}
