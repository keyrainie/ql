using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Linq;
using Nesoft.ECWeb.MobileService.Core;

namespace Nesoft.ECWeb.MobileService.Models.Version
{
    public class VersionManager
    {
        public static VersionInfoModel CheckVersion(string versionCode)
        {
            ClientType clientType = HeaderHelper.GetClientType();

            return CheckVersionUpdate(versionCode, clientType);
        }

        #region 私有方法

        private static VersionInfoModel CheckVersionUpdate(string versionCode, ClientType clientType)
        {
            List<VersionInfoModel> list = GetVersionInfoList(clientType);
            if (list != null && list.Count > 0)
            {
                VersionInfoModel versinoInfo = list[list.Count() - 1];//最新版本信息
                if (list.Exists(item => item.Code == versionCode))
                {
                    switch (CompareVersionCode(versionCode, versinoInfo.Code))
                    {   case 0:
                            versinoInfo.IsUpdate = false;
                            break;
                        case 1://服务端版本大于客户端版本
                            versinoInfo.IsUpdate = true;
                            break;
                        case 2://相等
                            {
                                versinoInfo.IsUpdate = false;
                                versinoInfo.IsForcedUpdate = false;
                            }break;
                        default:
                            break;
                    }
                }
                else
                {
                    versinoInfo.IsForcedUpdate = true;
                    versinoInfo.IsUpdate = true;
                }

                return versinoInfo;
            }

            return null;
        }

        /// <summary>
        /// 比较客户端版本和服务端版本
        /// </summary>
        /// <param name="clientVersionCode"></param>
        /// <param name="serveVersionCode"></param>
        /// <returns></returns>
        private static int CompareVersionCode(string clientVersionCode, string serveVersionCode)
        {
            if (!string.IsNullOrEmpty(clientVersionCode) && !string.IsNullOrEmpty(serveVersionCode))
            {
                string clientVersion = clientVersionCode.Trim().Replace(".", "");
                string serveVersion = serveVersionCode.Trim().Replace(".", "");
                int serveValue = ConvertStringToInt(serveVersionCode.Trim().Replace(".", ""));
                int clientValue = ConvertStringToInt(clientVersionCode.Trim().Replace(".", ""));
                if (serveValue ==clientValue)
                {
                    return 2;
                }
                else if (serveValue > clientValue)
                {
                    return 1;
                }
            }
            return 0;
        }

        private static List<VersionInfoModel> GetVersionInfoList(ClientType clientType)
        {
            XDocument doc = XDocument.Load(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Configuration/Version.config"));
            if (null != doc)
            {
                XElement root = doc.Root;
                IEnumerable<XElement> versionCollection = root.Elements("versionCollection");
                if (versionCollection != null && versionCollection.Count() > 0)
                {
                    foreach (XElement item in versionCollection)
                    {
                        int type = 0;
                        int.TryParse(item.Attribute("clientType").Value, out type);
                        if (type == (int)clientType)
                        {
                            string downloadPath = item.Attribute("downloadPath").Value;
                            List<VersionInfoModel> versionInfoList = new List<VersionInfoModel>();
                            IEnumerable<XElement> versionList = item.Elements("version");
                            if (versionList != null && versionList.Count() > 0)
                            {
                                foreach (XElement versionXElement in versionList)
                                {
                                    VersionInfoModel version = new VersionInfoModel();
                                    version.Code = GetElementValue(versionXElement, "code");
                                    string forcedUpdate = GetElementValue(versionXElement, "forcedUpdate");
                                    if (forcedUpdate == "true")
                                    {
                                        version.IsForcedUpdate = true;
                                    }
                                    else
                                    {
                                        version.IsForcedUpdate = false;
                                    }
                                    version.Description = GetElementValue(versionXElement, "description");
                                    version.DownloadPath = downloadPath;

                                    versionInfoList.Add(version);
                                }
                            }

                            return versionInfoList;
                        }
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// 获取子节点值
        /// </summary>
        /// <param name="parentElement"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        private static string GetElementValue(XElement parentElement, string key)
        {
            if (parentElement != null && !string.IsNullOrEmpty(key))
            {
                XElement element = parentElement.Element(key.Trim());
                if (element != null)
                    return element.Value;
            }

            return string.Empty;
        }

        private static int ConvertStringToInt(string value)
        {
            int v = 0;
            int.TryParse(value, out v);
            return v;
        }

        #endregion
    }
}