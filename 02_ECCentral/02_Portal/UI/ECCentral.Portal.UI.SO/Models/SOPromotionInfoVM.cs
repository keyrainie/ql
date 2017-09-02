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
using ECCentral.BizEntity.MKT;
using System.Collections.Generic;
using ECCentral.BizEntity.SO;

namespace ECCentral.Portal.UI.SO.Models
{
    public class SOPromotionInfoVM : ModelBase
    {
        public  SOPromotionInfoVM()
        {          
            GiftListVM = new List<GiftInfoVM>();
            MasterListVM = new List<MasterInfoVM>();
            CouponCodeList = new List<string>();
            PromotionDetailsVM = new List<SOPromotionDetailInfoVM>();
        }

        private Int32? m_SOSysNo;
        public Int32? SOSysNo
        {
            get { return this.m_SOSysNo; }
            set { this.SetValue("SOSysNo", ref m_SOSysNo, value); }
        }

        private SOPromotionType? m_PromotionType;
        public SOPromotionType? PromotionType
        {
            get { return this.m_PromotionType; }
            set { this.SetValue("PromotionType", ref m_PromotionType, value); }
        }

        private Int32? m_PromotionSysNo;
        public Int32? PromotionSysNo
        {
            get { return this.m_PromotionSysNo; }
            set { this.SetValue("PromotionSysNo", ref m_PromotionSysNo, value); }
        }

        private String m_PromotionName;
        public String PromotionName
        {
            get { return this.m_PromotionName; }
            set { this.SetValue("PromotionName", ref m_PromotionName, value); }
        }

        private Int32? m_Time;
        public Int32? Time
        {
            get { return this.m_Time; }
            set { this.SetValue("Time", ref m_Time, value); }
        }

        private Decimal? m_Discount;
        public Decimal? Discount
        {
            get { return this.m_Discount; }
            set { this.SetValue("Discount", ref m_Discount, value); }
        }

        private Decimal? m_DiscountAmount;
        public Decimal? DiscountAmount
        {
            get { return this.m_DiscountAmount; }
            set { this.SetValue("DiscountAmount", ref m_DiscountAmount, value); }
        }

        private Int32? m_GainPoint;
        public Int32? GainPoint
        {
            get { return this.m_GainPoint; }
            set { this.SetValue("GainPoint", ref m_GainPoint, value); }
        }

        private List<String> m_CouponCodeList;
        public List<String> CouponCodeList
        {
            get { return this.m_CouponCodeList; }
            set { this.SetValue("CouponCodeList", ref m_CouponCodeList, value); }
        }

        private List<GiftInfoVM> m_GiftListVM;
        public List<GiftInfoVM> GiftListVM
        {
            get { return this.m_GiftListVM; }
            set { this.SetValue("GiftListVM", ref m_GiftListVM, value); }
        }

        private List<MasterInfoVM> m_MasterListVM;
        public List<MasterInfoVM> MasterListVM
        {
            get { return this.m_MasterListVM; }
            set { this.SetValue("MasterListVM",ref m_MasterListVM, value); }
        }


        private String m_Note;
        public String Note
        {
            get { return this.m_Note; }
            set { this.SetValue("Note", ref m_Note, value); }
        }

        private List<SOPromotionDetailInfoVM> m_PromotionDetailsVM;
        public List<SOPromotionDetailInfoVM> PromotionDetailsVM
        {
            get { return this.m_PromotionDetailsVM; }
            set { this.SetValue("PromotionDetailsVM", ref m_PromotionDetailsVM, value); }
        }

        private string m_InnerType;
        /// <summary>
        /// 当前促销活动内部定义类型，如赠品类促销的内部类型：单品买赠，满额赠送等
        /// </summary>
        public string InnerType
        {
            get { return this.m_InnerType; }
            set { this.SetValue("InnerType", ref m_InnerType, value); }
        }

    }
}
