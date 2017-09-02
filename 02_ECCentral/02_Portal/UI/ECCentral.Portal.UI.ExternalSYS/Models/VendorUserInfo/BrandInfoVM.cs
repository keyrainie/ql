using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using ECCentral.BizEntity;
using ECCentral.BizEntity.ExternalSYS;
using ECCentral.BizEntity.IM;
using ECCentral.BizEntity.Common;

namespace ECCentral.Portal.UI.ExternalSYS.Models
{
    public class BrandInfoVM : ModelBase
    {
        /// <summary>
        /// 系统编号
        /// </summary>
        private int? sysNo;

        public int? SysNo
        {
            get { return sysNo; }
            set { base.SetValue("SysNo", ref sysNo, value); }
        }

        /// <summary>
        /// 品牌本地化名称
        /// </summary>
        public LanguageContent BrandNameLocal { get; set; }

        /// <summary>
        /// 品牌国际化名称
        /// </summary>
        public string BrandNameGlobal { get; set; }

        /// <summary>
        /// 品牌状态
        /// </summary>
        public ECCentral.BizEntity.ExternalSYS.ValidStatus Status { get; set; }

        /// <summary>
        /// 品牌描述
        /// </summary>
        public LanguageContent BrandDescription { get; set; }

        /// <summary>
        /// 所属生产商
        /// </summary>
        public ManufacturerInfo Manufacturer { get; set; }

        /// <summary>
        /// 品牌支持信息
        /// </summary>
        public BrandSupportInfo BrandSupportInfo { get; set; }

        /// <summary>
        /// 店铺类型
        /// </summary>
        public BrandStoreType BrandStoreType { get; set; }

        /// <summary>
        /// 是否有logo
        /// </summary>
        public string IsLogo { get; set; }

        /// <summary>
        /// 品牌故事
        /// </summary>
        public string BrandStory { get; set; }

        /// <summary>
        /// 商品ID 用于连接
        /// </summary>
        public string ProductId { get; set; }

        public string LanguageCode
        {
            get;
            set;
        }

        public string CompanyCode
        {
            get;
            set;
        }

        public UserInfo User;
    }
}
