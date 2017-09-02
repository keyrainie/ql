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
using ECCentral.QueryFilter.Customer;
using System.Collections.Generic;
using ECCentral.BizEntity.Customer;
using ECCentral.Portal.UI.Customer.Models;
using ECCentral.Service.Customer.Restful.RequestMsg;

namespace ECCentral.Portal.UI.Customer.Facades
{
    public class FPCheckFacade
    {
        private readonly RestClient restClient;
        private string ServiceBaseUrl
        {
            get
            {
                return CPApplication.Current.CurrentPage.Context.Window.Configuration.GetConfigValue("Customer", "ServiceBaseUrl");
            }
        }
        public FPCheckFacade()
        {
            restClient = new RestClient(ServiceBaseUrl);
        }
        public FPCheckFacade(IPage page)
        {
            restClient = new RestClient(ServiceBaseUrl, page);

        }
        /// <summary>
        /// 查询FP状态
        /// </summary>
        /// <param name="request"></param>
        /// <param name="callback"></param>
        public void Query(FPCheckQueryFilter request, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            string relativeUrl = "/CustomerService/FPCheckMaster/Query";
            restClient.QueryDynamicData(relativeUrl, request, callback);
        }
        /// <summary>
        /// 更新FP状态
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="callback"></param>
        public void BatchUpdate(List<FPCheck> msg, EventHandler<RestClientEventArgs<object>> callback)
        {
            string relativeUrl = "/CustomerService/FPCheckMaster/BatchUpdate";
            restClient.Update(relativeUrl, msg, callback);
        }
        /// <summary>
        /// 查询串货订单设置限制
        /// </summary>
        /// <param name="vm"></param>
        /// <param name="callback"></param>
        public void Query(CHQueryFilter filter, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            string relativeUrl = "/CustomerService/CHSet/Query";
            restClient.QueryDynamicData(relativeUrl, filter, callback);
        }
        /// <summary>
        /// 新建串货订单
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="callback"></param>
        public void CreateCH(CHMaintainVM cm, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            string relativeUrl = "/CustomerService/CHSet/CreateCH";
            CHSetReq request = cm.ConvertVM<CHMaintainVM, CHSetReq>();
            restClient.Create(relativeUrl, request, callback);
        }
        /// <summary>
        /// 设置串货项的状态
        /// </summary>
        /// <param name="id"></param>
        /// <param name="callback"></param>
        public void UpdateCHItemStatus(int id, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            string relativeUrl = "/CustomerService/CHSet/UpdateCHItemStatus";
            restClient.Update(relativeUrl, id, callback);
        }

        public void QueryETC(string WebChannelID, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            string relativeUrl = "/CustomerService/ETC/Query";
            restClient.QueryDynamicData(relativeUrl, WebChannelID, callback);
        }

        public void UpdateETC(List<CCSetReq> request, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            string relativeUrl = "/CustomerService/ETC/UpdateETC";
            restClient.Update(relativeUrl, request, callback);
        }
    }
}
