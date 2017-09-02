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
using ECCentral.Portal.UI.MKT.Models;
using ECCentral.Portal.UI.MKT.Facades;
using ECCentral.Portal.Basic.Components.UserControls.ProductPicker;
using Newegg.Oversea.Silverlight.Controls.Components;
using ECCentral.Portal.Basic.Utilities;
using Newegg.Oversea.Silverlight.Utilities.Validation;
using ECCentral.QueryFilter.Common;
using ECCentral.BizEntity.IM;
using System.Collections.ObjectModel;
using Newegg.Oversea.Silverlight.Controls.Data;

namespace ECCentral.Portal.UI.MKT.Views
{
    [View(IsSingleton = true, SingletonType = SingletonTypes.Url)]
    public partial class GiftCardProductMaintain : PageBase
    {
        private GiftCardProductVM _voucherProductVM;
        private GiftCardFacade _facade;
        private List<int> selected;
        private GiftVoucherProductRelationVM selectedRelation;

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

        public GiftCardProductMaintain()
        {
            InitializeComponent();
        }
        /// <summary>
        ///  礼品券商品的3级分别编号
        /// </summary>
        private int? _c3SysNo;
        public override void OnPageLoad(object sender, EventArgs e)
        {
            base.OnPageLoad(sender, e);

            this._voucherProductVM = new GiftCardProductVM();
            this._facade = new GiftCardFacade(this);
            selected = new List<int>();
            this.voucherProduct.ProductSelected += voucherProduct_ProductSelected;

            _facade.GetGiftCardC3SysNo((no) =>
                {
                    _c3SysNo = no;
                });
            if (RequestSysNo.HasValue)
            {
                this._voucherProductVM.SysNo = RequestSysNo.Value;

                _facade.GetGiftVoucherProductInfo(RequestSysNo.Value, (ret) =>
                {
                    this._voucherProductVM = ret;
                    foreach (var item in this._voucherProductVM.RelationProducts)
                    {
                        item.IsChecked = false;
                    }
                    SetDataContext();
                });
            }
            else
            {
                //this.voucherProductgd
                SetDataContext();
            }
        }

        private void voucherProduct_ProductSelected(object sender, ProductSelectedEventArgs e)
        {
            ProductVM product = e.SelectedProduct;

        }

        private void SetDataContext()
        {
            this.voucherProductgd.DataContext = this._voucherProductVM;
            if (RequestSysNo.HasValue)
            {
                this.productgd.Bind();
                this.productreqgd.Bind();
            }
        }

        #region UI Event
        private void save_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidationManager.Validate(this))
            {
                return;
            }

            if (!PreCheckCommon())
                return;
            if (voucherProduct.SelectedProductInfo != null && voucherProduct.SelectedProductInfo.C3SysNo != _c3SysNo)
            {
                Window.Alert("相关联的商品类型不是礼品卡商品！");
                return;
            }
            if (this.RequestSysNo.HasValue)
            {
                UpdateVoucherProduct();
            }
            else
            {

                SaveVoucherProduct();
            }
        }

        private void UpdateVoucherProduct()
        {
            _facade.UpdateVoucherProductInfo(_voucherProductVM, (result) =>
            {
                if (result == null)
                {
                    Window.Alert("更新失败！");
                }
                else
                {
                    Window.Alert("提示", "更新成功", MessageType.Information, (obj, args) =>
                    {
                        //_facade.GetGiftVoucherProductInfo(RequestSysNo.Value, (vm) =>
                        //{
                        //    Window.Refresh();
                        //});
                        Window.Refresh();
                    });
                }
            });
        }

        private void SaveVoucherProduct()
        {
            _facade.AddGiftVoucherProductInfo(_voucherProductVM, (result) =>
            {
                int sysNo = result;
                if (sysNo > 0)
                {
                    Window.Alert("提示", "创建成功!", MessageType.Information, (obj, args) =>
                    {
                        //this._voucherProductVM.SysNo = sysNo;
                        Window.Close();
                        Window.Navigate(string.Format("/ECCentral.Portal.UI.MKT/GiftCardProductMaintain/{0}", sysNo), null, true);
                    });
                }
            });
        }

        private void hplDel_Click(object sender, RoutedEventArgs e)
        {
            var selected = this.productgd.SelectedItem as GiftVoucherProductRelationVM;

            if (!selected.SysNo.HasValue)
            {
                this._voucherProductVM.RelationProducts.Remove(selected);
                return;
            }

            foreach (GiftVoucherProductRelationVM item in this.productgd.ItemsSource)
            {
                if (item.ProductSysNo == selected.ProductSysNo)
                {
                    item.IsUIClickDel = true;
                    item.Type = GVRReqType.Delete;
                }
            }

        }

        private void hpladdProduct_Click(object sender, RoutedEventArgs e)
        {
            UCProductSearch uc = new UCProductSearch();
            uc.SelectionMode = SelectionMode.Multiple;
            //uc.ProductC3SysNo = _c3SysNo;

            IDialog dialog = Window.ShowDialog("添加商品", uc, (obj, args) =>
            {
                if (args.DialogResult == DialogResultType.OK)
                {
                    List<ProductVM> products = args.Data as List<ProductVM>;
                    if (products != null && products.Count > 0)
                    {
                        products.ForEach(p =>
                        {
                            GiftVoucherProductRelationVM temp = this._voucherProductVM.RelationProducts.FirstOrDefault(item =>
                            {
                                return item.ProductSysNo == p.SysNo;
                            });

                            if (temp == null)
                            {
                                GiftVoucherProductRelationVM vm = EntityConverter<ProductVM, GiftVoucherProductRelationVM>.Convert(p, (s, t) =>
                                {
                                    t.ProductSysNo = s.SysNo.Value;
                                    t.ProductStatus = s.Status;
                                    t.SysNo = null;
                                });
                                vm.IsChecked = false;
                                vm.Type = GVRReqType.Add;
                                vm.SaleInWeb = true;
                                _voucherProductVM.RelationProducts.Add(vm);
                            }
                            else
                            {
                                Window.Alert(string.Format("已存在编号为{0}的商品.", p.ProductID));
                            }
                        });

                        this.productgd.ItemsSource = this._voucherProductVM.RelationProducts;
                    }
                }
            });
            uc.DialogHandler = dialog;
        }

        //private void ProductName_Click(object sender, RoutedEventArgs e) { }

        //private void DataGridCheckBoxAll_Click(object sender, RoutedEventArgs e)
        //{
        //    selected.Clear();

        //    foreach (var item in this._voucherProductVM.RelationProducts)
        //    {
        //        if (item.IsChecked)
        //        {
        //            item.IsChecked = false;
        //        }
        //        else
        //        {
        //            item.IsChecked = true;
        //            selected.Add(item.SysNo);
        //        }
        //    }
        //}

        //private void ReqDataGridCheckBoxAll_Click(object sender, RoutedEventArgs e)
        //{

        //}

        private void audit_Click(object sender, RoutedEventArgs e)
        {
            selected.Clear();

            string tip = string.Empty;

            foreach (GiftVoucherProductRelationReqVM item in this.productreqgd.ItemsSource)
            {
                if (item.IsChecked)
                {
                    selected.Add(item.SysNo);
                }
            }

            if (!string.IsNullOrEmpty(tip))
            {
                Window.Alert("提示", tip, MessageType.Information, (obj, args) =>
                {
                    Window.Refresh();
                });
                return;
            }

            if (selected.Count == 0)
            {
                Window.Alert("请选择至少选择一个商品！");
                return;
            }

            if (selected != null && selected.Count > 0)
            {

                _facade.BatchAuditVoucherRequest(selected, (msg) =>
                {
                    Window.Alert("提示", msg, MessageType.Information, (obj, args) =>
                    {
                        //_facade.GetGiftVoucherProductInfo(RequestSysNo.Value, (vm) =>
                        //{
                        //    this._voucherProductVM = vm;
                        //    Window.Refresh();
                        //});
                        Window.Refresh();
                    });
                });
            }

        }

        private void cancelAudit_Click(object sender, RoutedEventArgs e)
        {
            selected.Clear();

            string tip = string.Empty;

            foreach (GiftVoucherProductRelationReqVM item in this.productreqgd.ItemsSource)
            {
                if (item.IsChecked)
                {
                    selected.Add(item.SysNo);
                }
            }

            if (!string.IsNullOrEmpty(tip))
            {
                Window.Alert("提示", tip, MessageType.Information, (obj, args) =>
                {
                    Window.Refresh();
                });
                return;
            }

            if (selected.Count == 0)
            {
                Window.Alert("请选择至少选择一个商品！");
                return;
            }

            if (selected != null && selected.Count > 0)
            {

                _facade.BatchCancelAuditVoucherRequest(selected, (msg) =>
                {
                    Window.Alert("提示", msg, MessageType.Information, (obj, args) =>
                    {
                        Window.Refresh();
                    });
                });
            }
        }

        private void productreqgd_LoadingDataSource(object sender, LoadingDataEventArgs e)
        {
            _facade.QueryGiftVoucherProductRelationReq(new GiftCardProductFilterVM { SysNo = RequestSysNo }, e.PageSize, e.PageIndex, e.SortField, (obj, args) =>
            {

                ObservableCollection<GiftVoucherProductRelationReqVM> coll = DynamicConverter<GiftVoucherProductRelationReqVM>.ConvertToVMList<ObservableCollection<GiftVoucherProductRelationReqVM>>(args.Result.Rows);
                this.productreqgd.ItemsSource = coll;
                this.productreqgd.TotalCount = args.Result.TotalCount;
            });
        }

        private void productgd_LoadingDataSource(object sender, LoadingDataEventArgs e)
        {
            _facade.QueryGiftVoucherProductRelation(new GiftCardProductFilterVM { SysNo = RequestSysNo }, e.PageSize, e.PageIndex, e.SortField, (ret, totalCount) =>
            {

                if (this._voucherProductVM.RelationProducts == null)
                {
                    this._voucherProductVM.RelationProducts = new ObservableCollection<GiftVoucherProductRelationVM>();
                }
                this._voucherProductVM.RelationProducts = ret;
                this.productgd.ItemsSource = ret;
                this.productgd.TotalCount = totalCount;
            });
        }

        private void productgd_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //HyperlinkButton hlbtn = sender as HyperlinkButton;

            //if (e.AddedItems != null && e.AddedItems.Count > 0)
            //{
            //    GiftVoucherProductRelationVM selected = e.AddedItems[0] as GiftVoucherProductRelationVM;
            //    if (null != selected&&selected.SysNo.HasValue)
            //    {
            //        this.selectedRelation = selected;
            //        this.productreqgd.Bind();
            //    }
            //}
        }
        #endregion


        private bool PreCheckCommon()
        {
            bool resutl = true;
            string tip = string.Empty;
            if (this._voucherProductVM.RelationProducts.Count < 1)
            {
                tip += "请添加商品信息！\r\n";
                resutl = false;
            }
            else
            {
                foreach (var item in this._voucherProductVM.RelationProducts)
                {
                    if (item.ProductSysNo == Convert.ToInt32(this._voucherProductVM.ProductSysNo))
                    {
                        resutl = false;
                        tip += string.Format("兑换商品列表不能包含关联商品【{0}】\r\n", item.ProductID);
                    }
                }
            }

            if (!string.IsNullOrEmpty(tip))
            {
                Window.Alert(tip);
            }

            return resutl;
        }

        private void ReqDataGridCheckBoxAll_Click(object sender, RoutedEventArgs e)
        {
            selected.Clear();
            var checkBoxAll = sender as CheckBox;
            if (productreqgd.ItemsSource.GetCount() > 0 && checkBoxAll != null)
            {
                foreach (GiftVoucherProductRelationReqVM item in this.productreqgd.ItemsSource)
                {
                    item.IsChecked = checkBoxAll.IsChecked ?? false;
                    if (item.IsChecked)
                    {
                        selected.Add(item.SysNo);
                    }
                }
            }
        }
    }
}
