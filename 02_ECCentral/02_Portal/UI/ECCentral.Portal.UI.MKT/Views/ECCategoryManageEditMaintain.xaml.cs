using ECCentral.Portal.UI.MKT.Facades;
using ECCentral.Portal.UI.MKT.Models.ECCategory;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using Newegg.Oversea.Silverlight.Controls;
using Newegg.Oversea.Silverlight.Controls.Components;
using Newegg.Oversea.Silverlight.Utilities.Validation;
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

namespace ECCentral.Portal.UI.MKT.Views
{
    public partial class ECCategoryManageEditMaintain : UserControl
    {
        public IDialog Dialog { get; set; }
        private ECCategoryFacade facade;
        public ECCCategoryManagerVM Data { get; set; }
        public IPage Curpage;
        public ECCategoryManageEditMaintain()
        {
            InitializeComponent();
            Curpage = CPApplication.Current.CurrentPage;
            loaded();
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidationManager.Validate(this))
            {
                return;
            }
            facade = new ECCategoryFacade(Curpage);
            
            facade.UpdateECCCategory(Data, (obj, arg) =>
                {
                    if (arg.FaultsHandle())
                    {
 
                        return;
                    }
                    CPApplication.Current.CurrentPage.Context.Window.Alert("保存成功");
                    CloseDialog(DialogResultType.OK);
                });
            
        }
        /// <summary>
        /// 加载
        /// </summary>
        private void loaded()
        {
            this.Loaded += (sender, e) =>
                {
                    this.myCategoryConnection.cbCategoryType.IsEnabled = false;
                    btnSave.Visibility = Visibility.Visible;
                    this.myCategoryConnection.myCategory.LoadCategoryCompleted += (obj, arg) =>
                    {
                        if (Data.Category2SysNo == 0)
                        {
                            Data.Category2SysNo = null;
                            Data.Category1SysNo = null;
                        }
                        this.DataContext = Data;
                    };
                };
        }
        /// <summary>
        /// 关闭窗口
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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

        ///// <summary>
        ///// 获取表单内容
        ///// </summary>
        ///// <returns></returns>
        //private ECCCategoryManagerVM GetVM()
        //{
        //    ECCCategoryManagerVM info = new ECCCategoryManagerVM()
        //    {
        //        CategoryStatus = (CategoryStatus)Data.Status,
        //        CategoryType = Data.Type,
        //        CategoryName = Data.CategoryName,
        //        Status = 0,
        //        CategorySysNo = Data.SysNo

        //    };
        //    switch (myCategoryConnection.CategoryType)
        //    {
        //        case CategoryType.CategoryType1:
        //            info.ParentSysNumber = 0;
        //            break;
        //        case CategoryType.CategoryType2:
        //            info.ParentSysNumber = Data.Category1SysNo;
        //            break;
        //        case CategoryType.CategoryType3:
        //            info.ParentSysNumber = Data.Category2SysNo;
        //            info.C3Code = Data.C3Code;
        //            break;
        //        default:
        //            info.ParentSysNumber = 0;
        //            break;
        //    }
        //    return info;
        //}
    }
}
