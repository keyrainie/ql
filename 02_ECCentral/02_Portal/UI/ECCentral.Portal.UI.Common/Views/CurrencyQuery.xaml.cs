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
using ECCentral.Portal.Basic;
using ECCentral.Portal.UI.Common.Resources;
using ECCentral.BizEntity.Common;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.BizEntity.Enum.Resources;

namespace ECCentral.Portal.UI.Common.Views
{
    [View(IsSingleton = true)]
    public partial class CurrencyQuery  :PageBase
    {
        public CurrencyFacade _facade;

        public CurrencyQuery()
        {
            InitializeComponent();
        }

        public override void OnPageLoad(object sender, EventArgs e)
        {
            base.OnPageLoad(sender, e);
            _facade = new CurrencyFacade(this);
            QueryResult.Bind();
        }

        private void QueryResult_LoadingDataSource(object sender, Newegg.Oversea.Silverlight.Controls.Data.LoadingDataEventArgs e)
        {
            _facade.QueryCurrencyList((obj, args) =>
            {
                List<CurrencyInfoVM> provinceList = _facade.ConvertCurrencyInfoEntityToCurrencyInfoVm(args.Result);
                QueryResult.ItemsSource = provinceList;
                QueryResult.TotalCount = provinceList.Count;

            });
        }

        private void btnNewArea_Click(object sender, RoutedEventArgs e)
        {
            
            this.Window.Navigate(ConstValue.Common_CurrencyMaintainUrlFormat + "?operation=add", null, true);
        }

        private void Hyperlink_EditData_Click(object sender, RoutedEventArgs e)
        {
            dynamic rows = QueryResult.SelectedItem;
            if (rows == null)
            {
                return;
            }
            var sysno = rows.SysNo;
            string url = string.Format(ConstValue.Common_CurrencyMaintainUrlFormat + "?sysno={0}&operation=edit", sysno);
            this.Window.Navigate(url, null, true);
        }

    }
}
