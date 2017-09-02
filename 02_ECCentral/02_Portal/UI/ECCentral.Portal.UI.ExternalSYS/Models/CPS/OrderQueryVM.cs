using System.Collections.Generic;
using ECCentral.BizEntity.ExternalSYS;
using ECCentral.Portal.Basic;
using ECCentral.Portal.Basic.Utilities;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using Newegg.Oversea.Silverlight.Utilities.Validation;
using System;

namespace ECCentral.Portal.UI.ExternalSYS.Models
{
    public class OrderQueryVM : ModelBase
    {
        public OrderQueryVM()
        {
            this.OrderTypeList = EnumConverter.GetKeyValuePairs<CPSOrderType>(EnumConverter.EnumAppendItemType.All);
            this.SettledStatusList = EnumConverter.GetKeyValuePairs<FinanceStatus>(EnumConverter.EnumAppendItemType.All);
        }


        /// <summary>
        /// 获取或设置单据类型
        /// </summary>
        public CPSOrderType? orderType;
        public CPSOrderType? OrderType
        {
            get { return orderType; }
            set { SetValue("OrderType", ref orderType, value); }
        }

        /// <summary>
        /// 获取或设置单据编号
        /// </summary>
        public int? OrderSysNo { get; set; }

        private string orderSysNoList;
        [Validate(ValidateType.Regex, @"^((\d+[,，]){0,10000})\d+$", ErrorMessage = "输入数字以,隔开 PS 1,2,3 ")]
        public string OrderSysNoList 
        {
            get { return orderSysNoList; }
            set { SetValue("OrderSysNoList", ref orderSysNoList, value); }
        }

        /// <summary>
        /// 获取或设置主渠道ID
        /// </summary>
        public string MasterChannelID { get; set; }

        /// <summary>
        /// 获取或设置子渠道ID
        /// </summary>
        public string SubChannelID { get; set; }

        /// <summary>
        /// 获取或设置下单日期(Begin)
        /// </summary>
        public DateTime? CreateDateBegin { get; set; }

        /// <summary>
        /// 获取或设置下单日期(End)
        /// </summary>
        public DateTime? CreateDateEnd { get; set; }

        /// <summary>
        /// 获取或设置交易完成日期 (Begin)
        /// </summary>
        public DateTime? FinishDateBegin { get; set; }

        /// <summary>
        /// 获取或设置交易完成日期 (End)
        /// </summary>
        public DateTime? FinishDateEnd { get; set; }

        /// <summary>
        /// 获取或设置结算日期 (Begin)
        /// </summary>
        private DateTime? settlementDateBegin;
        public DateTime? SettlementDateBegin 
        {
            get { return settlementDateBegin; }
            set { SetValue("SettlementDateBegin", ref settlementDateBegin, value); }
        }

        /// <summary>
        /// 获取或设置结算日期 (End)
        /// </summary>
        private DateTime? settlementDateEnd;
        public DateTime? SettlementDateEnd 
        {
            get { return settlementDateEnd; }
            set { SetValue("SettlementDateEnd", ref settlementDateEnd, value); }
        }

        /// <summary>
        /// 获取或设置结算状态
        /// </summary>
        public FinanceStatus? SettledStatus { get; set; }

        public List<KeyValuePair<CPSOrderType?, string>> OrderTypeList { get; set; }
        public List<KeyValuePair<FinanceStatus?, string>> SettledStatusList { get; set; }

    }
}
