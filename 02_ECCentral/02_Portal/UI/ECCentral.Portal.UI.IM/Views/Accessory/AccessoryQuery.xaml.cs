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
using Newegg.Oversea.Silverlight.Controls.Data;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;

using ECCentral.Portal.Basic;
using ECCentral.Portal.UI.IM.Resources;
using ECCentral.Portal.UI.IM.Models;
using ECCentral.Portal.UI.IM.Facades;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.Portal.Basic.Components.UserControls.Language;

namespace ECCentral.Portal.UI.IM.Views
{
    [View(IsSingleton = true, SingletonType = SingletonTypes.Url)]
    public partial class AccessoryQuery : PageBase
    {
        #region 属性
        AccessoryQueryVM model;
        private List<AccessoryVM> _vmList;
        #endregion

        #region 初始化加载

        public AccessoryQuery()
        {
            InitializeComponent();
        }  

        public override void OnPageLoad(object sender, EventArgs e)
        {
            base.OnPageLoad(sender, e);
            model = new AccessoryQueryVM();
            this.DataContext = model;
        }

        #endregion

        #region 查询绑定
        private void btnAccessorySearch_Click(object sender, RoutedEventArgs e)
        {
            dgAccessoryQueryResult.Bind();
        }

        private void dgAccessoryQueryResult_LoadingDataSource(object sender, LoadingDataEventArgs e)
        {
            AccessoryQueryFacade facade = new AccessoryQueryFacade(this);

            facade.QueryAccessory(model, e.PageSize, e.PageIndex, e.SortField, (obj, args) =>
            {
                _vmList = DynamicConverter<AccessoryVM>.ConvertToVMList<List<AccessoryVM>>(args.Result.Rows);
                this.dgAccessoryQueryResult.ItemsSource = _vmList;
                this.dgAccessoryQueryResult.TotalCount = args.Result.TotalCount;
            });
        }
        #endregion

        #region 页面内按钮处理事件

        #region 界面事件
        private void ckbSelectAllRow_Click(object sender, RoutedEventArgs e)
        { 
        
        }

        //多语言编辑
        private void MultiLanguagehyperlinkAccessoryID_Click(object sender, RoutedEventArgs e)
        {
            dynamic manufacturer = this.dgAccessoryQueryResult.SelectedItem as dynamic;
            UCMultiLanguageMaintain item = new UCMultiLanguageMaintain(manufacturer.SysNo, "Accessories");

            item.Dialog = Window.ShowDialog("编辑配件多语言", item, (s, args) =>
            {

            }, new Size(750, 600));
        }
        #endregion

        #endregion

        #region 跳转

        //新建配件
        private void btnAccessoryNew_Click(object sender, RoutedEventArgs e)
        {
            Window.Navigate(ConstValue.IM_AccessoryMaintainCreateFormat, null, true);
        }

        //编辑配件
        private void hyperlinkAccessoryID_Click(object sender, RoutedEventArgs e)
        {
            dynamic manufacturer = this.dgAccessoryQueryResult.SelectedItem as dynamic;
            if (manufacturer != null)
            {
                this.Window.Navigate(string.Format(ConstValue.IM_AccessoryMaintainUrlFormat, manufacturer.SysNo), null, true);
            }
        }
        #endregion
    }
}
