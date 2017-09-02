using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Data;
using AutoClose.Model;
using Newegg.Oversea.Framework.DataAccess;

namespace AutoClose.DAL
{
    public enum POItemCheckStatus
    {
        /// <summary>
        /// 未检查
        /// </summary>
        UnCheck = -1,

        /// <summary>
        ///已检查(符合)
        /// </summary>
        Accordant = 0,

        /// <summary>
        /// 已检查(不符合)
        /// </summary>
        UnAccordant = 1
    }

    public class AutoCloseDA
    {
        public static List<POEimsEntity> GetPOEimsRelevanceInfo(int poSysNo)
        {
            DataCommand command = DataCommandManager.GetDataCommand("GetPOEimsRelevanceInfo");
            command.SetParameterValue("@POSysNo", poSysNo);
            command.SetParameterValue("@CompanyCode", Settings.CompanyCode);
            return command.ExecuteEntityList<POEimsEntity>();
        }
        //获得需要关闭的POSynsno
        public static List<PoSysNoItem> GetNeedClesePoSysno()
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("GetNeedClosePoSysNo");

            return cmd.ExecuteEntityList<PoSysNoItem>();
        }

        public static bool SendEmail(int SOSysNo, string MailAddress, string MailSubject, string MailBody)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("InsertToSendEmail");

            cmd.SetParameterValue("@MailAddress", MailAddress);
            cmd.SetParameterValue("@MailSubject", MailSubject);
            cmd.SetParameterValue("@MailBody", MailBody);
            cmd.SetParameterValue("@LanguageCode", Settings.LanguageCode);
            cmd.SetParameterValue("@CompanyCode", Settings.CompanyCode);
            cmd.SetParameterValue("@StoreCompanyCode", Settings.StoreCompanyCode);

            return cmd.ExecuteNonQuery() > 0;
        }

        //得到要发送邮件的PO
        public static List<AutoClose.Model.POMailInfo> GetNeedSendMailPo(int PoSysNo)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("GetNeedSendEmailPO");
            cmd.SetParameterValue("@PoSysNo", PoSysNo);

            return cmd.ExecuteEntityList<POMailInfo>();
        }

        //修伽使用返点
        public static bool UpdateUseReturnPoint(AutoClose.Model.POEmisInfo poeimsInfo)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("SetUsePointData");
            cmd.SetParameterValue("@SysNo", poeimsInfo.SysNo);
            cmd.SetParameterValue("@UsingReturnPoint", poeimsInfo.sumEimsCount);
            cmd.SetParameterValue("@CompanyCode", Settings.CompanyCode);

            return cmd.ExecuteNonQuery() > 0;
        }

        //入库总金额=该PO各批次入库总金额
        public static bool SetCountToamt(int PoSysNo)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("SetPoMasterToAmt");
            cmd.SetParameterValue("@PoSysNo", PoSysNo);

            return cmd.ExecuteNonQuery() > 0;
        }

        //获得需要进行扣减采购的集合
        public static List<AutoClose.Model.PoPurQtyInfo> GetPoPurQtyInfoList(int PoSysNo)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("GetPurQtyInfo");
            cmd.SetParameterValue("@PoSysNo", PoSysNo);

            return cmd.ExecuteEntityList<PoPurQtyInfo>();
        }

        //对于最后一次Task打印时间起超过30天未继续到货的“部分入库（status=6）”状态PO，系统则自动关闭该PO，将PO的状态修改为“系统关闭 8”
        public static bool SetStatus6To8(int PoSysNo, string closeUser)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("SetPoStatueIsClose");
            cmd.SetParameterValue("@PoSysNo", PoSysNo);
            cmd.SetParameterValue("@CloseUser", closeUser);
            cmd.SetParameterValue("@CloseTime", DateTime.Now);
            
            return cmd.ExecuteNonQuery() > 0;
        }

        //获得需要调整返点的PO信息
        public static List<AutoClose.Model.POEmisInfo> GetPoEmisInfo(int PoSysNo)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("GetPoMasterEmis");
            cmd.SetParameterValue("@PoSysNo", PoSysNo);

            return cmd.ExecuteEntityList<POEmisInfo>();            
        }

        public static DataTable GetProductAccessoriesByPOSysno(int poSysNo)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("GetProductAccessoriesByPOSysno");
            cmd.SetParameterValue("@POSysNo", poSysNo);
            cmd.SetParameterValue("@CompanyCode", Settings.CompanyCode);

            return cmd.ExecuteDataSet().Tables[0];            
        }

        /// <summary>
        /// ////////////////////////////////
        /// </summary>
        /// <param name="productSysNo"></param>
        /// <returns></returns>
        public static ProductPriceInfo QueryProductPriceInfo(int productSysNo)
        {
            ProductPriceInfo result = new ProductPriceInfo();
            DataCommand cmd = DataCommandManager.GetDataCommand("GetProductPriceById");
            cmd.SetParameterValue("@SysNo", productSysNo);
            cmd.SetParameterValue("@CompanyCode", Settings.CompanyCode);

            using (DataTable drs = cmd.ExecuteDataSet().Tables[0])
            {
                foreach (DataRow dr in drs.Rows)
                {
                    //result.Id = dr["ProductID"].ToString();
                    result.SysNo = (int)dr["SysNo"];
                    if (dr["CurrentPrice"] != null && dr["CurrentPrice"].ToString() != "") result.CurrentPrice = (decimal)dr["CurrentPrice"];
                    if (dr["ItemPoint"] != null && dr["ItemPoint"].ToString() != "") result.Point = Convert.ToDecimal(dr["ItemPoint"]);
                    if (dr["Category3SysNo"] != null && dr["Category3SysNo"].ToString() != "") result.C3SysNo = Convert.ToInt32(dr["Category3SysNo"]);
                    if (dr["UnitCost"] != null && dr["UnitCost"].ToString() != "") result.UnitCost = Convert.ToInt32(dr["UnitCost"]);
                }
            }

            return result;
        }

        private static decimal? CalculateProductRate(POItem item)
        {
            decimal rate = 0m;
            ProductPriceInfo price = QueryProductPriceInfo(item.ProductSysNo);
            if ((price != null) && (price.CurrentPrice - price.Point * 0.10m) != 0)
            {
                rate = (price.CurrentPrice.Value - price.Point.Value * 0.10m - item.OrderPrice.Value) / (price.CurrentPrice.Value - price.Point.Value * 0.10m);
            }

            return rate;
        }

        /// <summary>
        /// 判断是否是虚库商品
        /// </summary>
        /// <param name="productSysNo"></param>
        /// <returns></returns>
        private static bool IsVirtualStockProduct(int productSysNo)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("V_INM_Inventory");
            cmd.SetParameterValue("@ItemSysNumber", productSysNo);
            cmd.SetParameterValue("@CompanyCode", Settings.CompanyCode);
            DataTable dt = cmd.ExecuteDataSet().Tables[0];
            if (dt.Rows.Count > 0)
            {
                if (dt.Rows[0]["VirtualQty"] != null && (int)dt.Rows[0]["VirtualQty"] != 0)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// 计算京东价毛利率
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        private static decimal? CalculateJDRate(POItem item)
        {
            decimal rate = 0m;
            if (item.JDPrice.HasValue && Convert.ToDecimal(item.JDPrice) > 0)
            {
                rate = (Convert.ToDecimal(item.JDPrice.Value) - item.OrderPrice.Value) / Convert.ToDecimal(item.JDPrice.Value);
                return rate;
            }

            return null;
        }

        public static List<POItem> QueryPOItemsForPrint(PO entity)
        {
            List<POItem> result = new List<POItem>();
            DataCommand cmd = DataCommandManager.GetDataCommand("GetPOItemsByPOSysNo");
            cmd.SetParameterValue("@POSysNo", entity.SysNo);
            cmd.SetParameterValue("@CompanyCode", Settings.CompanyCode);
            result = cmd.ExecuteEntityList<POItem>();

            foreach (POItem item in result)
            {
                item.LineReturnedPointCost = item.UnitCost * item.Quantity;
                if (item.CheckStatus.HasValue)
                {
                    item.CheckStatusStr = ((POItemCheckStatus)item.CheckStatus).GetDesc();
                }
                else
                {
                    item.CheckStatusStr = POItemCheckStatus.UnCheck.GetDesc();
                }
                item.LineCost = item.OrderPrice.Value * item.Quantity;

                item.CurrencySysmbol = entity.CurrencySymbol;


                item.Tax = CalculateProductRate(item);
                item.JDTax = CalculateJDRate(item);

                if (IsVirtualStockProduct(item.ProductSysNo))
                {
                    item.IsVirtualStockProduct = "虚库商品";
                }

            }

            return result;
        }

        public static List<POItem> QueryPOItemsForPrint(int SysNo)
        {
            List<POItem> result = new List<POItem>();
            DataCommand cmd = DataCommandManager.GetDataCommand("GetPOItemsByPOSysNo");
            cmd.SetParameterValue("@POSysNo", SysNo);
            cmd.SetParameterValue("@CompanyCode", Settings.CompanyCode);
            result = cmd.ExecuteEntityList<POItem>();

            return result;
        }

        #region CRL17821

        /// <summary>
        /// 获得POEntity数据
        /// </summary>
        /// <param name="poSysNo"></param>
        /// <returns></returns>
        public static NewPOEntity GetPOMaster(int poSysNo)
        {
            DataCommand command = DataCommandManager.GetDataCommand("GetPOMaster");
            command.SetParameterValue("@SysNo", poSysNo);
            command.SetParameterValue("@CompanyCode", 8601);

            return command.ExecuteEntity<NewPOEntity>();
        }

        public static int CreatePOSSBLog(POSSBLogEntity log)
        {
            DataCommand command = DataCommandManager.GetDataCommand("CreatePOSSBLog");

            command.SetParameterValue("@POSysNo", log.POSysNo);
            command.SetParameterValue("@Content", log.Content);
            command.SetParameterValue("@ActionType", log.ActionType);
            command.SetParameterValue("@InUser", log.InUser);
            command.SetParameterValue("@ErrMSg", log.ErrMSg);
            command.SetParameterValue("@ErrMSgTime", log.ErrMSgTime);
            command.SetParameterValue("@SendErrMail", log.SendErrMail);
            command.SetParameterValue("@CompanyCode", log.CompanyCode);
            command.SetParameterValue("@LanguageCode", log.LanguageCode);
            command.SetParameterValue("@StoreCompanyCode", log.StoreCompanyCode);

            return command.ExecuteNonQuery();
        }

        public static List<POSSBLogEntity> GetPOSSBLog(int poSysNo, string actionType)
        {
            DataCommand command = DataCommandManager.GetDataCommand("GetPOSSBLog");

            command.SetParameterValue("@POSysNo", poSysNo);
            command.SetParameterValue("@ActionType", actionType);

            return command.ExecuteEntityList<POSSBLogEntity>();
        }

        public static int CallSSBMessageSP(string message)
        {
            DataCommand command = DataCommandManager.GetDataCommand("SendSSBMessage");

            command.SetParameterValue("@RequestMSG", message);

            return command.ExecuteNonQuery();
        }

        public static string GetPOOfflineStatus()
        {
            DataCommand command = DataCommandManager.GetDataCommand("GetPOOfflineStatus");

            command.SetParameterValue("@Key", "POOffLine");

            return command.ExecuteScalar<string>();
        }

        #endregion
    }

    public static class POItemCheckStatusExtension
    {
        public static string GetDesc(this POItemCheckStatus checkStatus)
        {
            switch (checkStatus)
            {
                case POItemCheckStatus.UnCheck:
                    return "未检查";
                case POItemCheckStatus.Accordant:
                    return "已检查（符合）";
                case POItemCheckStatus.UnAccordant:
                    return "已检查（不符合）";
                default:
                    return string.Empty;
            }
        }
    }
}
