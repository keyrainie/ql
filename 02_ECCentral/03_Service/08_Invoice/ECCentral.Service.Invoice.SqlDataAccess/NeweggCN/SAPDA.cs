using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.Utility;
using ECCentral.Service.Invoice.IDataAccess;
using ECCentral.Service.Utility.DataAccess;
using ECCentral.BizEntity.Invoice.SAP;

namespace ECCentral.Service.Invoice.SqlDataAccess
{
    [VersionExport(typeof(ISAPDA))]
    public class SAPDA : ISAPDA
    {
        #region Vendor
        /// <summary>
        /// 创建SAP供应商配置
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public virtual int CreateSAPVendor(SAPVendorInfo entity)
        {
            DataCommand dc = DataCommandManager.GetDataCommand("CreateSAPVendor");
            dc.SetParameterValue("@VendorSysNo", entity.VendorSysNo);
            dc.SetParameterValue("@SAPVendorID", entity.SAPVendorID);
            dc.SetParameterValue("@VendorShortName", entity.SAPVendorName);
            dc.SetParameterValue("@PaymentTerm", entity.PaymentTerm);
            dc.SetParameterValue("@CompanyCode", entity.CompanyCode);

            return dc.ExecuteNonQuery();
        }
        #endregion
        #region Company
        /// <summary>
        /// 检查是否有相同StockID的公司编码配置
        /// </summary>
        /// <param name="stockID"></param>
        /// <param name="companyCode"></param>
        /// <returns>相同StockID数量</returns>
        public virtual int CheckSAPStock(int stockID, string companyCode)
        {
            DataCommand command = DataCommandManager.GetDataCommand("CheckSAPWarehouse");
            command.SetParameterValue("@WarehouseNumber", stockID);
            command.SetParameterValue("@CompanyCode", companyCode);

            return Convert.ToInt32(command.ExecuteScalar());
        }
        /// <summary>
        /// 检查是否有相同SAP公司代码的公司编码配置
        /// </summary>
        /// <param name="stockID"></param>
        /// <param name="sapCoCode"></param>
        /// <param name="companyCode"></param>
        /// <returns>相同SAP公司代码数量</returns>
        public virtual int CheckSAPCoCode(int stockID, string sapCoCode, string companyCode)
        {
            DataCommand command = DataCommandManager.GetDataCommand("CheckSAPCoCode");
            command.SetParameterValue("@WarehouseNumber", stockID);
            command.SetParameterValue("@SAPCoCode", sapCoCode);
            command.SetParameterValue("@CompanyCode", companyCode);

            return Convert.ToInt32(command.ExecuteScalar());
        }
        /// <summary>
        /// 根据仓库编号获取SAP公司编码配置信息
        /// </summary>
        /// <param name="stockID"></param>
        /// <param name="companyCode"></param>
        /// <returns></returns>
        public virtual SAPCompanyInfo GetSAPCompanyInfoByStockID(int stockID, string companyCode)
        {
            DataCommand command = DataCommandManager.GetDataCommand("GetSAPCompanyInfo");
            command.SetParameterValue("@WarehouseNumber", stockID);
            command.SetParameterValue("@CompanyCode", companyCode);

            return command.ExecuteEntity<SAPCompanyInfo>();
        }

        /// <summary>
        /// 创建SAP公司编码信息
        /// </summary>
        /// <param name="entity"></param>
        public virtual void CreateSAPCompany(SAPCompanyInfo entity)
        {
            DataCommand dc = DataCommandManager.GetDataCommand("CreateSAPCompany");
            dc.SetParameterValue("@WarehouseNumber", entity.StockID);
            dc.SetParameterValue("@WarehouseName", entity.StockName);
            dc.SetParameterValue("@SapCoCode", entity.SAPCompanyCode);
            dc.SetParameterValue("@SapBusinessArea", entity.SAPBusinessArea);
            dc.SetParameterValue("@SalesTaxRate", entity.SalesTaxRate);
            dc.SetParameterValue("@PurchaseTaxRate", entity.PurchaseTaxRate);
            dc.SetParameterValue("@CompanyCode", entity.CompanyCode);
            dc.SetParameterValue("@WorkStatus", entity.WorkStatus);

            dc.ExecuteNonQuery();
        }
        /// <summary>
        /// 更新相应StockID的SAP公司编码信息为无效状态
        /// </summary>
        /// <param name="stockID"></param>
        /// <param name="companyCode"></param>
        public virtual void UpdateSAPCompanyWorkStatus(int stockID, string companyCode)
        {
            DataCommand dc = DataCommandManager.GetDataCommand("UpdateSAPCompanyWorkStatus");
            dc.SetParameterValue("@WarehouseNumber", stockID);
            dc.SetParameterValue("@CompanyCode", companyCode);

            dc.ExecuteNonQuery();

        }
        #endregion
        /// <summary>
        /// 更新SAP确认人信息
        /// </summary>
        /// <param name="entity"></param>
        public virtual void UpdateIPPUser(SAPIPPUserInfo entity)
        {
            DataCommand dc = DataCommandManager.GetDataCommand("UpdateIPPUser");
            dc.SetParameterValue("@SysNo", entity.SysNo);
            dc.SetParameterValue("@CustID", entity.CustID);
            dc.SetParameterValue("@SystemConfirmID", entity.SystemConfirmID);
            dc.ExecuteNonQuery();
        }
        /// <summary>
        /// 检查支付类型是否重复
        /// </summary>
        /// <param name="payTypeSysNo"></param>
        /// <param name="companyCode"></param>
        /// <returns></returns>
        public virtual int CheckPayTypeForSAPIPPUser(int payTypeSysNo, string companyCode)
        {
            DataCommand dc = DataCommandManager.GetDataCommand("CheckPayTypeForSAPIPPUser");
            dc.SetParameterValue("@PayType", payTypeSysNo);
            dc.SetParameterValue("@CompanyCode", companyCode);

            return Convert.ToInt32(dc.ExecuteScalar());
        }
        /// <summary>
        /// 创建SAP确认人配置
        /// </summary>
        /// <param name="entity"></param>
        public virtual void CreateSAPIPPUser(SAPIPPUserInfo entity)
        {
            DataCommand dc = DataCommandManager.GetDataCommand("CreateSAPIPPUser");
            dc.SetParameterValue("@PayType", entity.PayTypeSysNo);
            dc.SetParameterValue("@CustID", entity.CustID);
            dc.SetParameterValue("@CustDescription", entity.CustDescription);
            dc.SetParameterValue("@SystemConfirmID", entity.SystemConfirmID);
            dc.SetParameterValue("@CompanyCode", entity.CompanyCode);

            dc.ExecuteNonQuery();
        }
    }
}
