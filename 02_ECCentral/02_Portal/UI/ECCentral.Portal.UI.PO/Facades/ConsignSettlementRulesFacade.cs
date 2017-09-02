using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using ECCentral.Portal.Basic.Utilities;
using Newegg.Oversea.Silverlight.Controls;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using ECCentral.QueryFilter.PO;
using ECCentral.BizEntity.PO;

namespace ECCentral.Portal.UI.PO.Facades
{
    public class ConsignSettlementRulesFacade
    {
        private readonly RestClient restClient;

        private string ServiceBaseUrl
        {
            get
            {
                return CPApplication.Current.CurrentPage.Context.Window.Configuration.GetConfigValue("PO", "ServiceBaseUrl");
            }
        }
        public ConsignSettlementRulesFacade(IPage page)
        {
            restClient = new RestClient(ServiceBaseUrl, page);
        }

        /// <summary>
        /// 代销商品规则设置 - 列表查询
        /// </summary>
        /// <param name="queryFilter"></param>
        /// <param name="callback"></param>
        public void QueryConsignSettleRulesList(SettleRuleQueryFilter queryFilter, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            queryFilter.CompanyCode = CPApplication.Current.CompanyCode;
            string relativeUrl = "/POService/ConsignSettlementRules/QueryConsignSettleRulesList";
            restClient.QueryDynamicData(relativeUrl, queryFilter, callback);
        }

        /// <summary>
        /// 导出ExcelAll
        /// </summary>
        /// <param name="request"></param>
        /// <param name="columns"></param>
        public void ExportExcelForVendors(SettleRuleQueryFilter request, ColumnSet[] columns)
        {
            request.CompanyCode = CPApplication.Current.CompanyCode;
            string relativeUrl = "/POService/ConsignSettlementRules/QueryConsignSettleRulesList";
            restClient.ExportFile(relativeUrl, request, columns);
        }


        /// <summary>
        /// 创建代销商品规则
        /// </summary>
        /// <param name="info"></param>
        /// <param name="callback"></param>
        public void CreateConsignSettleRule(ConsignSettlementRulesInfo info, EventHandler<RestClientEventArgs<ConsignSettlementRulesInfo>> callback)
        {
            info.CompanyCode = CPApplication.Current.CompanyCode;
            info.EditUserSysNo = CPApplication.Current.LoginUser.UserSysNo;
            info.EditUser = CPApplication.Current.LoginUser.DisplayName;
            info.CreateUser = CPApplication.Current.LoginUser.DisplayName;
            string relativeUrl = "/POService/ConsignSettlementRules/CreateConsignSettleRule";
            restClient.Update<ConsignSettlementRulesInfo>(relativeUrl, info, callback);
        }


        /// <summary>
        /// 更新代销商品规则
        /// </summary>
        /// <param name="info"></param>
        /// <param name="callback"></param>
        public void UpdateConsignSettleRule(ConsignSettlementRulesInfo info, EventHandler<RestClientEventArgs<ConsignSettlementRulesInfo>> callback)
        {
            info.CompanyCode = CPApplication.Current.CompanyCode;
            info.EditUserSysNo = CPApplication.Current.LoginUser.UserSysNo;
            info.EditUser = CPApplication.Current.LoginUser.DisplayName;
            string relativeUrl = "/POService/ConsignSettlementRules/UpdateConsignSettleRule";
            restClient.Update<ConsignSettlementRulesInfo>(relativeUrl, info, callback);
        }

        /// <summary>
        /// 审核代销商品规则
        /// </summary>
        /// <param name="info"></param>
        /// <param name="callback"></param>
        public void AuditConsignSettleRule(ConsignSettlementRulesInfo info, EventHandler<RestClientEventArgs<ConsignSettlementRulesInfo>> callback)
        {
            info.CompanyCode = CPApplication.Current.CompanyCode;
            info.EditUserSysNo = CPApplication.Current.LoginUser.UserSysNo;
            info.EditUser = CPApplication.Current.LoginUser.DisplayName;
            string relativeUrl = "/POService/ConsignSettlementRules/AuditConsignSettleRule";
            restClient.Update<ConsignSettlementRulesInfo>(relativeUrl, info, callback);
        }

        /// <summary>
        /// 终止代销商品规则
        /// </summary>
        /// <param name="info"></param>
        /// <param name="callback"></param>
        public void StopConsignSettleRule(ConsignSettlementRulesInfo info, EventHandler<RestClientEventArgs<ConsignSettlementRulesInfo>> callback)
        {
            info.CompanyCode = CPApplication.Current.CompanyCode;
            info.EditUserSysNo = CPApplication.Current.LoginUser.UserSysNo;
            info.EditUser = CPApplication.Current.LoginUser.DisplayName;
            string relativeUrl = "/POService/ConsignSettlementRules/StopConsignSettleRule";
            restClient.Update<ConsignSettlementRulesInfo>(relativeUrl, info.SettleRulesCode, callback);
        }

        /// <summary>
        /// 作废代销商品规则
        /// </summary>
        /// <param name="info"></param>
        /// <param name="callback"></param>
        public void AbandonConsignSettleRule(ConsignSettlementRulesInfo info, EventHandler<RestClientEventArgs<ConsignSettlementRulesInfo>> callback)
        {
            info.CompanyCode = CPApplication.Current.CompanyCode;
            info.EditUserSysNo = CPApplication.Current.LoginUser.UserSysNo;
            info.EditUser = CPApplication.Current.LoginUser.DisplayName;
            string relativeUrl = "/POService/ConsignSettlementRules/AbandonConsignSettleRule";
            restClient.Update<ConsignSettlementRulesInfo>(relativeUrl, info.SettleRulesCode, callback);
        }
    }
}
