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
    public class FreeShippingChargeRuleFacade
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

        public FreeShippingChargeRuleFacade(IPage page)
        {
            this.Page = page;
            ServiceBaseUrl = page.Context.Window.Configuration.GetConfigValue("Common", "ServiceBaseUrl");
            restClient = new RestClient(ServiceBaseUrl, page);
        }

        public void Query(FreeShippingChargeRuleQueryVM model, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            FreeShippingChargeRuleQueryFilter filter = model.ConvertVM<FreeShippingChargeRuleQueryVM, FreeShippingChargeRuleQueryFilter>();

            string relativeUrl = "/CommonService/FreeShippingChargeRule/Query";

            restClient.QueryDynamicData(relativeUrl, filter, (_, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }
                callback(this, new RestClientEventArgs<dynamic>(args.Result, this.Page));
            });
        }

        public void Load(int sysno, EventHandler<RestClientEventArgs<FreeShippingChargeRuleVM>> callback)
        {
            string relativeUrl = string.Format("/CommonService/FreeShippingChargeRule/{0}", sysno);

            restClient.Query<FreeShippingChargeRuleInfo>(relativeUrl, (_, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }
                FreeShippingChargeRuleVM model = args.Result.Convert<FreeShippingChargeRuleInfo, FreeShippingChargeRuleVM>();
                callback(this, new RestClientEventArgs<FreeShippingChargeRuleVM>(model, this.Page));
            });
        }

        public void Save(FreeShippingChargeRuleVM model, EventHandler<RestClientEventArgs<FreeShippingChargeRuleVM>> callback)
        {
            string relativeUrl = "/CommonService/FreeShippingChargeRule";

            FreeShippingChargeRuleInfo info = model.ConvertVM<FreeShippingChargeRuleVM, FreeShippingChargeRuleInfo>();
            if (!info.SysNo.HasValue)
            {
                restClient.Create<FreeShippingChargeRuleInfo>(relativeUrl + "/Create", info, (_, args) =>
                {
                    if (args.FaultsHandle())
                    {
                        return;
                    }
                    model = args.Result.Convert<FreeShippingChargeRuleInfo, FreeShippingChargeRuleVM>();
                    callback(this, new RestClientEventArgs<FreeShippingChargeRuleVM>(model, this.Page));
                });
            }
            else
            {
                restClient.Update(relativeUrl + "/Update", info, (_, args) =>
                {
                    if (args.FaultsHandle())
                    {
                        return;
                    }
                    callback(this, new RestClientEventArgs<FreeShippingChargeRuleVM>(model, this.Page));
                });
            }
        }

        public void Valid(int sysno, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            string relativeUrl = "/CommonService/FreeShippingChargeRule/Valid";
            restClient.Update(relativeUrl, sysno, callback);
        }

        public void Invalid(int sysno, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            string relativeUrl = "/CommonService/FreeShippingChargeRule/Invalid";
            restClient.Update(relativeUrl, sysno, callback);
        }

        public void Delete(int sysno, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            string relativeUrl = "/CommonService/FreeShippingChargeRule/Delete";
            restClient.Delete(relativeUrl, sysno, callback);
        }
    }
}
