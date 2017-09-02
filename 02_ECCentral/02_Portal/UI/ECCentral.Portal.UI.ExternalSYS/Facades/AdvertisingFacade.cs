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
using ECCentral.Portal.UI.ExternalSYS.Models;
using ECCentral.QueryFilter.ExternalSYS;
using ECCentral.Portal.Basic;
using ECCentral.BizEntity.ExternalSYS;

namespace ECCentral.Portal.UI.ExternalSYS.Facades
{
    public class AdvertisingFacade
    {
         private readonly RestClient restClient;
        private string serviceBaseUrl = CPApplication.Current.CurrentPage.Context.Window.Configuration.GetConfigValue(ConstValue.DomainName_ExternalSYS, ConstValue.Key_ServiceBaseUrl);

        public AdvertisingFacade()
        {
            restClient = new RestClient(serviceBaseUrl);
        }

        public AdvertisingFacade(IPage page)
        {
            restClient = new RestClient(serviceBaseUrl, page);
        }

        public void Query(AdvertisingQueryVM model, int PageSize, int PageIndex, string SortField, EventHandler<RestClientEventArgs<dynamic>> callback) 
        {
            string GetProductLineByQueryUrl = "ExternalSYSService/Advertising/AdvertisingQuery";

            AdvertisingQueryFilter filter = model.ConvertVM<AdvertisingQueryVM, AdvertisingQueryFilter>();
            filter.PageInfo = new QueryFilter.Common.PagingInfo
            {
                PageSize = PageSize,
                PageIndex = PageIndex,
                SortBy = SortField
            };
            restClient.QueryDynamicData(GetProductLineByQueryUrl, filter, callback);
        }

        public void Load(int sysNo, EventHandler<RestClientEventArgs<AdvertisingVM>> callback)
        {
            string relativeUrl = string.Format("ExternalSYSService/Advertising/{0}", sysNo);
            restClient.Query<AdvertisingInfo>(relativeUrl, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }
                AdvertisingInfo entity = args.Result;
                AdvertisingVM _viewModel = new AdvertisingVM();

                callback(obj, new RestClientEventArgs<AdvertisingVM>(_viewModel, restClient.Page));
            });
        }

        public void Create(AdvertisingVM model, EventHandler<RestClientEventArgs<int?>> callback)
        {
            string relativeUrl = "ExternalSYSService/Advertising/Create";
            AdvertisingInfo msg = model.ConvertVM<AdvertisingVM, AdvertisingInfo>();
            msg.CompanyCode = CPApplication.Current.CompanyCode;
            restClient.Create<int?>(relativeUrl, msg, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }
                model.SysNo = args.Result;
                callback(obj, args);
            });
        }

        public void Update(AdvertisingVM model, EventHandler<RestClientEventArgs<AdvertisingVM>> callback)
        {
            string relativeUrl = "ExternalSYSService/Advertising/Update";
            AdvertisingInfo msg = model.ConvertVM<AdvertisingVM, AdvertisingInfo>();
            restClient.Update<AdvertisingInfo>(relativeUrl, msg, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }
                var vm = args.Result.Convert<AdvertisingInfo, AdvertisingVM>();
                RestClientEventArgs<AdvertisingVM> e = new RestClientEventArgs<AdvertisingVM>(vm, restClient.Page);
                callback(obj, e);
            });
        }
        
        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="SysNo"></param>
        /// <param name="callback"></param>
        public void Delete(int SysNo, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            string relativeUrl = "ExternalSYSService/Advertising/Delete";
            restClient.Delete(relativeUrl, SysNo, callback);

        }

        /// <summary>
        /// 获取生产线分类
        /// </summary>
        /// <param name="callback"></param>
        public void GetAllProductLineCategory(EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            string GetAllProductLineCategoryUrl = "ExternalSYSService/ProductLine/GetAllProductLineCategory";
            restClient.QueryDynamicData(GetAllProductLineCategoryUrl, null, callback);
        }

        /// <summary>
        /// 根据产品线类别SysNo得到产品线
        /// </summary>
        /// <param name="sysNo"></param>
        /// <param name="callback"></param>
        public void GetProductLineByProductLineCategorySysNo(int sysNo, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            string GetAllProductLineCategoryUrl = "ExternalSYSService/ProductLine/GetProductLineByProductLineCategorySysNo";
            restClient.QueryDynamicData(GetAllProductLineCategoryUrl, sysNo, callback);
        }
        
    }
}
