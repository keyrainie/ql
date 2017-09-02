using System;
using System.Net;
using System.Linq;
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
using System.Collections.Generic;
using ECCentral.Service.IM.Restful;
using ECCentral.BizEntity.IM;
using ECCentral.BizEntity.Common;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using ECCentral.QueryFilter;
using ECCentral.Portal.UI.MKT.Models;
using ECCentral.Portal.Basic.Components.UserControls.ProductPicker;
using ECCentral.QueryFilter.Common;
using ECCentral.QueryFilter.IM;
using System.Collections.ObjectModel;

namespace ECCentral.Portal.UI.MKT.Facades
{
    public class GiftCardFacade
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

        public GiftCardFacade(IPage page)
        {
            ServiceBaseUrl = page.Context.Window.Configuration.GetConfigValue("IM", "ServiceBaseUrl");
            restClient = new RestClient(ServiceBaseUrl, page);
        }

        /// <summary>
        /// 查询礼品卡
        /// </summary>
        /// <param name="callback"></param>
        public void QueryGiftCardInfo(ECCentral.QueryFilter.IM.GiftCardFilter filter, int PageSize, int PageIndex, string SortField, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            filter.PageInfo = new ECCentral.QueryFilter.Common.PagingInfo()
            {
                PageSize = PageSize,
                PageIndex = PageIndex,
                SortBy = SortField
            };
            string relativeUrl = "/IMService/GiftCardInfo/QueryGiftCardInfo";
            restClient.QueryDynamicData(relativeUrl, filter, callback);
        }

        public void ExportExcelFile(ECCentral.QueryFilter.IM.GiftCardFilter filter, ColumnSet[] columns)
        {
            string relativeUrl = "/IMService/GiftCardInfo/QueryGiftCardInfo";
            restClient.ExportFile(relativeUrl, filter, columns);
        }


        /// <summary>
        /// 批量作废
        /// </summary>
        /// <param name="items"></param>
        /// <param name="callback"></param>
        public void BatchSetGiftCardInvalid(List<GiftCardInfo> items, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            string relativeUrl = "/IMService/GiftCardInfo/BatchSetGiftCardInvalid";
            items.ForEach(p => p.EditUser = new UserInfo { SysNo = CPApplication.Current.LoginUser.userSysNo, UserName = CPApplication.Current.LoginUser.LoginName, UserDisplayName = CPApplication.Current.LoginUser.DisplayName });
            restClient.Update(relativeUrl, items, callback);
        }

        /// <summary>
        /// 作废
        /// </summary>
        /// <param name="items"></param>
        /// <param name="callback"></param>
        public void SetGiftCardInvalid(GiftCardInfo item, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            string relativeUrl = "/IMService/GiftCardInfo/SetGiftCardInvalid";
            item.EditUser = new UserInfo { SysNo = CPApplication.Current.LoginUser.userSysNo, UserName = CPApplication.Current.LoginUser.LoginName, UserDisplayName = CPApplication.Current.LoginUser.DisplayName };
            restClient.Update(relativeUrl, item, callback);
        }

        /// <summary>
        /// 批量锁定
        /// </summary>
        /// <param name="items"></param>
        /// <param name="callback"></param>
        public void BatchLockGiftCard(List<int> items, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            string relativeUrl = "/IMService/GiftCardInfo/BatchLockGiftCard";
            restClient.Update(relativeUrl, items, callback);
        }

        /// <summary>
        /// 批量解锁
        /// </summary>
        /// <param name="items"></param>
        /// <param name="callback"></param>
        public void BatchUnLockGiftCard(List<int> items, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            string relativeUrl = "/IMService/GiftCardInfo/BatchUnLockGiftCard";
            restClient.Update(relativeUrl, items, callback);
        }

        /// <summary>
        /// 更新 
        /// </summary>
        /// <param name="item"></param>
        /// <param name="callback"></param>
        public void UpdateGiftCardInfo(ECCentral.BizEntity.IM.GiftCardInfo item, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            string relativeUrl = "/IMService/GiftCardInfo/UpdateGiftCardInfo";
            item.EditUser = new UserInfo { SysNo = CPApplication.Current.LoginUser.userSysNo, UserName = CPApplication.Current.LoginUser.LoginName, UserDisplayName = CPApplication.Current.LoginUser.DisplayName };
            restClient.Update(relativeUrl, item, callback);
        }

        /// <summary>
        /// 获取礼品卡操作日志
        /// </summary>
        /// <param name="code"></param>
        /// <param name="callback"></param>
        public void GetGiftCardOperateLogByCode(string code, EventHandler<RestClientEventArgs<List<ECCentral.BizEntity.IM.GiftCardOperateLog>>> callback)
        {
            string relativeUrl = "/IMService/GiftCardInfo/GetGiftCardOperateLogByCode";
            restClient.Query<List<ECCentral.BizEntity.IM.GiftCardOperateLog>>(relativeUrl, code, callback);
        }

        /// <summary>
        /// 获取礼品卡操作日志
        /// </summary>
        /// <param name="code"></param>
        /// <param name="callback"></param>
        public void GetGiftCardRedeemLogJoinSOMaster(string code, EventHandler<RestClientEventArgs<List<ECCentral.BizEntity.IM.GiftCardRedeemLog>>> callback)
        {
            string relativeUrl = "/IMService/GiftCardInfo/GetGiftCardRedeemLogJoinSOMaster";
            restClient.Query<List<ECCentral.BizEntity.IM.GiftCardRedeemLog>>(relativeUrl, code, callback);
        }

        #region 礼品卡制作

        /// <summary>
        /// 查询礼品卡
        /// </summary>
        /// <param name="callback"></param>
        public void QueryGiftCardFabricationMaster(ECCentral.QueryFilter.IM.GiftCardFabricationFilter filter, int PageSize, int PageIndex, string SortField, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            filter.PageInfo = new ECCentral.QueryFilter.Common.PagingInfo()
            {
                PageSize = PageSize,
                PageIndex = PageIndex,
                SortBy = SortField
            };
            string relativeUrl = "/IMService/GiftCardInfo/QueryGiftCardFabricationMaster";
            restClient.QueryDynamicData(relativeUrl, filter, callback);
        }

        public void ExportFabricationInfoExcelFile(ECCentral.QueryFilter.IM.GiftCardFabricationFilter filter, ColumnSet[] columns)
        {
            string relativeUrl = "/IMService/GiftCardInfo/QueryGiftCardFabricationMaster";
            restClient.ExportFile(relativeUrl, filter, columns);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sysNo"></param>
        /// <param name="callback"></param>
        public void GetGiftCardFabricationItem(int sysNo, EventHandler<RestClientEventArgs<GiftCardFabricationItemRsp>> callback)
        {
            string relativeUrl = "/IMService/GiftCardInfo/GetGiftCardFabricationItem";
            restClient.Query<GiftCardFabricationItemRsp>(relativeUrl, sysNo, callback);
        }

        #endregion

        /// <summary>
        /// 更新礼品卡制作单
        /// </summary>
        /// <param name="item"></param>
        /// <param name="callback"></param>
        public void UpdateGiftCardFabrications(GiftCardFabricationMaster item, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            string relativeUrl = "/IMService/GiftCardInfo/UpdateGiftCardFabrications";
            restClient.Update(relativeUrl, item, callback);
        }

        //public void ResetGiftCardFabrication(int sysNo, EventHandler<RestClientEventArgs<dynamic>> callback)
        //{
        //    string relativeUrl = "/IMService/GiftCardInfo/ResetGiftCardFabrication";
        //    restClient.Update(relativeUrl, sysNo, callback);
        //}

        public void DeleteGiftCardFabrication(int sysNo, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            string relativeUrl = "/IMService/GiftCardInfo/DeleteGiftCardFabrication";
            restClient.Update(relativeUrl, sysNo, callback);
        }

        public void CreatePOGiftCardFabrication(GiftCardFabricationMaster item, EventHandler<RestClientEventArgs<int>> callback)
        {
            string relativeUrl = "/IMService/GiftCardInfo/CreatePOGiftCardFabrication";
            restClient.Create<int>(relativeUrl, item, callback);
        }

        /// <summary>
        /// 导出制卡信息
        /// </summary>
        /// <param name="sysNo"></param>
        /// <param name="callback"></param>
        public void GetAddGiftCardInfoList(int sysNo, EventHandler<RestClientEventArgs<bool>> callback)
        {
            string relativeUrl = "/IMService/GiftCardInfo/GetAddGiftCardInfoList";
            restClient.Query<bool>(relativeUrl, sysNo, callback);
        }

        /// <summary>
        /// 导出新生成的礼品卡信息
        /// </summary>
        /// <param name="sysNo"></param>
        /// <param name="columns"></param>
        public void ExportGiftCardExcelFile(int sysNo, ColumnSet[] columns)
        {
            string relativeUrl = "/IMService/GiftCardInfo/GetGiftCardInfoByGiftCardFabricationSysNo";
            restClient.ExportFile(relativeUrl, sysNo, columns);
        }

        public void BatchActivateGiftCard(List<int> sysNoList, Action<string> callback)
        {
            string relativeUrl = "/IMService/GiftCardInfo/BatchActivateGiftCard";

            restClient.Update<string>(relativeUrl, sysNoList, (obj, args) =>
            {
                if (args.FaultsHandle())
                    return;

                callback(args.Result);
            });
        }

        #region GiftVoucher

        public void QueryGiftCardProduct(GiftCardProductFilterVM vm, int PageSize, int PageIndex, string SortField, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            GiftCardProductFilter filter = vm.ConvertVM<GiftCardProductFilterVM, GiftCardProductFilter>();

            filter.PageInfo = new ECCentral.QueryFilter.Common.PagingInfo()
            {
                PageSize = PageSize,
                PageIndex = PageIndex,
                SortBy = SortField
            };
            string relativeUrl = "/IMService/GiftCardInfo/QueryGiftCardProductInfo";
            restClient.QueryDynamicData(relativeUrl, filter, (obj, args) =>
            {
                if (args.FaultsHandle())
                    return;
                callback(obj, args);
            });
        }

        /// <summary>
        /// 查询礼品券关联商品
        /// </summary>
        /// <param name="vm"></param>
        /// <param name="PageSize"></param>
        /// <param name="PageIndex"></param>
        /// <param name="SortField"></param>
        /// <param name="callback"></param>
        public void QueryGiftVoucherProductRelation(GiftCardProductFilterVM vm, int PageSize, int PageIndex, string SortField,
            Action<ObservableCollection<GiftVoucherProductRelationVM>, int> callback)
        {

            GiftCardProductFilter filter = vm.ConvertVM<GiftCardProductFilterVM, GiftCardProductFilter>();

            filter.PageInfo = new ECCentral.QueryFilter.Common.PagingInfo()
            {
                PageSize = PageSize,
                PageIndex = PageIndex,
                SortBy = SortField
            };
            string relativeUrl = "/IMService/GiftCardInfo/QueryGiftVoucherProductRelation";
            restClient.QueryDynamicData(relativeUrl, filter, (obj, args) =>
            {
                if (args.FaultsHandle())
                    return;

                ObservableCollection<GiftVoucherProductRelationVM> result = DynamicConverter<GiftVoucherProductRelationVM>
                    .ConvertToVMList<ObservableCollection<GiftVoucherProductRelationVM>>(args.Result.Rows);

                foreach (var item in result)
                {
                    foreach (var source in args.Result.Rows.ToList())
                    {
                        if (source.SysNo == item.SysNo)
                        {
                            item.RelationStatus = source.Status;
                            //add by cesc 2013-10-30 
                            //如果不将请求类型'删除'手动更改,每次点页面上的'保存'按钮时,会将未审核通过的请求加载出来
                            if (item.Type == GVRReqType.Delete)
                            {
                                item.Type = null;
                            }
                        }
                    }
                }

                if (result.Count > 0)
                {
                    // 根据商品编号获取商品信息
                    PagingInfo paging = new PagingInfo
                    {
                        PageIndex = 0,
                        PageSize = 1,
                    };

                    int count = 0;

                    foreach (var item in result)
                    {
                        restClient.QueryDynamicData("/IMService/Product/QueryProduct", new ProductQueryFilter()
                        {
                            PagingInfo = paging,
                            ProductSysNo = item.ProductSysNo
                        }, (obj1, args1) =>
                        {
                            if (args1.FaultsHandle())
                            {
                                return;
                            }
                            List<ProductVM> productLst = DynamicConverter<ProductVM>.ConvertToVMList<List<ProductVM>>(args1.Result.Rows);
                            ProductVM pvm = productLst.FirstOrDefault();
                            if (null != pvm)
                            {
                                GiftVoucherProductRelationVM vpvm = EntityConverter<ProductVM, GiftVoucherProductRelationVM>.Convert(pvm, (s, t) =>
                                {
                                    t.ProductSysNo = s.SysNo.Value;
                                    t.ProductStatus = s.Status;
                                    t.SaleInWeb = s.GiftVoucherType == 1 ? true : false;
                                });

                                item.ProductID = vpvm.ProductID;
                                item.ProductName = vpvm.ProductName;
                                item.ProductType = vpvm.ProductType;
                                item.ProductStatus = vpvm.ProductStatus;
                                item.AvailableQty = vpvm.AvailableQty;
                                item.InventoryType = vpvm.InventoryType;
                                item.IsConsign = vpvm.IsConsign;
                                item.SaleInWeb = vpvm.SaleInWeb;
                            }

                            count++;

                            if (count == result.Count)
                            {
                                callback(result, args.Result.TotalCount);
                            }
                        });
                    }
                }
            });
        }

        /// <summary>
        /// 查询礼品券关联请求
        /// </summary>
        /// <param name="vm"></param>
        /// <param name="PageSize"></param>
        /// <param name="PageIndex"></param>
        /// <param name="SortField"></param>
        /// <param name="callback"></param>
        public void QueryGiftVoucherProductRelationReq(GiftCardProductFilterVM vm, int PageSize, int PageIndex, string SortField, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            GiftCardProductFilter filter = vm.ConvertVM<GiftCardProductFilterVM, GiftCardProductFilter>();

            filter.PageInfo = new ECCentral.QueryFilter.Common.PagingInfo()
            {
                PageSize = PageSize,
                PageIndex = PageIndex,
                SortBy = SortField
            };
            string relativeUrl = "/IMService/GiftCardInfo/QueryGiftVoucherProductRelationReq";
            restClient.QueryDynamicData(relativeUrl, filter, (obj, args) =>
            {
                if (args.FaultsHandle())
                    return;

                //ObservableCollection<GiftVoucherProductRelationVM>;
                callback(obj, args);
            });
        }


        /// <summary>
        /// Batch audit gift card product
        /// </summary>
        /// <param name="sysNos"></param>
        /// <param name="callback"></param>
        public void BatchAuditGiftCardProduct(List<int> sysNos, Action<string> callback)
        {
            string relativeUrl = "/IMService/GiftCardInfo/BatchAuditGiftCardProduct";

            restClient.Update<string>(relativeUrl, sysNos, (obj, args) =>
            {
                if (args.FaultsHandle())
                    return;

                callback(args.Result);
            });
        }

        /// <summary>
        /// Batch cancel audit gift card product
        /// </summary>
        /// <param name="sysNos"></param>
        /// <param name="callback"></param>
        public void BatchCancelAuditGiftCardProduct(List<int> sysNos, Action<string> callback)
        {
            string relativeUrl = "/IMService/GiftCardInfo/BatchCancelAuditGiftCardProduct";

            restClient.Update<string>(relativeUrl, sysNos, (obj, args) =>
            {
                if (args.FaultsHandle())
                    return;

                callback(args.Result);
            });
        }

        /// <summary>
        /// Batch void gift card product
        /// </summary>
        /// <param name="sysNos"></param>
        /// <param name="callback"></param>
        public void BatchVoidGiftCardProduct(List<int> sysNos, Action<string> callback)
        {
            string relativeUrl = "/IMService/GiftCardInfo/BatchVoidGiftCardProduct";

            restClient.Update<string>(relativeUrl, sysNos, (obj, args) =>
            {
                if (args.FaultsHandle())
                    return;

                callback(args.Result);
            });
        }

        /// <summary>
        /// 添加礼品券商品
        /// </summary>
        /// <param name="voucherProductVM"></param>
        /// <param name="callback"></param>
        public void AddGiftVoucherProductInfo(GiftCardProductVM voucherProductVM, Action<int> callback)
        {
            string relativeUrl = "/IMService/GiftCardInfo/AddGiftVoucherProductInfo";
            GiftVoucherProduct entity = new GiftVoucherProduct();
            entity.RelationProducts = new List<GiftVoucherProductRelation>();

            entity = voucherProductVM.ConvertVM<GiftCardProductVM, GiftVoucherProduct>((s, t) =>
            {
            });

            restClient.Create<int>(relativeUrl, entity, (obj, args) =>
            {

                if (args.FaultsHandle())
                {
                    return;
                }
                callback(args.Result);
            });
        }

        /// <summary>
        /// 加载礼品券商品
        /// </summary>
        /// <param name="sysNo"></param>
        /// <param name="callback"></param>
        public void GetGiftVoucherProductInfo(int sysNo, Action<GiftCardProductVM> callback)
        {
            string relativeUrl = string.Format("/IMService/GiftCardInfo/GetGiftVoucherProductInfo/{0}", sysNo);
            restClient.Query<GiftVoucherProduct>(relativeUrl, (obj, args) =>
            {
                if (args.FaultsHandle())
                    return;
                GiftVoucherProduct entity = args.Result;

                var vm = entity.Convert<GiftVoucherProduct, GiftCardProductVM>((s, w) =>
                {
                });

                foreach (var item in vm.RelationProducts)
                {
                    item.RelationStatus = entity.RelationProducts.Where(p => p.SysNo == item.SysNo).FirstOrDefault().Status;
                }

                int count = 0;

                if (vm != null && vm.RelationProducts != null && vm.RelationProducts.Count <= 0)
                {
                    callback(vm);
                }

                if (vm != null && vm.RelationProducts.Count > 0)
                {
                    // 根据商品编号获取商品信息
                    PagingInfo paging = new PagingInfo
                    {
                        PageIndex = 0,
                        PageSize = 1,
                    };

                    foreach (var item in vm.RelationProducts)
                    {
                        restClient.QueryDynamicData("/IMService/Product/QueryProduct", new ProductQueryFilter()
                        {
                            PagingInfo = paging,
                            ProductSysNo = item.ProductSysNo
                        }, (obj1, args1) =>
                        {
                            if (args1.FaultsHandle())
                            {
                                return;
                            }
                            List<ProductVM> productLst = DynamicConverter<ProductVM>.ConvertToVMList<List<ProductVM>>(args1.Result.Rows);
                            ProductVM pvm = productLst.FirstOrDefault();
                            if (null != pvm)
                            {
                                GiftVoucherProductRelationVM vpvm = EntityConverter<ProductVM, GiftVoucherProductRelationVM>.Convert(pvm, (s, t) =>
                                {
                                    t.ProductSysNo = s.SysNo.Value;
                                    t.ProductStatus = s.Status;
                                    t.SaleInWeb = s.GiftVoucherType == 1 ? true : false;
                                });

                                item.ProductID = vpvm.ProductID;
                                item.ProductName = vpvm.ProductName;
                                item.ProductType = vpvm.ProductType;
                                item.ProductStatus = vpvm.ProductStatus;
                                item.AvailableQty = vpvm.AvailableQty;
                                item.InventoryType = vpvm.InventoryType;
                                item.IsConsign = vpvm.IsConsign;
                                item.SaleInWeb = vpvm.SaleInWeb;
                            }

                            count++;

                            if (count == vm.RelationProducts.Count)
                            {
                                callback(vm);
                            }
                        });
                    }
                }
            });
        }

        /// <summary>
        /// 更新礼品券商品
        /// </summary>
        /// <param name="voucherProductVM"></param>
        /// <param name="callback"></param>
        public void UpdateVoucherProductInfo(GiftCardProductVM voucherProductVM, Action<GiftCardProductVM> callback)
        {
            string relativeUrl = "/IMService/GiftCardInfo/UpdateVoucherProductInfo";
            GiftVoucherProduct entity = new GiftVoucherProduct();
            entity.RelationProducts = new List<GiftVoucherProductRelation>();

            entity = voucherProductVM.ConvertVM<GiftCardProductVM, GiftVoucherProduct>((s, t) =>
            {
                foreach (var s1 in s.RelationProducts)
                {

                }
            });

            entity.Status = GiftVoucherProductStatus.Audit;

            restClient.Create<GiftVoucherProduct>(relativeUrl, entity, (obj, args) =>
            {
                if (args.FaultsHandle())
                    return;

                var vm = args.Result.Convert<GiftVoucherProduct, GiftCardProductVM>();

                callback(vm);
            });
        }


        /// <summary>
        /// Batch audit gift card relation product
        /// </summary>
        /// <param name="sysNos"></param>
        /// <param name="callback"></param>
        public void BatchAuditVoucherRequest(List<int> sysNos, Action<string> callback)
        {
            string relativeUrl = "/IMService/GiftCardInfo/BatchAuditVoucherRequest";

            restClient.Update<string>(relativeUrl, sysNos, (obj, args) =>
            {
                if (args.FaultsHandle())
                    return;

                callback(args.Result);
            });
        }

        /// <summary>
        /// Batch cancel audit gift card relation product
        /// </summary>
        /// <param name="sysNos"></param>
        /// <param name="callback"></param>
        public void BatchCancelAuditVoucherRequest(List<int> sysNos, Action<string> callback)
        {
            string relativeUrl = "/IMService/GiftCardInfo/BatchCancelAuditVoucherRequest";

            restClient.Update<string>(relativeUrl, sysNos, (obj, args) =>
            {
                if (args.FaultsHandle())
                    return;

                callback(args.Result);
            });
        }

        public void GetGiftVoucherProductRelationRequest(int sysno, Action<List<GiftVoucherProductRelationReqVM>> callback)
        {
            string relativeUrl = string.Format("/IMService/GiftCardInfo/GetGiftVoucherProductRelationRequest/{0}", sysno);

            restClient.Query<List<GiftVoucherProductRelationRequest>>(relativeUrl, (obj, args) =>
            {
                if (args.FaultsHandle())
                    return;

                List<GiftVoucherProductRelationReqVM> vmList = EntityConverter<List<GiftVoucherProductRelationRequest>, List<GiftVoucherProductRelationReqVM>>.Convert(args.Result);

                callback(vmList);
            });
        }



        #endregion


        public void GetGiftCardC3SysNo(Action<int?> callback)
        {
            string relativeUrl = "/CommonService/Common/GetSystemConfigurationValue/GiftCardCategory";

            restClient.Query<string>(relativeUrl, (obj, args) =>
            {
                int? c3No = null;
                if (!args.FaultsHandle())
                {
                    int no = 0;
                    if (int.TryParse(args.Result, out no))
                    {
                        c3No = no;
                    }
                }
                callback(c3No);
            });
        }
    }
}
