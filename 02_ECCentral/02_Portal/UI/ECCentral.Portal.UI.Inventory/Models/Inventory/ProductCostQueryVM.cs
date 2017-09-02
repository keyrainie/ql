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
    public class ProductCostQueryVM : ModelBase
    {
        public ProductCostQueryVM()
        {
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
        /// 渠道仓库编号
        /// </summary>
        private int? stockID;

        public int? StockID
        {
            get { return stockID; }
            set { base.SetValue("StockID", ref stockID, value); }
        }


        ///// <summary>
        ///// 渠道仓库编号(冗余字段)
        ///// </summary>
        //private int? stockSysNo;
        //public int? StockSysNo
        //{
        //    get { return stockSysNo; }
        //    set { base.SetValue("StockSysNo", ref stockSysNo, value); }
        //}

        ///// <summary>
        ///// 渠道仓库编号
        ///// </summary>
        //private int? stockChannelID;

        //public int? StockChannelID
        //{
        //    get { return stockChannelID; }
        //    set { base.SetValue("StockChannelID", ref stockChannelID, value); }
        //}

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

        //有可用库存
        private bool isAvailableInventory;

        public bool IsAvailableInventory
        {
            get { return isAvailableInventory; }
            set { base.SetValue("IsAvailableInventory", ref isAvailableInventory, value); }
        }
    }
}
