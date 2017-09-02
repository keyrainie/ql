using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;

namespace ECCentral.Portal.UI.PO.Models
{
    public class ConsignSettlementEIMSInfoVM : ModelBase
    {

        private int? m_PMSysNo;

        public int? PMSysNo
        {
            get { return m_PMSysNo; }
            set { base.SetValue("PMSysNo", ref m_PMSysNo, value); }
        }
        /// <summary>
        /// 返点编号
        /// </summary>
        private decimal? returnPointSysNo;

        public decimal? ReturnPointSysNo
        {
            get { return returnPointSysNo; }
            set { base.SetValue("ReturnPointSysNo", ref returnPointSysNo, value); }
        }

        /// <summary>
        /// 返点名称
        /// </summary>
        private string returnPointName;

        public string ReturnPointName
        {
            get { return returnPointName; }
            set { base.SetValue("ReturnPointName", ref returnPointName, value); }
        }

        /// <summary>
        /// 返点金额
        /// </summary>
        private decimal? returnPoint;

        public decimal? ReturnPoint
        {
            get { return returnPoint; }
            set { base.SetValue("ReturnPoint", ref returnPoint, value); }
        }
        /// <summary>
        /// 已使用返点数
        /// </summary>
        private decimal? usingReturnPoint;

        public decimal? UsingReturnPoint
        {
            get { return usingReturnPoint; }
            set { base.SetValue("UsingReturnPoint", ref usingReturnPoint, value); }
        }
        /// <summary>
        /// 剩余返点金额
        /// </summary>
        private decimal? remnantReturnPoint;

        public decimal? RemnantReturnPoint
        {
            get { return remnantReturnPoint; }
            set { base.SetValue("RemnantReturnPoint", ref remnantReturnPoint, value); }
        }

        private int? vendorSysNo;

        public int? VendorSysNo
        {
            get { return vendorSysNo; }
            set { base.SetValue("VendorSysNo", ref vendorSysNo, value); }
        }
    }
}
