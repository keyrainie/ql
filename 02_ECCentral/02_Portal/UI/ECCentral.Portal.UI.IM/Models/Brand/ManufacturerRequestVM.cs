
using ECCentral.BizEntity.IM;
using System.Collections.Generic;
using ECCentral.Portal.Basic;
using ECCentral.Portal.Basic.Utilities;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using Newegg.Oversea.Silverlight.Utilities.Validation;
using ECCentral.Portal.UI.IM.Resources;

namespace ECCentral.Portal.UI.IM.Models
{
    public class ManufacturerRequestVM : ModelBase
    {

        public ManufacturerRequestVM()
        {
            ManufacturerStatusList = EnumConverter.GetKeyValuePairs<ManufacturerStatus>(EnumConverter.EnumAppendItemType.None);
            BrandStoreTypeList = EnumConverter.GetKeyValuePairs<BrandStoreType>(EnumConverter.EnumAppendItemType.None);
        }
        /// <summary>
        /// 系统编号
        /// </summary>
        public int SysNo { get; set; }

        /// <summary>
        /// 生产商系统编号
        /// </summary>
        public int? ManufacturerSysNo { get; set; }

        /// <summary>
        /// 生产商名称
        /// </summary>
        public string manufacturerName;

        public string ManufacturerName
        {
            get { return manufacturerName; }
            set { SetValue("ManufacturerName", ref manufacturerName, value); }
        }

        /// <summary>
        /// 生产商状态
        /// </summary>
        private ManufacturerStatus manufacturerStatus = ManufacturerStatus.DeActive;
        public ManufacturerStatus ManufacturerStatus
        {
            get { return manufacturerStatus; }
            set { manufacturerStatus = value; }
        }
        /// <summary>
        /// 产品线
        /// </summary>
        public string productLine;
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

        /// <summary>
        /// 状态
        /// </summary>
        public int? Status { get; set; }

        /// <summary>
        /// 其他名称
        /// </summary>
        private string manufacturerBriefName;
        [Validate(ValidateType.Regex, @"^[A-Za-z0-9]", ErrorMessageResourceType=typeof(ResManufacturerRequest),ErrorMessageResourceName="Error_InputCharAndNumber")]
        public string ManufacturerBriefName
        {
            get { return manufacturerBriefName; }
            set { SetValue("ManufacturerBriefName", ref manufacturerBriefName, value); }
        }
        /// <summary>
        /// 店铺类型 
        /// </summary>
        private BrandStoreType brandStoreType = BrandStoreType.OrdinaryStore;
        public BrandStoreType BrandStoreType
        {
            get { return brandStoreType; }
            set { SetValue("BrandStoreType", ref brandStoreType, value); }
        }
        /// <summary>
        /// 操作类型
        /// </summary>
        public int OperationType { get; set; }

        private string clientPhone;
        public string ClientPhone
        {
            get { return clientPhone; }
            set { SetValue("ClientPhone", ref clientPhone, value); }
        }
        private bool isShowZone = false;
        public bool IsShowZone
        {
            get { return isShowZone; }
            set { SetValue("IsShowZone", ref isShowZone, value); }
        }
        private bool isLogo = false;
        public bool IsLogo
        {
            get { return isLogo; }
            set { SetValue("IsLogo", ref isLogo, value); }
        }
        private string showUrl;
        [Validate(ValidateType.URL)]
        public string ShowUrl
        {
            get { return showUrl; }
            set { SetValue("ShowUrl", ref showUrl, value); }
        }
        private string brandImage;
        [Validate(ValidateType.URL)]
        public string BrandImage
        {
            get { return brandImage; }
            set { SetValue("BrandImage", ref brandImage, value); }
        }
        private string info;
        public string Info
        {
            get { return info; }
            set { SetValue("Info", ref info, value); }
        }
        private string afterSalesSupportEmail;
        [Validate(ValidateType.Email)]
        public string AfterSalesSupportEmail
        {
            get { return afterSalesSupportEmail; }
            set { SetValue("AfterSalesSupportEmail", ref afterSalesSupportEmail, value); }
        }
        private string afterSalesSupportLink;
        [Validate(ValidateType.URL)]
        public string AfterSalesSupportLink
        {
            get { return afterSalesSupportLink; }
            set { SetValue("AfterSalesSupportLink", ref afterSalesSupportLink, value); }
        }
        private string mannfacturerLink;
        [Validate(ValidateType.URL)]
        public string MannfacturerLink
        {
            get { return mannfacturerLink; }
            set { SetValue("MannfacturerLink", ref mannfacturerLink, value); }
        }

        public List<KeyValuePair<ManufacturerStatus?, string>> ManufacturerStatusList { get; set; }
        public List<KeyValuePair<BrandStoreType?, string>> BrandStoreTypeList { get; set; }


        public bool HasManufacturerRequestApprovalPermission
        {
            get { return AuthMgr.HasFunctionPoint(AuthKeyConst.IM_Manufacturer_ManufacturerRequestApproval); }
        }

        public bool HasManufacturerRequestApplyPermission
        {
            get { return AuthMgr.HasFunctionPoint(AuthKeyConst.IM_Manufacturer_ManufacturerRequestApply); }
        }
    }
}
