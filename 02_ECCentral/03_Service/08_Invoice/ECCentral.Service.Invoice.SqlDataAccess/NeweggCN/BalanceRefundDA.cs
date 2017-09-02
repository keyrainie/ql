using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.Invoice.Refund;
using ECCentral.Service.Invoice.IDataAccess;
using ECCentral.Service.Utility;
using ECCentral.Service.Utility.DataAccess;
using ECCentral.BizEntity.Customer;
using System.Data;

namespace ECCentral.Service.Invoice.SqlDataAccess
{
    [VersionExport(typeof(IBalanceRefundDA))]
    public class BalanceRefundDA : IBalanceRefundDA
    {
        #region IBalanceRefundDA Members

        public BalanceRefundInfo Create(BalanceRefundInfo entity)
        {
            DataCommand dataCommand = DataCommandManager.GetDataCommand("InsertBalanceRefund");
            dataCommand.SetParameterValue(entity);
            entity.SysNo = Convert.ToInt32(dataCommand.ExecuteScalar());

            return Load(entity.SysNo.Value);
        }

        public BalanceRefundInfo Load(int sysNo)
        {
            DataCommand dataCommand = DataCommandManager.GetDataCommand("GetBalanceRefundBySysNo");
            dataCommand.SetParameterValue("@SysNo", sysNo);
            return dataCommand.ExecuteEntity<BalanceRefundInfo>();
        }

        public void Update(BalanceRefundInfo entity)
        {
            DataCommand dataCommand = DataCommandManager.GetDataCommand("UpdateBalanceRefund");
            dataCommand.SetParameterValue(entity);
            dataCommand.ExecuteNonQuery();
        }

        public void UpdateStatusForFinConfirm(int sysNo, int finAuditUserSysNo, BizEntity.Invoice.BalanceRefundStatus status)
        {
            DataCommand dataCommand = DataCommandManager.GetDataCommand("UpdateBalanceRefundStatusForFinConfirm");
            dataCommand.SetParameterValue("@SysNo", sysNo);
            dataCommand.SetParameterValue("@AuditUserSysNo", finAuditUserSysNo);
            dataCommand.SetParameterValue("@Status", status);

            dataCommand.ExecuteNonQuery();
        }

        public void UpdateStatusForCSConfirm(int sysNo, int csAuditUserSysNo, BizEntity.Invoice.BalanceRefundStatus status)
        {
            DataCommand dataCommand = DataCommandManager.GetDataCommand("UpdateBalanceRefundStatusForCSConfirm");
            dataCommand.SetParameterValue("@SysNo", sysNo);
            dataCommand.SetParameterValue("@CSAuditUserSysNo", csAuditUserSysNo);
            dataCommand.SetParameterValue("@Status", status);

            dataCommand.ExecuteNonQuery();
        }

        public void UpdateStatus(int sysNo, BizEntity.Invoice.BalanceRefundStatus status)
        {
            DataCommand dataCommand = DataCommandManager.GetDataCommand("UpdateBalanceRefundStatus");
            dataCommand.SetParameterValue("@SysNo", sysNo);
            dataCommand.SetParameterValue("@Status", status);

            dataCommand.ExecuteNonQuery();
        }

        public void SetReferenceID(int sysNo, string referenceID)
        {
            DataCommand dataCommand = DataCommandManager.GetDataCommand("SetBalanceRefundReferenceID");
            dataCommand.SetParameterValue("@SysNo", sysNo);
            dataCommand.SetParameterValue("@ReferenceID", referenceID);

            dataCommand.ExecuteNonQuery();
        }

        public int UpdatePointExpiringDate(int sysNo, DateTime expiredDate)
        {
            DataCommand dataCommand = DataCommandManager.GetDataCommand("Invoice.UpdatePointExpiringDate");
            dataCommand.SetParameterValue("@SysNo", sysNo);
            dataCommand.SetParameterValue("@DateTime", expiredDate);

            return dataCommand.ExecuteNonQuery();
        }

        /// <summary>
        /// 调整顾客积分
        /// </summary>
        /// <param name="adujstInfo"></param>
        /// <returns></returns>
        public virtual object Adjust(AdjustPointRequest adujstInfo)
        {
            CustomDataCommand cmd = DataCommandManager.CreateCustomDataCommandFromConfig("Invoice.AdjustPoint");
            cmd.AddInputParameter("@CustomerSysno", DbType.Int32, adujstInfo.CustomerSysNo);
            cmd.AddInputParameter("@Point", DbType.Int32, adujstInfo.Point);
            cmd.AddInputParameter("@PointType", DbType.Int32, adujstInfo.PointType);
            cmd.AddInputParameter("@Source", DbType.String, adujstInfo.Source);
            cmd.AddInputParameter("@Memo", DbType.String, adujstInfo.Memo);
            cmd.SetParameterValueAsCurrentUserSysNo("@InUser");
            cmd.AddInputParameter("@OperationType", DbType.Int32, adujstInfo.OperationType);
            cmd.AddInputParameter("@SoSysNo", DbType.Int32, adujstInfo.SOSysNo);
            cmd.AddInputParameter("@ExpireDate", DbType.DateTime, adujstInfo.PointExpiringDate);
            cmd.AddOutParameter("@returnCode", DbType.Int32, 0);
            cmd.CommandTimeout = 120;
            cmd.ExecuteNonQuery();
            object obj = cmd.GetParameterValue("@returnCode");
            return obj;
        }

        /// <summary>
        /// 调整顾客积分预检查
        /// </summary>
        /// <param name="adujstInfo"></param>
        /// <returns></returns>
        public virtual object AdjustPointPreCheck(AdjustPointRequest adujstInfo)
        {
            CustomDataCommand cmd = DataCommandManager.CreateCustomDataCommandFromConfig("Invoice.AdjustPointPreCheck");
            cmd.AddInputParameter("@CustomerSysno", DbType.Int32, adujstInfo.CustomerSysNo);
            cmd.AddInputParameter("@Point", DbType.Int32, adujstInfo.Point);
            cmd.AddInputParameter("@PointType", DbType.Int32, adujstInfo.PointType);
            cmd.AddInputParameter("@Source", DbType.String, adujstInfo.Source);
            cmd.AddInputParameter("@Memo", DbType.String, adujstInfo.Memo);
            cmd.SetParameterValueAsCurrentUserSysNo("@InUser");
            cmd.AddInputParameter("@OperationType", DbType.Int32, adujstInfo.OperationType);
            cmd.AddInputParameter("@SoSysNo", DbType.Int32, adujstInfo.SOSysNo);
            cmd.AddInputParameter("@ExpireDate", DbType.DateTime, adujstInfo.PointExpiringDate);
            cmd.AddOutParameter("@returnCode", DbType.Int32, 0);
            cmd.CommandTimeout = 120;
            cmd.ExecuteNonQuery();
            object obj = cmd.GetParameterValue("@returnCode");
            return obj;
        }

        public virtual object SplitSOPointLog(int customerSysNo, BizEntity.SO.SOBaseInfo master, List<BizEntity.SO.SOBaseInfo> subSoList)
        {
            object o = null;
            //先给obtain表添加原来母单消费的积分  //然后重新记录子订单消费的积分信息
            CustomDataCommand cmd = DataCommandManager.CreateCustomDataCommandFromConfig("Invoice.AddNewPointForSplitSO");
            cmd.AddInputParameter("@CustomerSysno", DbType.Int32, customerSysNo);
            cmd.AddInputParameter("@Point", DbType.Int32, master.PointPay);
            cmd.AddInputParameter("@ObtainType", DbType.Int32, AdjustPointType.UpdateSO);
            cmd.AddInputParameter("@Memo", DbType.String, ResouceManager.GetMessageString("Customer.CustomerPointsAddRequest", "SplitSOPointLogMemo_Obtain"));
            cmd.SetParameterValueAsCurrentUserSysNo("@InUser");
            cmd.AddInputParameter("@SoSysNo", DbType.Int32, master.SysNo);
            cmd.AddInputParameter("@CompanyCode", DbType.AnsiStringFixedLength, master.CompanyCode);
            cmd.AddOutParameter("@returnCode", DbType.Int32, 0);
            cmd.CommandTimeout = 120;
            cmd.ExecuteNonQuery();
            object obj = cmd.GetParameterValue("@returnCode");
            if (Convert.ToString(obj) != "1000099")
            {
                return obj;
            }
            else
            {
                //然后重新记录子订单消费的积分信息           

                foreach (BizEntity.SO.SOBaseInfo subentity in subSoList)
                {
                    CustomDataCommand cmdSub = DataCommandManager.CreateCustomDataCommandFromConfig("Invoice.ConsumePointForSplitSO");
                    cmdSub.AddInputParameter("@CustomerSysno", DbType.Int32, master.CustomerSysNo);
                    cmdSub.AddInputParameter("@Point", DbType.Int32, subentity.PointPay);
                    cmdSub.AddInputParameter("@ConsumeType", DbType.Int32, AdjustPointType.CreateOrder);
                    cmdSub.AddInputParameter("@Memo", DbType.String, ResouceManager.GetMessageString("Customer.CustomerPointsAddRequest", "SplitSOPointLogMemo_Consume"));
                    cmdSub.SetParameterValueAsCurrentUserSysNo("@InUser");
                    cmdSub.AddInputParameter("@SoSysNo", DbType.Int32, subentity.SysNo);
                    cmdSub.AddInputParameter("@CompanyCode", DbType.AnsiStringFixedLength, master.CompanyCode);
                    cmdSub.AddOutParameter("@returnCode", DbType.Int32, 0);
                    cmdSub.CommandTimeout = 120;
                    cmdSub.ExecuteNonQuery();
                    o = cmdSub.GetParameterValue("@returnCode");
                }
                return o;
            }
        }

        public virtual object CancelSplitSOPointLog(int customerSysNo, BizEntity.SO.SOBaseInfo master, List<BizEntity.SO.SOBaseInfo> subSoList)
        {
            object o = null;
            //先给obtain表添加原来子单消费的积分之和
            //在 重新记录母订单消费的积分信息
            CustomDataCommand cmd = DataCommandManager.CreateCustomDataCommandFromConfig("Invoice.AddNewPointForSplitSO");


            cmd.AddInputParameter("@CustomerSysno", DbType.Int32, master.CustomerSysNo);
            cmd.AddInputParameter("@Point", DbType.Int32, master.PointPay);
            cmd.AddInputParameter("@ObtainType", DbType.Int32, AdjustPointType.CreateOrder);
            cmd.AddInputParameter("@Memo", DbType.String, ResouceManager.GetMessageString("Customer.CustomerPointsAddRequest", "CancelSplitSOPointLog_Obtain"));
            cmd.SetParameterValueAsCurrentUserSysNo("@InUser");
            cmd.AddInputParameter("@SoSysNo", DbType.Int32, master.SysNo);
            cmd.AddInputParameter("@CompanyCode", DbType.AnsiStringFixedLength, master.CompanyCode);
            cmd.AddOutParameter("@returnCode", DbType.Int32, 0);
            cmd.CommandTimeout = 120;
            cmd.ExecuteNonQuery();
            object obj = cmd.GetParameterValue("@returnCode");
            if (Convert.ToString(obj) != "1000099")
            {
                return obj;
            }
            else
            {
                //然后重新记录母订单消费的积分信息
                CustomDataCommand cmdSub = DataCommandManager.CreateCustomDataCommandFromConfig("Invoice.ConsumePointForSplitSO");

                cmdSub.AddInputParameter("@CustomerSysno", DbType.Int32, master.CustomerSysNo);
                cmdSub.AddInputParameter("@Point", DbType.Int32, master.PointPay);
                cmdSub.AddInputParameter("@ConsumeType", DbType.Int32, 3);
                cmdSub.AddInputParameter("@Memo", DbType.String, ResouceManager.GetMessageString("Customer.CustomerPointsAddRequest", "CancelSplitSOPointLog_Consume"));
                cmdSub.SetParameterValueAsCurrentUserSysNo("@InUser");
                cmdSub.AddInputParameter("@SoSysNo", DbType.Int32, master.SysNo);
                cmdSub.AddInputParameter("@CompanyCode", DbType.AnsiStringFixedLength, master.CompanyCode);
                cmdSub.AddOutParameter("@returnCode", DbType.Int32, 0);
                cmdSub.CommandTimeout = 120;
                cmdSub.ExecuteNonQuery();
                o = cmdSub.GetParameterValue("@returnCode");
                return o;
            }
        }

        #endregion IBalanceRefundDA Members
    }
}