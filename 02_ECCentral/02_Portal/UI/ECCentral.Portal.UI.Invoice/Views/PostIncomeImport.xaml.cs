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
using System.Windows.Navigation;
using System.Windows.Shapes;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using Newegg.Oversea.Silverlight.Controls;
using ECCentral.Portal.UI.Invoice.Facades;
using ECCentral.Portal.Basic.Controls.Uploader;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.Portal.Basic;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using ECCentral.Portal.UI.Invoice.Resources;

namespace ECCentral.Portal.UI.Invoice.Views
{
    /// <summary>
    /// 导入电汇邮局收款
    /// </summary>
    [View(IsSingleton = true, SingletonType = SingletonTypes.Page, NeedAccess = true)]
    public partial class PostIncomeImport : PageBase
    {
        private PostIncomeFacade facade;
        public PostIncomeImport()
        {
            InitializeComponent();
            RegisterEvents();
        }
        private void RegisterEvents()
        {
            this.Loaded += new RoutedEventHandler(PostIncomeImport_Loaded);
            this.uploader.AllFileUploadCompleted += new Basic.Controls.Uploader.AllUploadCompletedEvent(uploader_AllFileUploadCompleted);
        }

        private void PostIncomeImport_Loaded(object sender, RoutedEventArgs e)
        {
            this.uploader.IsEnabled = AuthMgr.HasFunctionPoint(AuthKeyConst.Invoice_PostIncomeImport_Import);

           
            facade = new PostIncomeFacade(this);
        }

        private void uploader_AllFileUploadCompleted(object sender, Basic.Controls.Uploader.AllUploadCompletedEventArgs args)
        {
            uploader.Clear();
            if (args.UploadInfo[0].UploadResult == SingleFileUploadStatus.Failed)
            {
                Window.Alert(ResPostIncomeImport.Upload_Fault, Newegg.Oversea.Silverlight.Controls.Components.MessageType.Error);
                return;
            }

            facade.ImportPostIncome(args.UploadInfo[0].ServerFilePath, result =>
            {
                this.dgConfirmSuccessResult.ItemsSource = result.SuccessList;
                this.dgConfirmFailedsResult.ItemsSource = result.FaultList;
                Window.Alert(result.Message);
            });
        }


        private void btnDownloadTemp_Click(object sender, RoutedEventArgs e)
        {
            


            AppSettingHelper.GetSetting(ConstValue.DomainName_Invoice, "ImportPostIncomeTemplate", (obj, args) =>
            {
                string url = CPApplication.Current.CurrentPage.Context.Window.Configuration.GetConfigValue(ConstValue.DomainName_Invoice, "ServiceBaseUrl") + args.Result;

                UtilityHelper.OpenWebPage(url);
            });

        }

       
    }
}