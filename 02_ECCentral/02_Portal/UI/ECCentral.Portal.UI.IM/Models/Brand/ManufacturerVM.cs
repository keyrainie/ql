//************************************************************************
// 用户名				泰隆优选
// 系统名				生产商管理
// 子系统名		        生产商管理Models
// 作成者				Tom
// 改版日				2012.5.14
// 改版内容				新建
//************************************************************************
using System;
using System.Collections.Generic;
using ECCentral.BizEntity.IM;
using ECCentral.Portal.Basic.Utilities;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using Newegg.Oversea.Silverlight.Utilities.Validation;
using ECCentral.Portal.UI.IM.Resources;

namespace ECCentral.Portal.UI.IM.Models
{
    /// <summary>
    /// 生产商信息
    /// </summary>
    public class ManufacturerVM : ModelBase
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public ManufacturerVM()
        {
            ManufacturerStatusList = EnumConverter.GetKeyValuePairs<ManufacturerStatus>(EnumConverter.EnumAppendItemType.None);
        }


        private int? _sysNo;
        /// <summary>
        /// 系统编号
        /// </summary>
        public int? SysNo
        {
            get { return _sysNo; }
            set { SetValue("SysNo", ref _sysNo, value); }
        }

        private string _manufacturerID;
        /// <summary>
        /// 生产商ID
        /// </summary>
        public string ManufacturerID
        {
            get { return _manufacturerID; }
            set { SetValue("ManufacturerID", ref _manufacturerID, value); }
        }

        private string _manufacturerNameLocal;
        /// <summary>
        /// 生产商本地化名称
        /// </summary>
        public string ManufacturerNameLocal
        {
            get { return _manufacturerNameLocal; }
            set { SetValue("ManufacturerNameLocal", ref _manufacturerNameLocal, value); }
        }

        /// <summary>
        /// 生产商国际化名称
        /// </summary>

        private string manufacturerNameGlobal;

        [Validate(ValidateType.Regex, @"^[A-Za-z0-9]+$", ErrorMessageResourceType=typeof(ResManufacturerRequest),ErrorMessageResourceName="Error_InputCharAndNumber")]
        public string ManufacturerNameGlobal
        {
            get { return manufacturerNameGlobal; }
            set { base.SetValue("ManufacturerNameGlobal", ref manufacturerNameGlobal, value); }
        }


        /// <summary>
        /// 生产商状态
        /// </summary>
        public ManufacturerStatus Status { get; set; }

        public bool IsReadOnly
        {
            get { return true; }
        }

        /// <summary>
        /// 生产商描述
        /// </summary>
        public string ManufacturerDescription { get; set; }

        /// <summary>
        /// 生产商信息
        /// </summary>
        public ManufacturerSupportVM SupportInfo { get; set; }

        public List<KeyValuePair<ManufacturerStatus?, string>> ManufacturerStatusList { get; set; }
    }

    /// <summary>
    /// 生产商支持信息
    /// </summary>
    public class ManufacturerSupportVM : ModelBase
    {


        /// <summary>
        /// 客服电话
        /// </summary>
        private string phone;

        [Validate(ValidateType.Phone)]
        public String ServicePhone
        {
            get
            {
                return phone;
            }
            set
            {
                base.SetValue("ServicePhone", ref phone, value);
            }
        }

        /// <summary>
        /// 售后支持邮箱
        /// </summary>
        private string serviceEmail;

        [Validate(ValidateType.Email)]
        public String ServiceEmail
        {
            get
            {
                return serviceEmail;
            }
            set
            {
                base.SetValue("ServiceEmail", ref serviceEmail, value);
            }
        }

        /// <summary>
        /// 售后支持链接
        /// </summary>
        private string url;

        [Validate(ValidateType.URL)]
        public String ServiceUrl
        {
            get
            {
                return url;
            }
            set
            {
                base.SetValue("ServiceUrl", ref url, value);
            }
        }

        /// <summary>
        /// 厂商链接
        /// </summary>
        [Validate(ValidateType.URL)]
        public String ManufacturerUrl
        {
            get
            {
                return url;
            }
            set
            {
                base.SetValue("ManufacturerUrl", ref url, value);
            }
        }
    }
}
