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
using ECCentral.QueryFilter.Common;
using ECCentral.BizEntity.Inventory;
using System.Collections.Generic;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.Portal.UI.Inventory.Resources;

namespace ECCentral.Portal.UI.Inventory.Models
{
    public class AdjustRequestQueryVM : ModelBase
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
        /// 
        [Validate(ValidateType.Regex, @"^[\d]*$", ErrorMessageResourceName = "Msg_RequestID_Format", ErrorMessageResourceType = typeof(ResAdjustRequestQuery))]
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


        private AdjustRequestStatus? requestStatus;
        public AdjustRequestStatus? RequestStatus
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

        private bool? isNegative;
        public bool? IsNegative
        {
            get { return isNegative; }
            set { base.SetValue("IsNegative", ref isNegative, value); }
        }

        private AdjustRequestProperty? adjustProperty;
        public AdjustRequestProperty? AdjustProperty
        {
            get { return adjustProperty; }
            set { base.SetValue("AdjustProperty", ref adjustProperty, value); }
        }

        private RequestConsignFlag? consignFlag;
        public RequestConsignFlag? ConsignFlag
        {
            get { return consignFlag; }
            set { base.SetValue("ConsignFlag", ref consignFlag, value); }
        }
        #region 页面初始化数据
        private List<KeyValuePair<AdjustRequestStatus?, string>> adjustRequestStatusList;
        public List<KeyValuePair<AdjustRequestStatus?, string>> AdjustRequestStatusList
        {
            get
            {
                adjustRequestStatusList = adjustRequestStatusList ?? EnumConverter.GetKeyValuePairs<AdjustRequestStatus>(EnumConverter.EnumAppendItemType.All);
                return adjustRequestStatusList;
            }
        }

        private List<KeyValuePair<RequestConsignFlag?, string>> consignFlagList;
        public List<KeyValuePair<RequestConsignFlag?, string>> ConsignFlagList
        {
            get
            {
                consignFlagList = consignFlagList ?? EnumConverter.GetKeyValuePairs<RequestConsignFlag>(EnumConverter.EnumAppendItemType.All);
                return consignFlagList;
            }
        }

        private List<KeyValuePair<AdjustRequestProperty?, string>> adjustRequestPropertyList;
        public List<KeyValuePair<AdjustRequestProperty?, string>> AdjustRequestPropertyList
        {
            get
            {
                adjustRequestPropertyList = adjustRequestPropertyList ?? EnumConverter.GetKeyValuePairs<AdjustRequestProperty>(EnumConverter.EnumAppendItemType.All);
                return adjustRequestPropertyList;
            }
        }
        private List<KeyValuePair<bool?, string>> booleanList;
        public List<KeyValuePair<bool?, string>> BooleanList
        {
            get
            {
                booleanList = booleanList ?? BooleanConverter.GetKeyValuePairs(EnumConverter.EnumAppendItemType.All);
                return booleanList;
            }
        }
        #endregion
    }


    public class AdjustRequestQueryView : ModelBase
    {
        public AdjustRequestQueryVM QueryInfo
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

        public AdjustRequestQueryView()
        {
            QueryInfo = new AdjustRequestQueryVM();
        }
    }
}
