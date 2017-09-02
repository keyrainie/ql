using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;

namespace ECCentral.Portal.UI.PO.Models
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
        /// 品牌编号
        /// </summary>
        private int? brandID;

        public int? BrandID
        {
            get { return brandID; }
            set { base.SetValue("BrandID", ref brandID, value); }
        }

        /// <summary>
        /// 品牌名称
        /// </summary>
        private string brandNameDisplay;

        public string BrandNameDisplay
        {
            get { return brandNameDisplay; }
            set { base.SetValue("BrandNameDisplay", ref brandNameDisplay, value); }
        }

        /// <summary>
        /// 品牌状态
        /// </summary>
        private int? status;

        public int? Status
        {
            get { return status; }
            set { base.SetValue("Status", ref status, value); }
        }
    }
}
