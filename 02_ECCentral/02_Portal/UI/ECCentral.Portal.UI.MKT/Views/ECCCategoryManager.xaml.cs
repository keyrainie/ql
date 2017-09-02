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
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using Newegg.Oversea.Silverlight.Controls;
using ECCentral.BizEntity.MKT;
using ECCentral.Portal.UI.MKT.Models.ECCategory;
using ECCentral.Portal.UI.MKT.Facades;

namespace ECCentral.Portal.UI.MKT.Views
{
    [View(IsSingleton = true, SingletonType = SingletonTypes.Url)]
    public partial class ECCCategoryManager : PageBase
    {
        ECCCategoryManagerVM model;
        private ECCCategoryManagerType catetype;
        public ECCCategoryManager()
        {
            InitializeComponent();
        }

        #region 初始化加载
        public override void OnPageLoad(object sender, EventArgs e)
        {
            base.OnPageLoad(sender, e);
            model = new ECCCategoryManagerVM();
            this.DataContext = model;

        }
        #endregion

        private void btnCategorySearch_Click(object sender, RoutedEventArgs e)
        {
            catetype = myConnection.ucECCategoryManagerType;
            switch (catetype)
            {
                case ECCCategoryManagerType.ECCCategoryType1:
                    dgCategoryQueryResult.Columns[1].Visibility = Visibility.Visible;
                    dgCategoryQueryResult.Columns[2].Visibility = Visibility.Collapsed;
                    dgCategoryQueryResult.Columns[3].Visibility = Visibility.Collapsed;
                    break;
                case ECCCategoryManagerType.ECCCategoryType2:
                    dgCategoryQueryResult.Columns[1].Visibility = Visibility.Visible;
                    dgCategoryQueryResult.Columns[2].Visibility = Visibility.Visible;
                    dgCategoryQueryResult.Columns[3].Visibility = Visibility.Collapsed;
                    break;
                case ECCCategoryManagerType.ECCCategoryType3:
                    dgCategoryQueryResult.Columns[1].Visibility = Visibility.Visible;
                    dgCategoryQueryResult.Columns[2].Visibility = Visibility.Visible;
                    dgCategoryQueryResult.Columns[3].Visibility = Visibility.Visible;
                    break;
                default:
                    dgCategoryQueryResult.Columns[1].Visibility = Visibility.Visible;
                    dgCategoryQueryResult.Columns[2].Visibility = Visibility.Visible;
                    dgCategoryQueryResult.Columns[3].Visibility = Visibility.Visible;
                    break;
            }
            dgCategoryQueryResult.Bind();
        }

        private void dgCategoryQueryResult_LoadingDataSource(object sender, Newegg.Oversea.Silverlight.Controls.Data.LoadingDataEventArgs e)
        {
            ECCategoryFacade facade = new ECCategoryFacade(this);
            facade.QueryECCCategory(model, e.PageSize, e.PageIndex, e.SortField, (obj, args) =>
            {
                this.dgCategoryQueryResult.ItemsSource = args.Result.Rows;
                this.dgCategoryQueryResult.TotalCount = args.Result.TotalCount;
            });
        }

        private void hyperlinkCategorySysNo_Click(object sender, RoutedEventArgs e)
        {
            dynamic d = this.dgCategoryQueryResult.SelectedItem as dynamic;
            ECCategoryManageEditMaintain item = new ECCategoryManageEditMaintain();
            item.Data = new ECCCategoryManagerVM()
            {
                Category1SysNo = d.Category1SysNo,
                Category2SysNo = d.Category2SysNo,
                CategoryName = d.CategoryName,
                Status = (ECCCategoryManagerStatus)d.CategoryStatus,
                SysNo = d.CategorySysNo,
                Type = catetype
            };
            item.Dialog = Window.ShowDialog("更新类别", item, (s, args) =>
            {
                if (args.DialogResult == Newegg.Oversea.Silverlight.Controls.Components.DialogResultType.OK)
                {
                    dgCategoryQueryResult.Bind();
                }
            }, new Size(600, 300));
        }
    }
}
