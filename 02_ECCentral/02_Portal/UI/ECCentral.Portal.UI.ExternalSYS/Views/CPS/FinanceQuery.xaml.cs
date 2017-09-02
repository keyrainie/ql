using System;
using System.Windows;
using ECCentral.Portal.Basic;
using ECCentral.Portal.UI.ExternalSYS.Facades;
using ECCentral.Portal.UI.ExternalSYS.Models;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using Newegg.Oversea.Silverlight.Controls;
using Newegg.Oversea.Silverlight.Controls.Data;
using System.Collections.Generic;
using System.Windows.Controls;
using ECCentral.Portal.UI.ExternalSYS.UserControls;
using Newegg.Oversea.Silverlight.Utilities.Validation;
using Newegg.Oversea.Silverlight.Controls.Components;
using ECCentral.BizEntity.ExternalSYS;
using ECCentral.QueryFilter.Common;

namespace ECCentral.Portal.UI.ExternalSYS.Views
{
    [View(IsSingleton = true, SingletonType = SingletonTypes.Url)]
    public partial class FinanceQuery : PageBase
    {
        #region 属性
        FinanceQueryVM model;
        private FinanceFacade facade;
        private bool isLink = false;//是否连接到此页面 
        #endregion

        #region 初始化加载

        public FinanceQuery()
        {
            InitializeComponent();
            this.Loaded += new RoutedEventHandler(FinanceQuery_Loaded);
        }

        void FinanceQuery_Loaded(object sender, RoutedEventArgs e)
        {
            facade = new FinanceFacade();
            model = new FinanceQueryVM();

            if (!String.IsNullOrEmpty(Request.Param))
            {
                isLink = true;
                model.SysNoList = Request.Param;
                this.DataContext = model;
                this.dgFinanceQueryResult.Bind();
            }
            else
            {
                model.SysNoList = "";
            }
            
              this.DataContext = model;
            
        }

     

        #endregion

        #region 查询绑定
        private void btnFinanceSearch_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidationManager.Validate(this))
            {
                return;
            }
            isLink = false;
            model.SysNoList = "";
            dgFinanceQueryResult.Bind();
        }

        private void dgFinanceQueryResult_LoadingDataSource(object sender, LoadingDataEventArgs e)
        {
          
            model = (FinanceQueryVM)this.DataContext;

            PagingInfo p = new PagingInfo
            {
                PageIndex = e.PageIndex,
                PageSize = e.PageSize,
                SortBy = e.SortField
            };

            facade.GetAllFinance(model, p, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }
                if (isLink == true)
                {
                    string tempstr=string.Empty;
                    List<DateTime> list = new List<DateTime>();
                    foreach (var item in args.Result.Rows)
                    {
                        tempstr=item.CustomerID;
                        list.Add(item.SettledTime);
                    }
                    list.Sort();
                    if (list.Count > 0)
                    {
                        model.SettleDateFrom =DateTime.Parse(list[0].ToShortDateString());
                        model.SettleDateTo = DateTime.Parse(list[list.Count - 1].ToShortDateString());
                    }
                    model.CustomerID = tempstr;
                }
                this.dgFinanceQueryResult.ItemsSource = args.Result.Rows.ToList();
                this.dgFinanceQueryResult.TotalCount = args.Result.TotalCount;

            });
        }
        #endregion

        #region 跳转

     
        private void hyperlinkFinanceID_Click(object sender, RoutedEventArgs e)
        {
            HyperlinkButton link = (HyperlinkButton)sender;
            StackPanel sp = (StackPanel)((StackPanel)link.Parent).FindName("spAction");
            sp.Visibility = Visibility.Visible;
            link.Visibility = Visibility.Collapsed;
            FrameworkElement element = dgFinanceQueryResult.Columns[5].GetCellContent(this.dgFinanceQueryResult.SelectedItem);
            if (element.GetType() == typeof(TextBox))
            {
                ((TextBox)element).IsEnabled = true;
            }
        }       

        private void hyperlinkOrderSysNo_Click(object sender, RoutedEventArgs e)
        {
            HyperlinkButton link = (HyperlinkButton)sender;
            string str = "";
            if (link.Tag != null)
            {
                str = link.Tag.ToString();
                str = str.Substring(0, str.Length - 1);
            }
            Window.Navigate(string.Format(ConstValue.ExternalSYS_OrderManagement, str), null, true);
        }

        #endregion

        private void hlUpdate_Click(object sender, RoutedEventArgs e)
        {
            dynamic d = this.dgFinanceQueryResult.SelectedItem as dynamic;

            string ConfirmCommission = d.ConfirmCommissionAmt.ToString();
            decimal result;
            if (!decimal.TryParse(ConfirmCommission, out result))
            {
                Window.MessageBox.Show("请输入正确的结算金额!", MessageBoxType.Error);
                return;
            }
            FinanceVM vm = new FinanceVM() 
            {
                SysNo=d.SysNo,
                CommisonConfirmAmt = result
            };
            facade.UpdateCommisonConfirmAmt(vm, (obj, arg) => 
            {
                if (arg.FaultsHandle())
                {
                    return;
                }
                Window.MessageBox.Show("更新成功!", MessageBoxType.Success);
                HyperlinkButton link = (HyperlinkButton)sender;
                SetActionStyle(link);
            });
        }

        private void hlClose_Click(object sender, RoutedEventArgs e)
        {
            HyperlinkButton link = (HyperlinkButton)sender;
            SetActionStyle(link);
        }

        /// <summary>
        /// 设置操作的样式
        /// </summary>
        /// <param name="link"></param>
        private void SetActionStyle(HyperlinkButton link)
        {
            StackPanel sp = (StackPanel)link.Parent;
            sp.Visibility = Visibility.Collapsed;
            ((HyperlinkButton)((StackPanel)sp.Parent).FindName("HyperlinkFinanceID")).Visibility = Visibility.Visible;
            FrameworkElement element = dgFinanceQueryResult.Columns[5].GetCellContent(this.dgFinanceQueryResult.SelectedItem);
            if (element.GetType() == typeof(TextBox))
            {
                ((TextBox)element).IsEnabled = false;
            }
        }

    }
}
