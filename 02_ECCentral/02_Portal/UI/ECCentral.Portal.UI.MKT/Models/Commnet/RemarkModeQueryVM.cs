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
using System.ComponentModel;
using ECCentral.BizEntity.MKT;
using ECCentral.Portal.Basic.Utilities;
using System.Collections.Generic;

namespace ECCentral.Portal.UI.MKT.Models
{
    public class RemarkModeQueryVM: ModelBase
    {
        public RemarkModeQueryVM()
        {
            this.ShowModeList = EnumConverter.GetKeyValuePairs<RemarkTypeShow>(EnumConverter.EnumAppendItemType.None);
        }

        /// <summary>
        /// 展示模式
        /// </summary>
        public List<KeyValuePair<ECCentral.BizEntity.MKT.RemarkTypeShow?, string>> ShowModeList { get; set; }

        public int? SysNo { get; set; }

        public string CompanyCode { get; set; }

        /// <summary>
        /// 三级类别名称
        /// </summary>
        public string C3Name { get; set; }

        /// <summary>
        /// 是否选中
        /// </summary>
        private bool isChecked;
        public bool IsChecked
        {
            get { return isChecked; }
            set { base.SetValue("IsChecked", ref isChecked, value); }
        }

        private int? category1SysNo;
        public int? Category1SysNo
        {
            get { return category1SysNo; }
            set { base.SetValue("Category1SysNo", ref category1SysNo, value); }
        }

        private int? category2SysNo;
        public int? Category2SysNo
        {
            get { return category2SysNo; }
            set { base.SetValue("Category2SysNo", ref category2SysNo, value); }
        }

        private int? category3SysNo;
        public int? Category3SysNo
        {
            get { return category3SysNo; }
            set { base.SetValue("Category3SysNo", ref category3SysNo, value); }
        }

        /// <summary>
        /// 类型
        /// P=评论
        ///R=公告及促销
        ///D=网友讨论
        ///C=购物咨询
        /// </summary>
        public RemarksType RemarkType { get; set; }

        /// <summary>
        /// 节假日自动展示
        /// </summary>
        private YesOrNoBoolean? weekendRule;
        public YesOrNoBoolean? WeekendRule
        {
            get { return weekendRule; }
            set { base.SetValue("WeekendRule", ref weekendRule, value); }
        }

        /// <summary>
        /// 状态
        /// </summary>
        private RemarkTypeShow? status = RemarkTypeShow.Auto;
        public RemarkTypeShow? Status
        {
            get { return status; }
            set { base.SetValue("Status", ref status, value); }
        }

        #region UI扩展属性

        public string Manual
        {
            get
            {
                return EnumConverter.GetDescription(RemarkTypeShow.Manual);
            }
        }

        /// <summary>
        /// ”无效“字符串
        /// </summary>
        public string Auto
        {
            get
            {
                return EnumConverter.GetDescription(RemarkTypeShow.Auto);
            }
        }

        public bool IsActive
        {
            get
            {
                return Status == RemarkTypeShow.Auto;
            }
            set
            {
                if (value)
                    Status = RemarkTypeShow.Auto;
                else
                    Status = RemarkTypeShow.Manual;
            }
        }

        /// <summary>
        /// 用来更新假期对应的数据
        /// </summary>
        public bool BWeekendRule
        {
            get
            {
                return WeekendRule == YesOrNoBoolean.Yes;
            }
            set
            {
                if (value)
                    WeekendRule = YesOrNoBoolean.Yes;
                else
                    WeekendRule = YesOrNoBoolean.No;
            }
        }

        public bool StatusValue
        {
            get
            {
                return WeekendRule == YesOrNoBoolean.Yes;
            }
            set
            {
                if (value) WeekendRule = YesOrNoBoolean.Yes;
                else WeekendRule = YesOrNoBoolean.No;
            }
        }
        #endregion
    }
}
