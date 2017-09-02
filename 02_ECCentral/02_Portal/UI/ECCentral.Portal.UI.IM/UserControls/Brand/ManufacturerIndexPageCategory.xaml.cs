using System.Windows;
using System.Windows.Controls;
using Newegg.Oversea.Silverlight.Controls.Components;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using ECCentral.Portal.UI.IM.Facades;
using ECCentral.BizEntity.IM;
using ECCentral.Portal.UI.IM.Resources;

namespace ECCentral.Portal.UI.IM.UserControls
{
    public partial class ManufacturerIndexPageCategory : UserControl
    {
        public IDialog Dialog { get; set; }

        /// <summary>
        /// 生产商编号
        /// </summary>
        public int ManufacturerSysNo { get; set; }
        public ManufacturerIndexPageCategory()
        {
            InitializeComponent();
            dgManufacturerIndexPageCategory.Bind();
        }

        private void dgManufacturerIndexPageCategory_LoadingDataSource(object sender, Newegg.Oversea.Silverlight.Controls.Data.LoadingDataEventArgs e)
        {
            var facade = new ManufacturerQueryFacade();
            if (ManufacturerSysNo != 0)
            {
                facade.QueryManufacturerCategory(ManufacturerSysNo.ToString(), e.PageSize, e.PageIndex, e.SortField, (obj, args) =>
                {
                    dgManufacturerIndexPageCategory.ItemsSource = args.Result.Rows;
                    dgManufacturerIndexPageCategory.TotalCount = args.Result.TotalCount;
                });
            }
        }

        private void HyperlinkDelete_Click(object sender, RoutedEventArgs e)
        {
            CPApplication.Current.CurrentPage.Context.Window.Confirm(ResManufacturerIndexPageCategory.Msg_IsDeleteCategory, (o, a) =>
            {
                if (a.DialogResult == DialogResultType.OK)
                {
                    var brandShipCategory = this.dgManufacturerIndexPageCategory.SelectedItem as dynamic;
                    int sysNo = brandShipCategory.SysNo;
                    var facade = new ManufacturerFacade();
                    if (brandShipCategory != null)
                    {
                        facade.DeleteBrandShipCategory(sysNo, (obj, args) => dgManufacturerIndexPageCategory.Bind());
                    }
                }
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

        private void BtnCanel_Click(object sender, RoutedEventArgs e)
        {
            CloseDialog(DialogResultType.Cancel);
        }

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(txtCategoryID.Text))
            {
                CPApplication.Current.CurrentPage.Context.Window.Alert(ResManufacturerIndexPageCategory.Msg_CategoryIDIsEmpty, MessageType.Warning);
                return;
            }
            int categodyID;
            if (!int.TryParse(txtCategoryID.Text, out categodyID))
            {
                CPApplication.Current.CurrentPage.Context.Window.Alert(ResManufacturerIndexPageCategory.Msg_CategoryIDIsInvalid, MessageType.Warning);
                return;
            }
            var brandShipCategory = new BrandShipCategory();
            int categoryID;
            int.TryParse(txtCategoryID.Text, out categoryID);
            brandShipCategory.BrandShipCategoryID = categoryID;
            brandShipCategory.ManufacturerSysNo = ManufacturerSysNo;
            if (brandShipCategory.BrandShipCategoryID != 0 && ManufacturerSysNo != 0)
            {
                var facade = new ManufacturerFacade();
                facade.CreateBrandShipCategory(brandShipCategory, (obj, args) =>
                {
                    if (args.FaultsHandle())
                    {
                        return;
                    }
                    CPApplication.Current.CurrentPage.Context.Window.Alert(ResManufacturerIndexPageCategory.Msg_AddSuccessfully);
                    dgManufacturerIndexPageCategory.Bind();
                });
            }
        }
    }
}
