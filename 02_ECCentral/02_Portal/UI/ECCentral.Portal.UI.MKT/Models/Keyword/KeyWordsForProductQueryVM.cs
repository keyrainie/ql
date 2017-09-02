using System;
using System.Net;
using System.Windows;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;

using System.Collections.Generic;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.BizEntity.MKT;
using Newegg.Oversea.Silverlight.Utilities.Validation;
namespace ECCentral.Portal.UI.MKT.Models
{
    public class KeyWordsForProductQueryVM : ModelBase
    {
        public string CompanyCode { get; set; }

        private int? sysNo;
        public int? SysNo
        {
            get { return sysNo; }
            set { base.SetValue("SysNo", ref sysNo, value); }
        }

        /// <summary>
        /// 商品编号
        /// </summary>
        private string productSysNo;
        public string ProductSysNo
        {
            get { return productSysNo; }
            set { base.SetValue("ProductSysNo", ref productSysNo, value); }
        }

        /// <summary>
        /// 商品ID
        /// </summary>
        private string productID;
        public string ProductID
        {
            get { return productID; }
            set { base.SetValue("ProductID", ref productID, value); }
        }

        /// <summary>
        /// 商品
        /// </summary>
        private string productName;
        public string ProductName
        {
            get { return productName; }
            set { base.SetValue("ProductName", ref productName, value); }
        }

        /// <summary>
        /// 关键字
        /// </summary>
        private string keywords;
        public string Keywords
        {
            get { return keywords; }
            set { base.SetValue("Keywords", ref keywords, value); }
        }

        /// <summary>
        /// 优先级
        /// </summary>
        private string priority;
        [Validate(ValidateType.Interger)]
        public string Priority
        {
            get { return priority; }
            set { base.SetValue("Priority", ref priority, value); }
        }

        /// <summary>
        /// 是否选中
        /// </summary>
        private bool isChecked;
        public bool IsChecked
        {
            get { return isChecked; }
            set { base.SetValue("IsChecked", ref isChecked, value); }
        }

        private ADStatus? status;
        public ADStatus? Status
        {
            get { return status; }
            set { base.SetValue("Status", ref status, value); }
        }

        public string InUser { get; set; }

        public DateTime? InDate { get; set; }

        #region 扩展属性

        /// <summary>
        /// ”有效“字符串
        /// </summary>
        public string ActiveString
        {
            get { return EnumConverter.GetDescription(ADStatus.Active); }
        }

        /// <summary>
        /// ”无效“字符串
        /// </summary>
        public string DeactiveString
        {
            get { return EnumConverter.GetDescription(ADStatus.Deactive); }
        }

        public bool IsActive
        {
            get
            {
                return Status == ADStatus.Active;
            }
            set
            {
                if (value)
                    Status = ADStatus.Active;
                else
                    Status = ADStatus.Deactive;
            }
        }
        #endregion

        /// <summary>
        /// 渠道编号
        /// </summary>
        private string channelID;
        public string ChannelID
        {
            get { return channelID; }
            set
            {
                base.SetValue("ChannelID", ref channelID, value);
            }
        }

        /// <summary>
        /// 渠道列表
        /// </summary>
        public List<UIWebChannel> ChannelList
        {
            get
            {
                return CPApplication.Current.CurrentWebChannelList.ToList<UIWebChannel>();
            }
        }
    }
}
