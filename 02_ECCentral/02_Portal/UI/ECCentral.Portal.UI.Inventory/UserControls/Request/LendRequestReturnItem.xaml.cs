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
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;


#region ECCentral Libs
using ECCentral.BizEntity.Inventory;
using ECCentral.Portal.UI.Inventory.Facades;
using ECCentral.Portal.UI.Inventory.Models;
using ECCentral.Portal.UI.Inventory.Resources;

#endregion ECCentral Libs

#region Newegg.Oversea.Oversea Libs

using Newegg.Oversea.Silverlight.ControlPanel.Core;
using Newegg.Oversea.Silverlight.Controls;
using Newegg.Oversea.Silverlight.Controls.Components;
using Newegg.Oversea.Silverlight.Utilities;
using Newegg.Oversea.Silverlight.Utilities.Validation;

#endregion Newegg.Oversea.Oversea Libs

namespace ECCentral.Portal.UI.Inventory.UserControls
{
    public partial class LendRequestReturnItem : UserControl
    {
        public IDialog Dialog
        {
            get;
            set;
        }

        private IWindow CurrentWindow
        {
            get
            {
                return CPApplication.Current.CurrentPage.Context.Window;
            }
        }

        private List<LendRequestReturnItemInfo> returnItemList;
        public List<LendRequestReturnItemInfo> ReturnItemList
        {
            get
            {
                return returnItemList;
            }
            set
            {
                returnItemList = value;
                this.dgReturnItemList.ItemsSource = value;
            }
        }   
   
         #region 初始化

        public LendRequestReturnItem()
        {
            InitializeComponent();
        }

        #endregion
    }
}
