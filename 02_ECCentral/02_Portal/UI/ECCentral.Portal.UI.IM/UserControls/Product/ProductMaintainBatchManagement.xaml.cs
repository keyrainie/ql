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
using Newegg.Oversea.Silverlight.Controls.Data;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;

using ECCentral.Portal.Basic;
using ECCentral.Portal.UI.IM.Resources;
using ECCentral.Portal.UI.IM.Models;
using ECCentral.Portal.UI.IM.Facades;
using ECCentral.Portal.Basic.Interface;
using ECCentral.Portal.UI.IM.Models.Product.ProductMaintain;
using Newegg.Oversea.Silverlight.Utilities.Validation;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using ECCentral.Portal.UI.IM.Views;
using System.Text;

namespace ECCentral.Portal.UI.IM.UserControls
{
    public partial class ProductMaintainBatchManagement : UserControl, ISave
    {
        public ProductMaintainBatchManagementInfoVM VM
        {
            get { return DataContext as ProductMaintainBatchManagementInfoVM; }
        }

        public ProductMaintain Page
        {
            get
            {
                return CPApplication.Current.CurrentPage as ProductMaintain;
            }
        }

        public ProductMaintainBatchManagement()
        {
            InitializeComponent();
        }

        public void Save()
        {
            this.VM.ProductSysNo = (this.Page.DataContext as ProductVM).ProductSysNo;            
            new ProductExtFacade(this.Page).UpdateBatchManagementInfo(this.VM, (o, e) =>
            {
                if (e.FaultsHandle())
                {
                    return;
                }
                if (e.Result != null)
                {
                    this.VM.Logs = e.Result.Logs;
                    this.VM.FirePropertyChanged("HistoryNote");
                }
                              
                CPApplication.Current.CurrentPage.Context.Window.Alert("保存成功！");
            });
        }     
    }
}
