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
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

using ECCentral.Portal.UI.Inventory.Facades;
using ECCentral.Portal.UI.Inventory.Models;
using ECCentral.Portal.UI.Inventory.Resources;

using Newegg.Oversea.Silverlight.ControlPanel.Core;
using Newegg.Oversea.Silverlight.Controls;
using Newegg.Oversea.Silverlight.Controls.Components;
using Newegg.Oversea.Silverlight.Utilities;
using Newegg.Oversea.Silverlight.Utilities.Validation;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.BizEntity.Inventory;
using ECCentral.Portal.Basic;

namespace ECCentral.Portal.UI.Inventory.UserControls
{
    public partial class RMAShift : UserControl
    {
        public IDialog Dialog
        {
            get;
            set;
        }

        private IWindow CurrentWindow
        {
            get
            {
                return CPApplication.Current.CurrentPage.Context.Window;
            }
        }

        private List<RMAShiftVM> m_RMAShiftList;
        public List<RMAShiftVM> CurrentRMAShiftVMList
        {
            get
            {
                return m_RMAShiftList;
            }
            private set
            {
                m_RMAShiftList = value;
            }
        }

        private int shiftSysNo;

        public RMAShift(int shiftSysNo)
        {
            InitializeComponent();
            this.shiftSysNo = shiftSysNo;
            Loaded += RMAShift_Loaded;
        }

        void RMAShift_Loaded(object sender, RoutedEventArgs e)
        {
            gridShiftList.Bind();
        }

        private void gridShiftList_LoadingDataSource_1(object sender, Newegg.Oversea.Silverlight.Controls.Data.LoadingDataEventArgs e)
        {
            new ShiftRequestQueryFacade(CPApplication.Current.CurrentPage).GetRMAShift(shiftSysNo, (obj, args) =>
            {
                if (args.FaultsHandle()) return;

                CurrentRMAShiftVMList = DynamicConverter<RMAShiftVM>.ConvertToVMList(args.Result.Rows);
                if (CurrentRMAShiftVMList.Count != 0)
                {
                    LayoutRoot.DataContext = CurrentRMAShiftVMList[0];
                    gridShiftList.ItemsSource = CurrentRMAShiftVMList;
                    gridShiftList.TotalCount = CurrentRMAShiftVMList.Count;
                    txtTotalCount.Text = string.Format("共{0}条明细!", CurrentRMAShiftVMList.Count);
                }
            });
        }


        private void CloseDialog(ResultEventArgs args)
        {
            if (Dialog != null)
            {
                Dialog.ResultArgs = args;
                Dialog.Close();
            }
        }

        private void hplRegisterSysNo_Click_1(object sender, RoutedEventArgs e)
        {
            //TODO:链接至相应单据维护页面:
            HyperlinkButton btn = sender as HyperlinkButton;
            if (null != btn)
            {
                CurrentWindow.Navigate(String.Format(ConstValue.RMA_RegisterMaintainUrl, btn.CommandParameter), null, true);
            }
        }

    }
}
