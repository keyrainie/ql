using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace ECCentral.BizEntity.PO
{
    /// <summary>
    /// 供应商基本信息
    /// </summary>
    [Serializable]
    [DataContract]
    public class VendorBasicInfo
    {
        public VendorBasicInfo()
        {
            HoldPMList = new List<VendorHoldPMInfo>();
        }
        /// <summary>
        /// 供应商编号
        /// </summary>
       [DataMember]
        public string VendorID { get; set; }

        /// <summary>
        /// 供应商账号
        /// </summary>
       [DataMember]
       public string Account { get; set; }

        /// <summary>
        /// SellerID
        /// </summary>
       [DataMember]
       public string SellerID { get; set; }

        /// <summary>
        /// 供应商状态
        /// </summary>
       [DataMember]
       public VendorStatus? VendorStatus { get; set; }

        /// <summary>
        /// 是否合作
        /// </summary>
       [DataMember]
       public VendorIsCooperate? VendorIsCooperate { get; set; }

       /// <summary>
       /// 
       /// </summary>
       public string VendorContractInfo { get; set; }

        /// <summary>
        /// 供应商本地化名称
        /// </summary>
       [DataMember]
       public string VendorNameLocal { get; set; }

        /// <summary>
        /// 供应商国际化名称
        /// </summary>
       [DataMember]
       public string VendorNameGlobal { get; set; }

        /// <summary>
        /// 供应商简称
        /// </summary>
       [DataMember]
       public string VendorBriefName { get; set; }

        /// <summary>
        /// 供应商类型
        /// </summary>
       [DataMember]
       public VendorType? VendorType { get; set; }

       [DataMember]
       public PaySettleCompany? PaySettleCompany { get; set; }

        /// <summary>
        /// 区域
        /// </summary>
       [DataMember]
       public string District { get; set; }

        /// <summary>
        /// 地址
        /// </summary>
       [DataMember]
       public string Address { get; set; }

        /// <summary>
        /// 邮编
        /// </summary>
       [DataMember]
       public string ZipCode { get; set; }

        /// <summary>
        /// 联系人
        /// </summary>
       [DataMember]
       public string Contact { get; set; }

        /// <summary>
        /// 电话
        /// </summary>
       [DataMember]
       public string Phone { get; set; }

        /// <summary>
        /// 传真
        /// </summary>
       [DataMember]
       public string Fax { get; set; }

        /// <summary>
        /// 电子邮箱
        /// </summary>
       [DataMember]
       public string EmailAddress { get; set; }

        /// <summary>
        /// 手机
        /// </summary>
       [DataMember]
       public string CellPhone { get; set; }

        /// <summary>
        /// 网址
        /// </summary>
       [DataMember]
       public string Website { get; set; }


        /// <summary>
        /// 供应商扩展信息
        /// </summary>
       [DataMember]
       public VendorExtendInfo ExtendedInfo { get; set; }

        /// <summary>
        /// 代销标识 (供应商属性：经销,代销，代收)
        /// </summary>
       [DataMember]
       public VendorConsignFlag? ConsignFlag { get; set; }

        /// <summary>
        /// 供应商等级
        /// </summary>
       [DataMember]
       public VendorRank VendorRank { get; set; }

        /// <summary>
        ///  请求供应商等级(用于审核等级操作)
        /// </summary>
       [DataMember]
       public VendorRank? RequestVendorRank { get; set; }

        /// <summary>
        /// 下单日期
        /// </summary>
       [DataMember]
       public string BuyWeekDayVendor { get; set; }

        /// <summary>
        /// 请求下单日期(用于审核下单日期操作)
        /// </summary>
       [DataMember]
       public string RequestBuyWeekDayVendor { get; set; }

        /// <summary>
        /// 供应商是否锁定
        /// </summary>
       [DataMember]
       public bool? HoldMark { get; set; }

        /// <summary>
        /// 注释
        /// </summary>
       [DataMember]
       public string Comment { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
       [DataMember]
       public string Note { get; set; }

        /// <summary>
        /// 供应商锁定/解除原因
        /// </summary>
       [DataMember]
       public string HoldReason { get; set; }

        /// <summary>
        /// 锁定的PM信息列表：
        /// </summary>
       [DataMember]
       public List<VendorHoldPMInfo> HoldPMList { get; set; }

        /// <summary>
        ///是否转租赁 
        /// </summary>
        [DataMember]
        public VendorIsToLease? VendorIsToLease { get; set; }

        #region 跨境
        [DataMember]
        public int? EPortSysNo
        {
            get;
            set;
        }
        [DataMember]
        public string EPortName
        {
            get;
            set;
        }
        #endregion
    }
}
