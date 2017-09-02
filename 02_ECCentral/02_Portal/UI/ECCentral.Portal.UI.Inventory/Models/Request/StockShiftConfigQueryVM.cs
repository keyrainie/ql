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
    public class StockShiftConfigQueryVM : ModelBase
    {
        public PagingInfo PagingInfo { get; set; }

        private int? outStockSysNo;
        public int? OutStockSysNo 
        {
            get { return outStockSysNo; }
            set
            {
                SetValue("OutStockSysNo", ref outStockSysNo, value);
            }
        }

        private int? inStockSysNo;
        public int? InStockSysNo
        {
            get { return inStockSysNo; }
            set
            {
                SetValue("InStockSysNo", ref inStockSysNo, value);
            }
        }

        private string splInterval;
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
    }

    public class StockShiftConfigView : ModelBase
    {
        public StockShiftConfigQueryVM QueryInfo
        {
            get;
            set;
        }

        private List<dynamic> result;
        public List<dynamic> Result
        {
            get { return result; }
            set
            {
                SetValue("Result", ref result, value);
            }
        }

        private int totalCount;
        public int TotalCount
        {
            get { return totalCount; }
            set
            {
                SetValue<int>("TotalCount", ref totalCount, value);
            }
        }

        public StockShiftConfigView()
        {
            QueryInfo = new StockShiftConfigQueryVM();
        }
    }
}
