using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using ECCentral.Service.Utility;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using Newegg.Oversea.Silverlight.Utilities.Validation;

using ECCentral.BizEntity.Inventory;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.QueryFilter.Common;
using ECCentral.BizEntity.SO;
using ECCentral.Portal.UI.Inventory.Resources;

namespace ECCentral.Portal.UI.Inventory.Models
{
    public class ShiftRequestQueryVM : ModelBase
    {
        public PagingInfo PagingInfo { get; set; }

        /// <summary>
        /// PM权限
        /// </summary>
        public BizEntity.Common.PMQueryType? PMQueryRightType { get; set; }

        public string UserName { get; set; }
        public int? UserSysNo { get; set; }
        private string companyCode;
        public string CompanyCode
        {
            get { return companyCode; }
            set { base.SetValue("CompanyCode", ref companyCode, value); }
        }

        private string requestID;
        /// <summary>
        ///  单据编号
        /// </summary>
        [Validate(ValidateType.Regex, @"^[\d]*$", ErrorMessageResourceName = "Msg_RequestID_Format", ErrorMessageResourceType = typeof(ResShiftRequestQuery))]
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


        private int? sourceStockSysNo;
        public int? SourceStockSysNo
        {
            get
            {
                return sourceStockSysNo;
            }
            set
            {
                SetValue("SourceStockSysNo", ref sourceStockSysNo, value);
            }
        }

        private int? targetStockSysNo;
        public int? TargetStockSysNo
        {
            get
            {
                return targetStockSysNo;
            }
            set
            {
                SetValue("TargetStockSysNo", ref targetStockSysNo, value);
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

        private ShiftRequestStatus? requestStatus;
        public ShiftRequestStatus? RequestStatus
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

        private DateTime? outStockDateFrom;
        public DateTime? OutStockDateFrom
        {
            get
            {
                return outStockDateFrom;
            }
            set
            {
                SetValue("OutStockDateFrom", ref outStockDateFrom, value);
            }
        }

        private DateTime? outStockDateTo;
        public DateTime? OutStockDateTo
        {
            get
            {
                return outStockDateTo;
            }
            set
            {
                SetValue("OutStockDateTo", ref outStockDateTo, value);
            }
        }

        private DateTime? inStockDateFrom;
        public DateTime? InStockDateFrom
        {
            get
            {
                return inStockDateFrom;
            }
            set
            {
                SetValue("InStockDateFrom", ref inStockDateFrom, value);
            }
        }

        private DateTime? inStockDateTo;
        public DateTime? InStockDateTo
        {
            get
            {
                return inStockDateTo;
            }
            set
            {
                SetValue("InStockDateTo", ref inStockDateTo, value);
            }
        }

        private string shiftShippingType;
        public string ShiftShippingType
        {
            get
            {
                return shiftShippingType;
            }
            set
            {
                SetValue("ShiftShippingType", ref shiftShippingType, value);
            }
        }

        private ShiftRequestType? shiftRquestType;
        public ShiftRequestType? ShiftRquestType
        {
            get
            {
                return shiftRquestType;
            }
            set
            {
                SetValue("ShiftRquestType", ref shiftRquestType, value);
            }
        }

        private bool? isSpecialShift;
        public bool? IsSpecialShift
        {
            get
            {
                return isSpecialShift;
            }
            set
            {
                SetValue("IsSpecialShift", ref isSpecialShift, value);
            }
        }

        private RequestConsignFlag? consignFlag;
        public RequestConsignFlag? ConsignFlag
        {
            get { return consignFlag; }
            set { base.SetValue("ConsignFlag", ref consignFlag, value); }
        }

        private SpecialShiftRequestType? specialStatus;
        public SpecialShiftRequestType? SpecialShiftRequestStatus
        {
            get { return specialStatus; }
            set { base.SetValue("SpecialShiftRequestStatus", ref specialStatus, value); }
        }

        private SOStatus? soStatus;
        public SOStatus? SOStatus
        {
            get { return soStatus; }
            set { base.SetValue("SOStatus", ref soStatus, value); }
        }

        public List<int> SOSysNoList 
        {
            get
            {
                List<int> soNoList = new List<int>();
                if (!string.IsNullOrEmpty(SOSysNo) && Regex.IsMatch(SOSysNo, @"^[. ,\d]*$"))
                {
                    string[] noList = SOSysNo.Split(new char[] { ' ', ',', '.' }, StringSplitOptions.RemoveEmptyEntries);
                    if (noList != null && noList.Length > 0)
                    {
                        noList.ForEach(no =>
                        {
                            int t;
                            if (int.TryParse(no, out t))
                            {
                                soNoList.Add(t);
                            }
                        });
                    }
                }
                return soNoList;
            }
        }

        private string soSysNo;
        [Validate(ValidateType.Regex, @"^[\d]*$", ErrorMessageResourceName = "Msg_RequestID_Format", ErrorMessageResourceType = typeof(ResShiftRequestQuery))]
        public string SOSysNo
        {

            get { return soSysNo; }
            set { base.SetValue("SOSysNo", ref soSysNo, value); }
        }

        private int? createUserSysNo;
        public int? CreateUserSysNo
        {
            get { return createUserSysNo; }
            set { base.SetValue("CreateUserSysNo", ref createUserSysNo, value); }
        }

        public List<int> TransferSysNumbers { get; set; }

        private string sapDocNo;
        public string SAPDocNo
        {
            get { return sapDocNo; }
            set { base.SetValue("SAPDocNo", ref sapDocNo, value); }
        }

        private DateTime? sapPostDateFrom;
        public DateTime? SAPPostDateFrom
        {
            get { return sapPostDateFrom; }
            set { base.SetValue("SAPPostDateFrom", ref sapPostDateFrom, value); }
        }

        private DateTime? sapPostDateTo;
        public DateTime? SAPPostDateTo
        {
            get { return sapPostDateTo; }
            set { base.SetValue("SAPPostDateTo", ref sapPostDateTo, value); }
        }

        private string sapImportedStatus;
        public string SAPImportedStatus
        {
            get { return sapImportedStatus; }
            set { base.SetValue("SAPImportedStatus", ref sapImportedStatus, value); }
        }

        private string authorizedPMsSysNumber;
        public string AuthorizedPMsSysNumber
        {
            get { return authorizedPMsSysNumber; }
            set { base.SetValue("AuthorizedPMsSysNumber", ref authorizedPMsSysNumber, value); }
        }

        private List<UserInfoVM> createUserList;
        public List<UserInfoVM> CreateUserList
        {
            get { return createUserList; }
            set { base.SetValue("CreateUserList", ref createUserList, value); }
        }
        #region 源数据

        private List<KeyValuePair<RequestConsignFlag?, string>> consignFlagList;
        public List<KeyValuePair<RequestConsignFlag?, string>> ConsignFlagList
        {
            get
            {
                consignFlagList = consignFlagList ?? EnumConverter.GetKeyValuePairs<RequestConsignFlag>(EnumConverter.EnumAppendItemType.All);
                return consignFlagList;
            }
        }


        private List<KeyValuePair<ShiftRequestType?, string>> shiftRequestTypeList;
        public List<KeyValuePair<ShiftRequestType?, string>> ShiftRequestTypeList
        {
            get
            {
                shiftRequestTypeList = shiftRequestTypeList ?? EnumConverter.GetKeyValuePairs<ShiftRequestType>(EnumConverter.EnumAppendItemType.All);
                return shiftRequestTypeList;
            }
        }


        private List<CodeNamePair> shiftShippingTypeList;
        public List<CodeNamePair> ShiftShippingTypeList
        {
            get { return shiftShippingTypeList; }
            set { base.SetValue("ShiftShippingTypeList", ref shiftShippingTypeList, value); }
        }

        private List<KeyValuePair<ShiftRequestStatus?, string>> shiftRequestStatusList;
        public List<KeyValuePair<ShiftRequestStatus?, string>> ShiftRequestStatusList
        {
            get
            {
                shiftRequestStatusList = shiftRequestStatusList ?? EnumConverter.GetKeyValuePairs<ShiftRequestStatus>(EnumConverter.EnumAppendItemType.All);
                return shiftRequestStatusList;
            }
        }


        private List<KeyValuePair<SpecialShiftRequestType?, string>> specialShiftRequestStatusList;
        public List<KeyValuePair<SpecialShiftRequestType?, string>> SpecialShiftRequestStatusList
        {
            get
            {
                specialShiftRequestStatusList = specialShiftRequestStatusList ?? EnumConverter.GetKeyValuePairs<SpecialShiftRequestType>(EnumConverter.EnumAppendItemType.All);
                //去除"非特殊移仓单"的选项
                specialShiftRequestStatusList.RemoveAt(1);
                return specialShiftRequestStatusList;
            }
        }
        private List<KeyValuePair<SOStatus?, string>> soStatusList;
        public List<KeyValuePair<SOStatus?, string>> SOStatusList
        {
            get
            {
                soStatusList = soStatusList ?? EnumConverter.GetKeyValuePairs<SOStatus>(EnumConverter.EnumAppendItemType.All);
                return soStatusList;
            }
        }
        private List<KeyValuePair<Boolean?, string>> booleanList;
        public List<KeyValuePair<Boolean?, string>> BooleanList
        {
            get
            {
                booleanList = booleanList ?? BooleanConverter.GetKeyValuePairs(EnumConverter.EnumAppendItemType.All);
                return booleanList;
            }
        }
        #endregion

        private VirtualTransferType? isVirtualTransfer;
        public VirtualTransferType? IsVirtualTransfer
        {
            get { return isVirtualTransfer; }
            set { base.SetValue("IsVirtualTransfer", ref isVirtualTransfer, value); }
        }

        private List<KeyValuePair<VirtualTransferType?, string>> isVirtualTransferStatusList;
        public List<KeyValuePair<VirtualTransferType?, string>> VirtualTransferTypeStatusList
        {
            get
            {
                isVirtualTransferStatusList = isVirtualTransferStatusList ?? EnumConverter.GetKeyValuePairs<VirtualTransferType>(EnumConverter.EnumAppendItemType.All);
                return isVirtualTransferStatusList;
            }
        }
    }
    public class ShiftRequestQueryView : ModelBase
    {
        public ShiftRequestQueryVM QueryInfo
        {
            get;
            set;
        }

        private List<ShiftRequestVM> result;
        public List<ShiftRequestVM> Result
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

        public ShiftRequestQueryView()
        {
            QueryInfo = new ShiftRequestQueryVM();
        }
    }
}
