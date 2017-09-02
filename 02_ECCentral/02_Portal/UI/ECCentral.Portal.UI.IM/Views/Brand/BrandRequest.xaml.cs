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
using ECCentral.BizEntity.IM;

namespace ECCentral.Portal.UI.IM.Views
{
    [View(IsSingleton = true, SingletonType = SingletonTypes.Url)]
    public partial class BrandRequest : PageBase
    {
        private BrandRequestFacade facade;
        public BrandRequest()
        {
            InitializeComponent();
            this.Loaded += (sender, e) => { this.BrandRequestResult.Bind(); };
            this.BrandRequestResult.LoadingDataSource += new EventHandler<LoadingDataEventArgs>(BrandRequestResult_LoadingDataSource);
        }

        public override void OnPageLoad(object sender, EventArgs e)
        {
            base.OnPageLoad(sender, e);
            facade = new BrandRequestFacade();
         }

        void BrandRequestResult_LoadingDataSource(object sender, LoadingDataEventArgs e)
        {
            facade.GetAllBrandRequest(e.PageSize, e.PageIndex, e.SortField, (obj, arg) => 
            {
                this.BrandRequestResult.ItemsSource = arg.Result.Rows;
                this.BrandRequestResult.TotalCount = arg.Result.TotalCount;

            });
        }

        private void HyperlinkButton_Click(object sender, RoutedEventArgs e)
        {
            dynamic d = this.BrandRequestResult.SelectedItem as dynamic;
            BrandRequestMaintain item = new BrandRequestMaintain();
            item.Data = new BrandRequestVM() 
            {
                BrandName_Ch = d.BrandName_Ch,
                BrandName_En = d.BrandName_En,
                ProductLine = d.ProductLine,
                Reasons = d.Reason,
                Info = d.Note,
                SupportEmail = d.SupportEmail,
                SupportUrl = d.SupportURL,
                BrandStoreType = d.Type == null ? BrandStoreType.OrdinaryStore : (BrandStoreType)d.Type,
                IsLogo = d.HasLogo == null ? false : d.HasLogo,
                IsDisPlayZone = d.IsShowInZone == "Y",
                AdImage = d.AdImage,
                ImageUrl = d.AdImage,
                ShowStoreUrl = d.NeweggUrl,
                CustomerServicePhone = d.CustomerServicePhone,
                BrandStatus = ValidStatus.Active,//默认有效
                ManufacturerName = d.ManufacturerName,
                Auditor =d.Auditor,
                SysNo=d.SysNo,
                ManufacturerBriefName=d.BriefName,
                ManufacturerWebsite=d.ManufacturerWebsite,
                ManufacturerSysNo = Convert.ToString(d.ManufacturerSysno),
                ProductId = d.ProductId,
                BrandCode = d.BrandCode

            };
            item.Action = BrandAction.Audit;
            item.Dialog = Window.ShowDialog("品牌审核", item, (s, args) =>
            {
                if (args.DialogResult == Newegg.Oversea.Silverlight.Controls.Components.DialogResultType.OK)
                {
                    this.BrandRequestResult.Bind();
                }
            }, new Size(650, 600));
        }
       

    }
}
