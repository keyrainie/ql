using ECCentral.Portal.Basic;
using ECCentral.Portal.Basic.Utilities;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using Newegg.Oversea.Silverlight.Controls;
using System;
using ECCentral.BizEntity.Customer;
using System.Collections.Generic;
using ECCentral.BizEntity.SO;

namespace ECCentral.Portal.UI.SO.Facades
{
    public class SOInternalMemoFacade
    {
        private readonly RestClient restClient;
        private string serviceBaseUrl;

        public SOInternalMemoFacade(IPage page)
        {
            serviceBaseUrl = CPApplication.Current.CurrentPage.Context.Window.Configuration.GetConfigValue(ConstValue.DomainName_SO, ConstValue.Key_ServiceBaseUrl);
            restClient = new RestClient(serviceBaseUrl, page);
        }

        public SOInternalMemoFacade()
            : this(null)
        {
        }

        /// <summary>
        /// 查询所有创建订单跟进日志的人员名单，应用于下拉框数据加载
        /// </summary>
        public void QuerySOLogCreater(string companyCode, EventHandler<RestClientEventArgs<List<CSInfo>>> callback)
        {
            restClient.Query("/SOService/SO/QueryCreateLogUser/" + companyCode, callback);
        }

        /// <summary>
        /// 创建日志
        /// </summary>
        /// <param name="req"></param>
        /// <param name="callback"></param>
        public void Create(SOInternalMemoInfo req, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            req.CompanyCode = CPApplication.Current.CompanyCode;
            restClient.Create("/SOService/SO/AddSOInternalMemoInfo" , req, callback);
        }

        /// <summary>
        /// 查询所有修改订单跟进日志的人员名单，应用于下拉框数据加载
        /// </summary>
        public void QuerySOLogUpdater(string companyCode, EventHandler<RestClientEventArgs<List<CSInfo>>> callback)
        {
            restClient.Query("/SOService/SO/QueryUpdateLogUser/" + companyCode, callback);
        }

        public void Close(SOInternalMemoInfo req,EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            restClient.Update("/SOService/SO/CloseSOInternalMemo", req, callback);
        }

        public void CancelAssign(List<SOInternalMemoInfo> req, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            restClient.Update("/SOService/SO/BathCanceAssignSOInternalMemo", req, callback);
        }

        public void Assign(List<SOInternalMemoInfo> req, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            restClient.Update("/SOService/SO/BatchAssignSOInternalMemo", req, callback);
        }
    }
}
