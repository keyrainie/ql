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

namespace ECCentral.Portal.Basic.Components.Models
{
    public class CompanyVM : ModelBase
    {
        private string companyCode;
        public string CompanyCode {
            get
            {
                return companyCode;
            }
            set
            {
                base.SetValue("CompanyCode", ref companyCode, value);
            }
        
        }

        private string companyName;
        public string CompanyName {
            get
            {
                return companyName;
            }
            set
            {
                base.SetValue("CompanyName", ref companyName, value);
            }
        
        }
 
    }
}
