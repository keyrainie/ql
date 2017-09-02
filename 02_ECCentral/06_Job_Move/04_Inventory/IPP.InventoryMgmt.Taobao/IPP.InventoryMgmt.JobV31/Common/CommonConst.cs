using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.Text.RegularExpressions;

namespace IPP.InventoryMgmt.JobV31.Common
{
    public class CommonConst
    {
        /// <summary>
        /// 第三方合作的标识
        /// </summary>
        public string PartnerType
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
        public int[] WareHourseNumbers
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
        public int ThirdPartWareHourseNumber
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
        public string ThirdPartWareHourseName
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

        public int InventoryAlarmQty
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

        /// <summary>
        /// 分批次同步库存的数据量
        /// </summary>
        public int BatchNumber
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

        public string ICalculateInventoryQtyAssembly
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
        public SynType ThirdPartSynType
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
    }
}
