using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

using ECCentral.Portal.Basic.Utilities;
using ECCentral.Portal.UI.IM.Facades;
using ECCentral.Portal.UI.IM.Models;
using ECCentral.Portal.UI.IM.UserControls;

using Newegg.Oversea.Silverlight.ControlPanel.Core;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using Newegg.Oversea.Silverlight.Controls;
using Newegg.Oversea.Silverlight.Controls.Components;
using System;
using System.Windows.Documents;
using System.Windows.Media;
using ECCentral.Portal.UI.IM.Views.ProductLine;

namespace ECCentral.Portal.UI.IM.UserControls
{
    public partial class UCProductLineList : UserControl, IListControl<ProductLineVM>
    {
        private ProductLineManagement Page
        {
            get
            {
                return CPApplication.Current.CurrentPage as ProductLineManagement;
            }
        }

        public UCProductLineList()
        {
            InitializeComponent();
        }
        private static string FilterPMName { get; set; }
        private void dataProductDomainList_LoadingDataSource(object sender, Newegg.Oversea.Silverlight.Controls.Data.LoadingDataEventArgs e)
        {
            var filter = this.dataProductLineList.QueryCriteria as ProductLineQueryVM;
            new ProductLineFacade(Page).QueryProductLine(filter, e.PageSize, e.PageIndex, e.SortField, (obj, args) =>
            {
                FilterPMName = filter.PMUserName;
                this.dataProductLineList.ItemsSource = args.Result.Rows.ToList("IsChecked", false);
                this.dataProductLineList.TotalCount = args.Result.TotalCount;
            });
        }

        private void ckbSelectAllRow_Click(object sender, RoutedEventArgs e)
        {
            CheckBox chk = (CheckBox)sender;
            dynamic rows = this.dataProductLineList.ItemsSource;
            if (rows != null)
            {
                foreach (dynamic row in rows)
                {
                    row.IsChecked = chk.IsChecked.Value;
                }
            }
        }

        public void BindData(object filter)
        {
            this.dataProductLineList.QueryCriteria = filter;
            this.dataProductLineList.Bind();
        }

        private void btnEdit_Click(object sender, RoutedEventArgs e)
        {
            HyperlinkButton btn = sender as HyperlinkButton;
            dynamic d = btn.DataContext as dynamic;
            ProductLineVM vm = DynamicConverter<ProductLineVM>.ConvertToVM(d);

            vm.BrandEnabled = false;
            vm.CategoryEnabled = false;

            UCProductLineDetail uc = new UCProductLineDetail(vm);

            IDialog dialog = CPApplication.Current.CurrentPage.Context.Window.ShowDialog("修改产品线信息", uc, (obj, args) =>
            {
                if (args.DialogResult == DialogResultType.OK)
                {
                    this.BindData(this.dataProductLineList.QueryCriteria);
                }
            });
            uc.Dialog = dialog;
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            dynamic d = (sender as HyperlinkButton).DataContext;
            CPApplication.Current.CurrentPage.Context.Window.Confirm("确定要删除吗?", (obj, args) =>
            {
                if (args.DialogResult == DialogResultType.OK)
                {
                    int sysNo = d.SysNo;
                    new ProductLineFacade(this.Page).Delete(sysNo, (o, a) =>
                    {
                        var list = this.dataProductLineList.ItemsSource as List<dynamic>;
                        list.RemoveAll(p => p.SysNo == d.SysNo);
                        this.dataProductLineList.ItemsSource = list;

                        this.Page.Window.MessageBox.Show("操作成功!", MessageBoxType.Success);
                    });
                }
            });
        }

        /// <summary>
        /// 数据全部导出
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void QueryResultGrid_ExportAllClick(object sender, EventArgs e)
        {
            var filter = this.dataProductLineList.QueryCriteria as ProductLineQueryVM;
            if (filter == null || dataProductLineList.TotalCount < 1)
            {
                Page.Context.Window.MessageBox.Show("导出失败，请先查询需要导出的结果集。");
                return;
            }
            ColumnSet col = new ColumnSet(this.dataProductLineList);
            ProductLineFacade facade = new ProductLineFacade();
            facade.ExportExcelFile(filter, new ColumnSet[] { col });
        }

        public List<ProductLineVM> GetSelectedSysNoList()
        {
            List<ProductLineVM> list = new List<ProductLineVM>();
            dynamic rows = this.dataProductLineList.ItemsSource;
            if (rows != null)
            {
                foreach (dynamic row in rows)
                {
                    if (row.IsChecked)
                    {
                        ProductLineVM vm = new ProductLineVM
                        {
                            C2SysNo = row.C2SysNo,
                            C1SysNo = row.C1SysNo,
                            SysNo = row.SysNo
                        };
                        list.Add(vm);
                    }
                }
            }
            return list;
        }

        #region 高亮显示备份PM中的PM
        public static string GetStyleText(TextBlock wb)
        {
            return wb.GetValue(TextProperty) as string;
        }
        public static void SetStyleText(TextBlock wb, string text)
        {
            wb.SetValue(TextProperty, text);
        }

        public static readonly DependencyProperty TextProperty =
        DependencyProperty.RegisterAttached("StyleText", typeof(string), typeof(UCProductLineList), new PropertyMetadata("", OnTextChanged));
        private static void OnTextChanged(DependencyObject depObj, DependencyPropertyChangedEventArgs e)
        {
            var txtBox = depObj as TextBlock;
            string bakpms = string.Format("{0}", e.NewValue);
            if (depObj is TextBlock && bakpms.Contains(FilterPMName))
            {
                txtBox.Inlines.Clear();
                Run content = new Run();
                content.Text = FilterPMName;
                content.Foreground = new SolidColorBrush(Colors.Red);

                string[] datas = bakpms.Split(new string[] { FilterPMName }, StringSplitOptions.None);
                for (int i = 0; i < datas.Length; i++)
                {
                    Run rc = new Run();
                    rc.Text = datas[i];
                    txtBox.Inlines.Add(rc);
                    if(i<1)
                    {
                        txtBox.Inlines.Add(content);
                    }
                }
            }
            else
            {
                txtBox.Text = bakpms;
            }
        }
        #endregion
    }
}
