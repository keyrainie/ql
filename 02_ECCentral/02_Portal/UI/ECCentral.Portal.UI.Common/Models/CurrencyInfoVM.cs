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
using System.Collections.Generic;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using Newegg.Oversea.Silverlight.Utilities;
using ECCentral.QueryFilter.Common;
using Newegg.Oversea.Silverlight.Utilities.Validation;
using ECCentral.BizEntity.Common;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.BizEntity.Enum.Resources;

namespace ECCentral.Portal.UI.Common.Models
{
    public class CurrencyInfoVM : ModelBase
    {
        public CurrencyInfoVM()
        {
           
        }

        //系统编号
        public int? sysNo;
        public int? SysNo
        {
            get { return sysNo; }
            set { SetValue("SysNo", ref sysNo, value); }
        }

        
        //编号
        public string currencyID;
        public string CurrencyID
        {
            get { return currencyID; }
            set { SetValue("CurrencyID", ref currencyID, value); }
        }

        //货币名称
        public string currencyName;
        public string CurrencyName
        {
            get { return currencyName; }
            set { SetValue("CurrencyName", ref currencyName, value); }
        }

        //货币符号
        public string currencySymbol;
        public string CurrencySymbol
        {
            get { return currencySymbol; }
            set { SetValue("CurrencySymbol", ref currencySymbol, value); }
        }

        ///是否本地货币
        private bool? isLocal;
        [Validate(ValidateType.Required)]
        public bool? IsLocal
        {
            get { return isLocal; }
            set { SetValue("IsLocal", ref isLocal, value); }
        }

        //兑换本地货币汇率
        public decimal? exchangeRate;
        public decimal? ExchangeRate
        {
            get { return exchangeRate; }
            set { SetValue("ExchangeRate", ref exchangeRate, value); }
        }

        //排序编号
        public int? listOrder;
        public int? ListOrder
        {
            get { return listOrder; }
            set { SetValue("ListOrder", ref listOrder, value); }
        }

        
        //货币状态
        public CurrencyStatus? status;
        [Validate(ValidateType.Required)]
        public CurrencyStatus? Status
        {
            get { return status; }
            set { SetValue("Status", ref status, value); }
        }

    }
}
