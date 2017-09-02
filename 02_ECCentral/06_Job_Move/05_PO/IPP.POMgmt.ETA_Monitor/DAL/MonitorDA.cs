using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using IPPOversea.Invoicemgmt.DAL;
using IPPOversea.POmgmt.Model;
using Newegg.Oversea.Framework.DataAccess;
using IPPOversea.POmgmt.ETA.Model;
using System.Data;


namespace IPPOversea.Invoicemgmt.DAL
{
    class MonitorDA
    {
        public static List<POEimsEntity> GetPOEimsRelevanceInfo(int poSysNo)
        {
            DataCommand command = DataCommandManager.GetDataCommand("GetPOEimsRelevanceInfo");
            command.SetParameterValue("@POSysNo", poSysNo);
            command.SetParameterValue("@CompanyCode", Settings.CompanyCode);
            return command.ExecuteEntityList<POEimsEntity>();
        }

        public static List<POEntity> GetPO(string companyCode)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("GetPO");

            cmd.SetParameterValue("@Companycode",companyCode);

            return cmd.ExecuteEntityList<POEntity>();
        }

        public static FinancePayItemEntity GetFinacePayItemByPOSysNo(int poSysNo)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("GetFinacePayItemByPOSysNo");

            cmd.SetParameterValue("@OrderSysNo", poSysNo);

            return cmd.ExecuteEntity<FinancePayItemEntity>();
        }

        public static List<POEntity> AbandonPO(int SysNo, string companyCode)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("AbandonPO");

            cmd.SetParameterValue("@SysNo", SysNo);

            cmd.SetParameterValue("@Companycode", companyCode);

            return cmd.ExecuteEntityList<POEntity>();            
        }

        public static void AbandonETA(int poSysNo)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("AbandonETA");
            cmd.SetParameterValue("@SysNo", poSysNo);

            cmd.ExecuteNonQuery();            
        }

        public static void UpdateExtendPOInfo(int sysNO, string companyCode, int productSysNo)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("UpdateExtendPOInfo");
            cmd.SetParameterValue("@SysNo", sysNO);
            cmd.SetParameterValue("@Companycode", companyCode);
            cmd.SetParameterValue("@ProductSysNo", productSysNo);

            cmd.ExecuteNonQuery();
        }

        public static List<POItem> GetPOItemBySys(int poSys, string companyCode)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("GetPOItemBySys");
            cmd.SetParameterValue("@SysNo", poSys);
            cmd.SetParameterValue("@Companycode", companyCode);

            return cmd.ExecuteEntityList<POItem>();            
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

        public static void InsertFinancePayAndItem(FinancePayItemEntity fpItem, int userSysNo)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("InsertFinancePayAndItem");

            cmd.SetParameterValue("@OrderSysNo", fpItem.OrderSysNo);
            cmd.SetParameterValue("@PrePayItemSysNo", fpItem.SysNo);
            cmd.SetParameterValue("@CreateUserSysNo", userSysNo);
            cmd.SetParameterValue("@PayAmt", -1 * Math.Abs(fpItem.AvailableAmt));
            cmd.SetParameterValue("@CompanyCode", fpItem.CompanyCode);
            cmd.SetParameterValue("@LanguageCode", fpItem.LanguageCode);
            cmd.SetParameterValue("@StoreCompanyCode", fpItem.StoreCompanyCode);

            cmd.ExecuteNonQuery();
        }
    }
}
