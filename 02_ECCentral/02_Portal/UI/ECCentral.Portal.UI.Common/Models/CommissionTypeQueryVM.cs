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
using ECCentral.QueryFilter.Common;
using Newegg.Oversea.Silverlight.Utilities.Validation;
using ECCentral.BizEntity.Common;
using ECCentral.Portal.Basic.Utilities;
using System.Collections.Generic;


namespace ECCentral.Portal.UI.Common.Models
{
    public class CommissionTypeQueryVM : ModelBase
    {
        public List<KeyValuePair<SYNStatus?, string>> ListIsOnLineShow { get; set; }
         public CommissionTypeQueryVM()
        {
            this.ListIsOnLineShow = EnumConverter.GetKeyValuePairs<SYNStatus>(EnumConverter.EnumAppendItemType.All);
            this.PagingInfo = new PagingInfo();
        }

        public PagingInfo PagingInfo { get; set; }

        private string sysNo;
        [Validate(ValidateType.Interger)]
		public string SysNo 
		{ 
			get
			{
				return sysNo;
			}			
			set
			{
				SetValue("SysNo", ref sysNo, value);
			} 
		}
		
        private string commissionTypeName;
        public string CommissionTypeName 
		{ 
			get
			{
                return commissionTypeName;
			}			
			set
			{
                SetValue("CommissionTypeName", ref commissionTypeName, value);
			} 
		}

        private SYNStatus? m_isOnlineShow;
        public SYNStatus? IsOnlineShow
        {
            get { return m_isOnlineShow; }
            set
            {
                SetValue("IsOnlineShow", ref m_isOnlineShow, value);
            }
        }
    }
}
