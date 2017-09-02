using System;
using System.Windows;
using System.Windows.Controls;
using System.Collections.Generic;

using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using ECCentral.Portal.UI.Inventory.Resources;
using Newegg.Oversea.Silverlight.Utilities.Validation;
using ECCentral.BizEntity.Inventory;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.QueryFilter.Common;


namespace ECCentral.Portal.UI.Inventory.Models
{
    public class ConvertRequestQueryVM : ModelBase
    {
        public PagingInfo PagingInfo { get; set; }

        private int? sysNo;
        public int? SysNo
        {
            get
            {
                return sysNo;
            }
            set
            {
                SetValue("SysNo", ref sysNo, value);
            }
        }
        /// <summary>
        /// PM权限
        /// </summary>
        public BizEntity.Common.PMQueryType? PMQueryRightType { get; set; }


        public int? UserSysNo { get; set; }
        private string requestID;
        /// <summary>
        ///  单据编号
        /// </summary>
        [Validate(ValidateType.Regex, @"^[\d]*$", ErrorMessageResourceName = "Msg_RequestID_Format", ErrorMessageResourceType = typeof(ResConvertRequestQuery))]
        public string RequestID
        {
            get
            {
                return requestID;
            }
            set
            {
                SetValue("RequestID", ref requestID, value);
            }
        }

        private int? productSysNo;
        public int? ProductSysNo
        {
            get
            {
                return productSysNo;
            }
            set
            {
                SetValue("ProductSysNo", ref productSysNo, value);
            }
        }

        private int? stockSysNo;
        public int? StockSysNo
        {
            get
            {
                return stockSysNo;
            }
            set
            {
                SetValue("StockSysNo", ref stockSysNo, value);
            }
        }

        private DateTime? createDateFrom;
        public DateTime? CreateDateFrom
        {
            get
            {
                return createDateFrom;
            }
            set
            {
                SetValue("CreateDateFrom", ref createDateFrom, value);
            }
        }

        private DateTime? createDateTo;
        public DateTime? CreateDateTo
        {
            get
            {
                return createDateTo;
            }
            set
            {
                SetValue("CreateDateTo", ref createDateTo, value);
            }
        }


        private ConvertRequestStatus? requestStatus;
        public ConvertRequestStatus? RequestStatus
        {
            get
            {
                return requestStatus;
            }
            set
            {
                SetValue("RequestStatus", ref requestStatus, value);
            }
        }

        private string companyCode;
        public string CompanyCode
        {
            get { return companyCode; }
            set { base.SetValue("CompanyCode", ref companyCode, value); }
        }
        private List<KeyValuePair<ConvertRequestStatus?, string>> requestStatusList;
        public List<KeyValuePair<ConvertRequestStatus?, string>> RequestStatusList
        {
            get
            {
                requestStatusList = requestStatusList ?? EnumConverter.GetKeyValuePairs<ConvertRequestStatus>(EnumConverter.EnumAppendItemType.All);
                return requestStatusList;
            }
        }

    }

    public class ConvertRequestQueryView : ModelBase
    {
        public ConvertRequestQueryVM QueryInfo
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

        public ConvertRequestQueryView()
        {
            QueryInfo = new ConvertRequestQueryVM();
        }
    }
}
