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
using Newegg.Oversea.Silverlight.Controls;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.BizEntity.Common;
using System.Collections.Generic;

namespace ECCentral.Portal.Basic.Components.UserControls.LanguagesDescription
{
    public class Facade
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

        public Facade(IPage page)
        {
            ServiceBaseUrl = page.Context.Window.Configuration.GetConfigValue("Common", "ServiceBaseUrl");
            restClient = new RestClient(ServiceBaseUrl, page);
        }



        public void Create(BizObjecLanguageDescVM item, EventHandler<RestClientEventArgs<bool>> callback)
        {
            BizObjectLanguageDesc entity = EntityConverter<BizObjecLanguageDescVM, BizObjectLanguageDesc>.Convert(item);
            string url = "/CommonService/BizObject/Create";
            restClient.Create<bool>(url, entity, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }
                callback(obj, args);
            });
        }

        public void Update(BizObjecLanguageDescVM item, Action<bool> callback)
        {
            BizObjectLanguageDesc entity = EntityConverter<BizObjecLanguageDescVM, BizObjectLanguageDesc>.Convert(item);
            string url = "/CommonService/BizObject/Update";
            restClient.Update<bool>(url, entity, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }
                callback(args.Result);
            });
        }


        public void LoadBizObjectLanguageDescList(string bizObjectType, int? bizObjectSysNo,string bizObjectID, EventHandler<RestClientEventArgs<List<BizObjecLanguageDescVM>>> callback)
        {
            BizObjecLanguageDescVM vm = new BizObjecLanguageDescVM();
            string relativeUrl = string.Empty;
            if (bizObjectSysNo.HasValue && bizObjectSysNo.Value > 0)
            {
                relativeUrl = string.Format("/CommonService/BizObject/GetBySysNo/{0}/{1}", bizObjectType, bizObjectSysNo.Value);
            }
            else
            {
                relativeUrl = string.Format("/CommonService/BizObject/Get/{0}/{1}/{2}", bizObjectType, bizObjectSysNo.HasValue ? bizObjectSysNo : 0, bizObjectID);
            }
              
            //restClient.Query<List<BizObjectLanguageDesc>>(relativeUrl, callback);
            restClient.Query<List<BizObjectLanguageDesc>>(relativeUrl,(obj, args) =>
                {
                    if (args.FaultsHandle())
                    {
                        return;
                    }
                    List<BizObjecLanguageDescVM> list = new List<BizObjecLanguageDescVM>();
                    if (args.Result != null && args.Result.Count > 0)
                    {
                        foreach (BizObjectLanguageDesc item in args.Result)
                        {
                            vm = item.Convert<BizObjectLanguageDesc, BizObjecLanguageDescVM>();
                            list.Add(vm);
                        }
                    }
                   
                    callback(obj, new RestClientEventArgs<List<BizObjecLanguageDescVM>>(list, restClient.Page));
                });

        }
    }
}
