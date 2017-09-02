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

namespace ECCentral.Portal.UI.RMA.Models
{
    public class SellerRelatedVM : ModelBase
    {
        private Int32? m_SysNo;
        public Int32? SysNo
        {
            get { return this.m_SysNo; }
            set { this.SetValue("SysNo", ref m_SysNo, value); }
        }        

        private string sellerMemo;
		public string SellerMemo 
		{ 
			get
			{
				return sellerMemo;
			}			
			set
			{
                SetValue("SellerMemo", ref sellerMemo, value);
			} 
		}
		
        private string sellerOperationInfo;
		public string SellerOperationInfo 
		{ 
			get
			{
                return sellerOperationInfo;
			}			
			set
			{
                SetValue("SellerOperationInfo", ref sellerOperationInfo, value);
			} 
		}
		
        private string sellerReceived;
		public string SellerReceived 
		{ 
			get
			{
				return sellerReceived;
			}			
			set
			{
				SetValue("SellerReceived", ref sellerReceived, value);
			} 
		}		            
    }
}
