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
using ECCentral.BizEntity.PO.PurchaseOrder;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.Portal.UI.PO.Models.PurchaseOrder;
using ECCentral.QueryFilter.PO;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using Newegg.Oversea.Silverlight.Controls;

namespace ECCentral.Portal.UI.PO
{
    public class DeductFacade
    {
        private readonly RestClient restClient;

        private string ServiceBaseUrl
        {
            get
            {
                return CPApplication.Current.CurrentPage.Context.Window.Configuration.GetConfigValue("PO", "ServiceBaseUrl");
            }
        }

        public DeductFacade(IPage page)
        {
            restClient = new RestClient(ServiceBaseUrl, page);
        }

        /// <summary>
        /// 查询扣款项维护列表
        /// </summary>
        /// <param name="request"></param>
        /// <param name="callback"></param>
        public void QueryDeducts(DeductQueryFilter request, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            string relativeUrl = "/POService/Deduct/QueryDeductList";
            restClient.QueryDynamicData(relativeUrl, request, callback);
        }

        /// <summary>
        /// 作废单个扣款维护项信息
        /// </summary>
        /// <param name="sysNo"></param>
        /// <param name="callback"></param>
        public void DelSingleDeductInfo(int sysNo,EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            string relativeUrl = "/POService/Deduct/DeleteDeductBySysNo";
            restClient.Delete(relativeUrl, sysNo, callback);
        }

        /// <summary>
        /// 加载单个扣款项信息
        /// </summary>
        /// <param name="vendorSysNo"></param>
        /// <param name="callback"></param>
        public void GetSingleDeductBySysNo(string sysNo, EventHandler<RestClientEventArgs<Deduct>> callback)
        {
            string relativeUrl = string.Format("/POService/Deduct/GetSingleDeductBySysNo/{0}", sysNo);
            restClient.Query<Deduct>(relativeUrl, callback);
        }

        /// <summary>
        /// 更新扣款项信息
        /// </summary>
        /// <param name="deduct"></param>
        /// <param name="callback"></param>
        public void UpdateDeductInfo(Deduct deduct,EventHandler<RestClientEventArgs<Deduct>> callback)
        {
            string relativeUrl = "/POService/Deduct/UpdateDeduct";
            restClient.Update<Deduct>(relativeUrl,deduct,callback);
        }

        /// <summary>
        /// 创建扣款项
        /// </summary>
        /// <param name="newdeductVM"></param>
        /// <param name="callback"></param>
        public void AddDeductInfo(DeductQueryVM newdeductVM,EventHandler<RestClientEventArgs<Deduct>> callback)
        {
            Deduct deductInfo = EntityConverter<DeductQueryVM, Deduct>.Convert(newdeductVM);
            string relativeUrl = "/POService/Deduct/CreateDeduct";
            restClient.Create<Deduct>(relativeUrl,deductInfo,callback);
        }
    }
}
