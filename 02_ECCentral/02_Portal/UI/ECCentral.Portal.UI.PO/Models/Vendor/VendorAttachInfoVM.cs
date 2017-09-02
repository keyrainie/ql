using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;

namespace ECCentral.Portal.UI.PO.Models
{
    public class VendorAttachInfoVM : ModelBase
    {
        /// <summary>
        /// 供应商申请表
        /// </summary>
        private string vendorApplyForm;

        public string VendorApplyForm
        {
            get { return vendorApplyForm; }
            set { base.SetValue("VendorApplyForm", ref vendorApplyForm, value); }
        }

        /// <summary>
        /// 组织机构代码证
        /// </summary>
        private string organizeCodeCertificate;

        public string OrganizeCodeCertificate
        {
            get { return organizeCodeCertificate; }
            set { base.SetValue("OrganizeCodeCertificate", ref organizeCodeCertificate, value); }
        }

        /// <summary>
        /// 企业法人营业执照
        /// </summary>
        private string enterpriseBusinessLicence;

        public string EnterpriseBusinessLicence
        {
            get { return enterpriseBusinessLicence; }
            set { base.SetValue("EnterpriseBusinessLicence", ref enterpriseBusinessLicence, value); }
        }

        /// <summary>
        /// 税务登记证
        /// </summary>
        private string taxationAffairsRegistration;

        public string TaxationAffairsRegistration
        {
            get { return taxationAffairsRegistration; }
            set { base.SetValue("TaxationAffairsRegistration", ref taxationAffairsRegistration, value); }
        }

        /// <summary>
        /// 经销协议
        /// </summary>
        private string agreementBeingSold;

        public string AgreementBeingSold
        {
            get { return agreementBeingSold; }
            set { 

                base.SetValue("AgreementBeingSold", ref agreementBeingSold, value);
                if (!string.IsNullOrEmpty(value))
                    this.HasAgreementBeingSold = true;
                else
                    this.HasAgreementBeingSold = false;
 
            }
        }

        /// <summary>
        ///是否含有经销协议
        /// </summary>
        private bool hasAgreementBeingSold;
        public bool HasAgreementBeingSold
        {
            get { return hasAgreementBeingSold; }
            set { base.SetValue("HasAgreementBeingSold",ref hasAgreementBeingSold, value); }
        }


        /// <summary>
        /// 代销协议
        /// </summary>
        private string agreementConsign;

        public string AgreementConsign
        {
            get { return agreementConsign; }
            set { 
                base.SetValue("AgreementConsign", ref agreementConsign, value);
                if (!string.IsNullOrEmpty(value))
                    this.HasAgreementConsign = true;
                else
                    this.HasAgreementConsign = false;
            }
        }


        private bool hasAgreementConsign;
        public bool HasAgreementConsign
        {
            get { return hasAgreementConsign; }
            set { base.SetValue("HasAgreementConsign", ref hasAgreementConsign, value); }
        }

        /// <summary>
        /// 售后协议
        /// </summary>
        private string agreementAfterSold;

        public string AgreementAfterSold
        {
            get { return agreementAfterSold; }
            set { 
                base.SetValue("AgreementAfterSold", ref agreementAfterSold, value);
                if (!string.IsNullOrEmpty(value))
                    this.HasAgreementAfterSold = true;
                else
                    this.HasAgreementAfterSold = false;
            }
        }

        public bool hasAgreementAfterSold;
        public bool HasAgreementAfterSold
        {
            get { return hasAgreementAfterSold; }
            set { base.SetValue("HasAgreementAfterSold", ref hasAgreementAfterSold ,value); }
        }


        /// <summary>
        /// 其它协议
        /// </summary>
        private string agreementOther;

        public string AgreementOther
        {
            get { return agreementOther; }
            set { base.SetValue("AgreementOther", ref agreementOther, value); }
        }

    }
}
