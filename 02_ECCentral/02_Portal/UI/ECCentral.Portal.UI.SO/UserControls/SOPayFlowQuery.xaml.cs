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
using ECCentral.Portal.UI.SO.Models;
using ECCentral.Portal.UI.SO.Facades;
using Newegg.Oversea.Silverlight.Controls;
using Newegg.Oversea.Silverlight.Controls.Components;

namespace ECCentral.Portal.UI.SO.UserControls
{
    public partial class SOPayFlowQuery : UserControl
    {
        public int SoSysNo { get; set; }

        private SOPayFlowQueryVM soPayVM;

        public IDialog Dialog
        {
            get;
            set;
        }

        public IPage Page
        {
            get;
            set;
        }

        SOFacade soFacade;

        public SOPayFlowQuery(IPage page, int soSysNo)
        {
            this.Page = page;
            this.SoSysNo = soSysNo;
            InitializeComponent();
            Loaded += new RoutedEventHandler(HoldSO_Loaded);
        }

        void HoldSO_Loaded(object sender, RoutedEventArgs e)
        {
            soFacade = new SOFacade(Page);
            soFacade.QueryPayFlow(SoSysNo, vm =>
            {
                gridInfo.DataContext = vm;
            });
        }
    }
}
