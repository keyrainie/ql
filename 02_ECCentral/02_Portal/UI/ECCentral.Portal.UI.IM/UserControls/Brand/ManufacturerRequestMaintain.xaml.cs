using System.Windows;
using System.Windows.Controls;
using ECCentral.BizEntity.IM;
using Newegg.Oversea.Silverlight.Controls.Components;
using ECCentral.Portal.UI.IM.Models;
using ECCentral.Portal.UI.IM.Facades;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using Newegg.Oversea.Silverlight.Utilities.Validation;
using Newegg.Oversea.Silverlight.Controls;
using ECCentral.Portal.Basic;
using ECCentral.Portal.UI.IM.Resources;

namespace ECCentral.Portal.UI.IM.UserControls
{
    public partial class ManufacturerRequestMaintain : UserControl
    {
        private ManufacturerRequestFacade facade;
        /// <summary>
        /// 审核数据
        /// </summary>
        public ManufacturerRequestVM Data { get; set; }
        public IDialog Dialog { get; set; }
        /// <summary>
        /// 操作类型
        /// </summary>
        public ManufacturerAction Action { get; set; }

        /// <summary>
        /// 编辑生产商的SysNo
        /// </summary>
        public int ManufacturerSysNo { get; set; }
        public ManufacturerRequestMaintain()
        {
            InitializeComponent();
            this.Loaded += (sender, e) =>
            {
                facade = new ManufacturerRequestFacade();
                switch (Action)
                {
                    case ManufacturerAction.Edit:

                        facade.GetManufacturerBySysNo(ManufacturerSysNo, (obj, args) =>
                        {
                            if (args.FaultsHandle())
                            {
                                return;
                            }
                            if (args.Result == null)
                            {
                                return;
                            }
                            ManufacturerRequestVM vm = new ManufacturerRequestVM()
                            {
                                ManufacturerName = args.Result.ManufacturerNameLocal.Content,
                                ManufacturerBriefName = args.Result.ManufacturerNameGlobal,
                                ManufacturerStatus = args.Result.Status,
                                SysNo = (int)args.Result.SysNo,
                                AfterSalesSupportEmail = args.Result.SupportInfo.ServiceEmail,
                                AfterSalesSupportLink = args.Result.SupportInfo.ServiceUrl,
                                ClientPhone = args.Result.SupportInfo.ServicePhone,
                                Info = args.Result.ManufacturerDescription.Content,
                                MannfacturerLink = args.Result.SupportInfo.ManufacturerUrl,
                                IsLogo = args.Result.IsLogo,
                                IsShowZone = args.Result.IsShowZone == "Y" ? true : false,
                                BrandStoreType = args.Result.BrandStoreType,
                                BrandImage = args.Result.BrandImage,
                                ShowUrl = args.Result.ShowUrl
                            };
                            this.DataContext = vm;
                            //将所有控件IsEnabled改为true
                            foreach (var stack in mygrid.Children)
                            {
                                if (stack.GetType() == typeof(StackPanel))
                                {
                                    foreach (var item in ((StackPanel)stack).Children)
                                    {
                                        if (item.GetType() == typeof(TextBox))
                                        {
                                            ((TextBox)item).IsEnabled = true;
                                        }
                                        if (item.GetType() == typeof(Combox))
                                        {
                                            ((Combox)item).IsEnabled = true;
                                        }
                                        if (item.GetType() == typeof(CheckBox))
                                        {
                                            ((CheckBox)item).IsEnabled = true;
                                        }
                                    }
                                }
                            }
                            this.txtProductLine.IsEnabled = true;
                            this.txtReanson.IsEnabled = true;
                            this.BtnSave.Visibility = Visibility.Visible;
                            this.BtnMaintainBrand.Visibility = Visibility.Visible;
                            this.myAuditInfo.Visibility = Visibility.Visible;

                            if (args.Result.BrandStoreType == BrandStoreType.FlagshipStore)
                            {
                                HyperlinkSetIndexPageCatetory.Visibility = System.Windows.Visibility.Visible;
                            }
                        });
                        break;
                    case ManufacturerAction.Add:
                        this.DataContext = new ManufacturerRequestVM();
                        this.txtNameCh.IsEnabled = true;
                        this.txtNameEn.IsEnabled = true;
                        this.txtProductLine.IsEnabled = true;
                        this.txtReanson.IsEnabled = true;
                        this.cboStatus.IsEnabled = true;
                        this.BtnAudit.Visibility = Visibility.Visible;
                        myAuditInfo.Visibility = Visibility.Visible;
                        break;
                    case ManufacturerAction.Audit:
                        this.DataContext = Data;
                        this.BtnAuditByNo.Visibility = Visibility.Visible;
                        this.BtnAuditByOk.Visibility = Visibility.Visible;
                        this.BtnAuditClose.Visibility = Visibility.Visible;
                        myAuditInfo.Visibility = Visibility.Visible;
                        break;
                    default:
                        break;
                }



            };
        }



        private void Button_Click(object sender, RoutedEventArgs e)
        {
            CloseDialog(DialogResultType.Cancel);
        }
        private void CloseDialog(DialogResultType dialogResult)
        {
            if (Dialog != null)
            {
                Dialog.ResultArgs.DialogResult = dialogResult;
                Dialog.Close();
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            ManufacturerRequestVM model = this.DataContext as ManufacturerRequestVM;
            model.Status = 1;
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

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            ManufacturerRequestVM model = this.DataContext as ManufacturerRequestVM;
            model.Status = -1;
            facade.AuditManufacturerRequest(model, (obj, arg) =>
            {
                if (arg.FaultsHandle())
                {
                    return;
                }
                CPApplication.Current.CurrentPage.Context.Window.Alert("提交成功");
                CloseDialog(DialogResultType.OK);
            });
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            ManufacturerRequestVM model = this.DataContext as ManufacturerRequestVM;
            model.Status = -2;
            facade.AuditManufacturerRequest(model, (obj, arg) =>
            {
                if (arg.FaultsHandle())
                {
                    return;
                }
                CPApplication.Current.CurrentPage.Context.Window.Alert("提交成功");
                CloseDialog(DialogResultType.OK);
            });
        }

        private void BtnAudit_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidationManager.Validate(this))
            {
                return;

            }
            ManufacturerRequestVM model = this.DataContext as ManufacturerRequestVM;
            if (string.IsNullOrEmpty(model.ManufacturerBriefName) && string.IsNullOrEmpty(model.ManufacturerName))
            {
                CPApplication.Current.CurrentPage.Context.Window.Alert("生产商中文名称和英文名称必填一个!");
                return;
            }
            model.OperationType = 1;
            facade.InsertManufacturerRequest(model, (obj, arg) =>
            {
                if (arg.FaultsHandle())
                {
                    return;
                }
                CPApplication.Current.CurrentPage.Context.Window.Alert("提交成功");
                CloseDialog(DialogResultType.OK);
            });
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidationManager.Validate(this.expander1))
            {
                return;

            }
            ManufacturerRequestVM model = this.DataContext as ManufacturerRequestVM;
            if (string.IsNullOrEmpty(model.ManufacturerBriefName) && string.IsNullOrEmpty(model.ManufacturerName))
            {
                CPApplication.Current.CurrentPage.Context.Window.Alert("生产商中文名称和英文名称必填一个!");
                return;
            }
            if (model.BrandStoreType == BrandStoreType.FlagshipStore && string.IsNullOrEmpty(model.BrandImage))
            {
                CPApplication.Current.CurrentPage.Context.Window.Alert("店铺类型为旗舰店的时,品牌店广告图不能为空");
                return;
            }

            CPApplication.Current.CurrentPage.Context.Window.Confirm("是否更新该生产商下所有品牌信息", (obj, arg) =>
            {
                if (arg.DialogResult == Newegg.Oversea.Silverlight.Controls.Components.DialogResultType.OK)
                {
                    facade.UpdateBrandMasterByManufacturerSysNo(model, (objs, args) =>
                    {
                        if (args.FaultsHandle())
                        {
                            return;
                        }

                    });
                }
                facade.UpdateManufacturer(model, (o, a) =>
                   {
                       if (a.FaultsHandle())
                       {
                           return;
                       }
                       CPApplication.Current.CurrentPage.Context.Window.Alert("提交成功");
                       CloseDialog(DialogResultType.OK);
                   });


            });




        }

        private void BtnMaintainBrand_Click(object sender, RoutedEventArgs e)
        {
            CPApplication.Current.CurrentPage.Context.Window.Navigate(string.Format(ConstValue.IM_BrandQueryUrlFormat, ManufacturerSysNo),null,true);
            CloseDialog(DialogResultType.OK);
        }

        private void HyperlinkSetIndexPageCatetory_Click(object sender, RoutedEventArgs e)
        {
            var item = new ManufacturerIndexPageCategory();
            item.ManufacturerSysNo = ManufacturerSysNo;
            item.Dialog = CPApplication.Current.CurrentPage.Context.Window.ShowDialog(ResManufacturerIndexPageCategory.SetBrandShipCategoryTitle, item, null, new Size(750, 550));
        }
    }
    public enum ManufacturerAction
    {
        //编辑
        Edit = 0,
        //添加
        Add,
        //审核
        Audit
    }
}
