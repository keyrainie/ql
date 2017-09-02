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
using ECCentral.Portal.Basic.Utilities;
using ECCentral.QueryFilter.Common;
using ECCentral.Portal.UI.MKT.Models.Promotion.QueryFilter;
using ECCentral.QueryFilter.MKT;
using ECCentral.Portal.UI.MKT.Models.Promotion;
using ECCentral.BizEntity.MKT;
using System.Collections.Generic;
using ECCentral.Service.MKT.Restful.ResponseMsg;
using ECCentral.BizEntity.Inventory;
using ECCentral.BizEntity.Common;

namespace ECCentral.Portal.UI.MKT.Facades.Promotion
{
    public class CountdownFacade
    {
        private IPage _Page;
        private readonly RestClient restClient;
        /// <summary>
        /// 服务基地址
        /// </summary>
        protected string ServiceBaseUrl
        {
            get;
            private set;
        }
        public CountdownFacade(IPage page)
        {
            _Page = page;
            ServiceBaseUrl = page.Context.Window.Configuration.GetConfigValue("MKT", "ServiceBaseUrl");
            restClient = new RestClient(ServiceBaseUrl, page);
        }
        /// <summary>
        /// 查询
        /// </summary>
        /// <param name="vm"></param>
        /// <param name="pagingInfo"></param>
        /// <param name="callback"></param>
        public void Query(CountdownQueryFilterVM vm, PagingInfo pagingInfo, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            var data = vm.ConvertVM<CountdownQueryFilterVM, CountdownQueryFilter>((v, filter) =>
            {
                if (v.IsGroupOn.HasValue)
                {
                    filter.IsGroupOn = vm.IsGroupOn.Value ? "Y" : "N";
                }
                else
                {
                    filter.IsGroupOn = null;
                }

                if (v.IsShowCategory.HasValue && v.IsShowCategoryAll.HasValue && !v.IsShowCategoryAll.Value)
                {
                    filter.IsC1Show = v.IsShowCategory.Value ? "Y" : "N";
                    filter.IsC2Show = v.IsShowCategory.Value ? "N" : "Y";
                }
                else
                {
                    filter.IsC1Show = null;
                    filter.IsC2Show = null;
                }
                if (v.IsHomePageShow.HasValue)
                {
                    filter.IsHomePageShow = v.IsHomePageShowVal;
                }
                if (v.IsCountDownAreaShow.HasValue)
                {
                    filter.IsCountDownAreaShow = v.IsCountDownAreaShowVal;
                }
            });
            data.PageInfo = pagingInfo;
            string relativeUrl = "/MKTService/Countdown/Query";
            restClient.QueryDynamicData(relativeUrl, data, callback);
        }
        /// <summary>
        /// 加载
        /// </summary>
        /// <param name="sysNo"></param>
        /// <param name="callback"></param>
        public void Load(int sysNo, EventHandler<RestClientEventArgs<CountdownInfoVM>> callback)
        {
            string relativeUrl = string.Format("/MKTService/CountdownInfo/{0}", sysNo);

            restClient.Query<CountdownInfo>(relativeUrl, (o, arg) =>
            {
                CountdownInfoVM countdownInfoVM = arg.Result.Convert<CountdownInfo, CountdownInfoVM>((entity, vm) =>
                {
                    vm.StartTime = entity.StartTime;
                    vm.EndTime = entity.EndTime;
                });

                callback(o, new RestClientEventArgs<CountdownInfoVM>(countdownInfoVM, _Page));
            });
        }

        public void GetCountDownCreateUserList(string channelID, EventHandler<RestClientEventArgs<List<UserInfo>>> callback)
        {
            string relativeUrl = string.Format("/MKTService/GetAllCountdownCreateUser/{0}", channelID);
            restClient.Query<List<UserInfo>>(relativeUrl, callback);
        }

        public void GetQuickTimes(EventHandler<RestClientEventArgs<List<CodeNamePair>>> callback)
        {
            string relativeUrl = "/MKTService/Countdown/GetQuickTimes";
            restClient.Query<List<CodeNamePair>>(relativeUrl, callback);

        }
        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="CountdownVM"></param>
        /// <param name="callback"></param>
        public void Create(CountdownInfoVM CountdownVM, EventHandler<RestClientEventArgs<CountdownInfo>> callback)
        {
            var data = CountdownVM.ConvertVM<CountdownInfoVM, CountdownInfo>((v, en) =>
            {
            });

            string relativeUrl = "/MKTService/Countdown/Create";

            restClient.Create<CountdownInfo>(relativeUrl, data, callback);
        }
        /// <summary>
        /// 编辑更新
        /// </summary>
        /// <param name="CountdownVM"></param>
        /// <param name="callback"></param>
        public void Update(CountdownInfoVM CountdownVM, EventHandler<RestClientEventArgs<CountdownInfo>> callback)
        {
            var data = CountdownVM.ConvertVM<CountdownInfoVM, CountdownInfo>((v, en) =>
            {
                en.WebChannel = new BizEntity.Common.WebChannel() { ChannelID = v.ChannelID };
            });

            string relativeUrl = "/MKTService/Countdown/Update";

            restClient.Update<CountdownInfo>(relativeUrl, data, callback);
        }

        public void Verify(CountdownInfoVM CountdownVM, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            var data = CountdownVM.ConvertVM<CountdownInfoVM, CountdownInfo>((v, en) =>
            {
                en.WebChannel = new BizEntity.Common.WebChannel() { ChannelID = v.ChannelID };
            });

            string relativeUrl = "/MKTService/Countdown/Verify";

            restClient.Update(relativeUrl, data, callback);
        }

        public void CheckOptionalAccessoriesInfoMsg(CountdownInfoVM CountdownVM, EventHandler<RestClientEventArgs<string>> callback)
        {
            var data = CountdownVM.ConvertVM<CountdownInfoVM, CountdownInfo>((v, en) =>
            {
                en.WebChannel = new BizEntity.Common.WebChannel() { ChannelID = v.ChannelID };
            });

            string relativeUrl = "/MKTService/Countdown/CheckOptionalAccessoriesInfoMsg";

            restClient.Update<string>(relativeUrl, data, callback);
        }

        /// <summary>
        /// 加载毛利率相关
        /// </summary>
        /// <param name="CountdownVM"></param>
        /// <param name="callback"></param>
        public void LoadMarginRateInfo(CountdownInfoVM CountdownVM, EventHandler<RestClientEventArgs<string>> callback)
        { }
        /// <summary>
        /// 批量终止
        /// </summary>
        /// <param name="sysNoList"></param>
        /// <param name="callback"></param>
        public void Stop(CountdownInfoVM CountdownVM, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            var data = CountdownVM.ConvertVM<CountdownInfoVM, CountdownInfo>((v, en) =>
            {
                en.WebChannel = new BizEntity.Common.WebChannel() { ChannelID = v.ChannelID };
            });

            string relativeUrl = "/MKTService/Countdown/Interrupt";

            restClient.Update(relativeUrl, data, callback);
        }
        /// <summary>
        /// 批量作废
        /// </summary>
        /// <param name="sysNoList"></param>
        /// <param name="callback"></param>
        public void Void(CountdownInfoVM CountdownVM, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            var data = CountdownVM.ConvertVM<CountdownInfoVM, CountdownInfo>((v, en) =>
            {
                en.WebChannel = new BizEntity.Common.WebChannel() { ChannelID = v.ChannelID };
            });

            string relativeUrl = "/MKTService/Countdown/Abandon";

            restClient.Update(relativeUrl, data, callback);
        }
        /// <summary>
        /// 加载商品明细
        /// </summary>
        /// <param name="productSysNo"></param>
        public void LoadProductInfoDetail(string productSysNo, EventHandler<RestClientEventArgs<MKTProductDetailMsg>> callback)
        {
            string relativeUrl = string.Format("/MKTService/MKTProductDetailMsg/{0}", productSysNo);

            restClient.Query<MKTProductDetailMsg>(relativeUrl, callback);
        }
        /// <summary>
        /// 加载分仓数量列表
        /// </summary>
        public void LoadStockQtyList(string productSysNo, EventHandler<RestClientEventArgs<List<ProductInventoryInfo>>> callback)
        {
            string relativeUrl = string.Format("InventoryService/Inventory/GetProductInventoryInfo");
            restClient.Query<List<ProductInventoryInfo>>(relativeUrl, productSysNo, callback);
        }

        /// <summary>
        /// 加载分仓数量列表
        /// </summary>
        public void GetGrossMargin(CountdownInfoVM CountdownVM, EventHandler<RestClientEventArgs<GrossMarginMsg>> callback)
        {
            var data = CountdownVM.ConvertVM<CountdownInfoVM, CountdownInfo>((v, en) =>
            {
                en.WebChannel = new BizEntity.Common.WebChannel() { ChannelID = v.ChannelID };
            });
            string relativeUrl = "/MKTService/Countdown/GetGrossMargin";
            restClient.Query<GrossMarginMsg>(relativeUrl, data, callback);
        }

        public void GetCountdownVendorList(EventHandler<RestClientEventArgs<List<ECCentral.BizEntity.PO.VendorInfo>>> callback) 
        {
            string relativeUrl = "/MKTService/Countdown/GetVendorList";
            restClient.Query<List<ECCentral.BizEntity.PO.VendorInfo>>(relativeUrl, callback);
        }
        /// <summary>
        /// 导入促销计划
        /// </summary>
        /// <param name="fileIdentity"></param>
        /// <param name="callback"></param>
        public void BatchImportSchedule(CountdownInfo data, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            string relativeUrl = "/MKTService/Countdown/BatchImportSchedule";
            restClient.QueryDynamicData(relativeUrl, data, callback);
        }
        /// <summary>
        /// 导入限时抢购
        /// </summary>
        /// <param name="fileIdentity"></param>
        /// <param name="callback"></param>
        public void BatchImportCountDown(CountdownInfo data, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            string relativeUrl = "/MKTService/Countdown/BatchImportCountDown";
            restClient.QueryDynamicData(relativeUrl, data, callback);
        }

        /// <summary>
        /// 通过产品编号获取所属PM
        /// </summary>
        /// <param name="productSysNo"></param>
        /// <param name="callback"></param>
        public void GetPMByProductSysNo(int productSysNo, EventHandler<RestClientEventArgs<string>> callback)
        {
            var relativeUrl = string.Format("/MKTService/GetPMByProductSysNo/{0}", productSysNo);

            restClient.Query(relativeUrl, callback);
        }
    }
}
