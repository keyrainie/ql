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
using ECCentral.Portal.UI.PO.Facades;
using ECCentral.BizEntity.PO;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.Portal.UI.PO.Models;

namespace ECCentral.Portal.UI.PO.UserControls
{
    public partial class BasketImportDataView : UserControl
    {
        public IDialog Dialog { get; set; }
        public PurchaseOrderBasketFacade serviceFacade;
        public IWindow CurrentWindow
        {
            get
            {
                return CPApplication.Current.CurrentPage.Context.Window;
            }
        }

        public string FileIdentity
        {
            get;
            set;
        }

        public BasketImportDataView()
        {
            InitializeComponent();
            this.Loaded += new RoutedEventHandler(BasketImportDataView_Loaded);
        }

        void BasketImportDataView_Loaded(object sender, RoutedEventArgs e)
        {
            this.Loaded -= new RoutedEventHandler(BasketImportDataView_Loaded);
            serviceFacade = new PurchaseOrderBasketFacade(CPApplication.Current.CurrentPage);
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            //取消
            this.Dialog.ResultArgs.Data = null;
            this.Dialog.Close(true);
        }

        private void ucFileUploader_AllFileUploadCompleted(object sender, Basic.Controls.Uploader.AllUploadCompletedEventArgs args)
        {
            if (args.UploadInfo != null && args.UploadInfo.Count > 0 && args.UploadInfo[0].UploadResult == Basic.Controls.Uploader.SingleFileUploadStatus.Success)
            {
                string getFileIdentity = args.UploadInfo[0].ServerFilePath;
                this.FileIdentity = getFileIdentity;

                //解析上传Excel，返回List<>:
                serviceFacade.ConvertBasketTemplateFileToEntityList(getFileIdentity, (objs, argss) =>
                {
                    if (argss.FaultsHandle())
                    {
                        return;
                    }
                    this.Dialog.ResultArgs.Data = argss.Result;
                    //CPApplication.Current.CurrentPage.Context.Window.Alert(string.Format("导入成功{0}条，导入失败{1}条", (string)argss.Result[1].Rows[0]["successCount"], (string)argss.Result[1].Rows[0]["failedCount"]));
                    this.Dialog.Close(true);
                });
            }
            else
            {
                CurrentWindow.Alert("上传模板失败!");
                return;
            }
        }

        private void btnDownloadTemplate_Click(object sender, RoutedEventArgs e)
        {
            //模板下载操作 ：
            AppSettingHelper.GetSetting("PO", "BasketTemplateFileDownloadPath", (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }
                UtilityHelper.OpenWebPage(System.IO.Path.Combine(CPApplication.Current.CurrentPage.Context.Window.Configuration.GetConfigValue("PO", "ServiceBaseUrl"), args.Result).Replace('\\', '/'));
            });
        }

    }
}
