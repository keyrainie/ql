using System;
using System.Linq;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

using ECCentral.BizEntity.MKT;
using ECCentral.Portal.Basic.Components.UserControls.ProductPicker;
using ECCentral.Portal.UI.MKT.Facades;
using ECCentral.Portal.UI.MKT.Models;
using ECCentral.Portal.UI.MKT.Resources;
using ECCentral.Portal.UI.MKT.UserControls;

using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using Newegg.Oversea.Silverlight.Controls;
using Newegg.Oversea.Silverlight.Controls.Components;
using Newegg.Oversea.Silverlight.Utilities.Validation;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.BizEntity.IM;

namespace ECCentral.Portal.UI.MKT.Views
{
    [View(IsSingleton = true, SingletonType = SingletonTypes.Url)]
    public partial class GroupBuyingMaintain : PageBase
    {
        private string _op;
        private string sysNo;
        private GroupBuyingMaintainVM _viewModel;
        private GroupBuyingFacade _Facade;

        public GroupBuyingMaintain()
        {
            InitializeComponent();
           
        }


        void ucItemMaster_ProductSelected(object sender, ProductSelectedEventArgs e)
        {
            if (e.SelectedProduct.ProductType != ProductType.Virtual&& _viewModel.CategoryType== GroupBuyingCategoryType.Virtual)
            {
                //Window.Alert("虚拟团购只能选择虚拟商品！");
                Window.Alert(ResGroupBuyingMaintain.Info_VirtualByVirtual);
                _viewModel.ProductSysNo = null;
                _viewModel.ProductID = null;
                return;
            }
            _viewModel.BasicPrice = e.SelectedProduct.BasicPrice.HasValue ? Math.Round(e.SelectedProduct.BasicPrice.Value, 2) : new decimal?();
            
            if(_viewModel.Status.HasValue 
                && _viewModel.Status.Value== GroupBuyingStatus.Active 
                && _viewModel.Status.Value== GroupBuyingStatus.Finished)
            {
                return;
            }
            _viewModel.OriginalPrice = Math.Round(e.SelectedProduct.CurrentPrice, 2);

            //new ExternalServiceFacade(this).GetProductInfo(e.SelectedProduct.SysNo.Value, (obj, args) =>
            //{
            //    if (args.FaultsHandle())
            //        return;
            //    _viewModel.GroupBuyingDescLong = args.Result.ProductBasicInfo.LongDescription.Content;
            //    _viewModel.GroupBuyingVendorSysNo = args.Result.Merchant.SysNo.Value;
            //    _viewModel.GroupBuyingVendorName = args.Result.Merchant.MerchantName;
            //});
            /*
            new GroupBuyingFacade(this).GetProductOriginalPrice(e.SelectedProduct.SysNo.Value, _viewModel.IsByGroup ? "Y" : "N", (obj, args) =>
            {
                _viewModel.OriginalPrice = (args.Result == null || args.Result[0] == null ? 0m : decimal.Round(decimal.Parse(args.Result[0].ToString()), 2));
                //if (args.Result.Count > 4)
                //{
                //    _viewModel.GroupBuyingVendorSysNo = int.Parse(args.Result[3].ToString());
                //    _viewModel.GroupBuyingVendorName = args.Result[4].ToString();
                //}
            });*/
        }

        public override void OnPageLoad(object sender, EventArgs e)
        {
            base.OnPageLoad(sender, e);
            this.ucItemMaster.ProductSelected += new EventHandler<ProductSelectedEventArgs>(ucItemMaster_ProductSelected);

            _Facade = new GroupBuyingFacade(this);
            _viewModel = new GroupBuyingMaintainVM();

            if (this.Request.QueryString.ContainsKey("op"))
            {
                _op = this.Request.QueryString["op"];
            }
            if (this.Request.QueryString.ContainsKey("sysNo"))
            {
                sysNo = this.Request.QueryString["sysNo"];
                _Facade.Load(sysNo, (s, args) =>
                {
                    _viewModel = args.Result;
                    this.DataContext = _viewModel;
                    _viewModel.ValidationErrors.Clear();

                    if (_viewModel.CategoryType == GroupBuyingCategoryType.Virtual)
                    {
                        new ExternalServiceFacade(this).QueryVendorStoreList(this._viewModel.GroupBuyingVendorSysNo, (se, a) =>
                        {
                            if (a.FaultsHandle())
                            {
                                return;
                            }
                            List<VendorStoreVM> list = DynamicConverter<VendorStoreVM>.ConvertToVMList(a.Result.Rows);
                            _viewModel.VendorStoreList.Clear();
                            list.ForEach(p =>
                            {
                                _viewModel.VendorStoreList.Add(p);
                            });
                            foreach (var item in _viewModel.VendorStoreList)
                            {
                                var re = _viewModel.VendorStoreSysNoList.FirstOrDefault(p => p == item.SysNo.Value);
                                if (re >0)
                                {
                                    item.IsChecked = true;
                                }
                            }
                        });                        
                    }

                    InitPage(sysNo);

                    new GroupBuyingFacade(this).GetProductOriginalPrice(int.Parse(_viewModel.ProductSysNo), _viewModel.IsByGroup ? "Y" : "N", (obj, a) =>
                    {
                        //_viewModel.OriginalPrice = (a.Result == null || a.Result[0] == null ? 0m : decimal.Round(decimal.Parse(a.Result[0].ToString()), 2));
                        if (a.Result.Count > 4)
                        {
                            _viewModel.GroupBuyingVendorSysNo = int.Parse(a.Result[3].ToString());
                            _viewModel.GroupBuyingVendorName = a.Result[4].ToString();
                        }
                    });
                });
            }
            else
            {
                _Facade.GetGroupBuyingTypes((s, args) =>
                {
                    if (!args.FaultsHandle())
                    {
                       // _viewModel.GroupBuyingTypeList.Add(0, ECCentral.BizEntity.Enum.Resources.ResCommonEnum.Enum_All);
                        foreach (var gbt in args.Result)
                        {
                            _viewModel.GroupBuyingTypeList.Add(gbt.Key, gbt.Value);
                        }
                        _Facade.GetGroupBuyingAreas((s1, args1) =>
                        {
                            if (!args1.FaultsHandle())
                            {
                                //_viewModel.GroupBuyingAreaList.Add(0, ECCentral.BizEntity.Enum.Resources.ResCommonEnum.Enum_All);
                                foreach (var gba in args1.Result)
                                {
                                    _viewModel.GroupBuyingAreaList.Add(gba.Key, gba.Value);
                                }
                                this.DataContext = _viewModel;

                                InitPage(sysNo);
                            }
                        });
                    }
                });
            }
            //this.DataContext = _viewModel;
        }

        private void InitPage(string sysNo)
        {
            ButtonStop.Visibility = System.Windows.Visibility.Visible;
            ButtonVoid.Visibility = System.Windows.Visibility.Visible;
            ButtonSave.Visibility = System.Windows.Visibility.Visible;
            #region 审核相关功能
            btnAuditApprove.Visibility = System.Windows.Visibility.Collapsed;
            btnAuditRefuse.Visibility = System.Windows.Visibility.Collapsed;
            btnCancelAudit.Visibility = System.Windows.Visibility.Collapsed;
            btnSubmitAudit.Visibility = System.Windows.Visibility.Collapsed;
            txtGroupBuyingReason.IsEnabled = false;
            #endregion
            ButtonStop.IsEnabled = false;
            ButtonVoid.IsEnabled = false;
            ButtonSave.IsEnabled = false;

            switch (this._op)
            {
                case "mgt":
                    // this.Title = string.Format("团购[{0}]-管理", sysNo);
                    this.Title = string.Format(ResGroupBuyingMaintain.Msg_GroupBuyManage, sysNo);
                    if (_viewModel.Status == GroupBuyingStatus.Active)
                    {
                        ButtonStop.IsEnabled = true;
                    }
                    if (_viewModel.Status == GroupBuyingStatus.Pending
                        || _viewModel.Status == GroupBuyingStatus.Init
                        || _viewModel.Status == GroupBuyingStatus.WaitingAudit
                        || _viewModel.Status == GroupBuyingStatus.VerifyFaild)
                    {
                        ButtonVoid.IsEnabled = true;
                    }
                    this.SetControlEnable(_viewModel.Status ?? GroupBuyingStatus.Init);
                    break;
                case "new":
                    //this.Title = "团购-新建";
                    this.Title = ResGroupBuyingMaintain.Msg_GroupNew;
                    ButtonStop.Visibility = System.Windows.Visibility.Collapsed;
                    ButtonVoid.Visibility = System.Windows.Visibility.Collapsed;
                    ButtonSave.Visibility = System.Windows.Visibility.Visible;
                    ButtonSave.IsEnabled = true;
                    this.SetControlEnable(_viewModel.Status ?? GroupBuyingStatus.Init);
                    #region 隐藏审核理由
                    txtBGroupBuyingReason.Visibility = System.Windows.Visibility.Collapsed;
                    txtGroupBuyingReason.Visibility = System.Windows.Visibility.Collapsed;
                    #endregion

                    if (_viewModel.GroupBuyingTypeList.Count > 0) { this.cmbGroupBuyingType.SelectedIndex = 0; }
                    if (_viewModel.GroupBuyingAreaList.Count > 0) { this.cmbGroupBuyingArea.SelectedIndex = 0; }
                    break;
                case "edt":
                    //this.Title = string.Format("团购[{0}]-编辑", sysNo);
                    this.Title = string.Format(ResGroupBuyingMaintain.Msg_GroupEdit, sysNo);
                    if (_viewModel.Status == GroupBuyingStatus.Active
                        || _viewModel.Status == GroupBuyingStatus.Init
                        || _viewModel.Status == GroupBuyingStatus.Pending
                        || _viewModel.Status == GroupBuyingStatus.VerifyFaild)
                    {
                        ButtonSave.IsEnabled = true;
                    }
                    this.SetControlEnable(_viewModel.Status ?? GroupBuyingStatus.Init);
                    break;
                default:
                    ButtonStop.Visibility = System.Windows.Visibility.Collapsed;
                    ButtonVoid.Visibility = System.Windows.Visibility.Collapsed;
                    ButtonSave.Visibility = System.Windows.Visibility.Collapsed;
                    // this.Title = string.Format("团购[{0}]-查看", sysNo);
                    this.Title = string.Format(ResGroupBuyingMaintain.Msg_GroupView, sysNo);
                    SetAllReadOnly();
                    break;
            }
        }

        private void SetControlEnable(GroupBuyingStatus _gbStatus)
        {
            if (_gbStatus == GroupBuyingStatus.WaitingAudit || _gbStatus == GroupBuyingStatus.Pending
                || _gbStatus == GroupBuyingStatus.WaitHandling || _gbStatus == GroupBuyingStatus.Active)
            {
                ucItemMaster.IsEnabled = false;
                cmbGroupBuyingType.IsEnabled = false;
                chkIsByGroup.IsEnabled = false;
                dtBeginDateTime.IsEnabled = false;             
                dtEndDateTime.IsEnabled = false;
                rbNoLimit.IsEnabled = false;
                rbLimitOneTime.IsEnabled = false;
                txtMaxCountPerOrder.IsEnabled = false;
                foreach (UIElement item in gridPrice.Children)
                {
                    if (item is TextBox)
                    {
                        ((TextBox)item).IsEnabled = false;
                    }
                }
            }
            else
            {
                ucItemMaster.IsEnabled = true;
                cmbGroupBuyingType.IsEnabled = true;
                chkIsByGroup.IsEnabled = true;
                dtBeginDateTime.IsEnabled = true;              
                dtEndDateTime.IsEnabled = true;
                rbNoLimit.IsEnabled = true;
                rbLimitOneTime.IsEnabled = true;
                txtMaxCountPerOrder.IsEnabled = true;
                foreach (UIElement item in gridPrice.Children)
                {
                    if (item is TextBox)
                    {
                        ((TextBox)item).IsEnabled = true;
                        txtPrice1.IsEnabled = (_viewModel.GroupBuyingTypeSysNo != 6);
                    }
                }
            }

            #region SetAuditButtonShow
            if ((_gbStatus == GroupBuyingStatus.Init && _op != "new") || _gbStatus == GroupBuyingStatus.VerifyFaild)
            {
                btnSubmitAudit.Visibility = System.Windows.Visibility.Visible;
            }
            else if (_gbStatus == GroupBuyingStatus.WaitingAudit)
            {
                //if (instance.ApproveRight == true)
                //{//TODO:权限...
                btnAuditApprove.Visibility = System.Windows.Visibility.Visible;
                btnAuditRefuse.Visibility = System.Windows.Visibility.Visible;
                txtGroupBuyingReason.IsEnabled = true;
                //}

                if (_viewModel.RequestSysNo <= 0)
                {
                    btnCancelAudit.Visibility = System.Windows.Visibility.Visible;
                }
            }
            #endregion
        }

        private void SetAllReadOnly()
        {
            foreach (UIElement item in Grid.Children)
            {
                if (item is TextBox)
                {
                    ((TextBox)item).IsReadOnly = true;
                }

                if (item is DatePicker)
                {
                    ((DatePicker)item).IsEnabled = false;
                }

                if (item is Combox)
                {
                    ((Combox)item).IsEnabled = false;
                }
                if (item is UCProductPicker)
                {
                    ((UCProductPicker)item).IsEnabled = false;
                }
            }

            foreach (UIElement item in gridPrice.Children)
            {
                if (item is TextBox)
                {
                    ((TextBox)item).IsEnabled = false;
                }
            }
        }

        private void ButtonSave_Click(object sender, RoutedEventArgs e)
        {
            //_viewModel.GroupBuyingTypeSysNo = (int)cmbGroupBuyingCategoryType.SelectedValue;
            _viewModel.GroupBuyingTypeSysNo = 0;
            if (ValidationManager.Validate(Grid) && ValidationManager.Validate(gridPrice))
            {
                if (_viewModel.BeginDate==null)
                {
                    //Window.Alert("开始日期不能为空");
                    Window.Alert(ResGroupBuyingMaintain.Info_StartDateNotNull);
                    return;
                }
                if (_viewModel.EndDate == null)
                {
                    //Window.Alert("结束日期不能为空");
                    Window.Alert(ResGroupBuyingMaintain.Info_EndDateNotNull);
                    return;
                }
                if (_viewModel.CategoryType== GroupBuyingCategoryType.Virtual && string.IsNullOrEmpty(_viewModel.CouponValidDate))
                {
                     //Window.Alert("虚拟团购有效日期不能为空");
                    Window.Alert(ResGroupBuyingMaintain.Info_ActiveDateNotNull);
                     return;
                }
                if (_viewModel.GroupBuyingTypeSysNo != 6)
                {
                    if (string.IsNullOrWhiteSpace(_viewModel.ProductID) || _viewModel.ProductSysNo == null)
                    {
                        Window.Alert(ResGroupBuyingMaintain.Msg_IsProductNull);
                        return;
                    }
                    int _sysNoTmp = -1;
                    if (!int.TryParse(_viewModel.ProductSysNo.ToString(), out _sysNoTmp))
                    {
                        Window.Alert(ResGroupBuyingMaintain.Msg_IsProductSysNoFormatError);
                        return;
                    }
                }

                if (this._viewModel.CategoryType == GroupBuyingCategoryType.Virtual)
                {
                    if (this._viewModel.GroupBuyingVendorSysNo == null)
                    {
                        //Window.Alert("请选择商家！", MessageType.Warning);
                        Window.Alert(ResGroupBuyingMaintain.Info_SelectMerchant, MessageType.Warning);
                        return;
                    }
                    if (this._viewModel.VendorStoreList.Where(prop => prop.IsChecked).Count() == 0)
                    {
                        //Window.Alert("请选择门店！", MessageType.Warning);
                        Window.Alert(ResGroupBuyingMaintain.Info_SelectShop, MessageType.Warning);
                        return;
                    }
                }

                if (_viewModel.GroupBuyingTypeSysNo != 6)
                {
                    _Facade.LoadMarginRateInfo(_viewModel, (obj0, args0) =>
                    {
                        if (args0.FaultsHandle())
                        {
                            return;
                        }

                        UCGroupBuySaveInfo saveInfoView = new UCGroupBuySaveInfo();
                        saveInfoView.MsgListVM = args0.Result;
                        saveInfoView.vm = _viewModel;
                        saveInfoView.Dialog = Window.ShowDialog(ResGroupBuyingMaintain.Msg_MakeSure, saveInfoView, (o, arg) =>
                          {
                              if (arg.DialogResult == DialogResultType.OK)
                              {
                                  SaveAction();
                              }
                          });
                    });
                }
                else
                {
                    SaveAction();
                }
            }
        }

        private void ButtonVoid_Click(object sender, RoutedEventArgs e)
        {
            Window.Confirm(ResGroupBuyingMaintain.Info_ConfirmDeActive, (s, a) =>
            {
                if (a.DialogResult == DialogResultType.OK)
                {
                    _Facade.Void(new List<int>() { Convert.ToInt32(sysNo) }, (result) =>
                    {
                        if (result)
                        {
                            // Window.Alert("作废成功!");
                            Window.Alert(ResGroupBuyingMaintain.Msg_VoidSuccess);
                            InitPage(sysNo);
                            this.Window.Refresh();
                        }

                    });
                }
            });
        }

        private void ButtonStop_Click(object sender, RoutedEventArgs e)
        {
            Window.Confirm(ResGroupBuyingMaintain.Info_ConfirmStop, (s, a) =>
           {
               if (a.DialogResult == DialogResultType.OK)
               {
                   _Facade.Stop(new List<int>() { Convert.ToInt32(sysNo) }, (result) =>
                   {
                       if (result)
                       {
                           //Window.Alert("中止处理成功，该团购将在1分钟内中止！");
                           Window.Alert(ResGroupBuyingMaintain.Msg_StopSuccess);
                           ButtonStop.IsEnabled = false;
                           InitPage(sysNo);
                           this.Window.Refresh();
                       }

                   });
               }
           });           
        }

        private void btnSubmitAudit_Click(object sender, RoutedEventArgs e)
        {
            Window.Confirm(ResGroupBuyingMaintain.Info_ConfirmSubmitAudit, (s, a) =>
            {
                if (a.DialogResult == DialogResultType.OK)
                {
                    //提交审核
                    _Facade.SubmitAudit(Convert.ToInt32(sysNo), (result) =>
                    {
                        if (result)
                        {
                            Window.Alert(ResGroupBuyingMaintain.Msg_UpdateSuccess);
                            InitPage(sysNo);
                            this.Window.Refresh();
                        }

                    });
                }
            });            
        }

        private void btnCancelAudit_Click(object sender, RoutedEventArgs e)
        {
            Window.Confirm(ResGroupBuyingMaintain.Info_ConfirmCancelAudit, (s, a) =>
          {
              if (a.DialogResult == DialogResultType.OK)
              {
                  //撤销审核
                  _Facade.CancelAudit(Convert.ToInt32(sysNo), (result) =>
                  {
                      if (result)
                      {
                          Window.Alert(ResGroupBuyingMaintain.Msg_UpdateSuccess);
                          InitPage(sysNo);
                          this.Window.Refresh();
                      }

                  });
              }
          });
        }

        private void btnAuditApprove_Click(object sender, RoutedEventArgs e)
        {
            Window.Confirm(ResGroupBuyingMaintain.Info_ConfirmAuditPass, (s, a) =>
         {
             if (a.DialogResult == DialogResultType.OK)
             {
                 //审核通过
                 _Facade.AuditApprove(Convert.ToInt32(sysNo), _viewModel.GroupBuyingReason, (result) =>
                 {
                     if (result)
                     {
                         Window.Alert(ResGroupBuyingMaintain.Msg_UpdateSuccess);
                         InitPage(sysNo);
                         this.Window.Refresh();
                     }

                 });
             }
         });
        }

        private void btnAuditRefuse_Click(object sender, RoutedEventArgs e)
        {
            Window.Confirm(ResGroupBuyingMaintain.Info_ConfirmAuditRefuse, (s, a) =>
           {
               if (a.DialogResult == DialogResultType.OK)
               {
                   //审核拒绝
                   _Facade.AuditRefuse(Convert.ToInt32(sysNo), _viewModel.GroupBuyingReason, (result) =>
                   {
                       if (result)
                       {
                           Window.Alert(ResGroupBuyingMaintain.Msg_UpdateSuccess);
                           InitPage(sysNo);
                           this.Window.Refresh();
                       }

                   });
               }
           });
        }

        private void chkIsByGroup_Click(object sender, RoutedEventArgs e)
        {
            if (this._viewModel != null && !string.IsNullOrEmpty(this._viewModel.ProductSysNo))
            {
                new GroupBuyingFacade(this).GetProductOriginalPrice(int.Parse(_viewModel.ProductSysNo), _viewModel.IsByGroup ? "Y" : "N", (obj, a) =>
                {
                    //_viewModel.OriginalPrice = (a.Result == null || a.Result[0] == null ? 0m : decimal.Round(decimal.Parse(a.Result[0].ToString()), 2));
                    if (a.Result.Count > 4)
                    {
                        _viewModel.GroupBuyingVendorSysNo = int.Parse(a.Result[3].ToString());
                        _viewModel.GroupBuyingVendorName = a.Result[4].ToString();
                    }
                });
            }
        }

        private void hyperlinkView_Click(object sender, RoutedEventArgs e)
        {
            HtmlViewHelper.ViewHtmlInBrowser("IM", "<div align=\"left\" style=\"overflow:auto;height:585px;width:790px\">"
                + this.txtGroupBuyingRules.Text + "<BR />"
                + this.txtGroupBuyingDescLong.Text + "</div>", null, new Size(800, 600), false, false);

        }

        private void cmbGroupBuyingType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox cbGroupBuyingType = sender as ComboBox;
            if (cbGroupBuyingType.SelectedValue != null)
            {
                if (((KeyValuePair<Int32, String>)cbGroupBuyingType.SelectedValue).Key == 6)
                {
                    foreach (var r in gridPrice.RowDefinitions)
                    {
                        r.Height = new GridLength(0);
                    }
                    gridPrice.RowDefinitions[1].Height = GridLength.Auto;
                    txtPrice1.Text = "0";
                    txtPrice1.IsEnabled = false;

                    txtBPoint.Visibility = Visibility.Visible;
                    txtPoint.Visibility = Visibility.Visible;
                    //商品选择
                    Grid.RowDefinitions[2].Height = new GridLength(0);
                    //txtVendorName.Text = "泰隆优选商都";
                    txtVendorName.Text = ResGroupBuyingMaintain.Info_AppName;
                }
                else
                {
                    foreach (var r in gridPrice.RowDefinitions)
                    {
                        r.Height = GridLength.Auto;
                    }
                    txtPrice1.IsEnabled = (_viewModel.GroupBuyingTypeSysNo != 6);
                    txtBPoint.Visibility = Visibility.Collapsed;
                    txtPoint.Visibility = Visibility.Collapsed;
                    //商品选择
                    Grid.RowDefinitions[2].Height = GridLength.Auto;
                }
            }
        }

        private void SaveAction()
        {
            switch (this._op)
            {
                case "mgt": break;
                case "new":
                    _Facade.Create(_viewModel, (s, args) =>
                    {
                        if (args.FaultsHandle())
                            return;
                        //Window.Alert("新建团购成功。");
                        Window.Alert(ResGroupBuyingMaintain.Msg_NewSuccess);
                        _viewModel.SysNo = args.Result.SysNo;
                        _viewModel.Status = GroupBuyingStatus.Init;
                        this._op = "edt";
                        InitPage(args.Result.ToString());
                        this.Window.Close();
                    });
                    break;
                case "edt":
                    _Facade.Update(_viewModel, (s, args) =>
                    {
                        if (args.FaultsHandle())
                            return;
                        // Window.Alert("更新团购成功。");
                        Window.Alert(ResGroupBuyingMaintain.Msg_UpdateSuccess);
                        this.Window.Refresh();
                    });
                    break;
            }
        }

        private void UCVendorPicker_VendorSelected(object sender, Basic.Components.UserControls.VendorPicker.VendorSelectedEventArgs e)
        {
            if (e.SelectedVendorInfo != null)
            {                
                LoadVendorStore();             
            }
        }

        private void LoadVendorStore()
        {
            new ExternalServiceFacade(this).QueryVendorStoreList(this._viewModel.GroupBuyingVendorSysNo, (se, a) =>
            {
                if (a.FaultsHandle())
                {
                    return;
                }
                List<VendorStoreVM> list = DynamicConverter<VendorStoreVM>.ConvertToVMList(a.Result.Rows);
                _viewModel.VendorStoreList.Clear();
                list.ForEach(p =>
                {
                    _viewModel.VendorStoreList.Add(p);
                });

                foreach (var item in _viewModel.VendorStoreList)
                {
                    if (_viewModel.VendorStoreSysNoList != null)
                    {
                        var re = _viewModel.VendorStoreSysNoList.FirstOrDefault(p => p == item.SysNo.Value);
                        if (re > 0)
                        {
                            item.IsChecked = true;
                        }
                    }
                }
            });
        }

        private void cmbGroupBuyingCategoryType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this._viewModel.CategoryType == GroupBuyingCategoryType.Virtual && this._viewModel.GroupBuyingVendorSysNo > 0)
            {
                LoadVendorStore();
            }

            if (this._viewModel.CategoryType.HasValue)
            {
                LoadGroupBuyingCategory(this._viewModel.CategoryType.Value);
            }
        }

        private void LoadGroupBuyingCategory(GroupBuyingCategoryType type)
        {
            new GroupBuyingFacade(this).GetAllGroupBuyingCategory((se, a) =>
            {
                if (a.FaultsHandle())
                {
                    return;
                }
                _viewModel.GroupBuyingCategoryList.Clear();
                if (a.Result == null || a.Result.Count < 1)
                {
                    return;
                }
                var list = a.Result.Where(p => p.CategoryType == type).ToList();
                list.ForEach(p =>
                {
                    var v = EntityConverter<GroupBuyingCategoryInfo,GroupBuyingCategoryVM>.Convert(p);
                    _viewModel.GroupBuyingCategoryList.Add(v);
                });

                if (this._viewModel.SysNo.HasValue && this._viewModel.SysNo > 0)
                {
                    int? categorySysNo = this._viewModel.GroupBuyingCategorySysNo;
                    this._viewModel.GroupBuyingCategorySysNo = 0;
                    this._viewModel.GroupBuyingCategorySysNo = categorySysNo;
                }
                else
                {
                    if (list.Count > 0)
                    {
                        this._viewModel.GroupBuyingCategorySysNo = list.FirstOrDefault().SysNo;
                    }
                }
                if (this._viewModel.GroupBuyingCategorySysNo == null)
                {
                    this.cmbGroupBuyingCategory.SelectedIndex = 0;
                }
            });
        }
    }
}
