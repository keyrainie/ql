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
using ECCentral.BizEntity.IM;
using Newegg.Oversea.Silverlight.Controls.Components;
using ECCentral.Portal.UI.IM.Models;
using ECCentral.Portal.UI.IM.Facades;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using Newegg.Oversea.Silverlight.Utilities.Validation;
using Newegg.Oversea.Silverlight.Controls;
using ECCentral.Portal.Basic;

namespace ECCentral.Portal.UI.IM.UserControls
{
    public partial class BrandRequestMaintain : UserControl
    {
        private BrandRequestFacade facade;
        public IDialog Dialog { get; set; }
        /// <summary>
        /// 审核的数据
        /// </summary>
        public BrandRequestVM Data { private get; set; }
        /// <summary>
        /// 不同的操作
        /// </summary>
        public BrandAction Action { private get; set; }

        /// <summary>
        /// 编辑时需要品牌SysNo
        /// </summary>
        public int BrandSysNo { private get; set; }

        public BrandRequestMaintain()
        {
            InitializeComponent();
            this.Loaded += (sender, e) =>
            {
                facade = new BrandRequestFacade();

                switch (Action)
                {
                    case BrandAction.Edit://编辑

                        //根据SysNo得到数据并bing
                        facade.GetBrandBySysNo(BrandSysNo, (obj, arg) =>
                        {
                            //将所有控件IsEnabled设为true
                            foreach (var item in myGrid.Children)
                            {
                                if (item.GetType() == typeof(StackPanel))
                                {
                                    StackPanel sp = (StackPanel)item;

                                    foreach (var s in sp.Children)
                                    {
                                        if (s.GetType() == typeof(TextBox))
                                        {
                                            ((TextBox)s).IsEnabled = true;
                                        }
                                        if (s.GetType() == typeof(Combox))
                                        {
                                            ((Combox)s).IsEnabled = true;
                                        }
                                        if (s.GetType() == typeof(CheckBox))
                                        {
                                            ((CheckBox)s).IsEnabled = true;
                                        }
                                        if (s.GetType() == typeof(Button))
                                        {
                                            ((Button)s).IsEnabled = true;
                                        }
                                    }
                                }
                            }
                            this.linkPreview.IsEnabled = true;
                            this.BtnSave.Visibility = Visibility.Visible;
                          //  this.myBrandAuthorized.Visibility = Visibility.Visible;
                            this.DataContext = new BrandRequestVM()
                            {
                                BrandName_Ch = arg.Result.BrandNameLocal.Content,
                                BrandName_En = arg.Result.BrandNameGlobal,
                                BrandStatus = arg.Result.Status,
                                Info = arg.Result.BrandDescription.Content,
                                ManufacturerSysNo = arg.Result.Manufacturer.SysNo.ToString(),
                                SupportEmail = arg.Result.BrandSupportInfo.ServiceEmail,
                                SupportUrl = arg.Result.BrandSupportInfo.ServiceUrl,
                                ManufacturerWebsite = arg.Result.BrandSupportInfo.ManufacturerUrl,
                                AdImage = arg.Result.Manufacturer.BrandImage,
                                IsDisPlayZone = arg.Result.Manufacturer.IsShowZone == "Y",
                                IsLogo = arg.Result.IsLogo == "Y",
                                BrandStoreType = arg.Result.BrandStoreType,
                                CustomerServicePhone = arg.Result.BrandSupportInfo.ServicePhone,
                                BrandStory = arg.Result.BrandStory,
                                ShowStoreUrl = arg.Result.Manufacturer.ShowUrl,
                                ProductId = arg.Result.ProductId,
                                BrandCode = arg.Result.BrandCode
                            };
                        });
                        myBrandAuthorizedMaintain.BrandSysNo = BrandSysNo;
                        break;
                    case BrandAction.Add: //添加
                        this.BtnAudit.Visibility = Visibility.Visible;
                        this.myAudit.Visibility = Visibility.Visible;
                        this.txtNameCh.IsEnabled = true;
                        this.txtProductLine.IsEnabled = true;
                        this.txtReason.IsEnabled = true;
                        this.ManufacturerPicker.IsEnabled = true;
                        this.txtNameEn.IsEnabled = true;
                        txtBrandStory.IsEnabled = false;
                        txtBrandCode.IsEnabled = true;
                        facade.GetBrandCode((obj, arg) =>
                        {
                            this.DataContext = new BrandRequestVM() { BrandStatus = ValidStatus.DeActive, BrandCode = arg.Result };//默认无效
                        });
                        break;
                    case BrandAction.Audit: //审核
                        this.linkPreview.IsEnabled = true;
                        txtBrandCode.IsEnabled = false;
                        this.myAudit.Visibility = Visibility.Visible;
                        this.BtnAuditByNo.Visibility = Visibility.Visible;
                        this.BtnAuditByOk.Visibility = Visibility.Visible;
                        this.DataContext = Data;
                        break;
                    default:
                        break;
                }


            };
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            BrandRequestVM model = this.DataContext as BrandRequestVM;
            model.RequestStatus = "A";
            facade.AuditManufacturerRequest(model, (obj, arg) =>
            {
                if (arg.FaultsHandle())
                {
                    return;
                }
                CloseDialog(DialogResultType.OK);
                CPApplication.Current.CurrentPage.Context.Window.Alert("提交成功");
            });
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            CloseDialog(DialogResultType.Cancel);
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            BrandRequestVM model = this.DataContext as BrandRequestVM;
            model.RequestStatus = "D";
            facade.AuditManufacturerRequest(model, (obj, arg) =>
            {
                if (arg.FaultsHandle())
                {
                    return;
                }
                CloseDialog(DialogResultType.OK);
                CPApplication.Current.CurrentPage.Context.Window.Alert("提交成功");
            });
        }
        private void CloseDialog(DialogResultType dialogResult)
        {
            if (Dialog != null)
            {
                Dialog.ResultArgs.DialogResult = dialogResult;
                Dialog.Close();
            }
        }

        private void BtnAudit_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidationManager.Validate(this))
            {
                return;
            }
            BrandRequestVM model = this.DataContext as BrandRequestVM;
            if (string.IsNullOrEmpty(model.BrandName_Ch) && string.IsNullOrEmpty(model.BrandName_En))
            {
                CPApplication.Current.CurrentPage.Context.Window.Alert("品牌的中文名和英文名必填一个!", MessageType.Error);
                return;
            }
            model.RequestStatus = "O";
            facade.InsertBrandRequest(model, (obj, arg) =>
            {
                if (arg.FaultsHandle())
                {
                    return;
                }
                CloseDialog(DialogResultType.OK);
                CPApplication.Current.CurrentPage.Context.Window.Alert("提交成功");
            });
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidationManager.Validate(this.expander1))
            {
                return;
            }
            BrandRequestVM model = this.DataContext as BrandRequestVM;
            if (!CheckModelByUpdate())
            {
                return;
            }
            model.SysNo = BrandSysNo;
            facade.UpdateBrand(model, (obj, arg) =>
            {
                if (arg.FaultsHandle())
                {
                    return;
                }
                CPApplication.Current.CurrentPage.Context.Window.Alert("提交成功");
            });
        }

        private void HyperlinkButton_Click(object sender, RoutedEventArgs e)
        {
            //HyperlinkButton link = sender as HyperlinkButton;

            //Ocean.20130514, Move to ControlPanelConfiguration
            //string urlFormat = CPApplication.Current.CurrentPage.Context.Window.Configuration.GetConfigValue(ConstValue.DomainName_ExternalSYS, ConstValue.ConfigKey_External_BrandProductPreviewUrl);
            //CPApplication.Current.CurrentPage.Context.Window.Navigate(string.Format(urlFormat, link.Tag));
            CPApplication.Current.CurrentPage.Context.Window.Navigate(txtImageUrl.Text);
        }
        /// <summary>
        /// 更新时check
        /// </summary>
        /// <returns></returns>
        private bool CheckModelByUpdate()
        {
            BrandRequestVM model = this.DataContext as BrandRequestVM;
            if (string.IsNullOrEmpty(model.BrandName_Ch) && string.IsNullOrEmpty(model.BrandName_En))
            {
                CPApplication.Current.CurrentPage.Context.Window.Alert("品牌的中文名和英文名必填一个!", MessageType.Error);
                return false;
            }
            if (model.BrandStoreType == BrandStoreType.FlagshipStore)
            {
                if (string.IsNullOrEmpty(model.AdImage))
                {
                    CPApplication.Current.CurrentPage.Context.Window.Alert("品牌类型为旗舰店，品牌店广告图不能为空!");
                    return false;
                }
                if (!model.IsLogo)
                {
                    CPApplication.Current.CurrentPage.Context.Window.Alert("品牌类型为旗舰店，是否有Logo必须选中!");
                    return false;
                }

            }
            return true;
        }

        private void BtnUploadBrandLogo_Click(object sender, RoutedEventArgs e)
        {
            CPApplication.Current.CurrentPage.Context.Window.Navigate("/ECCentral.Portal.UI.MKT/UnifiedImageMaintain", true);
        }
    }

    public enum BrandAction
    {
        //编辑
        Edit = 0,
        //添加
        Add,
        //审核
        Audit
    }
}
