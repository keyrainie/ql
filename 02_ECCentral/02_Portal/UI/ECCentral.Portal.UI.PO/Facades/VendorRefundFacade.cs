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
using Newegg.Oversea.Silverlight.Controls;
using ECCentral.QueryFilter.PO;
using ECCentral.BizEntity.PO;

namespace ECCentral.Portal.UI.PO.Facades
{
    public class VendorRefundFacade
    {
        private readonly RestClient restClient;

        private string ServiceBaseUrl
        {
            get
            {
                return CPApplication.Current.CurrentPage.Context.Window.Configuration.GetConfigValue("PO", "ServiceBaseUrl");
            }
        }

        public VendorRefundFacade(IPage page)
        {
            restClient = new RestClient(ServiceBaseUrl, page);
        }

        /// <summary>
        /// 查询供应商退款单列表
        /// </summary>
        /// <param name="queryFilter"></param>
        /// <param name="callback"></param>
        public void QueryVendorRMARefundList(VendorRMARefundQueryFilter queryFilter, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            queryFilter.CompanyCode = CPApplication.Current.CompanyCode;
            string relativeUrl = "POService/VendorRefund/QueryVendorRefundList";
            restClient.QueryDynamicData(relativeUrl, queryFilter, callback);
        }

        public void ExportExcelForVendorRMARefundList(VendorRMARefundQueryFilter queryFilter, ColumnSet[] columns)
        {
            queryFilter.CompanyCode = CPApplication.Current.CompanyCode;
            string relativeUrl = "POService/VendorRefund/QueryVendorRefundList";
            restClient.ExportFile(relativeUrl, queryFilter, columns);
        }

        /// <summary>
        /// 加载供应商退款单信息
        /// </summary>
        /// <param name="refundSysNo"></param>
        /// <param name="callback"></param>
        public void LoadVendorRefundInfo(string refundSysNo, EventHandler<RestClientEventArgs<VendorRefundInfo>> callback)
        {
            string relativeUrl = string.Format("POService/VendorRefund/LoadVendorRefundInfo/{0}", refundSysNo);
            restClient.Query<VendorRefundInfo>(relativeUrl, callback);
        }

        /// <summary>
        /// 更新供应商退款单
        /// </summary>
        /// <param name="info"></param>
        /// <param name="callback"></param>
        public void UpdateVendorRefundInfo(VendorRefundInfo info, EventHandler<RestClientEventArgs<VendorRefundInfo>> callback)
        {
            info.CompanyCode = CPApplication.Current.CompanyCode;
            string relativeUrl = "POService/VendorRefund/UpdateVendorRefundInfo";
            restClient.Update<VendorRefundInfo>(relativeUrl, info, callback);
        }

        /// <summary>
        /// 供应商退款单 - 审核通过
        /// </summary>
        /// <param name="info"></param>
        /// <param name="callback"></param>
        public void ApproveVendorRefundInfo(VendorRefundInfo info, EventHandler<RestClientEventArgs<VendorRefundInfo>> callback)
        {
            info.CompanyCode = CPApplication.Current.CompanyCode;
            string relativeUrl = "POService/VendorRefund/ApproveVendorRefundInfo";
            restClient.Update<VendorRefundInfo>(relativeUrl, info, callback);
        }

        /// <summary>
        /// 供应商退款单 - 审核拒绝
        /// </summary>
        /// <param name="info"></param>
        /// <param name="callback"></param>
        public void RejectVendorRefundInfo(VendorRefundInfo info, EventHandler<RestClientEventArgs<VendorRefundInfo>> callback)
        {
            info.CompanyCode = CPApplication.Current.CompanyCode;
            string relativeUrl = "POService/VendorRefund/RejectVendorRefundInfo";
            restClient.Update<VendorRefundInfo>(relativeUrl, info, callback);
        }

        /// <summary>
        /// 提交PMCC审核
        /// </summary>
        /// <param name="info"></param>
        /// <param name="callback"></param>
        public void SubmitToPMCC(VendorRefundInfo info, EventHandler<RestClientEventArgs<VendorRefundInfo>> callback)
        {
            info.CompanyCode = CPApplication.Current.CompanyCode;
            string relativeUrl = "POService/VendorRefund/SubmitToPMCC";
            restClient.Update<VendorRefundInfo>(relativeUrl, info, callback);
        }

        public void GetVendorPayBalanceByVendorSysNo(int vendorSysNo, EventHandler<RestClientEventArgs<decimal>> callback)
        {
            string relativeUrl = "POService/VendorRefund/GetVendorPayBalanceByVendorSysNo";
            restClient.Query<decimal>(relativeUrl, vendorSysNo.ToString(), callback);
        }
    }
}
