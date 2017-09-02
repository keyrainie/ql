using System;
using System.Collections.Generic;

using ECCentral.BizEntity.MKT;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.Portal.UI.MKT.Models;
using ECCentral.QueryFilter.MKT;
using ECCentral.Service.MKT.Restful.RequestMsg;
using ECCentral.Portal.UI.MKT.Models.Promotion;

using Newegg.Oversea.Silverlight.ControlPanel.Core;
using Newegg.Oversea.Silverlight.Controls;

namespace ECCentral.Portal.UI.MKT.Facades
{
    public class OptionalAccessoriesFacade
    {
        private readonly RestClient restClient;
        private readonly IPage viewPage;

        protected string ServiceBaseUrl
        {
            get
            {
                return CPApplication.Current.CurrentPage.Context.Window.Configuration.GetConfigValue("MKT", "ServiceBaseUrl");
            }
        }

        public OptionalAccessoriesFacade(IPage page)
        {
            viewPage = page;
            restClient = new RestClient(ServiceBaseUrl, page);
        }

        public void Query(OptionalAccessoriesQueryReqVM vm, int PageSize, int PageIndex, string SortField, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            OptionalAccessoriesQueryFilter filter = vm.ConvertVM<OptionalAccessoriesQueryReqVM, OptionalAccessoriesQueryFilter>();
            filter.CompanyCode = CPApplication.Current.CompanyCode;
            filter.PagingInfo = new QueryFilter.Common.PagingInfo
            {
                PageSize = PageSize,
                PageIndex = PageIndex,
                SortBy = SortField
            };
            string relativeUrl = "/MKTService/OptionalAccessories/Query";
            restClient.QueryDynamicData(relativeUrl, filter, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }
                callback(obj, args);
            });
        }

        public void Load(int sysNo, EventHandler<RestClientEventArgs<OptionalAccessoriesVM>> callback)
        {
            string relativeUrl = string.Format("/MKTService/OptionalAccessories/{0}", sysNo);
            restClient.Query<OptionalAccessoriesInfo>(relativeUrl, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }
                //var vm = args.Result.Convert<OptionalAccessoriesInfo, OptionalAccessoriesVM>();
                //vm.Name = args.Result.Name.Content;
                //RestClientEventArgs<OptionalAccessoriesVM> e = new RestClientEventArgs<OptionalAccessoriesVM>(vm, this.viewPage);

                //callback(obj, e);

                OptionalAccessoriesInfo entity = args.Result;
                OptionalAccessoriesVM _viewModel = null;
                if (entity == null)
                {
                    _viewModel = new OptionalAccessoriesVM();
                }
                else
                {
                    _viewModel = EtoV(entity);
                }

                callback(obj, new RestClientEventArgs<OptionalAccessoriesVM>(_viewModel, this.viewPage));
            });
        }

        public void Create(OptionalAccessoriesVM model, EventHandler<RestClientEventArgs<int?>> callback)
        {
            string relativeUrl = "/MKTService/OptionalAccessories/Create";
            OptionalAccessoriesInfo msg = model.ConvertVM<OptionalAccessoriesVM, OptionalAccessoriesInfo>();
            msg.Name.Content = model.Name;
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

        public void Update(OptionalAccessoriesVM model, EventHandler<RestClientEventArgs<OptionalAccessoriesVM>> callback)
        {
            string relativeUrl = "/MKTService/OptionalAccessories/Update";
            OptionalAccessoriesInfo msg = model.ConvertVM<OptionalAccessoriesVM, OptionalAccessoriesInfo>();
            msg.Name.Content = model.Name;
            restClient.Update<OptionalAccessoriesInfo>(relativeUrl, msg, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }
                var vm = args.Result.Convert<OptionalAccessoriesInfo, OptionalAccessoriesVM>();
                RestClientEventArgs<OptionalAccessoriesVM> e = new RestClientEventArgs<OptionalAccessoriesVM>(vm, this.viewPage);
                callback(obj, e);
            });
        }

        public void ApproveOptionalAccessories(int sysNo, bool isPass, EventHandler<RestClientEventArgs<OptionalAccessoriesVM>> callback)
        {
            string relativeUrl = "/MKTService/OptionalAccessories/ApproveOptionalAccessories";
            ComboUpdateStatusReq msg = new ComboUpdateStatusReq
            {
                SysNo = sysNo,
                TargetStatus = isPass ? ComboStatus.Active : ComboStatus.Deactive
            };
            restClient.Update<OptionalAccessoriesVM>(relativeUrl, msg, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }
                callback(obj, args);
            });
        }

        public void CheckOptionalAccessoriesItemIsPass(OptionalAccessoriesVM model, EventHandler<RestClientEventArgs<List<string>>> callback)
        {
            string relativeUrl = "/MKTService/OptionalAccessories/CheckOptionalAccessoriesItemIsPass";
            OptionalAccessoriesInfo msg = model.ConvertVM<OptionalAccessoriesVM, OptionalAccessoriesInfo>();
            msg.CompanyCode = CPApplication.Current.CompanyCode;
            restClient.Create<List<string>>(relativeUrl, msg, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }
                callback(obj, args);
            });
        }

        public void CheckSaleRuleItemAndDiys(List<int> sysNos, EventHandler<RestClientEventArgs<List<string>>> callback)
        {
            string relativeUrl = "/MKTService/OptionalAccessories/CheckSaleRuleItemAndDiys";
            restClient.Create<List<string>>(relativeUrl, sysNos, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }
                callback(obj, args);
            });
        }

        private OptionalAccessoriesVM EtoV(OptionalAccessoriesInfo entity)
        {
            OptionalAccessoriesVM viewmodel = entity.Convert<OptionalAccessoriesInfo, OptionalAccessoriesVM>((en, vm) =>
            {
                vm.Name = en.Name.Content;
                foreach (var item in vm.Items)
                {
                    entity.Items.ForEach(i =>
                    {
                        if (i.SysNo == item.SysNo)
                        {
                            //Format折扣
                            var dpdTmp = i.Discount == null ? 0m : i.Discount.Value;
                            item.Discount = dpdTmp.ToString("0");
                            //Format折扣比例
                            dpdTmp = i.DiscountPercent == null ? 0m :
                                (i.DiscountPercent.Value <= 1 ? i.DiscountPercent.Value * 100 : i.DiscountPercent.Value);
                            item.DiscountPercent = dpdTmp.ToString("0");
                        }
                    });
                }
            });

            return viewmodel;
        }
    }
}