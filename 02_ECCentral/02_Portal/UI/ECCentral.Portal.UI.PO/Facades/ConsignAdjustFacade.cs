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
using ECCentral.BizEntity.PO;
using ECCentral.BizEntity.PO.PurchaseOrder;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.Portal.UI.PO.Models;
using ECCentral.Portal.UI.PO.Models.PurchaseOrder;
using ECCentral.QueryFilter.PO;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using Newegg.Oversea.Silverlight.Controls;

namespace ECCentral.Portal.UI.PO
{
    public class ConsignAdjustFacade
    {
        private readonly RestClient restClient;

        private string ServiceBaseUrl
        {
            get
            {
                return CPApplication.Current.CurrentPage.Context.Window.Configuration.GetConfigValue("PO", "ServiceBaseUrl");
            }
        }

        public ConsignAdjustFacade(IPage page)
        {
            restClient = new RestClient(ServiceBaseUrl, page);
        }

        /// <summary>
        /// 查询扣款项维护列表
        /// </summary>
        /// <param name="request"></param>
        /// <param name="callback"></param>
        public void Query(ConsignAdjustQueryFilter request, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            string relativeUrl = "/POService/ConsignAdjust/QueryConsignAdjustList";
            restClient.QueryDynamicData(relativeUrl, request, callback);
        }


        /// <summary>
        /// 加载单个信息
        /// </summary>
        /// <param name="vendorSysNo"></param>
        /// <param name="callback"></param>
        public void LoadInfo(string sysNo, EventHandler<RestClientEventArgs<ConsignAdjustInfo>> callback)
        {
            string relativeUrl = string.Format("/POService/ConsignAdjust/LoadConsignAdjust/{0}", sysNo);
            restClient.Query<ConsignAdjustInfo>(relativeUrl, callback);
        }

        /// <summary>
        /// 更新信息
        /// </summary>
        /// <param name="deduct"></param>
        /// <param name="callback"></param>
        public void Update(ConsignAdjustInfo info, EventHandler<RestClientEventArgs<ConsignAdjustInfo>> callback)
        {
            string relativeUrl = "/POService/ConsignAdjust/UpdateConsignAdjust";
            restClient.Update<ConsignAdjustInfo>(relativeUrl, info, callback);
        }

        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="newdeductVM"></param>
        /// <param name="callback"></param>
        public void Add(ConsignAdjustInfo info, EventHandler<RestClientEventArgs<ConsignAdjustInfo>> callback)
        {
            string relativeUrl = "/POService/ConsignAdjust/CreateConsignAdjust";
            restClient.Create<ConsignAdjustInfo>(relativeUrl, info, callback);
        }

        /// <summary>
        /// 审核
        /// </summary>
        /// <param name="info"></param>
        /// <param name="callback"></param>
        public void Audit(ConsignAdjustInfo info, EventHandler<RestClientEventArgs<ConsignAdjustInfo>> callback)
        {
            string relativeUrl = "/POService/ConsignAdjust/MaintainConsignAdjustStatus";
            restClient.Update<ConsignAdjustInfo>(relativeUrl, info, callback);
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="info"></param>
        /// <param name="callback"></param>
        public void Delete(int sysNo, EventHandler<RestClientEventArgs<ConsignAdjustInfo>> callback)
        {
            string relativeUrl = "/POService/ConsignAdjust/DelConsignAdjust";
            restClient.Update<ConsignAdjustInfo>(relativeUrl, sysNo, callback);
        }
    }
}
