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
using System.Windows.Navigation;

using Newegg.Oversea.Silverlight.Controls;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using ECCentral.Portal.UI.IM.Models;
using ECCentral.Portal.UI.IM.UserControls;
using ECCentral.Portal.UI.IM.Facades;
using ECCentral.Portal.Basic.Utilities;
using Newegg.Oversea.Silverlight.Controls.Components;
using ECCentral.Portal.UI.IM.Resources;
using ECCentral.Portal.Basic.Components.UserControls.Language;

namespace ECCentral.Portal.UI.IM.Views
{
    [View(IsSingleton = true, SingletonType = SingletonTypes.Url)]
    public partial class PropertyQuery : PageBase
    {

        PropertyQueryVM model;
        private List<PropertyVM> _vmList;

        public PropertyQuery()
        {
            InitializeComponent();
        }

        public override void OnPageLoad(object sender, EventArgs e)
        {
            base.OnPageLoad(sender, e);
            model = new PropertyQueryVM();
            this.DataContext = model;
            cbPropertyStatus.SelectedIndex = 0;
        }

        private void btnNewItem_Click(object sender, RoutedEventArgs e)
        {
            //属性添加
            PropertyMaintain propertyMainUC = new PropertyMaintain();
            propertyMainUC.Dialog = Window.ShowDialog(ECCentral.Portal.UI.IM.Resources.ResPropertyMaintain.Dialog_AddProperty, propertyMainUC, (s, args) =>
            {
                if (args.DialogResult == Newegg.Oversea.Silverlight.Controls.Components.DialogResultType.OK)
                {
                    dgPropertyQueryResult.Bind();
                }
            }, new Size(500, 200));            
        }

        private void btnPropertySearch_Click(object sender, RoutedEventArgs e)
        {
            dgPropertyQueryResult.Bind();
        }

        private void hyperlinkPropertySysNo_Click(object sender, RoutedEventArgs e)
        {
            dynamic property = this.dgPropertyQueryResult.SelectedItem as dynamic;
            if (property != null)
            {
                PropertyMaintain propertyMainUC = new PropertyMaintain();
                propertyMainUC.BeginEditing(property.SysNo);

                propertyMainUC.Dialog = Window.ShowDialog(ECCentral.Portal.UI.IM.Resources.ResPropertyMaintain.Dialog_EditProperty, propertyMainUC, (s, args) =>
                {
                    if (args.DialogResult == Newegg.Oversea.Silverlight.Controls.Components.DialogResultType.OK)
                    {
                        dgPropertyQueryResult.Bind();
                    }
                }, new Size(500, 200));
            }
            else
            {
                Window.Alert(ResPropertyQuery.Msg_OnSelectProperty, MessageType.Error);
            }
        }

        private void dgPropertyQueryResult_LoadingDataSource(object sender, Newegg.Oversea.Silverlight.Controls.Data.LoadingDataEventArgs e)
        {
            PropertyQueryFacade facade = new PropertyQueryFacade(this);

            facade.QueryProperty(model, e.PageSize, e.PageIndex, e.SortField, (obj, args) =>
            {
                _vmList = DynamicConverter<PropertyVM>.ConvertToVMList<List<PropertyVM>>(args.Result.Rows);
                this.dgPropertyQueryResult.ItemsSource = _vmList;
                this.dgPropertyQueryResult.TotalCount = args.Result.TotalCount;
            });
        }

        private void hyperlinkEditPropertyValueSysNo_Click(object sender, RoutedEventArgs e)
        {
            //属性值编辑
            dynamic property = this.dgPropertyQueryResult.SelectedItem as dynamic;

            PropertyValueMaintain propertyValueMainUC = new PropertyValueMaintain();
            propertyValueMainUC.BeginEditing(property.SysNo);
            propertyValueMainUC.MyWindow = Window;
            propertyValueMainUC.Dialog = Window.ShowDialog(ECCentral.Portal.UI.IM.Resources.ResPropertyValueMaintain.Dialog_EditPropertyValue, propertyValueMainUC, (s, args) =>
            {
                if (args.DialogResult == Newegg.Oversea.Silverlight.Controls.Components.DialogResultType.OK)
                {
                    dgPropertyQueryResult.Bind();
                }
            }, new Size(770, 600));
        }

        private void hyperlinkEditEditPropertyMultiLanguage_Click(object sender, RoutedEventArgs e)
        {
            dynamic property = this.dgPropertyQueryResult.SelectedItem as dynamic;

            UCMultiLanguageMaintain item = new UCMultiLanguageMaintain(property.SysNo, "PIM_Property");

            item.Dialog = Window.ShowDialog("更新属性多语言", item, (s, args) =>
            {
                if (args.DialogResult == Newegg.Oversea.Silverlight.Controls.Components.DialogResultType.OK)
                {
                    this.dgPropertyQueryResult.Bind();
                }
            }, new Size(750, 600));
        }


    }

}
