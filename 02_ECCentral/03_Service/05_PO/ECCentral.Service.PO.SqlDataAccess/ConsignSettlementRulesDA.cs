using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.Utility;
using ECCentral.Service.PO.IDataAccess;
using ECCentral.Service.Utility.DataAccess;
using ECCentral.BizEntity.PO;

namespace ECCentral.Service.PO.SqlDataAccess
{
    [VersionExport(typeof(IConsignSettlementRulesDA))]
    public class ConsignSettlementRulesDA : IConsignSettlementRulesDA
    {

        public BizEntity.PO.ConsignSettlementRulesInfo GetConsignSettleRuleByCode(string settleRulesCode)
        {
            DataCommand command = DataCommandManager.GetDataCommand("QueryConsignSettleRuleByCode");
            command.SetParameterValue("@SettleRulesCode", settleRulesCode);

            ConsignSettlementRulesInfo returnEntity = command.ExecuteEntity<ConsignSettlementRulesInfo>();
            ConvertStatus(returnEntity);
            return returnEntity;
        }

        public int UpdateConsignSettlementRulesInfo(ConsignSettlementRulesInfo entity, BizEntity.PO.ConsignSettleRuleActionType actionType)
        {
            DataCommand command = DataCommandManager.GetDataCommand("UpdateConsignSettleRule");
            if (actionType == BizEntity.PO.ConsignSettleRuleActionType.Audit)
            {
                command.SetParameterValue("@ApprovedDate", DateTime.Now);
                command.SetParameterValue("@ApprovedUser", entity.EditUser);
            }
            else
            {
                command.SetParameterValue("@ApprovedDate", DBNull.Value);
                command.SetParameterValue("@ApprovedUser", DBNull.Value);
            }

            command.SetParameterValue("@BeginDate", entity.BeginDate);
            command.SetParameterValue("@CurrencyCode", entity.CurrencyCode);
            command.SetParameterValue("@EditDate", DateTime.Now);
            command.SetParameterValue("@EditUser", entity.EditUser);
            command.SetParameterValue("@EndDate", entity.EndDate);
            command.SetParameterValue("@LanguageCode", "zh-CN");
            command.SetParameterValue("@NewSettlePrice", entity.NewSettlePrice);
            command.SetParameterValue("@OldSetttlePrice", entity.OldSettlePrice);
            command.SetParameterValue("@ProductSysNo", entity.ProductSysNo);
            command.SetParameterValue("@SettleedQuantity", DBNull.Value);
            command.SetParameterValue("@SettleRuleName", entity.SettleRulesName);
            command.SetParameterValue("@SettleRuleQuantity", entity.SettleRulesQuantity);
            command.SetParameterValue("@SettleRulesCode", entity.SettleRulesCode);
            command.SetParameterValue("@Status", (char)entity.Status);
            if (actionType == BizEntity.PO.ConsignSettleRuleActionType.Stop)
            {
                command.SetParameterValue("@StopDate", DateTime.Now);
                command.SetParameterValue("@StopUser", entity.EditUser);
            }
            else
            {
                command.SetParameterValue("@StopDate", DBNull.Value);
                command.SetParameterValue("@StopUser", DBNull.Value);
            }
            command.SetParameterValue("@VendorSysNo", entity.VendorSysNo);
            command.SetParameterValue("@SysNo", entity.RuleSysNo);
            return command.ExecuteNonQuery();
        }

        public int CreateConsignSettlementRule(ConsignSettlementRulesInfo entity)
        {
            DataCommand command = DataCommandManager.GetDataCommand("InsertConsignSettleRule");
            command.SetParameterValue("@ApprovedDate", DBNull.Value);
            command.SetParameterValue("@ApprovedUser", DBNull.Value);
            command.SetParameterValue("@BeginDate", entity.BeginDate);
            command.SetParameterValue("@CompanyCode", entity.CompanyCode);
            command.SetParameterValue("@CreateDate", DateTime.Now);
            command.SetParameterValue("@CreateUser", entity.CreateUser);
            command.SetParameterValue("@CurrencyCode", entity.CurrencyCode);
            command.SetParameterValue("@EditDate", DateTime.Now);
            command.SetParameterValue("@EditUser", entity.CreateUser);
            command.SetParameterValue("@EndDate", entity.EndDate);
            command.SetParameterValue("@LanguageCode", "zh-CN");
            command.SetParameterValue("@NewSettlePrice", entity.NewSettlePrice);
            command.SetParameterValue("@OldSetttlePrice", entity.OldSettlePrice);
            command.SetParameterValue("@ProductSysNo", entity.ProductSysNo);
            command.SetParameterValue("@SettleedQuantity", DBNull.Value);
            command.SetParameterValue("@SettleRuleName", entity.SettleRulesName);
            command.SetParameterValue("@SettleRuleQuantity", entity.SettleRulesQuantity);
            command.SetParameterValue("@SettleRulesCode", entity.SettleRulesCode);
            command.SetParameterValue("@Status", (char)entity.Status);
            command.SetParameterValue("@StopDate", DBNull.Value);
            command.SetParameterValue("@StopUser", DBNull.Value);
            command.SetParameterValue("@StoreCompanyCode", entity.CompanyCode);
            command.SetParameterValue("@VendorSysNo", entity.VendorSysNo);

            int sysNo = Convert.ToInt32(command.ExecuteScalar());

            return sysNo;
        }

        public ConsignSettlementRulesInfo GetSettleRuleByItemVender(ConsignSettlementRulesInfo entity)
        {
            DataCommand command = DataCommandManager.GetDataCommand("GetSettleRuleByItemVender");

            command.SetParameterValue("@SettleRulesCode", entity.SettleRulesCode);
            command.SetParameterValue("@ProductSysNo", entity.ProductSysNo);
            command.SetParameterValue("@VendorSysNo", entity.VendorSysNo);
            command.SetParameterValue("@BeginDate", entity.BeginDate);
            command.SetParameterValue("@EndDate", entity.EndDate);

            ConsignSettlementRulesInfo returnEntity = command.ExecuteEntity<ConsignSettlementRulesInfo>();
            ConvertStatus(returnEntity);
            return returnEntity;
        }

        public int CheckConsignProduct(int productSysNo)
        {
            DataCommand command = DataCommandManager.GetDataCommand("ExistsProductConsign");

            command.SetParameterValue("@ProductSysNo", productSysNo);

            return Convert.ToInt32(command.ExecuteScalar());
        }

        private void ConvertStatus(ConsignSettlementRulesInfo entity)
        {
            if (null != entity && entity.StatusString.HasValue)
            {
                switch (entity.StatusString)
                {
                    case (char)ConsignSettleRuleStatus.Wait_Audit:
                        entity.Status = ConsignSettleRuleStatus.Wait_Audit;
                        break;
                    case (char)ConsignSettleRuleStatus.Stop:
                        entity.Status = ConsignSettleRuleStatus.Stop;
                        break;
                    case (char)ConsignSettleRuleStatus.Forbid:
                        entity.Status = ConsignSettleRuleStatus.Forbid;
                        break;
                    case (char)ConsignSettleRuleStatus.Enable:
                        entity.Status = ConsignSettleRuleStatus.Enable;
                        break;
                    case (char)ConsignSettleRuleStatus.Disable:
                        entity.Status = ConsignSettleRuleStatus.Disable;
                        break;
                    case (char)ConsignSettleRuleStatus.Available:
                        entity.Status = ConsignSettleRuleStatus.Available;
                        break;
                    default:
                        break;
                }
            }
        }


        /// <summary>
        /// 根据Id列表获取规则集合
        /// </summary>
        /// <param name="sysNos">Id列表</param>
        /// <returns>规则集合</returns>
        public List<ConsignSettlementRulesInfo> GetSettleRuleListBySysNos(string sysNos)
        {
            DataCommand command = DataCommandManager.GetDataCommand("GetConsignRuleBySysNos");
            command.ReplaceParameterValue("#WHERE#", sysNos);
            return command.ExecuteEntityList<ConsignSettlementRulesInfo>();
        }
    }
}
