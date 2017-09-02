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
using ECCentral.BizEntity.SO;
using ECCentral.Portal.UI.SO.Resources;

namespace ECCentral.Portal.UI.SO.Models
{
    public class SOSIMCardAndContractPhoneInfoVM : ModelBase
    {
        public SOSIMCardAndContractPhoneInfoVM()
        {
            ContractPhoneDetailInfoVM = new SOContractPhoneDetailInfoVM();
        }

        /// <summary>
        /// SIM、联通合约机信息表主键编号
        /// </summary>
        private Int32? m_SysNo;
        public Int32? SysNo
        {
            get { return this.m_SysNo; }
            set { this.SetValue("SysNo", ref m_SysNo, value); }
        }

        /// <summary>
        ///SIM 卡对应的订单编号
        /// </summary>
        private Int32? m_SOSysNo;
        public Int32? SOSysNo
        {
            get { return this.m_SOSysNo; }
            set { this.SetValue("SOSysNo", ref m_SOSysNo, value); }
        }

        /// <summary>
        /// SIM 卡对应的机主编号
        /// </summary>
        private Int32? m_CustomerSysNo;
        public Int32? CustomerSysNo
        {
            get { return this.m_CustomerSysNo; }
            set { this.SetValue("CustomerSysNo", ref m_CustomerSysNo, value); }
        }

        /// <summary>
        /// SIM 卡对应的机主姓名
        /// </summary>
        private String m_CustomerName;
        public String CustomerName
        {
            get { return this.m_CustomerName; }
            set { this.SetValue("CustomerName", ref m_CustomerName, value); }
        }

        /// <summary>
        /// SIM 卡对应的 证件类型
        /// </summary>
        private CertificateType? m_CertificateType;
        public CertificateType? CertificateType
        {
            get { return this.m_CertificateType; }
            set { this.SetValue("CertificateType", ref m_CertificateType, value); }
        }

        /// <summary>
        /// SIM 卡对应的 证件信息
        /// </summary>
        private String m_CertificateValue;
        public String CertificateValue
        {
            get { return this.m_CertificateValue; }
            set { this.SetValue("CertificateValue", ref m_CertificateValue, value); }
        }

        /// <summary>
        /// SIM 卡对应的 证件有效期截止日
        /// </summary>
        private DateTime? m_CertificateDate;
        public DateTime? CertificateDate
        {
            get { return this.m_CertificateDate; }
            set { this.SetValue("CertificateDate", ref m_CertificateDate, value); }
        }

        /// <summary>
        /// SIM 卡对应的 证件有效期截止日 显示在页面上 为短日期格式
        /// </summary>        
        public String CertificateDateDisplay
        {
            get { return this.CertificateDate.HasValue?this.CertificateDate.Value.ToShortDateString():string.Empty; }
        }

        /// <summary>
        /// SIM 卡对应的 证件地址
        /// </summary>
        private String m_CertificateAddress;
        public String CertificateAddress
        {
            get { return this.m_CertificateAddress; }
            set { this.SetValue("CertificateAddress", ref m_CertificateAddress, value); }
        }

        /// <summary>
        /// SIM 卡对应的 证件地区编号
        /// </summary>
        private Int32? m_CertificateAreaSysno;
        public Int32? CertificateAreaSysno
        {
            get { return this.m_CertificateAreaSysno; }
            set { this.SetValue("CertificateAreaSysno", ref m_CertificateAreaSysno, value); }
        }

        /// <summary>
        /// SIM 卡对应的 机主联系地址
        /// </summary>
        private String m_Address;
        public String Address
        {
            get { return this.m_Address; }
            set { this.SetValue("Address", ref m_Address, value); }
        }

        /// <summary>
        /// SIM 卡对应的 机主联系邮编
        /// </summary>
        private String m_ZipCode;
        public String ZipCode
        {
            get { return this.m_ZipCode; }
            set { this.SetValue("ZipCode", ref m_ZipCode, value); }
        }

        /// <summary>
        /// SIM 卡对应的 机主联系电话
        /// </summary>
        private String m_Phone;
        public String Phone
        {
            get { return this.m_Phone; }
            set { this.SetValue("Phone", ref m_Phone, value); }
        }

        /// <summary>
        /// 商品编号
        /// </summary>
        private Int32? m_ProductSysno;
        public Int32? ProductSysno
        {
            get { return this.m_ProductSysno; }
            set { this.SetValue("ProductSysno", ref m_ProductSysno, value); }
        }

        /// <summary>
        /// SIM 卡 对应的 卡号
        /// </summary>
        private String m_CellPhone;
        public String CellPhone
        {
            get { return this.m_CellPhone; }
            set { this.SetValue("CellPhone", ref m_CellPhone, value); }
        }

        /// <summary>
        /// 所选套餐 编号
        /// </summary>
        private Int32? m_SuitID;
        public Int32? SuitID
        {
            get { return this.m_SuitID; }
            set { this.SetValue("SuitID", ref m_SuitID, value); }
        }

        /// <summary>
        /// 所选套餐 名称
        /// </summary>
        private String m_SuitName;
        public String SuitName
        {
            get { return this.m_SuitName; }
            set { this.SetValue("SuitName", ref m_SuitName, value); }
        }

        /// <summary>
        /// 付款类型（预付款 或 后付款）
        /// </summary>
        public String m_PaymentType;
        public String PaymentType
        {
            get { return this.m_PaymentType; }
            set { this.SetValue("PaymentType", ref m_PaymentType, value); }
        }
        /// <summary>
        /// 合约机订单编号
        /// </summary>
        private String m_UnicomOrderNo;
        public String UnicomOrderNo
        {
            get { return this.m_UnicomOrderNo; }
            set { this.SetValue("UnicomOrderNo", ref m_UnicomOrderNo, value); }
        }

        /// <summary>
        /// 合约类型
        /// </summary>
        private String m_UnicomType;
        public String UnicomType
        {
            get { return this.m_UnicomType; }
            set { this.SetValue("UnicomType", ref m_UnicomType, value); }
        }

        /// <summary>
        /// 合约类型 供页面显示
        /// </summary>
        public String UnicomTypeDisplay
        {
            get { return UnicomType == "B" ? ResSOMaintain.Info_ContractPhoneSO_Type_SaveMoneyGainMobile : ResSOMaintain.Info_ContractPhoneSO_Type_BuyMobileGainTariff; }
        }
        
        private String m_FirstMonthPaymethod;
        public String FirstMonthPaymethod
        {
            get { return this.m_FirstMonthPaymethod; }
            set { this.SetValue("FirstMonthPaymethod", ref m_FirstMonthPaymethod, value); }
        }

        private Int32? m_FirstMonthPaymethodNO;
        public Int32? FirstMonthPaymethodNO
        {
            get { return this.m_FirstMonthPaymethodNO; }
            set { this.SetValue("FirstMonthPaymethodNO", ref m_FirstMonthPaymethodNO, value); }
        }

        /// <summary>
        /// SIM卡 序列号
        /// </summary>
        private String m_SIMSN;
        public String SIMSN
        {
            get { return this.m_SIMSN; }
            set { this.SetValue("SIMSN", ref m_SIMSN, value); }
        }

        /// <summary>
        /// SIM卡状态
        /// </summary>
        private SIMStatus? m_SIMStatus;
        public SIMStatus? SIMStatus
        {
            get { return this.m_SIMStatus; }
            set { this.SetValue("SIMStatus", ref m_SIMStatus, value); }
        }

        /// <summary>
        /// SIM卡备注
        /// </summary>
        private String m_Memo;
        public String Memo
        {
            get { return this.m_Memo; }
            set { this.SetValue("Memo", ref m_Memo, value); }
        }

        /// <summary>
        /// 创建日期
        /// </summary>
        private DateTime? m_InDate;
        public DateTime? InDate
        {
            get { return this.m_InDate; }
            set { this.SetValue("InDate", ref m_InDate, value); }
        }

        /// <summary>
        /// 创建人
        /// </summary>
        private String m_InUser;
        public String InUser
        {
            get { return this.m_InUser; }
            set { this.SetValue("InUser", ref m_InUser, value); }
        }

        /// <summary>
        /// 修改日期
        /// </summary>
        private DateTime? m_EditDate;
        public DateTime? EditDate
        {
            get { return this.m_EditDate; }
            set { this.SetValue("EditDate", ref m_EditDate, value); }
        }

        /// <summary>
        /// 修改人
        /// </summary>
        private String m_EditUser;
        public String EditUser
        {
            get { return this.m_EditUser; }
            set { this.SetValue("EditUser", ref m_EditUser, value); }
        }

        /// <summary>
        /// 手机串号
        /// </summary>
        private String m_IMEI;
        public String IMEI
        {
            get { return this.m_IMEI; }
            set { this.SetValue("IMEI", ref m_IMEI, value); }
        }

        /// <summary>
        /// 公司编号
        /// </summary>
        private String m_CompanyCode;
        public String CompanyCode
        {
            get { return this.m_CompanyCode; }
            set { this.SetValue("CompanyCode", ref m_CompanyCode, value); }
        }

        /// <summary>
        /// 联通0元购机订单套餐详细信息
        /// </summary>
        private SOContractPhoneDetailInfoVM m_ContractPhoneDetailInfoVM;
        public SOContractPhoneDetailInfoVM ContractPhoneDetailInfoVM
        {
            get { return this.m_ContractPhoneDetailInfoVM; }
            set { this.SetValue("ContractPhoneDetailInfoVM", ref m_ContractPhoneDetailInfoVM, value); }
        }
    }
}
