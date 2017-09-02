//************************************************************************
// 用户名				泰隆优选
// 系统名				品牌管理
// 子系统名		        品牌管理Models
// 作成者				Tom
// 改版日				2012.5.14
// 改版内容				新建
//************************************************************************

using System;
using System.Collections.Generic;
using ECCentral.BizEntity.IM;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.Portal.UI.IM.Resources;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using Newegg.Oversea.Silverlight.Utilities.Validation;

namespace ECCentral.Portal.UI.IM.Models
{
    /// <summary>
    /// 品牌视图
    /// </summary>
    public class BrandVM : ModelBase
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public BrandVM()
        {
            BrandStatusList = EnumConverter.GetKeyValuePairs<ValidStatus>(EnumConverter.EnumAppendItemType.None);
        }

        private int? _sysNo;
        public int? SysNo
        {
            get { return _sysNo; }
            set { base.SetValue("SysNo", ref _sysNo, value); }
        }

        private string brandNameLocal;
        /// <summary>
        /// 品牌本地化名称
        /// </summary>
        [Validate(ValidateType.MaxLength, 100)]
        public string BrandNameLocal
        {
            get { return brandNameLocal; }
            set { base.SetValue("BrandNameLocal", ref brandNameLocal, value); }
        }

        /// <summary>
        /// 品牌国际化名称
        /// </summary>
        private string brandNameGlobal;


        [Validate(ValidateType.Regex, new object[] {"^[A-Za-z0-9\\s]+$"}, ErrorMessageResourceType = typeof (ResProductAttachmentMaintain), ErrorMessageResourceName = "BrandVM_BrandNameErrorInfo")]
        public string BrandNameGlobal
        {
            get { return brandNameGlobal; }
            set { base.SetValue("BrandNameGlobal", ref brandNameGlobal, value); }
        }

        /// <summary>
        /// 品牌状态
        /// </summary>
        public ValidStatus Status { get; set; }

        /// <summary>
        /// 品牌ID
        /// </summary>
        private string _brandID;
        public string BrandID
        {
            get { return _brandID; }
            set { base.SetValue("BrandID", ref _brandID, value); }
        }

        /// <summary>
        /// 品牌描述
        /// </summary>
        [Validate(ValidateType.MaxLength, 500)]
        public string BrandDescription { get; set; }

        /// <summary>
        /// 所属生产商
        /// </summary>
        public ManufacturerVM ManufacturerInfo { get; set; }

        /// <summary>
        /// 品牌支持信息
        /// </summary>
        public BrandSupportVM BrandSupportInfo { get; set; }

        public List<KeyValuePair<ValidStatus?, string>> BrandStatusList { get; set; }
    }

    /// <summary>
    /// 品牌支持信息
    /// </summary>
    public class BrandSupportVM : ModelBase
    {
        /// <summary>
        /// 客服电话
        /// </summary>
        private string phone;

        [Validate(ValidateType.Phone)]
        public String ServicePhone
        {
            get { return phone; }
            set { base.SetValue("ServicePhone", ref phone, value); }
        }

        /// <summary>
        /// 售后支持邮箱
        /// </summary>
        private string serviceEmail;

        [Validate(ValidateType.Email)]
        public String ServiceEmail
        {
            get { return serviceEmail; }
            set { base.SetValue("ServiceEmail", ref serviceEmail, value); }
        }

        /// <summary>
        /// 售后支持链接
        /// </summary>
        private string serviceUrl;

        [Validate(ValidateType.URL)]
        public String ServiceUrl
        {
            get { return serviceUrl; }
            set { base.SetValue("ServiceUrl", ref serviceUrl, value); }
        }

        /// <summary>
        /// 厂商链接
        /// </summary>        
        private string manufacturerUrl;

        [Validate(ValidateType.URL)]
        public String ManufacturerUrl
        {
            get { return manufacturerUrl; }
            set { base.SetValue("ManufacturerUrl", ref manufacturerUrl, value); }
        }
    }

}

