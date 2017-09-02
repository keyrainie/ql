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
    [VersionExport(typeof(IComboDA))]
    public class ComboDA : IComboDA
    { 
        /// <summary>
        /// 加载Combo所有信息
        /// </summary>
        /// <param name="sysNo"></param>
        /// <returns></returns>
        public virtual  ComboInfo Load(int sysNo)
        {
            ComboInfo info = null;
            DataCommand cmd = DataCommandManager.GetDataCommand("LoadComboInfo");
            cmd.SetParameterValue("@SysNo", sysNo);
            DataSet ds = cmd.ExecuteDataSet();
            DataTable dtMaster = ds.Tables[0];
            DataTable dtItem = ds.Tables[1];
            if (dtMaster == null || dtMaster.Rows.Count == 0)
            {
                return null;
            }

            info = DataMapper.GetEntity<ComboInfo>(dtMaster.Rows[0],(row,entity)=>
                {
                    entity.Name = new BizEntity.LanguageContent(row["SaleRuleName"].ToString().Trim());
                    entity.TargetStatus = entity.Status;
                    entity.IsShowName = row["IsShow"].ToString().Trim().ToUpper() == "Y" ? true : false;
                    
                });

            info.Items = DataMapper.GetEntityList<ComboItem,List<ComboItem>>(dtItem.Rows, (row, entity) =>
                {
                    entity.IsMasterItemB = row["IsMasterItem"] == null ? false : (row["IsMasterItem"].ToString() == "1" ? true : false);                   
                });

            return info;
        }

        public virtual  int CreateMaster(ComboInfo info)
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
            cmd.SetParameterValue("@ReferenceType", info.ReferenceType ?? 1);
            cmd.SetParameterValue("@Reason", info.Reason);
            cmd.SetParameterValue("@CreateUserName", ServiceContext.Current.UserDisplayName);

            cmd.ExecuteNonQuery();
            info.SysNo = (int)cmd.GetParameterValue("@SysNo");
            return info.SysNo.Value;
        }

        public virtual  void UpdateMaster(ComboInfo info)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("UpdateCombo");
            cmd.SetParameterValue("@SysNo", info.SysNo);
            cmd.SetParameterValue("@SaleRuleName", info.Name.Content);
            cmd.SetParameterValue("@Status", info.TargetStatus);
            cmd.SetParameterValue("@IsShow", info.IsShowName.Value ? "Y" : "N");
            cmd.SetParameterValue("@Priority", info.Priority);
            cmd.SetParameterValue("@SaleRuleType", info.SaleRuleType);
            cmd.SetParameterValue("@Reason", info.Reason);
            cmd.ExecuteNonQuery();
        }

        public virtual void UpdateStatus(int? sysNo, ComboStatus targetStatus)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("UpdateStatus");
            cmd.SetParameterValue("@SysNo", sysNo); 
            cmd.SetParameterValue("@Status", targetStatus);
            cmd.ExecuteNonQuery();
        }

        public virtual int AddComboItem(ComboItem item)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("AddComboItem");
            cmd.SetParameterValue("@SaleRuleSysNo", item.ComboSysNo);
            cmd.SetParameterValue("@ProductSysNo", item.ProductSysNo);
            cmd.SetParameterValue("@Quantity", item.Quantity);
            cmd.SetParameterValue("@Discount", item.Discount);
            cmd.SetParameterValue("@IsMasterItem", item.IsMasterItemB.Value ? 1 : 0); 
            cmd.ExecuteNonQuery();
            item.SysNo = (int)cmd.GetParameterValue("@SysNo");
            return item.SysNo.Value;

        }
        
        public virtual void DeleteComboAllItem(int comboSysNo)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("DeleteComboAllItem");
            cmd.SetParameterValue("@ComboSysNo", comboSysNo);
            cmd.ExecuteNonQuery();
        }

        public virtual List<ComboInfo> GetComboListForCurrentSO(List<int> productSysNoList)
        { 
            var cmd = DataCommandManager.CreateCustomDataCommandFromConfig("GetComboListForCurrentSO");
            string sql = cmd.CommandText;
            string itemlistString = productSysNoList.Join(",").Trim(',');
            sql = sql.Replace("#SOItemList#", itemlistString);
            cmd.CommandText = sql;
            DataSet  ds = cmd.ExecuteDataSet();
            List<ComboInfo> comboList = new List<ComboInfo>();
            DataTable dtMaster = ds.Tables[0];
            DataTable dtItems = ds.Tables[1];
            if (dtMaster != null && dtMaster.Rows.Count > 0)
            {
                comboList = DataMapper.GetEntityList<ComboInfo, List<ComboInfo>>(dtMaster.Rows);
                List<ComboItem> tempItemList = DataMapper.GetEntityList<ComboItem, List<ComboItem>>(dtItems.Rows,(row, entity) =>
                {
                    entity.IsMasterItemB = row["IsMasterItem"] == null ? false : (row["IsMasterItem"].ToString() == "1" ? true : false);                   
                });
                foreach (ComboInfo combo in comboList)
                {
                    combo.Items = new List<ComboItem>();
                    foreach (ComboItem item in tempItemList)
                    {
                        if (combo.SysNo == item.ComboSysNo)
                        {
                            combo.Items.Add(item);
                        }
                    }
                }
            }
            return comboList;
        }

        public List<ComboInfo> GetComboListByProductSysNo(int productSysNo)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("GetComboListByProductSysNo");
            cmd.SetParameterValue("@ProductSysNo", productSysNo);
            DataSet ds = cmd.ExecuteDataSet();
            List<ComboInfo> comboList = new List<ComboInfo>();
            DataTable dtMaster = ds.Tables[0];
            DataTable dtItems = ds.Tables[1];
            if (dtMaster != null && dtMaster.Rows.Count > 0)
            {
                comboList = DataMapper.GetEntityList<ComboInfo, List<ComboInfo>>(dtMaster.Rows);
                List<ComboItem> tempItemList = DataMapper.GetEntityList<ComboItem, List<ComboItem>>(dtItems.Rows, (row, entity) =>
                {
                    entity.IsMasterItemB = row["IsMasterItem"] == null ? false : (row["IsMasterItem"].ToString() == "1" ? true : false);
                });
                foreach (ComboInfo combo in comboList)
                {
                    combo.Items = new List<ComboItem>();
                    foreach (ComboItem item in tempItemList)
                    {
                        if (combo.SysNo == item.ComboSysNo)
                        {
                            combo.Items.Add(item);
                        }
                    }
                }
            }
            return comboList;
        }

        public virtual int CheckComboExits(string comboName, int productSysNo)
        {
            DataCommand dc = DataCommandManager.GetDataCommand("CheckSaleRuleExits");

            dc.SetParameterValue("@ProductSysNo", productSysNo);
            dc.SetParameterValue("@SaleRuleName", comboName);
            dc.SetParameterValue("@CreateTime", DateTime.Now.Date);
            
            return dc.ExecuteScalar<int>();
        }

        /// <summary>
        /// 同步状态
        /// </summary>
        /// <param name="requestSysNo"></param>
        /// <param name="status"></param>
        public void SyncSaleRuleStatus(int requestSysNo, ComboStatus? status)
        {
            DataCommand command = DataCommandManager.GetDataCommand("SyncSaleRuleStatus");
            command.SetParameterValue("@RequestSysNo", requestSysNo);
            command.SetParameterValue("@Status", status);
            command.ExecuteNonQuery();
        }
    }
}
