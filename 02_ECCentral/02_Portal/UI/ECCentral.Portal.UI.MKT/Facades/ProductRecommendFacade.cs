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
using ECCentral.BizEntity.IM;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.Service.Utility;
using Newegg.Oversea.Silverlight.Controls;
using ECCentral.Portal.UI.MKT.Models;
using ECCentral.QueryFilter.Common;
using ECCentral.QueryFilter.MKT;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using ECCentral.BizEntity.MKT;
using System.Collections.Generic;

namespace ECCentral.Portal.UI.MKT.Facades
{
    public class ProductRecommendFacade
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

        public ProductRecommendFacade(IPage page)
        {
            ServiceBaseUrl = page.Context.Window.Configuration.GetConfigValue("MKT", "ServiceBaseUrl");
            restClient = new RestClient(ServiceBaseUrl, page);
        }

         public void Query(ProductRecommendQueryVM queryVM, PagingInfo p, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            var queryFilter = queryVM.ConvertVM<ProductRecommendQueryVM, ProductRecommendQueryFilter>();
            queryFilter.CompanyCode = CPApplication.Current.CompanyCode;
            queryFilter.PagingInfo = p;
            string relativeUrl = "/MKTService/ProductRecommend/Query";
            restClient.QueryDynamicData(relativeUrl, queryFilter,
             (obj, args) =>
             {
                 if (args.FaultsHandle())
                 {
                     return;
                 }

                 if (!(args == null || args.Result == null || args.Result.Rows == null))
                 {
                     foreach (var item in args.Result.Rows)
                     {
                         item.ProductName = String.IsNullOrWhiteSpace(item.ProductName)
                                                ? item.ProductName
                                                : StringUtility.RemoveHtmlTag(item.ProductName);
                     }
                 }
                 callback(obj, args);
             }
             );
            //restClient.QueryDynamicData(relativeUrl, queryFilter, callback);

        }

         public void Deactive(int sysNo, EventHandler<RestClientEventArgs<object>> callback)
         {
             string relativeUrl = "/MKTService/ProductRecommend/Deactive/"+sysNo.ToString();
             restClient.Update(relativeUrl, null, callback);
         }

         public void Load(string sysNo, EventHandler<RestClientEventArgs<ProductRecommendInfo>> callback)
         {
             string relativeUrl = "/MKTService/ProductRecommend/Load/" + sysNo;
             restClient.Query<ProductRecommendInfo>(relativeUrl, callback);
         }

         public void Update(ProductRecommendVM vm, EventHandler<RestClientEventArgs<object>> callback)
         {
             var model = vm.ConvertVM<ProductRecommendVM, ProductRecommendInfo>((v,entity)=>{
                 entity.Location = v.LocationVM.ConvertVM<ProductRecommendLocationVM, ProductRecommendLocation>();
                 entity.Location.CompanyCode = CPApplication.Current.CompanyCode;
                 entity.Location.WebChannel = new BizEntity.Common.WebChannel { ChannelID = vm.ChannelID };
             }); ;
             model.CompanyCode = CPApplication.Current.CompanyCode;
             model.WebChannel = new BizEntity.Common.WebChannel { ChannelID = vm.ChannelID};
             string relativeUrl = "/MKTService/ProductRecommend/Update";
             restClient.Update(relativeUrl, model, callback);
         }

         public void Create(ProductRecommendVM vm, EventHandler<RestClientEventArgs<object>> callback)
         {
             var model = vm.ConvertVM<ProductRecommendVM, ProductRecommendInfo>((v, entity) =>
             {
                 entity.Location = v.LocationVM.ConvertVM<ProductRecommendLocationVM, ProductRecommendLocation>();
                 entity.Location.CompanyCode = CPApplication.Current.CompanyCode;
                 entity.Location.WebChannel = new BizEntity.Common.WebChannel { ChannelID = vm.ChannelID };
             }); ;
             model.CompanyCode = CPApplication.Current.CompanyCode;
             model.WebChannel = new BizEntity.Common.WebChannel { ChannelID = vm.ChannelID };
             string relativeUrl = "/MKTService/ProductRecommend/Create";
             restClient.Create(relativeUrl, model, callback);
         }

         public void GetPosition(int pageType, Action<List<CodeNamePair>> cb)
         {
             string relativeUrl = "/MKTService/ProductRecommend/GetPosition/" + pageType.ToString();
             restClient.Query<List<CodeNamePair>>(relativeUrl, (s, args) =>
             {
                 if (args.FaultsHandle() || cb == null) return;

                 cb(args.Result);
             });
         }

         public void GetBrandPosition(int pageID, string companyCode, string channelID, Action<List<CodeNamePair>> cb)
         {
             string relativeUrl = string.Format("/MKTService/ProductRecommend/GetBrandPosition/{0}/{1}/{2}", pageID, companyCode, channelID);
             restClient.Query<List<CodeNamePair>>(relativeUrl, (s, args) =>
             {
                 if (args.FaultsHandle() || cb == null) return;

                 cb(args.Result);
             });
         }
    }
}
