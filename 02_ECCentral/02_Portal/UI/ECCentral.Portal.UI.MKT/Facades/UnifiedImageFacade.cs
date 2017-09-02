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
using ECCentral.QueryFilter.MKT;
using ECCentral.BizEntity.MKT;
using System.Collections.Generic;
using ECCentral.Portal.UI.MKT.Models;

namespace ECCentral.Portal.UI.MKT.Facades
{
    public class UnifiedImageFacade
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

        public UnifiedImageFacade(IPage page)
        {
            ServiceBaseUrl = page.Context.Window.Configuration.GetConfigValue("MKT", "ServiceBaseUrl");
            restClient = new RestClient(ServiceBaseUrl, page);
        }

        /// <summary>
        /// 查询图片
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="callback"></param>
        public void QueryUnifiedImages(UnifiedImageQueryFilterVM filter, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            string relativeUrl = "/MKTService/UnifiedImage/Query";
            var queryEntity= filter.ConvertVM<UnifiedImageQueryFilterVM, UnifiedImageQueryFilter>();
            restClient.QueryDynamicData(relativeUrl, queryEntity, callback);
        }

        
        /// <summary>
        /// 新增图片信息
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="callback"></param>
        public void CreateUnifiedImage(UnifiedImageVM vm, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            
            string relativeUrl = "/MKTService/UnifiedImage/Create";
            var entity = vm.ConvertVM<UnifiedImageVM, ECCentral.BizEntity.MKT.UnifiedImage>();
            restClient.Create(relativeUrl, entity, callback);
        }
    }
}
