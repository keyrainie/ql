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
using Newegg.Oversea.Silverlight.Controls;
using ECCentral.BizEntity.MKT;
using System.Collections.Generic;
using ECCentral.BizEntity.Common;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using ECCentral.QueryFilter.Common;
using ECCentral.Portal.UI.MKT.Models;
using ECCentral.QueryFilter.MKT;

namespace ECCentral.Portal.UI.MKT.Facades
{
    public class BrandRecommendedFacade
    {
        private readonly RestClient restClient;
        private const string GetBrandRecommendedListUrl = "/MKTService/BrandRecommended/GetBrandRecommendedList";
        private const string UpdateBrandRecommendedUrl = "/MKTService/BrandRecommended/UpdateBrandRecommended";
        private const string CreateBrandRecommendedUrl = "/MKTService/BrandRecommended/CreateBrandRecommended";
        private const string GetCategory1ListUrl = "/MKTService/BrandRecommended/GetCategory1List";
        private const string GetCategory2ListUrl = "/MKTService/BrandRecommended/GetCategory2List";

        /// <summary>
        /// MKTService服务基地址
        /// </summary>
        protected string ServiceBaseUrl
        {
            get
            {
                return CPApplication.Current.CurrentPage.Context.Window.Configuration.GetConfigValue("MKT", "ServiceBaseUrl");
            }
        }

        public BrandRecommendedFacade()
        {
            restClient = new RestClient(ServiceBaseUrl);
        }

        public BrandRecommendedFacade(IPage page)
        {
            restClient = new RestClient(ServiceBaseUrl, page);

        }
        public void GetCategoryRelatedByQuery(BrandRecommendedQueryVM model, int PageSize, int PageIndex, string SortField, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            BrandRecommendedQueryFilter query = new BrandRecommendedQueryFilter() { BrandType = model.BrandType, LevelCode = model.LevelCode, LevelCodeParent = model.LevelCodeParent };
            query.PagingInfo = new PagingInfo() { PageIndex = PageIndex, PageSize = PageSize, SortBy = SortField };

            restClient.QueryDynamicData(GetBrandRecommendedListUrl, query, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }
                callback(obj, args);
            });
        }
        public void UpdateBrandRecommended(BrandRecommendedVM model, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            BrandRecommendedInfo info = new BrandRecommendedInfo()
            {
                //BrandRank = model.BrandRank,
                BrandSysNo = model.BrandSysNo.Value,
                Level_Name = model.Level_Name,
                Sysno = model.Sysno,
                User = new UserInfo() { UserName = CPApplication.Current.LoginUser.LoginName, SysNo = CPApplication.Current.LoginUser.UserSysNo }
            };
            restClient.Update(UpdateBrandRecommendedUrl, info, callback);
        }

        public void CreateBrandRecommended(BrandRecommendedVM model, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            BrandRecommendedInfo info = new BrandRecommendedInfo()
            {
                //BrandRank = model.BrandRank,
                BrandSysNo = model.BrandSysNo.Value,
                Level_No = model.Level_No,
                Level_Code = model.Level_Code,
                Level_Name = model.Level_Name,
                User = new UserInfo() { UserName = CPApplication.Current.LoginUser.LoginName, SysNo = CPApplication.Current.LoginUser.UserSysNo }
            };
            restClient.Create(CreateBrandRecommendedUrl, info, callback);
        }




        public void GetCategory1List(EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            restClient.QueryDynamicData(GetCategory1ListUrl, null, (obj, arg) =>
            {
                if (arg.FaultsHandle())
                {
                    return;
                }
                callback(obj, arg);
            });

        }

        public void GetCategory2List(EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            restClient.QueryDynamicData(GetCategory2ListUrl, null, (obj, arg) =>
            {
                if (arg.FaultsHandle())
                {
                    return;
                }
                callback(obj, arg);
            });
        }


    }
}
