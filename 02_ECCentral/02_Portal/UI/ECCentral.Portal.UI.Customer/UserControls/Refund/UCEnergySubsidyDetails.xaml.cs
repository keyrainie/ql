using System.Windows;
using System.Windows.Controls;
using ECCentral.Portal.UI.Customer.Facades;
using ECCentral.Portal.UI.Customer.Models;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using Newegg.Oversea.Silverlight.Controls;
using Newegg.Oversea.Silverlight.Controls.Components;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.BizEntity.Customer;

namespace ECCentral.Portal.UI.Customer.UserControls.Refund
{
    public partial class UCEnergySubsidyDetails : UserControl
    {
        #region 页面初始化
        public IDialog Dialog { get; set; }

        private IWindow CurrentWindow
        {
            get
            {
                return CPApplication.Current.CurrentPage.Context.Window;
            }
        }

        private EnergySubsidyVM vm;
        private RefundAdjustFacade facade;

        public int SOSysNo { get; set; }

        public string ViewType { get; set; }

        public UCEnergySubsidyDetails()
        {
            InitializeComponent();
        }

        public UCEnergySubsidyDetails(int soSysNo, string viewType)
        {
            InitializeComponent();
            this.SOSysNo = soSysNo;
            this.ViewType = viewType;
            facade = new RefundAdjustFacade(CPApplication.Current.CurrentPage);
            this.gridBasicInfo.DataContext = vm = new EnergySubsidyVM();
            vm.ViewType = this.ViewType;
            vm.SOSysNo = this.SOSysNo;
            this.Loaded += new RoutedEventHandler(UCEnergySubsidyDetails_Loaded);
        }

        void UCEnergySubsidyDetails_Loaded(object sender, RoutedEventArgs e)
        {
            vm.QueryType = "BasicInfo";
            facade.GetEnergySubsidyDetails(vm, (obj, args) =>
                {
                    if (args.FaultsHandle()) return;
                    if (args.Result != null)
                    {
                        this.gridBasicInfo.DataContext = vm = args.Result[0].Convert<EnergySubsidyInfo, EnergySubsidyVM>();
                        this.dgProductInfo.Bind();
                    }
                });
        }

        #endregion

        //关闭
        private void Button_Close_Click(object sender, RoutedEventArgs e)
        {
            this.Dialog.Close();
        }

        private void dgProductInfo_LoadingDataSource(object sender, Newegg.Oversea.Silverlight.Controls.Data.LoadingDataEventArgs e)
        {
            vm.QueryType = "ProductInfo";
            facade.GetEnergySubsidyDetails(vm, (obj, args) =>
                {
                    if (args.FaultsHandle()) return;
                    if (args.Result != null)
                    {
                        this.dgProductInfo.ItemsSource = args.Result;
                    }
                });
        }
    }
}
