using System;
using System.Windows;
using System.Windows.Controls;
using System.Collections.Generic;

using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using Newegg.Oversea.Silverlight.Utilities.Validation;

using ECCentral.BizEntity.Inventory;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.QueryFilter.Common;
using ECCentral.Portal.UI.Inventory.Resources;

namespace ECCentral.Portal.UI.Inventory.Models
{
    public class LendRequestQueryVM : ModelBase
    {      
        public PagingInfo PagingInfo { get; set; }

        /// <summary>
        /// PM权限
        /// </summary>
        public BizEntity.Common.PMQueryType? PMQueryRightType { get; set; }
 
        #region 属性
        public int? UserSysNo { get; set; }
        private string requestID;

        [Validate(ValidateType.Regex, @"^[\d]*$", ErrorMessageResourceName = "Msg_RequestID_Format", ErrorMessageResourceType = typeof(ResLendRequestQuery))]
        public string RequestID
        {
            get
            {
                return requestID;
            }
            set
            {
                base.SetValue("RequestID", ref requestID, value);
            }
        }

        private string productID;
        public string ProductID
        {
            get
            {
                return productID;
            }
            set
            {
                base.SetValue("ProductID", ref productID, value);
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
                base.SetValue("ProductSysNo", ref productSysNo, value);
            }
        }

        private int? lendUserSysNo;
        public int? LendUserSysNo
        {
            get
            {
                return lendUserSysNo;
            }
            set
            {
                base.SetValue("LendUserSysNo", ref lendUserSysNo, value);
            }
        }

        private int? pmUserSysNo;
        public int? PMUserSysNo
        {
            get
            {
                return pmUserSysNo;
            }
            set
            {
                base.SetValue("PMUserSysNo", ref pmUserSysNo, value);
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
                base.SetValue("StockSysNo", ref stockSysNo, value);
            }
        }

        private string stockName;
        public string StockName
        {
            get
            {
                return stockName;
            }
            set
            {
                base.SetValue("StockName", ref stockName, value);
            }
        }

        private LendRequestStatus? requestStatus;
        public LendRequestStatus? RequestStatus
        {
            get
            {
                return requestStatus;
            }
            set
            {
                base.SetValue("RequestStatus", ref requestStatus, value);
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
                base.SetValue("CreateDateFrom", ref createDateFrom, value);   
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
                base.SetValue("CreateDateTo", ref createDateTo, value);
            }
        }

        private string companyCode;
        public string CompanyCode
        {
            get
            {
                return companyCode;
            }
            set
            {
                base.SetValue("CompanyCode", ref companyCode, value);
            }
        }        

        #endregion 属性
        #region 页面初始化数据
        private List<KeyValuePair<LendRequestStatus?, string>> lendRequestStatusList;
        public List<KeyValuePair<LendRequestStatus?, string>> LendRequestStatusList
        {
            get
            {
                lendRequestStatusList = lendRequestStatusList ?? EnumConverter.GetKeyValuePairs<LendRequestStatus>(EnumConverter.EnumAppendItemType.All);
                return lendRequestStatusList;
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

    public class LendRequestQueryView : ModelBase
    {
        public LendRequestQueryVM QueryInfo
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

        public LendRequestQueryView()
        {
            QueryInfo = new LendRequestQueryVM();
        }
    }
}
