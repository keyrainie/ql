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
using ECCentral.Portal.UI.Common.Models;
using ECCentral.QueryFilter.Common;
using ECCentral.BizEntity.Common;
using System.Collections.Generic;

namespace ECCentral.Portal.UI.Common.Facades
{
    public class CurrencyFacade
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

        public CurrencyFacade(IPage page)
        {
            this.Page = page;
            ServiceBaseUrl = page.Context.Window.Configuration.GetConfigValue("Common", "ServiceBaseUrl");
            restClient = new RestClient(ServiceBaseUrl, page);
        }

        public void QueryCurrencyList(EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            string relativeUrl = "/CommonService/Currency/QueryCurrencyList";

            restClient.Query<List<CurrencyInfo>>(relativeUrl, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }
                callback(this, new RestClientEventArgs<dynamic>(args.Result, this.Page));
            });
        }

        public void Create(CurrencyInfoVM infoVM, EventHandler<RestClientEventArgs<CurrencyInfoVM>> callback)
        {
            string relativeUrl = "/CommonService/Currency/CreateCurrency";
            var msg = infoVM.ConvertVM<CurrencyInfoVM, CurrencyInfo>((s, t) =>
            {
                t.CurrencyName = s.CurrencyName;
                t.CurrencySymbol = s.CurrencySymbol;
                t.ExchangeRate = s.exchangeRate;
                t.ListOrder = s.ListOrder;
                t.IsLocal = s.IsLocal;
                t.Status = s.Status == CurrencyStatus.Active ? 0 : -1;
            });
            restClient.Create<CurrencyInfoVM>(relativeUrl, msg, (s, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }
                infoVM = args.Result;
                callback(s, new RestClientEventArgs<CurrencyInfoVM>(infoVM, restClient.Page));

            });
        }

        public void Update(CurrencyInfoVM _viewMode, EventHandler<RestClientEventArgs<CurrencyInfoVM>> callback)
        {
            string relativeUrl = "/CommonService/Currency/UpdateCurrency";
            var msg = _viewMode.ConvertVM<CurrencyInfoVM, CurrencyInfo>((s, t) =>
            {
                if (string.IsNullOrEmpty(s.CurrencyName))
                    t.CurrencyName = s.CurrencyName;
                if (string.IsNullOrEmpty(s.CurrencySymbol))
                    t.CurrencySymbol = s.CurrencySymbol;
                if (s.ExchangeRate.HasValue)
                    t.ExchangeRate = s.ExchangeRate;
                if (s.IsLocal.HasValue)
                    t.IsLocal = s.IsLocal;
                if (s.Status.HasValue)
                    t.Status = s.Status == CurrencyStatus.Active ? 0 : -1;
                if (s.ListOrder.HasValue)
                    t.ListOrder = s.ListOrder;

            });
            restClient.Update(relativeUrl, msg, callback);

        }

        public List<CurrencyInfoVM> ConvertCurrencyInfoEntityToCurrencyInfoVm(List<CurrencyInfo> data)
        {
            var list = new List<CurrencyInfoVM>();
            data.ForEach(area =>
            {
                var vm = new CurrencyInfoVM();
                if (area.SysNo.HasValue)
                {
                    vm.SysNo = area.SysNo.Value;
                    vm.CurrencyID = area.CurrencyID;
                    vm.CurrencyName = area.CurrencyName;
                    vm.CurrencySymbol = area.CurrencySymbol;
                    vm.ExchangeRate = area.ExchangeRate;
                    vm.ListOrder = area.ListOrder;
                    vm.IsLocal = area.IsLocal;
                    vm.Status = area.Status == 0 ? CurrencyStatus.Active : CurrencyStatus.Deactive;
                }
                list.Add(vm);
            });
            return list;
        }

        public void Load(int? sysNo, EventHandler<RestClientEventArgs<CurrencyInfoVM>> callback)
        {
            string relativeUrl = "/CommonService/Currency/LoadCurrency/" + sysNo;
            if (sysNo.HasValue)
            {
                restClient.Query<CurrencyInfo>(relativeUrl, (obj, args) =>
                {
                    if (args.FaultsHandle())
                    {
                        return;
                    }

                    CurrencyInfo entity = args.Result;
                    CurrencyInfoVM _viewModel = null;
                    if (entity == null)
                    {
                        _viewModel = new CurrencyInfoVM();
                    }
                    else
                    {
                        _viewModel = entity.Convert<CurrencyInfo, CurrencyInfoVM>();
                    }

                    callback(obj, new RestClientEventArgs<CurrencyInfoVM>(_viewModel, restClient.Page));
                });
            }
            else
            {
                callback(new Object(), new RestClientEventArgs<CurrencyInfoVM>(new CurrencyInfoVM(), restClient.Page));
            }
        }

    }
}
