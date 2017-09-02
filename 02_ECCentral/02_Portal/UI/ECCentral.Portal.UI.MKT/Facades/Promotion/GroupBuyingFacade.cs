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
using ECCentral.Portal.UI.MKT.Models;
using ECCentral.QueryFilter.Common;
using ECCentral.QueryFilter.MKT;
using ECCentral.BizEntity.MKT;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using System.Collections.Generic;
using ECCentral.Portal.UI.MKT.Models.GroupBuying;
using ECCentral.BizEntity.MKT.Promotion;
using ECCentral.Service.MKT.Restful.RequestMsg;

namespace ECCentral.Portal.UI.MKT.Facades
{
    public class GroupBuyingFacade
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
        public GroupBuyingFacade(IPage page)
        {
            _Page = page;
            ServiceBaseUrl = page.Context.Window.Configuration.GetConfigValue("MKT", "ServiceBaseUrl");
            restClient = new RestClient(ServiceBaseUrl, page);
        }

        public void Query(GroupBuyingQueryVM vm, PagingInfo pagingInfo, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            var data = vm.ConvertVM<GroupBuyingQueryVM, GroupBuyingQueryFilter>();
            data.PagingInfo = pagingInfo;
            string relativeUrl = "/MKTService/GroupBuying/Query";
            restClient.QueryDynamicData(relativeUrl, data, callback);
        }

        public void Create(GroupBuyingMaintainVM vm, EventHandler<RestClientEventArgs<GroupBuyingInfo>> callback)
        {
            vm.CompanyCode = CPApplication.Current.CompanyCode;
            GroupBuyingInfo entity = VtoE(vm);

            string relativeUrl = "/MKTService/GroupBuying/Create";
            restClient.Create(relativeUrl, entity, callback);
        }

        public void Update(GroupBuyingMaintainVM vm, EventHandler<RestClientEventArgs<GroupBuyingInfo>> callback)
        {
            GroupBuyingInfo entity = VtoE(vm);
            string relativeUrl = "/MKTService/GroupBuying/Update";
            restClient.Update(relativeUrl, entity, callback);
        }

        public void Load(string sysNo, EventHandler<RestClientEventArgs<GroupBuyingMaintainVM>> callback)
        {
            string relativeUrl = "/MKTService/GroupBuying/" + sysNo;
            restClient.Query<GroupBuyingInfo>(relativeUrl, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }

                GroupBuyingInfo entity = args.Result;
                GroupBuyingMaintainVM _viewModel = null;
                if (entity == null)
                {
                    _viewModel = new GroupBuyingMaintainVM();
                }
                else
                {
                    _viewModel = EtoV(entity);
                }

                callback(obj, new RestClientEventArgs<GroupBuyingMaintainVM>(_viewModel, restClient.Page));
            });
        }

        public void GetGroupBuyingTypes(EventHandler<RestClientEventArgs<Dictionary<int, string>>> callback)
        {
            string relativeUrl = "/MKTService/GroupBuying/GetGroupBuyingTypes";
            restClient.Query<Dictionary<int, string>>(relativeUrl, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }
                callback(obj, new RestClientEventArgs<Dictionary<int, string>>(args.Result, _Page));
            });
        }

        public void GetGroupBuyingAreas(EventHandler<RestClientEventArgs<Dictionary<int, string>>> callback)
        {
            string relativeUrl = "/MKTService/GroupBuying/GetGroupBuyingAreas";
            restClient.Query<Dictionary<int, string>>(relativeUrl, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }
                callback(obj, new RestClientEventArgs<Dictionary<int, string>>(args.Result, _Page));
            });
        }

        public void GetGroupBuyingVendors(EventHandler<RestClientEventArgs<Dictionary<int, string>>> callback)
        {
            string relativeUrl = "/MKTService/GroupBuying/GetGroupBuyingVendors";
            restClient.Query<Dictionary<int, string>>(relativeUrl, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }
                callback(obj, new RestClientEventArgs<Dictionary<int, string>>(args.Result, _Page));
            });
        }

        public void Void(List<int> sysNoList, Action<bool> callback)
        {
            string relativeUrl = "/MKTService/GroupBuying/GroupBuyingVoid";

            restClient.Query<bool>(relativeUrl, sysNoList, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }
                callback(true);
            });
        }

        public void Stop(List<int> sysNoList, Action<bool> callback)
        {
            string relativeUrl = "/MKTService/GroupBuying/GroupBuyingStop";

            restClient.Query<bool>(relativeUrl, sysNoList, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }
                callback(true);
            });
        }

        public void SubmitAudit(int sysNo, Action<bool> callback)
        {
            string relativeUrl = "/MKTService/GroupBuying/GroupBuyingSubmitAudit";
            restClient.Update(relativeUrl, sysNo, (obj, args) =>
            {
                if (args.FaultsHandle()) { return; }
                callback(true);
            });
        }

        public void CancelAudit(int sysNo, Action<bool> callback)
        {
            string relativeUrl = "/MKTService/GroupBuying/GroupBuyingCancelAudit";
            restClient.Update(relativeUrl, sysNo, (obj, args) =>
            {
                if (args.FaultsHandle()) { return; }
                callback(true);
            });
        }

        public void AuditApprove(int sysNo, string reasonStr, Action<bool> callback)
        {
            string relativeUrl = "/MKTService/GroupBuying/GroupBuyingAuditApprove";
            GroupBuyingAuditReq msg = new GroupBuyingAuditReq()
            {
                SysNo = sysNo,
                Reasons = reasonStr
            };
            restClient.Update(relativeUrl, msg, (obj, args) =>
            {
                if (args.FaultsHandle()) { return; }
                callback(true);
            });
        }

        public void AuditRefuse(int sysNo, string reasonStr, Action<bool> callback)
        {
            string relativeUrl = "/MKTService/GroupBuying/GroupBuyingAuditRefuse";
            GroupBuyingAuditReq msg = new GroupBuyingAuditReq()
            {
                SysNo = sysNo,
                Reasons = reasonStr
            };
            restClient.Update(relativeUrl, msg, (obj, args) =>
            {
                if (args.FaultsHandle()) { return; }
                callback(true);
            });
        }

        public void LoadMarginRateInfo(GroupBuyingMaintainVM vm, EventHandler<RestClientEventArgs<List<GroupBuySaveInfoVM>>> callback)
        {
            GroupBuyingInfo entity = VtoE(vm);

            string relativeUrl = "/MKTService/GroupBuying/LoadMarginRateInfo";
            restClient.Query<List<GroupBuySaveInfo>>(relativeUrl, entity, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }

                List<GroupBuySaveInfoVM> infoVMList = new List<GroupBuySaveInfoVM>();
                foreach (GroupBuySaveInfo info in args.Result)
                {
                    infoVMList.Add(EntityConverter<GroupBuySaveInfo, GroupBuySaveInfoVM>.Convert(info));

                }
                callback(obj, new RestClientEventArgs<List<GroupBuySaveInfoVM>>(infoVMList, _Page));
            });
        }

        public void GetProductOriginalPrice(int productSysNo, string isByGroup, EventHandler<RestClientEventArgs<List<object>>> callback)
        {
            OriginalPriceReq req = new OriginalPriceReq
            {
                ProductSysNo = productSysNo,
                IsByGroup = isByGroup,
                CompanyCode = CPApplication.Current.CompanyCode
            };
            string relativeUrl = "/MKTService/GroupBuying/GetProductOriginalPrice";

            restClient.Query<List<object>>(relativeUrl, req, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }
                if (args.Result.Count > 4)
                {
                    args.Result[3] = args.Result[3] == null ? 0
                            : int.Parse(string.IsNullOrEmpty(args.Result[3].ToString()) ? "0" : args.Result[3].ToString());
                    args.Result[4] = args.Result[4] == null ? "" : args.Result[4].ToString();
                }
                callback(obj, args);
            });
        }
        /// <summary>
        /// 获取随心配在团购中的毛利率
        /// </summary>
        /// <param name="vm"></param>
        /// <param name="callback"></param>
        public void GetProductPromotionMargionByGroupBuying(GroupBuyingMaintainVM vm, EventHandler<RestClientEventArgs<string>> callback)
        {
            string relativeUrl = "/MKTService/GroupBuying/GetProductPromotionMarginByGroupBuying";
            restClient.Query(relativeUrl, VtoE(vm), callback);
        }

        private GroupBuyingMaintainVM EtoV(GroupBuyingInfo entity)
        {
            GroupBuyingMaintainVM viewmodel = entity.Convert<GroupBuyingInfo, GroupBuyingMaintainVM>((en, vm) =>
            {
                if (en.PriceRankList.Count == 1)
                {
                    vm.SysNo1 = en.PriceRankList[0].ProductSysNo.Value;
                    vm.SellCount1 = en.PriceRankList[0].MinQty.ToString();
                    vm.GroupBuyingPrice1 = en.PriceRankList[0].DiscountValue.ToString();
                    //显示第一个阶梯价格
                    vm.Price = vm.GroupBuyingPrice1;
                }
                if (en.PriceRankList.Count == 2)
                {
                    vm.SysNo1 = en.PriceRankList[0].ProductSysNo.Value;
                    vm.SellCount1 = en.PriceRankList[0].MinQty.ToString();
                    vm.GroupBuyingPrice1 = en.PriceRankList[0].DiscountValue.ToString();

                    vm.SysNo2 = en.PriceRankList[1].ProductSysNo.Value;
                    vm.SellCount2 = en.PriceRankList[1].MinQty.ToString();
                    vm.GroupBuyingPrice2 = en.PriceRankList[1].DiscountValue.ToString();
                }
                if (en.PriceRankList.Count == 3)
                {
                    vm.SysNo1 = en.PriceRankList[0].ProductSysNo.Value;
                    vm.SellCount1 = en.PriceRankList[0].MinQty.ToString();
                    vm.GroupBuyingPrice1 = en.PriceRankList[0].DiscountValue.ToString();

                    vm.SysNo2 = en.PriceRankList[1].ProductSysNo.Value;
                    vm.SellCount2 = en.PriceRankList[1].MinQty.ToString();
                    vm.GroupBuyingPrice2 = en.PriceRankList[1].DiscountValue.ToString();

                    vm.SysNo3 = en.PriceRankList[2].ProductSysNo.Value;
                    vm.SellCount3 = en.PriceRankList[2].MinQty.ToString();
                    vm.GroupBuyingPrice3 = en.PriceRankList[2].DiscountValue.ToString();
                }

                vm.GroupBuyingReason = en.Reasons;
            });

            return viewmodel;
        }

        private GroupBuyingInfo VtoE(GroupBuyingMaintainVM vm)
        {
            var entity = vm.ConvertVM<GroupBuyingMaintainVM, GroupBuyingInfo>((v, en) =>
            {
                en.GroupBuyingDesc = new BizEntity.LanguageContent(vm.GroupBuyingDesc);
                en.GroupBuyingDescLong = new BizEntity.LanguageContent(vm.GroupBuyingDescLong);
                en.GroupBuyingPicUrl = new BizEntity.LanguageContent(vm.GroupBuyingPicUrl);
                en.GroupBuyingMiddlePicUrl = new BizEntity.LanguageContent(vm.GroupBuyingMiddlePicUrl);
                en.GroupBuyingSmallPicUrl = new BizEntity.LanguageContent(vm.GroupBuyingSmallPicUrl);
                en.GroupBuyingTitle = new BizEntity.LanguageContent(vm.GroupBuyingTitle);
                en.GroupBuyingRules = new BizEntity.LanguageContent(vm.GroupBuyingRules);
            });
            entity.VendorStoreSysNoList = new List<int>();
            if (vm.VendorStoreList != null)
            {
                foreach (var item in vm.VendorStoreList)
                {
                    if (item.IsChecked)
                    {
                        entity.VendorStoreSysNoList.Add(item.SysNo.Value);
                    }
                }
            }
            entity.PriceRankList = vm.ConvertPriceRank();
            return entity;
        }

        public void CreateGroupBuyingCategory(GroupBuyingCategoryVM vm, EventHandler<RestClientEventArgs<GroupBuyingCategoryInfo>> callback)
        {
            var msg = EntityConverter<GroupBuyingCategoryVM, GroupBuyingCategoryInfo>.Convert(vm);
            msg.CompanyCode = CPApplication.Current.CompanyCode;
            
            string relativeUrl = "/MKTService/GroupBuying/CreateGroupBuyingCategory";
            restClient.Create(relativeUrl, msg, callback);
        }

        public void UpdateGroupBuyingCategory(GroupBuyingCategoryVM vm, EventHandler<RestClientEventArgs<GroupBuyingCategoryInfo>> callback)
        {
            var msg = EntityConverter<GroupBuyingCategoryVM, GroupBuyingCategoryInfo>.Convert(vm);

            string relativeUrl = "/MKTService/GroupBuying/UpdateGroupBuyingCategory";
            restClient.Update(relativeUrl, msg, callback);
        }

        public void GetAllGroupBuyingCategory(EventHandler<RestClientEventArgs<List<GroupBuyingCategoryInfo>>> callback)
        {            
            string relativeUrl = "/MKTService/GroupBuying/GetAllGroupBuyingCategory";
            restClient.Query<List<GroupBuyingCategoryInfo>>(relativeUrl,  callback);
        }       

        public void BatchReadGroupbuyingFeedback(List<int> data, EventHandler<RestClientEventArgs<string>> callback)
        {
            string relativeUrl = "/MKTService/GroupBuying/BatchReadGroupbuyingFeedback";
            restClient.Update(relativeUrl, data, callback);
        }

        public void BatchHandleGroupbuyingBusinessCooperation(List<int> data, EventHandler<RestClientEventArgs<string>> callback)
        {
            string relativeUrl = "/MKTService/GroupBuying/BatchHandleGroupbuyingBusinessCooperation";
            restClient.Update(relativeUrl, data, callback);
        }

        public void HandleGroupbuyingBusinessCooperation(int sysNo, EventHandler<RestClientEventArgs<string>> callback)
        {
            string relativeUrl = "/MKTService/GroupBuying/HandleGroupbuyingBusinessCooperation";
            restClient.Update(relativeUrl, sysNo, callback);
        }

        public void QueryGroupBuyingFeedback(FeedbackQueryVM vm, PagingInfo pagingInfo, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            var data = vm.ConvertVM<FeedbackQueryVM, GroupBuyingFeedbackQueryFilter>();
            data.CompanyCode = CPApplication.Current.CompanyCode;
            data.PagingInfo = pagingInfo;
            string relativeUrl = "/MKTService/GroupBuying/QueryGroupbuyingFeedback";
            restClient.QueryDynamicData(relativeUrl, data, callback);
        }

        public void QueryBusinessCooperation(BusinessCooperationQueryVM vm, PagingInfo pagingInfo, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            var data = vm.ConvertVM<BusinessCooperationQueryVM, BusinessCooperationQueryFilter>();
            data.PagingInfo = pagingInfo;
            data.CompanyCode = CPApplication.Current.CompanyCode;
            string relativeUrl = "/MKTService/GroupBuying/QueryBusinessCooperation";
            restClient.QueryDynamicData(relativeUrl, data, callback);
        }

        public void QueryGroupBuyingTicket(GroupBuyingTicketQueryVM vm, PagingInfo pagingInfo, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            var data = vm.ConvertVM<GroupBuyingTicketQueryVM, GroupBuyingTicketQueryFilter>();
            data.PagingInfo = pagingInfo;
            data.CompanyCode = CPApplication.Current.CompanyCode;
            string relativeUrl = "/MKTService/GroupBuying/QueryGroupBuyingTicket";
            restClient.QueryDynamicData(relativeUrl, data, callback);
        }

        public void QuerySettlement(GroupBuyingSettlementQueryVM vm, PagingInfo pagingInfo, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            var data = vm.ConvertVM<GroupBuyingSettlementQueryVM, GroupBuyingSettlementQueryFilter>();
            data.PagingInfo = pagingInfo;
            data.CompanyCode = CPApplication.Current.CompanyCode;
            string relativeUrl = "/MKTService/GroupBuying/QuerySettlement";
            restClient.QueryDynamicData(relativeUrl, data, callback);
        }

        public void BatchAuditGroupBuyingSettlement(List<int> data, EventHandler<RestClientEventArgs<string>> callback)
        {
            string relativeUrl = "/MKTService/GroupBuying/BatchAuditGroupBuyingSettlement";
            restClient.Update(relativeUrl, data, callback);
        }

        public void LoadGroupBuyingSettlementItemBySettleSysNo(int settlementSysNo, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            string relativeUrl = "/MKTService/GroupBuying/LoadGroupBuyingSettlementItemBySettleSysNo";
            restClient.QueryDynamicData(relativeUrl, settlementSysNo, callback);
        }

        public void LoadTicketByGroupBuyingSysNo(int groupBuyingSysNo, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            string relativeUrl = "/MKTService/GroupBuying/LoadTicketByGroupBuyingSysNo";
            restClient.QueryDynamicData(relativeUrl, groupBuyingSysNo, callback);
        }

        public void BatchVoidGroupBuyingTicket(List<int> sysNos, EventHandler<RestClientEventArgs<string>> callback)
        {
            string relativeUrl = "/MKTService/GroupBuying/BatchVoidGroupBuyingTicket";
            restClient.Update(relativeUrl, sysNos, callback);
        }
    }
}