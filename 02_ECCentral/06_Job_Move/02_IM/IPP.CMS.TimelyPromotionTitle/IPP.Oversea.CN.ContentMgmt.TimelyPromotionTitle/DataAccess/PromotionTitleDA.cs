using System.Collections.Generic;
using Newegg.Oversea.Framework.DataAccess;
using Newegg.Oversea.Framework.Utilities;
using System.Data;

namespace IPP.Oversea.CN.ContentMgmt.TimelyPromotionTitle.DataAccess
{
    public static class PromotionTitleDA
    {
        /// <summary>
        /// 从Product_PromotionTitle 表取类型不为Normal, Status为等待,并且当前时间大于等于StartTime并且小于EndTime的数据
        /// </summary>
        /// <returns></returns>
        public static DataTable GetNowActiveSQL()
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("GetNowActiveSQL");
            return cmd.ExecuteDataSet().Tables[0];
        }
        /// <summary>
        /// 从Product_PromotionTitle 表取取类型不为Normal, Status为有效中或者等待,并且当前时间大于等于EndTime的数据
        /// </summary>
        /// <returns></returns>
        public static DataTable GetNowDisableSQL()
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("GetNowDisableSQL");
            return cmd.ExecuteDataSet().Tables[0];
        }
        /// <summary>
        /// 根据ProductSysNo取Normal的PromotionTitle
        /// </summary>
        /// <returns></returns>
        public static string GetNormalPromotionTitle(int ProductSysNo)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("GetNormalPromotionTitle");
            cmd.SetParameterValue("@ProductSysNo", ProductSysNo);
            DataTable dt= cmd.ExecuteDataSet().Tables[0];
            if (dt != null && dt.Rows.Count > 0 && dt.Rows[0]["PromotionTitle"] != null)
                return dt.Rows[0]["PromotionTitle"].ToString();
            else
                return "";
        }
        /// <summary>
        /// 更新Product_PromotionTitle表状态
        /// </summary>
        /// <param name="intSysNo"></param>
        /// <param name="strStatus"></param>
        /// <returns></returns>
        public static int UpdatePromotionTitleStatus(int intSysNo, string strStatus)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("UpdatePromotionTitleStatus");
            cmd.SetParameterValue("@SysNo", intSysNo);
            cmd.SetParameterValue("@Status", strStatus);
            return cmd.ExecuteNonQuery();
        }
        /// <summary>
        /// 更新Product表的ProductName、ProductTitle
        /// </summary>
        /// <param name="intProductSysNo"></param>
        /// <param name="strPromotionTitle"></param>
        /// <returns></returns>
        public static int UpdatePromotionTitleAndPromotionName(int intProductSysNo, string strPromotionTitle)
        {
            string strRedFontPromotionTitle = "";
            if (strPromotionTitle.Trim() != "")
                strRedFontPromotionTitle = "<font color='red'>" + strPromotionTitle + "</font>";
            DataCommand cmd = DataCommandManager.GetDataCommand("UpdatePromotionTitleAndPromotionName");
            cmd.SetParameterValue("@ProductSysNo", intProductSysNo);
            cmd.SetParameterValue("@PromotionTitle", strPromotionTitle);
            cmd.SetParameterValue("@RedFontPromotionTitle", strRedFontPromotionTitle);
            return cmd.ExecuteNonQuery();
        }
    }
}
