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

using Newegg.Oversea.Silverlight.Controls;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using Newegg.Oversea.Silverlight.Controls.Components;
using ECCentral.QueryFilter.Customer;
using ECCentral.Portal.UI.Customer.Facades;
using ECCentral.QueryFilter.Common;
using ECCentral.BizEntity.RMA;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.Portal.UI.Customer.Resources;
using Newegg.Oversea.Silverlight.ControlPanel.Core;

namespace ECCentral.Portal.UI.Customer.Views.Calling
{
    [View]
    public partial class RMAAdd : PageBase
    {
        public IDialog Dialog;
        public RMARegisterQueryFilter filter;
        public CustomerCallingFacade facade;
        public RMAAdd()
        {
            filter = new RMARegisterQueryFilter();

            InitializeComponent();
        }

        private void DataGrid1_LoadingDataSource(object sender, Newegg.Oversea.Silverlight.Controls.Data.LoadingDataEventArgs e)
        {
            filter.PagingInfo = new PagingInfo()
            {
                PageSize = e.PageSize,
                PageIndex = e.PageIndex,
                SortBy = e.SortField
            };
            if (!string.IsNullOrEmpty(productPicker1.ProductSysNo))
                filter.ProductSysNo = int.Parse(productPicker1.ProductSysNo);
            if (!string.IsNullOrEmpty(tbRequestSysNo.Text))
                filter.RequestSysNo = int.Parse(tbRequestSysNo.Text);
            filter.CompanyCode = CPApplication.Current.CompanyCode;
            new CustomerCallingFacade(this).GetRMARegisterList(filter, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }
                DataGrid1.ItemsSource = args.Result.Rows;
                DataGrid1.TotalCount = args.Result.TotalCount;
            });
        }

        private void ButtonSearch_Click(object sender, RoutedEventArgs e)
        {
            DataGrid1.Bind();
        }

        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            tbRegisterSysNo.Text = (DataGrid1.SelectedItem as dynamic).RegisterSysNo.ToString();
        }

        private void ButtonSave_Click(object sender, RoutedEventArgs e)
        {
            List<ValidationEntity> veForRegisterSysNo = new List<ValidationEntity>();
            veForRegisterSysNo.Add(new ValidationEntity(ValidationEnum.IsNotEmpty, null, ResRMAAdd.msg_ValidateRegister));
            List<ValidationEntity> veForContent = new List<ValidationEntity>();
            veForContent.Add(new ValidationEntity(ValidationEnum.IsNotEmpty, null, ResRMAAdd.msg_ValidateContent));
            if (!ValidationHelper.Validation(this.tbRegisterSysNo, veForRegisterSysNo) || !ValidationHelper.Validation(this.tbContent, veForContent))
                return;
            InternalMemoInfo request = new InternalMemoInfo();
            request.SysNo = int.Parse(Request.Param);
            request.RegisterSysNo = int.Parse(tbRegisterSysNo.Text);
            request.Note = tbContent.Text;
            new CustomerCallingFacade(this).CallingToRMA(request, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }
                Window.Alert(ResRMAAdd.msg_Success);
            });
        }
    }

}
