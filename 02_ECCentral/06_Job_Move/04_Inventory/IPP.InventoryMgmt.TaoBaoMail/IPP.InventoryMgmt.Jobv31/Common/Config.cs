using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.Text.RegularExpressions;

namespace IPP.InventoryMgmt.JobV31.Common
{
    public class Config
    {
        /// <summary>
        /// 第三方合作的标识
        /// </summary>
        public static string PartnerType
        {
            get
            {
                string partner = ConfigurationManager.AppSettings["PartnerType"];

                if (string.IsNullOrEmpty(partner))
                {
#if DEBUG
                    //测试数据
                    partner = "T";
#else
                    //正式数据
                    //throw new Exception("未能检测到 PartnerType 的相关数据，请检查配置节点 Appsettings -> PartnerType 是否配置正确。");
#endif
                }
                return partner;
            }
        }

        /// <summary>
        /// 第三方支持的newegg仓库编号
        /// </summary>
        public static int[] WareHourseNumbers
        {
            get
            {
                string warehoursenumbers = ConfigurationManager.AppSettings["WareHourseNumbers"];
                if (string.IsNullOrEmpty(warehoursenumbers))
                {
#if DEBUG
                    //测试数据
                    warehoursenumbers = "51";//上海仓
#else
                    //正式数据
                    throw new Exception("未能检测到 WareHourseNumbers 的相关数据，请检测配置节点 Appsettings -> WareHourseNumbers 是否配置正确。");
#endif
                }
                if (warehoursenumbers.IndexOf(',') > -1)
                {
                    Regex reg = new Regex(",+");
                    warehoursenumbers = reg.Replace(warehoursenumbers, ",");
                    reg = new Regex("^,|,$");
                    warehoursenumbers = reg.Replace(warehoursenumbers, "");
                }
                string[] arr = warehoursenumbers.Split(',');

                return Util.Converts<int, string>(arr);
            }
        }
        /// <summary>
        /// 第三方的仓库编号
        /// </summary>
        public static int ThirdPartWareHourseNumber
        {
            get
            {
                string warehoursenumbers = ConfigurationManager.AppSettings["ThirdPartWareHourseNumber"];
                if (string.IsNullOrEmpty(warehoursenumbers))
                {
#if DEBUG
                    //测试数据
                    warehoursenumbers = "51";//上海仓
#else
                    //正式数据
                    throw new Exception("未能检测到 ThirdPartWareHourseNumber 的相关数据，请检测配置节点 Appsettings -> ThirdPartWareHourseNumber 是否配置正确。");
#endif
                }

                return int.Parse(warehoursenumbers);
            }
        }
        /// <summary>
        /// 第三方的仓库名称
        /// </summary>
        public static string ThirdPartWareHourseName
        {
            get
            {
                string warehoursenumbers = ConfigurationManager.AppSettings["ThirdPartWareHourseName"];
                if (string.IsNullOrEmpty(warehoursenumbers))
                {
#if DEBUG
                    //测试数据
                    warehoursenumbers = "淘宝仓库";//上海仓
#else
                    //正式数据
                    throw new Exception("未能检测到 ThirdPartWareHourseName 的相关数据，请检测配置节点 Appsettings -> ThirdPartWareHourseName 是否配置正确。");
#endif
                }

                return warehoursenumbers;
            }
        }

        public static int InventoryAlarmQty
        {
            get
            {
                string temp = ConfigurationManager.AppSettings["InventoryAlarmQty"];
                int result = 0;
                if (int.TryParse(temp, out result))
                {
                    return result;
                }
                else
                {
#if DEBUG
                    //测试数据
                    result = 10;
#else
                    //正式数据
                    throw new Exception("请检测配置节点 Appsettings -> InventoryAlarmQty 是否配置正确。");
#endif
                }
                return result;
            }
        }

        /// <summary>
        /// 企业编码
        /// </summary>
        public static string CompanyCode
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

        public static string StoreCompanyCode
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

        public static string UserDisplayName
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

        public static string UserLoginName
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

        /// <summary>
        /// 分批次同步库存的数据量
        /// </summary>
        public static int BatchNumber
        {
            get
            {
                string temp = ConfigurationManager.AppSettings["BatchNumber"];
                if (string.IsNullOrEmpty(temp))
                {
#if DEBUG
                    //测试数据
                    temp = "50";
#else
                    //正式数据
                    throw new Exception("未能检测到 BatchNumber 的相关数据，请检测配置节点 Appsettings -> BatchNumber 是否配置正确。");
#endif
                }

                return int.Parse(temp);
            }
        }

        public static string ICalculateInventoryQtyAssembly
        {
            get
            {
                string temp = ConfigurationManager.AppSettings["ICalculateInventoryQtyAssembly"];
                if (string.IsNullOrEmpty(temp))
                {
#if DEBUG
                    //测试数据
                    temp = "IPP.InventoryMgmt.JobV31.CalculateInventoryQty, IPP.InventoryMgmt.JobV31";
#else
                    //正式数据
                    throw new Exception("未能检测到 ICalculateInventoryQtyAssembly 的相关数据，请检测配置节点 Appsettings -> ICalculateInventoryQtyAssembly 是否配置正确。");
#endif
                }

                return temp;
            }
        }

        /// <summary>
        /// 第三方的同步方式
        /// </summary>
        public static SynType ThirdPartSynType
        {
            get
            {
                string temp = ConfigurationManager.AppSettings["SynType"];
                if (string.IsNullOrEmpty(temp))
                    return SynType.Default;
                try
                {
                    return (SynType)Enum.Parse(typeof(SynType), temp);
                }
                catch (ArgumentException)
                {
                    return SynType.Default;
                }
            }
        }

        public static string TaoBaoAppKey
        {
            get
            {
                string temp = ConfigurationManager.AppSettings["TaoBaoAppKey"];
                if (string.IsNullOrEmpty(temp))
                {
#if DEBUG
                    //测试数据
                    temp = "12375619";
#else
                    //正式数据
                    throw new Exception("未能检测到 TaoBaoAppKey 的相关数据，请检测配置节点 Appsettings -> TaoBaoAppKey 是否配置正确。");
#endif
                }

                return temp;
            }
        }

        public static string TaoBaoAppSecret
        {
            get
            {
                string temp = ConfigurationManager.AppSettings["TaoBaoAppSecret"];
                if (string.IsNullOrEmpty(temp))
                {
#if DEBUG
                    //测试数据
                    temp = "f7b3b59e282361296aac0b0aeb929fae";
#else
                    //正式数据
                    throw new Exception("未能检测到 TaoBaoAppSecret 的相关数据，请检测配置节点 Appsettings -> TaoBaoAppSecret 是否配置正确。");
#endif
                }

                return temp;
            }
        }

        public static string TaoBaoSession
        {
            get
            {
                string temp = ConfigurationManager.AppSettings["TaoBaoSession"];
                if (string.IsNullOrEmpty(temp))
                {
#if DEBUG
                    //测试数据
                    temp = "610081971b91a8d0aeb737ad3961662efcc87ccd58fa798770648912";
#else
                    //正式数据
                    throw new Exception("未能检测到 TaoBaoSession 的相关数据，请检测配置节点 Appsettings -> TaoBaoSession 是否配置正确。");
#endif
                }

                return temp;
            }
        }

        public static string TaoBaoURL
        {
            get
            {
                string temp = ConfigurationManager.AppSettings["TaoBaoURL"];
                if (string.IsNullOrEmpty(temp))
                {
#if DEBUG
                    //测试数据
                    temp = "http://gw.api.taobao.com/router/rest";
#else
                    //正式数据
                    throw new Exception("未能检测到 TaoBaoURL 的相关数据，请检测配置节点 Appsettings -> TaoBaoURL 是否配置正确。");
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
                    throw new Exception("未能检测到 MailCCAddress 的相关数据，请检测配置节点 Appsettings -> MailCCAddress 是否配置正确。");
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
                    temp = "Kilin.H.Zhang@newegg.com;Ray.L.Xing@newegg.com;Tank.W.Wei@newegg.com;Acro.F.Chao@newegg.com";
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
                    temp = "support@newegg.com";
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
                    temp = "淘宝库存异常检测_测试邮件，请忽略";
#else
                    //正式数据
                    throw new Exception("未能检测到 MailSubject 的相关数据，请检测配置节点 Appsettings -> MailSubject 是否配置正确。");
#endif
                }

                return temp;
            }
        }


        public static int MailContentPageSize
        {
            get
            {
                return 150;
            }
        }
    }
}
