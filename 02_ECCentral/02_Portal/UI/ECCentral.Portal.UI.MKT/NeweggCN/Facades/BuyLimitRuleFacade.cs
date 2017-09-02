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
    public class BuyLimitRuleFacade
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

        public BuyLimitRuleFacade(IPage page)
        {
            ServiceBaseUrl = page.Context.Window.Configuration.GetConfigValue("MKT", "ServiceBaseUrl");
            restClient = new RestClient(ServiceBaseUrl, page);
        }

        public void Query(BuyLimitRuleQueryVM queryVM, PagingInfo p, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            var queryFilter = queryVM.ConvertVM<BuyLimitRuleQueryVM, BuyLimitRuleQueryFilter>();
            queryFilter.CompanyCode = CPApplication.Current.CompanyCode;
            queryFilter.PagingInfo = p;
            string relativeUrl = "/MKTService/BuyLimitRule/Query";
            restClient.QueryDynamicData(relativeUrl, queryFilter, callback);

        }

        public void Create(BuyLimitRuleVM vm, EventHandler<RestClientEventArgs<object>> callback)
        {
            var entity = BuildEntityFromVM(vm);
            string relativeUrl = "/MKTService/BuyLimitRule/Create";
            restClient.Create(relativeUrl, entity, callback);
        }

        public void Update(BuyLimitRuleVM vm, EventHandler<RestClientEventArgs<object>> callback)
        {
            var entity = BuildEntityFromVM(vm);
            string relativeUrl = "/MKTService/BuyLimitRule/Update";
            restClient.Update(relativeUrl, entity, callback);
        }

        public void Load(string sysNo, EventHandler<RestClientEventArgs<BuyLimitRule>> callback)
        {
            string relativeUrl = "/MKTService/BuyLimitRule/" + sysNo;
            restClient.Query(relativeUrl, callback);
        }

        private BuyLimitRule BuildEntityFromVM(BuyLimitRuleVM vm)
        {
            var entity = vm.ConvertVM<BuyLimitRuleVM, BuyLimitRule>();
            if (vm.LimitType == LimitType.Combo)
            {
                entity.ItemSysNo = int.Parse(vm.ComboSysNo);
            }
            else
            {
                entity.ItemSysNo = int.Parse(vm.ProductSysNo);
            }
            entity.CompanyCode = CPApplication.Current.CompanyCode;
            entity.WebChannel = new BizEntity.Common.WebChannel
            {
                ChannelID = ""
            };
            return entity;
        }
    }
}
