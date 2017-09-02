using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using Newegg.Oversea.Silverlight.Utilities.Validation;
using ECCentral.Portal.UI.PO.Resources;

namespace ECCentral.Portal.UI.PO.Models
{
    public class EIMSInfoVM : ModelBase
    {
        /// <summary>
        /// 系统编号
        /// </summary>
        private int? eIMSSysNo;

        public int? EIMSSysNo
        {
            get { return eIMSSysNo; }
            set { this.SetValue("EIMSSysNo", ref eIMSSysNo, value); }
        }


        /// <summary>
        /// 返点名称
        /// </summary>
        private string eIMSName;

        public string EIMSName
        {
            get { return eIMSName; }
            set { this.SetValue("EIMSName", ref eIMSName, value); }
        }

        /// <summary>
        /// 管理采购单编号
        /// </summary>
        private int? purchaseOrderSysNo;

        public int? PurchaseOrderSysNo
        {
            get { return purchaseOrderSysNo; }
            set { this.SetValue("PurchaseOrderSysNo", ref purchaseOrderSysNo, value); }
        }

        /// <summary>
        /// 返点使用金额
        /// </summary>
        private string eIMSAmt;
        [Validate(ValidateType.Regex, @"^[0-9]+\.?[0-9]{0,6}$", ErrorMessageResourceName = "Decimal_Required", ErrorMessageResourceType = typeof(ResValidationErrorMessage))]
        public string EIMSAmt
        {
            get { return eIMSAmt; }
            set { this.SetValue("EIMSAmt", ref eIMSAmt, value); }
        }

        /// <summary>
        /// 下单时返点使用情况（已使用金额）
        /// </summary>
        private decimal? alreadyUseAmt;

        public decimal? AlreadyUseAmt
        {
            get { return alreadyUseAmt; }
            set { this.SetValue("AlreadyUseAmt", ref alreadyUseAmt, value); }
        }

        /// <summary>
        /// 下单时返点剩余金额
        /// </summary>
        private decimal? eIMSLeftAmt;

        public decimal? EIMSLeftAmt
        {
            get { return eIMSLeftAmt; }
            set { this.SetValue("EIMSLeftAmt", ref eIMSLeftAmt, value); }
        }

        /// <summary>
        /// 返点剩余金额（每次入库修改）
        /// </summary>
        private decimal? leftAmt;

        public decimal? LeftAmt
        {
            get { return leftAmt; }
            set { this.SetValue("LeftAmt", ref leftAmt, value); }
        }

        /// <summary>
        /// 返点总金额
        /// </summary>
        private decimal? eIMSTotalAmt;

        public decimal? EIMSTotalAmt
        {
            get { return eIMSTotalAmt; }
            set { this.SetValue("EIMSTotalAmt", ref eIMSTotalAmt, value); }
        }


        private bool isChecked;

        public bool IsChecked
        {
            get { return isChecked; }
            set { this.SetValue("IsChecked", ref isChecked, value); }
        }

        private DateTime m_ApproveDate;
        public DateTime ApproveDate
        {
            get { return m_ApproveDate; }
            set { this.SetValue("ApproveDate", ref m_ApproveDate, value); }
        }

        public bool IsRed
        {
            get
            {
                return IsSetRedByApproveDate(this.ApproveDate);
            }
        }

        /// <summary>
        /// 审核时间超过
        /// </summary>
        /// <param name="approveDate"></param>
        /// <returns></returns>
        private bool IsSetRedByApproveDate(DateTime approveDate)
        {
            bool isSetRed = false;
            TimeSpan tsNowDate = new TimeSpan(DateTime.Now.Ticks);
            TimeSpan tsApproveDate = new TimeSpan(approveDate.Ticks);

            TimeSpan tsDiff = tsNowDate.Subtract(tsApproveDate).Duration();
            if (tsDiff.Days > 30)
            {
                isSetRed = true;
            }

            return isSetRed;
        }
        
    }
}
