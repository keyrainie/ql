using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.IO;
using System.Xml.Linq;
using System.Configuration;

namespace ECommerce.Utility
{
    public static class CodeNamePairManager
    {
        #region Private

        private const string NODE_NAME = "CodeNamePairsPath";
        private static readonly string s_BaseFolderPath = GetBaseFolderPath();

        private static string GetBaseFolderPath()
        {
            string path = ConfigurationManager.AppSettings[NODE_NAME];
            if (path == null || path.Trim().Length <= 0)
            {
                return Path.Combine(AppDomain.CurrentDomain.SetupInformation.ApplicationBase, "Configuration/CodeNamePairs");
            }
            string p = Path.GetPathRoot(path);
            if (p == null || p.Trim().Length <= 0) // 说明是相对路径
            {
                path = Path.Combine(AppDomain.CurrentDomain.SetupInformation.ApplicationBase, path);
            }
            return path;
        }
        
        private static Dictionary<string, List<CodeNamePair>> LoadConfigFile(string getFullConfigPath, string languageCode)
        {            
            if (!File.Exists(getFullConfigPath))
            {
                return new Dictionary<string, List<CodeNamePair>>(0);
                //throw new FileNotFoundException("Can't find the CodeNamePair config file: " + getFullConfigPath + "!");
            }

            Dictionary<string, List<CodeNamePair>> rst = new Dictionary<string, List<CodeNamePair>>();
            XElement doc = XElement.Load(getFullConfigPath);
            foreach (var x in doc.Descendants("codeNamePair"))
            {
                if (x.Attribute("key") == null || x.Attribute("key").Value == null || x.Attribute("key").Value.Trim().Length <= 0)
                {
                    throw new ApplicationException("There are some 'codeNamePair' node without attribute 'key' in the CodeNamePair config file: " + getFullConfigPath + ", please check it!");
                }
                string key = x.Attribute("key").Value.Trim().ToUpper();
                if (rst.ContainsKey(key))
                {
                    throw new ApplicationException("Duplicated value '" + x.Attribute("key").Value.Trim() + "' of attribute 'key' in 'codeNamePair' node in the CodeNamePair config file: " + getFullConfigPath + ", please check it (ex: ignore case)!");
                }
                List<CodeNamePair> v = new List<CodeNamePair>();
                foreach (var listItem in x.Descendants("item"))
                {
                    if (listItem.Attribute("code") != null && listItem.Attribute("code").Value != null && listItem.Attribute("code").Value.Trim().Length > 0)
                    {
                        string itemKey = listItem.Attribute("code").Value.Trim();
                        string itemText = listItem.Attribute("name") == null ? null : (listItem.Attribute("name").Value == null ? null : listItem.Attribute("name").Value.Trim());
                        v.Add(new CodeNamePair { Code = itemKey, Name = EnvironmentVariableManager.ReplaceVariable(itemText) });
                    }
                }
                rst.Add(key, v);
            }
            return rst;
        }

        /// <summary>
        ///  获取配置的对应语言编码的CodeNamePair列表
        /// </summary>
        /// <param name="domainName">Domain名称: [Customer,IM,Inventory,Invoice,MKT,PO,RMA,SO,Common]</param>
        /// <param name="key">对应到配置文件中xml节点codeNamePair的属性key的值</param>
        /// <param name="languageCode">语言编码</param>
        /// <returns>返回对应key的codeNamePair节点下的所有item的列表，每个item节点对应一个KeyValuePair对象，item节点的code属性的值赋值到KeyValuePair对象的属性Key上，item节点的name属性的值赋值到KeyValuePair对象的属性Value上</returns>
        private static List<CodeNamePair> GetList(string domainName, string key, string languageCode)
        {
            string fileName = domainName.Trim().Trim('.').Replace('.', Path.DirectorySeparatorChar).ToUpper();
            string configPath = Path.Combine(s_BaseFolderPath, fileName + "." + languageCode + ".config");
            string cacheKey = fileName + "." + languageCode.ToUpper().Trim();
            Dictionary<string, List<CodeNamePair>> data = CacheManager.GetWithLocalCache<Dictionary<string, List<CodeNamePair>>>(cacheKey,
                ()=>LoadConfigFile(configPath, languageCode), configPath);
            List<CodeNamePair> list;
            if (!data.TryGetValue(key.ToUpper(), out list) || list == null || list.Count <= 0)
            {
                return new List<CodeNamePair>(0);
            }
            return new List<CodeNamePair>(list);
        }

        #endregion

        /// <summary>
        /// 获取CodeNamePair配置文件所在的根目录（该目录路径默认为应用程序根目录下的Configuration/CodeNamePairs，但可以由web.config或app.config的appsettings/add节点来配置：<add key="CodeNamePairsPath" value="" />）
        /// </summary>
        public static string BaseFolderPath
        {
            get { return s_BaseFolderPath; }
        }

        /// <summary>
        ///  获取配置的CodeNamePair列表（根据当前线程获取LanguageCode）
        /// </summary>
        /// <param name="fileTitle">配置文件的名称（不包含语言和后缀名部分，但需要包含相对于CodeNamePair根目录的路径，使用‘.’分隔目录，不区分大小写）</param>
        /// <param name="key">对应到配置文件中xml节点codeNamePair的属性key的值</param>
        /// <returns>返回对应key的codeNamePair节点下的所有item的列表，每个item节点对应一个KeyValuePair对象，item节点的code属性的值赋值到KeyValuePair对象的属性Key上，item节点的name属性的值赋值到KeyValuePair对象的属性Value上</returns>
        public static List<CodeNamePair> GetList(string fileTitle, string key)
        {
            return GetList(fileTitle, key, Thread.CurrentThread.CurrentCulture.Name);
        }

        /// <summary>
        /// 获取配置的CodeNamePair列表（根据当前线程获取LanguageCode）中指定code的name
        /// </summary>
        /// <param name="fileTitle">配置文件的名称（不包含语言和后缀名部分，但需要包含相对于CodeNamePair根目录的路径，使用‘.’分隔目录，不区分大小写）</param>
        /// <param name="key">对应到配置文件中xml节点codeNamePair的属性key的值</param>
        /// <param name="code">对应key的codeNamePair节点下的item的code属性</param>
        /// <returns>返回对应key的codeNamePair节点下的指定code的item的name属性值</returns>
        public static string GetName(string fileTitle, string key, string code)
        {
            List<CodeNamePair> list = GetList(fileTitle, key, Thread.CurrentThread.CurrentCulture.Name);
            CodeNamePair cnp = list.Find(f => f.Code.Trim().ToLower() == code.Trim().ToLower());
            if (cnp.Code != null)
            {
                return cnp.Name;
            }
            return null;
        }
    }

    /// <summary>
    /// 编码、名称 这类key-vale模式的简单数据类型
    /// </summary>
    public struct CodeNamePair
    {
        public string Code { get; set; }

        public string Name { get; set; }

        public override string ToString()
        {
            return Code;
        }

        public static implicit operator CodeNamePair(string code)
        {
            return new CodeNamePair { Code = code };
        }

        public static implicit operator CodeNamePair(int code)
        {
            return new CodeNamePair { Code = code.ToString() };
        }
    }
}
