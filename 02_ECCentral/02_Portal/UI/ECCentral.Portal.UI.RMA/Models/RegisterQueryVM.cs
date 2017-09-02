using System;
using System.Net;
using System.Windows;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using ECCentral.Portal.Basic.Components.Models;
using System.Collections.Generic;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.BizEntity.RMA;
using ECCentral.BizEntity.Common;
using Newegg.Oversea.Silverlight.Utilities.Validation;
using System.Collections.ObjectModel;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using ECCentral.BizEntity.Enum.Resources;

namespace ECCentral.Portal.UI.RMA.Models
{
    public class RegisterQueryVM : ModelBase
    {
        public RegisterQueryVM()
        {
            this.WebChannelList = CPApplication.Current.CurrentWebChannelList.ToList<UIWebChannel>();
            //修改UIWebChannelType.publicChennel 后放开
            //this.WebChannelList.Insert(0, new UIWebChannel { ChannelName = ResCommonEnum.Enum_All, ChannelType = UIWebChannelType.publicChennel });
            this.SellerTypeList = EnumConverter.GetKeyValuePairs<SellersType>(EnumConverter.EnumAppendItemType.All);  
            this.YNList = BooleanConverter.GetKeyValuePairs(EnumConverter.EnumAppendItemType.All);
            this.RevertStatusList = EnumConverter.GetKeyValuePairs<RMARevertStatus>(EnumConverter.EnumAppendItemType.All);
            this.NewProductStatusList = EnumConverter.GetKeyValuePairs<RMANewProductStatus>(EnumConverter.EnumAppendItemType.All);
            this.OutBoundStatusList = EnumConverter.GetKeyValuePairs<RMAOutBoundStatus>(EnumConverter.EnumAppendItemType.All);
            this.ReturnStatusList = EnumConverter.GetKeyValuePairs<RMAReturnStatus>(EnumConverter.EnumAppendItemType.All);
            this.RefundStatusList = EnumConverter.GetKeyValuePairs<RMARefundStatus>(EnumConverter.EnumAppendItemType.All);
            this.RMARequestTypeList = EnumConverter.GetKeyValuePairs<RMARequestType>(EnumConverter.EnumAppendItemType.All);
            this.RMANextHandlerList = EnumConverter.GetKeyValuePairs<RMANextHandler>(EnumConverter.EnumAppendItemType.All);
            this.CompareSymbolList = EnumConverter.GetKeyValuePairs<CompareSymbolType>();
            this.PMUserList = new List<UserInfo>();
            this.SellerOperationInfoList = new List<CodeNamePair>();
            this.RMAReasonList = new List<CodeNamePair>();
            this.IsUnCheck = this.IsUnOutbound = this.IsUnReceive = this.IsUnRefund = this.IsUnResponse = this.IsUnReturn = this.IsUnRevert = false;
            this.CheckBoxList = new ObservableCollection<CheckBoxVM>();
            this.QuickSearchCondition = new QuickSearchConditionVM();

            List<KeyValuePair<RMARequestStatus?,string>> tmpList = EnumConverter.GetKeyValuePairs<RMARequestStatus>(EnumConverter.EnumAppendItemType.All);
            List<KeyValuePair<RMARequestStatus?, string>> list = new List<KeyValuePair<RMARequestStatus?, string>>();
            foreach (var item in tmpList)
            {
                if (item.Key == null
                    ||item.Key == RMARequestStatus.Origin
                    || item.Key == RMARequestStatus.Handling
                    || item.Key == RMARequestStatus.Complete
                    || item.Key == RMARequestStatus.Abandon)
                {
                    list.Add(item);
                }
            }
            this.RequestStatusList = list;
        }
        #region 基本查询条件
        private String m_RegisterSysNo;
        [Validate(ValidateType.Interger)]
        public String RegisterSysNo
        {
            get { return this.m_RegisterSysNo; }
            set { base.SetValue("RegisterSysNo", ref m_RegisterSysNo, value); }
        }

        private String m_RequestID;
        public String RequestID
        {
            get { return this.m_RequestID; }
            set { base.SetValue("RequestID", ref m_RequestID, value); }
        }

        private String m_SOSysNo;
        [Validate(ValidateType.Interger)]
        public String SOSysNo
        {
            get { return this.m_SOSysNo; }
            set { base.SetValue("SOSysNo", ref m_SOSysNo, value); }
        }

        private RMARequestType? m_RequestType;
        public RMARequestType? RequestType
        {
            get { return this.m_RequestType; }
            set { base.SetValue("RequestType", ref m_RequestType, value); }
        }

        private string m_CustomerSysNo;
        [Validate(ValidateType.Interger)]
        public string CustomerSysNo
        {
            get { return this.m_CustomerSysNo; }
            set { base.SetValue("CustomerSysNo", ref m_CustomerSysNo, value); }
        }

        private string m_CustomerID;
        public string CustomerID
        {
            get { return this.m_CustomerID; }
            set { base.SetValue("CustomerID", ref m_CustomerID, value); }
        }

        private int? m_ProductSysNo;
        public int? ProductSysNo
        {
            get { return this.m_ProductSysNo; }
            set { base.SetValue("ProductSysNo", ref m_ProductSysNo, value); }
        }
        private string m_ProductID;
        public string ProductID
        {
            get { return this.m_ProductID; }
            set { base.SetValue("ProductID", ref m_ProductID, value); }
        }
        private SellersType? sellersType;
        public SellersType? SellersType
        {
            get { return this.sellersType; }
            set { base.SetValue("SellersType", ref sellersType, value); }
        }

        private Boolean? m_IsLabelPrinted;
        public Boolean? IsLabelPrinted
        {
            get { return this.m_IsLabelPrinted; }
            set { base.SetValue("IsLabelPrinted", ref m_IsLabelPrinted, value); }
        }

        private String m_SellerOperationInfo;
        public String SellerOperationInfo
        {
            get { return this.m_SellerOperationInfo; }
            set { base.SetValue("SellerOperationInfo", ref m_SellerOperationInfo, value); }
        }

        private String m_CompanyCode;
        public String CompanyCode
        {
            get { return this.m_CompanyCode; }
            set { base.SetValue("CompanyCode", ref m_CompanyCode, value); }
        }

        private String m_ChannelID;
        public String ChannelID
        {
            get { return this.m_ChannelID; }
            set { base.SetValue("ChannelID", ref m_ChannelID, value); }
        }

        private Boolean? m_IsUnReceive;
        public Boolean? IsUnReceive
        {
            get { return this.m_IsUnReceive; }
            set { base.SetValue("IsUnReceive", ref m_IsUnReceive, value); }
        }

        private Boolean? m_IsUnCheck;
        public Boolean? IsUnCheck
        {
            get { return this.m_IsUnCheck; }
            set { base.SetValue("IsUnCheck", ref m_IsUnCheck, value); }
        }

        private Boolean? m_IsUnOutbound;
        public Boolean? IsUnOutbound
        {
            get { return this.m_IsUnOutbound; }
            set { base.SetValue("IsUnOutbound", ref m_IsUnOutbound, value); }
        }

        private Boolean? m_IsUnResponse;
        public Boolean? IsUnResponse
        {
            get { return this.m_IsUnResponse; }
            set { base.SetValue("IsUnResponse", ref m_IsUnResponse, value); }
        }

        private Boolean? m_IsUnRefund;
        public Boolean? IsUnRefund
        {
            get { return this.m_IsUnRefund; }
            set { base.SetValue("IsUnRefund", ref m_IsUnRefund, value); }
        }

        private Boolean? m_IsUnReturn;
        public Boolean? IsUnReturn
        {
            get { return this.m_IsUnReturn; }
            set { base.SetValue("IsUnReturn", ref m_IsUnReturn, value); }
        }

        private Boolean? m_IsUnRevert;
        public Boolean? IsUnRevert
        {
            get { return this.m_IsUnRevert; }
            set { base.SetValue("IsUnRevert", ref m_IsUnRevert, value); }
        }
        #endregion
        #region 高级查询条件

        private bool m_IsMoreQueryBuilderCheck = false;
        public bool IsMoreQueryBuilderCheck
        {
            get { return this.m_IsMoreQueryBuilderCheck; }
            set { base.SetValue("IsMoreQueryBuilderCheck", ref m_IsMoreQueryBuilderCheck, value); }
        }
        private DateTime? m_CreateTimeFrom;
        public DateTime? CreateTimeFrom
        {
            get { return this.m_CreateTimeFrom; }
            set { base.SetValue("CreateTimeFrom", ref m_CreateTimeFrom, value); }
        }

        private DateTime? m_CreateTimeTo;
        public DateTime? CreateTimeTo
        {
            get { return this.m_CreateTimeTo; }
            set { base.SetValue("CreateTimeTo", ref m_CreateTimeTo, value); }
        }

        private DateTime? m_RecvTimeFrom;
        public DateTime? RecvTimeFrom
        {
            get { return this.m_RecvTimeFrom; }
            set { base.SetValue("RecvTimeFrom", ref m_RecvTimeFrom, value); }
        }

        private DateTime? m_RecvTimeTo;
        public DateTime? RecvTimeTo
        {
            get { return this.m_RecvTimeTo; }
            set { base.SetValue("RecvTimeTo", ref m_RecvTimeTo, value); }
        }

        private DateTime? m_CheckTimeFrom;
        public DateTime? CheckTimeFrom
        {
            get { return this.m_CheckTimeFrom; }
            set { base.SetValue("CheckTimeFrom", ref m_CheckTimeFrom, value); }
        }

        private DateTime? m_CheckTimeTo;
        public DateTime? CheckTimeTo
        {
            get { return this.m_CheckTimeTo; }
            set { base.SetValue("CheckTimeTo", ref m_CheckTimeTo, value); }
        }

        private DateTime? m_OutboundTimeFrom;
        public DateTime? OutboundTimeFrom
        {
            get { return this.m_OutboundTimeFrom; }
            set { base.SetValue("OutboundTimeFrom", ref m_OutboundTimeFrom, value); }
        }

        private DateTime? m_OutboundTimeTo;
        public DateTime? OutboundTimeTo
        {
            get { return this.m_OutboundTimeTo; }
            set { base.SetValue("OutboundTimeTo", ref m_OutboundTimeTo, value); }
        }

        private DateTime? m_ResponseTimeFrom;
        public DateTime? ResponseTimeFrom
        {
            get { return this.m_ResponseTimeFrom; }
            set { base.SetValue("ResponseTimeFrom", ref m_ResponseTimeFrom, value); }
        }

        private DateTime? m_ResponseTimeTo;
        public DateTime? ResponseTimeTo
        {
            get { return this.m_ResponseTimeTo; }
            set { base.SetValue("ResponseTimeTo", ref m_ResponseTimeTo, value); }
        }

        private DateTime? m_RefundTimeFrom;
        public DateTime? RefundTimeFrom
        {
            get { return this.m_RefundTimeFrom; }
            set { base.SetValue("RefundTimeFrom", ref m_RefundTimeFrom, value); }
        }

        private DateTime? m_RefundTimeTo;
        public DateTime? RefundTimeTo
        {
            get { return this.m_RefundTimeTo; }
            set { base.SetValue("RefundTimeTo", ref m_RefundTimeTo, value); }
        }

        private DateTime? m_ReturnTimeFrom;
        public DateTime? ReturnTimeFrom
        {
            get { return this.m_ReturnTimeFrom; }
            set { base.SetValue("ReturnTimeFrom", ref m_ReturnTimeFrom, value); }
        }

        private DateTime? m_ReturnTimeTo;
        public DateTime? ReturnTimeTo
        {
            get { return this.m_ReturnTimeTo; }
            set { base.SetValue("ReturnTimeTo", ref m_ReturnTimeTo, value); }
        }

        private DateTime? m_RevertTimeFrom;
        public DateTime? RevertTimeFrom
        {
            get { return this.m_RevertTimeFrom; }
            set { base.SetValue("RevertTimeFrom", ref m_RevertTimeFrom, value); }
        }

        private DateTime? m_RevertTimeTo;
        public DateTime? RevertTimeTo
        {
            get { return this.m_RevertTimeTo; }
            set { base.SetValue("RevertTimeTo", ref m_RevertTimeTo, value); }
        }

        private RMARevertStatus? m_RevertStatus;
        public RMARevertStatus? RevertStatus
        {
            get { return this.m_RevertStatus; }
            set { base.SetValue("RevertStatus", ref m_RevertStatus, value); }
        }

        private RMANewProductStatus? m_NewProductStatus;
        public RMANewProductStatus? NewProductStatus
        {
            get { return this.m_NewProductStatus; }
            set { base.SetValue("NewProductStatus", ref m_NewProductStatus, value); }
        }

        private RMAOutBoundStatus? m_OutBoundStatus;
        public RMAOutBoundStatus? OutBoundStatus
        {
            get { return this.m_OutBoundStatus; }
            set { base.SetValue("OutBoundStatus", ref m_OutBoundStatus, value); }
        }

        private RMAReturnStatus? m_ReturnStatus;
        public RMAReturnStatus? ReturnStatus
        {
            get { return this.m_ReturnStatus; }
            set { base.SetValue("ReturnStatus", ref m_ReturnStatus, value); }
        }

        private RMARequestStatus? m_RequestStatus;
        public RMARequestStatus? RequestStatus
        {
            get { return this.m_RequestStatus; }
            set { base.SetValue("RequestStatus", ref m_RequestStatus, value); }
        }

        private RMARefundStatus? m_RefundStatus;
        public RMARefundStatus? RefundStatus
        {
            get { return this.m_RefundStatus; }
            set { base.SetValue("RefundStatus", ref m_RefundStatus, value); }
        }

        private Int32? m_PMUserSysNo;
        public Int32? PMUserSysNo
        {
            get { return this.m_PMUserSysNo; }
            set { base.SetValue("PMUserSysNo", ref m_PMUserSysNo, value); }
        }

        private RMANextHandler? m_NextHandler;
        public RMANextHandler? NextHandler
        {
            get { return this.m_NextHandler; }
            set { base.SetValue("NextHandler", ref m_NextHandler, value); }
        }

        private Boolean? m_IsVIP;
        public Boolean? IsVIP
        {
            get { return this.m_IsVIP; }
            set { base.SetValue("IsVIP", ref m_IsVIP, value); }
        }

        private Boolean? m_IsWithin7Days;
        public Boolean? IsWithin7Days
        {
            get { return this.m_IsWithin7Days; }
            set { base.SetValue("IsWithin7Days", ref m_IsWithin7Days, value); }
        }

        private Int32? m_RMAReason;
        public Int32? RMAReason
        {
            get { return this.m_RMAReason; }
            set { base.SetValue("RMAReason", ref m_RMAReason, value); }
        }

        private Boolean? m_IsRecommendRefund;
        public Boolean? IsRecommendRefund
        {
            get { return this.m_IsRecommendRefund; }
            set { base.SetValue("IsRecommendRefund", ref m_IsRecommendRefund, value); }
        }

        private Boolean? m_IsRepeatRegister;
        public Boolean? IsRepeatRegister
        {
            get { return this.m_IsRepeatRegister; }
            set { base.SetValue("IsRepeatRegister", ref m_IsRepeatRegister, value); }
        }
        private Int32? m_Category1SysNo;
        public Int32? Category1SysNo
        {
            get { return this.m_Category1SysNo; }
            set { base.SetValue("Category3SysNo", ref m_Category1SysNo, value); }
        }
        private Int32? m_Category2SysNo;
        public Int32? Category2SysNo
        {
            get { return this.m_Category2SysNo; }
            set { base.SetValue("Category3SysNo", ref m_Category2SysNo, value); }
        }
        private Int32? m_Category3SysNo;
        public Int32? Category3SysNo
        {
            get { return this.m_Category3SysNo; }
            set { base.SetValue("Category3SysNo", ref m_Category3SysNo, value); }
        }
        private string m_ProductCount;
        [Validate(ValidateType.Interger)]
        public string ProductCount
        {
            get { return this.m_ProductCount; }
            set { base.SetValue("ProductCount", ref m_ProductCount, value); }
        }
        private CompareSymbolType? m_CompareSymbol;
        public CompareSymbolType? CompareSymbol
        {
            get { return this.m_CompareSymbol; }
            set { base.SetValue("CompareSymbol", ref m_CompareSymbol, value); }
        }
        private string nextHandlerList;
        public string NextHandlerList
        {
            get
            {
                return nextHandlerList;
            }
            set
            {
                base.SetValue("NextHandlerList", ref nextHandlerList, value);
            }
        }
        #endregion

        #region 扩展
        public List<UIWebChannel> WebChannelList { get; set; }
        public List<KeyValuePair<Boolean?, string>> YNList { get; set; }
        public List<KeyValuePair<CompareSymbolType?, string>> CompareSymbolList { get; set; }

        public List<KeyValuePair<RMARevertStatus?, string>> RevertStatusList { get; set; }
        public List<KeyValuePair<RMANewProductStatus?, string>> NewProductStatusList { get; set; }
        public List<KeyValuePair<RMAOutBoundStatus?, string>> OutBoundStatusList { get; set; }
        public List<KeyValuePair<RMAReturnStatus?, string>> ReturnStatusList { get; set; }
        public List<KeyValuePair<RMARequestStatus?, string>> RequestStatusList { get; set; }
        public List<KeyValuePair<RMARefundStatus?, string>> RefundStatusList { get; set; }
        public List<KeyValuePair<RMARequestType?, string>> RMARequestTypeList { get; set; }
        public List<KeyValuePair<RMANextHandler?, string>> RMANextHandlerList { get; set; }
        public List<UserInfo> PMUserList { get; set; }

        public List<KeyValuePair<SellersType?, string>> SellerTypeList { get; set; }

        private List<CodeNamePair> sellerOperationInfoList;
        public List<CodeNamePair> SellerOperationInfoList
        {
            get
            {
                return sellerOperationInfoList;
            }
            set
            {
                base.SetValue("SellerOperationInfoList", ref sellerOperationInfoList, value);
            }
        }
        private List<CodeNamePair> rMAReasonList;
        public List<CodeNamePair> RMAReasonList
        {
            get
            {
                return rMAReasonList;
            }
            set
            {
                base.SetValue("RMAReasonList", ref rMAReasonList, value);
            }
        }
        


        private ObservableCollection<CheckBoxVM> _CheckBoxVM;
        public ObservableCollection<CheckBoxVM> CheckBoxList
        {
            get { return _CheckBoxVM; }
            set { base.SetValue("CheckBoxList", ref _CheckBoxVM, value); }
        }
        private bool isQuickSearch;
        public bool IsQuickSearch
        {
            get { return isQuickSearch; }
            set { base.SetValue("IsQuickSearch", ref isQuickSearch, value); }
        }
        private QuickSearchConditionVM quickSearchCondition;
        public QuickSearchConditionVM QuickSearchCondition
        {
            get { return quickSearchCondition; }
            set { base.SetValue("QuickSearchCondition", ref quickSearchCondition, value); }
        }
       
        #endregion
    }

    public class CheckBoxVM : ModelBase
    {
        private bool isChecked;

        public bool IsChecked
        {
            get { return isChecked; }
            set { base.SetValue("IsChecked", ref isChecked, value); }
        }
        private string _name;
        public string Name
        {
            get { return _name; }
            set { base.SetValue("Name", ref _name, value); }
        }
        private string _code;
        public string Code
        {
            get { return _code; }
            set { base.SetValue("Code", ref _code, value); }
        }
    }


    public class QuickSearchConditionVM : ModelBase
    {
        private int? recvTimeFromDiffDays;
        public int? RecvTimeFromDiffDays
        {
            get { return recvTimeFromDiffDays; }
            set { base.SetValue("RecvTimeFromDiffDays", ref recvTimeFromDiffDays, value); }
        }
        private int? recvTimeToDiffDays;
        public int? RecvTimeToDiffDays
        {
            get { return recvTimeToDiffDays; }
            set { base.SetValue("RecvTimeToDiffDays", ref recvTimeToDiffDays, value); }
        }
        private RMARequestStatus? requestStatus;
        public RMARequestStatus? RequestStatus
        {
            get { return requestStatus; }
            set { base.SetValue("RequestStatus", ref requestStatus, value); }
        }
        private bool? isWithin7Days;
        public bool? IsWithin7Days
        {
            get { return isWithin7Days; }
            set { base.SetValue("IsWithin7Days", ref isWithin7Days, value); }
        }
    }

}
