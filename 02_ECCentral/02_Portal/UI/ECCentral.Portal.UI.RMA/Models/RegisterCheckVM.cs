using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;

using ECCentral.BizEntity.Common;
using ECCentral.Portal.Basic.Utilities;

using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using Newegg.Oversea.Silverlight.Utilities.Validation;


namespace ECCentral.Portal.UI.RMA.Models
{
    public class RegisterCheckVM : ModelBase
    {
        public RegisterCheckVM()
        {
            this.InspectionResultTypes = new List<CodeNamePair>();
        }

        private Int32? m_sysNo;
        public Int32? SysNo
        {
            get { return this.m_sysNo; }
            set { this.SetValue("SysNo", ref m_sysNo, value); }
        }

        private String m_InspectionResultType;
        [Validate(ValidateType.Required)]
        public String InspectionResultType
        {
            get { return this.m_InspectionResultType; }
            set { this.SetValue("InspectionResultType", ref m_InspectionResultType, value); }
        }

        private Int32? m_CheckUserSysNo;
        public Int32? CheckUserSysNo
        {
            get { return this.m_CheckUserSysNo; }
            set { this.SetValue("CheckUserSysNo", ref m_CheckUserSysNo, value); }
        }

        private string checkUserName;
		public string CheckUserName 
		{ 
			get
			{
				return checkUserName;
			}			
			set
			{
				SetValue("CheckUserName", ref checkUserName, value);
			} 
		}		

        private DateTime? m_CheckTime;
        public DateTime? CheckTime
        {
            get { return this.m_CheckTime; }
            set { this.SetValue("CheckTime", ref m_CheckTime, value); }
        }

        private String m_CheckDesc;
        public String CheckDesc
        {
            get { return this.m_CheckDesc; }
            set { this.SetValue("CheckDesc", ref m_CheckDesc, value); }
        }

        private bool m_IsRecommendRefund;
        public bool IsRecommendRefund
        {
            get { return this.m_IsRecommendRefund; }
            set { this.SetValue("IsRecommendRefund", ref m_IsRecommendRefund, value); }
        }

        public List<CodeNamePair> InspectionResultTypes { get; set; }
    }
}
