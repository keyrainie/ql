using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using ECommerce.Utility;
using System.Xml;

namespace ECommerce.WebFramework.Mail
{
    public class MailConfig
    {
        private const string COOKIE_CONFIG_FILE_PATH = "Configuration/Mail/MailTemplates.config";

        private static string ConfigFilePath
        {
            get
            {
                string path = COOKIE_CONFIG_FILE_PATH;
                string p = Path.GetPathRoot(path);
                if (p == null || p.Trim().Length <= 0) // 说明是相对路径
                {
                    return Path.Combine(AppDomain.CurrentDomain.SetupInformation.ApplicationBase, path);
                }
                return path;
            }
        }

        private static Dictionary<string, MailTemplate> GetAllMailTemplateConfig()
        {
            string path = ConfigFilePath;
            if (string.IsNullOrWhiteSpace(path) || File.Exists(path) == false)
            {
                return new Dictionary<string, MailTemplate>(0);
            }
            return CacheManager.GetWithLocalCache<Dictionary<string, MailTemplate>>("WEB_MailTemplateConfig_GetMailTemplateConfig", () =>
            {
                Dictionary<string, MailTemplate> dic = new Dictionary<string, MailTemplate>();
                XmlDocument doc = new XmlDocument();
                doc.Load(ConfigFilePath);
                XmlNodeList nodeList = doc.GetElementsByTagName("mailTemplate");
                if (nodeList != null && nodeList.Count > 0)
                {
                    foreach (XmlNode xmlNode in nodeList)
                    {
                        if (xmlNode == null)
                        {
                            continue;
                        }
                        MailTemplate entity = new MailTemplate();
                        entity.TemplateID = xmlNode.Attributes["templateID"] != null ? xmlNode.Attributes["templateID"].Value : null;
                        entity.Subject = xmlNode.Attributes["subject"] != null ? xmlNode.Attributes["subject"].Value : null;
                        if (string.IsNullOrWhiteSpace(entity.TemplateID))
                        {
                            throw new ApplicationException("Not set node name for email config in file '" + path + "'");
                        }
                        if (dic.ContainsKey(entity.TemplateID))
                        {
                            throw new ApplicationException("Duplicated email config of node '" + entity.TemplateID + "' in file '" + path + "'");
                        }
                        entity.Body = xmlNode.ChildNodes[0].InnerText;
                        dic.Add(entity.TemplateID, entity);
                    }
                }
                return dic;
            }, path);
        }

        public static MailTemplate BuildMailTemplate(string templateID, KeyValueVariables keyValues, KeyTableVariables keyTables)
        {
            MailTemplate entity = GetMailTemplate(templateID);
            if (entity == null)
            {
                throw new ApplicationException("目标模板[" + templateID + "]未找到!");
            }
            entity.Subject = TemplateString.BuildHtml(entity.Subject, keyValues, keyTables);
            entity.Body = TemplateString.BuildHtml(entity.Body, keyValues, keyTables);

            return entity;
        }

        public static MailTemplate BuildMailTemplate(MailTemplate template, KeyValueVariables keyValues, KeyTableVariables keyTables)
        {
            if (template == null)
            {
                throw new ArgumentException("参数不能为null!", "template");
            }
            template.Subject = TemplateString.BuildHtml(template.Subject, keyValues, keyTables);
            template.Body = TemplateString.BuildHtml(template.Subject, keyValues, keyTables);

            return template;
        }

        public static MailTemplate GetMailTemplate(string templateID)
        {
            Dictionary<string, MailTemplate> all = GetAllMailTemplateConfig();
            MailTemplate entity;
            if (all.TryGetValue(templateID, out entity) && entity != null)
                return entity;
            return null;
        }
    }

    public class MailTemplate
    {
        public string TemplateID { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }

        //private Dictionary<string, string> m_Properties = new Dictionary<string, string>();
        //public Dictionary<string, string> Properties { get { return m_Properties; } }
    }
}
