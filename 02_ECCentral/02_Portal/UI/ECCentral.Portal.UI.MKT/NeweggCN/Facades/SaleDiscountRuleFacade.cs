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
using ECCentral.BizEntity.MKT;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.Portal.UI.MKT.NeweggCN.Models;
using ECCentral.QueryFilter.Common;
using ECCentral.QueryFilter.MKT;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using Newegg.Oversea.Silverlight.Controls;

namespace ECCentral.Portal.UI.MKT.NeweggCN.Facades
{
    public class SaleDiscountRuleFacade
    {
        private readonly RestClient restClient;
        /// <summary>
        /// 服务基地址
        /// </summary>
        protected string ServiceBaseUrl
        {
            get;
            private set;
        }

        public SaleDiscountRuleFacade(IPage page)
        {
            ServiceBaseUrl = page.Context.Window.Configuration.GetConfigValue("MKT", "ServiceBaseUrl");
            restClient = new RestClient(ServiceBaseUrl, page);
        }

        public void Query(SaleDiscountRuleQueryVM queryVM, PagingInfo p, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            var queryFilter = queryVM.ConvertVM<SaleDiscountRuleQueryVM, SaleDiscountRuleQueryFilter>();
            queryFilter.CompanyCode = CPApplication.Current.CompanyCode;
            queryFilter.PagingInfo = p;
            string relativeUrl = "/MKTService/SaleDiscountRule/Query";
            restClient.QueryDynamicData(relativeUrl, queryFilter, callback);

        }

        public void Create(SaleDiscountRuleVM vm, EventHandler<RestClientEventArgs<object>> callback)
        {
            var entity = vm.ConvertVM<SaleDiscountRuleVM, SaleDiscountRule>();
            entity.CompanyCode = CPApplication.Current.CompanyCode;
            entity.WebChannel = new BizEntity.Common.WebChannel
            {
                ChannelID = ""
            };
            string relativeUrl = "/MKTService/SaleDiscountRule/Create";
            restClient.Create(relativeUrl, entity, callback);
        }

        public void Update(SaleDiscountRuleVM vm, EventHandler<RestClientEventArgs<object>> callback)
        {
            var entity = vm.ConvertVM<SaleDiscountRuleVM, SaleDiscountRule>();
            entity.CompanyCode = CPApplication.Current.CompanyCode;
            entity.WebChannel = new BizEntity.Common.WebChannel
            {
                ChannelID = ""
            };
            string relativeUrl = "/MKTService/SaleDiscountRule/Update";
            restClient.Update(relativeUrl, entity, callback);
        }

        public void Load(string sysNo, EventHandler<RestClientEventArgs<SaleDiscountRule>> callback)
        {
            string relativeUrl = "/MKTService/SaleDiscountRule/" + sysNo;
            restClient.Query(relativeUrl, callback);
        }
    }
}
