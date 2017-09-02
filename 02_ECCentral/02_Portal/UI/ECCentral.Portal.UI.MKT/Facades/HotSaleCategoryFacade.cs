using System;
using ECCentral.Portal.Basic.Utilities;
using Newegg.Oversea.Silverlight.Controls;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using ECCentral.BizEntity.Common;
using ECCentral.Portal.UI.MKT.Models;
using ECCentral.QueryFilter.Common;
using ECCentral.QueryFilter.MKT;
using ECCentral.BizEntity.MKT;
using System.Collections.Generic;

namespace ECCentral.Portal.UI.MKT.Facades
{
    public class HotSaleCategoryFacade
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

        public HotSaleCategoryFacade(IPage page)
        {
            ServiceBaseUrl = page.Context.Window.Configuration.GetConfigValue("MKT", "ServiceBaseUrl");
            restClient = new RestClient(ServiceBaseUrl, page);
        }

        public void Create(HotSaleCategoryVM vm, Action cb)
        {
            var data = ConvertVMToEntity(vm);
            string relativeUrl = "/MKTService/HotSaleCategory/Insert";
            restClient.Create<object>(relativeUrl, data, (s, args) =>
            {
                if (args.FaultsHandle() || cb == null) return;
                cb();
            });
        }

        public void Update(HotSaleCategoryVM vm, bool? updateSameGroupAll, Action cb)
        {
            var data = ConvertVMToEntity(vm);

            string relativeUrl = "/MKTService/HotSaleCategory/Update";
            if (updateSameGroupAll.HasValue && updateSameGroupAll.Value == true)
            {
                relativeUrl = "/MKTService/HotSaleCategory/UpdateSameGroupAll";
            }
            restClient.Update(relativeUrl, data, (s, args) =>
            {
                if (args.FaultsHandle() || cb == null) return;
                cb();
            });
        }

        private HotSaleCategory ConvertVMToEntity(HotSaleCategoryVM vm)
        {
            var data = vm.ConvertVM<HotSaleCategoryVM, HotSaleCategory>();
            data.CompanyCode = CPApplication.Current.CompanyCode;
            data.WebChannel = new WebChannel
            {
                ChannelID = vm.ChannelID
            };
           
            return data;
        }

        public void Load(int sysNo, Action<HotSaleCategoryVM> callback)
        {
            string relativeUrl = "/MKTService/HotSaleCategory/" + sysNo.ToString();
            restClient.Query<HotSaleCategory>(relativeUrl, (obj, args) =>
                {
                    if (args.FaultsHandle() || callback == null) return;
                    var result = args.Result.Convert<HotSaleCategory, HotSaleCategoryVM>((info, vm) =>
                        {
                            if (info.WebChannel != null)
                            {
                                vm.ChannelID = info.WebChannel.ChannelID;
                            }
                        });
                    callback(result);
                });
        }

        public void Query(HotSaleCategoryQueryVM vm, PagingInfo pagingInfo, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            var data = vm.ConvertVM<HotSaleCategoryQueryVM, HotSaleCategoryQueryFilter>();
            data.PageInfo = pagingInfo;
            data.CompanyCode = CPApplication.Current.CompanyCode;
            string relativeUrl = "/MKTService/HotSaleCategory/Query";
            restClient.QueryDynamicData(relativeUrl, data, callback);
        }

        //删除
        public void Delete(int sysNo, Action cb)
        {
            string relativeUrl = "/MKTService/HotSaleCategory/Delete/"+sysNo.ToString();
            restClient.Update(relativeUrl, null, (s, args) =>
            {
                if (args.FaultsHandle() || cb == null) return;
                cb();
            });

        }

        public void GetPositionList(string companyCode, string channelID, int pageType,Action<List<CodeNamePair>> cb)
        {
            string relativeUrl = string.Format("/MKTService/HotSaleCategory/GetPosition/{0}/{1}/{2}", companyCode, channelID, pageType);
            restClient.Query<List<CodeNamePair>>(relativeUrl, (s, args) =>
            {
                if (args.FaultsHandle() || cb == null) return;
                cb(args.Result);
            });
        }
    }
}
