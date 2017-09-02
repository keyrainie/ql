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
using ECCentral.BizEntity.PO;
using Newegg.Oversea.Silverlight.Utilities.Validation;
using System.Collections.Generic;
using ECCentral.Portal.Basic.Utilities;

namespace ECCentral.Portal.UI.PO.Models
{
    public class VendorBasicInfoVM : ModelBase
    {

        public VendorBasicInfoVM()
        {
            extendedInfo = new VendorExtendInfoVM();
            this.consignFlag = BizEntity.PO.VendorConsignFlag.Sell;
            this.vendorRank = BizEntity.PO.VendorRank.C;
            comment = "";
            note = "";
            vendorStatus = ECCentral.BizEntity.PO.VendorStatus.UnAvailable;
            HoldPMList = new List<VendorHoldPMInfoVM>();
        }

        public VendorBasicInfoVM(VendorInfoVM vendorInfo)
            :this()
        {
            this.vendorInfo = vendorInfo;
        }

        /// <summary>
        /// 父实体引用
        /// </summary>
        private VendorInfoVM vendorInfo { get; set; }


        /// <summary>
        /// 系统编号
        /// </summary>
        private string vendorID;

        public string VendorID
        {
            get { return vendorID; }
            set { base.SetValue("VendorID", ref vendorID, value); }
        }

        /// <summary>
        /// SellerID
        /// </summary>
        private string sellerID;

        public string SellerID
        {
            get { return sellerID; }
            set { base.SetValue("SellerID", ref sellerID, value); }
        }

        /// <summary>
        /// 供应商状态 (可用，不可用)
        /// </summary>
        private VendorStatus? vendorStatus;

        public VendorStatus? VendorStatus
        {
            get { return vendorStatus; }
            set { base.SetValue("VendorStatus", ref vendorStatus, value); }
        }

        /// <summary>
        /// 供应商类型
        /// </summary>
        private VendorType? vendorType;

        public VendorType? VendorType
        {
            get { return vendorType; }
            set { base.SetValue("VendorType", ref vendorType, value); }
        }

        /// <summary>
        /// 付款结算公司
        /// </summary>
        private PaySettleCompany? paySettleCompany;

        public PaySettleCompany? PaySettleCompany
        {
            get { return paySettleCompany; }
            set { base.SetValue("PaySettleCompany", ref paySettleCompany, value); }
        }

        /// <summary>
        /// 是否合作
        /// </summary>
        private VendorIsCooperate? vendorIsCooperate;

        public VendorIsCooperate? VendorIsCooperate
        {
            get { return vendorIsCooperate; }
            set { base.SetValue("VendorIsCooperate", ref vendorIsCooperate, value); }
        }

        /// <summary>
        /// 是否转租赁
        /// </summary>
        private VendorIsToLease? vendorIsToLease;

        public VendorIsToLease? VendorIsToLease
        {
            get { return vendorIsToLease == null ? ECCentral.BizEntity.PO.VendorIsToLease.UnLease : vendorIsToLease; }
            set { base.SetValue("VendorIsToLease", ref vendorIsToLease, value); }
        }

        /// <summary>
        /// 供应商本地化名称
        /// </summary>
        private string vendorNameLocal;
        [Validate(ValidateType.Required)]
        public string VendorNameLocal
        {
            get { return vendorNameLocal; }
            set { base.SetValue("VendorNameLocal", ref vendorNameLocal, value); }
        }

        /// <summary>
        /// 供应商简称
        /// </summary>
        private string vendorBriefName;

        [Validate(ValidateType.Required)]
        public string VendorBriefName
        {
            get { return vendorBriefName; }
            set { base.SetValue("VendorBriefName", ref vendorBriefName, value); }
        }

        /// <summary>
        /// 供应商国际化名称
        /// </summary>
        private string vendorNameGlobal;

        public string VendorNameGlobal
        {
            get { return vendorNameGlobal; }
            set { base.SetValue("VendorNameGlobal", ref vendorNameGlobal, value); }
        }

        /// <summary>
        /// 区域
        /// </summary>
        private string district;
        [Validate(ValidateType.Required)]
        public string District
        {
            get { return district; }
            set { base.SetValue("District", ref district, value); }
        }

        /// <summary>
        /// 邮编
        /// </summary>
        private string zipCode;
        [Validate(ValidateType.Regex, RegexHelper.ZIP, ErrorMessage = "请输入6位有效邮编")]
        public string ZipCode
        {
            get { return zipCode; }
            set { base.SetValue("ZipCode", ref zipCode, value); }
        }

        /// <summary>
        /// 地址
        /// </summary>
        private string address;
        [Validate(ValidateType.Required)]
        public string Address
        {
            get { return address; }
            set { base.SetValue("Address", ref address, value); }
        }

        /// <summary>
        /// 联系人
        /// </summary>
        private string contact;
        [Validate(ValidateType.Required)]
        public string Contact
        {
            get { return contact; }
            set { base.SetValue("Contact", ref contact, value); }
        }

        /// <summary>
        /// 传真
        /// </summary>
        private string fax;
        [Validate(ValidateType.Required)]
        public string Fax
        {
            get { return fax; }
            set { base.SetValue("Fax", ref fax, value); }
        }

        /// <summary>
        /// 电话
        /// </summary>
        private string phone;
        [Validate(ValidateType.Required)]
        public string Phone
        {
            get { return phone; }
            set { base.SetValue("Phone", ref phone, value); }
        }

        /// <summary>
        /// 手机
        /// </summary>
        private string cellPhone;
        [Validate(ValidateType.Required)]
        public string CellPhone
        {
            get { return cellPhone; }
            set { base.SetValue("CellPhone", ref cellPhone, value); }
        }

        /// <summary>
        /// 网址
        /// </summary>
        private string website;
        [Validate(ValidateType.URL)]
        public string Website
        {
            get { return website; }
            set { base.SetValue("Website", ref website, value); }
        }

        /// <summary>
        /// 电子邮箱
        /// </summary>
        private string emailAddress;
        public string EmailAddress
        {
            get { return emailAddress; }
            set { base.SetValue("EmailAddress", ref emailAddress, value); }
        }

        /// <summary>
        /// 供应商扩展信息
        /// </summary>
        private VendorExtendInfoVM extendedInfo;

        public VendorExtendInfoVM ExtendedInfo
        {
            get { return extendedInfo; }
            set { base.SetValue("ExtendedInfo", ref extendedInfo, value); }
        }

        /// <summary>
        /// 代销标识 (供应商属性：经销,代销，代收)
        /// </summary>
        private VendorConsignFlag? consignFlag;

        public VendorConsignFlag? ConsignFlag
        {
            get { return consignFlag; }
            set { base.SetValue("ConsignFlag", ref consignFlag, value); }
        }

        /// <summary>
        /// 供应商等级
        /// </summary>
        private VendorRank? vendorRank;

        public VendorRank? VendorRank
        {
            get { return vendorRank; }
            set { base.SetValue("VendorRank", ref vendorRank, value); }
        }


        /// <summary>
        /// 供应商等级(修改)
        /// </summary>
        private VendorRank? requestVendorRank;

        public VendorRank? RequestVendorRank
        {
            get { return requestVendorRank; }
            set { base.SetValue("RequestVendorRank", ref requestVendorRank, value); }
        }

        /// <summary>
        /// 下单日期
        /// </summary>
        private string buyWeekDayVendor;

        public string BuyWeekDayVendor
        {
            get { return buyWeekDayVendor; }
            set { base.SetValue("BuyWeekDayVendor", ref buyWeekDayVendor, value); }
        }

        //<summary>
        // 请求下单日期(用于审核下单日期操作)
        // </summary>
        private string requestBuyWeekDayVendor;

        public string RequestBuyWeekDayVendor
        {
            get { return requestBuyWeekDayVendor; }
            set { base.SetValue("RequestBuyWeekDayVendor", ref requestBuyWeekDayVendor, value); }
        }


        /// <summary>
        /// 供应商是否锁定
        /// </summary>
        private bool? holdMark;

        public bool? HoldMark
        {
            get { return holdMark; }
            set { base.SetValue("HoldMark", ref holdMark, value); }
        }

        /// <summary>
        /// 注释
        /// </summary>
        private string comment;

        public string Comment
        {
            get { return comment; }
            set { base.SetValue("Comment", ref comment, value); }
        }

        /// <summary>
        /// 备注
        /// </summary>
        private string note;

        public string Note
        {
            get { return note; }
            set { base.SetValue("Note", ref note, value); }
        }

        /// <summary>
        /// 供应商锁定/解除原因
        /// </summary>
        private string holdReason;

        public string HoldReason
        {
            get { return holdReason; }
            set { base.SetValue("HoldReason", ref holdReason, value); }
        }

        private List<VendorHoldPMInfoVM> holdPMList;


        /// <summary>
        /// 锁定的PM：
        /// </summary>
        public List<VendorHoldPMInfoVM> HoldPMList
        {
            get { return holdPMList; }
            set { base.SetValue("HoldPMList", ref holdPMList, value); }
        }

        public string VendorContractInfo
        {
            get 
            {
                if (this.vendorInfo != null
                    && this.vendorInfo.VendorAttachInfo != null)
                {
                    List<string> info=new List<string>();

                    if (this.vendorInfo.VendorAttachInfo.HasAgreementBeingSold)
                        info.Add("1");

                    if (this.vendorInfo.VendorAttachInfo.HasAgreementConsign)
                        info.Add("2");

                    if (this.vendorInfo.VendorAttachInfo.HasAgreementAfterSold)
                        info.Add("3");

                    if (info.Count > 0)
                        return string.Join<string>(",", info);
                    else
                        return null;

                }
                else
                {
                    return null;
                }
            }
        }

        #region 跨境
        private int? eportSysNo;
        public int? EPortSysNo
        {
            get;
            set;
        }
        #endregion

    }
}
