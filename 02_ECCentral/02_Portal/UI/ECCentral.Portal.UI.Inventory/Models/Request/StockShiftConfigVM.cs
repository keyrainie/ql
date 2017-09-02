using System;
using System.Windows;
using System.Windows.Controls;
using System.Collections.Generic;

using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;

using ECCentral.BizEntity.Inventory;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.QueryFilter.Common;
using Newegg.Oversea.Silverlight.Utilities.Validation;
using ECCentral.Portal.UI.Inventory.Resources;

namespace ECCentral.Portal.UI.Inventory.Models
{
    public class StockShiftConfigVM : ModelBase
    {
        public PagingInfo PagingInfo { get; set; }

        private int? sysNo;
        public int? SysNo
        {
            get { return sysNo; }
            set
            {
                SetValue("SysNo", ref sysNo, value);
            }
        }

        private string outStockSysNo;
        [Validate(ValidateType.Required)]
        public string OutStockSysNo
        {
            get { return outStockSysNo; }
            set
            {
                SetValue("OutStockSysNo", ref outStockSysNo, value);
            }
        }

        private string inStockSysNo;
        [Validate(ValidateType.Required)]
        public string InStockSysNo
        {
            get { return inStockSysNo; }
            set
            {
                SetValue("InStockSysNo", ref inStockSysNo, value);
            }
        }

        private string splInterval;
        [Validate(ValidateType.Required)]
        [Validate(ValidateType.Interger, ErrorMessageResourceName = "ValidateMsg_NumberOnly", ErrorMessageResourceType = typeof(ResInventoryCommon))]
        public string SPLInterval
        {
            get { return splInterval; }
            set
            {
                SetValue("SPLInterval", ref splInterval, value);
            }
        }

        private string shipInterval;
        [Validate(ValidateType.Required)]
        [Validate(ValidateType.Interger, ErrorMessageResourceName = "ValidateMsg_NumberOnly", ErrorMessageResourceType = typeof(ResInventoryCommon))]
        public string ShipInterval
        {
            get { return shipInterval; }
            set
            {
                SetValue("ShipInterval", ref shipInterval, value);
            }
        }

        private string shiftType;
        [Validate(ValidateType.Required)]
        public string ShiftType
        {
            get { return shiftType; }
            set
            {
                SetValue("ShiftType", ref shiftType, value);
            }
        }

        private string companyCode;
        public string CompanyCode
        {
            get { return companyCode; }
            set
            {
                SetValue("CompanyCode", ref companyCode, value);
            }
        }

        private List<CodeNamePair> shiftShippingTypeList;
        public List<CodeNamePair> ShiftShippingTypeList
        {
            get { return shiftShippingTypeList; }
            set { SetValue("ShiftShippingTypeList", ref shiftShippingTypeList, value); }
        }

        #region UI Model
        public bool IsCreateMode
        {
            get
            {
                return !this.SysNo.HasValue || this.SysNo <= 0;
            }
        }
        #endregion UI Model
    }

}
