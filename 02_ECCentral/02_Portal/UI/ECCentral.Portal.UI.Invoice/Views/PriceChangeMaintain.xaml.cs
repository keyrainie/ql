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
using System.Windows.Navigation;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using Newegg.Oversea.Silverlight.Controls;
using System.Collections.ObjectModel;
using ECCentral.Portal.UI.Invoice.Models;
using ECCentral.Portal.UI.Invoice.Facades;
using ECCentral.Portal.Basic.Components.UserControls.ProductPicker;
using Newegg.Oversea.Silverlight.Controls.Components;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.Portal.UI.Invoice.UserControls;
using ECCentral.BizEntity.Invoice;
using Newegg.Oversea.Silverlight.Utilities.Validation;
using ECCentral.Portal.Basic;
using ECCentral.Portal.UI.Invoice.Resources;

namespace ECCentral.Portal.UI.Invoice.Views
{
    [View(IsSingleton = true, SingletonType = SingletonTypes.Url)]
    public partial class PriceChangeMaintain : PageBase
    {
        private PriceChangeVM vm;
        private InvoiceFacade facade;

        private int? _requestSysNo;
        public int? RequestSysNo
        {
            get
            {
                if (!_requestSysNo.HasValue)
                {
                    int tSysNo = 0;
                    if (!string.IsNullOrEmpty(Request.Param) && int.TryParse(Request.Param, out tSysNo))
                    {
                        _requestSysNo = tSysNo;
                    }
                }
                return _requestSysNo;
            }
            set
            {
                _requestSysNo = value;
            }
        }

        public PriceChangeMaintain()
        {
            InitializeComponent();
        }

        public override void OnPageLoad(object sender, EventArgs e)
        {
            base.OnPageLoad(sender, e);

            this.vm = new PriceChangeVM();
            this.vm.ItemList = new ObservableCollection<ChangeItemVM>();
            this.facade = new InvoiceFacade(this);

            if (RequestSysNo.HasValue)
            {
                this.vm.SysNo = RequestSysNo;
                facade.GetPriceChangeBySysNo(this.vm.SysNo.Value, (vm) =>
                {
                    this.vm = vm;
                    this.root.DataContext = this.vm;

                    SetControllerVisibility();
                });
            }
            else
            {
                SetControllerVisibility();
                this.root.DataContext = this.vm;
            }
        }

        private void PriceType_Changed(object sender, RoutedEventArgs e)
        {
            Combox cb = sender as Combox;
            if (cb.SelectedValue != null)
            {
                foreach (var item in this.vm.ItemList)
                {
                    item.PriceType = (RequestPriceType?)cb.SelectedValue;
                }

                if ((RequestPriceType)cb.SelectedValue == RequestPriceType.PurchasePrice)
                {
                    this.productgd.Columns[5].Visibility = System.Windows.Visibility.Collapsed;
                    
                }
                else if ((RequestPriceType)cb.SelectedValue == RequestPriceType.SalePrice)
                {
                    this.productgd.Columns[5].Visibility = System.Windows.Visibility.Visible;
                }
            }
        }

        private void DataGridCheckBoxAll_Click(object sender, RoutedEventArgs e)
        {
            CheckBox cb = sender as CheckBox;

            foreach (var item in this.vm.ItemList)
            {
                item.IsChecked = cb.IsChecked ?? false;
            }
        }

        private void hlbDelete_Click(object sender, RoutedEventArgs e)
        {
            var selected = this.productgd.SelectedItem as ChangeItemVM;

            ChangeItemVM temp = this.vm.ItemList.FirstOrDefault(wc => wc.ProductSysNo == selected.ProductSysNo);

            if (null != temp)
            {
                this.vm.ItemList.Remove(temp);
            }
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidationManager.Validate(this.basicgd))
                return;

            if (!ValidationManager.Validate(this.productgd))
                return;

            if (vm.BeginDate > vm.EndDate)
            {
                //Window.Alert("结束日期不能小于开始日期!");
                Window.Alert(ResPriceChangeMaintain.Msg_EndDateNeedMoreThanBeginDate);
                return;
            }
            if (Convert.ToDateTime(vm.BeginDate.Value).CompareTo(Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd"))) < 0)
            {
                //Window.Alert("开始日期不能小于当前日期");
                Window.Alert(ResPriceChangeMaintain.Msg_EndDateNeedMoreThanBeginDate);
                return;
            }

                

            if (this.vm.SysNo.HasValue)
            {
                UpdatePriceChange();
            }
            else
            {
                SavePriceChange();
            }
        }

        private void btnAudit_Click(object sender, RoutedEventArgs e)
        {
            if (!AuthMgr.HasFunctionPoint(AuthKeyConst.Invoice_PriceChange_Audit))
            {
                //Window.Alert("不能进行审核操作，你没有审核权限！");
                Window.Alert(ResPriceChangeMaintain.Msg_NoAuditAuthority);
                return;
            }

            UCPriceChangeSetter uc = new UCPriceChangeSetter(this.vm);
            uc.IsAuditModel = true;

            IDialog dialog = Window.ShowDialog(null, uc, (obj1, args1) => 
            {
                if (args1.DialogResult == DialogResultType.OK)
                {
                    this.vm.AudtiMemo = Convert.ToString(args1.Data);

                    if (!string.IsNullOrEmpty(this.vm.AudtiMemo))
                    {
                        Dictionary<int, string> dic = new Dictionary<int, string>();
                        dic.Add(this.vm.SysNo.Value, this.vm.AudtiMemo);

                        facade.BatchAuditPriceChange(dic, (msg) =>
                        {
                            Window.Alert(ResPriceChangeMaintain.Msg_Tips, msg, Newegg.Oversea.Silverlight.Controls.Components.MessageType.Information, (obj, args) =>
                            {
                                Window.Refresh();
                            });
                        });
                    }
                    else
                    {
                        //Window.Alert("请输入审核备注信息！");
                        Window.Alert(ResPriceChangeMaintain.Msg_NeedEnterMemoInfo);
                    }
                }

            });
            uc.DialogHanlder = dialog;
        }

        private void btnVoid_Click(object sender, RoutedEventArgs e)
        {

            facade.BatchVoidPriceChange(new List<int>{this.vm.SysNo.Value}, (msg) =>
            {
                Window.Alert(ResPriceChangeMaintain.Msg_Tips, msg, Newegg.Oversea.Silverlight.Controls.Components.MessageType.Information, (obj, args) =>
                {
                    Window.Refresh();
                });
            });
        }

        private void btnRun_Click(object sender, RoutedEventArgs e)
        {

            facade.BatchRunPriceChange(new List<int> { this.vm.SysNo.Value }, (msg) =>
            {
                Window.Alert(ResPriceChangeMaintain.Msg_Tips, msg, Newegg.Oversea.Silverlight.Controls.Components.MessageType.Information, (obj, args) =>
                {
                    Window.Refresh();
                });
            });
        }

        private void btnAbort_Click(object sender, RoutedEventArgs e)
        {

            facade.BatchAbortPriceChange(new List<int> { this.vm.SysNo.Value }, (msg) =>
            {
                Window.Alert(ResPriceChangeMaintain.Msg_Tips, msg, Newegg.Oversea.Silverlight.Controls.Components.MessageType.Information, (obj, args) =>
                {
                    Window.Refresh();
                });
            });
        }

        private void btnPrint_Click(object sender, RoutedEventArgs e)
        {
            if (RequestSysNo.HasValue)
            {
                Dictionary<string, string> t = new Dictionary<string, string>();
                t.Add("SysNo", RequestSysNo.Value.ToString());
                HtmlViewHelper.WebPrintPreview("Invoice", "PriceChange", t);
            }
        }

        private void SavePriceChange()
        {
            facade.SavePriceChange(this.vm, (key) =>
            {
                if (key > 0)
                {
                    Window.Alert(ResPriceChangeMaintain.Msg_Tips, ResPriceChangeMaintain.Msg_SaveSuccess, MessageType.Information, (obj, args) =>
                    {
                        Window.Close();
                        Window.Navigate(string.Format("/ECCentral.Portal.UI.Invoice/PriceChangeMaintain/{0}",key), null, true);
                    });
                }
            });
        }

        private void UpdatePriceChange()
        {
            facade.UpdatePriceChange(this.vm, (ret) => 
            {
                Window.Alert(ResPriceChangeMaintain.Msg_Tips, ResPriceChangeMaintain.Msg_UpdateSuccess, MessageType.Information, (obj, args) => 
                {
                    this.vm = ret;
                    SetDataContext();
                });
            });
        }

        private void btnChangeRate_Click(object sender, RoutedEventArgs e)
        {
            UCPriceChangeSetter uc = new UCPriceChangeSetter(this.vm);

            IDialog dialog = Window.ShowDialog(ResPriceChangeMaintain.Msg_SetProportion, uc, (obj1, args1) => 
            {
                if (args1.DialogResult == DialogResultType.OK)
                {
                    decimal rate = Convert.ToDecimal(args1.Data);

                    foreach (var item in this.vm.ItemList)
                    {
                        if (this.vm.PriceType == RequestPriceType.SalePrice)
                        {
                            item.NewPrice = (item.OldPrice * rate).ToString("F2");
                            //item.NewShowPrice = (item.OldShowPrice * rate).ToString("F2");

                            // when newprice change update lessthangrossprofit.
                            if (item.NewPrice != null && item.NewPrice.Trim().Length > 0)
                            {
                                if (((Convert.ToDecimal(item.NewPrice) - item.UnitCost) / item.UnitCost) < item.MinMargin)
                                {
                                    //item.LessThanGrossProfit = "是";
                                    item.LessThanGrossProfit = ResPriceChangeMaintain.Msg_Yes;
                                }
                                else
                                {
                                    //item.LessThanGrossProfit = "否";
                                    item.LessThanGrossProfit = ResPriceChangeMaintain.Msg_No;
                                }
                            }
                        }
                        else
                        {
                            item.NewInstockPrice = (item.OldInstockPrice * rate).ToString("F2");
                        }
                        
                    }
                }
            });

            uc.DialogHanlder = dialog;
        }

        private void btnAddProduct_Click(object sender, RoutedEventArgs e)
        {
            if (!this.vm.PriceType.HasValue)
            {
               // Window.Alert("请先选择变价类型!");
                Window.Alert(ResPriceChangeMaintain.Msg_SelectPriceChangeType);
                return;
            }

            SetControllerVisibility();

            facade.GetVendorLastBasicInfo((obj1,args1) => 
            {
                UCProductSearch uc = new UCProductSearch();

                //if (!args1.FaultsHandle())
                //{
                //    dynamic ret = args1.Result;

                //    int vendorSysNo;
                //    string vendorName;

                //    if (ret != null && ret.ToList().Count > 0)
                //    {
                //        vendorSysNo = Convert.ToInt32(ret.ToList()[0].VendorSysNo);
                //        vendorName = Convert.ToString(ret.ToList()[0].VendorName);

                //        uc = new UCProductSearch(vendorSysNo, vendorName);
                //    }
                //}

                uc.SelectionMode = SelectionMode.Multiple;

                IDialog dialog = Window.ShowDialog(ResPriceChangeMaintain.Msg_AddProduct, uc, (obj, args) =>
                {
                    if (args.DialogResult == DialogResultType.OK)
                    {
                        List<ProductVM> products = args.Data as List<ProductVM>;
                        if (products != null)
                        {
                            products.ForEach(p =>
                            {
                                ChangeItemVM temp = this.vm.ItemList.FirstOrDefault(item => { return item.ProductSysNo == p.SysNo; });
                                if (null == temp)
                                {
                                    ChangeItemVM itemVM = EntityConverter<ProductVM, ChangeItemVM>.Convert(p, (s, t) =>
                                    {
                                        t.ProductSysNo = s.SysNo.Value;
                                        t.OldInstockPrice = s.VirtualPrice ?? 0;
                                        t.OldShowPrice = s.BasicPrice ?? 0;
                                        t.OldPrice = s.CurrentPrice;
                                        t.CurrentPrice = s.CurrentPrice;
                                        t.UnitCost = s.UnitCost;
                                        t.MinMargin = s.MinMargin;
                                        t.IsChecked = false;
                                        t.PriceType = this.vm.PriceType;
                                    });

                                    this.vm.ItemList.Add(itemVM);
                                }
                                else
                                {
                                    //Window.Alert(string.Format("已存在编号为{0}的商品.", p.ProductID));
                                    Window.Alert(string.Format(ResPriceChangeMaintain.Msg_ExsistTheProduct, p.ProductID));
                                }
                            });
                        }
                    }
                });

                uc.DialogHandler = dialog;

            });

            
        }

        private void SetDataContext()
        {
            this.root.DataContext = this.vm;
        }

        private void SetControllerVisibility()
        {
            // 隐藏多选列
            if (this.vm.SysNo.HasValue)
            {
                this.productgd.Columns[0].Visibility = System.Windows.Visibility.Collapsed;
            }
        }

    }
}
