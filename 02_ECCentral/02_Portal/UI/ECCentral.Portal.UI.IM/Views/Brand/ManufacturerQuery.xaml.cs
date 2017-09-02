using System;
using System.Windows;
using ECCentral.Portal.Basic;
using ECCentral.Portal.UI.IM.Facades;
using ECCentral.Portal.UI.IM.Models;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using Newegg.Oversea.Silverlight.Controls;
using Newegg.Oversea.Silverlight.Controls.Data;
using ECCentral.Portal.UI.IM.UserControls;
using ECCentral.Portal.Basic.Components.UserControls.Language;

namespace ECCentral.Portal.UI.IM.Views
{
    [View(IsSingleton = true, SingletonType = SingletonTypes.Url)]
    public partial class ManufacturerQuery : PageBase
    {
        #region 属性
        ManufacturerQueryVM model;
        #endregion

        #region 初始化加载

        public ManufacturerQuery()
        {
            InitializeComponent();
        }

        public override void OnPageLoad(object sender, EventArgs e)
        {
            base.OnPageLoad(sender, e);
            model = new ManufacturerQueryVM();
            this.DataContext = model;
        }

        #endregion

        #region 查询绑定
        private void btnManufacturerSearch_Click(object sender, RoutedEventArgs e)
        {
            dgManufacturerQueryResult.Bind();
        }

        private void dgManufacturerQueryResult_LoadingDataSource(object sender, LoadingDataEventArgs e)
        {
            ManufacturerQueryFacade facade = new ManufacturerQueryFacade(this);
            facade.QueryManufacturer(model, e.PageSize, e.PageIndex, e.SortField, (obj, args) =>
            {
                this.dgManufacturerQueryResult.ItemsSource = args.Result.Rows;
                this.dgManufacturerQueryResult.TotalCount = args.Result.TotalCount;
            });
        }
        #endregion

        #region 页面内按钮处理事件

        #region 界面事件
        private void ckbSelectAllRow_Click(object sender, RoutedEventArgs e)
        {

        }
        private void HyperlinkViewBrand_Click(object sender, RoutedEventArgs e)
        {
            dynamic manufacturer = this.dgManufacturerQueryResult.SelectedItem as dynamic;
            if (manufacturer != null)
            {
                this.Window.Navigate(string.Format(ConstValue.IM_BrandQueryUrlFormat, manufacturer.SysNo),null,true);
            }
        }


        



        private void MultiLanguagelinkManufacturer_Click(object sender, RoutedEventArgs e)
        {
            dynamic manufacturer = this.dgManufacturerQueryResult.SelectedItem as dynamic;

            UCMultiLanguageMaintain item = new UCMultiLanguageMaintain(manufacturer.SysNo, "Manufacturer");

            item.Dialog = Window.ShowDialog("编辑制造商多语言", item, (s, args) =>
            {
                
            }, new Size(750, 600));
        }
        #endregion

        #endregion

        #region 跳转

        //新建生产商
        private void btnManufacturerNew_Click(object sender, RoutedEventArgs e)
        {
            ManufacturerRequestMaintain item = new ManufacturerRequestMaintain();
            item.Action =ManufacturerAction.Add;
            item.Dialog = Window.ShowDialog("添加生产商", item, (s, args) =>
            {
                if (args.DialogResult == Newegg.Oversea.Silverlight.Controls.Components.DialogResultType.OK)
                {
                    this.dgManufacturerQueryResult.Bind();
                }
            }, new Size(600, 500));

           //Window.Navigate(ConstValue.IM_ManufacturerMaintainCreateFormat, null, true);
        }

        //编辑生产商
        private void hyperlinkManufacturerID_Click(object sender, RoutedEventArgs e)
        {
            dynamic manufacturer = this.dgManufacturerQueryResult.SelectedItem as dynamic;
            ManufacturerRequestMaintain item = new ManufacturerRequestMaintain();
            item.ManufacturerSysNo = manufacturer.SysNo;
            item.Action = ManufacturerAction.Edit;
            item.Dialog = Window.ShowDialog("更新生产商", item, (s, args) =>
            {
                if (args.DialogResult == Newegg.Oversea.Silverlight.Controls.Components.DialogResultType.OK)
                {
                    this.dgManufacturerQueryResult.Bind();
                }
            }, new Size(600, 500));
            //if (manufacturer != null)
            //{
            //    this.Window.Navigate(string.Format(ConstValue.IM_ManufacturerMaintainUrlFormat, manufacturer.SysNo), null, true);
            //}
        }

        //同步生产商
        private void ManufacturerRelation_Click(object sender, RoutedEventArgs e)
        {
            dynamic manufacturer = this.dgManufacturerQueryResult.SelectedItem as dynamic;
            ManufacturerRelationMaintain item = new ManufacturerRelationMaintain();
            item.LocalManufacturerSysNo = manufacturer.SysNo;
            item.Dialog = Window.ShowDialog("同步生产商", item, (s, args) =>
            {
                if (args.DialogResult == Newegg.Oversea.Silverlight.Controls.Components.DialogResultType.OK)
                {
                    this.dgManufacturerQueryResult.Bind();
                }
            }, new Size(350,300));
        }


        #endregion

        private void BtnAudit_Click(object sender, RoutedEventArgs e)
        {
            Window.Navigate(ConstValue.IM_ManufacturerRequestUrl,null,true);
        }

        

    }
}
