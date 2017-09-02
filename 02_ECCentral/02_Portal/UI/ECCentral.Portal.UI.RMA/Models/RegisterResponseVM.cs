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
using System.Collections.ObjectModel;
using ECCentral.BizEntity.Common;
using Newegg.Oversea.Silverlight.Utilities.Validation;
using System.Collections.Generic;
using ECCentral.Portal.Basic.Utilities;

namespace ECCentral.Portal.UI.RMA.Models
{
    public class RegisterResponseVM : ModelBase
    {
        private Int32? m_rSysNo;
        public Int32? SysNo
        {
            get { return this.m_rSysNo; }
            set { this.SetValue("SysNo", ref m_rSysNo, value); }
        }

        private String m_ResponseProductNo;
        [Validate(ValidateType.Required)]
        public String ResponseProductNo
        {
            get { return this.m_ResponseProductNo; }
            set { this.SetValue("ResponseProductNo", ref m_ResponseProductNo, value); }
        }

        private String m_VendorRepairResultType;
        [Validate(ValidateType.Required)]
        public String VendorRepairResultType
        {
            get { return this.m_VendorRepairResultType; }
            set { this.SetValue("VendorRepairResultType", ref m_VendorRepairResultType, value); }
        }

        private String m_ResponseDesc;
        public String ResponseDesc
        {
            get { return this.m_ResponseDesc; }
            set { this.SetValue("ResponseDesc", ref m_ResponseDesc, value); }
        }

        private Int32? m_ResponseUserSysNo;
        public Int32? ResponseUserSysNo
        {
            get { return this.m_ResponseUserSysNo; }
            set { this.SetValue("ResponseUserSysNo", ref m_ResponseUserSysNo, value); }
        }

        private string responseUserName;
		public string ResponseUserName 
		{ 
			get
			{
				return responseUserName;
			}			
			set
			{
				SetValue("ResponseUserName", ref responseUserName, value);
			} 
		}		

        private DateTime? m_ResponseTime;
        public DateTime? ResponseTime
        {
            get { return this.m_ResponseTime; }
            set { this.SetValue("ResponseTime", ref m_ResponseTime, value); }
        }

        public List<CodeNamePair> VendorRepairResultTypes { get; set; }
    }
}
