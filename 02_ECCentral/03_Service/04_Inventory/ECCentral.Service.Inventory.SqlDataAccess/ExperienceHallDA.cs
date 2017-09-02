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
    [VersionExport(typeof(IExperienceHallDA))]
    public class ExperienceHallDA : IExperienceHallDA
    {
        #region 调拨单维护

        /// <summary>
        /// 根据借货单SysNo获取借货商品信息
        /// </summary>
        /// <param name="requestSysNo"></param>
        /// <returns></returns>
        public virtual List<ExperienceItemInfo> GetExperienceItemListByRequestSysNo(int requestSysNo)
        {
            DataCommand dc = DataCommandManager.GetDataCommand("Inventory_GetExperienceItemListByRequestSysNo");
            dc.SetParameterValue("@RequestSysNo", requestSysNo);

            using (IDataReader reader = dc.ExecuteDataReader())
            {
                var list = DataMapper.GetEntityList<ExperienceItemInfo, List<ExperienceItemInfo>>(reader);
                return list;
            }
        }

        /// <summary>
        /// 根据SysNo获取调拨单信息
        /// </summary>
        /// <param name="requestSysNo"></param>
        /// <returns></returns>
        public virtual ExperienceInfo GetExperienceInfoBySysNo(int requestSysNo)
        {
            DataCommand dc = DataCommandManager.GetDataCommand("Inventory_GetExperienceInfoBySysNo");
            dc.SetParameterValue("@RequestSysNo", requestSysNo);
            return dc.ExecuteEntity<ExperienceInfo>();
        }

        /// <summary>
        /// 创建调拨单信息
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public virtual ExperienceInfo CreateExperience(ExperienceInfo entity)
        {
            DataCommand dc = DataCommandManager.GetDataCommand("Inventory_CreateExperience");

            dc.SetParameterValue("@StockSysNo", entity.StockSysNo);
            dc.SetParameterValue("@Memo", entity.Meno);
            dc.SetParameterValue("@AllocateType", (int)entity.AllocateType);
            dc.SetParameterValue("@Status", (int)entity.Status);
            dc.SetParameterValue("@InDate", entity.InDate);
            dc.SetParameterValue("@InUser", entity.InUser);

            int sysno = dc.ExecuteScalar<int>();
            entity.SysNo = sysno;

            return entity;
        }

        /// <summary>
        /// 删除调拨单中所有商品
        /// </summary>
        /// <param name="requestSysNo"></param>
        /// <returns></returns>
        /// 
        public virtual void DeleteExperienceItemByRequestSysNo(int requestSysNo)
        {
            DataCommand dc = DataCommandManager.GetDataCommand("Inventory_DeleteExperienceItemByRequestSysNo");
            dc.SetParameterValue("@AllocateSysNo", requestSysNo);
            dc.ExecuteNonQuery();
        }

        /// <summary>
        /// 创建商品记录
        /// </summary>
        /// <param name="requestSysNo"></param>
        /// <param name="lendItem"></param>
        /// <returns></returns>
        public virtual ExperienceItemInfo CreateExperienceItem(ExperienceItemInfo lendItem, int requestSysNo)
        {
            DataCommand dc = DataCommandManager.GetDataCommand("Inventory_CreateExperienceItem");

            dc.SetParameterValue("@AllocateSysNo", requestSysNo);
            dc.SetParameterValue("@ProductSysNo", lendItem.ProductSysNo);
            dc.SetParameterValue("@AllocateQty", lendItem.AllocateQty);
            dc.SetParameterValue("@CompanyCode", "8601");//[Mark][Alan.X.Luo 硬编码]

            lendItem.SysNo = dc.ExecuteScalar<int>();
            return lendItem;
        }

        /// <summary>
        /// 更新调拨单信息
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public virtual ExperienceInfo UpdateExperience(ExperienceInfo entity)
        {
            DataCommand dc = DataCommandManager.GetDataCommand("Inventory_UpdateExperience");
            dc.SetParameterValue("@RequestSysNo", entity.SysNo);
            dc.SetParameterValue("@StockSysNo", entity.StockSysNo);
            dc.SetParameterValue("@Memo", entity.Meno);
            dc.SetParameterValue("@AllocateType", (int)entity.AllocateType);
            dc.SetParameterValue("@Status", (int)entity.Status);
            dc.SetParameterValue("@EditUser", entity.EditUser);
            return dc.ExecuteEntity<ExperienceInfo>();
        }

        /// <summary>
        /// 审核时：调入添加库存、调出减少库存
        /// </summary>
        /// <param name="entity"></param>
        public virtual void AuditExperienceInOrOut(ExperienceItemInfo item, int sysno)
        {
            DataCommand dc = DataCommandManager.GetDataCommand("Inventory_AuditExperienceInOrOut");
            dc.SetParameterValue("@CompanyCode", item.CompanyCode);
            dc.SetParameterValue("@ProductSysNo", item.ProductSysNo);
            dc.SetParameterValue("@SysNo", sysno);
            dc.ExecuteNonQuery();
        }

        /// <summary>
        /// 取消审核时：调入减少库存、调出添加库存（要还原）
        /// </summary>
        /// <param name="entity"></param>
        public virtual void CancelAuditExperienceInOrOut(ExperienceItemInfo item, int sysno)
        {
            DataCommand dc = DataCommandManager.GetDataCommand("Inventory_CancelAuditExperienceInOrOut");
            dc.SetParameterValue("@CompanyCode", item.CompanyCode);
            dc.SetParameterValue("@ProductSysNo", item.ProductSysNo);
            dc.SetParameterValue("@SysNo", sysno);
            dc.ExecuteNonQuery();
        }

        /// <summary>
        /// 更新调拨单状态
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public virtual void UpdateExperienceStatus(ExperienceInfo entity, int status)
        {
            DataCommand dc = DataCommandManager.GetDataCommand("Inventory_UpdateExperienceStatus");
            dc.SetParameterValue("@Status", status);
            dc.SetParameterValue("@AuditUser", entity.AuditUser);
            dc.SetParameterValue("@EditUser", entity.EditUser);
            dc.SetParameterValue("@SysNo", entity.SysNo.Value);

            dc.ExecuteNonQuery();
        }

        /// <summary>
        /// 更新调拨单状态
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public virtual int GetValidStockQty(int productSysno)
        {
            DataCommand dc = DataCommandManager.GetDataCommand("Inventory_CheckedStockQty");
            dc.SetParameterValue("@CompanyCode", "8601");
            dc.SetParameterValue("@ProductSysNo", productSysno);

            return dc.ExecuteScalar<int>();
        }

        #endregion 调拨单维护
    }
}
