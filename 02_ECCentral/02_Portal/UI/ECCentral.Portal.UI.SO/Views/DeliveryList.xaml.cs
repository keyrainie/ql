using System;
using System.Windows;
using ECCentral.Portal.Basic;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.Portal.UI.SO.Facades;
using ECCentral.Portal.UI.SO.Models;
using ECCentral.Portal.UI.SO.Resources;

using Newegg.Oversea.Silverlight.ControlPanel.Core;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using Newegg.Oversea.Silverlight.Controls;
using Newegg.Oversea.Silverlight.Controls.Components;

namespace ECCentral.Portal.UI.SO.Views
{
    [View]
    public partial class DeliveryList : PageBase
    {
        SODeliveryAssignTaskSearchVM exportSODeliveryAssignTaskSearchVM = null;
        SODeliveryAssignTaskSearchVM queryVM = null;
        OtherDomainQueryFacade otherFacade;
        SOQueryFacade soQueryFacade;
        
        public DeliveryList()
        {
            InitializeComponent();
        }

        public override void OnPageLoad(object sender, EventArgs e)
        {
            base.OnPageLoad(sender, e);
            IniPageData();
        }

        private void IniPageData()
        {
            soQueryFacade = new SOQueryFacade(this);
            queryVM = new SODeliveryAssignTaskSearchVM();
            exportSODeliveryAssignTaskSearchVM = new SODeliveryAssignTaskSearchVM();

            otherFacade = new OtherDomainQueryFacade(this);

            otherFacade.GetFreightManList(true, freightManList =>
            {
                queryVM.FreightMenList = freightManList;
            });

            gridConditions.DataContext = queryVM;

            if (cmbDeliveryTimeRangeFrom.ItemsSource != null)
            {
                cmbDeliveryTimeRangeFrom.SelectedIndex = 0;
            }

            if (cmbDeliveryTimeRangeTo.ItemsSource != null)
            {
                cmbDeliveryTimeRangeTo.SelectedIndex = 1;
            }

            if (cmbOrderType.ItemsSource != null)
            {
                cmbOrderType.SelectedIndex = 0;
            }
        }

        private void dataGridDelivery_LoadingDataSource(object sender, Newegg.Oversea.Silverlight.Controls.Data.LoadingDataEventArgs e)
        {
            queryVM.CompanyCode = CPApplication.Current.CompanyCode;
            queryVM.PageInfo = new QueryFilter.Common.PagingInfo
            {
                PageIndex = e.PageIndex,
                PageSize = e.PageSize,
                SortBy = e.SortField
            };

            exportSODeliveryAssignTaskSearchVM = queryVM.DeepCopy();
            QueryDelivery();
        }

        private void QueryDelivery()
        {
            soQueryFacade.SODeliveryAssignTaskQuery(queryVM, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }

                dataGridDelivery.TotalCount = args.Result.TotalCount;
                dataGridDelivery.ItemsSource = args.Result.Rows;
            });
        }

        private void dataGridDelivery_ExportAllClick(object sender, EventArgs e)
        {
            if (!AuthMgr.HasFunctionPoint(AuthKeyConst.SO_ExcelExport))
            {
                Window.Alert(ResSO.Msg_Error_Right,MessageType.Error);
                return;
            }

            if (exportSODeliveryAssignTaskSearchVM != null && exportSODeliveryAssignTaskSearchVM.PageInfo != null)
            {
                ColumnSet col = new ColumnSet(dataGridDelivery);
                exportSODeliveryAssignTaskSearchVM.PageInfo.PageSize = dataGridDelivery.TotalCount;
                soQueryFacade.ExportSODeliveryAssignTask(exportSODeliveryAssignTaskSearchVM, new ColumnSet[] { col });
            }
        }

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            dataGridDelivery.Bind();
        }
    }

}
