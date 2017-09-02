using System.Linq;
using System.Windows;
using System.Windows.Controls;
using ECCentral.BizEntity.IM;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.Portal.UI.IM.Facades;
using ECCentral.Portal.UI.IM.Models.Category;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using Newegg.Oversea.Silverlight.Controls.Components;
using System.Collections.Generic;

namespace ECCentral.Portal.UI.IM.UserControls
{
    public partial class CategoryKPIDetail : UserControl
    {
        #region 属性以及字段
        public IDialog Dialog { get; set; }
        public int SysNo
        {
            get;
            set;
        }
        /// <summary>
        /// 类型
        /// </summary>
        public CategoryType type { get; set; }
        /// <summary>
        /// 类别3的list 
        /// </summary>
        public List<CategoryInfo> Category3List { get; set; }

        /// <summary>
        /// 类别2List
        /// </summary>
        public List<CategoryInfo> Category2List { get; set; }
        public CategoryKPIDetail()
        {
            InitializeComponent();
            //切换他不Item显示不同的操作
            this.myTable.SelectionChanged += (sender, e) => 
            {
                if (((TabControl)sender).SelectedItem == tab2 && type == CategoryType.CategoryType2)
                {
                    BtnSaveByCategory.Visibility = Visibility.Visible;
                }
                else
                {
                    BtnSaveByCategory.Visibility = Visibility.Collapsed;
                }
            };
                
            
        }

      
        #endregion

        #region 页面加载
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            tb_Status.IsEnabled = false;
            tb_C3Name.IsEnabled = false;
            tb_C2Name.IsEnabled = false;
            tb_C1Name.IsEnabled = false;
            if (type == CategoryType.CategoryType3)
            {
                txtLabel_C3Name.Visibility = Visibility.Visible;
                TabControl_Header_RMA.Visibility = Visibility.Visible;
            }
            BindPage();
        }

        private void BindPage()
        {

            if (SysNo > 0)
            {
               
                var _facade = new CategoryKPIFacade();
                _facade.GetCategorySettingBySysNo(SysNo,type, (obj, args) =>
                {
                    if (args.FaultsHandle())
                    {
                        return;
                    }
                    if (args.Result == null)
                    {
                        CPApplication.Current.CurrentPage.Context.Window.MessageBox.Show("没有获得品牌信息.", MessageBoxType.Warning);
                        return;
                    }
                    var vm = args.Result.Convert<CategorySetting, CategroyKPIDetailVM>();
                    if (args.Result.CategoryMinMarginInfo.Margin!=null)
                    {
                        var margin = (from c in args.Result.CategoryMinMarginInfo.Margin
                                      select new { c.Key, MinMarginKpivm = c.Value.Convert<MinMarginKPI, MinMarginKPIVM>() })
                                .ToDictionary(e => e.Key, e => e.MinMarginKpivm);
                        vm.CategoryMinMarginInfo.Margin = margin;
                    }                   
                    vm.CategoryBasicInfo.CategorySysNo = SysNo;
                    vm.CategoryMinMarginInfo.CategorySysNo = SysNo;
                    vm.CategoryMinMarginInfo.InitMargin();
                    if (type == CategoryType.CategoryType3)
                    {
                        vm.CategoryRMAInfo.CategorySysNo = SysNo;
                        ucCategoryKPIRMAInfo.Bind(vm.CategoryRMAInfo);
                    }
                    DataContext = vm;
                    ucCategoryKPIBasicInfo.Type = type;
                    ucCategoryKPIBasicInfo.Bind(vm.CategoryBasicInfo);
                    ucCategoryKPIMinMargin.Type = type;
                    ucCategoryKPIMinMargin.Bind(vm.CategoryMinMarginInfo);
                  
                });
            }
            else
            {
                CPApplication.Current.CurrentPage.Context.Window.MessageBox.Show("无法和获取三级类指标信息", MessageBoxType.Warning);
                return;
            }
        }
        #endregion

        #region 页面事件

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (ucCategoryKPIBasicInfo.Focus())
            {
                ucCategoryKPIBasicInfo.Save();
            }
            else if (ucCategoryKPIMinMargin.Focus())
            {
                ucCategoryKPIMinMargin.Save();
            }
            else
            {
                ucCategoryKPIRMAInfo.Save();
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
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

        #endregion

        private void BtnSaveByCategory_Click(object sender, RoutedEventArgs e)
        {
            CPApplication.Current.CurrentPage.Context.Window.Confirm("是否保存所有所属的三级类?", (obj, arg) =>
            {
                if (arg.DialogResult == DialogResultType.OK)
                {
                    //该类别下的所有三级类别
                    var categorylist = (from p in Category3List where p.ParentSysNumber == SysNo select p).ToList();
                    if (categorylist.Count > 0)
                    {
                        ucCategoryKPIMinMargin.SaveCategoryByType(categorylist, CategoryType.CategoryType3);
                    }
                    else
                    {
                        CPApplication.Current.CurrentPage.Context.Window.MessageBox.Show("该二级类下没有三级类!", MessageBoxType.Warning);
                    }
                }
            });
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            CPApplication.Current.CurrentPage.Context.Window.Confirm("是否保存同属一级类别的其他二级类别?", (obj, arg) =>
            {
                if (arg.DialogResult == DialogResultType.OK)
                {

                    //该类别的Category1的SysNo
                    var parentSysNumber = (from c in Category2List where c.SysNo == SysNo && c.SysNo != null select c.ParentSysNumber).Take(1).FirstOrDefault();

                    //同属一级类别的其他二级类别
                    var categorylist = (from p in Category2List where p.ParentSysNumber == parentSysNumber && p.SysNo != SysNo && p.SysNo != null select p).ToList();
                    if (categorylist.Count > 0)
                    {
                        ucCategoryKPIMinMargin.SaveCategoryByType(categorylist, CategoryType.CategoryType2);
                    }
                    else
                    {
                        CPApplication.Current.CurrentPage.Context.Window.MessageBox.Show("该一级类下没有其他类别!", MessageBoxType.Warning);
                    }
                }
            });
        }

    }
}
