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
using Newegg.Oversea.Silverlight.Utilities.Validation;

using ECCentral.Portal.UI.Inventory.Facades;
using ECCentral.Portal.UI.Inventory.Models;

namespace ECCentral.Portal.UI.Inventory.UserControls
{
    public partial class ShiftRequestMemo : UserControl
    {
        ShiftRequestMaintainFacade MaintainFacade;

        public IPage Page
        {
            get;
            set;
        }
        public IDialog Dialog
        {
            get;
            set;
        }

        private List<int> requestSysNoList;
        public List<int> RequestSysNoList
        {
            get
            {
                return requestSysNoList;
            }
            set
            {
                requestSysNoList = value;

            }
        }
        ShiftRequestMemoVM MemoVM;
        public ShiftRequestMemo()
        {
            InitializeComponent();
            Loaded += new RoutedEventHandler(ShiftRequestMemo_Loaded);
        }
        void ShiftRequestMemo_Loaded(object sender, RoutedEventArgs e)
        {
            MaintainFacade = new ShiftRequestMaintainFacade(Page);
            MemoVM = new ShiftRequestMemoVM();
            MemoVM.MemoStatus = BizEntity.Inventory.ShiftRequestMemoStatus.FollowUp;
            this.DataContext = MemoVM;
            Loaded -= new RoutedEventHandler(ShiftRequestMemo_Loaded);
        }

        private void btnAddLog_Click(object sender, RoutedEventArgs e)
        {
            ValidationManager.Validate(this);
            if (MemoVM.HasValidationErrors)
            {
                return;
            }

            List<ShiftRequestMemoVM> vmList = new List<ShiftRequestMemoVM>();
            RequestSysNoList.ForEach(sysNo =>
                {
                    vmList.Add(new ShiftRequestMemoVM
                    {
                        RequestSysNo = sysNo,
                        CreateUserSysNo = CPApplication.Current.LoginUser.UserSysNo,
                        CreateDate = DateTime.Now,
                        Content = MemoVM.Content,
                        RemindTime = MemoVM.RemindTime,
                        MemoStatus = MemoVM.MemoStatus
                    });
                });
            MaintainFacade.AddShiftRequestMemo(vmList, (list) =>
            {
                Page.Context.Window.Alert("添加成功");
                CloseDialog(new ResultEventArgs
                {
                    DialogResult = DialogResultType.OK,
                    Data = list
                });
            });
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            CloseDialog(new ResultEventArgs
            {
                DialogResult = DialogResultType.Cancel
            });
        }


        #region Helper Methods

        private void CloseDialog(ResultEventArgs args)
        {
            if (Dialog != null)
            {
                Dialog.ResultArgs = args;
                Dialog.Close();
            }
        }

        #endregion Helper Methods
    }
}
