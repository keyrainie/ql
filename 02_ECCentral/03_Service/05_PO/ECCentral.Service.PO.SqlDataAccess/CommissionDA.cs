using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.Utility;
using ECCentral.Service.PO.IDataAccess;
using ECCentral.Service.Utility.DataAccess;
using ECCentral.BizEntity.PO;
using System.Data;
using System.IO;
using System.Xml.Serialization;

namespace ECCentral.Service.PO.SqlDataAccess
{
    [VersionExport(typeof(ICommissionDA))]
    public class CommissionDA : ICommissionDA
    {
        #region ICommissionDA Members

        public int CloseCommission(BizEntity.PO.CommissionMaster commissionMaster)
        {
            DataCommand command = DataCommandManager.GetDataCommand("CloseCommission");
            command.SetParameterValue("@SysNo", commissionMaster.SysNo.Value);

            return command.ExecuteNonQuery();
            
        }

        public BizEntity.PO.CommissionRule CreateCommission(BizEntity.PO.CommissionRule newCommissionRule)
        {
            DataCommand command = DataCommandManager.GetDataCommand("InsertCommissionRule");
            command.SetParameterValue("@BrandSysNo", newCommissionRule.BrandSysNo);
            command.SetParameterValue("@ManufacturerSysNo", newCommissionRule.ManufacturerSysNo);
            command.SetParameterValue("@CostValue", newCommissionRule.CostValue);
            command.SetParameterValue("@RuleType", newCommissionRule.RuleType);
            command.SetParameterValue("@IsDefaultRule", newCommissionRule.IsDefaultRule);
            command.SetParameterValue("@Level", newCommissionRule.Level);
            command.SetParameterValue("@InUser", newCommissionRule.InUser);
            command.SetParameterValue("@EditUser", newCommissionRule.EditUser);

            #region [根据佣金等级获取类别编号:]
            int? categorySysNo = null;
            switch (newCommissionRule.Level)
            {
                case "0":
                    categorySysNo = null;
                    break;
                case "1":
                    categorySysNo = newCommissionRule.C1SysNo;
                    break;
                case "2":
                    categorySysNo = newCommissionRule.C2SysNo;
                    break;
                case "3":
                    categorySysNo = newCommissionRule.C3SysNo;
                    break;

            }
            #endregion

            command.SetParameterValue("@CategorySysNo", categorySysNo);
            command.ExecuteNonQuery();
            return newCommissionRule;
        }


        public BizEntity.PO.CommissionMaster LoadCommissionMaster(int commissionSysNo)
        {
            DataCommand command = DataCommandManager.GetDataCommand("GetCommissionBySysNo");
            command.SetParameterValue("@SysNo", commissionSysNo);

            return command.ExecuteEntity<CommissionMaster>();
        }

        public List<CommissionItem> LoadCommissionItems(int commissionSysNo)
        {
            List<CommissionItem> result = new List<CommissionItem>();

            CustomDataCommand dataCommand = DataCommandManager.CreateCustomDataCommandFromConfig("GetCommissionItemByCommissionSysNo");
            dataCommand.SetParameterValue("@CommissionMasterSysNo", commissionSysNo);
            result = dataCommand.ExecuteEntityList<CommissionItem>((s, t) =>
            {
                if (null != s["SalesRule"] && !string.IsNullOrEmpty(s["SalesRule"].ToString()))
                {
                    t.SaleRule = SerializationUtility.XmlDeserialize<VendorStagedSaleRuleEntity>(s["SalesRule"].ToString());
                }
            });
            return result;
        }

        public List<CommissionItemDetail> LoadCommissionItemDetails(int commissionSysNo, VendorCommissionItemType type)
        {
            List<CommissionItemDetail> result = new List<CommissionItemDetail>();

            CustomDataCommand dataCommand = DataCommandManager.CreateCustomDataCommandFromConfig("GetCommissionDetailByItemSysNoList");
            dataCommand.SetParameterValue("@Type", type.ToString());
            dataCommand.CommandText = dataCommand.CommandText.Replace("#CommissionItemList#", commissionSysNo.ToString());
            result = dataCommand.ExecuteEntityList<CommissionItemDetail>();
            return result;
        }

        public List<CommissionItemDetail> QueryCommissionItemDetails(int vendorSysNo, DateTime startDate, DateTime endDate, string companyCode)
        {
            CustomDataCommand command = DataCommandManager.CreateCustomDataCommandFromConfig("QueryManualSettleCommissionDetails");
            command.CommandTimeout = 120;

            string payType = AppSettingManager.GetSetting("PO", "ManualSettleCommissionPayType");
            command.CommandText = command.CommandText.Replace("#PayType#", payType);

            command.AddInputParameter("@VendorSysNo", DbType.Int32, vendorSysNo);
            command.AddInputParameter("@CompanyCode", DbType.AnsiStringFixedLength, companyCode);
            command.AddInputParameter("@StartDate", DbType.DateTime, startDate);
            command.AddInputParameter("@EndDate", DbType.DateTime, endDate);

            return command.ExecuteEntityList<CommissionItemDetail>();
        }

        public List<VendorCommissionInfo> QueryCommissionRuleByMerchantSysNo(int vendorSysNo)
        {
            CustomDataCommand dataCommand = DataCommandManager.CreateCustomDataCommandFromConfig("QueryCommissionRuleByMerchantSysNo");

            dataCommand.AddInputParameter("@MerchantSysNo", DbType.Int32, vendorSysNo);

            return dataCommand.ExecuteEntityList<VendorCommissionInfo>();
        }

        public CommissionItem QueryVendorManufacturerBySysNo(int vendorManufacturerSysNo)
        {
            CustomDataCommand dataCommand = DataCommandManager.CreateCustomDataCommandFromConfig("QueryVendorManufacturerBySysNo");

            dataCommand.AddInputParameter("@SysNo", DbType.Int32, vendorManufacturerSysNo);

            return dataCommand.ExecuteEntity<CommissionItem>();
        }

        public void InsertCommissionMaster(CommissionMaster req)
        {
            DataCommand command = DataCommandManager.GetDataCommand("InsertCommissionMaster");
            command.SetParameterValue<CommissionMaster>(req, true, false);
            command.SetParameterValue("@InUser", ServiceContext.Current.UserSysNo);
            req.SysNo = command.ExecuteScalar<int>();
        }

        public void InsertCommissionItems(CommissionMaster req)
        {
            foreach (var item in req.ItemList)
            {
                if (item.RuleSysNo == 0)
                {
                    item.DeliveryFee = item.TotalSaleAmt = item.OrderCommissionFee = item.SalesCommissionFee = item.RentFee = 0;
                }
                DataCommand command = DataCommandManager.GetDataCommand("InsertCommissionItem");
                command.SetParameterValue<CommissionItem>(item, true, false);
                command.SetParameterValue("@InUser", ServiceContext.Current.UserSysNo);
                command.SetParameterValue("@CompanyCode", req.CompanyCode);
                command.SetParameterValue("@CommissionMasterSysNo", req.SysNo);
                item.ItemSysNo = command.ExecuteScalar<int>();
            }
        }

        public void InsertCommissionDetail(CommissionItemDetail req,string companyCode,VendorCommissionItemType type)
        {
            DataCommand command = DataCommandManager.GetDataCommand("InsertCommissionDetail");
            command.SetParameterValue<CommissionItemDetail>(req, true, false);
            
            command.SetParameterValue("@InUser", ServiceContext.Current.UserSysNo);
            command.SetParameterValue("@CompanyCode", companyCode);
            command.SetParameterValue("@Type", type);
            command.ExecuteNonQuery();
        }

        #endregion
    }
}
