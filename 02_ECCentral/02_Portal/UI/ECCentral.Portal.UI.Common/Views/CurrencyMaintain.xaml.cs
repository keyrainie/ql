using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using ECCentral.Portal.UI.Common.Models;
using Newegg.Oversea.Silverlight.Controls;
using Newegg.Oversea.Silverlight.Utilities.Validation;
using ECCentral.Portal.UI.Common.Facades;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.BizEntity.Common;
using ECCentral.BizEntity.Enum.Resources;

namespace ECCentral.Portal.UI.Common.Views
{
    [View]
    public partial class CurrencyMaintain : PageBase
    {
        private CurrencyInfoVM _currencyInfoVm;
        private CurrencyFacade _currencyFacade;

        private string _op;
        private int? _sysNo;

        public CurrencyMaintain()
        {
            InitializeComponent();
        }

        public override void OnPageLoad(object sender, EventArgs e)
        {
            base.OnPageLoad(sender, e);
            List<KeyValuePair<CurrencyStatus?, string>> statusList = EnumConverter.GetKeyValuePairs<CurrencyStatus>();
            statusList.Insert(0, new KeyValuePair<CurrencyStatus?, string>(null, ResCommonEnum.Enum_Select));
            lisStatus.ItemsSource = statusList;

            List<KeyValuePair<bool?, string>> isLocalList = new List<KeyValuePair<bool?, string>>();
            isLocalList.Insert(0, new KeyValuePair<bool?, string>(true,"是"));
            isLocalList.Insert(1, new KeyValuePair<bool?, string>(false, "否"));
            lisIsLocal.ItemsSource = isLocalList;

            _currencyInfoVm = new CurrencyInfoVM();
            _currencyFacade = new CurrencyFacade(this);
            Currency_Feild.DataContext = _currencyInfoVm;

            if (this.Request.QueryString.ContainsKey("operation"))
            {
                _op = this.Request.QueryString["operation"];
            }
            if (this.Request.QueryString.ContainsKey("sysno"))
            {
                _sysNo = int.Parse(this.Request.QueryString["sysno"]);
                _currencyFacade.Load(_sysNo, (s, args) =>
                {
                    _currencyInfoVm = args.Result;
                    Currency_Feild.DataContext = _currencyInfoVm;
                });
            }

        }

        private void Save_New_Click(object sender, RoutedEventArgs e)
        {
            ValidationManager.Validate(Currency_Feild);
            if (_currencyInfoVm.HasValidationErrors) return;

            if (!string.IsNullOrEmpty(_op) && !string.IsNullOrEmpty(_sysNo.ToString()))
            {
                _currencyInfoVm.SysNo = _sysNo;
                _currencyFacade.Update(_currencyInfoVm, (s, args) =>
                {
                    if (args.FaultsHandle())
                        return;

                    this.Window.Alert("更新币种成功!");
                    _currencyInfoVm = args.Result;
                    //InitControl(_sysNo);
                    Currency_Feild.DataContext = _currencyInfoVm;
                });
            }
            else
            {
                _currencyFacade.Create(_currencyInfoVm, (s, args) =>
                {
                    if (args.FaultsHandle())
                        return;
                    _currencyInfoVm = args.Result;
                    this._op = "edit";
                    this._sysNo = args.Result.SysNo;
                    Currency_Feild.DataContext = _currencyInfoVm;
                    this.Window.Alert("添加币种成功");
                    //InitControl(_sysNo);

                });
            }
        }
    }
}
