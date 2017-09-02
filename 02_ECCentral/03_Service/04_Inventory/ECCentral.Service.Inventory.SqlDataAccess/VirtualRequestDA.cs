using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

using ECCentral.BizEntity.Common;
using ECCentral.BizEntity.Inventory;
using ECCentral.QueryFilter.Inventory;

using ECCentral.Service.Inventory.IDataAccess;
using ECCentral.Service.Utility;
using ECCentral.Service.Utility.DataAccess;
using System.Threading;


namespace ECCentral.Service.Inventory.SqlDataAccess
{
    [VersionExport(typeof(IVirtualRequestDA))]
    public class VirtualRequestDA : IVirtualRequestDA
    {
        #region 虚库申请单维护

        /// <summary>
        /// 根据SysNO获取虚库申请单信息
        /// </summary>
        /// <param name="sysNo"></param>
        /// <returns></returns>
        public VirtualRequestInfo GetVirtualRequestInfoBySysNo(int requestSysNo)
        {
            var command = DataCommandManager.GetDataCommand("GetVirtualRequestBySysNumber");
            command.SetParameterValue("@SysNo", requestSysNo);
            return command.ExecuteEntity<VirtualRequestInfo>();
        }

        /// <summary>
        /// 创建虚库申请单
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public VirtualRequestInfo CreateVirtualRequest(VirtualRequestInfo entityToCreate)
        {
            return null;
        }

        /// <summary>
        /// 更新虚库申请单
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public VirtualRequestInfo UpdateVirtualRequestStatus(VirtualRequestInfo entityToUpdate)
        {
            return null;
        }

        #endregion 虚库申请单

        #region Origin DA SourceCode, Must Refactor

        public int Create(VirtualRequestInfo entity)
        {
            var command = DataCommandManager.GetDataCommand("CreateVirtualInentory");
            command.SetParameterValue("@ProductSysNo", entity.VirtualProduct.SysNo);
            command.SetParameterValue("@VirtualQty", entity.VirtualQuantity);
            command.SetParameterValue("@CreateTime", entity.CreateDate);
            command.SetParameterValue("@CreateUserSysNo", entity.CreateUser.SysNo);
            command.SetParameterValue("@Note", entity.RequestNote);
            command.SetParameterValue("@StockSysNo", entity.Stock.SysNo);
            command.SetParameterValue("@VirtualType", entity.VirtualType);
            command.SetParameterValue("@CompanyCode", "8601");//[Mark][Alan.X.Luo 硬编码]

            return Convert.ToInt32(command.ExecuteScalar<decimal>());
        }

        public int Apply(VirtualRequestInfo entity)
        {
            var command = DataCommandManager.GetDataCommand("InsertVirtualRequest");
            command.SetParameterValue("@ProductSysNo", entity.VirtualProduct.SysNo);
            command.SetParameterValue("@StockSysNo", entity.Stock.SysNo);
            command.SetParameterValue("@VirtualQty", entity.VirtualQuantity);
            command.SetParameterValue("@CreateTime", entity.CreateDate);
            command.SetParameterValue("@CreateUserSysNo", entity.CreateUser.SysNo);
            command.SetParameterValue("@PMRequestNote", entity.RequestNote);
            command.SetParameterValue("@Status", (int)entity.RequestStatus);
            command.SetParameterValue("@VirtualType", entity.VirtualType);
            command.SetParameterValue("@StartTime", entity.StartDate);
            command.SetParameterValue("@EndTime", entity.EndDate);
            command.SetParameterValue("@CompanyCode", "8601");//[Mark][Alan.X.Luo 硬编码]
            entity.SysNo = command.ExecuteScalar<int>();
            return entity.SysNo.Value;
        }

        public bool Verify(VirtualRequestInfo entity)
        {
            var command = DataCommandManager.GetDataCommand("VerifyVirtualRequest");
            command.SetParameterValue("@SysNo", entity.SysNo);
            command.SetParameterValue("@AuditNote", entity.AuditNote);
            command.SetParameterValue("@AuditTime", DateTime.Now);
            command.SetParameterValueAsCurrentUserSysNo("@AuditUserSysNo");
            command.SetParameterValue("@Status", (int)entity.RequestStatus);
            command.SetParameterValue("@CompanyCode", "8601");//[Mark][Alan.X.Luo 硬编码]

            return command.ExecuteNonQuery() > 0;
        }

        public bool Abandon(int sysNumber, VirtualRequestStatus status, string companyCode)
        {
            var command = DataCommandManager.GetDataCommand("UpdateVirtualStatus");
            command.SetParameterValue("@SysNo", sysNumber);
            command.SetParameterValue("@Status", (int)status);
            command.SetParameterValue("@CompanyCode", "8601");//[Mark][Alan.X.Luo 硬编码]
            return command.ExecuteNonQuery() > 0;
        }

        public int ExistOriginRequestByStockAndItem(int stockSysNumber, int itemSysNumber, string companyCode)
        {
            var command = DataCommandManager.GetDataCommand("ExistOriginRequestByStockAndItem");
            command.SetParameterValue("@StockSysNo", stockSysNumber);
            command.SetParameterValue("@ProductSysNo", itemSysNumber);
            command.SetParameterValue("@Status", (int)VirtualRequestStatus.Origin);
            command.SetParameterValue("@CompanyCode", "8601");//[Mark][Alan.X.Luo 硬编码]
            object result = command.ExecuteScalar<object>();
            return result == null ? 0 : Convert.ToInt32(result);
        }

        public List<VirtualRequestInfo> ExistNeedCloseRequestByStockAndItem(int stockSysNumber, int itemSysNumber, string companyCode)
        {
            var command = DataCommandManager.GetDataCommand("ExistNeedCloseRequestByStockAndItem");
            command.SetParameterValue("@StockSysNo", stockSysNumber);
            command.SetParameterValue("@ProductSysNo", itemSysNumber);
            command.SetParameterValue("@CompanyCode", string.IsNullOrEmpty(companyCode) ? "8601" : companyCode);//[Mark][Alan.X.Luo 硬编码]
            return command.ExecuteEntityList<VirtualRequestInfo>();
        }

        public int ExistNeedCloseVirtualQuantity(int stockSysNumber, int productSysNumber, int sysNo, string companyCode)
        {
            var command = DataCommandManager.GetDataCommand("ExistNeedCloseVirtualQuantity");
            command.SetParameterValue("@StockSysNo", stockSysNumber);
            command.SetParameterValue("@ProductSysNo", productSysNumber);
            command.SetParameterValue("@SysNo", sysNo);
            command.SetParameterValue("@CompanyCode", string.IsNullOrEmpty(companyCode) ? "8601" : companyCode);     //[Mark][Alan.X.Luo 硬编码]
            object result =command.ExecuteScalar();
            return result == null ? 0 : Convert.ToInt32(result);
        }

        public int CloseRequest(int StVirtualRequestSysNo, VirtualRequestStatus Status, int IsAdjustVirtualQty, string CompanyCode)
        {
            var command = DataCommandManager.GetDataCommand("CloseRequest");
            command.SetParameterValue("@StVirtualRequestSysNo", StVirtualRequestSysNo);
            command.SetParameterValue("@Status", Status);
            command.SetParameterValue("@IsAdjustVirtualQty", IsAdjustVirtualQty);
            command.SetParameterValueAsCurrentUserAcct("@InUser");
            command.SetParameterValue("@CompanyCode", "8601");//[Mark][Alan.X.Luo 硬编码]
            command.SetParameterValue("@StoreCompanyCode", "8601");//[Mark][Alan.X.Luo 硬编码]
            command.SetParameterValue("@LanguageCode", Thread.CurrentThread.CurrentCulture.Name);
            command.ExecuteNonQuery();
            object result = command.GetParameterValue("@ReturnValue");
            return result == null ? 0 : Convert.ToInt32(result);
        }

        public bool UpdateProductExtension(int virtualType, int itemSysNumber, string companyCode)
        {
            var command = DataCommandManager.GetDataCommand("UpdateProductExtension");
            command.SetParameterValue("@VirtualType", virtualType);
            command.SetParameterValue("@ProductSysNo", itemSysNumber);
            command.SetParameterValue("@CompanyCode", "8601");//[Mark][Alan.X.Luo 硬编码]
            return command.ExecuteNonQuery() > 0;
        }

        public int CheckVirtualQty(int productSysNo, int quantity, int warehouseNumber, string companyCode)
        {
            var command = DataCommandManager.GetDataCommand("CheckVirtualQty");
            command.SetParameterValue("@ProductSysNo", productSysNo);
            command.SetParameterValue("@Quantity", quantity);
            command.SetParameterValue("@WarehouseNumber", warehouseNumber);
            command.SetParameterValue("@LanguageCode", Thread.CurrentThread.CurrentCulture.Name);
            command.SetParameterValue("@CompanyCode", "8601");//[Mark][Alan.X.Luo 硬编码]
            command.SetParameterValue("@StoreCompanyCode", "8601");//[Mark][Alan.X.Luo 硬编码]
            return (int)command.ExecuteScalar();
        }
        #endregion



    }
}
