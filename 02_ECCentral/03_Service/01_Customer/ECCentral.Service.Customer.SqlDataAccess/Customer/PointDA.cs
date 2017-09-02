using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.Customer.IDataAccess;
using ECCentral.Service.Utility;
using ECCentral.Service.Utility.DataAccess;
using System.Data;
using ECCentral.BizEntity.Customer;
using ECCentral.BizEntity.SO;

namespace ECCentral.Service.Customer.SqlDataAccess
{
    [VersionExport(typeof(IPointDA))]
    public class PointDA : IPointDA
    {
        public virtual object Adjust(AdjustPointRequest adujstInfo)
        {
            CustomDataCommand cmd = DataCommandManager.CreateCustomDataCommandFromConfig("AdjustPoint");
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
            CustomDataCommand cmd = DataCommandManager.CreateCustomDataCommandFromConfig("AddNewPointForSplitSO");
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

                foreach (SOBaseInfo subentity in subSoList)
                {
                    CustomDataCommand cmdSub = DataCommandManager.CreateCustomDataCommandFromConfig("ConsumePointForSplitSO");
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
            CustomDataCommand cmd = DataCommandManager.CreateCustomDataCommandFromConfig("AddNewPointForSplitSO");


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
                CustomDataCommand cmdSub = DataCommandManager.CreateCustomDataCommandFromConfig("ConsumePointForSplitSO");

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
        /// <summary>
        /// 改方法调用的存储过程需要重新实现逻辑
        /// </summary>
        /// <param name="customerSysNo"></param>
        /// <param name="exprireDate"></param>
        public virtual object UpateExpiringDate(int obtainSysNO, DateTime exprireDate)
        {
            DataCommand command = DataCommandManager.GetDataCommand("UpateExpiringDate");
            command.SetParameterValue("@SysNo", obtainSysNO);
            command.SetParameterValue("@ExpireDate", exprireDate);
            return command.ExecuteNonQuery();
        }

        public virtual DataTable QueryRequestItemsBySysNo(CustomerPointsAddRequest info)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("SelectCustomerPointAddRequestItem");
            cmd.SetParameterValue("@PointAddRequestSysNo", info.SysNo);
            cmd.SetParameterValue("@Status", "A");
            return cmd.ExecuteDataTable();
        }

        public virtual int? QueryRequestStatusBySysNo(CustomerPointsAddRequest info)
        {
            int? status = null;
            DataCommand cmd = DataCommandManager.GetDataCommand("SelectCustomerPointAddRequestStatus");
            cmd.SetParameterValue("@SysNo", info.SysNo);
            object returnObject = cmd.ExecuteScalar();
            if (null != returnObject)
            {
                status = Convert.ToInt16(returnObject);
            }
            return status;
        }

        public virtual void ConfirmRequest(CustomerPointsAddRequest requestInfo)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("ConfirmCustomerPointsAddRequest");
            cmd.SetParameterValue<CustomerPointsAddRequest>(requestInfo);
            cmd.ExecuteNonQuery();
        }

        public virtual int CreateRequest(CustomerPointsAddRequest info)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("CreateCustomerPointAddRequest");
            cmd.SetParameterValue<CustomerPointsAddRequest>(info);
            cmd.ExecuteNonQuery();
            int result = Convert.ToInt32(cmd.GetParameterValue("@SysNo"));
            info.SysNo = result;
            return result;
        }

        public virtual void CreateRequestItem(CustomerPointsAddRequestItem item)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("InsertPointAddRequestItem");
            cmd.SetParameterValue<CustomerPointsAddRequestItem>(item);
            cmd.ExecuteNonQuery();
        }


        #region IPointDA Members


        public int GetPriceprotectPoint(int soSysNo, List<int> productSysNoList)
        {
            CustomDataCommand cmd = DataCommandManager.CreateCustomDataCommandFromConfig("GetPriceprotectPoint");
            cmd.CommandText = cmd.CommandText.Replace("#ProductSysNoList", productSysNoList.Join(",").TrimEnd(','));
            cmd.SetParameterValue("@SOSysNo", soSysNo);
            return cmd.ExecuteScalar<int>();
        }
        #endregion
    }
}