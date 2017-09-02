using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using ECCentral.BizEntity.IM;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.Portal.UI.IM.Facades;
using ECCentral.Portal.UI.IM.Models;
using ECCentral.Portal.UI.IM.Resources;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using Newegg.Oversea.Silverlight.Controls.Components;
using Newegg.Oversea.Silverlight.Utilities.Validation;
using System.Windows.Media;

namespace ECCentral.Portal.UI.IM.UserControls
{
    public partial class ProductSellerPortalMaintainDetail : UserControl
    {

        #region 属性

        public IDialog Dialog { get; set; }

        public int? SysNo { get; set; }

        private SellerProductRequestFacade _facade;

        private int _sysNo;
        
        #endregion

        #region 初始化加载

        public ProductSellerPortalMaintainDetail()
        {
            InitializeComponent();
        }
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            dpCategory.LoadCategoryCompleted += BindSource;
        }

        private void BindSource(object sender, EventArgs e)
        {
            BindPage();
        }
        #endregion

        #region 查询绑定
        private void BindPage()
        {
            if (SysNo != null)
            {
                _facade = new SellerProductRequestFacade();
                _facade.GetSellerProductRequestBySysNo(SysNo.Value, (obj, args) =>
                {
                    if (args.FaultsHandle())
                    {
                        return;
                    }
                    if (args.Result == null)
                    {
                        CPApplication.Current.CurrentPage.Context.Window.MessageBox.Show("没有获得商家产品信息.", MessageBoxType.Warning);
                        return;
                    }
                    var vm = args.Result.Convert<SellerProductRequestInfo, SellerProductRequestVM>();

                    vm.CategoryInfo = args.Result.CategoryInfo.Convert<CategoryInfo, CategoryVM>
                           ((v, t) =>
                           {
                               t.CategoryName = v.CategoryName.Content;
                           });

                    vm.Brand = args.Result.Brand.Convert<BrandInfo, BrandVM>
                            ((v, t) =>
                            {
                                t.BrandNameLocal = v.BrandNameLocal.Content;
                            });

                    vm.Manufacturer = args.Result.Manufacturer.Convert<ManufacturerInfo, ManufacturerVM>
                            ((v, t) =>
                            {
                                t.ManufacturerNameLocal = v.ManufacturerNameLocal.Content;
                            });

                    _sysNo = SysNo.Value;

                    DataContext = vm;

                    this.dgPropertyQueryResult.ItemsSource = vm.SellerProductRequestPropertyList;
                    this.dgPropertyQueryResult.TotalCount = vm.SellerProductRequestPropertyList.Count;



                    foreach (SellerProductRequestFileVM item in vm.SellerProductRequestFileList)
                    {
                        Border border = new Border();
                        border.BorderThickness = new Thickness(1);
                        SolidColorBrush brush = new SolidColorBrush();
                        brush.Color = Colors.LightGray;
                        border.BorderBrush = brush;

                        Image image = new Image();
                        image.Width = 150;
                        image.Height = 130;
                        image.Margin = new Thickness(2, 2, 2, 2);
                        border.Child = image;
                        image.Source = new System.Windows.Media.Imaging.BitmapImage(item.AbsolutePathOnServer);
                        this.ImageListPanel.Children.Add(border);
                    }

                });
            }
            else
            {
                _sysNo = 0;
                var item = new SellerProductRequestVM();
                DataContext = item;
            }
        }


        #endregion

        #region 按钮事件

        private void btnDeny_Click(object sender, RoutedEventArgs e)
        {
            var vm = DataContext as SellerProductRequestVM;
            if (vm == null)
            {
                return;
            }

            if (string.IsNullOrEmpty(vm.Memo))
            {
                CPApplication.Current.CurrentPage.Context.Window.MessageBox.Show("必须输入退回理由.", MessageBoxType.Warning);
                return;
            }

            _facade = new SellerProductRequestFacade();
            vm.SysNo = _sysNo;
            if (vm.SysNo == null || vm.SysNo.Value <= 0)
            {
                return;
            }
            else
            {
                _facade.DenyProductRequest(vm, (obj, args) =>
                {
                    if (args.FaultsHandle())
                    {
                        return;
                    }
                    CPApplication.Current.CurrentPage.Context.Window.Alert(ResBrandMaintain.Info_SaveSuccessfully);

                    BindPage();
                });
            }
        }

        private void btnCreateID_Click(object sender, RoutedEventArgs e)
        {
            var vm = DataContext as SellerProductRequestVM;
            if (vm == null)
            {
                return;
            }

            if (!ValidationManager.Validate(this))
            {
                return;
            }

            if (vm.CategoryInfo == null || vm.CategoryInfo.SysNo == null || string.IsNullOrEmpty(vm.CategoryInfo.CategoryName))
            {
                CPApplication.Current.CurrentPage.Context.Window.MessageBox.Show("类别信息不正确.", MessageBoxType.Warning);
                return;
            }

            if (vm.Brand == null || vm.Brand.SysNo == null)
            {
                CPApplication.Current.CurrentPage.Context.Window.MessageBox.Show("品牌信息不正确.", MessageBoxType.Warning);
                return;
            }

            if (vm.PMUser == null || vm.PMUser.SysNo == null || vm.PMUser.SysNo == 0)
            {
                CPApplication.Current.CurrentPage.Context.Window.MessageBox.Show("PM信息不正确.", MessageBoxType.Warning);
                return;
            }

            _facade = new SellerProductRequestFacade();
            vm.SysNo = _sysNo;
            if (vm.SysNo == null || vm.SysNo.Value <= 0)
            {
                return;
            }
            else
            {
                _facade.CreateItemIDForNewProductRequest(vm, (obj, args) =>
                {
                    if (args.FaultsHandle())
                    {
                        return;
                    }
                    CPApplication.Current.CurrentPage.Context.Window.Alert(ResBrandMaintain.Info_SaveSuccessfully);
                   
                    CloseDialog(DialogResultType.OK);
                });
            }
        }

        private void btnAudit_Click(object sender, RoutedEventArgs e)
        {
            var vm = DataContext as SellerProductRequestVM;
            if (vm == null)
            {
                return;
            }

            _facade = new SellerProductRequestFacade();
            vm.SysNo = _sysNo;
            if (vm.SysNo == null || vm.SysNo.Value <= 0)
            {
                return;
            }
            else
            {
                _facade.ApproveProductRequest(vm, (obj, args) =>
                {
                    if (args.FaultsHandle())
                    {
                        return;
                    }
                    CPApplication.Current.CurrentPage.Context.Window.Alert(ResBrandMaintain.Info_SaveSuccessfully);

                    CloseDialog(DialogResultType.OK);
                });
            }
        }


        private void CloseDialog(DialogResultType dialogResult)
        {
            if (Dialog != null)
            {
                Dialog.ResultArgs.DialogResult = dialogResult;
                Dialog.Close();
            }
        }

        private void hyperlinkView_Click(object sender, RoutedEventArgs e)
        {
            var vm = DataContext as SellerProductRequestVM;

            if (vm == null)
            {
                return;
            }

            HtmlViewHelper.ViewHtmlInBrowser("IM", "<div align=\"left\" style=\"overflow:auto;height:585px;width:790px\">" + vm.ProductDescLong ?? string.Empty + "</div>", null, new Size(800, 600), false, false);

        }

        #endregion

    }
    public class IMPMDisplayConverter : System.Windows.Data.IValueConverter
    {

        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            string val = value as string;
            string result = string.Empty;
            if ("Color".Equals(parameter)) 
            {
                if (string.IsNullOrEmpty(val))
                {
                    result = "Red";
                }
                else 
                {
                    result = "black";
                }
            }
            else if ("Text".Equals(parameter)) 
            {
                if (string.IsNullOrEmpty(val))
                {
                    result = "请PMCC添加产品线归属的PM";
                }
                else
                {
                    result = value.ToString();
                }
            }
            return result;
        }
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return null;
        }
        #endregion
    }
}
