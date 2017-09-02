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
    [VersionExport(typeof(ILendRequestDA))]
    public class LendRequestDA : ILendRequestDA
    {
        #region 借货单维护

        /// <summary>
        /// 根据SysNo获取借货单信息
        /// </summary>
        /// <param name="requestSysNo"></param>
        /// <returns></returns>
        public virtual LendRequestInfo GetLendRequestInfoBySysNo(int requestSysNo)
        {
            DataCommand dc = DataCommandManager.GetDataCommand("Inventory_GetLendRequestInfoBySysNo");
            dc.SetParameterValue("@RequestSysNo", requestSysNo);
            return dc.ExecuteEntity<LendRequestInfo>();
        }

        /// <summary>
        /// 创建借货单信息
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public virtual LendRequestInfo CreateLendRequest(LendRequestInfo entity)
        {
            DataCommand dc = DataCommandManager.GetDataCommand("Inventory_CreateLendRequest");

            dc.SetParameterValue("@RequestSysNo", entity.SysNo);
            dc.SetParameterValue("@RequestID", entity.RequestID);
            dc.SetParameterValue("@StockSysNo", entity.Stock.SysNo);
            dc.SetParameterValue("@LendUserSysNo", entity.LendUser.SysNo);
            dc.SetParameterValue("@CreateDate", entity.CreateDate);
            dc.SetParameterValue("@CreateUserSysNo", entity.CreateUser.SysNo);
            dc.SetParameterValue("@RequestStatus", (int)entity.RequestStatus);
            dc.SetParameterValue("@Note", entity.Note);
            dc.SetParameterValue("@CompanyCode", "8601");//[Mark][Alan.X.Luo 硬编码]
            dc.SetParameterValue("@ProductLineSysno",entity.ProductLineSysno);
            return dc.ExecuteEntity<LendRequestInfo>();
        }

        /// <summary>
        /// 更新借货单信息
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public virtual LendRequestInfo UpdateLendRequest(LendRequestInfo entity)
        {
            DataCommand dc = DataCommandManager.GetDataCommand("Inventory_UpdateLendRequest");        
            dc.SetParameterValue("@RequestSysNo", entity.SysNo);
            dc.SetParameterValue("@LendUserSysNo", entity.LendUser.SysNo);
            dc.SetParameterValue("@Note", entity.Note);
            dc.SetParameterValue("@ProductLineSysNo", entity.ProductLineSysno);
            return dc.ExecuteEntity<LendRequestInfo>();
        }

        /// <summary>
        /// 更新借货单状态
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public virtual LendRequestInfo UpdateLendRequestStatus(LendRequestInfo entity)
        {
            DataCommand dc = DataCommandManager.GetDataCommand("Inventory_UpdateLendRequestStatus");
            dc.SetParameterValue("@RequestSysNo", entity.SysNo);
            dc.SetParameterValue("@RequestStatus", (int)entity.RequestStatus);
            dc.SetParameterValue("@AuditDate", entity.AuditDate);
            dc.SetParameterValue("@AuditUserSysNo", entity.AuditUser.SysNo);
            dc.SetParameterValue("@OutStockDate", entity.OutStockDate);
            dc.SetParameterValue("@OutStockUserSysNo", entity.OutStockUser.SysNo);            

            return dc.ExecuteEntity<LendRequestInfo>();
        }

        /// <summary>
        /// 为待创建的借货单获取系统编号
        /// </summary>        
        /// <returns></returns>
        public virtual int GetLendRequestSequence()
        {
            DataCommand dc = DataCommandManager.GetDataCommand("Inventory_GetLendRequestSequence");
            int result = Convert.ToInt32(dc.ExecuteScalar());
            return result;
        }

        #endregion 借货单维护

        #region 借货商品维护

        /// <summary>
        /// 根据借货单SysNo获取借货商品信息
        /// </summary>
        /// <param name="requestSysNo"></param>
        /// <returns></returns>
        public virtual List<LendRequestItemInfo> GetLendItemListByRequestSysNo(int requestSysNo)
        {
            DataCommand dc = DataCommandManager.GetDataCommand("Inventory_GetLendItemListByRequestSysNo");
            dc.SetParameterValue("@RequestSysNo", requestSysNo);
            
            using (IDataReader reader = dc.ExecuteDataReader())
            {
                var list = DataMapper.GetEntityList<LendRequestItemInfo, List<LendRequestItemInfo>>(reader);
                return list;
            }
        }

        /// <summary>
        /// 根据商品编号得到其所属产品线
        /// </summary>
        /// <param name="sysNo"></param>
        /// <returns></returns>
        public virtual LendRequestInfo GetProductLineInfo(int productSysNo)
        {
            DataCommand dc = DataCommandManager.GetDataCommand("Inventory_GetLendProductLineInfoByProductSysNo");
            dc.SetParameterValue("@ProductSysNo", productSysNo);
            return dc.ExecuteEntity<LendRequestInfo>();
        }

        /// <summary>
        /// 创建借货商品记录
        /// </summary>
        /// <param name="requestSysNo"></param>
        /// <param name="lendItem"></param>
        /// <returns></returns>
        public virtual LendRequestItemInfo CreateLendItem(LendRequestItemInfo lendItem, int requestSysNo)
        {
            DataCommand dc = DataCommandManager.GetDataCommand("Inventory_CreateLendItem");

            dc.SetParameterValue("@RequestSysNo", requestSysNo);
            dc.SetParameterValue("@ProductSysNo", lendItem.LendProduct.SysNo);
            dc.SetParameterValue("@LendQuantity", lendItem.LendQuantity);
            dc.SetParameterValue("@ExpectReturnDate", lendItem.ExpectReturnDate);
            dc.SetParameterValue("@LendUnitCost", lendItem.LendUnitCost);
            dc.SetParameterValue("@LendUnitCostWithoutTax", lendItem.LendUnitCostWithoutTax);
            dc.SetParameterValue("@LendUnitCostWhenCreate", lendItem.LendUnitCostWhenCreate);
            dc.SetParameterValue("@CompanyCode", "8601");//[Mark][Alan.X.Luo 硬编码]

            return dc.ExecuteEntity<LendRequestItemInfo>();
        }

        /// <summary>
        /// 更新借货商品记录
        /// </summary>
        /// <param name="lendItem"></param>
        /// <returns></returns>
        public virtual LendRequestItemInfo UpdateLendItem(LendRequestItemInfo lendItem)
        {
            DataCommand dc = DataCommandManager.GetDataCommand("Inventory_UpdateLendItem");

            dc.SetParameterValue("@LendItemSysNo", lendItem.SysNo);
            dc.SetParameterValue("@ProductSysNo", lendItem.LendProduct.SysNo);
            dc.SetParameterValue("@LendQuantity", lendItem.LendQuantity);
            dc.SetParameterValue("@ExpectReturnDate", lendItem.ExpectReturnDate);
            dc.SetParameterValue("@LendUnitCost", lendItem.LendUnitCost);
            dc.SetParameterValue("@LendUnitCostWithoutTax", lendItem.LendUnitCostWithoutTax);
            //dc.SetParameterValue("@LendUnitCostWhenCreate", lendItem.LendUnitCostWhenCreate);

            return dc.ExecuteEntity<LendRequestItemInfo>();
        }

        /// <summary>
        /// 删除借货单中所有借货商品
        /// </summary>
        /// <param name="requestSysNo"></param>
        /// <returns></returns>
        /// 
        public virtual void DeleteLendItemByRequestSysNo(int requestSysNo)
        {
            DataCommand dc = DataCommandManager.GetDataCommand("Inventory_DeleteLendItemByRequestSysNo");
            dc.SetParameterValue("@RequestSysNo", requestSysNo);
            dc.ExecuteNonQuery();
        }

        #endregion 借货商品维护

        #region 归还商品维护

        /// <summary>
        /// 根据借货单SysNo获取归还商品信息
        /// </summary>
        /// <param name="requestSysNo"></param>
        /// <returns></returns>
        public virtual List<LendRequestReturnItemInfo> GetReturnItemListByRequestSysNo(int requestSysNo)
        {
            DataCommand dc = DataCommandManager.GetDataCommand("Inventory_GetReturnItemListByRequestSysNo");
            dc.SetParameterValue("@RequestSysNo", requestSysNo);

            using (IDataReader reader = dc.ExecuteDataReader())
            {
                var list = DataMapper.GetEntityList<LendRequestReturnItemInfo, List<LendRequestReturnItemInfo>>(reader);
                return list;
            }
        }

        /// <summary>
        /// 创建借货商品归还记录
        /// </summary>
        /// <param name="requestSysNo"></param>
        /// <param name="returnItem"></param>
        /// <returns></returns>
        public virtual LendRequestReturnItemInfo CreateReturnItem(LendRequestReturnItemInfo returnItem, int requestSysNo)
        {
            DataCommand dc = DataCommandManager.GetDataCommand("Inventory_CreateReturnItem");
            dc.SetParameterValue("@RequestSysNo", requestSysNo);
            dc.SetParameterValue("@ProductSysNo", returnItem.ReturnProduct.SysNo);
            dc.SetParameterValue("@ReturnQuantity", returnItem.ReturnQuantity);
            dc.SetParameterValue("@ReturnDate", returnItem.ReturnDate);
            dc.SetParameterValue("@CompanyCode", "8601");//[Mark][Alan.X.Luo 硬编码]

            return dc.ExecuteEntity<LendRequestReturnItemInfo>();
        }

        /// <summary>
        /// 删除借货商品归还记录
        /// </summary>
        /// <param name="requestSysNo"></param>
        /// <param name="returnItem"></param>
        /// <returns></returns>
        public static void DeleteReturnItem(int returnItemSysNo)
        {
            DataCommand dc = DataCommandManager.GetDataCommand("Inventory_DeleteReturnItem");
            dc.SetParameterValue("@ReturnItemSysNo", returnItemSysNo);

            dc.ExecuteNonQuery();
        }

        #endregion 归还商品维护
    }
}
