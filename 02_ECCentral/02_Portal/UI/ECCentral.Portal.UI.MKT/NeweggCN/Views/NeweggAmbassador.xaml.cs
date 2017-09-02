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
using Newegg.Oversea.Silverlight.Controls;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using ECCentral.Portal.UI.MKT.Models;
using ECCentral.QueryFilter.Common;
using ECCentral.Portal.UI.MKT.NeweggCN.Facades;
using ECCentral.Portal.Basic;
using Newegg.Oversea.Silverlight.Controls.Components;
using ECCentral.Portal.UI.MKT.NeweggCN.Resources;
using ECCentral.Portal.UI.MKT.NeweggCN.UserControls;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using ECCentral.BizEntity.Enum;
using Newegg.Oversea.Silverlight.Utilities.Validation;

namespace ECCentral.Portal.UI.MKT.Views
{
    [View(IsSingleton = true, SingletonType = SingletonTypes.Url)]
    public partial class NeweggAmbassador : PageBase
    {
      

        public NeweggAmbassador()
        {
            InitializeComponent();
            
        }

        public override void OnPageLoad(object sender, EventArgs e)
        {
            base.OnPageLoad(sender, e);

        }

       
    }
}
