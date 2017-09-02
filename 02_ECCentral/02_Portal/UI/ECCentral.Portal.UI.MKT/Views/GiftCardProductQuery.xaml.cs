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
using ECCentral.QueryFilter;
using ECCentral.Portal.UI.MKT.Facades;
using ECCentral.Portal.UI.MKT.Models;
using Newegg.Oversea.Silverlight.Utilities.Validation;
using ECCentral.Portal.UI.MKT.UserControls;
using Newegg.Oversea.Silverlight.Controls.Components;

namespace ECCentral.Portal.UI.MKT.Views
{
    [View(IsSingleton = true, SingletonType = SingletonTypes.Url)]
    public partial class GiftCardProductQuery : PageBase
    {
        private GiftCardProductFilter _filter;
        private GiftCardProductFilterVM _filterVM;

        private GiftCardFacade _facade;
        private List<int> selectedSysNo;

        public GiftCardProductQuery()
        {
            InitializeComponent();
        }

        public override void OnPageLoad(object sender, EventArgs e)
        {
            base.OnPageLoad(sender, e);

            _filter = new GiftCardProductFilter();
            _filterVM = new GiftCardProductFilterVM();

            _facade = new GiftCardFacade(this);
            selectedSysNo = new List<int>();

            this.filtergd.DataContext = _filterVM;
        }


        #region UI Event

        private void Button_Search_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidationManager.Validate(this.filtergd))
                return;

            this.giftVoucherProductdg.Bind();
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            //Window.Navigate("/ECCentral.Portal.UI.MKT/GiftCardProductMaintain", null, true);
            UCGiftCardProductMaintain ucMaintain = new UCGiftCardProductMaintain();
            
            ucMaintain.giftCardProductVM = new GiftCardProductVM();

            //this.Window.ShowDialog("礼品卡商品维护", ucMaintain);
            ucMaintain.Dialog = Window.ShowDialog("礼品卡商品新增", ucMaintain, (obj, args) =>
            {
                if (args != null)
                {
                    if (args.DialogResult == DialogResultType.OK)
                        giftVoucherProductdg.Bind();
                }
            });
        }

        private void btnAudit_Click(object sender, RoutedEventArgs e)
        {
            selectedSysNo.Clear();

            dynamic source = this.giftVoucherProductdg.ItemsSource as dynamic;

            foreach (var item in source)
            {
                if (item.IsChecked)
                {
                    selectedSysNo.Add(item.SysNo);
                }
            }

            if (selectedSysNo.Count > 0)
            {
                _facade.BatchAuditGiftCardProduct(selectedSysNo, (msg) => 
                {
                    Window.Alert("提示信息", msg, Newegg.Oversea.Silverlight.Controls.Components.MessageType.Information, (obj, args) =>
                    {
                        this.giftVoucherProductdg.Bind();
                    });
                });
            }
            else
            {
                Window.Alert("请选择礼品卡商品！");
                return;
            }
        }

        private void btnCancelAudit_Click(object sender, RoutedEventArgs e)
        {
            selectedSysNo.Clear();

            dynamic source = this.giftVoucherProductdg.ItemsSource as dynamic;

            foreach (var item in source)
            {
                if (item.IsChecked)
                {
                    selectedSysNo.Add(item.SysNo);
                }
            }

            if (selectedSysNo.Count > 0)
            {
                _facade.BatchCancelAuditGiftCardProduct(selectedSysNo, (msg) =>
                {
                    Window.Alert("提示信息", msg, Newegg.Oversea.Silverlight.Controls.Components.MessageType.Information, (obj, args) =>
                    {
                        this.giftVoucherProductdg.Bind();
                    });
                });
            }
            else
            {
                Window.Alert("请选择礼品卡商品！");
                return;
            }
        }

        private void btnVoid_Click(object sender, RoutedEventArgs e)
        {
            selectedSysNo.Clear();

            dynamic source = this.giftVoucherProductdg.ItemsSource as dynamic;

            foreach (var item in source)
            {
                if (item.IsChecked)
                {
                    selectedSysNo.Add(item.SysNo);
                }
            }

            if (selectedSysNo.Count > 0)
            {
                _facade.BatchVoidGiftCardProduct(selectedSysNo, (msg) =>
                {
                    Window.Alert("提示信息", msg, Newegg.Oversea.Silverlight.Controls.Components.MessageType.Information, (obj, args) => 
                    {
                        this.giftVoucherProductdg.Bind();
                    });
                });
            }
            else
            {
                Window.Alert("请选择礼品卡商品！");
                return;
            }
        }

        private void hlEdit_Click(object sender, RoutedEventArgs e)
        {
            var selected = this.giftVoucherProductdg.SelectedItem as dynamic;
            //Window.Navigate(string.Format("/ECCentral.Portal.UI.MKT/GiftCardProductMaintain/{0}", selected.SysNo), null, true);
            UCGiftCardProductMaintain ucMaintain = new UCGiftCardProductMaintain();
            GiftCardProductVM item = ECCentral.Portal.Basic.Utilities.DynamicConverter<GiftCardProductVM>.ConvertToVM(selected);
            item.Price =decimal.Parse(item.Price).ToString("f2");
            ucMaintain.giftCardProductVM = item;

            //礼品卡商品维护
            ucMaintain.Dialog = Window.ShowDialog("礼品卡商品维护", ucMaintain, (obj, args) =>
            {
                if (args != null)
                {
                    if (args.DialogResult == DialogResultType.OK)
                        giftVoucherProductdg.Bind();
                }
            });
        }

        private void btnBatchDelete_Click(object sender, RoutedEventArgs e)
        {
            selectedSysNo.Clear();

            dynamic source = this.giftVoucherProductdg.ItemsSource as dynamic;
            if (source == null)
            {
                Window.Alert("请选择礼品卡商品！");
                return;
            }
            foreach (var item in source)
            {
                if (item.IsChecked)
                {
                    selectedSysNo.Add(item.SysNo);
                }
            }
            if (selectedSysNo.Count > 0)
            {
                _facade.BatchVoidGiftCardProduct(selectedSysNo, (msg) =>
                {
                    Window.Alert("提示信息", msg, Newegg.Oversea.Silverlight.Controls.Components.MessageType.Information, (obj, args) =>
                    {
                        this.giftVoucherProductdg.Bind();
                    });
                });
            }
            else
            {
                Window.Alert("请选择礼品卡商品！");
                return;
            }
        }
        private void DataGridCheckBoxAll_Click(object sender, RoutedEventArgs e)
        {
            CheckBox ckb = sender as CheckBox;

            dynamic source = this.giftVoucherProductdg.ItemsSource as dynamic;
            selectedSysNo.Clear();
            if (source != null)
            {
                foreach (var item in source)
                {

                    item.IsChecked = ckb.IsChecked;
                }

                this.giftVoucherProductdg.ItemsSource = source;
            }
        }

        #endregion

        

        private void giftVoucherProductdg_LoadingDataSource(object sender, Newegg.Oversea.Silverlight.Controls.Data.LoadingDataEventArgs e)
        {
            _facade.QueryGiftCardProduct(this._filterVM, e.PageSize, e.PageIndex, e.SortField, (obj, args) =>
            {
                this.giftVoucherProductdg.ItemsSource = args.Result.Rows.ToList("IsChecked", false);
                this.giftVoucherProductdg.TotalCount = args.Result.TotalCount;
            });
        }
    }
}
