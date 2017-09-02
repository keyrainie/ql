using System;
using System.Windows;
using ECCentral.Portal.Basic;
using ECCentral.Portal.UI.IM.Facades;
using ECCentral.Portal.UI.IM.Models;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using Newegg.Oversea.Silverlight.Controls;
using Newegg.Oversea.Silverlight.Controls.Data;
using System.Collections.Generic;
using System.Windows.Controls;
using ECCentral.Portal.UI.IM.UserControls;
using Newegg.Oversea.Silverlight.Utilities.Validation;
using Newegg.Oversea.Silverlight.Controls.Components;
using ECCentral.BizEntity.IM;
using ECCentral.Portal.Basic.Components.UserControls.Language;

namespace ECCentral.Portal.UI.IM.Views
{
    [View(IsSingleton = true, SingletonType = SingletonTypes.Url)]
    public partial class BrandQuery : PageBase
    {
        #region 属性
        BrandQueryVM model;
        int? manufactureSysNo;
        bool IsSetTop = false;
        #endregion

        #region 初始化加载

        public BrandQuery()
        {
            InitializeComponent();
          
        }

        public override void OnPageLoad(object sender, EventArgs e)
        {
            base.OnPageLoad(sender, e);
            model = new BrandQueryVM();
            this.DataContext = model;
            if (!String.IsNullOrEmpty(Request.Param))
            {
                manufactureSysNo = Convert.ToInt32(Request.Param);
                dgBrandQueryResult.Bind();
            }
        }

        #endregion

        #region 查询绑定
        private void btnBrandSearch_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidationManager.Validate(this))
            {
                return;
            }
            if ((model.Category1SysNo>0 &&(model.Category2SysNo == 0 || model.Category2SysNo == null))||(model.AuthorizedStatus!=null&&(model.Category1SysNo==0||model.Category1SysNo==null)))
            {
                Window.MessageBox.Show("查询至少指定二级类别", MessageBoxType.Warning);
                return;
            }
            dgBrandQueryResult.Bind();
        }

        private void dgBrandQueryResult_LoadingDataSource(object sender, LoadingDataEventArgs e)
        {
            BrandQueryFacade facade = new BrandQueryFacade(this);
            model = (BrandQueryVM)this.DataContext;
            model.ManufacturerSysNo = manufactureSysNo;
            if (IsSetTop)
            {
                e.SortField = "case when Brand.Priority is null then 1 else 0 end,Brand.Priority";
            }
            IsSetTop = false;
            facade.QueryBrand(model, e.PageSize, e.PageIndex, e.SortField, (obj, args) =>
            {
                this.dgBrandQueryResult.ItemsSource = args.Result.Rows.ToList("IsChecked", false); ;
                this.dgBrandQueryResult.TotalCount = args.Result.TotalCount;
            });
        }
        #endregion

        #region 页面内按钮处理事件

        #region 界面事件
        private void ckbSelectAllRow_Click(object sender, RoutedEventArgs e)
        {
            CheckBox cb = sender as CheckBox;
            if (cb != null)
            {
                dynamic viewlist = this.dgBrandQueryResult.ItemsSource as dynamic;
                if (viewlist != null)
                {
                    foreach (var item in viewlist)
                    {
                        item.IsChecked = cb.IsChecked != null ? cb.IsChecked.Value : false;
                    }
                }
            }
        }
        #endregion

        #endregion

        #region 跳转

        //新建生产商
        private void btnBrandNew_Click(object sender, RoutedEventArgs e)
        {
            BrandRequestMaintain item = new BrandRequestMaintain();
            item.Action = BrandAction.Add;
            item.Dialog = Window.ShowDialog("添加品牌", item, (s, args) =>
            {
                if (args.DialogResult == Newegg.Oversea.Silverlight.Controls.Components.DialogResultType.OK)
                {
                    this.dgBrandQueryResult.Bind();
                }
            }, new Size(650, 600));
            //Window.Navigate(ConstValue.IM_BrandMaintainCreateFormat, null, true);
        }

        //查看生产商
        private void hyperlinkBrandID_Click(object sender, RoutedEventArgs e)
        {
            dynamic manufacturer = this.dgBrandQueryResult.SelectedItem as dynamic;
            BrandRequestMaintain item = new BrandRequestMaintain();
            item.Action = BrandAction.Edit;
            item.BrandSysNo = manufacturer.SysNo;
            item.Dialog = Window.ShowDialog("更新品牌", item, (s, args) =>
            {
                if (args.DialogResult == Newegg.Oversea.Silverlight.Controls.Components.DialogResultType.OK)
                {
                    this.dgBrandQueryResult.Bind();
                }
            }, new Size(750, 600));


            //if (manufacturer != null)
            //{
            //    this.Window.Navigate(string.Format(ConstValue.IM_BrandMaintainUrlFormat, manufacturer.SysNo), null, true);
            //}
        }

        //更新品牌多语言信息
        private void hyperlinkBrandIDLang_Click(object sender, RoutedEventArgs e)
        {
            dynamic manufacturer = this.dgBrandQueryResult.SelectedItem as dynamic;

            UCMultiLanguageMaintain item = new UCMultiLanguageMaintain(manufacturer.SysNo,"Brand");

            item.Dialog = Window.ShowDialog("更新品牌多语言", item, (s, args) =>
            {
                if (args.DialogResult == Newegg.Oversea.Silverlight.Controls.Components.DialogResultType.OK)
                {
                    this.dgBrandQueryResult.Bind();
                }
            }, new Size(750, 600));


            //if (manufacturer != null)
            //{
            //    this.Window.Navigate(string.Format(ConstValue.IM_BrandMaintainUrlFormat, manufacturer.SysNo), null, true);
            //}
        }
        #endregion

        private void btnTop_Click(object sender, RoutedEventArgs e)
        {
            BrandQueryFacade facade = new BrandQueryFacade();
            List<string> list = new List<string>();

            dynamic viewlist = this.dgBrandQueryResult.ItemsSource as dynamic;
            if (viewlist != null)
            {
                foreach (var item in viewlist)
                {
                    if (item.IsChecked == true)
                    {
                        list.Add(item.SysNo.ToString());
                    }
                }
            }
            if (list.Count > 0)
            {
                facade.SetTopBrands(list, (obj, arg) =>
                {
                    if (arg.FaultsHandle())
                    {
                        return;
                    }

                    Window.Alert("置顶成功！");
                    dynamic d = this.dgBrandQueryResult.ItemsSource as dynamic;
                    if (viewlist != null)
                    {
                        foreach (var item in d)
                        {
                            item.IsChecked = false;
                        }
                    }
                    IsSetTop = true;
                    this.dgBrandQueryResult.Bind();
                });
            }
            else
            {
                Window.Alert("请先选择！");
            }
        }

    }
}
