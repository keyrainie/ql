using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

using ECCentral.BizEntity.MKT;
using ECCentral.Service.MKT.IDataAccess;
using ECCentral.Service.Utility;
using ECCentral.Service.Utility.DataAccess;

namespace ECCentral.Service.MKT.SqlDataAccess.Promotion
{
    [VersionExport(typeof(IOptionalAccessoriesDA))]
    public class OptionalAccessoriesDA : IOptionalAccessoriesDA
    { 
        /// <summary>
        /// 加载OptionalAccessories所有信息
        /// </summary>
        /// <param name="sysNo"></param>
        /// <returns></returns>
        public virtual OptionalAccessoriesInfo Load(int sysNo)
        {
            OptionalAccessoriesInfo info = null;
            DataCommand cmd = DataCommandManager.GetDataCommand("LoadOptionalAccessoriesInfo");
            cmd.SetParameterValue("@SysNo", sysNo);
            DataSet ds = cmd.ExecuteDataSet();
            DataTable dtMaster = ds.Tables[0];
            DataTable dtItem = ds.Tables[1];
            if (dtMaster == null || dtMaster.Rows.Count == 0)
            {
                return null;
            }

            info = DataMapper.GetEntity<OptionalAccessoriesInfo>(dtMaster.Rows[0], (row, entity) =>
                {
                    entity.Name = new BizEntity.LanguageContent(row["SaleRuleName"].ToString().Trim());
                    entity.TargetStatus = entity.Status;
                    entity.IsShowName = row["IsShow"].ToString().Trim().ToUpper() == "Y" ? true : false;
                    
                });

            info.Items = DataMapper.GetEntityList<OptionalAccessoriesItem, List<OptionalAccessoriesItem>>(dtItem.Rows, (row, entity) =>
                {
                    entity.IsMasterItemB = row["IsMasterItem"] == null ? false : (row["IsMasterItem"].ToString() == "1" ? true : false);                   
                });

            return info;
        }

        /// <summary>
        /// 获取随心配（ReferenceType为3）中，有效状态（为0）的所有item
        /// </summary>
        /// <param name="productSysNo"></param>
        /// <returns></returns>
        public List<OptionalAccessoriesItem> GetOptionalAccessoriesItemListByProductSysNo(int productSysNo)
        {
            List<OptionalAccessoriesItem> result = new List<OptionalAccessoriesItem>();
            DataCommand cmd = DataCommandManager.GetDataCommand("GetOptionalAccessoriesItemListByProductSysNo");
            cmd.SetParameterValue("@ProductSysNo", productSysNo);
            DataSet ds = cmd.ExecuteDataSet();
            DataTable dtItem = ds.Tables[0];
            if (dtItem != null && dtItem.Rows.Count > 0)
            {
                result = DataMapper.GetEntityList<OptionalAccessoriesItem, List<OptionalAccessoriesItem>>(dtItem.Rows, (row, entity) =>
                {
                    entity.IsMasterItemB = row["IsMasterItem"] == null ? false : (row["IsMasterItem"].ToString() == "1" ? true : false);
                });
            }

            return result;
        }

        /// <summary>
        /// 获取随心配（ReferenceType为3）中，有效状态（为0）的所有item
        /// </summary>
        /// <param name="productSysNo"></param>
        /// <returns></returns>
        public List<OptionalAccessoriesItem> GetOptionalAccessoriesItemListByProductSysNo(int productSysNo, int oaiSysNo)
        {
            List<OptionalAccessoriesItem> result = new List<OptionalAccessoriesItem>();
            DataCommand cmd = DataCommandManager.GetDataCommand("GetOptionalAccessoriesItemListByProductSysNoOROaiSysNo");
            cmd.SetParameterValue("@ProductSysNo", productSysNo);
            cmd.SetParameterValue("@SysNo", oaiSysNo);
            DataSet ds = cmd.ExecuteDataSet();
            DataTable dtItem = ds.Tables[0];
            if (dtItem != null && dtItem.Rows.Count > 0)
            {
                result = DataMapper.GetEntityList<OptionalAccessoriesItem, List<OptionalAccessoriesItem>>(dtItem.Rows, (row, entity) =>
                {
                    entity.IsMasterItemB = row["IsMasterItem"] == null ? false : (row["IsMasterItem"].ToString() == "1" ? true : false);
                });
            }

            return result;
        }

        public List<OptionalAccessoriesItem> GetOptionalAccessoriesItemListByOASysNo(int oaSysNo)
        {
            List<OptionalAccessoriesItem> result = new List<OptionalAccessoriesItem>();
            DataCommand cmd = DataCommandManager.GetDataCommand("GetOptionalAccessoriesItemListByOASysNo");
            cmd.SetParameterValue("@OptionalAccessoriesSysNo", oaSysNo);
            DataSet ds = cmd.ExecuteDataSet();
            DataTable dtItem = ds.Tables[0];
            if (dtItem != null && dtItem.Rows.Count > 0)
            {
                result = DataMapper.GetEntityList<OptionalAccessoriesItem, List<OptionalAccessoriesItem>>(dtItem.Rows, (row, entity) =>
                {
                    entity.IsMasterItemB = row["IsMasterItem"] == null ? false : (row["IsMasterItem"].ToString() == "1" ? true : false);
                });
            }

            return result;
        }

        public virtual int CreateMaster(OptionalAccessoriesInfo info)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("CreateCombo");
            cmd.SetParameterValue("@SaleRuleName", info.Name.Content);
            cmd.SetParameterValue("@SaleRuleType", info.SaleRuleType);
            cmd.SetParameterValue("@CreateUserSysNo", ServiceContext.Current.UserSysNo);
            cmd.SetParameterValue("@Status", info.Status);
            cmd.SetParameterValue("@IsShow", info.IsShowName.Value ? "Y" : "N");
            cmd.SetParameterValue("@Priority", info.Priority);
            cmd.SetParameterValue("@CompanyCode", info.CompanyCode);
            cmd.SetParameterValue("@ReferenceSysNo", info.ReferenceSysNo ?? 0);
            cmd.SetParameterValue("@ReferenceType", info.ReferenceType ?? 3);
            cmd.SetParameterValue("@Reason", ""); //销售规则管理加了该字段，用的同一个sql文，所以默认该字段为null
            cmd.ExecuteNonQuery();
            info.SysNo = (int)cmd.GetParameterValue("@SysNo");
            return info.SysNo.Value;
        }

        public virtual void UpdateMaster(OptionalAccessoriesInfo info)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("UpdateCombo");
            cmd.SetParameterValue("@SysNo", info.SysNo);
            cmd.SetParameterValue("@SaleRuleName", info.Name.Content);
            cmd.SetParameterValue("@Status", info.TargetStatus);
            cmd.SetParameterValue("@IsShow", info.IsShowName.Value ? "Y" : "N");
            cmd.SetParameterValue("@Priority", info.Priority);
            cmd.SetParameterValue("@SaleRuleType", info.SaleRuleType);
            cmd.SetParameterValue("@Reason", "");//销售规则管理加了该字段，用的同一个sql文，所以默认该字段为null
            cmd.ExecuteNonQuery();
        }

        public virtual void UpdateStatus(int? sysNo, ComboStatus targetStatus)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("UpdateStatus");
            cmd.SetParameterValue("@SysNo", sysNo); 
            cmd.SetParameterValue("@Status", targetStatus);
            cmd.ExecuteNonQuery();
        }

        public virtual int AddOptionalAccessoriesItem(OptionalAccessoriesItem item)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("AddOptionalAccessoriesItem");
            cmd.SetParameterValue("@SaleRuleSysNo", item.OptionalAccessoriesSysNo);
            cmd.SetParameterValue("@ProductSysNo", item.ProductSysNo);
            cmd.SetParameterValue("@Quantity", item.Quantity);
            cmd.SetParameterValue("@Discount", item.Discount);
            cmd.SetParameterValue("@IsMasterItem", item.IsMasterItemB.Value ? 1 : 0);
            cmd.SetParameterValue("@DiscountPercent", item.DiscountPercentVal);
            cmd.SetParameterValue("@Priority", item.Priority);
            cmd.ExecuteNonQuery();
            item.SysNo = (int)cmd.GetParameterValue("@SysNo");
            return item.SysNo.Value;

        }

        public void DeleteOptionalAccessoriesAllItem(int optionalAccessoriesSysNo)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("DeleteComboAllItem");
            cmd.SetParameterValue("@ComboSysNo", optionalAccessoriesSysNo);
            cmd.ExecuteNonQuery();
        }

    }
}
