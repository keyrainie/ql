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

using ECCentral.Portal.UI.IM.Facades;
using ECCentral.Portal.UI.IM.Models;

using Newegg.Oversea.Silverlight.ControlPanel.Core;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;

namespace ECCentral.Portal.UI.IM.UserControls
{
    public partial class UCProductLineListForEmptyCategory : UserControl, IListControl<ProductLineVM>
    {
        public UCProductLineListForEmptyCategory()
        {
            InitializeComponent();
        }

        private void dataProductLineList_LoadingDataSource(object sender, Newegg.Oversea.Silverlight.Controls.Data.LoadingDataEventArgs e)
        {
            var filter = this.dataProductLineList.QueryCriteria as ProductLineQueryVM;
            new ProductLineFacade(CPApplication.Current.CurrentPage).QueryProductLine(filter, e.PageSize, e.PageIndex, e.SortField, (obj, args) =>
            {
                this.dataProductLineList.ItemsSource = args.Result.Rows.ToList("IsChecked", false);
                this.dataProductLineList.TotalCount = args.Result.TotalCount;                
            });
        }

        public void BindData(object filter)
        {
            this.dataProductLineList.QueryCriteria = filter;
            this.dataProductLineList.Bind();
        }

        private void ckbSelectAllRow_Click(object sender, RoutedEventArgs e)
        {
            CheckBox chk = (CheckBox)sender;
            dynamic rows = this.dataProductLineList.ItemsSource;
            if (rows != null)
            {
                foreach (dynamic row in rows)
                {
                    row.IsChecked = chk.IsChecked.Value;
                }
            }
        }

        public List<ProductLineVM> GetSelectedSysNoList()
        {
            List<ProductLineVM> list = new List<ProductLineVM>();
            dynamic rows = this.dataProductLineList.ItemsSource;
            if (rows != null)
            {
                foreach (dynamic row in rows)
                {
                    if (row.IsChecked)
                    {
                        ProductLineVM vm = new ProductLineVM
                        {
                            C2SysNo = row.C2SysNo
                        };
                        list.Add(vm);
                    }
                }
            }
            return list;
        }
    }
}
