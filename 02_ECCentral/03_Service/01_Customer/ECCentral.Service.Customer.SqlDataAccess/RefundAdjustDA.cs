using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.Utility;
using ECCentral.Service.Customer.IDataAccess;
using ECCentral.BizEntity.Customer;
using ECCentral.Service.Utility.DataAccess;
using ECCentral.BizEntity.RMA;
using System.Data;

namespace ECCentral.Service.Customer.SqlDataAccess
{
    [VersionExport(typeof(IRefundAdjustDA))]
    public class RefundAdjustDA : IRefundAdjustDA
    {
        /// <summary>
        /// 创建补偿退款单
        /// </summary>
        /// <param name="entity"></param>
        public virtual void RefundAdjustCreate(RefundAdjustInfo entity)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("Customer_Create_RefundAdjust");
            cmd.SetParameterValue<RefundAdjustInfo>(entity);
            cmd.ExecuteNonQuery();
        }

        /// <summary>
        /// 根据补偿退款单系统编号获取补偿退款单详细信息
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public virtual RefundAdjustInfo GetRefundDetailBySysNo(RefundAdjustInfo entity)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("Customer_Get_RefundDetailBySysNo");
            cmd.SetParameterValue("@SysNo", entity.SysNo);
            cmd.SetParameterValue("@CompanyCode", entity.CompanyCode);
            return cmd.ExecuteEntity<RefundAdjustInfo>();
        }

        /// <summary>
        /// 根据订单编号获取退款单状态
        /// </summary>
        /// <param name="SoSysNo"></param>
        /// <returns></returns>
        public virtual int? GetRequestStatusBySoSysNo(int SoSysNo)
        {
            CustomDataCommand cmd = DataCommandManager.CreateCustomDataCommandFromConfig("Customer_Get_RequestStatusBySoSysNo");
            cmd.SetParameterValue("@SOSysNo", SoSysNo);
            if (cmd.ExecuteScalar() != null)
                return (int)cmd.ExecuteScalar();
            else
                return null;
        }

        /// <summary>
        /// 根据订单号获取补偿退款单相关信息
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public virtual RefundAdjustInfo GetRefundDetailBySoSysNo(RefundAdjustInfo entity)
        {
            CustomDataCommand cmd = DataCommandManager.CreateCustomDataCommandFromConfig("Customer_Get_RequestDetailBySoSysNo");
            cmd.SetParameterValue("@SOSysNo", entity.SOSysNo);
            cmd.SetParameterValue("@CompanyCode", entity.CompanyCode);
            return cmd.ExecuteEntity<RefundAdjustInfo>();
        }

        public RefundAdjustInfo GetCustomerIDBySOSysNo(RefundAdjustInfo entity)
        {
            CustomDataCommand cmd = DataCommandManager.CreateCustomDataCommandFromConfig("Customer_Get_CustomerIDBySoSysNo");
            cmd.SetParameterValue("@SOSysNo", entity.SOSysNo);
            cmd.SetParameterValue("@CompanyCode", entity.CompanyCode);
            return cmd.ExecuteEntity<RefundAdjustInfo>();
        }

        public void RefundAdjustUpdate(RefundAdjustInfo entity)
        {

        }

        /// <summary>
        /// 修改补偿退款单的状态
        /// </summary>
        /// <param name="SysNo"></param>
        /// <param name="companyCode"></param>
        /// <returns></returns>
        public bool UpdateRefundAdjustStatus(RefundAdjustInfo entity)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("Customer_Update_RequestStatusSysNo");
            cmd.SetParameterValue("@Status", (int)entity.Status);
            cmd.SetParameterValue("@SysNo", entity.SysNo);
            cmd.SetParameterValue("@CompanyCode", entity.CompanyCode);
            return cmd.ExecuteNonQuery() > 0 ? true : false;
        }

        /// <summary>
        /// 审核补偿退款单
        /// </summary>
        /// <param name="SysNo"></param>
        /// <param name="Status"></param>
        /// <param name="RefundUserSysNo"></param>
        /// <returns></returns>
        public bool AuditRefundAdjust(int SysNo, RefundAdjustStatus Status, int? RefundUserSysNo, DateTime? AuditTime)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("Customer_Audit_RequestAdjust");
            cmd.SetParameterValue("@Status", (int)Status);
            cmd.SetParameterValue("@SysNo", SysNo);
            cmd.SetParameterValue("@RefundUserSysNo", RefundUserSysNo);
            cmd.SetParameterValue("@RefundTime", AuditTime);
            return cmd.ExecuteNonQuery() > 0 ? true : false;
        }

        /// <summary>
        /// 获取补偿退款单的状态
        /// </summary>
        /// <param name="SysNo"></param>
        /// <returns></returns>
        public int? GetRefundAdjustStatus(int SysNo)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("Customer_Get_RefundAdjustStatus");
            cmd.SetParameterValue("@SysNo", SysNo);
            int? result = (int)cmd.ExecuteScalar();
            return result;
        }

        #region 节能补贴相关 Add By Norton Li 2012-12-25

        /// <summary>
        /// 根据订单号获取节能补贴信息
        /// </summary>
        /// <param name="SOSysNo"></param>
        /// <returns></returns>
        public virtual RefundAdjustInfo GetEnergySubsidyBySOSysNo(RefundAdjustInfo entity)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("Customer_Get_EnergySubsidyBySOSysNo");
            cmd.SetParameterValue("@SOSysNo", entity.SOSysNo);
            cmd.SetParameterValue("@CompanyCode", entity.CompanyCode);
            return cmd.ExecuteEntity<RefundAdjustInfo>();
        }

        /// <summary>
        /// 查看订单中的商品是否有节能补贴商品
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public virtual RefundAdjustInfo GetInProductEnergySubsidyBySOSysNo(RefundAdjustInfo entity)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("Customer_Get_InProductEnergySubsidyBySOSysNo");
            cmd.SetParameterValue("@SOSysNo", entity.SOSysNo);
            cmd.SetParameterValue("@CompanyCode", entity.CompanyCode);
            return cmd.ExecuteEntity<RefundAdjustInfo>();
        }

        /// <summary>
        /// 判断某订单是否已经存在有效的节能补贴信息（非【已作废】和【审核拒绝】的状态为有效）
        /// </summary>
        /// <param name="soSysNo"></param>
        /// <returns></returns>
        public virtual bool GetEffectiveEnergySubsidySO(int soSysNo)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("Customer_Get_EffectiveEnergySubsidySO");
            cmd.SetParameterValue("@SOSysNo", soSysNo);
            return cmd.ExecuteScalar() == null ? true : false;
        }

        /// <summary>
        /// 判断订单是否已经出库
        /// </summary>
        /// <param name="soSysNo"></param>
        /// <returns></returns>
        public virtual bool IsShippingOut(int soSysNo)
        {
            bool result = false;
            DataCommand dc = DataCommandManager.GetDataCommand("GetIsShippingOutBySOSysNo");
            dc.SetParameterValue("@CompanyCode", "8601");
            dc.SetParameterValue("@SOSysNo", soSysNo);
            int status = -99;
            var tmp = dc.ExecuteScalar();
            if (tmp != null && int.TryParse(dc.ExecuteScalar().ToString(), out status))
            {
                if (status == 4)
                {
                    result = true;
                }
            }
            return result;
        }

        /// <summary>
        /// 判断是否做过物流拒收  Add By Norton Li 2013-01-04
        /// </summary>
        /// <param name="SOSysNo"></param>
        /// <returns></returns>
        public virtual bool IsHaveAutoRMA(int SOSysNo)
        {
            bool result = false;
            DataCommand dc = DataCommandManager.GetDataCommand("GetHaveAutoRMABySOSysNo");
            dc.SetParameterValue("@CompanyCode", "8601");
            dc.SetParameterValue("@SOSysNo", SOSysNo);
            int haveAutoRMA = -99;
            var tmp = dc.ExecuteScalar();
            if (tmp != null && int.TryParse(tmp.ToString(), out haveAutoRMA))
            {
                if (haveAutoRMA == 0)
                    return true;
            }
            return result;
        }

        /// <summary>
        /// 获取节能补贴的详细信息
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public virtual List<EnergySubsidyInfo> GetEnergySubsidyDetialsBySOSysNo(EnergySubsidyInfo entity)
        {
            List<EnergySubsidyInfo> entityList = new List<EnergySubsidyInfo>();
            CustomDataCommand cmd = null;
            if (entity.QueryType == "BasicInfo")
                cmd = DataCommandManager.CreateCustomDataCommandFromConfig("Customer_Get_EnergySubsidyDetials");
            else if (entity.QueryType == "ProductInfo")
                cmd = DataCommandManager.CreateCustomDataCommandFromConfig("Customer_Get_EnergySubsidyProductInfo");
            cmd.AddInputParameter("@SOSysNo", DbType.Int32, entity.SOSysNo.Value);
            cmd.AddInputParameter("@CompanyCode", DbType.String, entity.CompanyCode);
            entityList = cmd.ExecuteEntityList<EnergySubsidyInfo>();
            return entityList;
        }

        /// <summary>
        /// 获取节能补贴信息 只关注订单中的商品是否存在于ProductEnergy表中
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public virtual List<EnergySubsidyInfo> GetInProductEnergySubsidyDetials(EnergySubsidyInfo entity)
        {
            List<EnergySubsidyInfo> entityList = new List<EnergySubsidyInfo>();
            CustomDataCommand cmd = null;
            if (entity.QueryType == "BasicInfo")
                cmd = DataCommandManager.CreateCustomDataCommandFromConfig("Customer_Get_InProductEnergySubsidyDetials");
            else if (entity.QueryType == "ProductInfo")
                cmd = DataCommandManager.CreateCustomDataCommandFromConfig("Customer_Get_InProductEnergySubsidyProductInfo");
            cmd.AddInputParameter("@SOSysNo", DbType.Int32, entity.SOSysNo.Value);
            cmd.AddInputParameter("@CompanyCode", DbType.String, entity.CompanyCode);
            entityList = cmd.ExecuteEntityList<EnergySubsidyInfo>();
            return entityList;
        }

        #endregion
    }
}
