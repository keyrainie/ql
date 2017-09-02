using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;

namespace ECCentral.Portal.UI.PO.Models
{
    public class CategoryInfoVM : ModelBase
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
        /// 类别编号
        /// </summary>
        private int? categoryID;

        public int? CategoryID
        {
            get { return categoryID; }
            set { base.SetValue("CategoryID", ref categoryID, value); }
        }

        /// <summary>
        /// 类别名称
        /// </summary>
        private string categoryName;

        public string CategoryName
        {
            get { return categoryName; }
            set { base.SetValue("CategoryName", ref categoryName, value); }
        }

        /// <summary>
        /// 类别状态
        /// </summary>
        private int? status;

        public int? Status
        {
            get { return status; }
            set { base.SetValue("Status", ref status, value); }
        }
    }
}
