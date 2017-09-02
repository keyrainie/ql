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
using Newegg.Oversea.Silverlight.Controls.Components;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using ECCentral.Portal.UI.SO.Resources;
using Newegg.Oversea.Silverlight.Utilities.Validation;
using ECCentral.Portal.UI.SO.Models;
using ECCentral.Portal.Basic;

namespace ECCentral.Portal.UI.SO.UserControls
{
    public partial class SOElectronicCard : UserControl
    {
        public IDialog Dialog
        {
            get;
            set;
        }

        private List<SOItemInfoVM> m_SelectedItemInfo;
        private List<SOItemInfoVM> SelectedItemInfo
        {
            set { m_SelectedItemInfo = value; }
            get { return m_SelectedItemInfo; }
        }

        public SOElectronicCard()
        {
            InitializeComponent();
        }

        #region Helper Methods

        private void CloseDialog(ResultEventArgs args)
        {
            if (Dialog != null)
            {
                Dialog.ResultArgs = args;
                Dialog.Close();
            }
        }

        #endregion Helper Methods

        private void Button_AddSOElectronicCard_Click(object sender, RoutedEventArgs e)
        {
            //由父窗口执行真正的保存操作                  
            CloseDialog(new ResultEventArgs
            {
                DialogResult = DialogResultType.OK,
                Data = SelectedItemInfo
            });
        }

        private void hlkb_SOElectronicCard_Caculater_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(txtOriginalPrice.Text)
                || string.IsNullOrEmpty(txtQuantity.Text)
                || string.IsNullOrEmpty(txtPrice.Text))
            {
                CPApplication.Current.CurrentPage.Context.Window.Alert(ResSOMaintain.Info_ElectronicCard_Error, MessageType.Error);
            }
            else
            {
                try
                {
                    decimal OriginalPrice = Convert.ToDecimal(txtOriginalPrice.Text);
                    int Quantity = Convert.ToInt32(txtQuantity.Text);
                    decimal Price = Convert.ToDecimal(txtPrice.Text);
                    SOItemInfoVM soItemInfoVM = new SOItemInfoVM();
                    soItemInfoVM.OriginalPrice = OriginalPrice;
                    soItemInfoVM.Quantity = Quantity;
                    soItemInfoVM.Price = Price;
                    soItemInfoVM.ProductType = ECCentral.BizEntity.SO.SOProductType.Product;
                    if (OriginalPrice < Price)
                    {
                        CPApplication.Current.CurrentPage.Context.Window.Alert(ResSOMaintain.Info_ElectronicCard_Price_Error, MessageType.Error);
                    }
                    else if (soItemInfoVM.Quantity < 0 || soItemInfoVM.Price < 0)
                    {
                        CPApplication.Current.CurrentPage.Context.Window.Alert(ResSOMaintain.Info_ElectronicCard_Price_Value_Error, MessageType.Error);
                    }
                    else
                    {
                        decimal TotalAmount = (decimal)(soItemInfoVM.Quantity * soItemInfoVM.OriginalPrice);
                        decimal TotalReceiveAmount = (decimal)(soItemInfoVM.Quantity * soItemInfoVM.Price);
                        //总面额
                        txtTotalAmount.Text = TotalAmount.ToString();
                        //总数量
                        txtTotalQuantity.Text = soItemInfoVM.Quantity.ToString();
                        //应付金额
                        txtReceiveAmount.Text = TotalReceiveAmount.ToString();
                        decimal DiscountRate = Math.Abs((decimal)(TotalAmount - TotalReceiveAmount) / TotalAmount) * 100;
                        txtDiscountRate.Text = Math.Round(DiscountRate, 2) + "%";
                        soItemInfoVM.PromotionAmount = -(TotalAmount - TotalReceiveAmount);
                        soItemInfoVM.Price = soItemInfoVM.OriginalPrice;
                        soItemInfoVM.OnlineQty = 99999;
                        soItemInfoVM.AvailableQty = 99999;
                        ECCentral.Portal.Basic.Utilities.AppSettingHelper.GetSetting(ConstValue.DomainName_SO, ConstValue.SOElectronicCard_ProductID, (obj, args) =>
                        {      
                            if (!string.IsNullOrEmpty(args.Result))
                            {
                                soItemInfoVM.ProductID = args.Result;
                            }
                        });
                        ECCentral.Portal.Basic.Utilities.AppSettingHelper.GetSetting(ConstValue.DomainName_SO, ConstValue.SOElectronicCard_ProductName, (obj, args) =>
                        {
                            if (!string.IsNullOrEmpty(args.Result))
                            {
                                soItemInfoVM.ProductName = args.Result;
                            }
                        });
                        ECCentral.Portal.Basic.Utilities.AppSettingHelper.GetSetting(ConstValue.DomainName_IM, ConstValue.IM_ElectronicCard_ProductSysNo_Key, (obj, args) =>
                        {
                            int pSysNo = 0;
                            if (int.TryParse(args.Result, out pSysNo))
                            {
                                soItemInfoVM.ProductSysNo = pSysNo;
                            }
                            SelectedItemInfo = new List<SOItemInfoVM>();
                            SelectedItemInfo.Add(soItemInfoVM);
                        });
                    }
                }
                catch (Exception)
                {
                    CPApplication.Current.CurrentPage.Context.Window.Alert(ResSOMaintain.Info_ElectronicCard_Price_Format_Error, MessageType.Information);
                }
            }
        }
    }
}
