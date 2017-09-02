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
    public class PayTypeQueryVM : ModelBase
    {
        public List<KeyValuePair<HYNStatus?, string>> ListIsOnLineShow { get; set; }
        public PayTypeQueryVM()
        {
            this.ListIsOnLineShow = EnumConverter.GetKeyValuePairs<HYNStatus>(EnumConverter.EnumAppendItemType.All);
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
		
        private string payTypeName;
		public string PayTypeName 
		{ 
			get
			{
				return payTypeName;
			}			
			set
			{
				SetValue("PayTypeName", ref payTypeName, value);
			} 
		}

        private HYNStatus? m_isOnlineShow;
        public HYNStatus? IsOnlineShow
        {
            get { return m_isOnlineShow; }
            set
            {
                SetValue("IsOnlineShow", ref m_isOnlineShow, value);
            }
        }

    }
}
