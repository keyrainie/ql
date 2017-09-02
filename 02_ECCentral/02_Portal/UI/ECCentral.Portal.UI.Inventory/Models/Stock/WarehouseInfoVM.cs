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
using Newegg.Oversea.Silverlight.Utilities.Validation;
using System.Collections.Generic;
using ECCentral.BizEntity.Inventory;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.Portal.UI.Inventory.Resources;
using ECCentral.BizEntity.Common;

namespace ECCentral.Portal.UI.Inventory.Models
{
    public class WarehouseInfoVM : ModelBase
    {

        public WarehouseInfoVM()
        {            
            warehouseType = BizEntity.Inventory.WarehouseType.Real;
            customsCode = CustomsCodeMode.DirectImportMode;
            stockType = TradeType.Internal;
            //stockInfoList = new List<StockInfoVM>();
        }
        private int? sysNo;
        public int? SysNo
        {
            get { return sysNo; }
            set { base.SetValue("SysNo", ref sysNo, value); }
        }

        private string companyCode;
        public string CompanyCode
        {
            get { return companyCode; }
            set { base.SetValue("CompanyCode", ref companyCode, value); }
        }

        private string warehouseID;
        /// <summary>
        /// 仓库编号
        /// </summary>
        [Validate(ValidateType.Required)]
        [Validate(ValidateType.Regex, @"^[\w-#]+$", ErrorMessageResourceName = "Meg_WHID_Format", ErrorMessageResourceType = typeof(ResWarehouseQuery))]
        public string WarehouseID
        {
            get { return warehouseID; }
            set { base.SetValue("WarehouseID", ref warehouseID, value); }
        }

        private string warehouseName;
        /// <summary>
        /// 仓库名称
        /// </summary>
        [Validate(Newegg.Oversea.Silverlight.Utilities.Validation.ValidateType.Required)]
        public string WarehouseName
        {
            get { return warehouseName; }
            set { base.SetValue("WarehouseName", ref warehouseName, value); }
        }
        private WarehouseType warehouseType;

        /// <summary>
        /// 仓库类型
        /// </summary>
        [Validate(Newegg.Oversea.Silverlight.Utilities.Validation.ValidateType.Required)]
        public WarehouseType WarehouseType
        {
            get { return warehouseType; }
            set { base.SetValue("WarehouseType", ref warehouseType, value); }
        }

        private string ownerSysNo;
        /// <summary>
        /// 仓库所有者
        /// </summary>        
        //[Validate(ValidateType.Required)]
        public string OwnerSysNo
        {
            get { return ownerSysNo; }
            set { base.SetValue("OwnerSysNo", ref ownerSysNo, value); }
        }


        private string ownerName;
        /// <summary>
        /// 仓库所有者
        /// </summary> 
        public string OwnerName
        {
            get { return ownerName; }
            set
            {
                SetValue("OwnerName", ref ownerName, value);
            }
        }
        
        private string address;
        /// <summary>
        /// 仓库地址
        /// </summary>
        [Validate(Newegg.Oversea.Silverlight.Utilities.Validation.ValidateType.Required)]
        public string Address
        {
            get { return address; }
            set { base.SetValue("Address", ref address, value); }
        }

        private string contact;
        /// <summary>
        /// 联系人
        /// </summary>
        [Validate(Newegg.Oversea.Silverlight.Utilities.Validation.ValidateType.Required)]
        public string Contact
        {
            get { return contact; }
            set { base.SetValue("Contact", ref contact, value); }
        }

        private string contactEmail;
        /// <summary>
        /// 联系人Email
        /// </summary>
        [Validate(Newegg.Oversea.Silverlight.Utilities.Validation.ValidateType.Email)]
        public string ContactEmail
        {
            get { return contactEmail; }
            set { base.SetValue("ContactEmail", ref contactEmail, value); }
        }

        private string phoneNumber;
        /// <summary>
        /// 联系电话
        /// </summary>
        [Validate(Newegg.Oversea.Silverlight.Utilities.Validation.ValidateType.Required)]
        //[Validate(Newegg.Oversea.Silverlight.Utilities.Validation.ValidateType.Regex, @"^((\d{11})|(\d{3,4}[ -]?\d{7,8}))$", ErrorMessageResourceName = "Msg_Phone_Format", ErrorMessageResourceType = typeof(ResWarehouseQuery))]
        public string PhoneNumber
        {
            get { return phoneNumber; }
            set { base.SetValue("PhoneNumber", ref phoneNumber, value); }
        }

        private string receiveAddress;
        /// <summary>
        /// 收货地址
        /// </summary>
        [Validate(Newegg.Oversea.Silverlight.Utilities.Validation.ValidateType.Required)]
        public string ReceiveAddress
        {
            get { return receiveAddress; }
            set { base.SetValue("ReceiveAddress", ref receiveAddress, value); }
        }

        private string receiveContact;
        /// <summary>
        /// 收货联系人
        /// </summary>
        [Validate(Newegg.Oversea.Silverlight.Utilities.Validation.ValidateType.Required)]
        public string ReceiveContact
        {
            get { return receiveContact; }
            set { base.SetValue("ReceiveContact", ref receiveContact, value); }
        }

        private string receiveContactPhoneNumber;
        /// <summary>
        /// 收货联系电话
        /// </summary>
        [Validate(Newegg.Oversea.Silverlight.Utilities.Validation.ValidateType.Required)]
        //[Validate(Newegg.Oversea.Silverlight.Utilities.Validation.ValidateType.Regex, @"^((\d{11})|(\d{3,4}[ -]?\d{7,8}))$", ErrorMessageResourceName = "Msg_Phone_Format", ErrorMessageResourceType = typeof(ResWarehouseQuery))]
        public string ReceiveContactPhoneNumber
        {
            get { return receiveContactPhoneNumber; }
            set { base.SetValue("ReceiveContactPhoneNumber", ref receiveContactPhoneNumber, value); }
        }

        private decimal transferRate=0.00M;
        /// <summary>
        /// 移仓分仓系数
        /// </summary>
        [Validate(ValidateType.Required)]
        [Validate(ValidateType.Regex, @"^[0-9]{1}(\.[0-9]{1,2})$", ErrorMessageResourceName = "Msg_TransferRate_Format_IPP", ErrorMessageResourceType = typeof(ResWarehouseQuery))]
        public decimal TransferRate
        {
            get { return transferRate; }
            set { base.SetValue("TransferRate", ref transferRate, value); }
        }


        public ValidStatus warehouseStatus;
        /// <summary>
        /// 仓库类型
        /// </summary>
        [Validate(Newegg.Oversea.Silverlight.Utilities.Validation.ValidateType.Required)]
        public ValidStatus WarehouseStatus
        {
            get { return warehouseStatus; }
            set { base.SetValue("WarehouseStatus", ref warehouseStatus, value); }
        }

        /// <summary>
        /// 创建人
        /// </summary>
        private int? createUserSysNo;

        public int? CreateUserSysNo
        {
            get { return createUserSysNo; }
            set { base.SetValue("CreateUserSysNo", ref createUserSysNo, value); }
        }

        /// <summary>
        /// 创建时间
        /// </summary>
        private DateTime? createDate;

        public DateTime? CreateDate
        {
            get { return createDate; }
            set { base.SetValue("CreateDate", ref createDate, value); }
        }

        /// <summary>
        /// 编辑人
        /// </summary>
        private int? editUserSysNo;

        public int? EditUserSysNo
        {
            get { return editUserSysNo; }
            set { base.SetValue("EditUserSysNo", ref editUserSysNo, value); }
        }

        /// <summary>
        /// 编辑时间
        /// </summary>
        private DateTime? editDate;

        public DateTime? EditDate
        {
            get { return editDate; }
            set { base.SetValue("EditDate", ref editDate, value); }
        }


        private string countryCode;
        /// <summary>
        /// 国家代码
        /// </summary>
        [Validate(Newegg.Oversea.Silverlight.Utilities.Validation.ValidateType.Required)]
        public string CountryCode
        {
            get { return countryCode; }
            set { base.SetValue("CountryCode", ref countryCode, value); }
        }

        private string zip;
        /// <summary>
        /// 邮编
        /// </summary>
        public string Zip
        {
            get { return zip; }
            set { base.SetValue("Zip", ref zip, value); }
        }

        private string companyName;
        /// <summary>
        /// 国家
        /// </summary>
        public string CompanyName
        {
            get { return companyName; }
            set { base.SetValue("CompanyName", ref companyName, value); }
        }

        private string province;
        /// <summary>
        /// 省份
        /// </summary>
        public string Province
        {
            get { return province; }
            set { base.SetValue("Province", ref province, value); }
        }

        private string city;
        /// <summary>
        /// 城市
        /// </summary>
        public string City
        {
            get { return city; }
            set { base.SetValue("City", ref city, value); }
        }


        private TradeType stockType;

        /// <summary>
        /// 贸易类型
        /// </summary>
        public TradeType StockType
        {
            get { return stockType; }
            set { base.SetValue("StockType", ref stockType, value); }
        }

        private CustomsCodeMode customsCode;

        /// <summary>
        /// 海关关区
        /// </summary>
        public CustomsCodeMode CustomsCode
        {
            get { return customsCode; }
            set { base.SetValue("CustomsCode", ref customsCode, value); }
        }

        #region 页面初始数据源

        private List<KeyValuePair<ValidStatus?, string>> validStatusList;
        public List<KeyValuePair<ValidStatus?, string>> ValidStatusList
        {
            get
            {
                validStatusList = validStatusList ?? EnumConverter.GetKeyValuePairs<ValidStatus>();
                return validStatusList;
            }
        }


        private List<KeyValuePair<WarehouseType?, string>> warehouseTypeList;
        public List<KeyValuePair<WarehouseType?, string>> WarehouseTypeList
        {
            get
            {
                warehouseTypeList = warehouseTypeList ?? EnumConverter.GetKeyValuePairs<WarehouseType>();
                return warehouseTypeList;
            }
        }
        private List<WarehouseOwnerInfoVM> ownerList;
        public List<WarehouseOwnerInfoVM> OwnerList
        {
            get { return ownerList; }
            set { SetValue("OwnerList", ref ownerList, value); }
        }


        private List<KeyValuePair<TradeType?, string>> stockTypeList;
        public List<KeyValuePair<TradeType?, string>> StockTypeList
        {
            get
            {
                stockTypeList = stockTypeList ?? EnumConverter.GetKeyValuePairs<TradeType>();
                return stockTypeList;
            }
        }

        private List<KeyValuePair<CustomsCodeMode?, string>> customsCodeList;
        public List<KeyValuePair<CustomsCodeMode?, string>> CustomsCodeList
        {
            get
            {
                customsCodeList = customsCodeList ?? EnumConverter.GetKeyValuePairs<CustomsCodeMode>();
                return customsCodeList;
            }
        }

        #endregion

        ///// <summary>
        ///// 渠道仓库列表
        ///// </summary>
        //private List<StockInfoVM> stockInfoList;

        //public List<StockInfoVM> StockInfoList
        //{
        //    get { return stockInfoList; }
        //    set { base.SetValue("StockInfoList", ref stockInfoList, value); }
        //}
    }

}
