using System;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using ECCentral.BizEntity.Enum.Resources;
using ECCentral.Portal.Basic;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.Portal.UI.Inventory.Facades;
using ECCentral.Portal.UI.Inventory.Models;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using Newegg.Oversea.Silverlight.Controls;
using Newegg.Oversea.Silverlight.Controls.Data;
using Newegg.Oversea.Silverlight.Utilities.Validation;

namespace ECCentral.Portal.UI.Inventory.Views
{
    /// <summary>
    /// 商品库龄报表
    /// </summary>
    [View(IsSingleton = true, SingletonType = SingletonTypes.Page)]
    public partial class StockAgeReport : PageBase
    {
        private StockAgeReportQueryVM PageQueryView;
        private InventoryQueryFacade PageQueryFacade;

        public StockAgeReport()
        {
            InitializeComponent();
        }

        public override void OnPageLoad(object sender, EventArgs e)
        {
            base.OnPageLoad(sender, e);

            PageQueryFacade = new InventoryQueryFacade(this);
            PageQueryView = new StockAgeReportQueryVM();
            this.SearchBuilder.DataContext = PageQueryView;
            this.dpStatisticDate.SelectedDate = DateTime.Today.AddDays(-1);

            CodeNamePairHelper.GetList(ConstValue.DomainName_Inventory, "StockAge", CodeNamePairAppendItemType.All, (_, args) =>
            {
                PageQueryView.StockAgeTypeList = args.Result.Convert<CodeNamePair, SelectionOptionVM>((s, t) =>
                {
                    t.Text = s.Name; t.Value = s.Code; t.Selected = true;
                });

                if (PageQueryView.StockAgeTypeList != null && PageQueryView.StockAgeTypeList.Count > 0)
                {
                    PageQueryView.StockAgeTypeList.ForEach(item =>
                    {
                        item.PropertyChanged += item_PropertyChanged;
                    });
                }
                this.SetSelectionText();
            });
        }

        private bool isSkipUpdateStockOptionAll;

        private void item_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            this.ComboBox_StockAgeList.SelectedItem = null;
            if (e.PropertyName == "Selected")
            {
                var optionAll = PageQueryView.StockAgeTypeList.FirstOrDefault(x => x.Value == null);
                if (optionAll == null)
                    return;

                if (((SelectionOptionVM)sender).Value == null)
                {
                    if (isSkipUpdateStockOptionAll)
                        return;

                    PageQueryView.StockAgeTypeList.Where(obj => obj.Text != ((SelectionOptionVM)sender).Text)
                        .ToList().ForEach(obj => obj.Selected = ((SelectionOptionVM)sender).Selected);
                }
                else
                {
                    if (((SelectionOptionVM)sender).Selected)
                    {
                        if (PageQueryView.StockAgeTypeList.Count(obj => obj.Value != null && !obj.Selected) == 0 && !optionAll.Selected)
                            optionAll.Selected = true;
                    }
                    else
                    {
                        if (optionAll.Selected)
                        {
                            isSkipUpdateStockOptionAll = true;
                            optionAll.Selected = false;
                            isSkipUpdateStockOptionAll = false;
                        }
                    }
                }

                this.SetSelectionText();
            }
        }

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidationManager.Validate(this.SearchBuilder))
            {
                return;
            }
            if (!PageQueryView.StatisticDate.HasValue)
            {
                this.dpStatisticDate.Validation("统计日期不能为空");
                this.dpStatisticDate.Focus();
                return;
            }
            if (PageQueryView.StockAgeTypeList.Count(x => x.Value != null && x.Selected) <= 0)
            {
                this.ComboBox_StockAgeList.Validation("请至少选择一种库龄时间类型");
                this.ComboBox_StockAgeList.Focus();
                return;
            }
            this.dpStatisticDate.ClearValidationError();
            this.ComboBox_StockAgeList.ClearValidationError();

            this.QueryResultGrid.Bind();
        }

        private void QueryResultGrid_LoadingDataSource(object sender, LoadingDataEventArgs e)
        {
            PageQueryFacade.QueryStockAgeReport(PageQueryView, e.PageIndex, e.PageSize, e.SortField, (obj, args) =>
             {
                 if (args.Result != null && args.Result.Rows != null)
                 {
                     this.QueryResultGrid.ItemsSource = args.Result.Rows;
                     this.QueryResultGrid.TotalCount = args.Result.TotalCount;
                 }
             });
        }

        private void QueryResultGrid_ExportAllClick(object sender, EventArgs e)
        {
            if (this.QueryResultGrid == null || this.QueryResultGrid.TotalCount == 0)
            {
                Window.Alert("没有可供导出的数据!");
                return;
            }
            //导出全部:
            if (null != PageQueryView)
            {
                ColumnSet columnSet = new ColumnSet(QueryResultGrid);
                PageQueryFacade.ExportExcelForStockAgeReport(PageQueryView, new ColumnSet[] { columnSet });
            }
        }

        private bool preOperIsOpen = false;

        private void ComboBox_StockAgeList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            this.ComboBox_StockAgeList.SelectedItem = null;
            if (e.AddedItems != null && e.AddedItems.Count > 0)
            {
                SelectionOptionVM item = (SelectionOptionVM)e.AddedItems[0];
                item.Selected = !item.Selected;
            }
        }

        private void TextBlock_StockAgeComboBoxText_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (preOperIsOpen)
            {
                this.ComboBox_StockAgeList.IsDropDownOpen = false;
                preOperIsOpen = false;
            }
        }

        private void TextBlock_StockAgeComboBoxText_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (!preOperIsOpen)
            {
                this.ComboBox_StockAgeList.IsDropDownOpen = true;
                preOperIsOpen = true;
            }
        }

        private void SetSelectionText()
        {
            string selectionString = "";
            int selectedItemCount = 0;
            foreach (SelectionOptionVM item in PageQueryView.StockAgeTypeList)
            {
                if (item.Selected)
                {
                    selectedItemCount++;
                    selectionString = selectionString + item.Text + ",";
                }
            }
            if (selectionString != "" && selectedItemCount > 0)
            {
                selectionString = selectionString.Substring(0, selectionString.Length - 1);
            }
            else
            {
                selectionString = ResCommonEnum.Enum_Select;
            }
            if (selectedItemCount == PageQueryView.StockAgeTypeList.Count)
            {
                selectionString = ResCommonEnum.Enum_All;
            }
            this.TextBlock_StockAgeComboBoxText.Text = selectionString;

            Size tbSelectedTextArea = new Size(this.TextBlock_StockAgeComboBoxText.ActualWidth, this.TextBlock_StockAgeComboBoxText.ActualHeight);
            this.TextBlock_StockAgeComboBoxText.Measure(tbSelectedTextArea);

            if (tbSelectedTextArea.Width > this.ComboBox_StockAgeList.ActualWidth - 20)
            {
                this.TextBlock_StockAgeComboBoxText.Text = string.Format("已选择{0}项", selectedItemCount);
            }
        }
    }
}