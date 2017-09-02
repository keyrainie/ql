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
using System.Windows.Navigation;

using Newegg.Oversea.Silverlight.Controls;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using ECCentral.Portal.Basic;
using ECCentral.Portal.Basic.Components.Models;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using ECCentral.BizEntity.Enum.Resources;
using ECCentral.Portal.UI.MKT.Resources;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.BizEntity.MKT;
using ECCentral.Portal.UI.MKT.Models.Promotion.QueryFilter;
using ECCentral.Portal.UI.MKT.Facades.Promotion;
using ECCentral.QueryFilter.Common;
using Newegg.Oversea.Silverlight.Controls.Components;
using Newegg.Oversea.Silverlight.Utilities.Validation;
using ECCentral.BizEntity.PO;
using ECCentral.Portal.UI.MKT.UserControls;

namespace ECCentral.Portal.UI.MKT.Views
{
    [View(IsSingleton = true, SingletonType = SingletonTypes.Url)]
    public partial class CountdownQuery : PageBase
    {
        private CountdownQueryFilterVM _ViewModel;
        private CountdownFacade _Facade;
        public CountdownQuery()
        {
            InitializeComponent();
        }
        public override void OnPageLoad(object sender, EventArgs e)
        {
            List<WebChannelVM> webChennelList = new List<WebChannelVM>();
            foreach (UIWebChannel uiChannel in CPApplication.Current.CurrentWebChannelList)
            {
                webChennelList.Add(new WebChannelVM() { ChannelID = uiChannel.ChannelID, ChannelName = uiChannel.ChannelName });
            }
            lstChannel.ItemsSource = webChennelList;

            List<KeyValuePair<CountdownStatus?, string>> statusList = EnumConverter.GetKeyValuePairs<CountdownStatus>();
            statusList.Insert(0, new KeyValuePair<CountdownStatus?, string>(null, ResCommonEnum.Enum_Select));
            lstStatus.ItemsSource = statusList;
            lstStatus.SelectedIndex = 0;

            List<KeyValuePair<int?, string>> PromotionType = new List<KeyValuePair<int?, string>>();
            PromotionType.Add(new KeyValuePair<int?, string>(null, ResCommonEnum.Enum_Select));
            // PromotionType.Add(new KeyValuePair<int?, string>(1, "促销计划"));
            //PromotionType.Add(new KeyValuePair<int?, string>(1, ResCountdownQuery.Msg_PromotionPlan));
            //PromotionType.Add(new KeyValuePair<int?, string>(0, "限时抢购"));
            PromotionType.Add(new KeyValuePair<int?, string>(0, ResCountdownQuery.Msg_Countdown));
            lstPromotionType.ItemsSource = PromotionType;
            lstPromotionType.SelectedIndex = 0;

            #region 是否秒杀
            List<KeyValuePair<int?, string>> skList = new List<KeyValuePair<int?, string>>();
            skList.Add(new KeyValuePair<int?, string>(null, ResCommonEnum.Enum_Select));
            skList.Add(new KeyValuePair<int?, string>(1, ResCountdownQuery.RadioButton_Yes));
            skList.Add(new KeyValuePair<int?, string>(0, ResCountdownQuery.RadioButton_No));
            cbIsSecondKill.ItemsSource = skList;
            cbIsSecondKill.SelectedIndex = 0;
            #endregion
            _Facade = new CountdownFacade(this);


            _ViewModel = new CountdownQueryFilterVM();
            _ViewModel.ChannelID = "1";
            this.DataContext = _ViewModel;
            base.OnPageLoad(sender, e);
        }



        private void ButtonSearch_Click(object sender, RoutedEventArgs e)
        {
            if (ValidationManager.Validate(this.Grid))
            {
                this.DataGrid.QueryCriteria = Newegg.Oversea.Silverlight.Utilities.UtilityHelper.DeepClone<CountdownQueryFilterVM>(this._ViewModel);
                DataGrid.Bind();
            }
        }

        private void DataGrid_LoadingDataSource(object sender, Newegg.Oversea.Silverlight.Controls.Data.LoadingDataEventArgs e)
        {
            PagingInfo pageInfo = new QueryFilter.Common.PagingInfo()
            {
                PageIndex = e.PageIndex,
                PageSize = e.PageSize,
                SortBy = e.SortField
            };
            //IsShowCategory  GroupName="rbIsShowCategory"
            var _vmTmp = this.DataGrid.QueryCriteria as CountdownQueryFilterVM;
            _vmTmp.IsShowCategoryAll = null;
            _vmTmp.IsShowCategoryAll = rbIsShowCategoryAll.IsChecked;
            _vmTmp.IsShowCategory = rbIsShowCategory.IsChecked;
            _vmTmp.PMUserName = CPApplication.Current.LoginUser.DisplayName;
            _Facade.Query(_vmTmp, pageInfo, (obj, args) =>
           {
               if (args.FaultsHandle())
               {
                   return;
               }
               List<CountdownQueryResultVM> dataSource = DynamicConverter<CountdownQueryResultVM>.ConvertToVMList(args.Result.Rows);
               dataSource.ForEach(s => 
               { if (s.IsPromotionSchedule) 
               {
                   if(string.IsNullOrEmpty(s.SalesVolume))
                   {s.SalesVolume="--";}
               } else 
               { if ( string.IsNullOrEmpty(s.SalesVolume)) 
               { 
                   s.SalesVolume = "0";
               }
               } 
               });
               DataGrid.ItemsSource = dataSource;
               DataGrid.TotalCount = args.Result.TotalCount;
           });

        }

        private void ButtonCreateCountdown_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            string op = btn.Name.Equals("ButtonCreateSchedule") ? "sdnew" : "cdnew";
            string url = string.Format(ConstValue.MKT_CountdownMaintainUrlFormat, op);
            this.Window.Navigate(url, null, true);
        }

        private void DataGridCheckBoxAllCode_Click(object sender, RoutedEventArgs e)
        {
            CheckBox chk = (CheckBox)sender;
            dynamic rows = DataGrid.ItemsSource;
            foreach (dynamic row in rows)
            {
                CountdownStatus status = (CountdownStatus)row.Status;

                if (status == CountdownStatus.Init
                    || status == CountdownStatus.WaitForPrimaryVerify
                    || status == CountdownStatus.WaitForVerify
                    || status == CountdownStatus.Ready
                    || status == CountdownStatus.VerifyFaild)
                {
                    row.IsChecked = chk.IsChecked.Value;
                }
            }
        }

        private void ButtonEdit_Click(object sender, RoutedEventArgs e)
        {
            dynamic data = this.DataGrid.SelectedItem as dynamic;
            HyperlinkButton btn = sender as HyperlinkButton;
            if (data != null)
            {
                string op = btn.Name.Equals("hybtnEdit") ? "edt" : "mge";
                //op = (data.PromotionType.Equals("限时抢购") ? "cd" : "sd") + op;
                op = (data.PromotionType.Equals(ResCountdownQuery.Msg_Countdown) ? "cd" : "sd") + op;
                string url = string.Format(ConstValue.MKT_CountdownMaintainUrlFormat, "?op=" + op + "&sysNo=" + data.SysNo);
                Window.Navigate(url, null, true);
            }
            else
            {
                //Window.Alert("请选择团购后操作。", MessageType.Error);
                Window.Alert(ResCountdownQuery.Msg_AfterSelcGroupBuy, MessageType.Error);
            }
        }



        private void hlbClick(object sender, RoutedEventArgs e)
        {
            var data = DataGrid.SelectedItem as CountdownQueryResultVM;
            this.Window.Navigate(string.Format(ConstValue.MKT_CountdownMaintainUrlFormat, data.SysNo), null, true);
        }

        private void lstPromotionType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var lstPT = sender as Combox;
            if (lstPT.SelectedValue == null || lstPT.SelectedValue.ToString() == "0")
            {
                this.Grid.RowDefinitions[1].Height = new GridLength(0);
            }
            else
            {
                this.Grid.RowDefinitions[1].Height = GridLength.Auto;
            }
        }

      
        private void BtnImportCountdown_Click(object sender, RoutedEventArgs e)
        {
            CountDownImport item = new CountDownImport() { IsPromotionSchedule=false};
            item.Dialog = Window.ShowDialog(ResCountdownQuery.Button_BatchImportCountdown, item, (s, args) =>
            {
                if (args.DialogResult == Newegg.Oversea.Silverlight.Controls.Components.DialogResultType.OK)
                {
                    
                }
            });
        }

        private void BtnImportSchedule_Click(object sender, RoutedEventArgs e)
        {
            CountDownImport item = new CountDownImport() { IsPromotionSchedule=true};
            item.Dialog = Window.ShowDialog(ResCountdownQuery.Button_BatchImportPaln, item, (s, args) =>
            {
                if (args.DialogResult == Newegg.Oversea.Silverlight.Controls.Components.DialogResultType.OK)
                {

                }
            });
        }


    }

}
