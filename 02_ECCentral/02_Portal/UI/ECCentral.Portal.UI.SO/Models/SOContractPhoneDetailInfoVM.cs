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

namespace ECCentral.Portal.UI.SO.Models
{
    public class SOContractPhoneDetailInfoVM : ModelBase
    {
        /// <summary>
        /// 合约机订单套餐编号 （ 对应 SOSIMCardAndContractPhoneInfoVM 的 SuitID ）
        /// </summary>
        private Int32? m_SysNo;
        public Int32? SysNo
        {
            get { return this.m_SysNo; }
            set { this.SetValue("SysNo", ref m_SysNo, value); }
        }

        private Int32? m_AreaSysNo;
        public Int32? AreaSysNo
        {
            get { return this.m_AreaSysNo; }
            set { this.SetValue("AreaSysNo", ref m_AreaSysNo, value); }
        }

        private Int32? m_PhoneProductSysNo;
        public Int32? PhoneProductSysNo
        {
            get { return this.m_PhoneProductSysNo; }
            set { this.SetValue("PhoneProductSysNo", ref m_PhoneProductSysNo, value); }
        }

        private String m_PlanType;
        public String PlanType
        {
            get { return this.m_PlanType; }
            set { this.SetValue("PlanType", ref m_PlanType, value); }
        }

        private String m_SuitName;
        public String SuitName
        {
            get { return this.m_SuitName; }
            set { this.SetValue("SuitName", ref m_SuitName, value); }
        }

        private Decimal? m_MonthPay;
        public Decimal? MonthPay
        {
            get { return this.m_MonthPay; }
            set { this.SetValue("MonthPay", ref m_MonthPay, value); }
        }

        private Int32? m_Period;
        public Int32? Period
        {
            get { return this.m_Period; }
            set { this.SetValue("Period", ref m_Period, value); }
        }

        private Decimal? m_PreSaveAmount;
        public Decimal? PreSaveAmount
        {
            get { return this.m_PreSaveAmount; }
            set { this.SetValue("PreSaveAmount", ref m_PreSaveAmount, value); }
        }

        private Decimal? m_BackAmount;
        public Decimal? BackAmount
        {
            get { return this.m_BackAmount; }
            set { this.SetValue("BackAmount", ref m_BackAmount, value); }
        }

        private Decimal? m_MonthAmount;
        public Decimal? MonthAmount
        {
            get { return this.m_MonthAmount; }
            set { this.SetValue("MonthAmount", ref m_MonthAmount, value); }
        }

        private Decimal? m_PhonePrice;
        public Decimal? PhonePrice
        {
            get { return this.m_PhonePrice; }
            set { this.SetValue("PhonePrice", ref m_PhonePrice, value); }
        }

        private Int32? m_ContractSysNo;
        public Int32? ContractSysNo
        {
            get { return this.m_ContractSysNo; }
            set { this.SetValue("ContractSysNo", ref m_ContractSysNo, value); }
        }

        private String m_Status;
        public String Status
        {
            get { return this.m_Status; }
            set { this.SetValue("Status", ref m_Status, value); }
        }     
    }
}
