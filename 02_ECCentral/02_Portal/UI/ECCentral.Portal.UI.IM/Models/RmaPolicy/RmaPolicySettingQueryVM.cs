using ECCentral.Portal.Basic.Utilities;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using System;
using System.Collections.Generic;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace ECCentral.Portal.UI.IM.Models
{
    public class RmaPolicySettingQueryVM : ModelBase
    {
        public RmaPolicySettingQueryVM()
        {
            RmaPolicyList = EnumConverter.GetKeyValuePairs<Int32>(EnumConverter.EnumAppendItemType.All);
        }

        private bool isChecked;
        public bool IsChecked
        {
            get { return isChecked; }
            set { base.SetValue("IsChecked", ref isChecked, value); }
        }


        /// <summary>
        /// 系统编号
        /// </summary>
        public int? SysNo { get; set; }

        /// <summary>
        /// 品牌编号
        /// </summary>
        public int? BrandSysNo { get; set; }

        /// <summary>
        /// 品牌名称
        /// </summary>
        public string BrandName { get; set; }

        /// <summary>
        /// 一级类
        /// </summary>
        public int? C1SysNo { get; set; }

        /// <summary>
        /// 二级类
        /// </summary>
        public int? C2SysNo { get; set; }

        /// <summary>
        /// 三级类
        /// </summary>
        public int? C3SysNo { get; set; }


        /// <summary>
        /// 一级分类名称
        /// </summary>
        public string C1Name { get; set; }

        /// <summary>
        /// 二级分类名称
        /// </summary>
        public string C2Name { get; set; }

        /// <summary>
        /// 三级类名称
        /// </summary>
        public string C3Name { get; set; }


        /// <summary>
        /// 退货政策编号
        /// </summary>
        public int? RMAPolicySysNo { get; set; }

        /// <summary>
        /// 退货政策名称
        /// </summary>
        public String RMAPolicyName { get; set; }

        /// <summary>
        /// 新建日期
        /// </summary>
        public DateTime InDate { get; set; }

        /// <summary>
        /// 新建用户
        /// </summary>
        public String InUser { get; set; }

        /// <summary>
        /// 创建人/创建时间
        /// </summary>
        public String InUserAndInDate
        {
            get
            {
                return String.Format("{0}[{1}]", InUser, InDate);
            }
        }

        /// <summary>
        /// 编辑日期
        /// </summary>
        public DateTime EditDate { get; set; }

        /// <summary>
        /// 编辑用户
        /// </summary>
        public String EditUser { get; set; }

        /// <summary>
        /// 编辑人/编辑时间
        /// </summary>
        public String EditUserAndEditDate
        {
            get
            {
                return
                    EditDate == DateTime.MinValue
                    ? String.Empty : String.Format("{0}[{1}]", EditUser, EditDate);
            }
        }

        /// <summary>
        /// 政策的下拉列表
        /// </summary>
        public List<KeyValuePair<int?, string>> RmaPolicyList { get; set; }
    }
}
