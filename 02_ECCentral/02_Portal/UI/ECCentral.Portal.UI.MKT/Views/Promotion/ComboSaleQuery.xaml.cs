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

using ECCentral.Portal.Basic;
using ECCentral.Portal.UI.MKT.Models;
using ECCentral.Portal.UI.MKT.Facades;
using ECCentral.BizEntity.MKT;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.Portal.UI.MKT.Resources;
using Newegg.Oversea.Silverlight.Utilities.Validation;

namespace ECCentral.Portal.UI.MKT.Views
{
    [View(IsSingleton = true, SingletonType = SingletonTypes.Url)]
    public partial class ComboSaleQuery : PageBase
    {
        public ComboQueryReqVM FilterVM
        {
            get
            {
                return this.expSearchCondition.DataContext as ComboQueryReqVM;
            }
            set
            {
                this.expSearchCondition.DataContext = value;
            }
        }        

        public ComboSaleQuery()
        {
            InitializeComponent();
        }

        public override void OnPageLoad(object sender, EventArgs e)
        {
            base.OnPageLoad(sender, e);          

            this.FilterVM = new ComboQueryReqVM();
        }

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidationManager.Validate(this))
                return;
            chkHidden.IsChecked = false;
            this.dataComboList.QueryCriteria = Newegg.Oversea.Silverlight.Utilities.UtilityHelper.DeepClone<ComboQueryReqVM>(this.FilterVM);
            this.dataComboList.Bind();
        }

        private void DataGrid_LoadingDataSource(object sender, Newegg.Oversea.Silverlight.Controls.Data.LoadingDataEventArgs e)
        {
            new ComboFacade(this).Query(this.dataComboList.QueryCriteria as ComboQueryReqVM, e.PageSize, e.PageIndex, e.SortField, (obj, args) =>
            {                     
                this.dataComboList.ItemsSource = args.Result.Rows.ToList();
                this.dataComboList.TotalCount = args.Result.TotalCount;
            });
        }              

        private void btnNew_Click(object sender, RoutedEventArgs e)
        {
            this.Window.Navigate(string.Format(ConstValue.MKT_ComboSaleMaintainUrlFormat, ""), null, true);
        }

        private void btnBatchDelete_Click(object sender, RoutedEventArgs e)
        {
            dynamic rows = this.dataComboList.ItemsSource;
            List<int> list = new List<int>();
            foreach (dynamic row in rows)
            {
                if (row.IsChecked == true && row.Status == ComboStatus.Active)
                {
                    list.Add(int.Parse(row["SysNo"].ToString()));
                }
            }
            if (list.Count == 0)
            {
                Window.Alert(ResComboSaleQuery.Warning_NoItemsSelected, Newegg.Oversea.Silverlight.Controls.Components.MessageType.Warning);
                return;
            }

            Window.Confirm(ResComboSaleQuery.Confirm_Delete, (obj, args) =>
            {
                if (args.DialogResult == Newegg.Oversea.Silverlight.Controls.Components.DialogResultType.OK)
                {                    
                    new ComboFacade(this).BatchDelete(list, (o, a) =>
                    {
                        foreach (var row in rows)
                        {
                            if (list.Contains(row.SysNo))
                            {
                                row.Status = ComboStatus.Deactive;
                            }
                        }

                        Window.Alert(ResComboSaleQuery.Info_Successfully);
                    });
                }
            });
        }

        private void btnBatchSet_Click(object sender, RoutedEventArgs e)
        {
            Window.Navigate("/ECCentral.Portal.UI.MKT/ComboSaleBatchMaintain", null, true);
        }

        private void btnEdit_Click(object sender, RoutedEventArgs e)
        {
            dynamic vm = (sender as HyperlinkButton).DataContext;
            this.Window.Navigate(string.Format(ConstValue.MKT_ComboSaleMaintainUrlFormat, vm.SysNo), null, true);
        }        

        private void ckbSelectAllRow_Click(object sender, RoutedEventArgs e)
        {
            CheckBox chk = (CheckBox)sender;
            dynamic rows = this.dataComboList.ItemsSource;
            if (rows != null)
            {
                foreach (dynamic row in rows)
                {
                    ComboStatus status = ComboStatus.Active;
                    Enum.TryParse<ComboStatus>(row.Status.ToString(), out status);

                    if (status == ComboStatus.Active)
                    {
                        row.IsChecked = chk.IsChecked.Value ? true : false;
                    }
                }
            }      
        }
    }
}
