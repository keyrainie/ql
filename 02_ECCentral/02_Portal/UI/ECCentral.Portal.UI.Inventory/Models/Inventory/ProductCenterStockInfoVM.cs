using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using ECCentral.BizEntity.Inventory;
using Newegg.Oversea.Silverlight.Utilities.Validation;
using ECCentral.Portal.UI.Inventory.Resources;
using System.ComponentModel.DataAnnotations;

namespace ECCentral.Portal.UI.Inventory.Models
{
    public class ProductCenterStockInfoVM : ModelBase
    {
        public ProductCenterItemInfoVM Parent { get; set; }

        public ProductCenterStockInfoVM()
        {
            m_OutStockWareHouseNumber = "-999";
            m_ShiftQty = "0";
            m_PurchaseQty = "0";
            m_OutStockShiftQtyDisplay = 0;
            m_NeedBufferEnable = YNStatus.No;
            m_NeedBufferVisible = true;
        }

        private Int32? m_ItemSysNumber;
        public Int32? ItemSysNumber
        {
            get { return this.m_ItemSysNumber; }
            set { this.SetValue("ItemSysNumber", ref m_ItemSysNumber, value); }
        }

        private Int32? m_AvailableQty;
        public Int32? AvailableQty
        {
            get { return this.m_AvailableQty; }
            set { this.SetValue("AvailableQty", ref m_AvailableQty, value); }
        }

        private Int32? m_VirtualQty;
        public Int32? VirtualQty
        {
            get { return this.m_VirtualQty; }
            set { this.SetValue("VirtualQty", ref m_VirtualQty, value); }
        }

        private Int32? m_OrderQty;
        public Int32? OrderQty
        {
            get { return this.m_OrderQty; }
            set { this.SetValue("OrderQty", ref m_OrderQty, value); }
        }

        private Int32? m_ConsignQty;
        public Int32? ConsignQty
        {
            get { return this.m_ConsignQty; }
            set { this.SetValue("ConsignQty", ref m_ConsignQty, value); }
        }

        private Int32? m_AvailableQtyStock;
        public Int32? AvailableQtyStock
        {
            get { return this.m_AvailableQtyStock; }
            set { this.SetValue("AvailableQtyStock", ref m_AvailableQtyStock, value); }
        }

        public int falgPurchase;// 是否 人为点击“ 是否中转”标志位 （1 表示  采购数量输入负值时触发的 "是否中转" 否）

        private string m_PurchaseQty;
        /// <summary>
        /// 采购数量
        /// </summary>
        [Validate(ValidateType.Required)]
        [Validate(ValidateType.Interger)]
        public string PurchaseQty
        {
            get { return m_PurchaseQty; }
            set
            {
              if (value!=m_PurchaseQty)
              { 
                int input = 0;
                if (!int.TryParse(value, out input))
                {
                    NeedBufferVisible = true;
                }
                if (input < 0)
                {
                    falgPurchase = 1; //采购数量为负的时候 理论上应该是人为输入的 此时 把标志位  定义为 1  说明是 采购数量输入负值是触发 的 “是否中转” 为否 
                    NeedBufferVisible = false;
                    NeedBufferEnable = YNStatus.No;
                }
                else
                {
                    falgPurchase = 0;//采购数量大于等于零时  取消  程序触发标志位。
                    NeedBufferVisible = true;
                }
                this.SetValue("PurchaseQty", ref m_PurchaseQty, value);
              }
            }
        }

        private Int32? m_PurchaseInQty;
        public Int32? PurchaseInQty
        {
            get { return this.m_PurchaseInQty; }
            set { this.SetValue("PurchaseInQty", ref m_PurchaseInQty, value); }
        }

        private Int32? m_ShiftInQty;
        public Int32? ShiftInQty
        {
            get { return this.m_ShiftInQty; }
            set { this.SetValue("ShiftInQty", ref m_ShiftInQty, value); }
        }
  
        private string m_ShiftQty;

        /// <summary>
        /// 移仓数量
        /// </summary>
        [Validate(ValidateType.Required)]
        [Validate(ValidateType.Interger)]
        public string ShiftQty
        {
            get { return this.m_ShiftQty; }
            set
            {
                if (Convert.ToInt32(m_ShiftQty) > OutStockShiftQtyDisplay)
                {
                    throw new ValidationException("移仓[" + WareHouseNumber + "]移仓数量不能大于[" + OutStockWareHouseNumber + "]可移数量");
                }
                this.SetValue("ShiftQty", ref m_ShiftQty, value);
            }
        }

        private string m_OutStockWareHouseNumber;
        /// <summary>
        /// 移仓出仓库
        /// </summary>
        public string OutStockWareHouseNumber
        {
            get { return m_OutStockWareHouseNumber; }
            set
            {
               if (value != m_OutStockWareHouseNumber)
               { 
                    m_OutStockWareHouseNumber = value;
                    if (value == "-999")
                    {
                        OutStockShiftQtyDisplay = 0;
                        ShiftQty = "0";
                    }
                    else
                    {
                        if (this.Parent != null)
                        {
                            foreach (ProductCenterStockInfoVM stock in this.Parent.SuggestTransferStocks)
                            {
                                if (stock.WareHouseNumber == value)
                                {
                                    OutStockShiftQtyDisplay = stock.OutStockShiftQty;
                                    break;
                                }
                            }
                        }
                    }
                    this.SetValue("OutStockWareHouseNumber", ref m_OutStockWareHouseNumber, value);
                }
            }
        }

        private Int32? m_D1;
        public Int32? D1
        {
            get { return this.m_D1; }
            set { this.SetValue("D1", ref m_D1, value); }
        }

        private Int32? m_D2;
        public Int32? D2
        {
            get { return this.m_D2; }
            set { this.SetValue("D2", ref m_D2, value); }
        }

        private Int32? m_D3;
        public Int32? D3
        {
            get { return this.m_D3; }
            set { this.SetValue("D3", ref m_D3, value); }
        }

        private Int32? m_D4;
        public Int32? D4
        {
            get { return this.m_D4; }
            set { this.SetValue("D4", ref m_D4, value); }
        }

        private Int32? m_D5;
        public Int32? D5
        {
            get { return this.m_D5; }
            set { this.SetValue("D5", ref m_D5, value); }
        }

        private Int32? m_D6;
        public Int32? D6
        {
            get { return this.m_D6; }
            set { this.SetValue("D6", ref m_D6, value); }
        }

        private Int32? m_D7;
        public Int32? D7
        {
            get { return this.m_D7; }
            set { this.SetValue("D7", ref m_D7, value); }
        }

        private Int32? m_D123;
        public Int32? D123
        {
            get { return this.m_D123; }
            set { this.SetValue("D123", ref m_D123, value); }
        }

        private Int32? m_W1;
        public Int32? W1
        {
            get { return this.m_W1; }
            set { this.SetValue("W1", ref m_W1, value); }
        }

        private Int32? m_W2;
        public Int32? W2
        {
            get { return this.m_W2; }
            set { this.SetValue("W2", ref m_W2, value); }
        }

        private Int32? m_W3;
        public Int32? W3
        {
            get { return this.m_W3; }
            set { this.SetValue("W3", ref m_W3, value); }
        }

        private Int32? m_W4;
        public Int32? W4
        {
            get { return this.m_W4; }
            set { this.SetValue("W4", ref m_W4, value); }
        }

        private Int32? m_M1;
        public Int32? M1
        {
            get { return this.m_M1; }
            set { this.SetValue("M1", ref m_M1, value); }
        }

        private Int32? m_M2;
        public Int32? M2
        {
            get { return this.m_M2; }
            set { this.SetValue("M2", ref m_M2, value); }
        }

        private Int32? m_M3;
        public Int32? M3
        {
            get { return this.m_M3; }
            set { this.SetValue("M3", ref m_M3, value); }
        }

        private Int32? m_W1RegionSalesQty;
        public Int32? W1RegionSalesQty
        {
            get { return this.m_W1RegionSalesQty; }
            set { this.SetValue("W1RegionSalesQty", ref m_W1RegionSalesQty, value); }
        }

        private Int32? m_W2RegionSalesQty;
        public Int32? W2RegionSalesQty
        {
            get { return this.m_W2RegionSalesQty; }
            set { this.SetValue("W2RegionSalesQty", ref m_W2RegionSalesQty, value); }
        }

        private Decimal? m_W1RegionC3SalesQtyRate;
        public Decimal? W1RegionC3SalesQtyRate
        {
            get { return this.m_W1RegionC3SalesQtyRate; }
            set { this.SetValue("W1RegionC3SalesQtyRate", ref m_W1RegionC3SalesQtyRate, value); }
        }

        private Decimal? m_W2RegionC3SalesQtyRate;
        public Decimal? W2RegionC3SalesQtyRate
        {
            get { return this.m_W2RegionC3SalesQtyRate; }
            set { this.SetValue("W2RegionC3SalesQtyRate", ref m_W2RegionC3SalesQtyRate, value); }
        }

        private Int32? m_M1RegionSalesQty;
        public Int32? M1RegionSalesQty
        {
            get { return this.m_M1RegionSalesQty; }
            set { this.SetValue("M1RegionSalesQty", ref m_M1RegionSalesQty, value); }
        }

        private Int32? m_SuggestQty;
        public Int32? SuggestQty
        {
            get { return this.m_SuggestQty.HasValue ? m_SuggestQty.Value : 0; }
            set { this.SetValue("SuggestQty", ref m_SuggestQty, value); }
        }

        private Int32? m_SuggestQtyDisplay;
        public Int32? SuggestQtyDisplay
        {
            get { return this.m_SuggestQtyDisplay.HasValue ? m_SuggestQtyDisplay.Value : 0; }
            set { this.SetValue("SuggestQtyDisplay", ref m_SuggestQtyDisplay, value); }
        }

        private Decimal? m_LastPrice;
        public Decimal? LastPrice
        {
            get { return this.m_LastPrice.HasValue ? m_LastPrice.Value : 0.00M; }
            set { this.SetValue("LastPrice", ref m_LastPrice, value); }
        }

        private DateTime? m_LastintimeForDBMap;
        public DateTime? LastintimeForDBMap
        {
            get { return this.m_LastintimeForDBMap; }
            set { this.SetValue("LastintimeForDBMap", ref m_LastintimeForDBMap, value); }
        }

        private DateTime? m_Lastintime;
        public DateTime? Lastintime
        {
            get { return this.m_Lastintime; }
            set { this.SetValue("Lastintime", ref m_Lastintime, value); }
        }

        private Int32? m_OutStockShiftQty;
        public Int32? OutStockShiftQty
        {
            get { return this.m_OutStockShiftQty; }
            set { this.SetValue("OutStockShiftQty", ref m_OutStockShiftQty, value); }
        }

        private Int32? m_OutStockShiftQtyDisplay;
        public Int32? OutStockShiftQtyDisplay
        {
            get { return this.m_OutStockShiftQtyDisplay.HasValue ? m_OutStockShiftQtyDisplay.Value : 0; }
            set { this.SetValue("OutStockShiftQtyDisplay", ref m_OutStockShiftQtyDisplay, value); }
        }

        private String m_WareHouseNumber;
        public String WareHouseNumber
        {
            get { return this.m_WareHouseNumber; }
            set { this.SetValue("WareHouseNumber", ref m_WareHouseNumber, value); }
        }

        private Int32? m_MinPackNumber;
        public Int32? MinPackNumber
        {
            get { return this.m_MinPackNumber; }
            set { this.SetValue("MinPackNumber", ref m_MinPackNumber, value); }
        }

        private String m_SendPeriod;
        public String SendPeriod
        {
            get { return this.m_SendPeriod; }
            set { this.SetValue("SendPeriod", ref m_SendPeriod, value); }
        }

        private Int32? m_SuggestQtyZhongZhuan;
        public Int32? SuggestQtyZhongZhuan
        {
            get { return this.m_SuggestQtyZhongZhuan; }
            set { this.SetValue("SuggestQtyZhongZhuan", ref m_SuggestQtyZhongZhuan, value); }
        }

        private Decimal? m_AVGDailySales;
        public Decimal? AVGDailySales
        {
            get { return this.m_AVGDailySales; }
            set { this.SetValue("AVGDailySales", ref m_AVGDailySales, value); }
        }

        private Int32? m_AvailableSalesDays;
        public Int32? AvailableSalesDays
        {
            get
            {
                if (m_VirtualQty > 0)
                {
                    return 0;
                }
                else if (m_AVGDailySales == 0 && (AvailableQty + m_ConsignQty) > 0)
                {
                    return 999;
                }
                else
                {
                    return m_AvailableSalesDays;
                }
            }
            set { this.SetValue("AvailableSalesDays", ref m_AvailableSalesDays, value); }
        }

        private string m_Price;
        /// <summary>
        /// 采购价格
        /// </summary>
        [Validate(ValidateType.Required)]
        [Validate(ValidateType.Regex, @"^[0-9]+\.?[0-9]{0,2}$", ErrorMessageResourceName = "ErrorMsg_ValidateDecimal", ErrorMessageResourceType = typeof(ResTransferStockingCenter))]
        public string Price
        {
            get { return m_Price; }
            set 
            {               
                if (WareHouseNumber == "51" && this.Parent!=null)
                {
                    foreach (var item in this.Parent.SuggestTransferStocks)
	                {
                       if(item.WareHouseNumber=="59")
                       {
                         item.Price = m_Price.ToString();
                       }		 
	                }
                }
                this.SetValue("Price", ref m_Price, value); 
            }
        }

        private YNStatus? m_NeedBufferEnable;
        public YNStatus? NeedBufferEnable
        {
            get { return m_NeedBufferEnable; }
            set { this.SetValue("NeedBufferEnable", ref m_NeedBufferEnable, value); }
        }

        private bool m_NeedBufferVisible;
        public bool NeedBufferVisible
        {
            get
            {
                if (this.WareHouseNumber == "56" || this.WareHouseNumber == "57" || this.WareHouseNumber == "58")
                {
                    return true;
                }
                return m_NeedBufferVisible;
            }
            set
            {
                this.SetValue("NeedBufferVisible", ref m_NeedBufferVisible, value);                           
            }           
        }
    }
}
