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
using System.Collections.Generic;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using ECCentral.BizEntity.SO;

namespace ECCentral.Portal.UI.SO.Models
{
    public class SOPromotionDetailInfoVM: ModelBase
    {
        public SOPromotionDetailInfoVM()
        {
            GiftListVM = new List<SOPromotionInfo.GiftInfo>();
            CouponCodeList = new List<string>();
        }

        private Int32? m_MasterProductSysNo;
        public Int32? MasterProductSysNo
        {
            get { return this.m_MasterProductSysNo; }
            set { this.SetValue("MasterProductSysNo", ref m_MasterProductSysNo, value); }
        }

        private SOProductType? m_MasterProductType;
        public SOProductType? MasterProductType
        {
            get { return this.m_MasterProductType; }
            set { this.SetValue("MasterProductType", ref m_MasterProductType, value); }
        }

        private Int32? m_MasterProductQuantity;
        public Int32? MasterProductQuantity
        {
            get { return this.m_MasterProductQuantity; }
            set { this.SetValue("MasterProductQuantity", ref m_MasterProductQuantity, value); }
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

        private List<ECCentral.BizEntity.SO.SOPromotionInfo.GiftInfo> m_GiftListVM;
        public List<ECCentral.BizEntity.SO.SOPromotionInfo.GiftInfo> GiftListVM
        {
            get { return this.m_GiftListVM; }
            set { this.SetValue("GiftListVM", ref m_GiftListVM, value); }
        }

    }
}
