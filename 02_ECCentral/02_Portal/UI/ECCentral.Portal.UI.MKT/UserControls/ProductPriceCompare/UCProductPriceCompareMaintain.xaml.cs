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
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using ECCentral.Portal.UI.MKT.Facades;
using ECCentral.BizEntity.MKT;
using ECCentral.Portal.UI.MKT.Models;
using ECCentral.Portal.Basic.Utilities;
using Newegg.Oversea.Silverlight.Controls.Components;
using Newegg.Oversea.Silverlight.Controls;
using Newegg.Oversea.Silverlight.Utilities.Validation;
using ECCentral.Portal.UI.MKT.Resources;

namespace ECCentral.Portal.UI.MKT.UserControls
{
    public partial class UCProductPriceCompareMaintain : UserControl
    {
        private ProductPriceCompareVM _currentVM;
        private List<InvalidReasonVM> _reasonList;

        /// <summary>
        /// 当前打开的Tab页面
        /// </summary>
        private IWindow CurrentWindow
        {
            get
            {
                return CPApplication.Current.CurrentPage.Context.Window;
            }
        }

        /// <summary>
        /// 窗口句柄
        /// </summary>
        public IDialog DialogHandle { get; set; }

        public UCProductPriceCompareMaintain(int currentSysNo)
        {
            InitializeComponent();

            this.lstChannelList.ItemsSource = CPApplication.Current.CurrentWebChannelList.ToList<UIWebChannel>();

            //当前界面正在编辑模式,加载数据
            var facade = new ProductPriceCompareFacade(CPApplication.Current.CurrentPage);
            facade.Load(currentSysNo, (vm) =>
                {
                    _currentVM = vm;
                    this.DataContext = _currentVM;
                    facade.GetInvalidReasons((reasonList) =>
                    {
                        _reasonList = reasonList;
                        this.lstInvalidReasons.ItemsSource = reasonList;
                    });
                });
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (_reasonList == null || _currentVM == null || _currentVM.SysNo == null)
            {
                return;
            }
            string commaSeperatedReasonIDs = "";
            foreach (var item in _reasonList)
            {
                if (item.IsChecked == true)
                {
                    commaSeperatedReasonIDs += "," + item.ReasonID;
                }
            }
            commaSeperatedReasonIDs = commaSeperatedReasonIDs.TrimEnd(',');
            if (commaSeperatedReasonIDs.Length == 0)
            {
                CurrentWindow.Alert(ResProductPriceCompare.Info_PleaseSelectInvalidReason);
                return;
            }
            var facade = new ProductPriceCompareFacade(CPApplication.Current.CurrentPage);
            //审核无效
            facade.AuditDecline(_currentVM.SysNo.Value, commaSeperatedReasonIDs, () =>
            {
                CurrentWindow.Alert(ResProductPriceCompare.Info_AuditDeclineSuccess);
                CloseDialog(DialogResultType.OK);
            });

        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            CloseDialog(DialogResultType.Cancel);
        }

        private void CloseDialog(DialogResultType dialogResultType)
        {
            if (DialogHandle != null)
            {
                DialogHandle.ResultArgs = new ResultEventArgs
                {
                    DialogResult = dialogResultType
                };
                DialogHandle.Close();
            }
        }
    }
}
