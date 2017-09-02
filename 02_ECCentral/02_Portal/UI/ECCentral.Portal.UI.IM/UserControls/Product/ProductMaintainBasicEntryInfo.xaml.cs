using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using ECCentral.Portal.UI.IM.Facades.Product;
using ECCentral.Portal.UI.IM.Models.Product;
using ECCentral.Portal.Basic.Interface;
using Newegg.Oversea.Silverlight.Controls;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using Newegg.Oversea.Silverlight.Controls.Components;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.BizEntity.IM.Product;
using System.Text.RegularExpressions;
using Newegg.Oversea.Silverlight.Utilities.Validation;
using ECCentral.BizEntity.Common;
using ECCentral.BizEntity.IM;
using ECCentral.Portal.UI.IM.UserControls.Product;

namespace ECCentral.Portal.UI.IM.UserControls
{
    public partial class ProductMaintainBasicEntryInfo : UserControl, ISave
    {
        public IWindow MyWindow { get; set; }
        public IDialog Dialog { get; set; }
        public ProductEntryFacade facades;
        public ProductEntryInfoVM vm;
        public IPage CurrentPage
        {
            get { return CPApplication.Current.CurrentPage; }
        }
        public ProductMaintainBasicEntryInfo()
        {
            //this.Loaded += new RoutedEventHandler(ProductMaintainBasicEntryInfo_Loaded);
            InitializeComponent();
        }

        //void ProductMaintainBasicEntryInfo_Loaded(object sender, RoutedEventArgs e)
        //{
        //    vm = new ProductEntryInfoVM();
        //    facades = new ProductEntryFacade(CPApplication.Current.CurrentPage);
        //    string tempSysNo = string.Empty;

        //    if (CurrentPage.Context.Request.QueryString != null)
        //    {
        //        tempSysNo = CurrentPage.Context.Request.QueryString["ProductSysNo"];
        //    }
        //    if (!string.IsNullOrEmpty(CurrentPage.Context.Request.Param))
        //    {
        //        tempSysNo = CurrentPage.Context.Request.Param;
        //    }
        //    int productSysNo;
        //    if (Int32.TryParse(tempSysNo, out productSysNo))
        //    {
        //        facades.LoadProductEntryInfo(productSysNo, (obj, args) =>
        //        {
        //            if (args.Result == null)
        //            {
        //                CPApplication.Current.CurrentPage.Context.Window.Alert("商品信息错误!");
        //                return;

        //            }

        //            vm = EntityConverter<ProductEntryInfo, ProductEntryInfoVM>.Convert(
        //                                           args.Result,
        //                                           (s, t) =>
        //                                           { });
        //            EntryBizcmb.SelectedIndex = 0;
        //            StoreTypecmb.SelectedIndex = 0;
                    
        //            ////待审核 显示审核按钮
        //            //if (vm.EntryStatus == ProductEntryStatus.WaitingAudit)
        //            //{
        //            //    this.btnAudit.Visibility = System.Windows.Visibility.Visible;
        //            //}
        //            ////审核成功 显示提交商检按钮
        //            //if (vm.EntryStatus == ProductEntryStatus.AuditSucess)
        //            //{
        //            //    this.btnToInspection.Visibility = System.Windows.Visibility.Visible;
        //            //}
        //            ////提交商检之后，备案审核状态变为备案中，扩展状态变为待商检
        //            //if (vm.EntryStatus == ProductEntryStatus.Entry || vm.EntryStatus == ProductEntryStatus.EntryFail)
        //            //{
        //            //    this.EntryStatusExLabel.Visibility = System.Windows.Visibility.Visible;
        //            //    this.EntryStatusExTxt.Visibility = System.Windows.Visibility.Visible;
        //            //    //扩展状态为带商检 显示商检按钮
        //            //    if (vm.EntryStatusEx == ProductEntryStatusEx.Inspection)
        //            //    {
        //            //        this.btnInspection.Visibility = System.Windows.Visibility.Visible;
        //            //    }
        //            //    //扩展状态为商检成功 显示提交报关按钮
        //            //    if (vm.EntryStatusEx == ProductEntryStatusEx.InspectionSucess)
        //            //    {
        //            //        this.btnToCustoms.Visibility = System.Windows.Visibility.Visible;
        //            //    }
        //            //    //扩展状态为待报关，显示报关按钮
        //            //    if (vm.EntryStatusEx == ProductEntryStatusEx.Customs)
        //            //    {
        //            //        this.btnCustoms.Visibility = System.Windows.Visibility.Visible;
        //            //    }
                        
        //            //}

        //            this.DataContext = vm;
        //        });
        //    }
        //    else
        //    {
        //        StoreTypecmb.SelectedIndex = 0;
        //        EntryBizcmb.SelectedIndex = 0;
        //        this.DataContext = vm;
        //    }
        //}

        public delegate void showbutton();

        public void ProductMaintainBasicEntryInfo_Loaded(showbutton showProductEntryButton)
        {
            vm = new ProductEntryInfoVM();
            facades = new ProductEntryFacade(CPApplication.Current.CurrentPage);
            string tempSysNo = string.Empty;

            if (CurrentPage.Context.Request.QueryString != null)
            {
                tempSysNo = CurrentPage.Context.Request.QueryString["ProductSysNo"];
            }
            if (!string.IsNullOrEmpty(CurrentPage.Context.Request.Param))
            {
                tempSysNo = CurrentPage.Context.Request.Param;
            }
            int productSysNo;
            if (Int32.TryParse(tempSysNo, out productSysNo))
            {
                facades.LoadProductEntryInfo(productSysNo, (obj, args) =>
                {
                    if (args.Result == null)
                    {
                        CPApplication.Current.CurrentPage.Context.Window.Alert("商品信息错误!");
                        return;

                    }

                    vm = EntityConverter<ProductEntryInfo, ProductEntryInfoVM>.Convert(
                                                   args.Result,
                                                   (s, t) =>
                                                   { });
                    EntryBizcmb.SelectedIndex = 0;
                    StoreTypecmb.SelectedIndex = 0;
                    showProductEntryButton();
                    this.DataContext = vm;
                });
            }
            else
            {
                StoreTypecmb.SelectedIndex = 0;
                EntryBizcmb.SelectedIndex = 0;
                this.DataContext = vm;
            }
        }

        ///// <summary>
        ///// 提交商检
        ///// </summary>
        ///// <param name="sender"></param>
        ///// <param name="e"></param>
        //private void ToInspection_Click(object sender, System.Windows.RoutedEventArgs e)
        //{
        //    facades.ToInspection(vm.ProductSysNo.Value, (args) =>
        //    {
        //        if (args)
        //        {
        //            CPApplication.Current.CurrentPage.Context.Window.Alert("提交商检成功！");
        //            CPApplication.Current.CurrentPage.Context.Window.Alert("Reload！");
        //        }
        //        else
        //        {
        //            CPApplication.Current.CurrentPage.Context.Window.Alert("提交商检失败！");
        //        }
        //    });
        //}

        ///// <summary>
        ///// 提交报关
        ///// </summary>
        ///// <param name="sender"></param>
        ///// <param name="e"></param>
        //private void ToCustoms_Click(object sender, System.Windows.RoutedEventArgs e)
        //{
        //    facades.ToCustoms(vm.ProductSysNo.Value, (args) =>
        //    {
        //        if (args)
        //        {
        //            CPApplication.Current.CurrentPage.Context.Window.Alert("提交报关成功！");
        //            CPApplication.Current.CurrentPage.Context.Window.Alert("Reload！");
        //        }
        //        else
        //        {
        //            CPApplication.Current.CurrentPage.Context.Window.Alert("提交报关失败！");
        //        }
        //    });
        //}

        //private void Audit_Click(object sender, System.Windows.RoutedEventArgs e)
        //{
        //    UCEntryStatusOperation addNews = new UCEntryStatusOperation(EntryStatusOperation.Audit, vm.ProductSysNo.Value);
        //    addNews.dialog = MyWindow.ShowDialog("备案审核", addNews, (obj, args) =>
        //    {
        //        if (args.DialogResult == DialogResultType.OK)
        //        {
        //            CPApplication.Current.CurrentPage.Context.Window.Alert("Reload！");
        //        }
        //    });
        //}

        //private void Inspection_Click(object sender, System.Windows.RoutedEventArgs e)
        //{
        //    UCEntryStatusOperation addNews = new UCEntryStatusOperation(EntryStatusOperation.Inspection, vm.ProductSysNo.Value);
        //    addNews.dialog = MyWindow.ShowDialog("备案商检", addNews, (obj, args) =>
        //    {
        //        if (args.DialogResult == DialogResultType.OK)
        //        {
        //            CPApplication.Current.CurrentPage.Context.Window.Alert("Reload！");
        //        }
        //    });
        //}

        //private void Customs_Click(object sender, System.Windows.RoutedEventArgs e)
        //{
        //    UCEntryStatusOperation addNews = new UCEntryStatusOperation(EntryStatusOperation.Customs, vm.ProductSysNo.Value);
        //    addNews.dialog = MyWindow.ShowDialog("备案报关", addNews, (obj, args) =>
        //    {
        //        if (args.DialogResult == DialogResultType.OK)
        //        {
        //            CPApplication.Current.CurrentPage.Context.Window.Alert("Reload！");
        //        }
        //    });
        //}

        public void Save()
        {
            ValidationManager.Validate(LayoutRootEntryInfo);
            Regex RtaxQty = new Regex(@"^\d{1,10}(\.\d{0,10})?$");
            if (!string.IsNullOrEmpty(vm.TaxQty))
            {

                if ((!RtaxQty.IsMatch(vm.TaxQty)) || vm.TaxQty.Trim().Length > 10)
                {
                    CPApplication.Current.CurrentPage.Context.Window.Alert(string.Format("计税单位数量({0})输入错误!", vm.TaxQty));
                    return;
                }
            }

            if (!string.IsNullOrEmpty(vm.ApplyQty))
            {
                int tempApplyQty = 0;
                if (!int.TryParse(vm.ApplyQty, out tempApplyQty) || vm.ApplyQty.Trim().Length > 10)
                {
                    CPApplication.Current.CurrentPage.Context.Window.Alert(string.Format("申报数量({0})输入错误!", vm.ApplyQty));
                    return;
                }
            }

            Regex r1 = new Regex(@"^\d{1,8}(\.\d{1,2})?$");
            if (!string.IsNullOrEmpty(vm.SuttleWeight))
            {

                if (!r1.IsMatch(vm.SuttleWeight))
                {
                    CPApplication.Current.CurrentPage.Context.Window.Alert(string.Format("净重（{0}）输入错误!", vm.SuttleWeight));
                    return;
                }
            }

            if (!string.IsNullOrEmpty(vm.GrossWeight))
            {
                if (!r1.IsMatch(vm.GrossWeight))
                {
                    CPApplication.Current.CurrentPage.Context.Window.Alert(string.Format("毛重（{0}）输入错误!", vm.GrossWeight));
                    return;
                }
            }

            Regex r2 = new Regex(@"^0\.[0-9]{0,2}$");
            if (!string.IsNullOrEmpty(vm.TariffRate))
            {
                if (!r2.IsMatch(vm.TariffRate))
                {
                    CPApplication.Current.CurrentPage.Context.Window.Alert(string.Format("税率（{0}）输入错误!", vm.TariffRate));
                    return;
                }
            }

            if (vm.ProductTradeType == TradeType.FTA)
            {
                if (string.IsNullOrWhiteSpace(vm.Note))
                {
                    CPApplication.Current.CurrentPage.Context.Window.Alert("自贸商品必填其他备注！");
                    return;
                }
                if (!vm.NeedValid.HasValue)
                {
                    CPApplication.Current.CurrentPage.Context.Window.Alert("自贸商品必选商品需效期！");
                    return;
                }
                if (!vm.NeedLabel.HasValue)
                {
                    CPApplication.Current.CurrentPage.Context.Window.Alert("自贸商品必选需粘贴中文标签！");
                    return;
                }
            }

            facades.Update(vm, (args) =>
            {

                if (args)
                {
                    CPApplication.Current.CurrentPage.Context.Window.Alert("更新成功！");
                }
                else
                {
                    CPApplication.Current.CurrentPage.Context.Window.Alert("更新失败！");
                }
            });
        }
    }

}
