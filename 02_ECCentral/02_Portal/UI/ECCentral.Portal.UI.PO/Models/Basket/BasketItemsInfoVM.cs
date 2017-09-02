using System;

using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.BizEntity.PO;
using System.Collections.Generic;
using Newegg.Oversea.Silverlight.Utilities.Validation;
using ECCentral.Portal.UI.PO.Resources;
using ECCentral.BizEntity.Inventory;

namespace ECCentral.Portal.UI.PO.Models
{
    public class BasketItemsInfoVM : ModelBase
    {

        private bool isChecked;

        public bool IsChecked
        {
            get { return isChecked; }
            set 
            {
                this.SetValue("IsChecked", ref isChecked, value);
            }
        }
        private Int32? m_ItemSysNo;
        public Int32? ItemSysNo
        {
            get { return this.m_ItemSysNo; }
            set { this.SetValue("ItemSysNo", ref m_ItemSysNo, value); }
        }

        private Int32? m_CreateUserSysNo;
        public Int32? CreateUserSysNo
        {
            get { return this.m_CreateUserSysNo; }
            set { this.SetValue("CreateUserSysNo", ref m_CreateUserSysNo, value); }
        }

        private Int32? m_ProductSysNo;
        public Int32? ProductSysNo
        {
            get { return this.m_ProductSysNo; }
            set { this.SetValue("ProductSysNo", ref m_ProductSysNo, value); }
        }

        private string productMode;

        public string ProductMode
        {
            get { return productMode; }
            set { this.SetValue("ProductMode", ref productMode, value); }
        }

        private string briefName;

        public string BriefName
        {
            get { return briefName; }
            set { this.SetValue("BriefName", ref briefName, value); }
        }

        private int? weight;

        public int? Weight
        {
            get { return weight; }
            set { this.SetValue("Weight", ref weight, value); }
        }

        private string m_Quantity;
        [Validate(ValidateType.Required)]
        [Validate(ValidateType.Interger)]
        public string Quantity
        {
            get { return this.m_Quantity; }
            set { this.SetValue("Quantity", ref m_Quantity, value); }
        }

        private string m_OrderPrice;
        [Validate(ValidateType.Required)]
        [Validate(ValidateType.Regex, @"^[0-9]+\.?[0-9]{0,6}$", ErrorMessageResourceName = "Decimal_Required", ErrorMessageResourceType = typeof(ResValidationErrorMessage))]
        public string OrderPrice
        {
            get { return this.m_OrderPrice; }
            set { this.SetValue("OrderPrice", ref m_OrderPrice, value); }
        }

        private DateTime? m_CreateTime;
        public DateTime? CreateTime
        {
            get { return this.m_CreateTime; }
            set { this.SetValue("CreateTime", ref m_CreateTime, value); }
        }

        private String m_ProductID;
        public String ProductID
        {
            get { return this.m_ProductID; }
            set { this.SetValue("ProductID", ref m_ProductID, value); }
        }

        private Int32? m_LastVendorSysNo;
        public Int32? LastVendorSysNo
        {
            get { return this.m_LastVendorSysNo; }
            set { this.SetValue("LastVendorSysNo", ref m_LastVendorSysNo, value); }
        }

        private int? m_StockSysNo;
        public int? StockSysNo
        {
            get { return this.m_StockSysNo; }
            set { this.SetValue("StockSysNo", ref m_StockSysNo, value); }
        }

        private ECCentral.BizEntity.PO.YNStatus? m_IsTransfer;
        public ECCentral.BizEntity.PO.YNStatus? IsTransfer
        {
            get { return this.m_IsTransfer; }
            set { this.SetValue("IsTransfer", ref m_IsTransfer, value); }
        }

        private Int32? m_ReadyQuantity;
        public Int32? ReadyQuantity
        {
            get { return this.m_ReadyQuantity; }
            set { this.SetValue("ReadyQuantity", ref m_ReadyQuantity, value); }
        }

        private String m_StockName;
        public String StockName
        {
            get { return this.m_StockName; }
            set { this.SetValue("StockName", ref m_StockName, value); }
        }

        private String m_ErrorMessage;
        public String ErrorMessage
        {
            get { return this.m_ErrorMessage; }
            set { this.SetValue("ErrorMessage", ref m_ErrorMessage, value); }
        }

        private int? vendorSysNo;

        public int? VendorSysNo
        {
            get { return vendorSysNo; }
            set { this.SetValue("VendorSysNo", ref vendorSysNo, value); }
        }

        private int? pMSysNo;

        public int? PMSysNo
        {
            get { return pMSysNo; }
            set { this.SetValue("PMSysNo", ref pMSysNo, value); }
        }

        private string pMName;

        public string PMName
        {
            get { return pMName; }
            set { this.SetValue("PMName", ref pMName, value); }
        }


        private string createUserName;

        public string CreateUserName
        {
            get { return createUserName; }
            set { this.SetValue("CreateUserName", ref createUserName, value); }
        }


        private string vendorName;

        public string VendorName
        {
            get { return vendorName; }
            set { this.SetValue("VendorName", ref vendorName, value); }
        }


        /// <summary>
        /// 验证采购蓝商品代销类型和商家代销类型是否一致
        /// </summary>
        private int? vendorIsConsign;

        public int? VendorIsConsign
        {
            get { return vendorIsConsign; }
            set { this.SetValue("VendorIsConsign", ref vendorIsConsign, value); }
        }

        private ECCentral.BizEntity.PO.YNStatus? isConsign;

        public ECCentral.BizEntity.PO.YNStatus? IsConsign
        {
            get { return isConsign; }
            set { this.SetValue("IsConsign", ref isConsign, value); }
        }

        private int? masterProductSysNo;

        public int? MasterProductSysNo
        {
            get { return masterProductSysNo; }
            set { this.SetValue("MasterProductSysNo", ref masterProductSysNo, value); }
        }

        /// <summary>
        /// 赠品编号
        /// </summary>
        private int? giftSysNo;

        public int? GiftSysNo
        {
            get { return giftSysNo; }
            set { this.SetValue("GiftSysNo", ref giftSysNo, value); }
        }


        public List<KeyValuePair<ECCentral.BizEntity.PO.YNStatus?, string>> IsTransferData
        {
            get;
            set;
        }

        public List<WarehouseInfo> TargetStockList { get; set; }

        private bool? isManagerPM;
        public bool? IsManagerPM
        {
            get { return isManagerPM; }
            set { this.SetValue("IsManagerPM", ref isManagerPM, value); }
        }

        #region UI 扩展属性

        private bool isEnabledTransfer;
        public bool IsEnabledTransfer
        {
            get { return isEnabledTransfer; }
            set { this.SetValue("IsEnabledTransfer", ref isEnabledTransfer, value); }
        }

        public PaySettleCompany PaySettleCompany
        {
            get;
            set;
        }

        #endregion
    }
}
