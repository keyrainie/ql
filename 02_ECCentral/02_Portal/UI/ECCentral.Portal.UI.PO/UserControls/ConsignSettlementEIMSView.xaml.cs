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
using Newegg.Oversea.Silverlight.Controls.Components;
using Newegg.Oversea.Silverlight.Controls;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using ECCentral.Portal.UI.PO.Models;
using ECCentral.Portal.UI.PO.Facades;
using ECCentral.Service.PO.Restful.RequestMsg;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.BizEntity.PO;
using ECCentral.Portal.UI.PO.Resources;

namespace ECCentral.Portal.UI.PO.UserControls
{
    public partial class ConsignSettlementEIMSView : UserControl
    {
        public ConsignSettlementInfoVM QueryVM { get; set; }

        public IDialog Dialog { get; set; }

        public ConsignSettlementFacade serviceFacade;

        public IWindow CurrentWindow
        {
            get
            {
                return CPApplication.Current.CurrentPage.Context.Window;
            }
        }

        public IPage CurrentPage
        {
            get
            {
                return CPApplication.Current.CurrentPage;
            }
        }

        public ConsignSettlementEIMSView(ConsignSettlementInfoVM vm)
        {
            InitializeComponent();
            this.QueryVM = vm;
            this.Loaded += new RoutedEventHandler(ConsignSettlementEIMSView_Loaded);
        }

        void ConsignSettlementEIMSView_Loaded(object sender, RoutedEventArgs e)
        {
            this.Loaded -= ConsignSettlementEIMSView_Loaded;
            serviceFacade = new ConsignSettlementFacade(CurrentPage);
            EIMSQueryResultGrid.Bind();
        }

        private void EIMSQueryResultGrid_LoadingDataSource(object sender, Newegg.Oversea.Silverlight.Controls.Data.LoadingDataEventArgs e)
        {
            ConsignSettlementEIMSQueryRsq request = new ConsignSettlementEIMSQueryRsq()
            {
                PageInfo = new QueryFilter.Common.PagingInfo()
                {
                    PageIndex = e.PageIndex,
                    PageSize = e.PageSize,
                    SortBy = e.SortField
                },
                queryCondition = EntityConverter<ConsignSettlementInfoVM, ConsignSettlementInfo>.Convert(QueryVM, (s, t) =>
                {
                    t.PMInfo = new BizEntity.IM.ProductManagerInfo()
                    {
                        SysNo = Convert.ToInt32(s.PMSysNo)
                    };
                })
            };
            serviceFacade.QueryConsignSettlementEIMSList(request, (obj, args) =>
            {

                if (args.FaultsHandle())
                {
                    return;
                }
                this.EIMSQueryResultGrid.TotalCount = args.Result.TotalCount;
                this.EIMSQueryResultGrid.ItemsSource = args.Result.ResultList;
            });
        }

        private void btnChooseReturnPoint_Click(object sender, RoutedEventArgs e)
        {
            ConsignSettlementEIMSInfo getSelectedItem = this.EIMSQueryResultGrid.SelectedItem as ConsignSettlementEIMSInfo;
            if (null == getSelectedItem)
            {
                CurrentWindow.Alert(ResConsignMaintain.InfoMsg_ChooseOneReturnPoint);
                return;
            }
            Dialog.ResultArgs.Data = getSelectedItem;
            Dialog.ResultArgs.DialogResult = DialogResultType.OK;
            Dialog.Close(true);
        }
    }
}
