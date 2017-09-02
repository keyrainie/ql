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
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using ECCentral.QueryFilter.PO;
using Newegg.Oversea.Silverlight.Controls;
using ECCentral.BizEntity.PO;
using ECCentral.Portal.UI.PO.Models;

namespace ECCentral.Portal.UI.PO.Facades
{
    public class VendorCommissionFacade
    {
        private readonly RestClient restClient;

        private string ServiceBaseUrl
        {
            get
            {
                return CPApplication.Current.CurrentPage.Context.Window.Configuration.GetConfigValue("PO", "ServiceBaseUrl");
            }
        }

        public VendorCommissionFacade(IPage page)
        {
            restClient = new RestClient(ServiceBaseUrl, page);
        }

        /// <summary>
        /// 查询佣金列表
        /// </summary>
        /// <param name="queryFilter"></param>
        /// <param name="callback"></param>
        public void QueryCommissions(CommissionQueryFilter queryFilter, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            queryFilter.CompanyCode = CPApplication.Current.CompanyCode;
            string relativeUrl = "/POService/Commission/QueryVendorCommissions";
            restClient.QueryDynamicData(relativeUrl, queryFilter, callback);
        }

        public void GetManualCommissionMaster(CommissionMasterVM queryFilter, EventHandler<RestClientEventArgs<CommissionMaster>> callback)
        {
            string relativeUrl = "/POService/Commission/GetManualCommissionMaster";
            var info = queryFilter.ConvertVM<CommissionMasterVM, CommissionMaster>();
            info.CompanyCode = CPApplication.Current.CompanyCode;
            restClient.Query(relativeUrl, info, callback);
        }

        public void CreateCommission(CommissionMasterVM req, EventHandler<RestClientEventArgs<CommissionMaster>> callback)
        {
            string relativeUrl = "/POService/Commission/CreateSettleCommission";
            var info = req.ConvertVM<CommissionMasterVM,CommissionMaster>();
            info.CompanyCode = CPApplication.Current.CompanyCode;
            restClient.Update<CommissionMaster>(relativeUrl, info, callback);
        }

        /// <summary>
        /// 查询佣金列表总金额
        /// </summary>
        /// <param name="queryFilter"></param>
        /// <param name="callback"></param>
        public void QueryCommissionsTotalAmt(CommissionQueryFilter queryFilter, EventHandler<RestClientEventArgs<decimal>> callback)
        {
            queryFilter.CompanyCode = CPApplication.Current.CompanyCode;
            string relativeUrl = "/POService/Commission/QueryVendorCommissionsTotalCount";
            restClient.Query<decimal>(relativeUrl, queryFilter, callback);
        }

        /// <summary>
        /// 加载佣金信息
        /// </summary>
        /// <param name="commissionSysNo"></param>
        /// <param name="callback"></param>
        public void GetCommissionInfoBySysNo(string commissionSysNo, EventHandler<RestClientEventArgs<CommissionMaster>> callback)
        {
            string relativeUrl = string.Format("/POService/Commission/LoadCommission/{0}", commissionSysNo);
            restClient.Query<CommissionMaster>(relativeUrl, callback);
        }

        /// <summary>
        /// 批量关闭佣金信息
        /// </summary>
        /// <param name="commissionSysNos"></param>
        /// <param name="callback"></param>
        public void BatchCloseCommissions(string commissionSysNos, EventHandler<RestClientEventArgs<int>> callback)
        {
            string relativeUrl = "/POService/Commission/BatchCloseVendorCommissions";
            restClient.Update(relativeUrl, commissionSysNos, callback);
        }

        /// <summary>
        /// 关闭佣金结算单
        /// </summary>
        /// <param name="commission"></param>
        /// <param name="callback"></param>
        public void CloseCommission(CommissionMaster commission, EventHandler<RestClientEventArgs<CommissionMaster>> callback)
        {
            commission.CompanyCode = CPApplication.Current.CompanyCode;
            string relativeUrl = "/POService/Commission/CloseVendorCommission";
            restClient.Update(relativeUrl, commission, callback);
        }

        public void ExportExcelFile(CommissionQueryFilter queryFilter, ColumnSet[] columns)
        {
            queryFilter.CompanyCode = CPApplication.Current.CompanyCode;
            string relativeUrl = "/POService/Commission/QueryVendorCommissions";

            restClient.ExportFile(relativeUrl, queryFilter, columns);
        }
    }
}
