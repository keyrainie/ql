using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

using ECCentral.BizEntity.Customer;
using ECCentral.BizEntity.Inventory;
using ECCentral.Service.Inventory.IDataAccess;
using ECCentral.Service.Utility;
using ECCentral.Service.Utility.DataAccess;

namespace ECCentral.Service.Inventory.SqlDataAccess
{
    [VersionExport(typeof(IAdjustRequestDA))]
    public class AdjustRequestDA : IAdjustRequestDA
    {
        #region 损益单主信息维护

        /// <summary>
        /// 根据SysNO获取损益单信息
        /// </summary>
        /// <param name="brandSysNo"></param>
        /// <returns></returns>
        public virtual AdjustRequestInfo GetAdjustRequestInfoBySysNo(int requestSysNo)
        {
            DataCommand dc = DataCommandManager.GetDataCommand("Inventory_GetAdjustRequestBySysNo");
            dc.SetParameterValue("@SysNo", requestSysNo);
            return dc.ExecuteEntity<AdjustRequestInfo>();
        }

        /// <summary>
        /// 根据商品编号得到其所属产品线
        /// </summary>
        /// <param name="sysNo"></param>
        /// <returns></returns>
        public virtual AdjustRequestInfo GetProductLineInfo(int productSysNo)
        {
            DataCommand dc = DataCommandManager.GetDataCommand("Inventory_GetAdjustProductLineInfoByProductSysNo");
            dc.SetParameterValue("@ProductSysNo", productSysNo);
            return dc.ExecuteEntity<AdjustRequestInfo>();
        }

        /// <summary>
        /// 创建损益单
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public virtual AdjustRequestInfo CreateAdjustRequest(AdjustRequestInfo entity)
        {
            DataCommand command = DataCommandManager.GetDataCommand("Inventory_CreateAdjustRequest");

            command.SetParameterValue("@RequestSysNo", entity.SysNo.Value);
            command.SetParameterValue("@RequestID", entity.RequestID);
            command.SetParameterValue("@StockSysNo", entity.Stock.SysNo);
            command.SetParameterValue("@CreateDate", entity.CreateDate);
            command.SetParameterValue("@CreateUserSysNo", entity.CreateUser.SysNo);                   
            command.SetParameterValue("@AuditDate", entity.AuditDate);
            command.SetParameterValue("@AuditUserSysNo", entity.AuditUser.SysNo);
            command.SetParameterValue("@OutStockDate", entity.OutStockDate);
            command.SetParameterValue("@OutStockUserSysNo", entity.OutStockUser.SysNo);
            command.SetParameterValue("@RequestStatus", (int?)entity.RequestStatus);
            command.SetParameterValue("@AdjustProperty", (int?)entity.AdjustProperty);
            command.SetParameterValue("@ConsignFlag", (int?)entity.ConsignFlag);
            command.SetParameterValue("@Note", entity.Note);
            command.SetParameterValue("@CompanyCode", "8601");//[Mark][Alan.X.Luo 硬编码]
            command.SetParameterValue("@ProductLineSysno", entity.ProductLineSysno);
            return command.ExecuteEntity<AdjustRequestInfo>();
        }

        /// <summary>
        /// 更新损益单
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public virtual AdjustRequestInfo UpdateAdjustRequest(AdjustRequestInfo entity)
        {
            DataCommand command = DataCommandManager.GetDataCommand("Inventory_UpdateAdjustRequest");
            command.SetParameterValue("@RequestSysNo", entity.SysNo.Value);
            command.SetParameterValue("@RequestID", entity.RequestID);
            command.SetParameterValue("@StockSysNo", entity.Stock.SysNo);
            command.SetParameterValue("@RequestStatus", (int?)entity.RequestStatus);
            command.SetParameterValue("@AdjustProperty", (int?)entity.AdjustProperty);
            command.SetParameterValue("@Note", entity.Note);
            command.SetParameterValue("@ProductLineSysno", entity.ProductLineSysno);
            return command.ExecuteEntity<AdjustRequestInfo>();
        }

        /// <summary>
        /// 更新损益单状态
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public virtual AdjustRequestInfo UpdateAdjustRequestStatus(AdjustRequestInfo entity)
        {
            DataCommand dc = DataCommandManager.GetDataCommand("Inventory_UpdateAdjustRequestStatus");
            dc.SetParameterValue("@RequestSysNo", entity.SysNo);
            dc.SetParameterValue("@RequestStatus", (int)entity.RequestStatus);
            dc.SetParameterValue("@AuditDate", entity.AuditDate);
            dc.SetParameterValue("@AuditUserSysNo", entity.AuditUser.SysNo);
            dc.SetParameterValue("@OutStockDate", entity.OutStockDate);
            dc.SetParameterValue("@OutStockUserSysNo", entity.OutStockUser.SysNo);

            return dc.ExecuteEntity<AdjustRequestInfo>();
        }
       

        /// <summary>
        /// 生成自增序列
        /// </summary>
        /// <returns></returns>
        public virtual int GetAdjustRequestSequence()
        {
            var command = DataCommandManager.GetDataCommand("Inventory_GetAdjustRequestSequence");
            int result = Convert.ToInt32(command.ExecuteScalar());
            return result;
        }

        /// <summary>
        /// 如果已存在有效的隔月补充增益单，不可以重新生成
        /// </summary>
        /// <param name="adjustSysNo"></param>
        /// <returns></returns>
        public virtual int CheckPositiveAdjustRequestSysNo(int adjustSysNo)
        {
            return 0;
        }

        #endregion 损益单主信息维护

        #region 损益单商品维护

        /// <summary>
        /// 根据SysNO获取损益单商品列表
        /// </summary>
        /// <param name="requestSysNo"></param>
        /// <returns></returns>
        public virtual List<AdjustRequestItemInfo> GetAdjustItemListByRequestSysNo(int requestSysNo)
        {
            DataCommand dc = DataCommandManager.GetDataCommand("Inventory_GetAdjustItemListByRequestSysNo");
            dc.SetParameterValue("@RequestSysNo", requestSysNo);

            using (IDataReader reader = dc.ExecuteDataReader())
            {
                var list = DataMapper.GetEntityList<AdjustRequestItemInfo, List<AdjustRequestItemInfo>>(reader);
                return list;
            }
        }

        /// <summary>
        /// 损益单商品
        /// </summary>
        /// <param name="adjustItem"></p                 aram>
        /// <param name="requestSysNo"></param>
        /// <returns></returns>
        public virtual AdjustRequestItemInfo CreateAdjustItem(AdjustRequestItemInfo adjustItem, int requestSysNo)
        {
            DataCommand dc = DataCommandManager.GetDataCommand("Inventory_CreateAdjustItem");

            dc.SetParameterValue("@RequestSysNo", requestSysNo);
            dc.SetParameterValue("@ProductSysNo", adjustItem.AdjustProduct.SysNo);
            dc.SetParameterValue("@AdjustQty", adjustItem.AdjustQuantity);
            dc.SetParameterValue("@AdjustCost", adjustItem.AdjustCost);

            return dc.ExecuteEntity<AdjustRequestItemInfo>();
        }

        /// <summary>
        /// 删除损益单商品
        /// </summary>
        /// <param name="requestSysNo"></param>
        public virtual void DeleteAdjustItemByRequestSysNo(int requestSysNo)
        {
            DataCommand command = DataCommandManager.GetDataCommand("Inventory_DeleteAdjustItemByRequestSysNo");
            command.SetParameterValue("@RequestSysNo", requestSysNo);
            command.ExecuteNonQuery();
        }

        /// <summary>
        /// 更新损益成本
        /// </summary>
        /// <param name="adjustItem"></param>      
        /// <returns></returns>
        public virtual void UpdateAdjustItemCost(AdjustRequestItemInfo adjustItem)
        {
            DataCommand command = DataCommandManager.GetDataCommand("Inventory_UpdateAdjustItemCost");
            command.SetParameterValue("@AdjustItemSysNo", adjustItem.SysNo);
            command.SetParameterValue("@AdjustCost", adjustItem.AdjustCost);
            command.ExecuteNonQuery();
        }

        /// <summary>
        /// 获取商品库存成本
        /// </summary>
        /// <param name="productSysNo"></param>
        /// <returns></returns>
        public decimal GetItemCost(int productSysNo)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("Inventory_GetItemCost");
            cmd.SetParameterValue("@ProductSysNo", productSysNo);
            return cmd.ExecuteScalar<decimal>();
        }
        #endregion 损益单商品维护

        #region 损益单发票维护

        /// <summary>
        /// 根据SysNO获取损益单发票信息
        /// </summary>
        /// <param name="requestSysNo"></param>
        /// <returns></returns>
        public virtual AdjustRequestInvoiceInfo GetInvoiceInfoByRequestSysNo(int requestSysNo)
        {
            DataCommand dc = DataCommandManager.GetDataCommand("Inventory_GetAdjustInvoiceByRequestSysNo");
            dc.SetParameterValue("@RequestSysNo", requestSysNo);
            return dc.ExecuteEntity<AdjustRequestInvoiceInfo>();
        }

        /// <summary>
        /// 创建发票信息
        /// </summary>
        /// <param name="invoiceInfo"></param>
        /// <param name="requestSysNo"></param>
        /// <returns></returns>
        public virtual AdjustRequestInvoiceInfo CreateAdjustRequestInvoice(AdjustRequestInvoiceInfo invoiceInfo, int requestSysNo)
        {
            DataCommand dc = DataCommandManager.GetDataCommand("Inventory_CreateAdjustInvoice");

            dc.SetParameterValue("@RequestSysNo", requestSysNo);
            dc.SetParameterValue("@ReceiveName", invoiceInfo.ReceiveName);
            dc.SetParameterValue("@ContactAddress", invoiceInfo.ContactAddress);
            dc.SetParameterValue("@ContactShippingAddress", invoiceInfo.ContactShippingAddress);
            dc.SetParameterValue("@ContactPhoneNumber", invoiceInfo.ContactPhoneNumber);
            dc.SetParameterValue("@CustomerID", invoiceInfo.CustomerID);
            dc.SetParameterValue("@InvoiceNumber", invoiceInfo.InvoiceNumber);
            dc.SetParameterValue("@Notes", invoiceInfo.Note);

            return dc.ExecuteEntity<AdjustRequestInvoiceInfo>();
        }

        /// <summary>
        /// 修改发票信息
        /// </summary>
        /// <param name="invoiceInfo"></param>
        /// <param name="requestSysNo"></param>
        /// <returns></returns>
        public virtual AdjustRequestInvoiceInfo UpdateAdjustRequestInvoice(AdjustRequestInvoiceInfo invoiceInfo)
        {            
            DataCommand dc = DataCommandManager.GetDataCommand("Inventory_UpdateAdjustInvoice");

            dc.SetParameterValue("@InvoiceSysNo", invoiceInfo.SysNo);
            dc.SetParameterValue("@ReceiveName", invoiceInfo.ReceiveName);
            dc.SetParameterValue("@ContactAddress", invoiceInfo.ContactAddress);
            dc.SetParameterValue("@ContactShippingAddress", invoiceInfo.ContactShippingAddress);
            dc.SetParameterValue("@ContactPhoneNumber", invoiceInfo.ContactPhoneNumber);
            dc.SetParameterValue("@CustomerID", invoiceInfo.CustomerID);
            dc.SetParameterValue("@InvoiceNumber", invoiceInfo.InvoiceNumber);
            dc.SetParameterValue("@Notes", invoiceInfo.Note);

            return dc.ExecuteEntity<AdjustRequestInvoiceInfo>();
        }

        #endregion 损益单发票维护
      
        #region 其他, SSB      

        #endregion 其他, SSB

        /// <summary>
        /// 检测 是否是批号管理商品
        /// </summary>
        /// <param name="productSysNo">商品编号</param>
        /// <returns></returns>
        public bool CheckISBatchNumberProduct(int productSysNo)
        {
            var command = DataCommandManager.GetDataCommand("Inventory_CheckISBatchNumberProduct");
            command.SetParameterValue("@SysNo", productSysNo);
            if (command.ExecuteScalar() != null && command.ExecuteScalar().ToString() == "Y")
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 单据出库后给仓库发SSB
        /// </summary>
        /// <param name="paramXml">SSB</param>
        /// <returns></returns>
        public void SendSSBToWMS(string paramXml)
        {
            DataCommand command = DataCommandManager.GetDataCommand("Inventory_SendSSBToWMS");
            command.SetParameterValue("@Msg", paramXml);
            command.ExecuteNonQuery();
        }
    }
}
