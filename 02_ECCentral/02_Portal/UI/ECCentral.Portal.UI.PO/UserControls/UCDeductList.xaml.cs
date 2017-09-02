using ECCentral.Portal.UI.PO.Models;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using Newegg.Oversea.Silverlight.Controls;
using Newegg.Oversea.Silverlight.Controls.Components;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using ECCentral.QueryFilter.PO;
using ECCentral.BizEntity.PO;
using ECCentral.Portal.Basic.Utilities;
using Newegg.Oversea.Silverlight.Controls.Data;

namespace ECCentral.Portal.UI.PO.UserControls
{
    public partial class UCDeductList : UserControl
    {
        public IDialog Dialog { get; set; }

        public DeductFacade serviceFacade;
        public List<DeductVM> ItemList;

        public IWindow CurrentWindow
        {
            get
            {
                return CPApplication.Current.CurrentPage.Context.Window;
            }
        }

        public UCDeductList()
        {
            InitializeComponent();
            this.Loaded += new RoutedEventHandler(UCDeductList_Loaded);
        }

        void dgDeductInfo_LoadingDataSource(object sender, LoadingDataEventArgs e)
        {
            DeductQueryFilter filter = new DeductQueryFilter() { DeductType = DeductType.Temp, Status = Status.Effective };
            serviceFacade.QueryDeducts(filter, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    ItemList = new List<DeductVM>();
                    return;
                }
                ItemList = DynamicConverter<DeductVM>.ConvertToVMList(args.Result.Rows);
                this.dgDeductInfo.ItemsSource = ItemList;
            });           
        }
        void UCDeductList_Loaded(object sender, RoutedEventArgs e)
        {
            this.Loaded -= UCDeductList_Loaded;
            serviceFacade = new DeductFacade(CPApplication.Current.CurrentPage);
            this.dgDeductInfo.Bind();
        }
       
        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
           
                var selectedDeduct = ItemList.Where(p => p.IsCheckedItem == true).Select(p => new ConsignAdjustItemVM
                {
                    DeductSysNo = p.SysNo,
                    AccountType = p.AccountType,
                    DeductMethod = p.DeductMethod,
                    DeductName = p.Name
                }).ToList();
                Dialog.ResultArgs.DialogResult = DialogResultType.OK;
                Dialog.ResultArgs.Data = selectedDeduct;
                Dialog.Close(true);           
           
        }
        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            //取消操作:
            Dialog.ResultArgs.Data = null;
            Dialog.ResultArgs.DialogResult = DialogResultType.Cancel;
            Dialog.Close(true);
        }

    }
}
   
   
