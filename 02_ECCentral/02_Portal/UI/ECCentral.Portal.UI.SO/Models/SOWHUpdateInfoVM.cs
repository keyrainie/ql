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
using Newegg.Oversea.Silverlight.Utilities.Validation;
using ECCentral.Portal.UI.SO.Resources;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using ECCentral.Portal.Basic.Components.UserControls.StockPicker;

namespace ECCentral.Portal.UI.SO.Models
{
    public class SOWHUpdateInfoVM : ModelBase
    {
        private string m_SOSysNo;

        [Validate(ValidateType.Required)]
        [Validate(ValidateType.Regex, @"^[.,\d]{1,10}$", ErrorMessageResourceName = "Msg_SOSysNo_Check", ErrorMessageResourceType = typeof(ResSO))]
        public string SOSysNo
        {
            get { return this.m_SOSysNo; }
            set { SetValue("SOSysNo", ref m_SOSysNo, value); }
        }

        private string m_SysNo;

        public string SysNo
        {
            get { return m_SysNo; }
            set { SetValue("SysNo", ref m_SysNo, value); }
        }

        private string m_ProductSysNo;
        public string ProductSysNo
        {
            get { return this.m_ProductSysNo; }
            set { SetValue("ProductSysNo", ref m_ProductSysNo, value); }
        }

        private string m_ProductType;

        public string ProductType
        {
            get { return m_ProductType; }
            set { SetValue("ProductType", ref m_ProductType, value); }
        }

        private string m_ProductID;

        public string ProductID
        {
            get { return m_ProductID; }
            set { SetValue("ProductID", ref m_ProductID, value); }
        }

        private string m_ProductName;

        public string ProductName
        {
            get { return m_ProductName; }
            set { SetValue("ProductName", ref m_ProductName, value); }
        }

        private string m_Quantity;

        public string Quantity
        {
            get { return m_Quantity; }
            set { SetValue("Quantity", ref m_Quantity, value); }
        }

        private string m_StockName;

        public string StockName
        {
            get { return m_StockName; }
            set { SetValue("StockName", ref m_StockName, value); }
        }

        private int? m_StockSysNo;

        public int? StockSysNo
        {
            get { return m_StockSysNo; }
            set { SetValue("StockSysNo", ref m_StockSysNo, value); }
        }

        private int? m_SourceStockSysNo;

        public int? SourceStockSysNo
        {
            get { return m_SourceStockSysNo; }
            set { SetValue("SourceStockSysNo", ref m_SourceStockSysNo, value); }
        }

        public List<StockVM> StockList
        {
            get;
            set;
        }

        private bool m_IsCheck;

        public bool IsCheck
        {
            get { return m_IsCheck; }
            set { SetValue("IsCheck", ref m_IsCheck, value); }
        }

        private string m_CompanyCode;

        public string CompanyCode
        {
            get { return m_CompanyCode; }
            set { SetValue("CompanyCode", ref m_CompanyCode, value); }
        }

    }
}
