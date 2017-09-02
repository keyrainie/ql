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
using Newegg.Oversea.Silverlight.Utilities.Validation;
using System.Collections.Generic;
using ECCentral.Portal.Basic.Utilities;

namespace ECCentral.Portal.UI.MKT.Models
{
    public class SaleAdvGroupVM : ModelBase
    {
        public SaleAdvGroupVM()
        {
            this.RecommendTypeList = EnumConverter.GetKeyValuePairs<RecommendType>();
            this.RecommendType = BizEntity.MKT.RecommendType.Normal;
        }

        private int? sysNo;
        public int? SysNo
        {
            get
            {
                return sysNo;
            }
            set
            {
                SetValue("SysNo", ref sysNo, value);
            }
        }

        public int? SaleAdvSysNo { get; set; }

        private string groupName;
        [Validate(ValidateType.Required)]
		public string GroupName 
		{ 
			get
			{
				return groupName;
			}			
			set
			{
				SetValue("GroupName", ref groupName, value);
			} 
		}
		
        private DateTime? showStartDate;
		public DateTime? ShowStartDate 
		{ 
			get
			{
				return showStartDate;
			}			
			set
			{
				SetValue("ShowStartDate", ref showStartDate, value);
			} 
		}
		
        private DateTime? showEndDate;
		public DateTime? ShowEndDate 
		{ 
			get
			{
				return showEndDate;
			}			
			set
			{
				SetValue("ShowEndDate", ref showEndDate, value);
			} 
		}

        private ADStatus? status;
        public ADStatus? Status
        {
            get
            {
                return status;
            }
            set
            {
                SetValue("Status", ref status, value);
            }
        }

        private string priority;
        [Validate(ValidateType.Required)]
        [Validate(ValidateType.Regex, @"^[0-9]\d{0,2}$", ErrorMessage = "请输入1至999的整数！")]
        public string Priority
        {
            get
            {
                return priority;
            }
            set
            {
                SetValue("Priority", ref priority, value);
            }
        }
        /// <summary>
        /// 分组更多链接
        /// </summary>
        private string otherGroupLink;
        [Validate(ValidateType.MaxLength, 200)]
        public string OtherGroupLink
        {
            get
            {
                return otherGroupLink;
            }
            set
            {
                SetValue("OtherGroupLink", ref otherGroupLink, value);
            }
        }

        /// <summary>
        /// 定位锚ID
        /// </summary>
        private string groupIDForAnchor;
        [Validate(ValidateType.MaxLength, 50)]
        public string GroupIDForAnchor
        {
            get
            {
                return groupIDForAnchor;
            }
            set
            {
                SetValue("GroupIDForAnchor", ref groupIDForAnchor, value);
            }
        }

        /// <summary>
        /// 分组Banner
        /// </summary>
        private string groupBannerHTML;
        [Validate(ValidateType.MaxLength, 2000)]
        public string GroupBannerHTML
        {
            get
            {
                return groupBannerHTML;
            }
            set
            {
                SetValue("GroupBannerHTML", ref groupBannerHTML, value);
            }
        }

        /// <summary>
        /// 组广告图片地址
        /// </summary>
        private string groupImgResourceAddr;
        [Validate(ValidateType.MaxLength, 200)]
        public string GroupImgResourceAddr
        {
            get
            {
                return groupImgResourceAddr;
            }
            set
            {
                SetValue("GroupImgResourceAddr", ref groupImgResourceAddr, value);
            }
        }

        /// <summary>
        /// 组广告图片链接
        /// </summary>
        private string groupImgResourceLink;
        [Validate(ValidateType.MaxLength, 200)]
        public string GroupImgResourceLink
        {
            get
            {
                return groupImgResourceLink;
            }
            set
            {
                SetValue("GroupImgResourceLink", ref groupImgResourceLink, value);
            }
        }

        /// <summary>
        /// 边框色
        /// </summary>
        private string borderColor;
        [Validate(ValidateType.MaxLength, 7)]
        public string BorderColor
        {
            get
            {
                return borderColor;
            }
            set
            {
                SetValue("BorderColor", ref borderColor, value);
            }
        }

        /// <summary>
        /// 标题字体色
        /// </summary>
        private string titleForeColor;
        [Validate(ValidateType.MaxLength, 7)]
        public string TitleForeColor
        {
            get
            {
                return titleForeColor;
            }
            set
            {
                SetValue("TitleForeColor", ref titleForeColor, value);
            }
        }

        /// <summary>
        /// 标题背景色
        /// </summary>
        private string titleBackColor;
        [Validate(ValidateType.MaxLength, 7)]
        public string TitleBackColor
        {
            get
            {
                return titleBackColor;
            }
            set
            {
                SetValue("TitleBackColor", ref titleBackColor, value);
            }
        }

        /// <summary>
        /// 推荐方式
        /// </summary>
        private RecommendType? recommendType;
        public RecommendType? RecommendType
        {
            get
            {
                return recommendType;
            }
            set
            {
                SetValue("RecommendType", ref recommendType, value);
            }
        }

        public List<KeyValuePair<RecommendType?, string>> RecommendTypeList { get; set; }

        private int? itemsCount;
		public int? ItemsCount 
		{ 
			get
			{
				return itemsCount;
			}			
			set
			{
				SetValue("ItemsCount", ref itemsCount, value);
			} 
		}		

        private bool allGroup;
		public bool AllGroup 
		{ 
			get
			{
				return allGroup;
			}			
			set
			{
				SetValue("AllGroup", ref allGroup, value);
			} 
		}		

        private string inUser;
		public string InUser 
		{ 
			get
			{
				return inUser;
			}			
			set
			{
				SetValue("InUser", ref inUser, value);
			} 
		}
		
        private DateTime? inDate;
		public DateTime? InDate 
		{ 
			get
			{
				return inDate;
			}			
			set
			{
				SetValue("InDate", ref inDate, value);
			} 
		}
		
        private string editUser;
		public string EditUser 
		{ 
			get
			{
				return editUser;
			}			
			set
			{
				SetValue("EditUser", ref editUser, value);
			} 
		}
		
        private DateTime? editDate;
		public DateTime? EditDate 
		{ 
			get
			{
				return editDate;
			}			
			set
			{
				SetValue("EditDate", ref editDate, value);
			} 
		}		
    }
}
