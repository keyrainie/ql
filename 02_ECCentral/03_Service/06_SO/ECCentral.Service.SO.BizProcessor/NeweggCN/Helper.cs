using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Text;
using ECCentral.BizEntity;
using ECCentral.BizEntity.SO;
using ECCentral.Service.SO.IDataAccess;
using ECCentral.Service.Utility;
using System.IO;

namespace ECCentral.Service.SO.BizProcessor
{
    public static partial class AppSettingHelper
    {
        /// <summary>
        /// 上海自提运送方式编号
        /// </summary>
        public static int NEG_SH_SelfTake_ShipSysNo
        {
            get
            {
                int t = 0;
                if (int.TryParse(AppSettingManager.GetSetting(SOConst.DomainName, "NEG_SH_SelfTake_ShipSysNo"), out t))
                {
                    return t;
                }
                return t;
            }
        } 
        /// <summary>
        /// 不提供增值税发票的仓库
        /// </summary>
        public static List<int> NEG_NotVAT_StockSysNoList
        {
            get
            {
                string noList_Str = AppSettingManager.GetSetting(SOConst.DomainName, "NEG_NotVAT_StockSysNoList");
                string[] no_Str_List = noList_Str == null ? null : noList_Str.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                List<int> noList = new List<int>();
                if (no_Str_List != null)
                {
                    foreach (string n in no_Str_List)
                    {
                        int no;
                        if (int.TryParse(n, out no))
                        {
                            noList.Add(no);
                        }
                    }
                }
                return noList;
            }
        }
    }
}