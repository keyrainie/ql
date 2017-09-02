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
using ECCentral.Portal.Basic;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using System.Collections.Generic;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using Newegg.Oversea.Silverlight.Utilities.Validation;

namespace ECCentral.Portal.UI.MKT.Models
{
    public class CategoryKeywordsVM : ModelBase
    {
        /// <summary>
        /// 编号
        /// </summary>
        private int? sysNo;
        public int? SysNo
        {
            get { return sysNo; }
            set { base.SetValue("SysNo", ref sysNo, value); }
        }

        /// <summary>
        /// 属性关键字ID集合
        /// </summary>
        private string propertyKeywordsList;
        public string PropertyKeywordsList
        {
            get { return propertyKeywordsList; }
            set { base.SetValue("PropertyKeywordsList", ref propertyKeywordsList, value); }
        }

        /// <summary>
        /// 商品一级类编号
        /// </summary>
        private int? category1SysNo;
        [Validate(ValidateType.Required)]
        public int? Category1SysNo
        {
            get { return category1SysNo; }
            set { base.SetValue("Category1SysNo", ref category1SysNo, value); }
        }

        /// <summary>
        /// 商品二级类编号
        /// </summary>
        private int? category2SysNo;
        [Validate(ValidateType.Required)]
        public int? Category2SysNo
        {
            get { return category2SysNo; }
            set { base.SetValue("Category2SysNo", ref category2SysNo, value); }
        }

        /// <summary>
        /// 商品三级类编号
        /// </summary>
        private int? category3SysNo;
        [Validate(ValidateType.Required)]
        public int? Category3SysNo
        {
            get { return category3SysNo; }
            set { base.SetValue("Category3SysNo", ref category3SysNo, value); }
        }

        /// <summary>
        /// 商品三级类名称
        /// </summary>
        private string category3Name;
        public string Category3Name
        {
            get { return category3Name; }
            set { base.SetValue("Category3Name", ref category3Name, value); }
        }

        /// <summary>
        /// 通用关键字
        /// </summary>
        private string commonKeywords;
        [Validate(ValidateType.Required)]
        public string CommonKeywords
        {
            get { return commonKeywords; }
            set { base.SetValue("CommonKeywords", ref commonKeywords, value); }
        }

        /// <summary>
        /// 属性关键字
        /// </summary>
        private string propertyKeywords;
        [Validate(ValidateType.Required)]
        public string PropertyKeywords
        {
            get { return propertyKeywords; }
            set { base.SetValue("PropertyKeywords", ref propertyKeywords, value); }
        }

        /// <summary>
        /// 创建人
        /// </summary>
        private string inUser;
        public string InUser
        {
            get { return inUser; }
            set { base.SetValue("InUser", ref inUser, value); }
        }

        /// <summary>
        /// 创建时间
        /// </summary>
        private DateTime? inDate;
        public DateTime? InDate
        {
            get { return inDate; }
            set { base.SetValue("InDate", ref inDate, value); }
        }

        /// <summary>
        /// 更新人
        /// </summary>
        private string editUser;
        public string EditUser
        {
            get { return editUser; }
            set { base.SetValue("EditUser", ref editUser, value); }
        }

        /// <summary>
        /// 更新时间
        /// </summary>
        private DateTime? editDate;
        public DateTime? EditDate
        {
            get { return editDate; }
            set { base.SetValue("EditDate", ref editDate, value); }
        }
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

        public string CompanyCode { get; set; }

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

    public class CategoryKeywordsQueryVM : ModelBase
    {
        /// <summary>
        /// 商品一级类编号
        /// </summary>
        private int? category1SysNo;
        public int? Category1SysNo
        {
            get { return category1SysNo; }
            set { base.SetValue("Category1SysNo", ref category1SysNo, value); }
        }

        /// <summary>
        /// 商品二级类编号
        /// </summary>
        private int? category2SysNo;
        public int? Category2SysNo
        {
            get { return category2SysNo; }
            set { base.SetValue("Category2SysNo", ref category2SysNo, value); }
        }

        /// <summary>
        /// 商品三级类编号
        /// </summary>
        private int? category3SysNo;
        public int? Category3SysNo
        {
            get { return category3SysNo; }
            set { base.SetValue("Category3SysNo", ref category3SysNo, value); }
        }

        /// <summary>
        /// 商品三级类名称
        /// </summary>
        private string category3Name;
        public string Category3Name
        {
            get { return category3Name; }
            set { base.SetValue("Category3Name", ref category3Name, value); }
        }

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

        public string CompanyCode { get; set; }

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

        public bool HasCreateCommonKeywordsPermission
        {
            get { return AuthMgr.HasFunctionPoint(AuthKeyConst.MKT_CategoryKeywords_CreateCommonKeywords); }
        }

        public bool HasCreatePropertyKeywordsPermission
        {
            get { return AuthMgr.HasFunctionPoint(AuthKeyConst.MKT_CategoryKeywords_CreatePropertyKeywords); }
        }

    }

    public class CategoryKeywordPropertyVM : ModelBase
    {
        /// <summary>
        /// 商品一级类编号
        /// </summary>
        private int? category1SysNo;
        [Validate(ValidateType.Required)]
        public int? Category1SysNo
        {
            get { return category1SysNo; }
            set { base.SetValue("Category1SysNo", ref category1SysNo, value); }
        }

        /// <summary>
        /// 商品二级类编号
        /// </summary>
        private int? category2SysNo;
        [Validate(ValidateType.Required)]
        public int? Category2SysNo
        {
            get { return category2SysNo; }
            set { base.SetValue("Category2SysNo", ref category2SysNo, value); }
        }

        /// <summary>
        /// 商品三级类编号
        /// </summary>
        private int? category3SysNo;
        [Validate(ValidateType.Required)]
        public int? Category3SysNo
        {
            get { return category3SysNo; }
            set { base.SetValue("Category3SysNo", ref category3SysNo, value); }
        }

        /// <summary>
        /// 商品三级类名称
        /// </summary>
        private string category3Name;
        public string Category3Name
        {
            get { return category3Name; }
            set { base.SetValue("Category3Name", ref category3Name, value); }
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

        /// <summary>
        /// 编号
        /// </summary>
        private int? sysNo;
        public int? SysNo
        {
            get { return sysNo; }
            set { base.SetValue("SysNo", ref sysNo, value); }
        }

        private string propertyName;
        public string PropertyName
        {
            get { return propertyName; }
            set
            {
                base.SetValue("PropertyName", ref propertyName, value);
            }
        }


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
