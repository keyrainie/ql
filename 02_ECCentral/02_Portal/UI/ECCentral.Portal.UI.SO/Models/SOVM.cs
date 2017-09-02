using System;
using System.Net;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using System.Collections.Generic;
using ECCentral.BizEntity.Common;
using ECCentral.BizEntity.IM;

namespace ECCentral.Portal.UI.SO.Models
{
    public class SOVM : ModelBase
    {
        public SOVM()
        {
            Init();
        }

        //public List<string> Anchors { get; set; }

        public void Init()
        {
            //Anchors = new List<string>();
            BaseInfoVM = new SOBaseInfoVM();
            StatusChangeInfoListVM = new List<SOStatusChangeInfoVM>();
            ReceiverInfoVM = new SOReceiverInfoVM();
            InvoiceInfoVM = new SOInvoiceInfoVM();
            ShippingInfoVM = new SOShippingInfoVM();
            FPInfoVM = new SOFPInfoVM();
            ClientInfoVM = new SOClientInfoVM();
            ItemsVM = new List<SOItemInfoVM>();
            PromotionsVM = new List<SOPromotionInfoVM>();
            GiftCardListVM = new List<GiftCardInfoVM>();
            GiftCardRedeemLogListVM = new List<GiftCardRedeemLogVM>();
            WHUpdateVM = new SOWHUpdateInfoVM();
            SOInterceptInfoVMList = new List<SOInterceptInfoVM>();
        }

        private Int32? m_SysNo;
        public Int32? SysNo
        {
            get { return this.m_SysNo; }
            set { this.SetValue("SysNo", ref m_SysNo, value); }
        }

        private String m_CompanyCode;
        public String CompanyCode
        {
            get { return this.m_CompanyCode; }
            set { this.SetValue("CompanyCode", ref m_CompanyCode, value); }
        }

        private WebChannel m_WebChannel;
        public WebChannel WebChannel
        {
            get { return this.m_WebChannel; }
            set { this.SetValue("WebChannel", ref m_WebChannel, value); }
        }

        private Merchant m_Merchant;
        public Merchant Merchant
        {
            get { return this.m_Merchant; }
            set { this.SetValue("Merchant", ref m_Merchant, value); }
        }

        private SOBaseInfoVM m_BaseInfoVM;
        public SOBaseInfoVM BaseInfoVM
        {
            get { return this.m_BaseInfoVM; }
            set { this.SetValue("BaseInfoVM", ref m_BaseInfoVM, value); }
        }

        private List<SOStatusChangeInfoVM> m_StatusChangeInfoListVM;
        public List<SOStatusChangeInfoVM> StatusChangeInfoListVM
        {
            get { return this.m_StatusChangeInfoListVM; }
            set { this.SetValue("StatusChangeInfoListVM", ref m_StatusChangeInfoListVM, value); }
        }

        private SOReceiverInfoVM m_ReceiverInfoVM;
        public SOReceiverInfoVM ReceiverInfoVM
        {
            get { return this.m_ReceiverInfoVM; }
            set { this.SetValue("ReceiverInfoVM", ref m_ReceiverInfoVM, value); }
        }

        private SOInvoiceInfoVM m_InvoiceInfoVM;
        public SOInvoiceInfoVM InvoiceInfoVM
        {
            get { return this.m_InvoiceInfoVM; }
            set { this.SetValue("InvoiceInfoVM", ref m_InvoiceInfoVM, value); }
        }

        private SOShippingInfoVM m_ShippingInfoVM;
        public SOShippingInfoVM ShippingInfoVM
        {
            get { return this.m_ShippingInfoVM; }
            set { this.SetValue("ShippingInfoVM", ref m_ShippingInfoVM, value); }
        }

        private SOFPInfoVM m_FPInfoVM;
        public SOFPInfoVM FPInfoVM
        {
            get { return this.m_FPInfoVM; }
            set { this.SetValue("FPInfoVM", ref m_FPInfoVM, value); }
        }

        private SOClientInfoVM m_ClientInfoVM;
        public SOClientInfoVM ClientInfoVM
        {
            get { return this.m_ClientInfoVM; }
            set { this.SetValue("ClientInfoVM", ref m_ClientInfoVM, value); }
        }

        private List<SOItemInfoVM> m_ItemsVM;
        public List<SOItemInfoVM> ItemsVM
        {
            get { return this.m_ItemsVM; }
            set { this.SetValue("ItemsVM", ref m_ItemsVM, value); }
        }

        private List<SOPromotionInfoVM> m_PromotionsVM;
        public List<SOPromotionInfoVM> PromotionsVM
        {
            get { return this.m_PromotionsVM; }
            set { this.SetValue("PromotionsVM", ref m_PromotionsVM, value); }
        }

        public List<SOPromotionInfoVM> ComboPromotionsVM
        {
            get {
                List<SOPromotionInfoVM> result = new List<SOPromotionInfoVM>();
                if (PromotionsVM != null)
                {
                    result = PromotionsVM.Where(p => p.PromotionType == ECCentral.BizEntity.SO.SOPromotionType.Combo).ToList();
                }
                return result;
            }
        }

        private List<GiftCardInfoVM> m_GiftCardListVM;
        public List<GiftCardInfoVM> GiftCardListVM
        {
            get { return this.m_GiftCardListVM; }
            set { this.SetValue("GiftCardListVM", ref m_GiftCardListVM, value); }
        }

        private List<GiftCardRedeemLogVM> m_GiftCardRedeemLogListVM;
        public List<GiftCardRedeemLogVM> GiftCardRedeemLogListVM
        {
            get { return this.m_GiftCardRedeemLogListVM; }
            set { this.SetValue("GiftCardRedeemLogListVM", ref m_GiftCardRedeemLogListVM, value); }
        }

        /// <summary>
        /// 优惠券代码。
        /// </summary>
        private String m_CouponCode;
        public String CouponCode
        {
            get { return this.m_CouponCode; }
            set { this.SetValue("CouponCode", ref m_CouponCode, value); }
        }

        private SOWHUpdateInfoVM m_WHUpdateVM;
        public SOWHUpdateInfoVM WHUpdateVM
        {
            get { return this.m_WHUpdateVM; }
            set { this.SetValue("WHUpdateVM", ref m_WHUpdateVM, value); }
        }

        /// <summary>
        /// 订单拦截邮件信息
        /// </summary>
        private List<SOInterceptInfoVM> m_SOInterceptInfoVMList;
        public List<SOInterceptInfoVM> SOInterceptInfoVMList
        {
            get { return this.m_SOInterceptInfoVMList; }
            set { this.SetValue("SOInterceptInfoVMList", ref m_SOInterceptInfoVMList, value); }
        }

        public bool IsManualChangePrice
        {
            get 
            {
                return this.ItemsVM.Count(p => !string.IsNullOrEmpty(p.AdjustPriceReason)) > 0;
            }
        }

        private SOCustomerAuthenticationVM m_CustomerAuthentication;
        public SOCustomerAuthenticationVM CustomerAuthentication
        {
            get { return this.m_CustomerAuthentication; }
            set { this.SetValue("CustomerAuthentication", ref m_CustomerAuthentication, value); }
        }
    }
}
