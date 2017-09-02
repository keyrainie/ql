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
using ECCentral.Portal.UI.Common.Facades;
using ECCentral.Portal.UI.Common.Models;
using Newegg.Oversea.Silverlight.Controls;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using Newegg.Oversea.Silverlight.Controls.Data;
using System.Collections;
using Newegg.Oversea.Silverlight.Utilities.Validation;
using ECCentral.Portal.Basic.Utilities;

namespace ECCentral.Portal.UI.Common.Views
{
    [View(IsSingleton = true, NeedAccess = false, SingletonType = SingletonTypes.Url)]
    public partial class QueryTariffInfo : PageBase
    {
        private TariffFacade m_facade;
        private TariffInfoQueryFilterVM VM;
        public QueryTariffInfo()
        {
            InitializeComponent();
            Loaded += QueryTariffInfo_Loaded;
        }

        public IPage Page
        {
            get
            {
                return CPApplication.Current.CurrentPage;
            }
        }

        void QueryTariffInfo_Loaded(object sender, RoutedEventArgs e)
        {
            VM = new TariffInfoQueryFilterVM();
            m_facade = new TariffFacade(this);
            this.DataContext = VM;
            Loaded -= QueryTariffInfo_Loaded;
        }

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidationManager.Validate(this.LayoutRoot))
                return;
            this.dgQueryResult.Bind();
        }

        private void Create_Click(object sender, RoutedEventArgs e)
        {
            Window.Navigate("/ECCentral.Portal.UI.Common/TariffMaintanin", null, true);
        }

        private void dgQueryResult_LoadingDataSource(object sender, LoadingDataEventArgs e)
        {
            List<TariffInfoVM> _vmList = new List<TariffInfoVM>();
            m_facade.TariffInfoQuery(VM, e.PageSize, e.PageIndex, e.SortField, (obj, args) =>
            {
                _vmList = DynamicConverter<TariffInfoVM>.ConvertToVMList<List<TariffInfoVM>>(args.Result.Rows);
                this.dgQueryResult.ItemsSource = _vmList;
                this.dgQueryResult.TotalCount = args.Result.TotalCount;
            });
        }

        private void HyperlinkSysNo_Click(object sender, RoutedEventArgs e)
        {
            IList SelectedItemList = dgQueryResult.SelectedItems;
            TariffInfoVM vm = SelectedItemList[0] as TariffInfoVM;
            Window.Navigate(string.Format("/ECCentral.Portal.UI.Common/TariffMaintanin/{0}", VM.SysNo), null, false);
        }

    }
}
