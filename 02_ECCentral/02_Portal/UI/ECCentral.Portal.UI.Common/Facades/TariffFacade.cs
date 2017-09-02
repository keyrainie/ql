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
using ECCentral.BizEntity.Common;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.Portal.UI.Common.Models;
using ECCentral.QueryFilter.Common;
using Newegg.Oversea.Silverlight.Controls;

namespace ECCentral.Portal.UI.Common.Facades
{
    public class TariffFacade
    {
         private RestClient restClient;
        /// <summary>
        /// 服务基地址
        /// </summary>
        protected string ServiceBaseUrl
        {
            get;
            private set;
        }

        public TariffFacade(IPage page)
        {
            ServiceBaseUrl = page.Context.Window.Configuration.GetConfigValue("Common", "ServiceBaseUrl");
            restClient = new RestClient(ServiceBaseUrl, page);
        }


      /// <summary>
      /// 创建税率信息
      /// </summary>
      /// <param name="item"></param>
      /// <param name="callback"></param>
        public void CreateTariffInfo(TariffInfoVM item, EventHandler<RestClientEventArgs<TariffInfo>> callback)
        {
            TariffInfo entity = EntityConverter<TariffInfoVM, TariffInfo>.Convert(item);
            string url = "/CommonService/Tariff/Create";
            restClient.Create<TariffInfo>(url, entity, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }
                callback(obj, args);
            });
        }

        
        /// <summary>
        /// 更新税率规则信息
        /// </summary>
        /// <param name="item"></param>
        /// <param name="callback"></param>
        public void UpdateTariffInfo(TariffInfoVM item, Action<bool> callback)
        {
            TariffInfo entity = EntityConverter<TariffInfoVM, TariffInfo>.Convert(item);
            string url = "/CommonService/Tariff/Update";
            restClient.Update<bool>(url, entity, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }
                callback(args.Result);
            });
        }

           /// <summary>
        /// 加载税率规则信息
        /// </summary>
        /// <param name="yyCardSysNo"></param>
        /// <param name="callback"></param>
        public void LoadTariffInfo(int sysNo, EventHandler<RestClientEventArgs<TariffInfo>> callback)
        {
            string url = string.Format("/CommonService/Tariff/Load/{0}", sysNo);
            restClient.Query<TariffInfo>(url, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }
                callback(obj, args);
            });
        }

        /// <summary>
        /// 税率规格信息查询
        /// </summary>
        /// <param name="queryVM"></param>
        /// <param name="PageSize"></param>
        /// <param name="PageIndex"></param>
        /// <param name="SortField"></param>
        /// <param name="callback"></param>
        public void TariffInfoQuery(TariffInfoQueryFilterVM queryVM, int PageSize, int PageIndex, string SortField, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            TariffInfoQueryFilter queryFilter = new TariffInfoQueryFilter();
            queryFilter = queryVM.ConvertVM<TariffInfoQueryFilterVM, TariffInfoQueryFilter>();
            queryFilter.PagingInfo = new PagingInfo
            {
                PageSize = PageSize,
                PageIndex = PageIndex,
                SortBy = SortField
            };
            string relativeUrl = "/CommonService/Tariff/Query";
            restClient.QueryDynamicData(relativeUrl, queryFilter, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }
                callback(obj, args);
            });
        }
    }
}
