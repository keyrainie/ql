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

namespace ECCentral.Portal.Basic.Components.UserControls.StockPicker
{
    public class StockQueryVM : ModelBase
    {
        private int? _stockSysNo;
        public int? StockSysNo
        {
            get { return _stockSysNo; }
            set
            {
                base.SetValue("StockSysNo", ref _stockSysNo, value);
            }
        }

        private string _stockID;
        public string StockID
        {
            get { return _stockID; }
            set
            {
                base.SetValue("StockID", ref _stockID, value);
            }
        }

        private string _stockName;
        public string StockName
        {
            get { return _stockName; }
            set
            {
                base.SetValue("StockName", ref _stockName, value);
            }
        }

        private string _companyCode;
        public string CompanyCode
        {
            get { return _companyCode; }
            set
            {
                base.SetValue("CompanyCode", ref _companyCode, value);
            }
        }
    }
}
