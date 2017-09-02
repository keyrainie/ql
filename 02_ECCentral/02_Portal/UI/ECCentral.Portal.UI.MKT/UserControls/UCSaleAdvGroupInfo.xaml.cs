using System.Windows;
using System.Windows.Controls;

using ECCentral.Portal.UI.MKT.Facades;
using ECCentral.Portal.UI.MKT.Models;
using ECCentral.Portal.UI.MKT.Views;

using Newegg.Oversea.Silverlight.Controls.Components;
using Newegg.Oversea.Silverlight.Utilities.Validation;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using System;
using ECCentral.Portal.Basic.Utilities;

namespace ECCentral.Portal.UI.MKT.UserControls
{
    public partial class UCSaleAdvGroupInfo : UserControl
    {
        public SaleAdvTemplateItemMaintain Page { get; set; }
        public IDialog Dialog { get; set; }

        public UCSaleAdvGroupInfo()
        {
            InitializeComponent();

            Loaded += new RoutedEventHandler(UCSaleAdvGroupInfo_Loaded);
        }

        void UCSaleAdvGroupInfo_Loaded(object sender, RoutedEventArgs e)
        {
            chkAllGroup.Visibility = ((this.DataContext as SaleAdvGroupVM).SysNo ?? 0) > 0 ? Visibility.Collapsed : Visibility.Visible;
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            var vm = this.DataContext as SaleAdvGroupVM;

            if (ValidationManager.Validate(this.LayoutRoot))
            {

                if (vm.ShowStartDate > vm.ShowEndDate)
                {
                    CPApplication.Current.CurrentPage.Context.Window.Alert("开始时间不能大于结束时间!", MessageType.Warning);
                    return;
                }

                if (vm.SysNo > 0)
                {
                    new SaleAdvTemplateFacade(this.Page).UpdateSaleAdvGroup(vm, (obj, args) =>
                    {
                        this.Dialog.ResultArgs = new ResultEventArgs { DialogResult = DialogResultType.OK, Data = args.Result };
                        this.Dialog.Close();
                    });
                }
                else
                {
                    new SaleAdvTemplateFacade(this.Page).CreateSaleAdvGroup(vm, (obj, args) =>
                    {
                        this.Dialog.ResultArgs = new ResultEventArgs { DialogResult = DialogResultType.OK, Data = args.Result };
                        this.Dialog.Close();
                    });
                }
            }
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Dialog.ResultArgs = new ResultEventArgs { DialogResult = DialogResultType.Cancel };
            this.Dialog.Close();
        }

        private void ComboxRecommendType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sender == null) return;
            ComboBox cb = sender as ComboBox;

            if (cb.SelectedItem != null)
            {
                ECCentral.BizEntity.MKT.RecommendType? recType = ((System.Collections.Generic.KeyValuePair<ECCentral.BizEntity.MKT.RecommendType?, string>)(cb.SelectedItem)).Key;
                this.imageSaleAdv.Source = new System.Windows.Media.Imaging.BitmapImage(SaleAdvPathOnServer(recType));

                if (recType == null || (int)recType <= 3)
                {
                    GridGroupInfo.RowDefinitions[10].Height = new GridLength(0);
                    GridGroupInfo.RowDefinitions[11].Height = new GridLength(0);
                }
                else
                {
                    GridGroupInfo.RowDefinitions[10].Height = GridLength.Auto;
                    GridGroupInfo.RowDefinitions[11].Height = GridLength.Auto;
                }
            }
        }

        private Uri SaleAdvPathOnServer(ECCentral.BizEntity.MKT.RecommendType? recommendType)
        {
            string url = string.Empty;
            url = "../WebResources/Images/SaleAdvTemplate/SaleAdvRecommend" + (recommendType == null ? 0 : (int)recommendType) + ".png";

            Uri imageServicePath = Application.Current.Host.Source;
            Uri outPath;

            if (Uri.TryCreate(imageServicePath, url, out outPath))
            {
                return outPath;
            }
            else
            {
                return null;
            }
        }
    }
}
