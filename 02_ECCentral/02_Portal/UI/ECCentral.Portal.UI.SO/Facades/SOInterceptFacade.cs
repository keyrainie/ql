using ECCentral.Portal.Basic;
using ECCentral.Portal.Basic.Utilities;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using Newegg.Oversea.Silverlight.Controls;
using System;
using ECCentral.BizEntity.Customer;
using System.Collections.Generic;
using ECCentral.BizEntity.SO;
using ECCentral.Service.SO.Restful.RequestMsg;

namespace ECCentral.Portal.UI.SO.Facades
{
    public class SOInterceptFacade
    {
        private readonly RestClient restClient;
        private string serviceBaseUrl;

        public SOInterceptFacade(IPage page)
        {
            serviceBaseUrl = CPApplication.Current.CurrentPage.Context.Window.Configuration.GetConfigValue(ConstValue.DomainName_SO, ConstValue.Key_ServiceBaseUrl);
            restClient = new RestClient(serviceBaseUrl, page);
        }

        public SOInterceptFacade()
            : this(null)
        {
        }

        /// <summary>
        /// 添加订单拦截设置信息
        /// </summary>
        /// <param name="info"></param>
        public void AddSOInterceptInfo(SOInterceptInfo req, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            req.CompanyCode = CPApplication.Current.CompanyCode;
            restClient.Create("/SOService/SO/AddSOInterceptInfo", req, callback);
        }

        /// <summary>
        /// 删除选中的数据
        /// </summary>
        /// <param name="req"></param>
        /// <param name="callback"></param>
        public void DeleteSOInterceptInfo(SOInterceptInfo req, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            restClient.Update("/SOService/SO/DeleteSOInterceptInfo", req, callback);
        }

        /// <summary>
        /// 批量修改订单拦截设置信息
        /// </summary>
        /// <param name="info"></param>
        public void BatchUpdateSOInterceptInfo(SOInterceptInfo req, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            restClient.Update("/SOService/SO/BatchUpdateSOInterceptInfo", req, callback);
        }

        /// <summary>
        /// 发送订单拦截邮件
        /// </summary>
        /// <param name="req"></param>
        public void SendSOOrderInterceptEmail(SendEmailReq req,EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            restClient.Update("/SOService/SO/SendSOInterceptEmail", req, callback);
        }

        /// <summary>
        /// 发送增票拦截邮件
        /// </summary>
        /// <param name="req"></param>
        public void SendSOFinanceInterceptEmail(SendEmailReq req, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            restClient.Update("/SOService/SO/SendSOFinanceInterceptEmail", req, callback);
        }
    }
}
