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
using ECCentral.Portal.UI.ExternalSYS.Facades;
using ECCentral.Portal.UI.ExternalSYS.Models;
using Newegg.Oversea.Silverlight.Controls.Data;
using Newegg.Oversea.Silverlight.Controls.Components;
using ECCentral.Portal.UI.ExternalSYS.UserControls;
using ECCentral.Portal.Basic;

namespace ECCentral.Portal.UI.ExternalSYS.Views
{
    [View(IsSingleton = true, SingletonType = SingletonTypes.Url)]
    public partial class CommissionToCashManagerment : PageBase
    {
        private CommissionToCashQueryVM model;
        private CommissionToCashFacade facade;
        public CommissionToCashManagerment()
        {
            InitializeComponent();
            this.CommissionResult.LoadingDataSource += new EventHandler<LoadingDataEventArgs>(CommissionResult_LoadingDataSource);
            this.Loaded += (sender, e) => 
            {
                facade = new CommissionToCashFacade();
                model = new CommissionToCashQueryVM();
                this.DataContext = model;
            };
        }

        void CommissionResult_LoadingDataSource(object sender, LoadingDataEventArgs e)
        {
            facade.GetCommissionToCashByQuery(model, e.PageSize, e.PageIndex, e.SortField, (obj, arg) => 
            {
                if (arg.FaultsHandle())
                {
                    return;
                }
                CommissionResult.ItemsSource = arg.Result.Rows;
                CommissionResult.TotalCount = arg.Result.TotalCount;
            });
        }

      
        private void BtnSearch_Click(object sender, RoutedEventArgs e)
        {
            CommissionResult.Bind();
        }

        private void hlEdit_Click(object sender, RoutedEventArgs e)
        {
            HyperlinkButton link = (HyperlinkButton)sender;
            if (link.Content.ToString() == "编辑")
            {
                link.Content = "修改";
                FrameworkElement element = CommissionResult.Columns[10].GetCellContent(this.CommissionResult.SelectedItem);
                if (element.GetType() == typeof(TextBox))
                {
                    ((TextBox)element).IsEnabled = true;
                }
            }
            else
            {

               
                dynamic d = this.CommissionResult.SelectedItem as dynamic;
                if (d.PayAmt == null)
                {
                    Window.MessageBox.Show("支付金额不能为空!", MessageBoxType.Error);
                    return;
                }
                string NewPayAmt = d.PayAmt.ToString();
                decimal result;
                if (!decimal.TryParse(NewPayAmt, out result))
                {
                    Window.MessageBox.Show("请输入正确的支付金额!", MessageBoxType.Error);
                    return;
                }
                CommissionToCashVM vm = new CommissionToCashVM() { SysNo = d.SysNo, NewPayAmt = NewPayAmt };
                facade.UpdateCommissionToCashPayAmt(vm, (obj, arg) => 
                {
                    if(arg.FaultsHandle())
                    {
                        return;
                    }
                    Window.MessageBox.Show("更新成功!", MessageBoxType.Success);
                    FrameworkElement element = CommissionResult.Columns[10].GetCellContent(this.CommissionResult.SelectedItem);
                    if (element.GetType() == typeof(TextBox))
                    {
                        ((TextBox)element).IsEnabled = false;
                    }
                    link.Content = "编辑";
                });
                
            }


        }

        private void hlAudit_Click(object sender, RoutedEventArgs e)
        {
            dynamic d = this.CommissionResult.SelectedItem as dynamic;
            string tempstr=ConvertAfterTax(d.ConfirmToCashAmt);
            CommissionToCashVM vm = new CommissionToCashVM()
            {
                OldPayAmt = tempstr,
                NewPayAmt = tempstr,
                Bonus = d.Bonus == null ? "" : d.Bonus.ToString(),
                SysNo = d.SysNo
            };
            CommissionToCashAuditComplete item = new CommissionToCashAuditComplete();
            item.Data = vm;
            item.Dialog = Window.ShowDialog("审核完成", item, (s, args) =>
            {
                if (args.DialogResult == Newegg.Oversea.Silverlight.Controls.Components.DialogResultType.OK)
                {
                    this.CommissionResult.Bind();
                }
            });

        }

        private void hlConfirm_Click(object sender, RoutedEventArgs e)
        {
             dynamic d = this.CommissionResult.SelectedItem as dynamic;
            CommissionToCashConfirmPay item = new CommissionToCashConfirmPay();
            CommissionToCashVM vm = new CommissionToCashVM()
            {
                SysNo = d.SysNo,
                ConfirmToCashAmt = d.ConfirmToCashAmt == null ? "" : d.ConfirmToCashAmt.ToString(),
                AfterTaxAmt = ConvertAfterTax(d.ConfirmToCashAmt),
                NewPayAmt = d.PayAmt == null ? "" : d.PayAmt.ToString()
            };
            item.Data = vm;
            item.Dialog = Window.ShowDialog("确认支付", item, (s, args) =>
            {
                if (args.DialogResult == Newegg.Oversea.Silverlight.Controls.Components.DialogResultType.OK)
                {
                    this.CommissionResult.Bind();
                }
            });
        }

        private void hlView_Click(object sender, RoutedEventArgs e)
        {
            HyperlinkButton link = (HyperlinkButton)sender;
            string str = "";
            if (link.Tag != null)
            {
                 str=link.Tag.ToString();
                str = str.Substring(0, str.Length - 1);
            }
            Window.Navigate(string.Format(ConstValue.ExternalSYS_OrderManagement, str), null, true);
        }
        private void hlSettledMonth_Click(object sender, RoutedEventArgs e)
        {
            string query = ((HyperlinkButton)sender).Tag.ToString();
            if (string.IsNullOrWhiteSpace(query))
                return;
            query = query.Substring(0, query.Length - 1);
            Window.Navigate(string.Format(ConstValue.ExternalSYS_FinanceQuery, query),null,true);
        }


        /// <summary>
        /// 计算税后
        /// </summary>
        /// <param name="originalAmt"></param>
        /// <returns></returns>
        private string  ConvertAfterTax(decimal originalAmt)
        {
          
            decimal afterTaxAmt = 0.00m;
            decimal taxAmt = 0.00m;         //税费
            decimal taxableIncome = 0.00m;  //应纳税所得额


            //计算应纳税所得额
            if (originalAmt <= 800)
            {
                taxableIncome = 0;
            }
            else if (originalAmt > 800 && originalAmt <= 4000)
            {
                taxableIncome = originalAmt - 800;
            }
            else if (originalAmt > 4000)
            {
                taxableIncome = originalAmt * 0.8m;
            }

            //计算个税
            if (taxableIncome <= 20000)
            {
                taxAmt = taxableIncome * 0.2m;
            }
            else if (taxableIncome > 20000 && taxableIncome <= 50000)
            {
                taxAmt = taxableIncome * 0.3m - 2000m;
            }
            else if (taxableIncome > 50000)
            {
                taxAmt = taxableIncome * 0.4m - 7000m;
            }

            afterTaxAmt = originalAmt - taxAmt;

            return afterTaxAmt.ToString();
        }

        

    }
}
