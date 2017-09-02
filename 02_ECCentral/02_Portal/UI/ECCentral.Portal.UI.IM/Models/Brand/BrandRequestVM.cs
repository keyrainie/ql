using ECCentral.BizEntity.IM;
using System.Collections.Generic;
using ECCentral.Portal.Basic;
using ECCentral.Portal.Basic.Utilities;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using Newegg.Oversea.Silverlight.Utilities.Validation;
using ECCentral.Portal.UI.IM.Resources;

namespace ECCentral.Portal.UI.IM.Models
{
    public class BrandRequestVM : ModelBase
    {
        public BrandRequestVM()
        {
            BrandStatusList = EnumConverter.GetKeyValuePairs<ValidStatus>(EnumConverter.EnumAppendItemType.None);
            BrandStoreTypeList = EnumConverter.GetKeyValuePairs<BrandStoreType>(EnumConverter.EnumAppendItemType.None);
        }
        /// <summary>
        /// 生产商
        /// </summary>
        private string manufacturerName;
        public string ManufacturerName
        {
            get { return manufacturerName; }
            set { base.SetValue("ManufacturerName", ref manufacturerName, value); }
        }

        /// <summary>
        /// 生产商Sysno
        /// </summary>
        private string manufacturerSysNo;
        public string ManufacturerSysNo
        {
            get { return manufacturerSysNo; }
            set { base.SetValue("ManufacturerSysNo", ref manufacturerSysNo, value); }
        }


        /// <summary>
        /// 生产商其他名称
        /// </summary>
        public string ManufacturerBriefName { get; set; }
        /// <summary>
        /// 品牌中文名称
        /// </summary>
        private string brandName_Ch;
        public string BrandName_Ch
        {
            get { return brandName_Ch; }
            set { SetValue("BrandName_Ch", ref brandName_Ch, value); }
        }
        /// <summary>
        /// 品牌英文名称
        /// </summary>
        private string brandName_En;
        [Validate(ValidateType.Regex, "^[A-Za-z0-9\\s]+$", ErrorMessageResourceType=typeof(ResBrandRequest),ErrorMessageResourceName="Error_InputEnglish")]
        public string BrandName_En
        {
            get { return brandName_En; }
            set { SetValue("BrandName_En", ref brandName_En, value); }
        }

        /// <summary>
        /// 售后支持邮箱
        /// </summary>
        private string supportEmail;
        [Validate(ValidateType.Email)]
        public string SupportEmail
        {
            get { return supportEmail; }
            set { SetValue("SupportEmail", ref supportEmail, value); }
        }

        /// <summary>
        /// 售后支持链接 
        /// </summary>
        private string supportUrl;
        [Validate(ValidateType.URL)]
        public string SupportUrl
        {
            get { return supportUrl; }
            set { SetValue("SupportUrl", ref supportUrl, value); }
        }

        /// <summary>
        /// 厂商链接
        /// </summary>
        private string manufacturerWebsite;
        [Validate(ValidateType.URL)]
        public string ManufacturerWebsite
        {
            get { return manufacturerWebsite; }
            set { SetValue("ManufacturerWebsite", ref manufacturerWebsite, value); }
        }

        /// <summary>
        /// 客户电话
        /// </summary>
        private string customerServicePhone;
        public string CustomerServicePhone
        {
            get { return customerServicePhone; }
            set { SetValue("CustomerServicePhone", ref customerServicePhone, value); }
        }

        /// <summary>
        /// 是否有logo
        /// </summary>
        public bool IsLogo { get; set; }

        /// <summary>
        /// 是否专区显示
        /// </summary>
        public bool IsDisPlayZone { get; set; }

        /// <summary>
        /// 状态
        /// </summary>

        private ValidStatus brandStatus = ValidStatus.Active;
        [Validate(ValidateType.Required)]
        public ValidStatus BrandStatus
        {
            get { return brandStatus; }
            set { SetValue("BrandStatus", ref brandStatus, value); }
        }

        /// <summary>
        /// 店铺类型 
        /// </summary>
        public BrandStoreType BrandStoreType { get; set; }

        /// <summary>
        /// 品牌故事
        /// </summary>
        public string BrandStory { get; set; }

        /// <summary>
        ///图片路径
        /// </summary>
        public string ImageUrl { get; set; }

        /// <summary>
        /// 前台专卖店url
        /// </summary>
        private string showStoreUrl;
        [Validate(ValidateType.URL)]
        public string ShowStoreUrl
        {
            get { return showStoreUrl; }
            set { SetValue("ShowStoreUrl", ref showStoreUrl, value); }
        }

        /// <summary>
        /// 广告图
        /// </summary>
        private string adImage;
        [Validate(ValidateType.URL)]
        public string AdImage 
        {
            get { return adImage; }
            set { SetValue("AdImage", ref adImage, value); }
        }

        /// <summary>
        /// 备注
        /// </summary>
        public string Info { get; set; }

        /// <summary>
        /// 产品线
        /// </summary>
        private string productLine;
        public string ProductLine
        {
            get { return productLine; }
            set { SetValue("ProductLine", ref productLine, value); }
        }

        /// <summary>
        /// 申请理由
        /// </summary>

        private string reasons;
        [Validate(ValidateType.Required)]
        public string Reasons
        {
            get { return reasons; }
            set { SetValue("Reasons", ref reasons, value); }
        }

        public string Auditor { get; set; }

        public int SysNo { get; set; }
        /// <summary>
        /// 商品ID 用于连接
        /// </summary>
        public string ProductId { get; set; }

        public string RequestStatus { get; set; }
        public List<KeyValuePair<ValidStatus?, string>> BrandStatusList { get; set; }
        public List<KeyValuePair<BrandStoreType?, string>> BrandStoreTypeList { get; set; }


        public bool HasBrandRequestApprovalPermission
        {
            get { return AuthMgr.HasFunctionPoint(AuthKeyConst.IM_Brand_BrandRequestApproval); }
        }

        public bool HasBrandRequestApplyPermission
        {
            get { return AuthMgr.HasFunctionPoint(AuthKeyConst.IM_Brand_BrandRequestApply); }
        }

        public bool HasBrandStoreMaintainPermission
        {
            get { return AuthMgr.HasFunctionPoint(AuthKeyConst.IM_Brand_BrandStoreMaintain); }
        }

        
        /// <summary>
        /// 用于生成ProductId
        /// </summary>
        private string brandCode;
        [Validate(ValidateType.Required)]
        [Validate(ValidateType.Regex, @"^([0-9]|[a-z]|[A-Z]){3}$", ErrorMessage = "请输入0-Z任意组合的3位字符！")]
        public string BrandCode
        {
            get { return brandCode; }
            set { SetValue("BrandCode", ref brandCode, value); }
        }


    }
}
