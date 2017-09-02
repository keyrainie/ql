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
using ECCentral.Service.Utility;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using Newegg.Oversea.Silverlight.Controls;
using ECCentral.Portal.UI.MKT.Models;
using ECCentral.QueryFilter.MKT.Promotion;
using ECCentral.BizEntity.MKT;
using System.Collections.Generic;
using ECCentral.Service.MKT.Restful.ResponseMsg;
using System.Collections.ObjectModel;

namespace ECCentral.Portal.UI.MKT.Facades.Promotion
{
    public class SaleGiftFacade
    {
        private readonly RestClient restClient;

        /// <summary>
        /// 服务基地址
        /// </summary>
        protected string ServiceBaseUrl
        {
            get
            {
                return CPApplication.Current.CurrentPage.Context.Window.Configuration.GetConfigValue("MKT", "ServiceBaseUrl");
            }
        }

        public SaleGiftFacade(IPage page)
        {
            restClient = new RestClient(ServiceBaseUrl, page);
        }

        /// <summary>
        /// 查询
        /// </summary>
        /// <param name="filterVM"></param>
        /// <param name="callback"></param>
        public void Query(SaleGiftQueryFilterViewModel filterVM, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            string relativeUrl = "/MKTService/SaleGift/Query";
            SaleGiftQueryFilter filter = filterVM.ConvertVM<SaleGiftQueryFilterViewModel, SaleGiftQueryFilter>();
            
            restClient.QueryDynamicData(relativeUrl, filter, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }
                callback(obj, args);
            });
        }


        /// <summary>
        /// 查询Log
        /// </summary>
        /// <param name="filterVM"></param>
        /// <param name="callback"></param>
        public void QueryLog(SaleGiftLogQueryFilterViewModel filterVM, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            string relativeUrl = "/MKTService/SaleGift/QuerySaleGiftLog";
            SaleGiftLogQueryFilter filter = filterVM.ConvertVM<SaleGiftLogQueryFilterViewModel, SaleGiftLogQueryFilter>();

            restClient.QueryDynamicData(relativeUrl, filter, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }
                callback(obj, args);
            });
        }

        public void Load(int? sysNo, SaleGiftInfoViewModel viewModel, EventHandler<RestClientEventArgs<SaleGiftInfoViewModel>> callback)
        {
            if (!sysNo.HasValue)
            {
                callback(new Object(), new RestClientEventArgs<SaleGiftInfoViewModel>(viewModel, restClient.Page));
                return;
            }
            string relativeUrl = string.Format("/MKTService/SaleGift/{0}", sysNo.Value);
            restClient.Query<SaleGiftInfo>(relativeUrl, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }

                SaleGiftInfo entity = args.Result;
                viewModel = EtoV(entity);

                callback(obj, new RestClientEventArgs<SaleGiftInfoViewModel>(viewModel, restClient.Page));
            });

        }

        public void CopyCreateNew(int? oldSysNo, EventHandler<RestClientEventArgs<int?>> callback)
        {
            if (oldSysNo.HasValue)
            {
                string relativeUrl = "/MKTService/SaleGift/CopyCreateNew";
                restClient.Create<int?>(relativeUrl, oldSysNo, (obj, args) =>
                    {
                        if (args.FaultsHandle())
                        {
                            return;
                        }
                        callback(obj, args);
                    });
            }

        }


        public void SaveMaster(SaleGiftInfoViewModel viewModel, EventHandler<RestClientEventArgs<SaleGiftInfoViewModel>> callback)
        {
            if (!viewModel.SysNo.HasValue)
            {
                string relativeUrl = "/MKTService/SaleGift/CreateMaster";
                viewModel.CompanyCode = CPApplication.Current.CompanyCode;
                restClient.Create<int?>(relativeUrl, VtoE(viewModel), (obj, args) =>
                {
                    if (args.FaultsHandle())
                    {
                        return;
                    }
                    viewModel.SysNo = args.Result;
                    viewModel.Status = SaleGiftStatus.Init;
                    viewModel.IsGlobalProduct = false;
                    callback(obj, new RestClientEventArgs<SaleGiftInfoViewModel>(viewModel, restClient.Page));
                });
            }
            else
            {
                string relativeUrl = "/MKTService/SaleGift/UpdateMaster";

                restClient.Update<CouponsInfo>(relativeUrl, VtoE(viewModel), (obj, args) =>
                {
                    if (args.FaultsHandle())
                    {
                        return;
                    }
                    
                    callback(obj, new RestClientEventArgs<SaleGiftInfoViewModel>(viewModel, restClient.Page));
                });
            }
        }

        public void SetSaleGiftSaleRules(SaleGiftInfoViewModel viewModel, EventHandler<RestClientEventArgs<object>> callback)
        {
            string relativeUrl = "/MKTService/SaleGift/SetSaleRules";
            restClient.Update(relativeUrl, VtoE(viewModel), (obj, args) =>
                {
                    if (args.FaultsHandle())
                    {
                        return;
                    }
                    callback(obj, args);
                });
        }

        public void SetSaleGiftGiftItemRules(SaleGiftInfoViewModel viewModel, EventHandler<RestClientEventArgs<object>> callback)
        {
            string relativeUrl = "/MKTService/SaleGift/SetGiftItemRules";
            restClient.Update(relativeUrl, VtoE(viewModel), (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }
                callback(obj, args);
            });
        }
        /// <summary>
        /// 有高级权限时
        /// step1 在服务器端去检查主商品是否有货
        /// step2 主商品有货时在检查赠品和主商品在同一个仓库是否都有货
        /// step3 前两个条件任何一个为否 将抛出异常 提示是否继续添加（高级权限可继续，一般权限则不能进行保存）
        /// </summary>
        /// <param name="viewModel"></param>
        /// <param name="callback"></param>
        public void CheckGiftStockResult(SaleGiftInfoViewModel viewModel, EventHandler<RestClientEventArgs<string>> callback)
        {
            string relativeUrl = "/MKTService/SaleGift/CheckGiftStockResult";
            restClient.Query(relativeUrl, VtoE(viewModel), callback);
        }

        //----------------批量处理---------------------------
        public void BatchProcessSaleGift(List<int?> sysNoList, PSOperationType operation, EventHandler<RestClientEventArgs<BatchResultRsp>> callback)
        {
            string relativeUrl = "/MKTService/SaleGift/{0}";
            switch (operation)
            {
                case PSOperationType.AuditApprove:
                    relativeUrl = string.Format(relativeUrl, "AuditApprove");
                    break;
                case PSOperationType.AuditRefuse:
                    relativeUrl = string.Format(relativeUrl, "AuditRefuse");
                    break;
                case PSOperationType.CancelAudit:
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

        /// <summary>
        /// 批量创建赠品。
        /// </summary>
        /// <param name="viewModel"></param>
        /// <param name="callback"></param>
        public void BatchCreateSaleGift(SaleGiftBatchInfoVM viewModel, EventHandler<RestClientEventArgs<object>> callback)
        {
            SaleGiftBatchInfo info = viewModel.ToEntity();
            info.InUser = CPApplication.Current.LoginUser.LoginName;
            info.CompanyCode = CPApplication.Current.CompanyCode;

            string relativeUrl = "/MKTService/SaleGift/BatchCreateSaleGift";
            restClient.Create<object>(relativeUrl, info, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }
                callback(obj, args);
            });
        }

        /// <summary>
        ///  获取赠品的所有商家
        /// </summary>
        /// <returns></returns>
        public void GetGiftVendorList(EventHandler<RestClientEventArgs<List<RelVendor>>> callback) 
        {
            string relativeUrl = "/MKTService/SaleGift/GetGiftVendorList";
            restClient.Query<List<RelVendor>>(relativeUrl, null, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }
                callback(obj, args);
            });
        }

        #region ViewModel 和Entity之间的双向Transform
        private SaleGiftInfo VtoE(SaleGiftInfoViewModel viewModel)
        {
            viewModel.ProductCondition = new ObservableCollection<SaleGift_RuleSettingViewModel>();
            if (viewModel.ProductOnlyList != null)
            {
                viewModel.ProductOnlyList.ForEach(p =>
                {
                    viewModel.ProductCondition.Add(p);
                });                
            }
            if (viewModel.BrandC3ScopeList != null)
            {
                viewModel.BrandC3ScopeList.ForEach(p =>
                {
                    viewModel.ProductCondition.Add(p);
                });                          
            }
            if (viewModel.ProductScopeList != null)
            {
                viewModel.ProductScopeList.ForEach(p =>
                {
                    viewModel.ProductCondition.Add(p);
                });                       
            }
            if (viewModel.GiftItemList != null)
            {
                if (viewModel.GiftComboType == SaleGiftGiftItemType.GiftPool)
                {
                    foreach (SaleGift_GiftItemViewModel giftitem in viewModel.GiftItemList)
                    {
                        giftitem.Count = null;
                    }
                }
            }

            SaleGiftInfo entity = viewModel.ConvertVM<SaleGiftInfoViewModel, SaleGiftInfo>();
            entity.Title = new BizEntity.LanguageContent(viewModel.Title);
            entity.Description = new BizEntity.LanguageContent(viewModel.Description);
            
            return entity;
        }

        private SaleGiftInfoViewModel EtoV(SaleGiftInfo entity)
        {
            SaleGiftInfoViewModel viewmodel = entity.Convert<SaleGiftInfo, SaleGiftInfoViewModel>((en, vm) =>
            {
                vm.Title = entity.Title.Content;
                vm.Description = entity.Description.Content;                
            });

            if (entity.WebChannel != null)
            {
                viewmodel.WebChannel = new Basic.Components.Models.WebChannelVM() { ChannelID = entity.WebChannel.ChannelID, ChannelName = entity.WebChannel.ChannelName };
            }
            else
            {
                viewmodel.WebChannel = viewmodel.WebChennelList[0];
            }
            //目前只有买满即送才有复杂的主商品范围规则
            viewmodel.BrandC3ScopeList = new ObservableCollection<SaleGift_RuleSettingViewModel>();
            viewmodel.ProductScopeList = new ObservableCollection<SaleGift_RuleSettingViewModel>();
            viewmodel.ProductOnlyList = new ObservableCollection<SaleGift_RuleSettingViewModel>();

            //modified by poseidon.y.tong at 2013-08-22 10:36:48
            //新增赠品类型:首次下单、满额加购都有复杂的商品范围规则
            if (viewmodel.Type.Value == SaleGiftType.Full)
            //||  viewmodel.Type.Value == SaleGiftType.FirstOrder  || viewmodel.Type.Value == SaleGiftType.Additional)
            {
                foreach(SaleGift_RuleSettingViewModel rule in viewmodel.ProductCondition)
                {
                    if (rule.Type.Value != SaleGiftSaleRuleType.Item)
                    {
                        viewmodel.BrandC3ScopeList.Add(rule);
                    }
                    else
                    {
                        viewmodel.ProductScopeList.Add(rule);
                    }
                }
            }
            else
            {
                viewmodel.ProductOnlyList = viewmodel.ProductCondition;
            }

            if (viewmodel.GiftItemList == null)
            {
                viewmodel.GiftItemList = new List<SaleGift_GiftItemViewModel>();
            }
            else
            {
                if (viewmodel.GiftComboType == SaleGiftGiftItemType.GiftPool)
                {
                    foreach (SaleGift_GiftItemViewModel giftitem in viewmodel.GiftItemList)
                    {
                        giftitem.Count = "0";
                    }
                }
            }

            return viewmodel;
        }

        #endregion
    }
}
