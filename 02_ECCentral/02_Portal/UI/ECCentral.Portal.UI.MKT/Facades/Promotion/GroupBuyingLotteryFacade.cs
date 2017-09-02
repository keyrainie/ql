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
using Newegg.Oversea.Silverlight.Controls;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.Portal.UI.MKT.Models;
using ECCentral.QueryFilter.Common;
using ECCentral.QueryFilter.MKT;
using ECCentral.BizEntity.Customer;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using System.Collections.Generic;
using ECCentral.Portal.UI.MKT.Models.GroupBuying;
using ECCentral.BizEntity.MKT.Promotion;
using ECCentral.Service.MKT.Restful.RequestMsg;

namespace ECCentral.Portal.UI.MKT.Facades
{
    public class GroupBuyingLotteryFacade
    {
        private IPage _Page;
        private readonly RestClient restClient;
        /// <summary>
        /// 服务基地址
        /// </summary>
        protected string ServiceBaseUrl
        {
            get;
            private set;
        }
        public GroupBuyingLotteryFacade(IPage page)
        {
            _Page = page;
            ServiceBaseUrl = page.Context.Window.Configuration.GetConfigValue("MKT", "ServiceBaseUrl");
            restClient = new RestClient(ServiceBaseUrl, page);
        }

        public void Query(GroupBuyingLotteryQueryVM vm, PagingInfo pagingInfo, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            var data = vm.ConvertVM<GroupBuyingLotteryQueryVM, GroupBuyingLotteryQueryFilter>();
            data.PagingInfo = pagingInfo;
            string relativeUrl = "/MKTService/GroupBuyingLottery/Query";
            restClient.QueryDynamicData(relativeUrl, data, callback);
        }
    }
}