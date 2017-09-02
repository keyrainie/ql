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
using ECCentral.Portal.UI.Customer.Models;
using ECCentral.Portal.UI.Customer.Facades;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.Portal.UI.Customer.Resources;
using Newegg.Oversea.Silverlight.Utilities.Validation;
using ECCentral.Portal.Basic.Components.UserControls.CustomerPicker;

namespace ECCentral.Portal.UI.Customer.Views
{
    [View(IsSingleton = true, NeedAccess = false, SingletonType = SingletonTypes.Url)]
    public partial class CustomerExperienceLogQuery : PageBase
    {
        #region 属性

        CustomerExperienceLogQueryVM model;

        #endregion

        #region 初始化加载

        public CustomerExperienceLogQuery()
        {
            model = new CustomerExperienceLogQueryVM();
            this.DataContext = model;
            InitializeComponent();
        }

        public override void OnPageLoad(object sender, EventArgs e)
        {
            int customerSysNo = 0;
            if (int.TryParse(this.Request.Param, out customerSysNo))
            {
                model.CustomerSysNo = customerSysNo.ToString();
                ucCustomerPicker.SetCustomerSysNo(customerSysNo);
                ucCustomerPicker.Focus();
                BindDataGrid();
            }
            base.OnPageLoad(sender, e);
        }

        #endregion

        #region 查询绑定数据

        private void Button_Search_Click(object sender, RoutedEventArgs e)
        {
            BindDataGrid();
        }

        private void BindDataGrid()
        {
            ValidationManager.Validate(this.SeachBuilder);
            if (!model.HasValidationErrors)
            {
                dataGrid1.Bind();
            }
        }

        private void dataGrid1_LoadingDataSource(object sender, Newegg.Oversea.Silverlight.Controls.Data.LoadingDataEventArgs e)
        {
            CustomerExperienceLogFacade facade = new CustomerExperienceLogFacade(this);
            facade.Query(model, e.PageSize, e.PageIndex, e.SortField, new EventHandler<RestClientEventArgs<dynamic>>(BindDataCallBack));
        }

        public void BindDataCallBack(object sender, RestClientEventArgs<dynamic> args)
        {
            if (args.FaultsHandle())
            {
                return;
            }
            dataGrid1.ItemsSource = args.Result.Rows;
            dataGrid1.TotalCount = args.Result.TotalCount;
        }

        #endregion
    }

}
