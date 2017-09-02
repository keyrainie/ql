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
using ECCentral.Portal.Basic.Controls.Uploader;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using ECCentral.Portal.UI.MKT.Facades.Promotion;
using ECCentral.BizEntity.MKT;
using ECCentral.Portal.Basic;

namespace ECCentral.Portal.UI.MKT.UserControls
{
    public partial class CountDownImport : UserControl
    {
        public IDialog Dialog { get; set; }
        public bool IsPromotionSchedule { private get; set; }
        public CountDownImport()
        {
            InitializeComponent();
            this.Loaded += (sender, e) => 
            {
                uploadImoportFile.UploadCompleted += new UploadCompletedEvent(uploadImoportFile_UploadCompleted);
            };
        }
        private void CloseDialog(DialogResultType dialogResult)
        {
            if (Dialog != null)
            {
                Dialog.ResultArgs.DialogResult = dialogResult;
                Dialog.Close();
            }
        }

        private void BtnCanel_Click(object sender, RoutedEventArgs e)
        {
            CloseDialog(DialogResultType.Cancel);
        }

        void uploadImoportFile_UploadCompleted(object sender, UploadCompletedEventArgs args)
        {
            uploadImoportFile.Clear();
            
            if (args.UploadResult == SingleFileUploadStatus.Failed)
            {
                CPApplication.Current.CurrentPage.Context.Window.Alert("上传文件失败!");
                return;
            }
            if (IsPromotionSchedule)
            {
                new CountdownFacade(CPApplication.Current.CurrentPage).BatchImportSchedule(new CountdownInfo() { FileIdentity = args.ServerFilePath, PMRole = this.PMRole }, (obj, arg) =>
                {
                    if (arg.FaultsHandle())
                    {
                        return;
                    }
                });
            }
            else
            {
                new CountdownFacade(CPApplication.Current.CurrentPage).BatchImportCountDown(new CountdownInfo() { FileIdentity = args.ServerFilePath, PMRole = this.PMRole }, (obj, arg) =>
                {
                    if (arg.FaultsHandle())
                    {
                        return;
                    }
                });
            }

        }
        private int _pmRole;
        /// <summary>
        /// PM权限
        /// </summary>
        public int PMRole
        {
            get
            {
                _pmRole = 0;
                if (AdvancedReadCountDown_Check())
                {
                    _pmRole = 2;
                }
                else if (PrimaryReadCountDown_Check())
                {
                    _pmRole = 1;
                }
                return _pmRole;
            }
        }
        private bool PrimaryReadCountDown_Check()
        {
            return AuthMgr.HasFunctionPoint(AuthKeyConst.MKT_Countdown_PrimaryReadCountDown_Check);
        }

        private bool AdvancedReadCountDown_Check()
        {
            return AuthMgr.HasFunctionPoint(AuthKeyConst.MKT_Countdown_AdvancedReadCountDown_Check);
        }
    }
}
