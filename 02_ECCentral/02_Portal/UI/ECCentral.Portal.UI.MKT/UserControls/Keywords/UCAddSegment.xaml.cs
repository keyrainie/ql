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
using ECCentral.Portal.UI.MKT.Models;
using ECCentral.Portal.UI.MKT.Facades;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using ECCentral.BizEntity.MKT;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.Portal.Basic;
using Newegg.Oversea.Silverlight.Utilities.Validation;
using ECCentral.Portal.UI.MKT.Resources;
using Newegg.Oversea.Silverlight.Controls.Components;

namespace ECCentral.Portal.UI.MKT.UserControls.Keywords
{
    public partial class UCAddSegment : UserControl
    {
        public IDialog Dialog { get; set; }
        public int SysNo{get;set;}
        private bool isAdd = true;
        private SegmentQueryVM vm;
        private SegmentQueryFacade facade;

        public UCAddSegment()
        {
            InitializeComponent();
            Loaded += new RoutedEventHandler(UCAddSegmentQuery_Loaded);
        }

        private void UCAddSegmentQuery_Loaded(object sender, RoutedEventArgs e)
        {
            Loaded -= new RoutedEventHandler(UCAddSegmentQuery_Loaded);
            facade = new SegmentQueryFacade(CPApplication.Current.CurrentPage);
            if (SysNo > 0)
            {
                isAdd = false;
                facade.LoadSegmentInfo(SysNo, (obj, args) =>
                {
                    if (args.FaultsHandle())
                        return;

                    vm = args.Result.Convert<SegmentInfo, SegmentQueryVM>();
                    vm.ChannelID="1";
                    LayoutRoot.DataContext = vm;
                });
            }
            else
            {
                vm = new SegmentQueryVM();
                vm.ChannelID = "1";
                LayoutRoot.DataContext = vm;
            }
        }

        private void btnClear_Click(object sender, RoutedEventArgs e)
        {
            vm.Keywords = string.Empty;
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            if (Dialog != null)
            {
                Dialog.ResultArgs.Data = null;
                Dialog.ResultArgs.DialogResult = DialogResultType.OK;
                Dialog.Close();
            }
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidationManager.Validate(this.LayoutRoot))
                return;
            vm = LayoutRoot.DataContext as SegmentQueryVM;
            if (string.IsNullOrEmpty(vm.Keywords))
            {
                CPApplication.Current.CurrentPage.Context.Window.Alert("关键字不能为空!", MessageType.Error);
                return;
            }
            SegmentInfo item = EntityConvertorExtensions.ConvertVM<SegmentQueryVM, SegmentInfo>(vm, (v, t) =>
            {
                t.Keywords = new BizEntity.LanguageContent(ConstValue.BizLanguageCode, v.Keywords);
            });
            item.Status = KeywordsStatus.Waiting;
            item.CompanyCode = Newegg.Oversea.Silverlight.ControlPanel.Core.CPApplication.Current.CompanyCode;
            if (isAdd)
            {
                facade.AddSegmentInfo(item, (obj, args) =>
                {
                    if (args.FaultsHandle())
                        return;

                    CPApplication.Current.CurrentPage.Context.Window.Alert(ResKeywords.Information_CreateSuccessful, Newegg.Oversea.Silverlight.Controls.Components.MessageType.Information);
                });
            }
            else
            {
                item.SysNo = SysNo;
                facade.UpdateSegmentInfo(item, (obj, args) =>
                {
                    if (args.FaultsHandle())
                        return;

                    CPApplication.Current.CurrentPage.Context.Window.Alert(ResKeywords.Information_UpdateSuccessful, Newegg.Oversea.Silverlight.Controls.Components.MessageType.Information);
                });
            }
        }
    }
}
