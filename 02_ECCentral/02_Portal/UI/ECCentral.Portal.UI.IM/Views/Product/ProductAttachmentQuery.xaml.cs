using System;
using System.Collections.Generic;
using System.Windows;
using ECCentral.Portal.Basic;
using ECCentral.Portal.Basic.Components.UserControls.ProductPicker;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.Portal.UI.IM.Models;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using Newegg.Oversea.Silverlight.Controls;
using Newegg.Oversea.Silverlight.Controls.Data;
using ECCentral.Portal.UI.IM.Facades;
using Newegg.Oversea.Silverlight.Controls.Components;
using ECCentral.Portal.UI.IM.Resources;
using ECCentral.BizEntity.IM;

namespace ECCentral.Portal.UI.IM.Views
{
    [View(IsSingleton = true, SingletonType = SingletonTypes.Url)]
    public partial class ProductAttachmentQuery : PageBase
    {

        ProductAttachmentQueryVM model;
        private List<ProductAttachmentVM> _vmList;

        public ProductAttachmentQuery()
        {
            InitializeComponent();
            //this.ucProductPicker.ProductSelected += ProductSelected_Click;
        }

        public override void OnPageLoad(object sender, EventArgs e)
        {
            base.OnPageLoad(sender, e);
            model = new ProductAttachmentQueryVM();
            this.DataContext = model;            
        }

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            model.ProductID = this.ucProductPicker.ProductID;
            model.AttachmentID = this.ucAttachmentPicker.ProductID;
            dgProductAttachmentQueryResult.Bind();
        }

        private void dgProductAttachmentQueryResult_LoadingDataSource(object sender, LoadingDataEventArgs e)
        {
            ProductAttachmentQueryFacade facade = new ProductAttachmentQueryFacade(this);

            facade.QueryProductAttachmentList(model, e.PageSize, e.PageIndex, e.SortField, (obj, args) =>
            {
                _vmList = DynamicConverter<ProductAttachmentVM>.ConvertToVMList<List<ProductAttachmentVM>>(args.Result.Rows);
                this.dgProductAttachmentQueryResult.ItemsSource = _vmList;
                this.dgProductAttachmentQueryResult.TotalCount = args.Result.TotalCount;
            });
        }

        private void btnNew_Click(object sender, RoutedEventArgs e)
        {
            Window.Navigate(ConstValue.IM_ProductAttachmentMaintainCreateFormat, null, true);
        }

        private void hyperlinkOperationEdit_Click(object sender, RoutedEventArgs e)
        {
            dynamic view = this.dgProductAttachmentQueryResult.SelectedItem as dynamic;
            if (view == null)
            {
                Window.Alert(ResProductAttachmentQuery.Msg_OnSelectProductAttachment, MessageType.Error);
                return;
            }
            else if (view.ProductStatus != ProductStatus.InActive_UnShow)
            {
                Window.Alert(ResProductAttachmentQuery.Msg_OnEditProductAttachment, MessageType.Error);
                return;
            }
            else
            {
                this.Window.Navigate(string.Format(ConstValue.IM_ProductAttachmentMaintainUrlFormat, view.ProductSysNo), null, true);
            }
        }

        private void hyperlinkOperationDelete_Click(object sender, RoutedEventArgs e)
        {
            dynamic view = this.dgProductAttachmentQueryResult.SelectedItem as dynamic;
            if (view == null)
            {
                Window.Alert(ResProductAttachmentQuery.Msg_OnSelectProductAttachment, MessageType.Error);
                return;
            }
            if (view.ProductStatus == ProductStatus.Active)
            {
                Window.Alert(ResProductAttachmentQuery.Msg_OnDeleteProductAttachment, MessageType.Error);
                return;
            }
            int productSysNo = view.ProductSysNo;
            Window.Confirm(ResProductAttachmentQuery.Confirm_Delete, (obj, args) =>
            {
                if (args.DialogResult == DialogResultType.OK)
                {
                    ProductAttachmentFacade facade = new ProductAttachmentFacade(this);
                    facade.DeleteAttachmentByProductSysNo(productSysNo, (o, a) =>
                    {
                        if (a.FaultsHandle())
                        {
                            return;
                        }
                        Window.Alert(ResProductAttachmentQuery.Info_Successfully);
                        dgProductAttachmentQueryResult.Bind();
                    });
                }
            });
        }


    }
}
