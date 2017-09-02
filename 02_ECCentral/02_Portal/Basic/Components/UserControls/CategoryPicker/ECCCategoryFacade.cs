using ECCentral.BizEntity.IM;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.QueryFilter.IM;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using Newegg.Oversea.Silverlight.Controls;
using System;
using System.Collections.Generic;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace ECCentral.Portal.Basic.Components.UserControls.CategoryPicker
{
    public class ECCCategoryFacade
    {
        private readonly RestClient restClient;

        public ECCCategoryFacade(IPage page)
        {
            restClient = new RestClient(CPApplication.Current.CurrentPage.Context.Window.Configuration.GetConfigValue("MKT", "ServiceBaseUrl"), page);

        }
        public void QueryCategoryLevel(string categoryLevel, CategoryQueryFilter queryFilter, EventHandler<RestClientEventArgs<List<CategoryInfo>>> callback)
        {
            queryFilter.CompanyCode = CPApplication.Current.CompanyCode;
            string relativeUrl = string.Format("/MKTService/ECCategory/QueryCategory{0}", categoryLevel);
            restClient.Query<List<CategoryInfo>>(relativeUrl, queryFilter, callback);
        }

        public void QueryAllCategory2(CategoryQueryFilter queryFilter, EventHandler<RestClientEventArgs<List<CategoryInfo>>> callback)
        {
            queryFilter.CompanyCode = CPApplication.Current.CompanyCode;
            string relativeUrl = string.Format("/MKTService/ECCategory/QueryAllCategory{0}", 2);
            restClient.Query<List<CategoryInfo>>(relativeUrl, queryFilter, callback);
        }

        public void QueryAllCategory3(CategoryQueryFilter queryFilter, EventHandler<RestClientEventArgs<List<CategoryInfo>>> callback)
        {
            queryFilter.CompanyCode = CPApplication.Current.CompanyCode;
            string relativeUrl = string.Format("/MKTService/ECCategory/QueryAllCategory{0}", 3);
            restClient.Query<List<CategoryInfo>>(relativeUrl, queryFilter, callback);
        }

        ///// <summary>
        ///// 获取2级分类信息
        ///// </summary>
        ///// <param name="c2SysNo"></param>
        ///// <param name="callback"></param>
        //public void QueryCategory2Info(string c2SysNo, EventHandler<RestClientEventArgs<CategoryInfo>> callback)
        //{
        //    string relativeUrl = string.Format("/MKTService/Category/QueryCategory2Info/{0}", c2SysNo);
        //    restClient.Query<CategoryInfo>(relativeUrl, callback);
        //}
        ///// <summary>
        ///// 获取3级分类信息
        ///// </summary>
        ///// <param name="c2SysNo"></param>
        ///// <param name="callback"></param>
        //public void QueryCategory3Info(string c3SysNo, EventHandler<RestClientEventArgs<CategoryInfo>> callback)
        //{
        //    string relativeUrl = string.Format("/MKTService/Category/QueryCategory3Info/{0}", c3SysNo);
        //    restClient.Query<CategoryInfo>(relativeUrl, callback);
        //}


        ///// <summary>
        ///// 查询类别数据
        ///// 本方法不区分1，2，3类别
        ///// 目前供IM类别控件使用
        ///// </summary>
        ///// <param name="queryFilter"></param>
        ///// <param name="callback"></param>
        //public void QueryAllPrimaryCategory(CategoryQueryFilter queryFilter, EventHandler<RestClientEventArgs<List<CategoryInfo>>> callback)
        //{
        //    string relativeUrl = "/MKTService/Category/QueryAllPrimaryCategory";
        //    restClient.Query<List<CategoryInfo>>(relativeUrl, queryFilter, callback);
        //}
    }
}
