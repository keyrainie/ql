using System;
using System.Threading;
using System.Windows;
using ECCentral.BizEntity.Common;
using ECCentral.BizEntity.MKT;
using ECCentral.Portal.Basic;
using ECCentral.Portal.UI.IM.Facades;
using ECCentral.Portal.UI.IM.Models;
using ECCentral.Portal.UI.MKT.UserControls;
using ECCentral.QueryFilter.Common;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using Newegg.Oversea.Silverlight.Controls;
using Newegg.Oversea.Silverlight.Controls.Data;
using System.Collections.Generic;
using System.Windows.Controls;
using Newegg.Oversea.Silverlight.Utilities.Validation;
using Newegg.Oversea.Silverlight.Controls.Components;

namespace ECCentral.Portal.UI.MKT.Views
{
    [View(IsSingleton = true, SingletonType = SingletonTypes.Url)]
    public partial class RepeatPromotionQuery : PageBase
    {
        #region 属性
        RepeatPromotionQueryVM _model;
        private RepeatPromotionQueryFacade _facade;
        #endregion

        #region 初始化加载

        public RepeatPromotionQuery()
        {
            InitializeComponent();
            Loaded += RepeatPromotionQuery_Loaded;
        }

        void RepeatPromotionQuery_Loaded(object sender, RoutedEventArgs e)
        {
            _model = new RepeatPromotionQueryVM();
            DataContext = _model;
            _facade = new RepeatPromotionQueryFacade(this);
            Loaded -= RepeatPromotionQuery_Loaded;
        }

        #endregion

        #region 查询绑定

        /// <summary>
        /// 查询
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidationManager.Validate(this))
            {
                return;
            }
            dgSaleRuleQueryResult.Bind();
            dgGiftQueryResult.Bind();
            dgCouponQueryResult.Bind();
            dgSaleCountDownQueryResult.Bind();
            dgSaleCountDownPlanQueryResult.Bind();
            dgProductGroupBuyingQueryResult.Bind();
        }

       
        #endregion

        #region 页面内按钮处理事件

        #region 界面事件

        /// <summary>
        /// 销售规则数据绑定
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgSaleRuleQueryResult_LoadingDataSource(object sender, LoadingDataEventArgs e)
        {
            _model.PageInfo = new PagingInfo
            {
                PageSize = e.PageSize,
                PageIndex = e.PageIndex,
                SortBy = e.SortField
            };
            ThreadPool.QueueUserWorkItem(BindSaleRuleQueryResult, _model);

        }

        private void BindSaleRuleQueryResult(object model)
        {
            if (model is RepeatPromotionQueryVM)
            {
                var condtion = (RepeatPromotionQueryVM) model;
                Dispatcher.BeginInvoke(() => _facade.GetSaleRules(condtion, (obj, args) =>
                        {
                            if (args.FaultsHandle())
                                return;
                            dgSaleRuleQueryResult.
                                ItemsSource =
                                args.Result.Rows;
                            dgSaleRuleQueryResult.
                                TotalCount =
                                args.Result.TotalCount;
                            _model.SaleRuleCount =  "(" + args.Result.TotalCount + ")";
                        }));
            }

        }

        /// <summary>
        /// 赠品数据绑定
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgGiftQueryResult_LoadingDataSource(object sender, LoadingDataEventArgs e)
        {
            _model.PageInfo = new PagingInfo
            {
                PageSize = e.PageSize,
                PageIndex = e.PageIndex,
                SortBy = e.SortField
            };
            ThreadPool.QueueUserWorkItem(BindGiftQueryResult, _model);
        }

        private void BindGiftQueryResult(object model)
        {
            if (model is RepeatPromotionQueryVM)
            {
                var condtion = (RepeatPromotionQueryVM)model;
                Dispatcher.BeginInvoke(() => _facade.GetGifts(condtion, (obj, args) =>
                {
                    if (args.FaultsHandle())
                        return;
                    dgGiftQueryResult.
                        ItemsSource =
                        args.Result.Rows;
                    dgGiftQueryResult.
                        TotalCount =
                        args.Result.TotalCount;
                    _model.GiftCount = "(" + args.Result.TotalCount + ")";
                }));
            }

        }

        /// <summary>
        /// 优惠券数据绑定
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgCouponQueryResult_LoadingDataSource(object sender, LoadingDataEventArgs e)
        {
            _model.PageInfo = new PagingInfo
            {
                PageSize = e.PageSize,
                PageIndex = e.PageIndex,
                SortBy = e.SortField
            };
            ThreadPool.QueueUserWorkItem(BindCouponQueryResult, _model);
        }

        private void BindCouponQueryResult(object model)
        {
            if (model is RepeatPromotionQueryVM)
            {
                var condtion = (RepeatPromotionQueryVM)model;
                Dispatcher.BeginInvoke(() => _facade.GetCoupons(condtion, (obj, args) =>
                {
                    if (args.FaultsHandle())
                        return;
                    dgCouponQueryResult.
                        ItemsSource =
                        args.Result.Rows;
                    dgCouponQueryResult.
                        TotalCount =
                        args.Result.TotalCount;
                    _model.CouponCount =  "(" + args.Result.TotalCount + ")";
                }));
            }

        }

        /// <summary>
        /// 限时抢购数据绑定
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgSaleCountDownQueryResult_LoadingDataSource(object sender, LoadingDataEventArgs e)
        {
            _model.PageInfo = new PagingInfo
            {
                PageSize = e.PageSize,
                PageIndex = e.PageIndex,
                SortBy = e.SortField
            };
            ThreadPool.QueueUserWorkItem(BindSaleCountDownQueryResult, _model);
        }

        private void BindSaleCountDownQueryResult(object model)
        {
            if (model is RepeatPromotionQueryVM)
            {
                var condtion = (RepeatPromotionQueryVM)model;
                Dispatcher.BeginInvoke(() => _facade.GetSaleCountDowns(condtion, (obj, args) =>
                {
                    if (args.FaultsHandle())
                        return;
                    dgSaleCountDownQueryResult.
                        ItemsSource =
                        args.Result.Rows;
                    dgSaleCountDownQueryResult.
                        TotalCount =
                        args.Result.TotalCount;
                    _model.SaleCountDownCount = "(" + args.Result.TotalCount + ")";
                }));
            }

        }

        /// <summary>
        /// 销售规则数据绑定
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgSaleCountDownPlanQueryResult_LoadingDataSource(object sender, LoadingDataEventArgs e)
        {
            _model.PageInfo = new PagingInfo
            {
                PageSize = e.PageSize,
                PageIndex = e.PageIndex,
                SortBy = e.SortField
            };
            ThreadPool.QueueUserWorkItem(BindSaleCountDownPlanQueryResult, _model);
        }

        private void BindSaleCountDownPlanQueryResult(object model)
        {
            if (model is RepeatPromotionQueryVM)
            {
                var condtion = (RepeatPromotionQueryVM)model;
                Dispatcher.BeginInvoke(() => _facade.GeSaleCountDownPlan(condtion, (obj, args) =>
                {
                    if (args.FaultsHandle())
                        return;
                    dgSaleCountDownPlanQueryResult.
                        ItemsSource =
                        args.Result.Rows;
                    dgSaleCountDownPlanQueryResult.
                        TotalCount =
                        args.Result.TotalCount;
                    _model.SaleCountDownPlanCount = "(" + args.Result.TotalCount + ")";
                }));
            }

        }

        /// <summary>
        /// 团购数据绑定
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgProductGroupBuyingQueryResult_LoadingDataSource(object sender, LoadingDataEventArgs e)
        {
            _model.PageInfo = new PagingInfo
            {
                PageSize = e.PageSize,
                PageIndex = e.PageIndex,
                SortBy = e.SortField
            };
            ThreadPool.QueueUserWorkItem(BindProductGroupBuyingQueryResult, _model);
        }

        private void BindProductGroupBuyingQueryResult(object model)
        {
            if (model is RepeatPromotionQueryVM)
            {
                var condtion = (RepeatPromotionQueryVM)model;
                Dispatcher.BeginInvoke(() => _facade.GetProductGroupBuying(condtion, (obj, args) =>
                {
                    if (args.FaultsHandle())
                        return;
                    dgProductGroupBuyingQueryResult.
                        ItemsSource =
                        args.Result.Rows;
                    dgProductGroupBuyingQueryResult.
                        TotalCount =
                        args.Result.TotalCount;
                    _model.ProductGroupBuyingCount =  "(" + args.Result.TotalCount + ")";
                }));
            }

        }

        private void hyperlinkSaleRuleID_Click(object sender, RoutedEventArgs e)
        {
            dynamic d = this.dgSaleRuleQueryResult.SelectedItem as dynamic;
            if(d!=null&&d.SysNo>0)
            {
                this.Window.Navigate(string.Format(ConstValue.MKT_ComboSaleMaintain, d.SysNo), null, true);
            }
          
        }

        private void hyperlinkGiftID_Click(object sender, RoutedEventArgs e)
        {
            dynamic d = this.dgGiftQueryResult.SelectedItem as dynamic;
            if (d != null && d.SysNo > 0)
            {
                this.Window.Navigate(string.Format(ConstValue.MKT_SaleGiftMaintainUrlFormatEdit, d.SysNo), null, true);
            }
        }

        private void hyperlinkCouponID_Click(object sender, RoutedEventArgs e)
        {
            dynamic d = this.dgCouponQueryResult.SelectedItem as dynamic;
            if (d != null && d.SysNo > 0)
            {
                this.Window.Navigate(string.Format(ConstValue.MKT_CouponMaintainUrlFormatEdit, d.SysNo), null, true);
            }
            
        }

        private void hyperlinkSaleCountDownID_Click(object sender, RoutedEventArgs e)
        {
            dynamic d = this.dgSaleCountDownQueryResult.SelectedItem as dynamic;
            if (d != null && d.SysNo > 0)
            {
                this.Window.Navigate(string.Format(ConstValue.MKT_CountdownMaintainUrlFormat, d.SysNo), null, true);
            }
        }

        private void hyperlinkSaleCountDownPlanID_Click(object sender, RoutedEventArgs e)
        {
            dynamic d = this.dgSaleCountDownPlanQueryResult.SelectedItem as dynamic;
            if (d != null && d.SysNo > 0)
            {
                this.Window.Navigate(string.Format(ConstValue.MKT_CountdownMaintainUrlFormat, d.SysNo), null, true);
            }
        }

        private void hyperlinkProductGroupBuyingID_Click(object sender, RoutedEventArgs e)
        {
            dynamic d = this.dgProductGroupBuyingQueryResult.SelectedItem as dynamic;
            if (d != null && d.SysNo > 0)
            {
                this.Window.Navigate(string.Format(ConstValue.MKT_GroupBuyingMaintainUrlFormatEdit, d.SysNo), null, true);
            }
        }

        #endregion

        #endregion

        #region 跳转

        #endregion



    }
}
