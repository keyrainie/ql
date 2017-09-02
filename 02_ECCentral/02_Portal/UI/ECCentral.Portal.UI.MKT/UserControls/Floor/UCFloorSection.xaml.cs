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
using ECCentral.BizEntity.MKT;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.Portal.UI.MKT.Facades;
using ECCentral.Portal.UI.MKT.Models.Floor;
using ECCentral.Portal.UI.MKT.Resources;
using ECCentral.Portal.UI.MKT.Views;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using Newegg.Oversea.Silverlight.Controls;
using Newegg.Oversea.Silverlight.Controls.Components;
using Newegg.Oversea.Silverlight.Utilities.Validation;

namespace ECCentral.Portal.UI.MKT.UserControls.Floor
{
    public partial class UCFloorSection : UserControl
    {
        public FloorSectionMaintain FloorSectionMaintain { get; set; }

        public FloorSectionVM CurrentVM
        {
            get { return this.DataContext as FloorSectionVM; }
            set { this.DataContext = value; }
        }

        public IDialog CurrentDialog { get; set; }

        private IWindow CurrentWindow
        {
            get { return CPApplication.Current.CurrentPage.Context.Window; }
        }

        public UCFloorSection()
        {
            InitializeComponent();
            CurrentVM = new FloorSectionVM();
            this.Loaded += new RoutedEventHandler(UCFloorSection_Loaded);
        }

        public void UCFloorSection_Loaded(object sender, RoutedEventArgs e)
        {
            this.Loaded -= UCFloorSection_Loaded;
            cmbStatus.ItemsSource = EnumConverter.GetKeyValuePairs<ADStatus>(EnumConverter.EnumAppendItemType.Select);
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidationManager.Validate(this.LayoutRoot)) return;
            var floorFacade = new FloorFacade(CPApplication.Current.CurrentPage);
            CurrentVM.FloorMasterSysNo = FloorSectionMaintain.FloorVM.SysNo;

            if (CurrentVM.SysNo.HasValue)
            {
                floorFacade.UpdateFloorSection(CurrentVM, (s, args) =>
                {
                    if (args.FaultsHandle()) return;
                    CurrentWindow.Alert(ResFloorMaintain.Info_EditSuccess);
                    for (int i = 0; i < FloorSectionMaintain.FloorSectionListVM.Count; i++)
                    {
                        if (FloorSectionMaintain.FloorSectionListVM[i].SysNo == CurrentVM.SysNo)
                        {
                            CurrentVM.IsChecked = true;
                            FloorSectionMaintain.FloorSectionListVM[i] = CurrentVM.DeepCopy();
                            FloorSectionMaintain.SectionTagResult.ItemsSource = FloorSectionMaintain.FloorSectionListVM;
                            break;
                        }
                    }
                });
            }
            else
            {
                floorFacade.CreateFloorSection(CurrentVM, (s, args) =>
                {
                    if (args.FaultsHandle()) return;
                    CurrentVM.SysNo = args.Result;
                    FloorSectionMaintain.FloorSectionListVM.Add(CurrentVM);
                    CurrentWindow.Alert(ResFloorMaintain.Info_CreateSuccess);
                    FloorSectionMaintain.SectionTagResult.ItemsSource = FloorSectionMaintain.FloorSectionListVM;
                });
            }
        }

        private void btnReset_Click(object sender, RoutedEventArgs e)
        {
            CurrentVM = new FloorSectionVM();
        }
    }
}
