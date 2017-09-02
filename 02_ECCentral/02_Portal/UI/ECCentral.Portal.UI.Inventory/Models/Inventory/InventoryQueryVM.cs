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

namespace ECCentral.Portal.UI.Inventory.Models
{
    public class InventoryQueryVM : ModelBase
    {

        public InventoryQueryVM()
        {
            isAccountQtyLargerThanZero = false;
            isUnPayShow = false;
            isShowTotalInventory = false;
        }
        /// <summary>
        ///   渠道编号
        /// </summary>
        private int? webChannelID;

        public int? WebChannelID
        {
            get { return webChannelID; }
            set { base.SetValue("WebChannelID", ref webChannelID, value); }
        }

        /// <summary>
        /// 仓库编号
        /// </summary>
        private int? wareHouseID;

        public int? WareHouseID
        {
            get { return wareHouseID; }
            set { base.SetValue("WareHouseID", ref wareHouseID, value); }
        }

        /// <summary>
        /// 库位
        /// </summary>
        private string positionInWareHouse;

        public string PositionInWareHouse
        {
            get { return positionInWareHouse; }
            set { base.SetValue("PositionInWareHouse", ref positionInWareHouse, value); }
        }

        /// <summary>
        /// 渠道仓库编号
        /// </summary>
        private int? stockID;

        public int? StockID
        {
            get { return stockID; }
            set { base.SetValue("StockID", ref stockID, value); }
        }


        /// <summary>
        /// 渠道仓库编号(冗余字段)
        /// </summary>
        private int? stockSysNo;
        public int? StockSysNo
        {
            get { return stockSysNo; }
            set { base.SetValue("StockSysNo", ref stockSysNo, value); }
        }

        /// <summary>
        /// 渠道仓库编号
        /// </summary>
        private int? stockChannelID;

        public int? StockChannelID
        {
            get { return stockChannelID; }
            set { base.SetValue("StockChannelID", ref stockChannelID, value); }
        }

        /// <summary>
        /// 供应商编号
        /// </summary>
        private int? vendorSysNo; 

        public int? VendorSysNo
        {
            get { return vendorSysNo; }
            set { base.SetValue("VendorSysNo", ref vendorSysNo, value); }
        }

        /// <summary>
        /// 厂商编号
        /// </summary>
        private int? manufacturerSysNo;

        public int? ManufacturerSysNo
        {
            get { return manufacturerSysNo; }
            set { base.SetValue("ManufacturerSysNo", ref manufacturerSysNo, value); }
        }

        /// <summary>
        /// 品牌编号
        /// </summary>
        private int? brandSysNo;

        public int? BrandSysNo
        {
            get { return brandSysNo; }
            set { base.SetValue("BrandSysNo", ref brandSysNo, value); }
        }

        /// <summary>
        /// 厂商名称
        /// </summary>
        private string manufacturerNameDisplay;

        public string ManufacturerNameDisplay
        {
            get { return manufacturerNameDisplay; }
            set { base.SetValue("ManufacturerNameDisplay", ref manufacturerNameDisplay, value); }
        }
        
        /// <summary>
        /// 品牌名称
        /// </summary>
        private string brandNameDisplay;

        public string BrandNameDisplay
        {
            get { return brandNameDisplay; }
            set { base.SetValue("BrandNameDisplay", ref brandNameDisplay, value); }
        }

        /// <summary>
        /// 商品一级类别编号
        /// </summary>
        private int? c1SysNo;

        public int? C1SysNo
        {
            get { return c1SysNo; }
            set { base.SetValue("C1SysNo", ref c1SysNo, value); }
        }
        /// <summary>
        /// 商品二级类别编号
        /// </summary>
        private int? c2SysNo;

        public int? C2SysNo
        {
            get { return c2SysNo; }
            set { base.SetValue("C2SysNo", ref c2SysNo, value); }
        }
        /// <summary>
        /// 商品三级类别编号
        /// </summary>
        private int? c3SysNo;

        public int? C3SysNo
        {
            get { return c3SysNo; }
            set { base.SetValue("C3SysNo", ref c3SysNo, value); }
        }

        /// <summary>
        ///  商品SysNo
        /// </summary>
        private int? productSysNo;

        public int? ProductSysNo
        {
            get { return productSysNo; }
            set { base.SetValue("ProductSysNo", ref productSysNo, value); }
        }

        //商品名称
        private string productName;

        public string ProductName
        {
            get { return productName; }
            set { base.SetValue("ProductName", ref productName, value); }
        }

        /// <summary>
        /// 是否显示总库存
        /// </summary>
        private bool isShowTotalInventory;

        public bool IsShowTotalInventory
        {
            get { return isShowTotalInventory; }
            set { base.SetValue("IsShowTotalInventory", ref isShowTotalInventory, value); }
        }

        /// <summary>
        /// 是否财务库存大于0
        /// </summary>
        private bool isAccountQtyLargerThanZero;

        public bool IsAccountQtyLargerThanZero
        {
            get { return isAccountQtyLargerThanZero; }
            set { base.SetValue("IsAccountQtyLargerThanZero", ref isAccountQtyLargerThanZero, value); }
        }

        /// <summary>
        /// 是否显示未支付订单数量
        /// </summary>
        private bool isUnPayShow;

        public bool IsUnPayShow
        {
            get { return isUnPayShow; }
            set { base.SetValue("IsUnPayShow", ref isUnPayShow, value); }
        }

        private string companyCode;

        public string CompanyCode
        {
            get { return companyCode; }
            set { base.SetValue("CompanyCode", ref companyCode, value); }
        }

        /// <summary>
        /// 库存预警
        /// </summary>
        private bool isInventoryWarning;

        public bool IsInventoryWarning
        {
            get { return isInventoryWarning; }
            set { base.SetValue("IsInventoryWarning", ref isInventoryWarning, value); }
        }

        #region UI Model
      
        #endregion UI Model
    }
}
