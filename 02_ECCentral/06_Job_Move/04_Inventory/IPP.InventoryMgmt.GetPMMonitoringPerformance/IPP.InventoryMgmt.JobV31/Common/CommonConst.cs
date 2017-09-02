using System;
using System.Configuration;

namespace IPP.InventoryMgmt.JobV31.Common
{
    public class CommonConst
    {
        /// <summary>
        /// 企业编码
        /// </summary>
        public string CompanyCode
        {
            get
            {
                string companyCode = ConfigurationManager.AppSettings["CompanyCode"];
                if (string.IsNullOrEmpty(companyCode))
                {
#if DEBUG
                    //测试数据
                    companyCode = "8601";
#else
                    //正式数据
                    throw new Exception("未能检测到 CompanyCode 的相关数据，请检测配置节点 Appsettings -> CompanyCode 是否配置正确。");
#endif
                }

                return companyCode;
            }
        }

        public string StoreCompanyCode
        {
            get
            {
                string _StoreCompanyCode = ConfigurationManager.AppSettings["StoreCompanyCode"];
                if (string.IsNullOrEmpty(_StoreCompanyCode))
                {
#if DEBUG
                    //测试数据
                    _StoreCompanyCode = "8601";
#else
                    //正式数据
                    throw new Exception("未能检测到 StoreCompanyCode 的相关数据，请检测配置节点 Appsettings -> StoreCompanyCode 是否配置正确。");
#endif
                }

                return _StoreCompanyCode;
            }
        }

        public string UserDisplayName
        {
            get
            {
                string temp = ConfigurationManager.AppSettings["UserDisplayName"];
                if (string.IsNullOrEmpty(temp))
                {
#if DEBUG
                    //测试数据
                    temp = "IPPSystemAdmin";
#else
                    //正式数据
                    throw new Exception("未能检测到 UserDisplayName 的相关数据，请检测配置节点 Appsettings -> UserDisplayName 是否配置正确。");
#endif
                }

                return temp;
            }
        }

        public string UserLoginName
        {
            get
            {
                string temp = ConfigurationManager.AppSettings["UserLoginName"];
                if (string.IsNullOrEmpty(temp))
                {
#if DEBUG
                    //测试数据
                    temp = "IPPSystemAdmin";
#else
                    //正式数据
                    throw new Exception("未能检测到 UserLoginName 的相关数据，请检测配置节点 Appsettings -> UserLoginName 是否配置正确。");
#endif
                }

                return temp;
            }
        }

        public string StoreSourceDirectoryKey
        {
            get
            {
                string temp = ConfigurationManager.AppSettings["StoreSourceDirectoryKey"];
                if (string.IsNullOrEmpty(temp))
                {
#if DEBUG
                    //测试数据
                    temp = "bitkoo";
#else
                    //正式数据
                    throw new Exception("未能检测到 StoreSourceDirectoryKey 的相关数据，请检测配置节点 Appsettings -> StoreSourceDirectoryKey 是否配置正确。");
#endif
                }

                return temp;
            }
        }
    }
}
