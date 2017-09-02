using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using ECCentral.BizEntity.IM;
using ECCentral.BizEntity.PO;
using ECCentral.Service.PO.IDataAccess;
using ECCentral.Service.Utility;
using ECCentral.Service.Utility.DataAccess;
using ECCentral.BizEntity.Common;

namespace ECCentral.Service.PO.SqlDataAccess
{
    [VersionExport(typeof(ICostChangeDA))]
    public class CostChangeDA : ICostChangeDA
    {
        public CostChangeBasicInfo LoadCostChangeBasicInfo(int ccSysNo)
        {
            CostChangeBasicInfo returnEntity = new CostChangeBasicInfo();
            CustomDataCommand command = DataCommandManager.CreateCustomDataCommandFromConfig("GetCostChangeBySysNo");

            using (DynamicQuerySqlBuilder builder = new DynamicQuerySqlBuilder(command.CommandText, command, null, "CC.SysNo DESC"))
            {
                builder.ConditionConstructor.AddCondition(
                    QueryConditionRelationType.AND,
                    "CC.SysNo",
                    DbType.Int32,
                    "@CostChangeSysNo",
                    QueryConditionOperatorType.Equal,
                    ccSysNo);

                command.CommandText = builder.BuildQuerySql();
                returnEntity = command.ExecuteEntity<CostChangeBasicInfo>();
            }
            
            return returnEntity;
        }

        public List<CostChangeItemsInfo> LoadCostChangeItemList(int ccSysNo)
        {
            List<CostChangeItemsInfo> returnList = new List<CostChangeItemsInfo>();
            DataCommand commandItem = DataCommandManager.GetDataCommand("GetCostChangeItemsByCCSysNo");

            commandItem.SetParameterValue("@CostChangeSysNo", ccSysNo);
            returnList = commandItem.ExecuteEntityList<CostChangeItemsInfo>();
            return returnList;
        }

        public void DeleteCostChangeItems(CostChangeItemsInfo deleteItemInfo)
        {
            List<CostChangeItemsInfo> returnList = new List<CostChangeItemsInfo>();
            DataCommand commandItem = DataCommandManager.GetDataCommand("DeleteCostChangeItems");

            commandItem.SetParameterValue("@ItemSysNo", deleteItemInfo.ItemSysNo);
            commandItem.ExecuteNonQuery();
        }

        public CostChangeInfo UpdateCostChange(CostChangeInfo costChangeInfo)
        {
            CostChangeInfo returnEntity = costChangeInfo;
            DataCommand command = DataCommandManager.GetDataCommand("UpdateCostChangeMaster");

            command.SetParameterValue("@CCSysNo", costChangeInfo.SysNo);
            command.SetParameterValue("@VendorSysNo", costChangeInfo.CostChangeBasicInfo.VendorSysNo);
            command.SetParameterValue("@PMSysNo", costChangeInfo.CostChangeBasicInfo.PMSysNo);
            command.SetParameterValue("@EditUser", costChangeInfo.CostChangeBasicInfo.EditUser);
            command.SetParameterValue("@Memo", costChangeInfo.CostChangeBasicInfo.Memo);
            command.SetParameterValue("@TotalDiffAmt", costChangeInfo.CostChangeBasicInfo.TotalDiffAmt);
            command.ExecuteNonQuery();

            if (costChangeInfo.CostChangeItems != null && costChangeInfo.CostChangeItems.Count > 0)
            {
                foreach (CostChangeItemsInfo item in costChangeInfo.CostChangeItems)
                {
                    if (item.ItemActionStatus==ItemActionStatus.Update)//更新
                    {
                        command = DataCommandManager.GetDataCommand("UpdateCostChangeItem");

                        command.SetParameterValue("@ItemSysNo", item.ItemSysNo.Value);
                        command.SetParameterValue("@NewPrice", item.NewPrice);
                        command.SetParameterValue("@ChangeCount", item.ChangeCount);
                        command.ExecuteNonQuery();
                    }
                    else if(item.ItemActionStatus==ItemActionStatus.Add) //新建
                    {
                        CreateCostChangeItemInfo(item, costChangeInfo.SysNo.Value);
                    }
                }
            }

            return costChangeInfo;
        }

        public CostChangeInfo CreateCostChange(CostChangeInfo costChangeInfo)
        {
            DataCommand command = DataCommandManager.GetDataCommand("CreateCostChange");
            command.SetParameterValue("@VendorSysNo", costChangeInfo.CostChangeBasicInfo.VendorSysNo);
            command.SetParameterValue("@PMSysNo", costChangeInfo.CostChangeBasicInfo.PMSysNo);
            command.SetParameterValue("@Status", (int)costChangeInfo.CostChangeBasicInfo.Status);
            command.SetParameterValue("@Memo", costChangeInfo.CostChangeBasicInfo.Memo);
            command.SetParameterValue("@InUser", costChangeInfo.CostChangeBasicInfo.InUser);
            command.SetParameterValue("@TotalDiffAmt", costChangeInfo.CostChangeBasicInfo.TotalDiffAmt);
            command.SetParameterValue("@CompanyCode", "8601");
            costChangeInfo.SysNo =Convert.ToInt32(command.ExecuteScalar());

            if (costChangeInfo.CostChangeItems != null && costChangeInfo.CostChangeItems.Count > 0)
            {
                foreach (CostChangeItemsInfo item in costChangeInfo.CostChangeItems)
                {
                    CreateCostChangeItemInfo(item, costChangeInfo.SysNo.Value);
                }
            }

            return costChangeInfo;
        }

        public CostChangeItemsInfo CreateCostChangeItemInfo(CostChangeItemsInfo costChangeItem, int ccSysNo)
        {
            DataCommand command = null;

            command = DataCommandManager.GetDataCommand("CreateCostChangeItemInfo");

            command.SetParameterValue("@CostChangeSysNo", ccSysNo);
            command.SetParameterValue("@ProductSysNo", costChangeItem.ProductSysNo);
            command.SetParameterValue("@POSysNo", costChangeItem.POSysNo);
            command.SetParameterValue("@OldPrice", costChangeItem.OldPrice);
            command.SetParameterValue("@NewPrice", costChangeItem.NewPrice);
            command.SetParameterValue("@ChangeCount", costChangeItem.ChangeCount);
            command.SetParameterValue("@CompanyCode", costChangeItem.CompanyCode);
            costChangeItem.ItemSysNo = System.Convert.ToInt32(command.ExecuteScalar());

            return costChangeItem;
        }

        public CostChangeInfo UpdateCostChangeStatus(CostChangeInfo costChangeInfo)
        {
            CostChangeInfo returnEntity = costChangeInfo;
            DataCommand commandItem = DataCommandManager.GetDataCommand("UpdateCostChangeStatus");

            commandItem.SetParameterValue("@CCSysNo", costChangeInfo.SysNo);
            commandItem.SetParameterValue("@Status", (int)costChangeInfo.CostChangeBasicInfo.Status);
            commandItem.ExecuteNonQuery();
            return costChangeInfo;
        }

        public CostChangeInfo UpdateCostChangeAuditStatus(CostChangeInfo costChangeInfo)
        {
            CostChangeInfo returnEntity = costChangeInfo;
            DataCommand commandItem = DataCommandManager.GetDataCommand("UpdateCostChangeAuditStatus");

            commandItem.SetParameterValue("@CCSysNo", costChangeInfo.SysNo);
            commandItem.SetParameterValue("@AuditUser", ServiceContext.Current.UserSysNo);
            commandItem.SetParameterValue("@AuditMemo", costChangeInfo.CostChangeBasicInfo.AuditMemo);
            commandItem.SetParameterValue("@Status", (int)costChangeInfo.CostChangeBasicInfo.Status);
            commandItem.ExecuteNonQuery();
            return costChangeInfo;
        }
    }
}
