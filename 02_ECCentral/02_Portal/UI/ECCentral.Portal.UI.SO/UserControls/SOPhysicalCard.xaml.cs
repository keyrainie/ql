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
using Newegg.Oversea.Silverlight.Controls.Containers;
using ECCentral.Portal.UI.SO.Facades;
using ECCentral.BizEntity.SO;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.Portal.UI.SO.Resources;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using ECCentral.QueryFilter.IM;
using ECCentral.QueryFilter.Common;
using Newegg.Oversea.Silverlight.Controls;
using ECCentral.BizEntity.IM;
using ECCentral.Portal.UI.SO.Models;

namespace ECCentral.Portal.UI.SO.UserControls
{
    public partial class SOPhysicalCard : UserControl
    {
        #region Member

        /// <summary>
        /// 暂定返回对象List<SOItemInfo>
        /// </summary>
        List<SOItemInfoVM> m_returnData;

        #endregion

        #region Property

        public IDialog Dialog
        {
            get;
            set;
        }

        private IWindow Window
        {
            get
            {
                return Page == null ? Page.Context.Window : CPApplication.Current.CurrentPage.Context.Window;
            }
        }

        private IPage Page
        {
            get;
            set;
        } 

        #endregion

        public SOPhysicalCard(IPage page, string companyCode)
        {
            this.Page = page;
            InitializeComponent();
            UtilityHelper.ReadOnlyControl(gridTotalInfo, gridTotalInfo.Children.Count,true);

            ProductQueryFilter query = new ProductQueryFilter();
            query.ProductID = "GC-002-";
            query.IsNotAbandon = true;
            query.CompanyCode = CPApplication.Current.CompanyCode;
            query.PagingInfo = new PagingInfo()
            {
                PageSize = 1000,
                PageIndex = 0,
                SortBy = "PP.CurrentPrice"
            };

            new OtherDomainQueryFacade(this.Page).QueryProductRequest(query, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }
                var list = args.Result.Rows;
                InitDataGrid(list);
            });
        }

        private void InitDataGrid(dynamic list)
        {
            foreach (var item in list)
            {
                //添加一列
                item["OriginalPrice"] = item["Price"];
                item["Quantity"] = 0;
                item["GainQuantity"] = 0;
            }
            dgridSOPhysicalCardInfo.ItemsSource = list;
        }

        private void hlkb_SOPhysicalCard_ClearData_Click(object sender, RoutedEventArgs e)
        {
            InitDataGrid(dgridSOPhysicalCardInfo.ItemsSource);
        }

        private void hlkb_SOPhysicalCard_SOCaculater_Click(object sender, RoutedEventArgs e)
        {
            if (ValidData())
            {
                if (m_returnData.Count > 0)
                {
                    var totalDenomination = m_returnData.Sum(p => p.OriginalPrice * (p.Quantity + p.GainQuantity));
                    var presentDenomination = m_returnData.Sum(p => p.OriginalPrice * p.GainQuantity);
                    var totalPrice = m_returnData.Sum(p => p.Quantity * p.Price);
                    var totalQty = m_returnData.Sum(p => p.Quantity + p.GainQuantity);
                    var totalPresentQty = m_returnData.Sum(p => p.GainQuantity);
                    var priceAmt = m_returnData.Sum(p => p.Price);
                    var denominationAmt = m_returnData.Sum(p => p.OriginalPrice);
                    var totalRate = 0.0M;
                    if (totalDenomination > 0)
                    {
                        totalRate = Math.Abs(1 - totalPrice.Value / totalDenomination.Value) * 100;
                    }
                    txtTotalAmount.Text = decimal.Round(totalDenomination.Value, 2).ToString();
                    txtGainTotalAmount.Text = decimal.Round(presentDenomination.Value, 2).ToString();
                    txtRecevieAmount.Text = decimal.Round(totalPrice.Value, 2).ToString();

                    txtTotalQuantity.Text = totalQty.ToString();
                    txtGainQuantity.Text = totalPresentQty.ToString();
                    txtDiscountRate.Text = string.Format("{0}%", decimal.Round(totalRate, 2));
                }
            }
        }

        private void btn_AddSOPhysicalCard_Click(object sender, RoutedEventArgs e)
        {
            if (Dialog != null)
            {
                if (ValidData())
                {
                    //在这里计算返回的值
                    m_returnData.ForEach(p => {
                        //计算总库存
                        p.Quantity = p.Quantity + p.GainQuantity;
                        p.PromotionAmount = 0.00M;
                        //计算总折扣总额
                        if (p.GainQuantity > 0)
                        {
                            p.PromotionAmount = p.GainQuantity * p.OriginalPrice;
                        }
                        else if (p.Price != p.OriginalPrice)
                        {
                            p.PromotionAmount = (p.OriginalPrice - p.Price) * p.Quantity;
                        }
                        p.PromotionAmount = -p.PromotionAmount;
                    });

                    Dialog.ResultArgs.Data = m_returnData;

                    Dialog.ResultArgs.DialogResult = DialogResultType.OK;
                    Dialog.Close();
                }
            }
        }

        private void txtPrice_LostFocus(object sender, RoutedEventArgs e)
        {
            TextBox input = sender as TextBox;
            decimal inputData = 0.00M;
            Decimal.TryParse(input.Text, out inputData);
            if (inputData < 0)
            {
                inputData = 0.00M;
            }
            input.Text = decimal.Round(inputData, 2).ToString();
        }

        private void txtQuantity_LostFocus(object sender, RoutedEventArgs e)
        {
            TextBox input = sender as TextBox;
            decimal inputData = 0;
            Decimal.TryParse(input.Text, out inputData);
            if (inputData < 0)
            {
                //不允许小于0
                inputData = 0;
            }
            input.Text = inputData.ToString();
        }

        private void txtGainQuantity_LostFocus(object sender, RoutedEventArgs e)
        {
            TextBox input = sender as TextBox;
            decimal inputData = 0;
            Decimal.TryParse(input.Text, out inputData);
            if (inputData < 0)
            {
                //不允许小于0
                inputData = 0;
            }
            input.Text = inputData.ToString();
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            if (Dialog != null)
            {
                Dialog.ResultArgs.DialogResult = DialogResultType.Cancel;
                Dialog.Close();
            }
        }

        /// <summary>
        /// 验证数据
        /// </summary>
        /// <returns></returns>
        bool ValidData()
        {
            bool result = false;
            //验证值输入
            var data = dgridSOPhysicalCardInfo.ItemsSource as IEnumerable<object>;
            m_returnData = new List<SOItemInfoVM>();
            if (data != null && data.Count() > 0)
            {
                foreach (var item in data)
                {
                    result = true;
                    var itemData = item as DynamicXml;
                    /*有以下几种验证
                     * 1、购买价格必须>0,且<=实物卡面额
                     * 2、购买数量+赠送数量不能大于库存数量
                     * 3、如果价格有优惠就不能设置赠送数量;赠送数量不能大于购买数量
                    */
                    //1
                    decimal price = decimal.Parse(itemData["Price"].ToString());
                    if (price == 0)
                    {
                        //验证通过，但是不予返回数据
                        continue;
                    }
                    decimal originPrice = (decimal)itemData["OriginalPrice"];
                    if (price > originPrice)
                    {
                        //未通过判断中断
                        Window.Alert(string.Format(ResSO.Msg_Error_LessZeroOrMoreOriginPrice,itemData["ProductID"]));
                        result = false;
                        break;
                    }
                    //2
                    int qty = int.Parse(itemData["Quantity"].ToString());
                    int gainQty = int.Parse(itemData["GainQuantity"].ToString());
                    int onlineQty = (int)itemData["OnlineQty"];
                    if (qty == 0)
                    {
                        //验证通过，但是不予返回数据
                        continue;
                    }
                    if ((qty + gainQty) > onlineQty)
                    {
                        //未通过判断中断
                        Window.Alert(string.Format(ResSO.Msg_Error_OrderQtyAndGainQtyMoreOnlineQty, itemData["ProductID"]));
                        result = false;
                        break;
                    }
                    //3
                    if ((gainQty > 0 && originPrice != price)
                        || gainQty > qty)
                    {
                        //未通过判断中断
                        Window.Alert(string.Format(ResSO.Msg_Error_NoGainQtyWhenHasPromotionPrice, itemData["ProductID"]));
                        result = false;
                        break;
                    }

                    //加载到最后返回中
                    var returnItem = DynamicConverter<SOItemInfoVM>.ConvertToVM(itemData,"ProductType");
                    //商品类型为礼品卡
                    returnItem.ProductType = SOProductType.Product;
                    m_returnData.Add(returnItem);
                }
            }
            else
            {
                result = true;
            }

            return result;
        }
    }
}
