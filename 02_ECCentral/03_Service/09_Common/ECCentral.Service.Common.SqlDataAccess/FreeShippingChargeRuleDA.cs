using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Transactions;
using ECCentral.BizEntity.Common;
using ECCentral.QueryFilter.Common;
using ECCentral.Service.Common.IDataAccess;
using ECCentral.Service.Common.IDataAccess.NoBizQuery;
using ECCentral.Service.Utility;
using ECCentral.Service.Utility.DataAccess;

namespace ECCentral.Service.Common.SqlDataAccess
{
    [VersionExport(typeof(IFreeShippingChargeRuleDA))]
    public class FreeShippingChargeRuleDA : IFreeShippingChargeRuleDA
    {
        public FreeShippingChargeRuleInfo Load(int sysno)
        {
            DataCommand command = DataCommandManager.GetDataCommand("GetFreeShippingChargeRuleBySysNo");
            command.SetParameterValue("@SysNo", sysno);

            FreeShippingChargeRuleInfo info = command.ExecuteEntity<FreeShippingChargeRuleInfo>(SettingValueMapper);
            this.SetMultipleSettingValue(new List<FreeShippingChargeRuleInfo>() { info });

            return info;
        }

        public FreeShippingChargeRuleInfo Create(FreeShippingChargeRuleInfo entity)
        {
            using (TransactionScope ts = new TransactionScope())
            {
                entity = CreateMasterInfo(entity);
                if (!entity.IsGlobal)
                {
                    UpdateProductSettingValue(entity);
                }
                ts.Complete();
            }
            LoadRuleProductSettingValue(entity);

            return entity;
        }

        public void UpdateInfo(FreeShippingChargeRuleInfo entity)
        {
            using (TransactionScope ts = new TransactionScope())
            {
                UpdateMasterInfo(entity);
                if (!entity.IsGlobal)
                {
                    UpdateProductSettingValue(entity);
                }
                ts.Complete();
            }
        }

        public void UpdateStatus(FreeShippingChargeRuleInfo entity)
        {
            DataCommand command = DataCommandManager.GetDataCommand("UpdateFreeShippingChargeRuleStatus");
            command.SetParameterValue("@SysNo", entity.SysNo);
            command.SetParameterValue("@Status", entity.Status);

            command.ExecuteNonQuery();
        }

        public void Delete(int sysno)
        {
            using (TransactionScope ts = new TransactionScope())
            {
                DataCommand command = DataCommandManager.GetDataCommand("DeleteFreeShippingChargeRule");
                command.SetParameterValue("@SysNo", sysno);
                command.ExecuteNonQuery();

                DeleteRuleAllProductSettingValue(sysno);

                ts.Complete();
            }
        }

        public List<FreeShippingChargeRuleInfo> GetAllByStatus(FreeShippingAmountSettingStatus status)
        {
            DataCommand command = DataCommandManager.GetDataCommand("GetAllFreeShippingChargeRuleByStatus");
            command.SetParameterValue("@Status", status);

            List<FreeShippingChargeRuleInfo> list = command.ExecuteEntityList<FreeShippingChargeRuleInfo>(SettingValueMapper);
            this.SetMultipleSettingValue(list);

            return list;
        }

        private void SettingValueMapper(DbDataReader reader, FreeShippingChargeRuleInfo entity)
        {
            if (!(reader["PayTypeSettingValueStr"] is DBNull) && reader["PayTypeSettingValueStr"] != null)
            {
                entity.PayTypeSettingValue = new List<SimpleObject>();
                foreach (string key in Convert.ToString(reader["PayTypeSettingValueStr"]).Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    entity.PayTypeSettingValue.Add(new SimpleObject() { ID = key });
                }
            }
            if (!(reader["ShipAreaSettingValueStr"] is DBNull) && reader["ShipAreaSettingValueStr"] != null)
            {
                entity.ShipAreaSettingValue = new List<SimpleObject>();
                foreach (string key in Convert.ToString(reader["ShipAreaSettingValueStr"]).Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    entity.ShipAreaSettingValue.Add(new SimpleObject() { ID = key });
                }
            }
        }

        private void SetMultipleSettingValue(List<FreeShippingChargeRuleInfo> list)
        {
            if (HasElements(list))
            {
                List<AreaInfo> allProvinceList = ObjectFactory<IAreaDA>.Instance.QueryProvinceAreaList();

                int totalCount;
                List<PayType> allPayTypeList = null;
                DataTable allPayTypeDT = ObjectFactory<IPayTypeQueryDA>.Instance.QueryPayType(new PayTypeQueryFilter()
                {
                    PagingInfo = new PagingInfo() {  PageIndex = 0, PageSize = 1000 }
                }, out totalCount);
                if (allPayTypeDT != null && totalCount > 0)
                {
                    allPayTypeList = DataMapper.GetEntityList<PayType, List<PayType>>(allPayTypeDT.Rows);
                }

                foreach (var item in list)
                {
                    if (HasElements(item.ShipAreaSettingValue) && HasElements(allProvinceList))
                    {
                        foreach (var simpleObj in item.ShipAreaSettingValue)
                        {
                            var p = allProvinceList.Find(x => x.SysNo.ToString() == simpleObj.ID);
                            if (p != null)
                            {
                                simpleObj.Name = p.ProvinceName;
                            }
                        }
                    }
                    if (HasElements(item.PayTypeSettingValue) && HasElements(allPayTypeList))
                    {
                        foreach (var simpleObj in item.PayTypeSettingValue)
                        {
                            var p = allPayTypeList.Find(x => x.SysNo.ToString() == simpleObj.ID);
                            if (p != null)
                            {
                                simpleObj.Name = p.PayTypeName;
                            }
                        }
                    }
                    if (!item.IsGlobal)
                    {
                        this.LoadRuleProductSettingValue(item);
                    }
                }
            }
        }

        private FreeShippingChargeRuleInfo CreateMasterInfo(FreeShippingChargeRuleInfo entity)
        {
            DataCommand command = DataCommandManager.GetDataCommand("InsertFreeShippingChargeRule");
            command.SetParameterValue("@StartDate", entity.StartDate);
            command.SetParameterValue("@EndDate", entity.EndDate);
            command.SetParameterValue("@Status", entity.Status);
            command.SetParameterValue("@AmountSettingType", entity.AmountSettingType);
            command.SetParameterValue("@AmountSettingValue", entity.AmountSettingValue);
            command.SetParameterValue("@IsGlobal", entity.IsGlobal);
            command.SetParameterValue("@Description", entity.Description);

            string payTypeSettingValue = string.Empty;
            if (entity.PayTypeSettingValue != null)
            {
                payTypeSettingValue = ECCentral.Service.Utility.StringUtility.Join(entity.PayTypeSettingValue, ",");
            }
            command.SetParameterValue("@PayTypeSettingValue", payTypeSettingValue);

            string shipAreaSettingValue = string.Empty;
            if (entity.ShipAreaSettingValue != null)
            {
                shipAreaSettingValue = ECCentral.Service.Utility.StringUtility.Join(entity.ShipAreaSettingValue, ",");
            }
            command.SetParameterValue("@ShipAreaSettingValue", shipAreaSettingValue);

            entity.SysNo = Convert.ToInt32(command.ExecuteScalar());

            return entity;
        }

        private void UpdateMasterInfo(FreeShippingChargeRuleInfo entity)
        {
            DataCommand command = DataCommandManager.GetDataCommand("UpdateFreeShippingChargeRule");
            command.SetParameterValue("@SysNo", entity.SysNo);
            command.SetParameterValue("@StartDate", entity.StartDate);
            command.SetParameterValue("@EndDate", entity.EndDate);
            command.SetParameterValue("@AmountSettingType", entity.AmountSettingType);
            command.SetParameterValue("@AmountSettingValue", entity.AmountSettingValue);
            command.SetParameterValue("@IsGlobal", entity.IsGlobal);
            command.SetParameterValue("@Description", entity.Description);

            string payTypeSettingValue = string.Empty;
            if (entity.PayTypeSettingValue != null)
            {
                payTypeSettingValue = ECCentral.Service.Utility.StringUtility.Join(entity.PayTypeSettingValue, ",");
            }
            command.SetParameterValue("@PayTypeSettingValue", payTypeSettingValue);

            string shipAreaSettingValue = string.Empty;
            if (entity.ShipAreaSettingValue != null)
            {
                shipAreaSettingValue = ECCentral.Service.Utility.StringUtility.Join(entity.ShipAreaSettingValue, ",");
            }
            command.SetParameterValue("@ShipAreaSettingValue", shipAreaSettingValue);

            command.ExecuteNonQuery();
        }

        private void UpdateProductSettingValue(FreeShippingChargeRuleInfo entity)
        {
            DeleteRuleAllProductSettingValue(entity.SysNo.Value);

            if (entity.ProductSettingValue != null && entity.ProductSettingValue.Count > 0)
            {
                foreach (var item in entity.ProductSettingValue)
                {
                    CreateRuleProductSettingItemValue(entity.SysNo.Value, item);
                }
            }
        }

        private void LoadRuleProductSettingValue(FreeShippingChargeRuleInfo entity)
        {
            DataCommand command = DataCommandManager.GetDataCommand("GetFreeShippingChargeRuleProductSettingValue");
            command.SetParameterValue("@RuleSysNo", entity.SysNo.Value);

            entity.ProductSettingValue = command.ExecuteEntityList<SimpleObject>();
        }

        private void CreateRuleProductSettingItemValue(int ruleSysNo, SimpleObject item)
        {
            DataCommand command = DataCommandManager.GetDataCommand("CreateFreeShippingChargeRuleProductSettingValue");
            command.SetParameterValue("@RuleSysNo", ruleSysNo);
            command.SetParameterValue("@ProductSysNo", item.SysNo.Value);

            command.ExecuteScalar();
        }

        private void DeleteRuleAllProductSettingValue(int ruleSysNo)
        {
            DataCommand command = DataCommandManager.GetDataCommand("DeleteFreeShippingChargeRuleProductSettingValue");
            command.SetParameterValue("@RuleSysNo", ruleSysNo);

            command.ExecuteNonQuery();
        }
            
        private bool HasElements(IList list)
        {
            return list != null && list.Count > 0;
        }
    }
}