using System;
using System.Windows;
using ECCentral.Portal.Basic;
using ECCentral.Portal.UI.IM.Facades;
using ECCentral.Portal.UI.IM.Models;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using Newegg.Oversea.Silverlight.Controls;
using Newegg.Oversea.Silverlight.Controls.Data;
using ECCentral.Portal.UI.IM.UserControls;
using System.Windows.Controls;
using System.Collections.Generic;
using Newegg.Oversea.Silverlight.Controls.Components;
using ECCentral.BizEntity.IM;
using ECCentral.Portal.UI.IM.Views;
using ECCentral.Portal.UI.IM.Models;
using System.Windows.Data;

namespace ECCentral.Portal.UI.IM.Views
{
    [View(IsSingleton = true, SingletonType = SingletonTypes.Url)]
    public partial class CategoryRequestApproval : PageBase
    {
        private CategoryRequestApprovalQueryVM model;
        private CategoryType cateType;
        public CategoryRequestApproval()
        {
            InitializeComponent();

            this.CategoryRequestApprovalResult.LoadingDataSource += new EventHandler<LoadingDataEventArgs>(CategoryRequestApprovalResult_LoadingDataSource);
        }

        void CategoryRequestApprovalResult_LoadingDataSource(object sender, LoadingDataEventArgs e)
        {   

            CategoryRequestApprovalFacade facade = new CategoryRequestApprovalFacade();
            facade.GetCategoryRequestApprovalList(model, e.PageSize, e.PageIndex, e.SortField, (obj, arg) =>
            {
                this.CategoryRequestApprovalResult.ItemsSource = arg.Result.Rows;
                this.CategoryRequestApprovalResult.TotalCount = arg.Result.TotalCount;

            });
        }
        public override void OnPageLoad(object sender, EventArgs e)
        {
            base.OnPageLoad(sender, e);
            model = new CategoryRequestApprovalQueryVM();
            this.DataContext = model;
        }

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            if ((int)cboCategoryTypeList.SelectedValue == 1)
            {
                cateType = CategoryType.CategoryType1;
                CategoryRequestApprovalResult.Columns[2].Visibility = Visibility.Visible;
                CategoryRequestApprovalResult.Columns[3].Visibility = Visibility.Collapsed;
                CategoryRequestApprovalResult.Columns[4].Visibility = Visibility.Collapsed;
            }
            if ((int)cboCategoryTypeList.SelectedValue == 2)
            {
                cateType = CategoryType.CategoryType2;
                CategoryRequestApprovalResult.Columns[2].Visibility = Visibility.Visible;
                CategoryRequestApprovalResult.Columns[3].Visibility = Visibility.Visible;
                CategoryRequestApprovalResult.Columns[4].Visibility = Visibility.Collapsed;
            }
            if ((int)cboCategoryTypeList.SelectedValue == 3)
            {
                cateType = CategoryType.CategoryType3;
                CategoryRequestApprovalResult.Columns[2].Visibility = Visibility.Visible;
                CategoryRequestApprovalResult.Columns[3].Visibility = Visibility.Visible;
                CategoryRequestApprovalResult.Columns[4].Visibility = Visibility.Visible;
            }
            this.CategoryRequestApprovalResult.Bind();
        }

        private void HyperlinkButton_Click(object sender, RoutedEventArgs e)
        {
            dynamic d = this.CategoryRequestApprovalResult.SelectedItem as dynamic;
            CategoryStatus originalStatus;
            CategoryStatus categoryStatus;
            if (d.OriginalStatus == 0)
            {
                originalStatus = CategoryStatus.Active;
            }
            else
            {
                originalStatus = CategoryStatus.DeActive;
            }
            if (d.CategoryStatus == 0)
            {
                categoryStatus = CategoryStatus.Active;
            }
            else
            {
                categoryStatus = CategoryStatus.DeActive;
            }
            CategoryRequestApprovalVM data;
            if ((int)cboCategoryTypeList.SelectedValue == 1)
            {
                data = new CategoryRequestApprovalVM() { Category1Name = d.Category1Name, CategoryName = d.Category1Name, OperationType = (OperationType)d.OperationType, OriginalCategory1Name = d.OriginalCategory1Name, Status = d.Status, Reansons = d.Reasons, SysNo = d.SysNo, CategorySysNo = d.CategorySysNo, CategoryType = CategoryType.CategoryType1, ParentSysNumber = 0, OriginalStatus = originalStatus, CategoryStatus = categoryStatus };
            }
            else if ((int)cboCategoryTypeList.SelectedValue == 2)
            {
                data = new CategoryRequestApprovalVM() { Category1Name = d.Category1Name, CategoryName = d.Category2Name, Category2Name = d.Category2Name, OperationType = (OperationType)d.OperationType, OriginalCategory1Name = d.OriginalCategory1Name, OriginalCategory2Name = d.OriginalCategory2Name, Status = d.Status, Reansons = d.Reasons, SysNo = d.SysNo, CategorySysNo = d.CategorySysNo, CategoryType = CategoryType.CategoryType2, ParentSysNumber = d.Category1SysNo, OriginalStatus = originalStatus, CategoryStatus = categoryStatus };
            }
            else if ((int)cboCategoryTypeList.SelectedValue == 3)
            {
                data = new CategoryRequestApprovalVM() { Category1Name = d.Category1Name, CategoryName = d.Category3Name, Category2Name = d.Category2Name, Category3Name = d.Category3Name, OperationType = (OperationType)d.OperationType, OriginalCategory1Name = d.OriginalCategory1Name, OriginalCategory2Name = d.OriginalCategory2Name, OriginalCategory3Name = d.OriginalCategory3Name, Status = d.Status, Reansons = d.Reasons, SysNo = d.SysNo, CategorySysNo = d.CategorySysNo, CategoryType = CategoryType.CategoryType3, ParentSysNumber = d.Category2SysNo, OriginalStatus = originalStatus, CategoryStatus = categoryStatus, C3Code = d.C3Code, OriginalC3Code = d.OriginalC3Code };
            }
            else
            {
                data = new CategoryRequestApprovalVM();
            }
            CategoryRequestApprovalMaintain item = new CategoryRequestApprovalMaintain();
            item.ActionType = data.OperationType;
            item.Category = cateType;
            item.Data = data;
            item.Dialog = Window.ShowDialog("类别审核", item, (s, args) =>
            {
                if (args.DialogResult == Newegg.Oversea.Silverlight.Controls.Components.DialogResultType.OK)
                {
                    this.CategoryRequestApprovalResult.Bind();
                }
            }, new Size(650, 400));
        }



    }
    public class RequestConvert : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            string result="";
            if (parameter.ToString() == "Category")
            {
                if ((int)value == 0)
                {
                    result= "申请状态";
                }
               
            }
            if (parameter.ToString() == "Manufacturer")
            {
                if ((int)value == 0)
                {
                    result = "待审核";
                }
                
            }
            if (parameter.ToString() == "Brand")
            {
                if (value != null)
                {
                    if (value.ToString().ToLower() == "o")
                    {
                        result = "待审核";
                    }
                }

            }
            return result;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
