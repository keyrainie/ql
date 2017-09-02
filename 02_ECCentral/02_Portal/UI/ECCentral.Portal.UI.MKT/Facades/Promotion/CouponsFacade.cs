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
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using Newegg.Oversea.Silverlight.Controls;
using ECCentral.Portal.UI.MKT.Models;
using ECCentral.BizEntity.MKT;
using Newegg.Oversea.Silverlight.Core.Components;
using ECCentral.BizEntity.Customer;
using System.Collections.Generic;
using ECCentral.BizEntity.Common;
using System.ServiceModel.DomainServices.Client;
using ECCentral.QueryFilter.MKT;
using ECCentral.Service.MKT.Restful.ResponseMsg;

namespace ECCentral.Portal.UI.MKT.Facades.Promotion
{
    public class CouponsFacade
    {
        private readonly RestClient restClient;

        public   CouponsInfoViewModel _viewModel;

        /// <summary>
        /// CustomerService服务基地址
        /// </summary>
        protected string ServiceBaseUrl
        {
            get
            {
                return CPApplication.Current.CurrentPage.Context.Window.Configuration.GetConfigValue("MKT", "ServiceBaseUrl");
            }
        }

        public CouponsFacade(IPage page,CouponsInfoViewModel vm)
        {
            restClient = new RestClient(ServiceBaseUrl,page);
            _viewModel = vm;
        }

        public CouponsFacade(IPage page)
        {
            restClient = new RestClient(ServiceBaseUrl, page);
        }

        public void Load(int? couponsSysNo,EventHandler<RestClientEventArgs<CouponsInfoViewModel>> callback)
        {
            if (couponsSysNo.HasValue)
            {
                string relativeUrl = string.Format("/MKTService/Coupons/{0}", couponsSysNo.Value);
                restClient.Query<CouponsInfo>(relativeUrl, (obj, args) =>
                    {
                        if (args.FaultsHandle())
                        {
                            return;
                        }

                        CouponsInfo entity = args.Result;
                        _viewModel = EtoV(entity);
                         
                        callback(obj, new RestClientEventArgs<CouponsInfoViewModel>(_viewModel, restClient.Page));
                    });
            }
            else
            {
                callback(new Object(), new RestClientEventArgs<CouponsInfoViewModel>(_viewModel, restClient.Page));
            }
        }

        public void GetCouponGrossMarginRate(int productSysNo, EventHandler<RestClientEventArgs<decimal>> callback)
        {
            string relativeUrl = string.Format("/MKTService/Coupons/GetCouponGrossMarginRate/{0}", productSysNo);
            restClient.Query<decimal>(relativeUrl, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }
                callback(obj, args);
            });            
        }
         
        public void SaveMaster(EventHandler<RestClientEventArgs<CouponsInfoViewModel>> callback)
        {
            if (!_viewModel.SysNo.HasValue)
            {
                string relativeUrl = "/MKTService/Coupons/CreateMaster";
                _viewModel.CompanyCode = CPApplication.Current.CompanyCode;
                restClient.Create<int?>(relativeUrl, VtoE(_viewModel), (obj, args) =>
                    {
                        if (args.FaultsHandle())
                        {
                            return;
                        }
                        _viewModel.SysNo = args.Result;
                        _viewModel.Status = CouponsStatus.Init;
                        callback(obj, new RestClientEventArgs<CouponsInfoViewModel>(_viewModel, restClient.Page));
                    });
            }
            else
            {
                string relativeUrl = "/MKTService/Coupons/UpdateMaster";

                restClient.Update<CouponsInfo>(relativeUrl, VtoE(_viewModel), (obj, args) =>
                {
                    if (args.FaultsHandle())
                    {
                        return;
                    }
                    CouponsInfo entity = args.Result;
                    _viewModel = new CouponsInfoViewModel();
                    _viewModel = EtoV(entity);
                    callback(obj, new RestClientEventArgs<CouponsInfoViewModel>(_viewModel, restClient.Page));
                });
            }
        }

        public void SetProductCondition(EventHandler<RestClientEventArgs<CouponsInfoViewModel>> callback)
        {
            string relativeUrl = "/MKTService/Coupons/SetProductCondition";

            restClient.Update<CouponsInfo>(relativeUrl, VtoE(_viewModel), (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }
                 
                callback(obj, new RestClientEventArgs<CouponsInfoViewModel>(_viewModel, restClient.Page));
            });
        }

        public void SetDiscountRule(EventHandler<RestClientEventArgs<CouponsInfoViewModel>> callback)
        {
            string relativeUrl = "/MKTService/Coupons/SetDiscountRule";

            restClient.Update<CouponsInfo>(relativeUrl, VtoE(_viewModel), (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }
                callback(obj, new RestClientEventArgs<CouponsInfoViewModel>(_viewModel, restClient.Page));
            });
        }

        public void SetSaleRuleEx(EventHandler<RestClientEventArgs<CouponsInfoViewModel>> callback)
        {
            string relativeUrl = "/MKTService/Coupons/SetSaleRuleEx";

            restClient.Update<CouponsInfo>(relativeUrl, VtoE(_viewModel), (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }
                callback(obj, new RestClientEventArgs<CouponsInfoViewModel>(_viewModel, restClient.Page));
            });
        }

        public void SetCustomerCondition(EventHandler<RestClientEventArgs<CouponsInfoViewModel>> callback)
        {
            string relativeUrl = "/MKTService/Coupons/SetCustomerCondition";

            restClient.Update<CouponsInfo>(relativeUrl, VtoE(_viewModel), (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }
                callback(obj, new RestClientEventArgs<CouponsInfoViewModel>(_viewModel, restClient.Page));
            });
        }

        public void QueryCouponCode(CouponCodeQueryFilterViewModel requestVM, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            string relativeUrl = "/MKTService/Coupons/CouponCode/Query";

            CouponCodeQueryFilter filter = requestVM.ConvertVM<CouponCodeQueryFilterViewModel, CouponCodeQueryFilter>();

            restClient.QueryDynamicData(relativeUrl,filter, (obj, args) =>
                {
                    if (args.FaultsHandle())
                    {
                        return;
                    }
                    callback(obj, args);
                });

        }

        public void CreateCouponCode(CouponCodeSettingViewModel codeVM, Action callback)
        {
            CouponCodeSetting entity = codeVM.ConvertVM<CouponCodeSettingViewModel, CouponCodeSetting>();
            string relativeUrl = "/MKTService/Coupons/CouponCode/Create";
            restClient.Create<CouponCodeSettingViewModel>(relativeUrl, entity, (obj, args) =>
                {
                    if (args.FaultsHandle())
                    {
                        return;
                    }
                    callback();
                });
            ;
        }

        public void DeleteCouponCodeList(List<int?> couponCodeSysNoList, Action callback)
        {
            string relativeUrl = "/MKTService/Coupons/CouponCode/DelCouponCode";
            restClient.Delete(relativeUrl, couponCodeSysNoList, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }
                callback();
            }); 
        }
        public void DeleteAllCouponCode(int? couponsSysNo, Action callback)
        {
            string relativeUrl = "/MKTService/Coupons/CouponCode/DelAllCouponCode";
            restClient.Delete(relativeUrl, couponsSysNo, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }
                callback();
            }); 
        }


        #region ViewModel 和Entity之间的双向Transform
        private CouponsInfoViewModel EtoV(CouponsInfo entity)
        {
            CouponsInfoViewModel viewmodel = entity.Convert<CouponsInfo, CouponsInfoViewModel>((en, vm) =>
            {
                vm.Title = entity.Title.Content;
            });

            if (entity.WebChannel != null)
            {
                viewmodel.WebChannel = new Basic.Components.Models.WebChannelVM() { ChannelID = entity.WebChannel.ChannelID, ChannelName = entity.WebChannel.ChannelName };
            }
            else
            {
                viewmodel.WebChannel = viewmodel.WebChennelList[0];
            }

            #region 商品条件处理
            if (viewmodel.ProductCondition != null)
            {
                if (viewmodel.ProductCondition.RelBrands != null && viewmodel.ProductCondition.RelBrands.BrandList != null)
                {
                    viewmodel.ProductCondition.RelBrands.IsExcludeRelation = !viewmodel.ProductCondition.RelBrands.IsIncludeRelation;
                    foreach (SimpleObjectViewModel sim in viewmodel.ProductCondition.RelBrands.BrandList)
                    {
                        sim.Relation = viewmodel.ProductCondition.RelBrands.IsIncludeRelation.Value ? PSRelationType.Include : PSRelationType.Exclude;
                        sim.IsChecked = false;
                    }
                }
                else
                {
                    if (viewmodel.ProductCondition.RelBrands == null) viewmodel.ProductCondition.RelBrands = new RelBrandViewModel();
                    viewmodel.ProductCondition.RelBrands.IsIncludeRelation = true;
                    viewmodel.ProductCondition.RelBrands.IsExcludeRelation = false;
                }

                if (viewmodel.ProductCondition.RelCategories != null && viewmodel.ProductCondition.RelCategories.CategoryList != null)
                {
                    viewmodel.ProductCondition.RelCategories.IsExcludeRelation = !viewmodel.ProductCondition.RelCategories.IsIncludeRelation;
                    foreach (SimpleObjectViewModel sim in viewmodel.ProductCondition.RelCategories.CategoryList)
                    {
                        sim.Relation = viewmodel.ProductCondition.RelCategories.IsIncludeRelation.Value ? PSRelationType.Include : PSRelationType.Exclude; ;
                        sim.IsChecked = false;
                    }
                }
                else
                {
                    if (viewmodel.ProductCondition.RelCategories == null) viewmodel.ProductCondition.RelCategories = new RelCategory3ViewModel ();
                    viewmodel.ProductCondition.RelCategories.IsIncludeRelation = true;
                    viewmodel.ProductCondition.RelCategories.IsExcludeRelation = false;                    
                }

                if (viewmodel.ProductCondition.RelProducts != null && viewmodel.ProductCondition.RelProducts.ProductList != null)
                {
                    foreach (RelProductAndQtyViewModel sim in viewmodel.ProductCondition.RelProducts.ProductList)
                    {
                        sim.IsChecked = false;
                    }
                }
                else
                {
                    if (viewmodel.ProductCondition.RelProducts == null) viewmodel.ProductCondition.RelProducts = new RelProductViewModel();
                    viewmodel.ProductCondition.RelProducts.IsIncludeRelation = true;
                    viewmodel.ProductCondition.RelProducts.IsExcludeRelation = false;
                }
                if (viewmodel.ProductCondition.ListRelVendorViewModel == null)
                {
                    viewmodel.ProductCondition.ListRelVendorViewModel = new List<RelVendorViewModel>();
                }
                if (entity.ProductCondition.ListRelVendor != null)
                {
                    foreach (var item in entity.ProductCondition.ListRelVendor)
                    {
                        viewmodel.ProductCondition.ListRelVendorViewModel.Add(new RelVendorViewModel() 
                        {
                            CouponsStatus=entity.Status,
                            VendorSysNo=item.VendorSysNo,
                            VendorName=item.VendorName,
                            IsChecked=false
                        });
                    }
                }
              
            }
            else
            {
                viewmodel.ProductCondition = new PSProductConditionViewModel();
                viewmodel.ProductCondition.RelBrands = new RelBrandViewModel();
                viewmodel.ProductCondition.RelBrands.IsIncludeRelation = true;
                viewmodel.ProductCondition.RelCategories = new RelCategory3ViewModel();
                viewmodel.ProductCondition.RelCategories.IsIncludeRelation = true;
                viewmodel.ProductCondition.RelProducts = new RelProductViewModel();
                viewmodel.ProductCondition.RelProducts.IsIncludeRelation = true;
            }
            #endregion

            #region 客户条件处理
            /*
             * 说明：由于原数据库设计变态，因此为了保证原来的兼容性,所以这里转换比较复杂
             * （1）如果限定类型是“无限定”，是插入了一条用户组的SysNo为-1记录到Coupon_SaleRules表：Type=R, CustomerRank=-1。
             * （2）如果限定类型是限定用户组，那么插入CustomerRank的SysNo记录到Coupon_SaleRules表：Type=R, CustomerRank=SysNo。
             * （3）如果限定类型是自选用户，那么插入Customer SysNo数据到Coupon_SaleRulesCustomer表。
             *      对于上述三种情况，转换到ViewModel中时，
             *      情况（1）则viewmodel.IsCustomerNoLimit = true，并且清空RelCustomerRanks.CustomerRankList
             *      但是在VtoE时，记得需要将viewmodel.IsCustomerNoLimit转换为CustomerRank=-1 的RelCustomerRanks.CustomerRankList一条记录
             * 
             * （4）如果再限定用户地区，那么插入地区的SysNo到Coupon_SaleRules表：Type=A, AreaSysNo=SysNo
             * */
            if (viewmodel.CustomerCondition == null)
            {
                viewmodel.CustomerCondition = new PSCustomerConditionViewModel();
                viewmodel.CustomerCondition.RelAreas = new RelAreaViewModel();
                viewmodel.CustomerCondition.RelCustomerRanks = new RelCustomerRankViewModel();
                viewmodel.CustomerCondition.RelCustomers = new RelCustomerViewModel();
                viewmodel.CustomerCondition.NeedEmailVerification = false;
                viewmodel.CustomerCondition.NeedMobileVerification = false;
                viewmodel.CustomerCondition.InvalidForAmbassador = false;
            }
            else
            {
                if(!viewmodel.CustomerCondition.NeedEmailVerification.HasValue) viewmodel.CustomerCondition.NeedEmailVerification = false;
                if(!viewmodel.CustomerCondition.NeedMobileVerification.HasValue) viewmodel.CustomerCondition.NeedMobileVerification = false;
                if(!viewmodel.CustomerCondition.InvalidForAmbassador.HasValue) viewmodel.CustomerCondition.InvalidForAmbassador = false;

                if (viewmodel.CustomerCondition.RelAreas != null && viewmodel.CustomerCondition.RelAreas.AreaList != null)
                {
                    foreach (SimpleObjectViewModel sim in viewmodel.CustomerCondition.RelAreas.AreaList)
                    {
                        sim.IsChecked = false;
                    }
                    if (viewmodel.CustomerCondition.RelAreas.AreaList.Count > 0)
                    {
                        viewmodel.IsAreaLimit = true;
                        viewmodel.IsAreaNoLimit = false;
                    }
                }
                else
                {
                    viewmodel.CustomerCondition.RelAreas = new RelAreaViewModel();
                }

                if (viewmodel.CustomerCondition.RelCustomerRanks != null && viewmodel.CustomerCondition.RelCustomerRanks.CustomerRankList != null)
                {
                    foreach (SimpleObjectViewModel sim in viewmodel.CustomerCondition.RelCustomerRanks.CustomerRankList)
                    {
                        sim.IsChecked = false;
                        CustomerRank cr = (CustomerRank)Enum.Parse(typeof(CustomerRank), sim.SysNo.Value.ToString(), true);
                        sim.Name = EnumConverter.GetDescription(cr);
                    }

                    if (viewmodel.CustomerCondition.RelCustomerRanks.CustomerRankList.Count > 0)
                    {
                        if (viewmodel.CustomerCondition.RelCustomerRanks.CustomerRankList.Count ==1 
                            && viewmodel.CustomerCondition.RelCustomerRanks.CustomerRankList[0].SysNo == -1)
                        {
                            viewmodel.IsCustomerRank = false;
                            viewmodel.IsCustomerID = false;
                            viewmodel.IsCustomerNoLimit = true;
                            viewmodel.CustomerCondition.RelCustomerRanks.CustomerRankList.Clear();
                        }
                        else
                        {
                            viewmodel.IsCustomerRank = true;
                            viewmodel.IsCustomerID = false;
                            viewmodel.IsCustomerNoLimit = false;
                        }
                    }
                }
                else
                {
                    viewmodel.CustomerCondition.RelCustomerRanks = new RelCustomerRankViewModel();
                }

                if (viewmodel.CustomerCondition.RelCustomers != null && viewmodel.CustomerCondition.RelCustomers.CustomerIDList != null)
                {
                    foreach (CustomerAndSendViewModel sim in viewmodel.CustomerCondition.RelCustomers.CustomerIDList)
                    {
                        sim.IsChecked = false;
                    }
                    if (viewmodel.CustomerCondition.RelCustomers.CustomerIDList.Count > 0)
                    {
                        viewmodel.IsCustomerRank = false;
                        viewmodel.IsCustomerID = true;
                        viewmodel.IsCustomerNoLimit = false;
                    }
                }
                else
                {
                    viewmodel.CustomerCondition.RelCustomers = new RelCustomerViewModel();
                }
            }

            #endregion

            if (viewmodel.OrderAmountDiscountRule == null)
            {
                viewmodel.OrderAmountDiscountRule = new PSOrderAmountDiscountRuleViewModel();
            }
            else
            {
                if (viewmodel.OrderAmountDiscountRule.OrderAmountDiscountRank != null 
                    && viewmodel.OrderAmountDiscountRule.OrderAmountDiscountRank.Count > 0)
                {
                    //foreach (OrderAmountDiscountRankViewModel rank in viewmodel.OrderAmountDiscountRule.OrderAmountDiscountRank)
                    //{
                    //    if (rank.DiscountType.Value == PSDiscountTypeForOrderAmount.OrderAmountPercentage)
                    //    {
                    //        rank.DiscountValue = rank.DiscountValue.Value * 100;
                    //    }
                    //}
                }
            }

            if (viewmodel.OrderCondition == null)
            {
                viewmodel.OrderCondition = new PSOrderConditionViewModel();
                
            }
            //SHIPTYPE ,PAYTYPE 转换
            if (viewmodel.OrderCondition != null 
                && viewmodel.OrderCondition.ShippingTypeSysNoList != null 
                && viewmodel.OrderCondition.ShippingTypeSysNoList.Count >0)
            {
                viewmodel.ShipTypeSysNo = entity.OrderCondition.ShippingTypeSysNoList[0];
            }
            
            if (viewmodel.OrderCondition != null 
                && viewmodel.OrderCondition.PayTypeSysNoList != null 
                && viewmodel.OrderCondition.PayTypeSysNoList.Count > 0)
            {
                viewmodel.PayTypeSysNo = entity.OrderCondition.PayTypeSysNoList[0];
            }

            if (viewmodel.BindCondition == null) viewmodel.BindCondition = CouponsBindConditionType.None;

            if (viewmodel.ValidPeriod == null) viewmodel.ValidPeriod = CouponsValidPeriodType.All;
            
            if (viewmodel.UsingFrequencyCondition == null) viewmodel.UsingFrequencyCondition = new PSActivityFrequencyConditionViewModel();

            if (!viewmodel.IsAutoBinding.HasValue) viewmodel.IsAutoBinding = false;
            if (!viewmodel.IsAutoUse.HasValue) viewmodel.IsAutoUse = false;
            if (!viewmodel.IsExistThrowInTypeCouponCode.HasValue) viewmodel.IsExistThrowInTypeCouponCode = false;
            if (!viewmodel.IsSendMail.HasValue) viewmodel.IsSendMail = false;

            if (viewmodel.BindRule == null)
            {
                viewmodel.BindRule = new CouponBindRuleViewModel();
            }

            if (viewmodel.BindRule.RelProducts == null)
            {
                viewmodel.BindRule.RelProducts = new RelProductViewModel();
            }
            //viewmodel.BindRule = new CouponBindRuleViewModel();
            //viewmodel.BindRule.RelProducts = new RelProductViewModel();
            //if (entity.BindCondition == CouponsBindConditionType.SO)
            //{
            //    if (entity.BindRule != null)
            //    {
            //        viewmodel.BindRule.AmountLimit = entity.BindRule.AmountLimit;
            //        if (entity.BindRule.RelProducts != null)
            //        {
            //            viewmodel.BindRule.RelProducts.IsIncludeRelation = entity.BindRule.RelProducts.IsIncludeRelation;
            //            viewmodel.BindRule.RelProducts.IsExcludeRelation = !entity.BindRule.RelProducts.IsIncludeRelation;
            //            if (entity.BindRule.RelProducts.ProductList != null && entity.BindRule.RelProducts.ProductList.Count > 0)
            //            {
            //                viewmodel.BindRule.RelProducts.ProductList = new List<RelProductAndQty>();
            //                foreach (var itemViewModel in entity.BindRule.RelProducts.ProductList)
            //                {
            //                    RelProductAndQtyViewModel item = new RelProductAndQtyViewModel();
            //                    item.ProductSysNo = itemViewModel.ProductSysNo;
            //                    item.MinQty = itemViewModel.MinQty;

            //                    item.ProductID = itemViewModel.ProductID;
            //                    item.ProductName = itemViewModel.ProductName;
            //                    item.AvailableQty = itemViewModel.AvailableQty;
            //                    item.ConsignQty = itemViewModel.ConsignQty;
            //                    item.VirtualQty = itemViewModel.VirtualQty;
            //                    item.UnitCost = itemViewModel.UnitCost;
            //                    item.CurrentPrice = itemViewModel.CurrentPrice;
            //                    item.GrossMarginRate = itemViewModel.GrossMarginRate;
            //                    item.Priority = itemViewModel.Priority;
            //                    item.ProductRangeType = itemViewModel.ProductRangeType;
            //                    viewmodel.BindRule.RelProducts.ProductList.Add(item);
            //                }
            //            }
            //        }
            //    }

            //}
            return viewmodel;
        }

        private CouponsInfo VtoE(CouponsInfoViewModel viewmodel)
        {
            if (viewmodel.PayTypeSysNo.HasValue)
            {
                viewmodel.OrderCondition.PayTypeSysNoList = new System.Collections.Generic.List<int>();
                viewmodel.OrderCondition.PayTypeSysNoList.Add(viewmodel.PayTypeSysNo.Value);
            }
            else
            {
                viewmodel.OrderCondition.PayTypeSysNoList = new System.Collections.Generic.List<int>();
            }

            if (viewmodel.ShipTypeSysNo.HasValue)
            {
                viewmodel.OrderCondition.ShippingTypeSysNoList = new System.Collections.Generic.List<int>();
                viewmodel.OrderCondition.ShippingTypeSysNoList.Add(viewmodel.ShipTypeSysNo.Value);
            }
            else
            {
                viewmodel.OrderCondition.ShippingTypeSysNoList = new System.Collections.Generic.List<int>();
            }

            if (viewmodel.OrderAmountDiscountRule.OrderAmountDiscountRank != null
                    && viewmodel.OrderAmountDiscountRule.OrderAmountDiscountRank.Count > 0)
            {
                foreach (OrderAmountDiscountRankViewModel rank in viewmodel.OrderAmountDiscountRule.OrderAmountDiscountRank)
                {
                    if (rank.DiscountType.Value == PSDiscountTypeForOrderAmount.OrderAmountPercentage)
                    {
                        rank.DiscountValue = rank.DiscountValue.Value ;
                    }
                }
            }


            CouponsInfo entity = viewmodel.ConvertVM<CouponsInfoViewModel, CouponsInfo>();

            entity.Title = new BizEntity.LanguageContent(viewmodel.Title);
            if (viewmodel.IsCustomerNoLimit.Value)
            {
                entity.CustomerCondition.RelCustomerRanks = new RelCustomerRank();
                entity.CustomerCondition.RelCustomerRanks.CustomerRankList = new List<BizEntity.Common.SimpleObject> ();
                entity.CustomerCondition.RelCustomerRanks.IsIncludeRelation = true;
                entity.CustomerCondition.RelCustomerRanks.CustomerRankList.Add(new SimpleObject { SysNo = -1 });
            }
            //商家限定条件
            if (viewmodel.ProductCondition != null)
            {
                if (viewmodel.ProductCondition.ListRelVendorViewModel != null)
                {
                    if (entity.ProductCondition == null)
                         entity.ProductCondition = new PSProductCondition();
                    if (entity.ProductCondition.ListRelVendor == null)
                        entity.ProductCondition.ListRelVendor = new List<RelVendor>();
                    
                    foreach (var item in viewmodel.ProductCondition.ListRelVendorViewModel)
                    {
                        entity.ProductCondition.ListRelVendor.Add(new RelVendor() { VendorName=item.VendorName,VendorSysNo=item.VendorSysNo});
                    }
                }
            }
            if (entity.BindRule == null)
            {
                entity.BindRule = new CouponBindRule();
            }
            if (entity.BindRule.RelProducts == null)
            {
                entity.BindRule.RelProducts = new RelProduct();
            }
            //entity.BindRule = new CouponBindRule();
            //entity.BindRule.RelProducts = new RelProduct();
            //if (viewmodel.BindCondition == CouponsBindConditionType.SO)
            //{
            //    if (viewmodel.BindRule != null)
            //    {
            //        entity.BindRule.AmountLimit = viewmodel.BindRule.AmountLimit;
            //        if (viewmodel.BindRule.RelProducts != null)
            //        {
            //            entity.BindRule.RelProducts.IsIncludeRelation = viewmodel.BindRule.RelProducts.IsIncludeRelation;
            //            if (viewmodel.BindRule.RelProducts.ProductList != null && viewmodel.BindRule.RelProducts.ProductList.Count > 0)
            //            {
            //                entity.BindRule.RelProducts.ProductList = new List<RelProductAndQty>();
            //                foreach (var itemViewModel in viewmodel.BindRule.RelProducts.ProductList)
            //                {
            //                    RelProductAndQty item=new RelProductAndQty();
            //                    item.ProductSysNo=itemViewModel.ProductSysNo;
            //                    item.MinQty = itemViewModel.MinQty;

            //                    item.ProductID = itemViewModel.ProductID;
            //                    item.ProductName = itemViewModel.ProductName;
            //                    item.AvailableQty = itemViewModel.AvailableQty;
            //                    item.ConsignQty = itemViewModel.ConsignQty;
            //                    item.VirtualQty = itemViewModel.VirtualQty;
            //                    item.UnitCost = itemViewModel.UnitCost;
            //                    item.CurrentPrice = itemViewModel.CurrentPrice;
            //                    item.GrossMarginRate = itemViewModel.GrossMarginRate;
            //                    item.Priority = itemViewModel.Priority;
            //                    item.PromotionSysNo = viewmodel.SysNo;
            //                    item.ProductRangeType = viewmodel.ProductRangeType;

            //                    entity.BindRule.RelProducts.ProductList.Add(item);
            //                }
            //            }
            //        }
            //    }

            //}

            return entity;
        }
        #endregion

         

        /****************************************************/
        public void Query(CouponsQueryFilterViewModel filterVM, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            string relativeUrl = "/MKTService/Coupons/Query";
            CouponsQueryFilter filter = filterVM.ConvertVM<CouponsQueryFilterViewModel, CouponsQueryFilter>();

            restClient.QueryDynamicData(relativeUrl, filter, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }
                callback(obj, args);
            });
        }

        //----------------批量处理---------------------------
        public void BatchProcessCoupons(List<int?> sysNoList, PSOperationType operation, EventHandler<RestClientEventArgs<BatchResultRsp>> callback)
        {
            string relativeUrl = "/MKTService/Coupons/{0}";
            switch (operation)
            {
                case PSOperationType.AuditApprove:
                    relativeUrl = string.Format(relativeUrl, "AuditApprove");
                    break;
                case  PSOperationType.AuditRefuse:
                    relativeUrl = string.Format(relativeUrl, "AuditRefuse");
                    break;
                case  PSOperationType.CancelAudit:
                    relativeUrl = string.Format(relativeUrl, "CancelAudit");
                    break;
                case PSOperationType.Stop:
                    relativeUrl = string.Format(relativeUrl, "ManualStop");
                    break;
                case PSOperationType.SubmitAudit:
                    relativeUrl = string.Format(relativeUrl, "SubmitAudit");
                    break;
                case PSOperationType.Void:
                    relativeUrl = string.Format(relativeUrl, "Void");
                    break;
            }
            restClient.Query<BatchResultRsp>(relativeUrl, sysNoList, (obj, args) =>
                {
                    if (args.FaultsHandle())
                    {
                        return;
                    }
                    callback(obj, args);
                });
        }

        public void ExportExcelForCouponsCodeList(CouponCodeQueryFilterViewModel queryFilter, ColumnSet[] columns)
        {
            string relativeUrl = "/MKTService/Coupons/CouponCode/Query";
            CouponCodeQueryFilter filter = queryFilter.ConvertVM<CouponCodeQueryFilterViewModel, CouponCodeQueryFilter>();
            restClient.ExportFile(relativeUrl, filter, columns);
        }
    }


}
