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
using ECCentral.BizEntity.Customer;
using ECCentral.Portal.UI.Customer.Resources;
using Newegg.Oversea.Silverlight.Utilities.Validation;
using ECCentral.Portal.Basic.Components.UserControls.CustomerPicker;
using ECCentral.Portal.UI.Customer.UserControls;
using Newegg.Oversea.Silverlight.Controls.Components;
using ECCentral.Portal.Basic;
using ECCentral.Portal.Basic.Utilities;

namespace ECCentral.Portal.UI.Customer.Views
{
    [View(IsSingleton = true, NeedAccess = false, SingletonType = SingletonTypes.Url)]
    public partial class CustomerPointLogQuery : PageBase
    {
        #region 属性

        CustomerPointLogQueryVM model;

        #endregion

        #region 初始化加载
        public CustomerPointLogQuery()
        {
            InitializeComponent();

            model = new CustomerPointLogQueryVM();
        }

        public override void OnPageLoad(object sender, EventArgs e)
        {
            int customerSysNo = 0;
            if (int.TryParse(this.Request.Param, out customerSysNo))
            {
                ucCustomerPicker.SetCustomerSysNo(customerSysNo);
                model.CustomerSysNo = customerSysNo.ToString();
                BindDataGrid();
            }
            base.OnPageLoad(sender, e);
            
            CodeNamePairHelper.GetList(ConstValue.DomainName_Customer, "AdjustPointType", CodeNamePairAppendItemType.All,
             (obj, args) =>
             {
                 if (!args.FaultsHandle() && args.Result != null)
                 {
                     model.PointLogTypes = args.Result;
                     this.cbPointType.ItemsSource = model.PointLogTypes;
                     //BindContext();
                 }
             });
            BindContext();
        }

        private void BindContext()
        {
            this.DataContext = model;
        }
        #endregion

        #region 查询绑定

        private void Query(Newegg.Oversea.Silverlight.Controls.Data.DataGrid dataGrid, Newegg.Oversea.Silverlight.Controls.Data.LoadingDataEventArgs e, int? ResultType)
        {
            CustomerPointLogQueryFacade facade = new CustomerPointLogQueryFacade(this);
            model.ResultType = ResultType;
            facade.Query(model, e.PageSize, e.PageIndex, e.SortField, (obj, args) =>
            {
                dataGrid.ItemsSource = args.Result.Rows;
                dataGrid.TotalCount = args.Result.TotalCount;
            });
        }

        private void dataGridPointAddLog_LoadingDataSource(object sender, Newegg.Oversea.Silverlight.Controls.Data.LoadingDataEventArgs e)
        {
            Query(dataGridPointAddLog, e, 1);
        }

        private void dataGridPointUseLog_LoadingDataSource(object sender, Newegg.Oversea.Silverlight.Controls.Data.LoadingDataEventArgs e)
        {
            if (model.IsCashPoint.HasValue && model.IsCashPoint.Value == YNStatus.Y)
            {
                dataGridPointUseLog.ItemsSource = new List<int>();

                dataGridPointUseLog.TotalCount = 0;
            }
            else
            {
                Query(dataGridPointUseLog, e, -1);
            }
        }

        private void Button_Search_Click(object sender, RoutedEventArgs e)
        {
            BindDataGrid();
        }

        private void BindDataGrid()
        {
            ValidationManager.Validate(this.SeachBuilder);
            if (!model.HasValidationErrors)
            {
                this.dataGridPointAddLog.Bind();
                this.dataGridPointUseLog.Bind();
            }
        }

        #endregion

        //更改积分有效期
        private void btnUpdateValidPoint_Click(object sender, RoutedEventArgs e)
        {
            dynamic obtainLog = this.dataGridPointAddLog.SelectedItem as dynamic;
            if (obtainLog != null)
            {
                CustomerPointExpiringDate DialogPage = new CustomerPointExpiringDate();
                DialogPage.ViewModel = DynamicConverter<CustomerPointExpiringDateVM>.ConvertToVM(obtainLog);
                DialogPage.DialogHandle = this.Window.ShowDialog(ECCentral.Portal.UI.Customer.Resources.ResCustomerQuery.PopTitle_UpdateValidPoint, DialogPage, (obj, args) =>
                {
                    if (args.DialogResult == DialogResultType.OK)
                    {
                        this.dataGridPointAddLog.Bind();
                    }
                });
            }
            else
            {
                Window.Alert(ResCustomerQuery.Msg_OnSelectCostomer, MessageType.Error);
            }

        }

    }

}
