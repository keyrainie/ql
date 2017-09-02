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
using ECCentral.BizEntity.Inventory;

namespace ECCentral.Portal.UI.Inventory.UserControls
{
    public partial class ShiftRequestMemoClose : UserControl
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

        private List<int> memoSysNoList;
        public List<int> MemoSysNoList
        {
            get
            {
                return memoSysNoList;
            }
            set
            {
                memoSysNoList = value;

            }
        }

        private ShiftRequestMemoVM memoVM;
        public ShiftRequestMemoVM MemoVM
        {
            get
            {
                return memoVM ?? new ShiftRequestMemoVM();
            }
            set
            {
                memoVM = value;
            }
        }
        public ShiftRequestMemoClose()
        {
            InitializeComponent();            
            Loaded += new RoutedEventHandler(ShiftRequestMemo_Loaded);
        }
        void ShiftRequestMemo_Loaded(object sender, RoutedEventArgs e)
        {
            MaintainFacade = new ShiftRequestMaintainFacade(Page);
            this.DataContext = MemoVM;
            Loaded -= new RoutedEventHandler(ShiftRequestMemo_Loaded);
        }
        
        private void btnCloseMemo_Click(object sender, RoutedEventArgs e)
        {
            ValidationManager.Validate(this);
            if (MemoVM.HasValidationErrors)
            {
                return;
            }

            List<ShiftRequestMemoVM> vmList = new List<ShiftRequestMemoVM>();
            MemoSysNoList.ForEach(x => {
               ShiftRequestMemoVM memoVM = new ShiftRequestMemoVM() {                     
                    SysNo = x,
                    MemoStatus = ShiftRequestMemoStatus.Finished,         
                    EditDate = DateTime.Now,
                    EditUserSysNo = CPApplication.Current.LoginUser.UserSysNo,
                    Note = MemoVM.Note
                };

               vmList.Add(memoVM);
            });            
          
            MaintainFacade.UpdateShiftRequestMemo(vmList, (list) =>
            {
                Page.Context.Window.Alert("关闭成功");
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
