using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;

namespace ECommerce.SOPipeline
{
    internal static class OrderPipelineConfiguration
    {
        #region Private Member

        private const string DEFAULT_CONFIG_FILE_PATH = "Configuration/SOPipeline.config";

        private static readonly string s_ConfigFilePath = GetConfigFilePath();

        private static readonly int s_CacheSlidingExpirationMinutes = GetCacheSlidingExpirationMinutes();

        private static string GetConfigFilePath()
        {
            string path = ConfigurationManager.AppSettings["SOPipelineConfigPath"];
            if (path == null || path.Trim().Length <= 0)
            {
                return Path.Combine(AppDomain.CurrentDomain.SetupInformation.ApplicationBase, DEFAULT_CONFIG_FILE_PATH);
            }
            string p = Path.GetPathRoot(path);
            if (p == null || p.Trim().Length <= 0) // 说明是相对路径
            {
                path = Path.Combine(AppDomain.CurrentDomain.SetupInformation.ApplicationBase, path);
            }
            return path;
        }

        private static int GetCacheSlidingExpirationMinutes()
        {
            const int defautMinutes = 60;
            if (File.Exists(s_ConfigFilePath) == false)
            {
                return defautMinutes;
            }
            XmlDocument x = new XmlDocument();
            x.Load(s_ConfigFilePath);
            XmlNode node = x.SelectSingleNode("//pipelineList");
            if (node == null)
            {
                return defautMinutes;
            }
            string tmp = GetNodeAttribute(node, "cacheSlidingExpirationMinutes");
            int min;
            if (tmp != null && tmp.Trim().Length > 0 && int.TryParse(tmp.Trim(), out min) && min > 0)
            {
                return min;
            }
            return defautMinutes;
        }

        private static List<string> GetTypeNameList(XmlNode root, string nodeName, string itemName)
        {
            XmlNode[] tmpArray = GetChildrenNodes(root, nodeName);
            if (tmpArray == null || tmpArray.Length <= 0)
            {
                return new List<string>(0);
            }
            XmlNode[] list = GetChildrenNodes(tmpArray[0], itemName);
            if (list == null || list.Length <= 0)
            {
                return new List<string>(0);
            }
            List<string> typeNameList = new List<string>(list.Length);
            foreach (var n in list)
            {
                string typeName = GetNodeAttribute(n, "type");
                if (typeName != null && typeName.Trim().Length > 0)
                {
                    typeNameList.Add(typeName);
                }
            }
            return typeNameList;
        }

        private static XmlNode[] GetChildrenNodes(XmlNode node, string nodeName)
        {
            return GetChildrenNodes(node, delegate(XmlNode child)
            {
                return child.Name == nodeName;
            });
        }

        private static XmlNode[] GetChildrenNodes(XmlNode node, Predicate<XmlNode> match)
        {
            if (node == null || node.ChildNodes == null || node.ChildNodes.Count <= 0)
            {
                return new XmlNode[0];
            }
            List<XmlNode> nodeList = new List<XmlNode>(node.ChildNodes.Count);
            foreach (XmlNode child in node.ChildNodes)
            {
                if (match(child))
                {
                    nodeList.Add(child);
                }
            }
            return nodeList.ToArray();
        }

        private static string GetNodeAttribute(XmlNode node, string attributeName)
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

        #endregion

        public static int CacheSlidingExpirationMinutes
        {
            get { return s_CacheSlidingExpirationMinutes; }
        }

        public static OrderPipelineElements GetElements(string pipelineID)
        {
            if (File.Exists(s_ConfigFilePath) == false)
            {
                return null;
            }
            XmlDocument x = new XmlDocument();
            x.Load(s_ConfigFilePath);
            XmlNode node = x.SelectSingleNode("//pipelineList/pipeline[@id='" + pipelineID + "']");
            if (node == null)
            {
                return null;
            }
            OrderPipelineElements els = new OrderPipelineElements();
            string tmp = GetNodeAttribute(node, "breakOnceValidationError");
            els.BreakOnceValidationError = (tmp != null && tmp.Trim().ToUpper() == "TRUE");
            tmp = GetNodeAttribute(node, "transactionWithPersisters");
            els.TransactionWithPersisters = (tmp == null || tmp.Trim().ToUpper() != "FALSE");

            List<string> initializerTypeList = GetTypeNameList(node, "initializers", "item");
            if (initializerTypeList.Count > 0)
            {
                els.Initializers = new List<IInitialize>(initializerTypeList.Count);
                foreach (string typeName in initializerTypeList)
                {
                    els.Initializers.Add((IInitialize)Activator.CreateInstance(Type.GetType(typeName, true)));
                }
            }

            List<string> preValidatorTypeList = GetTypeNameList(node, "preValidators", "item");
            if (preValidatorTypeList.Count > 0)
            {
                els.PreValidators = new List<IValidate>(preValidatorTypeList.Count);
                foreach (string typeName in preValidatorTypeList)
                {
                    els.PreValidators.Add((IValidate)Activator.CreateInstance(Type.GetType(typeName, true)));
                }
            }

            List<string> calculatorTypeList = GetTypeNameList(node, "calculators", "item");
            if (calculatorTypeList.Count > 0)
            {
                els.Calculators = new List<ICalculate>(calculatorTypeList.Count);
                foreach (string typeName in calculatorTypeList)
                {
                    els.Calculators.Add((ICalculate)Activator.CreateInstance(Type.GetType(typeName, true)));
                }
            }

            List<string> postValidatorTypeList = GetTypeNameList(node, "postValidators", "item");
            if (postValidatorTypeList.Count > 0)
            {
                els.PostValidators = new List<IValidate>(postValidatorTypeList.Count);
                foreach (string typeName in postValidatorTypeList)
                {
                    els.PostValidators.Add((IValidate)Activator.CreateInstance(Type.GetType(typeName, true)));
                }
            }

            List<string> persisterTypeList = GetTypeNameList(node, "persisters", "item");
            if (persisterTypeList.Count > 0)
            {
                els.Persisters = new List<IPersist>(persisterTypeList.Count);
                foreach (string typeName in persisterTypeList)
                {
                    els.Persisters.Add((IPersist)Activator.CreateInstance(Type.GetType(typeName, true)));
                }
            }

            return els;
        }
    }
}
