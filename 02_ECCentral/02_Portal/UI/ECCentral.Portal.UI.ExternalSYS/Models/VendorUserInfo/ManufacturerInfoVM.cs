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
using Newegg.Oversea.Silverlight.Utilities.Validation;
using ECCentral.BizEntity.IM;
using ECCentral.BizEntity;

namespace ECCentral.Portal.UI.ExternalSYS.Models
{
    public class ManufacturerInfoVM : ModelBase
    {
        private int? sysNo;

        public int? SysNo
        {
            get { return sysNo; }
            set { base.SetValue("SysNo", ref sysNo, value); }
        }

        /// <summary>
        /// 生产商ID
        /// </summary>
        public string ManufacturerID { get; set; }

        /// <summary>
        /// 生产商本地化名称
        /// </summary>
        public LanguageContent ManufacturerNameLocal { get; set; }

        /// <summary>
        /// 生产商国际化名称
        /// </summary>
        public string ManufacturerNameGlobal { get; set; }

        /// <summary>
        /// 生产商状态
        /// </summary>
        public ManufacturerStatus Status { get; set; }

        /// <summary>
        /// 生产商描述
        /// </summary>
        public LanguageContent ManufacturerDescription { get; set; }

        /// <summary>
        /// 生产商信息
        /// </summary>
        public ManufacturerSupportInfo SupportInfo { get; set; }

        /// <summary>
        /// 店铺类型
        /// </summary>
        public BrandStoreType BrandStoreType { get; set; }

        /// <summary>
        /// 是否有LOGO
        /// </summary>
        public bool IsLogo { get; set; }

        /// <summary>
        /// 是否前台专区显示
        /// </summary>
        public string IsShowZone { get; set; }

        /// <summary>
        /// 品牌店广告图
        /// </summary>
        public string BrandImage { get; set; }

        /// <summary>
        /// 前台专卖店URL
        /// </summary>
        public string ShowUrl { get; set; }

        public string CompanyCode
        {
            get;
            set;
        }
        public string LanguageCode
        {
            get;
            set;
        }
    }
}
