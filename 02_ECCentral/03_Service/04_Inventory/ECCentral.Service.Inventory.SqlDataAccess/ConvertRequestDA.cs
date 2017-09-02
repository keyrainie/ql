using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

using ECCentral.Service.Inventory.IDataAccess;
using ECCentral.Service.Utility;
using ECCentral.Service.Utility.DataAccess;
using ECCentral.BizEntity.Inventory;

namespace ECCentral.Service.Inventory.SqlDataAccess
{
    [VersionExport(typeof(IConvertRequestDA))]
    public class ConvertRequestDA : IConvertRequestDA
    {
        #region 转换单维护

        /// <summary>
        /// 根据SysNo获取转换单信息
        /// </summary>
        /// <param name="requestSysNo"></param>
        /// <returns></returns>
        public virtual ConvertRequestInfo GetConvertRequestInfoBySysNo(int requestSysNo)
        {
            DataCommand dc = DataCommandManager.GetDataCommand("Inventory_GetConvertRequest");
            dc.SetParameterValue("@RequestSysNo", requestSysNo);
            return dc.ExecuteEntity<ConvertRequestInfo>();
        }

        /// <summary>
        /// 获取转换单新增的SysNo
        /// </summary>
        /// <returns></returns>
        public int GetConvertRequestSequence()
        {
            var command = DataCommandManager.GetDataCommand("Inventory_GetConvertRequestSequence");
            int result = Convert.ToInt32(command.ExecuteScalar());
            return result;
        }

        /// <summary>
        /// 根据商品编号得到其所属产品线
        /// </summary>
        /// <param name="sysNo"></param>
        /// <returns></returns>
        public virtual ConvertRequestInfo GetProductLineInfo(int productSysNo)
        {
            DataCommand dc = DataCommandManager.GetDataCommand("Inventory_GetConvertProductLineInfoByProductSysNo");
            dc.SetParameterValue("@ProductSysNo", productSysNo);
            return dc.ExecuteEntity<ConvertRequestInfo>();
        }

        /// <summary>
        /// 创建转换单主信息
        /// </summary>
        /// <param name="convertEntity"></param>
        /// <returns></returns>
        public ConvertRequestInfo CreateConvertRequest(ConvertRequestInfo entity)
        {
            var command = DataCommandManager.GetDataCommand("Inventory_CreateConvertRequest");

            command.SetParameterValue("@RequestSysNo", entity.SysNo.Value);
            command.SetParameterValue("@RequestID", entity.RequestID);
            command.SetParameterValue("@StockSysNo", entity.Stock.SysNo);
            command.SetParameterValue("@CreateDate", entity.CreateDate);
            command.SetParameterValue("@CreateUserSysNo", entity.CreateUser.SysNo.Value);
            command.SetParameterValue("@RequestStatus", (int?)entity.RequestStatus);
            command.SetParameterValue("@Note", entity.Note);
            command.SetParameterValue("@CompanyCode", "8601");//[Mark][Alan.X.Luo 硬编码]
            command.SetParameterValue("@ProductLineSysno", entity.ProductLineSysno);
            return command.ExecuteEntity<ConvertRequestInfo>();
        }

        /// <summary>
        /// 更新转换单信息
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public ConvertRequestInfo UpdateConvertRequest(ConvertRequestInfo entity)
        {
            var command = DataCommandManager.GetDataCommand("Inventory_UpdateConvertRequest");
            command.SetParameterValue("@RequestSysNo", entity.SysNo.Value);
            command.SetParameterValue("@Note", entity.Note);
            command.SetParameterValue("@ProductLineSysno", entity.ProductLineSysno);
            return command.ExecuteEntity<ConvertRequestInfo>();
        }
        
        /// <summary>
        /// 更新转换单状态
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public virtual ConvertRequestInfo UpdateConvertRequestStatus(ConvertRequestInfo entity)
        {
            DataCommand dc = DataCommandManager.GetDataCommand("Inventory_UpdateConvertRequestStatus");
            dc.SetParameterValue("@RequestSysNo", entity.SysNo);
            dc.SetParameterValue("@RequestStatus", (int)entity.RequestStatus);
            dc.SetParameterValue("@AuditDate", entity.AuditDate);
            dc.SetParameterValue("@AuditUserSysNo", entity.AuditUser.SysNo);
            dc.SetParameterValue("@OutStockDate", entity.OutStockDate);
            dc.SetParameterValue("@OutStockUserSysNo", entity.OutStockUser.SysNo);

            return dc.ExecuteEntity<ConvertRequestInfo>();
        }

        #endregion 转换单维护 

        #region 转换商品维护

        /// <summary>
        /// 根据借货单SysNo获取转换商品信息
        /// </summary>
        /// <param name="requestSysNo"></param>
        /// <returns></returns>
        public virtual List<ConvertRequestItemInfo> GetConvertItemListByRequestSysNo(int requestSysNo)
        {
            DataCommand dc = DataCommandManager.GetDataCommand("Inventory_GetConvertItemListByRequestSysNo");
            dc.SetParameterValue("@RequestSysNo", requestSysNo);

            using (IDataReader reader = dc.ExecuteDataReader())
            {
                var list = DataMapper.GetEntityList<ConvertRequestItemInfo, List<ConvertRequestItemInfo>>(reader);
                return list;
            }
        }

        /// <summary>
        /// 创建转换单Item
        /// </summary>
        /// <param name="convertItem"></param>
        /// <param name="requestSysNo"></param>        
        /// <returns></returns>
        /// 
        public virtual ConvertRequestItemInfo CreateConvertItem(ConvertRequestItemInfo convertItem, int requestSysNo)
        {
            DataCommand dc = DataCommandManager.GetDataCommand("Inventory_CreateConvertItem");

            dc.SetParameterValue("@RequestSysNo", requestSysNo);
            dc.SetParameterValue("@ProductSysNo", convertItem.ConvertProduct.SysNo);
            dc.SetParameterValue("@ConvertQuantity", convertItem.ConvertQuantity);
            dc.SetParameterValue("@ConvertUnitCost", convertItem.ConvertUnitCost);
            dc.SetParameterValue("@ConvertUnitCostWithoutTax", convertItem.ConvertUnitCostWithoutTax);
            dc.SetParameterValue("@ConvertType", convertItem.ConvertType);

            return dc.ExecuteEntity<ConvertRequestItemInfo>();
        }

        /// <summary>
        /// 删除所有转换单Item
        /// </summary>        
        /// <param name="requestSysNo"></param>
        /// <returns></returns>
        /// 
        public void DeleteConvertItemByRequestSysNo(int requestSysNo)
        {
            DataCommand command = DataCommandManager.GetDataCommand("Inventory_DeleteConvertItemByRequestSysNo");
            command.SetParameterValue("@RequestSysNo", requestSysNo);
            command.ExecuteNonQuery();
        }     

        #endregion 转换商品维护           

    }
}
