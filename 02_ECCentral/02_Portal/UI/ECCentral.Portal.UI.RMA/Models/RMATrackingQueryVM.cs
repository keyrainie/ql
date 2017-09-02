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
using ECCentral.BizEntity.RMA;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.BizEntity.Common;

namespace ECCentral.Portal.UI.RMA.Models
{
    public class RMATrackingQueryVM : ModelBase
    {
        public RMATrackingQueryVM()
        {
            this.InternalMemoStatusList = EnumConverter.GetKeyValuePairs<InternalMemoStatus>(EnumConverter.EnumAppendItemType.All);
            this.NextHandlerList = EnumConverter.GetKeyValuePairs<RMANextHandler>(EnumConverter.EnumAppendItemType.All);
            this.CreateUserList = new List<UserInfo>();
            this.UpdateUserList = new List<UserInfo>();
        }
        /// <summary>
        /// 单件号
        /// </summary>
        private string registerSysNo;
        [Validate(ValidateType.Interger)]
        public string RegisterSysNo
        {
            get { return registerSysNo; }
            set { base.SetValue("RegisterSysNo", ref registerSysNo, value); }
        }
        /// <summary>
        /// 订单号
        /// </summary>
        private string sOSysNo;
        [Validate(ValidateType.Interger)]
        public string SOSysNo
        {
            get { return sOSysNo; }
            set { base.SetValue("SOSysNo", ref sOSysNo, value); }
        }
        /// <summary>
        /// 下部操作
        /// </summary>
        private RMANextHandler? nextHandler;
        public RMANextHandler? NextHandler
        {
            get { return nextHandler; }
            set { base.SetValue("NextHandler", ref nextHandler, value); }
        }
        /// <summary>
        /// 状态
        /// </summary>
        private InternalMemoStatus? status;
        public InternalMemoStatus? Status
        {
            get { return status; }
            set { base.SetValue("Status", ref status, value); }
        }
        /// <summary>
        /// 创建时间范围开始
        /// </summary>
        private DateTime? createTimeFrom;
        public DateTime? CreateTimeFrom
        {
            get { return createTimeFrom; }
            set { base.SetValue("CreateTimeFrom", ref createTimeFrom, value); }
        }
        /// <summary>
        /// 创建时间范围结束
        /// </summary>
        private DateTime? createTimeTo;
        public DateTime? CreateTimeTo
        {
            get { return createTimeTo; }
            set { base.SetValue("CreateTimeTo", ref createTimeTo, value); }
        }
        /// <summary>
        /// 关闭时间范围开始
        /// </summary>
        private DateTime? closeTimeFrom;
        public DateTime? CloseTimeFrom
        {
            get { return closeTimeFrom; }
            set { base.SetValue("CloseTimeFrom", ref closeTimeFrom, value); }
        }
        /// <summary>
        /// 关闭时间范围结束
        /// </summary>
        private DateTime? closeTimeTo;
        public DateTime? CloseTimeTo
        {
            get { return closeTimeTo; }
            set { base.SetValue("CloseTimeTo", ref closeTimeTo, value); }
        }
        /// <summary>
        /// 创建者
        /// </summary>
        private int? createUserSysNo;
        public int? CreateUserSysNo
        {
            get { return createUserSysNo; }
            set { base.SetValue("CreateUserSysNo", ref createUserSysNo, value); }
        }
        /// <summary>
        /// 处理者
        /// </summary>
        private int? updateUserSysNo;
        public int? UpdateUserSysNo
        {
            get { return updateUserSysNo; }
            set { base.SetValue("UpdateUserSysNo", ref updateUserSysNo, value); }
        }

        public string CompanyCode { get; set; }

        public List<KeyValuePair<InternalMemoStatus?, string>> InternalMemoStatusList { get; set; }
        public List<KeyValuePair<RMANextHandler?, string>> NextHandlerList { get; set; }
        public List<UserInfo> createUserList;
        public List<UserInfo> CreateUserList
        {
            get { return createUserList; }
            set { base.SetValue("CreateUserList", ref createUserList, value); }
        }
        public List<UserInfo> updateUserList;
        public List<UserInfo> UpdateUserList
        {
            get { return updateUserList; }
            set { base.SetValue("UpdateUserList", ref updateUserList, value); }
        }

    }

    public class RMADispatchTrackingVM : ModelBase
    {
        public int? handlerSysNo;
        public int? HandlerSysNo
        {
            get { return handlerSysNo; }
            set { base.SetValue("HandlerSysNo", ref handlerSysNo, value); }
        }
        public List<int> SysNoList { get; set; }
    }
}
