using System;
using System.Runtime.Serialization;
using ECCentral.BizEntity.Common;

namespace ECCentral.BizEntity.IM
{
    /// <summary>
    /// 商品调价请求
    /// </summary>
    [Serializable]
    [DataContract]
    public class ProductPriceRequestInfo : ProductPriceInfo, IIdentity
    {
        /// <summary>
        /// 拒绝理由
        /// </summary>
        [DataMember]
        public string DenyReason { get; set; }

        /// <summary>
        /// PM申请理由
        /// </summary>
        [DataMember]
        public string PMMemo { get; set; }

        /// <summary>
        /// TL审核理由
        /// </summary>
        [DataMember]
        public string TLMemo { get; set; }

        /// <summary>
        /// PMD审核理由
        /// </summary>
        [DataMember]
        public string PMDMemo { get; set; }

        /// <summary>
        /// 审核状态
        /// </summary>
        [DataMember]
        public ProductPriceRequestStatus? RequestStatus { get; set; }

        /// <summary>
        /// 审核类型
        /// </summary>
        [DataMember]
        public ProductPriceRequestAuditType? AuditType { get; set; }

        /// <summary>
        /// 申请人
        /// </summary>
        [DataMember]
        public UserInfo CreateUser { get; set; }

        /// <summary>
        /// 审核人
        /// </summary>
        [DataMember]
        public UserInfo AuditUser { get; set; }

        /// <summary>
        /// 审核时间
        /// </summary>
        [DataMember]
        public DateTime? AuditTime
        {
            get;
            set;
        }

        /// <summary>
        /// 审核人
        /// </summary>
        [DataMember]
        public UserInfo FinalAuditUser { get; set; }

        /// <summary>
        /// 审核时间
        /// </summary>
        [DataMember]
        public DateTime? FinalAuditTime
        {
            get;
            set;
        }

        /// <summary>
        ///旧价格单据
        /// </summary>
        [DataMember]
        public ProductPriceInfo OldPrice { get; set; }

        /// <summary>
        /// 可用库存
        /// </summary>
        [DataMember]
        public int? AvailableQty { get; set; }

        /// <summary>
        /// 代销库存
        /// </summary>
        [DataMember]
        public int? ConsignQty { get; set; }

        /// <summary>
        /// 最后一次采购日期
        /// </summary>
        [DataMember]
        public DateTime? LastInTime { get; set; }

        /// <summary>
        /// 唯一标识
        /// </summary>
        [DataMember]
        public int? SysNo { get; set; }

        /// <summary>
        /// 三级类
        /// </summary>
        [DataMember]
        public CategoryInfo Category { get; set; }

        /// <summary>
        /// 最后一次更新日期
        /// </summary>
        [DataMember]
        public DateTime? LastUpdateTime { get; set; }

        /// <summary>
        /// 初级审核权限
        /// </summary>
        [DataMember]
        public bool HasPrimaryAuditPricePermission { get; set; }

        /// <summary>
        /// 高级审核权限
        /// </summary>
        [DataMember]
        public bool HasAdvancedAuditPricePermission { get; set; }

        private bool _isOnePass = false;
        /// <summary>
        /// 是否直接审批通过
        /// </summary>
        [DataMember]
        public bool IsOnePass
        { 
           get { return _isOnePass; }
           set { _isOnePass = value; }
        }

        public string PMUserEmailAddress { get; set; }

        public string BackupPMUserEmailAddress { get; set; }

        public string CurrentUserEmailAddress { get; set; }

        public string CreateUserEmailAddress { get; set; }

        [DataMember]
        public int GiftSysNo { get; set; }
        [DataMember]
        public int CouponSysNo { get; set; }
    }
}
