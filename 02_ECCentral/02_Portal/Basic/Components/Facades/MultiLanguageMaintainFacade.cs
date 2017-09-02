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
using ECCentral.BizEntity.Common;
using System.Collections.Generic;
namespace ECCentral.Portal.Basic.Components.Facades
{
    public class MultiLanguageMaintainFacade
    {
        private readonly RestClient restClient;

        public IPage Page { get; set; }
        /// <summary>
        /// 服务基地址
        /// </summary>
        protected string ServiceBaseUrl
        {
            get;
            private set;
        }

        public MultiLanguageMaintainFacade(IPage page)
        {
            this.Page = page;
            ServiceBaseUrl = page.Context.Window.Configuration.GetConfigValue("Common", "ServiceBaseUrl");
            restClient = new RestClient(ServiceBaseUrl, page);
        }

        public void QueryMultiLanguageBizEntity(MultiLanguageDataContract multiLanguageData, EventHandler<RestClientEventArgs<List<MultiLanguageBizEntity>>> callback)
        {

            string relativeUrl = "/CommonService/MultiLanguage/GetMultiLanguageBizEntityList";

            restClient.Query<List<MultiLanguageBizEntity>>(relativeUrl, multiLanguageData, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }
                callback(obj, new RestClientEventArgs<List<MultiLanguageBizEntity>>(args.Result, restClient.Page));
            });
        }

        public void SetMultiLanguageBizEntity(MultiLanguageBizEntity entity, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            string relativeUrl = "/CommonService/MultiLanguage/SetMultiLanguageBizEntity";

            restClient.Update(relativeUrl, entity, callback);
        }
    }
}
