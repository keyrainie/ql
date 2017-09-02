using System;
using System.Windows;
using ECCentral.Portal.Basic;
using ECCentral.Portal.UI.IM.Facades;
using ECCentral.Portal.UI.IM.Models;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using Newegg.Oversea.Silverlight.Controls;
using Newegg.Oversea.Silverlight.Controls.Data;
using ECCentral.Portal.UI.IM.UserControls;
using System.Windows.Controls;
using System.Collections.Generic;
using Newegg.Oversea.Silverlight.Controls.Components;

namespace ECCentral.Portal.UI.IM.Views.Brand
{
    [View(IsSingleton = true, SingletonType = SingletonTypes.Url)]
    public partial class ManufacturerRequest :PageBase
    {
        
        public ManufacturerRequest()
        {
            InitializeComponent();
            this.Loaded += new RoutedEventHandler(ManufacturerRequest_Loaded);
            this.ManufacturerRequestResult.LoadingDataSource += new EventHandler<LoadingDataEventArgs>(ManufacturerRequestResult_LoadingDataSource);
        }

        void ManufacturerRequestResult_LoadingDataSource(object sender, LoadingDataEventArgs e)
        {
            ManufacturerRequestFacade facade = new ManufacturerRequestFacade();
            facade.GetAllManufacturerRequest(e.PageSize, e.PageIndex, e.SortField, (obj, arg) => 
            {
                if (arg.FaultsHandle())
                {
                    return;
                }
                this.ManufacturerRequestResult.ItemsSource = arg.Result.Rows;
                this.ManufacturerRequestResult.TotalCount = arg.Result.TotalCount;
            });
        }

        void ManufacturerRequest_Loaded(object sender, RoutedEventArgs e)
        {
            this.ManufacturerRequestResult.Bind();
        }
       
     

        private void HyperlinkButton_Click(object sender, RoutedEventArgs e)
        {
            dynamic d = this.ManufacturerRequestResult.SelectedItem as dynamic;
            ManufacturerRequestVM data = new ManufacturerRequestVM() 
            {
                SysNo = d.SysNo,
                Status = d.Status,
                Reasons = d.Reasons,
                ManufacturerSysNo = d.ManufacturerSysNo,
                ManufacturerStatus = d.ManufacturerStatus,
                ManufacturerBriefName = d.ManufacturerBriefName,
                ManufacturerName = d.ManufacturerName,
                OperationType = d.OperationType,
                ProductLine = d.ProductLine,
            };

              ManufacturerRequestMaintain item = new ManufacturerRequestMaintain();
              item.Action = ManufacturerAction.Audit;
              item.Data = data;
            item.Dialog = Window.ShowDialog("生产商审核", item, (s, args) =>
            {
                if (args.DialogResult == Newegg.Oversea.Silverlight.Controls.Components.DialogResultType.OK)
                {
                    this.ManufacturerRequestResult.Bind();
                }
            }, new Size(600, 500));
        }

    }
}
