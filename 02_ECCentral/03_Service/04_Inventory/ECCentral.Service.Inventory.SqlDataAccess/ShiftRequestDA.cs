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
    [VersionExport(typeof(IShiftRequestDA))]
    public class ShiftRequestDA : IShiftRequestDA
    {
        #region 移仓单主信息维护

        /// <summary>
        /// 根据SysNO获取移仓单信息
        /// </summary>
        /// <param name="sysNo"></param>
        /// <returns></returns>
        public virtual ShiftRequestInfo GetShiftRequestInfoBySysNo(int requestSysNo)
        {
            DataCommand dc = DataCommandManager.GetDataCommand("Inventory_GetShiftRequestBySysNo");
            dc.SetParameterValue("@RequestSysNo", requestSysNo);
            return dc.ExecuteEntity<ShiftRequestInfo>();
        }

        /// <summary>
        /// 根据商品编号得到其所属产品线
        /// </summary>
        /// <param name="sysNo"></param>
        /// <returns></returns>
        public virtual ShiftRequestInfo GetProductLineInfo(int productSysNo)
        {
            DataCommand dc = DataCommandManager.GetDataCommand("Inventory_GetProductLineInfoByProductSysNo");
            dc.SetParameterValue("@ProductSysNo", productSysNo);
            return dc.ExecuteEntity<ShiftRequestInfo>();
        }

        /// <summary>
        /// 创建移仓单
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public virtual ShiftRequestInfo CreateShiftRequest(ShiftRequestInfo entity)
        {
            DataCommand command = DataCommandManager.GetDataCommand("Inventory_CreateShiftRequest");

            command.SetParameterValue("@RequestSysNo", entity.SysNo.Value);
            command.SetParameterValue("@RequestID", entity.RequestID);
            command.SetParameterValue("@SourceStockSysNo", entity.SourceStock.SysNo);
            command.SetParameterValue("@TargetStockSysNo", entity.TargetStock.SysNo);
            command.SetParameterValue("@CreateDate", DateTime.Now);
            command.SetParameterValue("@CreateUserSysNo", entity.CreateUser.SysNo);            
            command.SetParameterValue("@RequestStatus", (int?)entity.RequestStatus);
            command.SetParameterValue("@Note", entity.Note);
            command.SetParameterValue("@ShiftRequestType", (int)entity.ShiftType);
            command.SetParameterValue("@ShipViaTerm", entity.ShiftShippingType);
            command.SetParameterValue("@ConsignFlag", (int)entity.ConsignFlag);
            command.SetParameterValue("@CompanyCode", "8601");//[Mark][Alan.X.Luo 硬编码]
            command.SetParameterValue("@ProductLineSysno", entity.ProductLineSysno);
            return command.ExecuteEntity<ShiftRequestInfo>();
        }


        /// <summary>
        /// 更新移仓单
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public virtual ShiftRequestInfo UpdateShiftRequest(ShiftRequestInfo entity)
        {

            DataCommand command = DataCommandManager.GetDataCommand("Inventory_UpdateShiftRequest");
            command.SetParameterValue("@RequestSysNo", entity.SysNo.Value);
            command.SetParameterValue("@Note", entity.Note);
            command.SetParameterValue("@ProductLineSysno", entity.ProductLineSysno);
            return command.ExecuteEntity<ShiftRequestInfo>();
        }

        /// <summary>
        /// 更新损益单状态
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public virtual ShiftRequestInfo UpdateShiftRequestStatus(ShiftRequestInfo entity)
        {
            DataCommand dc = DataCommandManager.GetDataCommand("Inventory_UpdateShiftRequestStatus");
            dc.SetParameterValue("@RequestSysNo", entity.SysNo);
            dc.SetParameterValue("@RequestStatus", (int)entity.RequestStatus);
            dc.SetParameterValue("@AuditDate", entity.AuditDate);
            dc.SetParameterValue("@AuditUserSysNo", entity.AuditUser.SysNo);
            dc.SetParameterValue("@InStockDate", entity.OutStockDate);
            dc.SetParameterValue("@InStockUserSysNo", entity.OutStockUser.SysNo);
            dc.SetParameterValue("@OutStockDate", entity.OutStockDate);
            dc.SetParameterValue("@OutStockUserSysNo", entity.OutStockUser.SysNo);

            return dc.ExecuteEntity<ShiftRequestInfo>();
        }

        /// <summary>
        /// 设置特殊状态
        /// </summary>
        /// <param name="entity"></param>
        public virtual ShiftRequestInfo UpdateSpecialShiftType(ShiftRequestInfo entity)
        {
            DataCommand dc = DataCommandManager.GetDataCommand("Inventory_UpdateSpecialShiftType");
            dc.SetParameterValue("@RequestSysNo", entity.SysNo);            
            dc.SetParameterValue("@SpecialShiftType", (int)entity.SpecialShiftType);
            dc.SetParameterValue("@SpecialShiftSetUserSysNo", entity.SpecialShiftSetUser.SysNo);
            dc.SetParameterValue("@SpecialShiftSetDate", entity.SpecialShiftSetDate);

            return dc.ExecuteEntity<ShiftRequestInfo>();
        }
        /// <summary>
        /// 生成自增序列
        /// </summary>
        /// <returns></returns>
        public virtual int GetShiftRequestSequence()
        {
            var command = DataCommandManager.GetDataCommand("Inventory_GetShiftRequestSequence");
            int result = Convert.ToInt32(command.ExecuteScalar());
            return result;
        }

        public void UpdateStshiftItemGoldenTaxNo(string GoldenTaxNo, int stSysNo)
        {
            DataCommand command = DataCommandManager.GetDataCommand("Inventory_UpdateProductShiftItemGoldenTaxNo");
            command.SetParameterValue("@GoldenTaxNo", GoldenTaxNo);
            command.SetParameterValue("@ShiftItemSysNo", stSysNo);
            command.ExecuteNonQuery();
        }

        #endregion  移仓单主信息维护

        #region 移仓商品维护

        /// <summary>
        /// 根据移仓单SysNo获取移仓商品信息列表
        /// </summary>
        /// <param name="requestSysNo"></param>
        /// <returns></returns>
        public virtual List<ShiftRequestItemInfo> GetShiftItemListByRequestSysNo(int requestSysNo)
        {
            DataCommand dc = DataCommandManager.GetDataCommand("Inventory_GetShiftItemListByRequestSysNo");
            dc.SetParameterValue("@RequestSysNo", requestSysNo);

            using (IDataReader reader = dc.ExecuteDataReader())
            {
                var list = DataMapper.GetEntityList<ShiftRequestItemInfo, List<ShiftRequestItemInfo>>(reader);
                return list;
            }
        }


        /// <summary>
        /// 创建移仓单商品
        /// </summary>
        /// <param name="shiftItem"></param>
        /// <param name="requestSysNo"></param>
        /// <returns></returns>
        public virtual ShiftRequestItemInfo CreateShiftItem(ShiftRequestItemInfo shiftItem, int requestSysNo)
        {
            DataCommand dc = DataCommandManager.GetDataCommand("Inventory_CreateShiftItem");

            dc.SetParameterValue("@RequestSysNo", requestSysNo);
            dc.SetParameterValue("@ProductSysNo", shiftItem.ShiftProduct.SysNo);
            dc.SetParameterValue("@ShiftQuantity", shiftItem.ShiftQuantity);
            dc.SetParameterValue("@ShiftUnitCost", shiftItem.ShiftUnitCost);
            dc.SetParameterValue("@ShiftUnitCostWithoutTax", shiftItem.ShiftUnitCost);

            return dc.ExecuteEntity<ShiftRequestItemInfo>();
        }

        /// <summary>
        /// 更新移仓单商品
        /// </summary>
        /// <param name="shiftItem"></param>        
        /// <returns></returns>
        public virtual ShiftRequestItemInfo UpdateShiftItem(ShiftRequestItemInfo shiftItem)
        {
            DataCommand dc = DataCommandManager.GetDataCommand("Inventory_UpdateShiftItem");

            dc.SetParameterValue("@ShiftItemSysNo", shiftItem.SysNo);
            dc.SetParameterValue("@ProductSysNo", shiftItem.ShiftProduct.SysNo);
            dc.SetParameterValue("@ShiftQuantity", shiftItem.ShiftQuantity);
            dc.SetParameterValue("@InStockQuantity", shiftItem.ShiftUnitCost);
            dc.SetParameterValue("@ShiftUnitCost", shiftItem.ShiftUnitCost);
            dc.SetParameterValue("@ShiftUnitCostWithoutTax", shiftItem.ShiftUnitCost);
            dc.SetParameterValue("@ShippingCost", shiftItem.ShippingCost);

            return dc.ExecuteEntity<ShiftRequestItemInfo>();
        }

        /// <summary>
        /// 根据移仓单SysNo，删除其下的商品
        /// </summary>
        /// <param name="requestSysNo"></param>
        public virtual void DeleteShiftItemByRequestSysNo(int requestSysNo)
        {
            DataCommand command = DataCommandManager.GetDataCommand("Inventory_DeleteShiftItemByRequestSysNo");
            command.SetParameterValue("@RequestSysNo", requestSysNo);
            command.ExecuteNonQuery();
        }

        #endregion 移仓商品维护

        #region 仓库移仓配置
        public StockShiftConfigInfo CreateStockShiftConfig(StockShiftConfigInfo info)
        {
            var command = DataCommandManager.GetDataCommand("Inventory_CreateInventoryTransferConfig");
            command.SetParameterValue(info);
            info.SysNo = command.ExecuteScalar<int>();
            return info;
        }
        public bool UpdateStockShiftConfig(StockShiftConfigInfo info)
        {
            var command = DataCommandManager.GetDataCommand("Inventory_UpdateInventoryTransferConfig");
            command.SetParameterValue(info);
            return command.ExecuteNonQuery() > 0;
        }


        public StockShiftConfigInfo GetStockShiftConfigBySysNo(int sysNumber) 
        {
            var command = DataCommandManager.GetDataCommand("Inventory_GetInventoryTransferConfigBySysNo");
            command.SetParameterValue("@SysNo", sysNumber);
            return command.ExecuteEntity<StockShiftConfigInfo>();
        }

        public bool IsExistStockShiftConfig(StockShiftConfigInfo info)
        {
            var command = DataCommandManager.GetDataCommand("Inventory_GetInventoryTransferConfigParams");
            command.SetParameterValue(info);
            return command.ExecuteEntity<StockShiftConfigInfo>() != null;
        }
        #endregion
    }
}
