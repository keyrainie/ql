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
using ECCentral.Portal.Basic;
using ECCentral.Portal.UI.IM.Models;
using Newegg.Oversea.Silverlight.Controls.Components;
using ECCentral.Portal.UI.IM.Facades;
using ECCentral.BizEntity.IM;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using Newegg.Oversea.Silverlight.Utilities.Validation;


namespace ECCentral.Portal.UI.IM.UserControls
{
    public partial class CategoryEditMaintain : UserControl
    {
        
        private CategoryFacade facade;
        public bool IsEdit { get; set; }
        public IDialog Dialog { get; set; }
        public CategoryVM Data { get; set; }
        public CategoryEditMaintain()
        {
            InitializeComponent();

            this.Loaded += (sender, e) => 
            {
                facade = new CategoryFacade();
                if (IsEdit)
                {
                    this.myCategoryConnection.cbCategoryType.IsEnabled = false;
                    btnSave.Visibility = Visibility.Visible;
                }
                else
                {
                    Data = new CategoryVM();
                }
                this.myCategoryConnection.CategoryTypeChanged += myCategoryConnection_CategoryTypeChanged;
                this.myCategoryConnection.myCategory.LoadCategoryCompleted += (obj, arg) =>
                {
                    if (Data.Category2SysNo == 0)
                    {
                        Data.Category2SysNo = null;
                        Data.Category1SysNo = null;
                    }
                    this.DataContext = Data;
                    if (Data != null)
                    {
                        SetC3CodeControlVisibility(Data.Type);
                    }
                };
               
               
            };
        }

        void SetC3CodeControlVisibility(CategoryType categoryType)
        {
            if (categoryType == CategoryType.CategoryType3)
            {
                this.lblC3Code.Visibility = System.Windows.Visibility.Visible;
                this.txtC3Code.Visibility = System.Windows.Visibility.Visible;
            }
            else
            {
                this.lblC3Code.Visibility = System.Windows.Visibility.Collapsed;
                this.txtC3Code.Visibility = System.Windows.Visibility.Collapsed;
            }
        }

        void myCategoryConnection_CategoryTypeChanged(object sender, EventArgs e)
        {
            SetC3CodeControlVisibility(this.myCategoryConnection.CategoryType);
            //throw new NotImplementedException();
        }

        private void btnOk_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidationManager.Validate(this))
            {
                return;
            }
            CategoryRequestApprovalVM info= GetVM();
          

            if (IsEdit)
            {
                info.OperationType = OperationType.Update;
                facade.CreateCategoryRequest(info, (obj, arg) =>
                {
                    if (arg.FaultsHandle())
                    {
                        return;

                    }
                    CloseDialog(DialogResultType.OK);
                    CPApplication.Current.CurrentPage.Context.Window.Alert("提交成功");
                });
            }
            else
            {
                info.OperationType = OperationType.Create;
                facade.CreateCategoryRequest(info, (obj, arg) =>
                {
                    if (arg.FaultsHandle())
                    {
                        return;

                    }
                    CPApplication.Current.CurrentPage.Context.Window.Alert("提交成功");
                    CloseDialog(DialogResultType.OK);
                });
            }
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            CloseDialog(DialogResultType.Cancel);
        }
        private void CloseDialog(DialogResultType dialogResult)
        {
            if (Dialog != null)
            {
                Dialog.ResultArgs.DialogResult = dialogResult;
                Dialog.Close();
            }
        }

        /// <summary>
        /// 保存
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidationManager.Validate(this))
            {
                return;
            }
            CategoryRequestApprovalVM info = GetVM();
            facade.UpdateCategory(info, (obj, arg) => 
            {
                if (arg.FaultsHandle())
                {
                    return;
                }
                CPApplication.Current.CurrentPage.Context.Window.Alert("保存成功");
                CloseDialog(DialogResultType.OK);
            });
        }

        private CategoryRequestApprovalVM GetVM()
        {
            CategoryRequestApprovalVM info = new CategoryRequestApprovalVM()
            {
                CategoryStatus = (CategoryStatus)Data.Status,
                CategoryType = Data.Type,
                CategoryName = Data.CategoryName,
                Reansons = Data.Reansons,
                Status = 0,
                CategorySysNo = Data.SysNo

            };
            switch (myCategoryConnection.CategoryType)
            {
                case CategoryType.CategoryType1:
                    info.ParentSysNumber = 0;
                    break;
                case CategoryType.CategoryType2:
                    info.ParentSysNumber = Data.Category1SysNo;
                    break;
                case CategoryType.CategoryType3:
                    info.ParentSysNumber = Data.Category2SysNo;
                    info.C3Code = Data.C3Code;
                    break;
                default:
                    info.ParentSysNumber = 0;
                    break;
            }
            return info;
        }
    }
}
