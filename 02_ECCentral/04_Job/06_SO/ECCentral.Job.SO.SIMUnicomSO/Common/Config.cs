using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace ECCentral.Job.SO.SIMUnicomSO.Common
{
  
       public class Config
       {
           public static string CompanyCode
           {
               get
               {
                   string companyCode = ConfigurationManager.AppSettings["CompanyCode"];
                   if (string.IsNullOrEmpty(companyCode))
                   {

                     //测试数据
                       companyCode = "8601";

                    //正式数据
                    throw new Exception("未能检测到 CompanyCode 的相关数据，请检测配置节点 Appsettings -> CompanyCode 是否配置正确。");

                   }

                   return companyCode;
               }
           }

           public static string StoreCompanyCode
           {
               get
               {
                   string _StoreCompanyCode = ConfigurationManager.AppSettings["StoreCompanyCode"];
                   if (string.IsNullOrEmpty(_StoreCompanyCode))
                   {

                       //测试数据
                       _StoreCompanyCode = "8601";

                    //正式数据
                    throw new Exception("未能检测到 StoreCompanyCode 的相关数据，请检测配置节点 Appsettings -> StoreCompanyCode 是否配置正确。");

                   }

                   return _StoreCompanyCode;
               }
           }

           public static string UserDisplayName
           {
               get
               {
                   string temp = ConfigurationManager.AppSettings["UserDisplayName"];
                   if (string.IsNullOrEmpty(temp))
                   {

                       //测试数据
                       temp = "IPPSystemAdmin";

                    //正式数据
                    throw new Exception("未能检测到 UserDisplayName 的相关数据，请检测配置节点 Appsettings -> UserDisplayName 是否配置正确。");

                   }

                   return temp;
               }
           }

           public static string UserLoginName
           {
               get
               {
                   string temp = ConfigurationManager.AppSettings["UserLoginName"];
                   if (string.IsNullOrEmpty(temp))
                   {

                       //测试数据
                       temp = "IPPSystemAdmin";

                    //正式数据
                    throw new Exception("未能检测到 UserLoginName 的相关数据，请检测配置节点 Appsettings -> UserLoginName 是否配置正确。");

                   }

                   return temp;
               }
           }

           public static string StoreSourceDirectoryKey
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



           public static string MailCCAddress
           {
               get
               {
                   string temp = ConfigurationManager.AppSettings["MailCCAddress"];
                   if (string.IsNullOrEmpty(temp))
                   {
#if DEBUG
                       //测试数据
                       temp = "";
#else
                    //正式数据
                    //throw new Exception("未能检测到 MailCCAddress 的相关数据，请检测配置节点 Appsettings -> MailCCAddress 是否配置正确。");
#endif
                   }

                   return temp;
               }
           }

           public static string MailAddress
           {
               get
               {
                   string temp = ConfigurationManager.AppSettings["MailAddress"];
                   if (string.IsNullOrEmpty(temp))
                   {
#if DEBUG
                       //测试数据
                       temp = "Ray.L.Xing@newegg.com;";
#else
                    //正式数据
                    throw new Exception("未能检测到 MailAddress 的相关数据，请检测配置节点 Appsettings -> MailAddress 是否配置正确。");
#endif
                   }

                   return temp;
               }
           }

           public static string MailFrom
           {
               get
               {
                   string temp = ConfigurationManager.AppSettings["MailFrom"];
                   if (string.IsNullOrEmpty(temp))
                   {
#if DEBUG
                       //测试数据
                       temp = "ipp@newegg.com";
#else
                    //正式数据
                    throw new Exception("未能检测到 MailFrom 的相关数据，请检测配置节点 Appsettings -> MailFrom 是否配置正确。");
#endif
                   }

                   return temp;
               }
           }

           public static string MailSubject
           {
               get
               {
                   string temp = ConfigurationManager.AppSettings["MailSubject"];
                   if (string.IsNullOrEmpty(temp))
                   {
#if DEBUG
                       //测试数据
                       temp = "联通合约机SIM卡激活提醒_订单号{0}_测试邮件，请忽略";
#else
                    //正式数据
                    throw new Exception("未能检测到 MailSubject 的相关数据，请检测配置节点 Appsettings -> MailSubject 是否配置正确。");
#endif
                   }

                   return temp;
               }
           }

           public static string SOMaintainURL
           {
               get
               {
                   string temp = ConfigurationManager.AppSettings["SOMaintainURL"];
                   if (string.IsNullOrEmpty(temp))
                   {
                       #if DEBUG
                       //测试数据
                       temp = "http://ssoversea02/IPPOversea/Portal/OrderMgmt/Pages/SO/SOMaintain.aspx?SOSysNo={0}";
                        #else
                    //正式数据
                    throw new Exception("未能检测到 SOMaintainURL 的相关数据，请检测配置节点 Appsettings -> SOMaintainURL 是否配置正确。");
#endif
                   }

                   return temp;
               }
           }

           public static SynType SendMailSynType
           {
               get
               {
                   string temp = ConfigurationManager.AppSettings["SynType"];
                   switch (temp)
                   {
                       case "Queue":
                           return SynType.Queue;
                       case "Async":
                       default:
                           return SynType.Async;
                   }
               }
           }

       }
}

