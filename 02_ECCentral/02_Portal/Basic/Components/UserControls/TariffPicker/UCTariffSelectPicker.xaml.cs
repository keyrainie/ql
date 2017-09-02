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
using ECCentral.Portal.Basic.Utilities;
using ECCentral.Portal.UI.Common.Models;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using Newegg.Oversea.Silverlight.Controls;
using Newegg.Oversea.Silverlight.Controls.Components;
using Newegg.Oversea.Silverlight.Utilities.Validation;

namespace ECCentral.Portal.Basic.Components.UserControls.TariffPicker
{
    public partial class UCTariffSelectPicker : UserControl
    {
        public IDialog Dialog { get; set; }
        private TariffFacade serviceFacade;
        public List<TariffInfoVM> _vmList;
        public TariffInfoQueryFilterVM queryfilter;


        private IWindow CurrentWindow
        {
            get
            {
                return CPApplication.Current.CurrentPage.Context.Window;
            }
        }

        public UCTariffSelectPicker()
        {
            InitializeComponent();
            queryfilter = new TariffInfoQueryFilterVM();
        
            Loaded += UCTariffSelectPicker_Loaded;
        }


        void UCTariffSelectPicker_Loaded(object sender, RoutedEventArgs e)
        {
            serviceFacade = new TariffFacade(CPApplication.Current.CurrentPage);

            this.DataContext = queryfilter;
            Loaded -= UCTariffSelectPicker_Loaded;
        } 

        protected void btnConfirm_Click(object sender, RoutedEventArgs e)
        {
            if (this.QueryResultGrid.ItemsSource != null)
            {
                foreach (TariffInfoVM entity in this.QueryResultGrid.ItemsSource)
                {
                    if (entity.IsChecked == true)
                    {
                        this.Dialog.ResultArgs.Data = entity;
                    }
                }
                this.Dialog.Close(true);
            }
        }

        private void QueryResultGrid_LoadingDataSource(object sender, Newegg.Oversea.Silverlight.Controls.Data.LoadingDataEventArgs e)
        {
            ValidationManager.Validate(this.QueryFilter);
            if (queryfilter.HasValidationErrors)
            {
                return;
            }

            serviceFacade.TariffInfoQuery(queryfilter, e.PageSize, e.PageIndex, e.SortField, (obj, args) =>
            {
                _vmList = DynamicConverter<TariffInfoVM>.ConvertToVMList<List<TariffInfoVM>>(args.Result.Rows);
                this.QueryResultGrid.ItemsSource = _vmList;
                this.QueryResultGrid.TotalCount = args.Result.TotalCount;
            });
        }

        private void Button_Search_Click(object sender, RoutedEventArgs e)
        {
            this.QueryResultGrid.Bind();

        }
        //private void ckbSelectAllRow_Click(object sender, RoutedEventArgs e)
        //{
        //    CheckBox ck = (CheckBox)sender;
        //    dynamic rows = QueryResultGrid.ItemsSource;
        //    if (rows != null)
        //    {
        //        foreach (var row in rows)
        //        {
        //            row.IsChecked = ck.IsChecked.Value;
        //        }
        //    }
        //}
    }
}
