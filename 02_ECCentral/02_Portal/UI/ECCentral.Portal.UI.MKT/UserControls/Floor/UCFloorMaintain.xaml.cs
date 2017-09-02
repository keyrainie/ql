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
using ECCentral.BizEntity.Enum.Resources;
using ECCentral.BizEntity.MKT;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.Portal.UI.MKT.Facades;
using ECCentral.Portal.UI.MKT.Models.Floor;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using Newegg.Oversea.Silverlight.Controls;
using Newegg.Oversea.Silverlight.Controls.Components;
using Newegg.Oversea.Silverlight.Utilities.Validation;

namespace ECCentral.Portal.UI.MKT.UserControls.Floor
{
    public partial class UCFloorMaintain : UserControl
    {
        private FloorFacade facade;
        public IDialog Dialog { get; set; }
        private IWindow CurrentWindow
        {
            get { return CPApplication.Current.CurrentPage.Context.Window; }
        }

        public UCFloorMaintain()
        {
            InitializeComponent();
            Model = new FloorVM();
            DataContext = Model;
            facade = new FloorFacade(CPApplication.Current.CurrentPage);
        }

        public UCFloorMaintain(FloorVM vm)
        {
            InitializeComponent();
            if (vm.SysNo > 0)
            {
                Model = new FloorVM()
                {
                    SysNo = vm.SysNo,
                    FloorLogoSrc = vm.FloorLogoSrc,
                    FloorName = vm.FloorName,
                    Remark = vm.Remark,
                    Status = vm.Status,
                    TemplateName = vm.TemplateName,
                    Templates = vm.Templates,
                    TemplateSysNo = vm.TemplateSysNo,
                    Priority = vm.Priority,
                    PageType = vm.PageType,
                    PageCode = vm.PageCode,
                    PageName = vm.PageName,
                };
            }
            else
            {
                Model = vm;
            }
            facade = new FloorFacade(CPApplication.Current.CurrentPage);
            cbPageCodes.ItemsSource = EnumConverter.GetKeyValuePairs<PageCodeType>(EnumConverter.EnumAppendItemType.Select);
            cbPageCodes.SelectedIndex = 0;

            this.cbPageCodes.SelectionChanged += new SelectionChangedEventHandler(cbPageCodes_SelectionChanged);
            this.DataContext = Model;
        }

        public FloorVM Model { get; set; }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            ValidationManager.Validate(this);
            if (Model.HasValidationErrors) return;

            Model.PageName = Model.PageType == PageCodeType.C1 || Model.PageType == PageCodeType.Promotion ? ((KeyValuePair<string, string>)cbPageID.SelectedItem).Value : ((PageCodeType)cbPageCodes.SelectedValue).ToDescription();
            facade = new FloorFacade(((IPage)CPApplication.Current.CurrentPage));
            facade.Save(Model, (s, args) =>
            {
                Dialog.Close(true);
            });
        }

        private void cbPageCodes_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cbPageCodes.SelectedItem == null) return;
            var pageType = (PageCodeType?)cbPageCodes.SelectedValue;
            spPageID.Visibility = Visibility.Collapsed;
            if (pageType == PageCodeType.C1 || pageType == PageCodeType.Promotion)
            {
                facade.QueryPageID(((int)pageType).ToString(), CPApplication.Current.CompanyCode, (s, args) =>
                {
                    args.Result.Insert(0, new KeyValuePair<string, string>(string.Empty, ResCommonEnum.Enum_Select));
                    cbPageID.ItemsSource = args.Result;
                    spPageID.Visibility = Visibility.Visible;
                    cbPageID.SelectedValue = string.IsNullOrEmpty(Model.PageCode) ? string.Empty : Model.PageCode;
                });
            }
            else
            {
                Model.PageCode = "1";
            }
        }
    }
}
