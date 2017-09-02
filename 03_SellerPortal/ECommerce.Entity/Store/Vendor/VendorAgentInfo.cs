using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using ECommerce.Utility;

namespace ECommerce.Entity.Store.Vendor
{
    /// <summary>
    /// 供应商代理信息
    /// </summary>
    [Serializable]
    [DataContract]
    public class VendorAgentInfo
    {

        [DataMember]
        public int? AgentSysNo { get; set; }
        /// <summary>
        /// 代理厂商信息
        /// </summary>
        [DataMember]
        public ManufacturerInfo ManufacturerInfo { get; set; }
        /// <summary>
        /// 代理品牌信息
        /// </summary>
        [DataMember]
        public BrandInfo BrandInfo { get; set; }

        /// <summary>
        /// 代理商品类别信息
        /// </summary>
        [DataMember]
        public CategoryInfo CategoryInfo { get; set; }

        /// <summary>
        /// 代理级别
        /// </summary>
        [DataMember]
        public string AgentLevel { get; set; }

        /// <summary>
        /// 送货周期
        /// </summary>
        [DataMember]
        public string SendPeriod { get; set; }

        /// <summary>
        /// 下单日期
        /// </summary>
        [DataMember]
        public string BuyWeekDay { get; set; }

        /// <summary>
        /// 请求送货周期
        /// </summary>
        [DataMember]
        public string RequestSendPeriod { get; set; }

        /// <summary>
        /// 请求下单日期
        /// </summary>
        [DataMember]
        public string RequestBuyWeekDay { get; set; }

        /// <summary>
        /// 代理状态
        /// </summary>
        [DataMember]
        public string AgentStatus { get; set; }

        /// <summary>
        /// 代理1级分类编号
        /// </summary>
        [DataMember]
        public int? C1SysNo { get; set; }

        /// <summary>
        /// 代理2级分类编号
        /// </summary>
        [DataMember]
        public int? C2SysNo { get; set; }

        /// <summary>
        /// 代理3级分类编号
        /// </summary>
        [DataMember]
        public int? C3SysNo { get; set; }

        /// <summary>
        /// 代理1级分类名称
        /// </summary>
        [DataMember]
        public string C1Name { get; set; }

        /// <summary>
        /// 代理2级分类名称
        /// </summary>
        [DataMember]
        public string C2Name { get; set; }

        /// <summary>
        /// 代理3级分类名称
        /// </summary>
        [DataMember]
        public string C3Name { get; set; }

        /// <summary>
        /// 代理类型(代销结算模式)
        /// </summary>
        [DataMember]
        public SettleType? SettleType { get; set; }

        /// <summary>
        /// 佣金百分比
        /// </summary>
        [DataMember]
        public decimal? SettlePercentage { get; set; }

        /// <summary>
        /// 审核状态
        /// </summary>
        [DataMember]
        public VendorModifyRequestStatus? RequestType
        {
            get;
            set;
        }
        public string RequestTypeStr
        {
            get
            {
                if (RequestType.HasValue)
                {
                    return EnumHelper.GetDescription(RequestType.Value);
                }
                return "";
            }
        }

        /// <summary>
        /// 操作类型
        /// </summary>
        [DataMember]
        public VendorModifyActionType? CheckType
        {
            get;
            set;
        }

        /// <summary>
        /// 操作内容
        /// </summary>
        [DataMember]
        public string Content
        {
            get;
            set;
        }

        /// <summary>
        /// 佣金信息
        /// </summary>
        [DataMember]
        public VendorCommissionInfo VendorCommissionInfo { get; set; }

        /// <summary>
        /// 对象状态
        /// </summary>
        [DataMember]
        public VendorRowState RowState { get; set; }

        /// <summary>
        /// 该Vendor供应的C3的自动作废未付款订单的最长允许时间（单位：Minute）
        /// </summary>
        [DataMember]
        public int? MaxNoPayTimeForOrder
        {
            get;
            set;
        }

        /// <summary>
        /// 请求的该Vendor供应的C3的自动作废未付款订单的最长允许时间（单位：Minute）
        /// </summary>
        [DataMember]
        public int? RequestMaxNoPayTimeForOrder
        {
            get;
            set;
        }

        /// <summary>
        /// 该Vendor供应的C3类别商品是否需要确认
        /// </summary>
        [DataMember]
        public bool? IsNeedConfirmOrder
        {
            get;
            set;
        }

        /// <summary>
        /// 请求的该Vendor供应的C3类别商品是否需要确认
        /// </summary>
        [DataMember]
        public bool? RequestIsNeedConfirmOrder
        {
            get;
            set;
        }

        /// <summary>
        /// 公司编号
        /// </summary>
        [DataMember]
        public string CompanyCode
        {
            get;
            set;
        }

        /// <summary>
        /// 代理状态
        /// </summary>
        [DataMember]
        public VendorAgentStatus Status
        {
            get;
            set;
        }

    }
}
