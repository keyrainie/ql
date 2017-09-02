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
using ECCentral.BizEntity.PO;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using Newegg.Oversea.Silverlight.Utilities.Validation;

namespace ECCentral.Portal.UI.ExternalSYS.Models
{
    public class VendorAgentInfoVM : ModelBase
    {
        private int? agentysNo;

        public int? AgentSysNo
        {
            get { return agentysNo; }
            set { base.SetValue("AgentSysNo", ref agentysNo, value); }
        }
        public VendorAgentInfoVM()
        {
            this.manufacturerInfo = new ManufacturerInfoVM();
            this.BrandInfo = new BrandInfoVM();
        }

        public string RoleString { get; set; }

        public bool? HasChangePrice { get; set; }

        /// <summary>
        /// 厂商信息
        /// </summary>
        private ManufacturerInfoVM manufacturerInfo;

        public ManufacturerInfoVM ManufacturerInfo
        {
            get { return manufacturerInfo; }
            set { base.SetValue("ManufacturerInfo", ref manufacturerInfo, value); }
        }
        /// <summary>
        /// 代理品牌信息
        /// </summary>
        private BrandInfoVM brandInfo;

        public BrandInfoVM BrandInfo
        {
            get { return brandInfo; }
            set { base.SetValue("BrandInfo", ref brandInfo, value); }
        }

        /// <summary>
        /// 代理商品类别信息
        /// </summary>
        private CategoryInfoVM categoryInfo;

        public CategoryInfoVM CategoryInfo
        {
            get { return categoryInfo; }
            set { base.SetValue("CategoryInfo", ref categoryInfo, value); }
        }

        /// <summary>
        /// 代理级别
        /// </summary>
        private string agentLevel;

        public string AgentLevel
        {
            get { return agentLevel; }
            set { base.SetValue("AgentLevel", ref agentLevel, value); }
        }


        /// <summary>
        /// 送货周期
        /// </summary>
        private string sendPeriod;

        public string SendPeriod
        {
            get { return sendPeriod; }
            set { base.SetValue("SendPeriod", ref sendPeriod, value); }
        }

        /// <summary>
        /// 下单日期
        /// </summary>
        private string buyWeekDay;

        public string BuyWeekDay
        {
            get { return buyWeekDay; }
            set { base.SetValue("BuyWeekDay", ref buyWeekDay, value); }
        }

        /// 请求送货周期
        /// </summary>
        private string requestSendPeriod;

        public string RequestSendPeriod
        {
            get { return requestSendPeriod; }
            set { base.SetValue("RequestSendPeriod", ref requestSendPeriod, value); }
        }

        /// <summary>
        /// 请求下单日期
        /// </summary>
        private string requestBuyWeekDay;

        public string RequestBuyWeekDay
        {
            get { return requestBuyWeekDay; }
            set { base.SetValue("RequestBuyWeekDay", ref requestBuyWeekDay, value); }
        }

        /// <summary>
        /// 代理状态
        /// </summary>
        private string agentStatus;

        public string AgentStatus
        {
            get { return agentStatus; }
            set { base.SetValue("AgentStatus", ref agentStatus, value); }
        }

        /// <summary>
        /// 代理2级分类编号
        /// </summary>
        private string c2SysNo;

        [Validate(ValidateType.Required)]
        public string C2SysNo
        {
            get { return c2SysNo; }
            set { base.SetValue("C2SysNo", ref c2SysNo, value); }
        }

        /// <summary>
        /// 代理3级分类编号
        /// </summary>
        private int? c3SysNo;

        public int? C3SysNo
        {
            get { return c3SysNo; }
            set { base.SetValue("C3SysNo", ref c3SysNo, value); }
        }

        /// <summary>
        /// 代理2级分类名称
        /// </summary>
        private string c2Name;

        public string C2Name
        {
            get { return c2Name; }
            set { base.SetValue("C2Name", ref c2Name, value); }
        }

        /// <summary>
        /// 代理3级分类名称
        /// </summary>
        private string c3Name;

        public string C3Name
        {
            get { return c3Name; }
            set { base.SetValue("C3Name", ref c3Name, value); }
        }

        public string CName
        {
            get { return (string.IsNullOrEmpty(c3Name) ? c2Name : c3Name); }
        }

        /// <summary>
        /// 代理类型(代销结算模式)
        /// </summary>
        private SettleType? settleType;

        public SettleType? SettleType
        {
            get { return settleType; }
            set { base.SetValue("SettleType", ref settleType, value); }
        }

        /// <summary>
        /// 佣金百分比
        /// </summary>
        private string settlePercentage;
        [Validate(ValidateType.Required)]
        //[Validate(ValidateType.Regex, @"^[0-9]+\.?[0-9]{0,6}$", ErrorMessageResourceName = "Decimal_Required", ErrorMessageResourceType = typeof(ResValidationErrorMessage))]
        public string SettlePercentage
        {
            get { return settlePercentage; }
            set { base.SetValue("SettlePercentage", ref settlePercentage, value); }
        }


        /// <summary>
        /// 审核状态
        /// </summary>
        private VendorModifyRequestStatus? requestType;

        public VendorModifyRequestStatus? RequestType
        {
            get { return requestType; }
            set { base.SetValue("RequestType", ref requestType, value); }
        }

        /// <summary>
        /// 操作类型
        /// </summary>
        private VendorModifyActionType? checkType;

        public VendorModifyActionType? CheckType
        {
            get { return checkType; }
            set { base.SetValue("CheckType", ref checkType, value); }
        }

        /// <summary>
        /// 操作内容
        /// </summary>
        private string content;

        public string Content
        {
            get { return content; }
            set { base.SetValue("Content", ref content, value); }
        }

        /// <summary>
        /// 对象状态
        /// </summary>
        private VendorRowState rowState;

        public VendorRowState RowState
        {
            get { return rowState; }
            set { base.SetValue("RowState", ref rowState, value); }
        }

        private string companyCode;

        public string CompanyCode
        {
            get { return companyCode; }
            set { base.SetValue("CompanyCode", ref companyCode, value); }
        }

    }
}
