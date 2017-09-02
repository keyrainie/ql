using System;
using System.Windows;
using ECCentral.Portal.Basic;
using ECCentral.Portal.UI.IM.Facades;
using ECCentral.Portal.UI.IM.Models;
using ECCentral.Portal.UI.IM.UserControls;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using Newegg.Oversea.Silverlight.Controls;
using Newegg.Oversea.Silverlight.Controls.Data;
using System.Windows.Controls;
using System.Collections.Generic;
using System.Linq;
using Newegg.Oversea.Silverlight.Controls.Components;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using ECCentral.BizEntity.IM;
using ECCentral.Portal.Basic.Components.UserControls.ProductPicker;
using ECCentral.BizEntity.Common;

namespace ECCentral.Portal.UI.IM.Views
{
    [View(IsSingleton = true, SingletonType = SingletonTypes.Url)]
    public partial class ProductChannelMemberManager : PageBase
    {
        #region Const
        ProductChannelMemberQueryVM model;
        public const string IM_ProductChannelMemberLogFormat = "/ECCentral.Portal.UI.IM/ProductChannelMemberLog";
        List<dynamic> selectSource;
        #endregion

        #region Method

        public ProductChannelMemberManager()
        {
            InitializeComponent();
        }
        public override void OnPageLoad(object sender, EventArgs e)
        {
            base.OnPageLoad(sender, e);
            model = new ProductChannelMemberQueryVM();
            ProductChannelMemberFacade facade = new ProductChannelMemberFacade();
            facade.GetProductChannelMemberInfoList((obj, arg) =>
            {
                if (arg.FaultsHandle()) return;

                model.ChannelList = arg.Result;
                model.ChannelList.Insert(0, new ProductChannelMemberInfo() { SysNo = 0, ChannelName = "所有" });
                this.DataContext = model;
            });

        }

        #region 查询绑定
        private void Button_Search_Click(object sender, RoutedEventArgs e)
        {
            this.dgProductChannelQueryResult.Bind();
        }
        private void dgProductChannelQueryResult_LoadingDataSource(object sender, LoadingDataEventArgs e)
        {
            ProductChannelMemberFacade facade = new ProductChannelMemberFacade(this);
            model = (ProductChannelMemberQueryVM)this.DataContext;
            facade.GetProductChannelMemberPriceInfo(model, e.PageSize, e.PageIndex, e.SortField, (obj, args) =>
            {
                var list = new List<dynamic>();
                foreach (var row in args.Result.Rows)
                {
                    list.Add(row);
                }
                //绑定控件上列表
                this.dgProductChannelQueryResult.ItemsSource = list;
                //绑定总共有多少行数
                this.dgProductChannelQueryResult.TotalCount = args.Result.TotalCount;
            });
            cbDemo.IsChecked = false;
        }
        private void ckbSelectAllRow_Click(object sender, RoutedEventArgs e)
        {
            var ckb = sender as CheckBox;
            if (ckb == null) return;
            var viewList = dgProductChannelQueryResult.ItemsSource as dynamic;
            foreach (var view in viewList)
            {
                view.IsChecked = ckb.IsChecked != null && ckb.IsChecked.Value;
            }
        }
        #endregion

        #region 页面内按钮处理事件
        #region 界面事件

        private void hyperlinkEdit_Click(object sender, RoutedEventArgs e)
        {
            dynamic item = this.dgProductChannelQueryResult.SelectedItem as dynamic;
            if (item == null) return;
            var sysNo = Convert.ToInt32(item.SysNo);
            ProductChannelProductEditDetail detail = new ProductChannelProductEditDetail();
            detail.SysNo = sysNo;
            detail.Dialog = Window.ShowDialog(ECCentral.Portal.UI.IM.Resources.ResProductChannelManagement.Dialog_Edit, detail, (s, args) =>
            {
                if (args.DialogResult == Newegg.Oversea.Silverlight.Controls.Components.DialogResultType.OK)
                {
                    dgProductChannelQueryResult.Bind();
                }
            }, new Size(750, 250));

        }

        private void hyperlinkProductID_Click(object sender, RoutedEventArgs e)
        {
            dynamic item = this.dgProductChannelQueryResult.SelectedItem as dynamic;
            if (item != null)
            {
                this.Window.Navigate(string.Format(ConstValue.IM_ProductMaintainUrlFormat, item.ProductSysno), null, true);
            }
        }

        #endregion
        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            model = (ProductChannelMemberQueryVM)this.DataContext;

            if (this.model.ChannelSysNo > 0)
            {

                var ucPicker = new UCProductSearch();
                ucPicker.SelectionMode = SelectionMode.Multiple;
                ucPicker.DialogHandler = CPApplication.Current.CurrentPage.Context.Window.ShowDialog("选择商品", ucPicker, (obj, args) =>
                {
                    if (args.DialogResult == DialogResultType.OK)
                    {
                        if (args.Data == null) return;

                        var facade = new ProductChannelMemberFacade(this);
                        List<ProductChannelMemberPriceInfo> productList = new List<ProductChannelMemberPriceInfo>();
                        var selectedProductList = args.Data as List<Basic.Components.UserControls.ProductPicker.ProductVM>;

                        #region 赋值
                        selectedProductList.ForEach(p => productList.Add(new ProductChannelMemberPriceInfo
                            {
                                ProductSysNo = p.SysNo == null ? 0 : int.Parse(p.SysNo.ToString()),
                                ChannelSysNO = model.ChannelSysNo,
                                MemberPrice = null,
                                MemberPricePercent = 1,
                                InDate = DateTime.Now,
                                InUser = CPApplication.Current.LoginUser.LoginName,
                                EditUser = null,
                                CompanyCode = CPApplication.Current.CompanyCode,
                                StoreCompanyCode = CPApplication.Current.CompanyCode,
                                LanguageCode = CPApplication.Current.LanguageCode
                            }));
                        #endregion

                        facade.InsertProductChannelMemberPrices(productList, (o, a) =>
                            {
                                if (a.FaultsHandle())
                                {
                                    dgProductChannelQueryResult.Bind();
                                    return;
                                }
                            });
                    }
                });
            }
            else
            {
                Window.Alert("请先选择渠道！", MessageType.Information);
                return;
            }
        }
        private void btnDel_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidateItemsSource()) return;
            Window.Confirm("确认删除?", (obj, arg) => {

                if (arg.DialogResult == DialogResultType.OK)
                {
                    var facade = new ProductChannelMemberFacade(this);
                    List<ProductChannelMemberPriceInfo> _ProductChannelMemberPriceInfos =
                       (from c in selectSource
                        select new ProductChannelMemberPriceInfo
                        {
                            SysNo = c.SysNo,
                            ProductSysNo = c.ProductSysNo,
                            ChannelName = c.ChannelName,
                            ProductName = c.ProductName,
                            CurrentPrice = Convert.ToDecimal(c.CurrentPrice),
                            MemberPrice = c.MemberPrice == null ? 0 : c.MemberPrice,
                            MemberPricePercent = c.MemberPricePercent == null ? 0 : c.MemberPricePercent,
                            EditDate = DateTime.Now,
                            EditUser = CPApplication.Current.LoginUser.LoginName,
                            CompanyCode = CPApplication.Current.CompanyCode,
                            StoreCompanyCode = CPApplication.Current.CompanyCode,
                            LanguageCode = CPApplication.Current.LanguageCode
                        }).ToList();

                    facade.DeleteProductChannelMemberPrices(_ProductChannelMemberPriceInfos, (o, a) =>
                    {
                        if (a.FaultsHandle())
                        {
                            dgProductChannelQueryResult.Bind();
                            return;
                        }
                    });
                }

            });
           
        }
        private void btnBatchUpdate_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidateItemsSource()) return;
            ProductChannelProductBatchEditDetail detail = new ProductChannelProductBatchEditDetail();
            List<ProductChannelMemberPriceInfo> _ProductChannelMemberPriceInfos =
                (from c in selectSource
                 select new ProductChannelMemberPriceInfo
                 {
                     SysNo = c.SysNo,
                     ProductSysNo = c.ProductSysNo,
                     ChannelName = c.ChannelName,
                     ProductName = c.ProductName,
                     CurrentPrice = decimal.Parse(c.CurrentPrice),
                     EditDate = DateTime.Now,
                     EditUser = CPApplication.Current.LoginUser.LoginName,
                     CompanyCode = CPApplication.Current.CompanyCode,
                     StoreCompanyCode = CPApplication.Current.CompanyCode,
                     LanguageCode = CPApplication.Current.LanguageCode
                 }).ToList();

            detail.ProductChannelMemberPriceInfos = _ProductChannelMemberPriceInfos;
            detail.Dialog = Window.ShowDialog(
                ECCentral.Portal.UI.IM.Resources.ResProductChannelManagement.Dialog_Edit, detail, (s, args) =>
            {
                if (args.DialogResult == Newegg.Oversea.Silverlight.Controls.Components.DialogResultType.OK)
                {
                    dgProductChannelQueryResult.Bind();
                }
            }, new Size(750, 200));


        }
        private void btnLogQuery_Click(object sender, RoutedEventArgs e)
        {
            Window.Navigate(IM_ProductChannelMemberLogFormat, null, true);
        }
        #endregion

        private Boolean ValidateItemsSource()
        {
            if (dgProductChannelQueryResult.ItemsSource == null) return false;
            selectSource = dgProductChannelQueryResult.ItemsSource as List<dynamic>;
            selectSource = selectSource.Where(p => p.IsChecked).ToList();
            if (selectSource == null || selectSource.Count == 0)
            {
                Window.Alert("请选择一条记录！", MessageType.Error);
                return false;
            }
            return true;
        }
        #endregion

    }
}
