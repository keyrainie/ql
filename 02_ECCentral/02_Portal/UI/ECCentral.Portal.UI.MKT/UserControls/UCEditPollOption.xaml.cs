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
using ECCentral.BizEntity.MKT;
using ECCentral.Portal.UI.MKT.Models;
using ECCentral.Portal.UI.MKT.Facades;
using ECCentral.Portal.Basic.Utilities;
using Newegg.Oversea.Silverlight.Utilities.Validation;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using ECCentral.Portal.Basic;
using Newegg.Oversea.Silverlight.Controls.Components;
using ECCentral.Portal.UI.MKT.Resources;

namespace ECCentral.Portal.UI.MKT.UserControls
{
    [View(IsSingleton = true, SingletonType = SingletonTypes.Url)]
    public partial class UCEditPollOption : UserControl
    {
        public IDialog Dialog { get; set; }
        public int SysNo { get; set; }
        public PollType? pollType { get; set; }
        private PollItemVM model;
        private PollFacade facade;

        /// <summary>
        /// 传递过来的投票主题
        /// </summary>
        //public PollMaster NewPollMaster { get; set; }

        public UCEditPollOption()
        {
            InitializeComponent();
            this.Loaded += new RoutedEventHandler(UCEditPollOption_Loaded);
        }

        void UCEditPollOption_Loaded(object sender, RoutedEventArgs e)
        {
            Loaded -= new RoutedEventHandler(UCEditPollOption_Loaded);
            model = new PollItemVM();
            facade = new PollFacade(CPApplication.Current.CurrentPage);
            PollItemInfo.DataContext = model;

            if (SysNo > 0)
            {
                PollItemInfoGrid.Bind();
            }

            if (pollType != PollType.Other)
            {
                this.SearchResult.Visibility = System.Windows.Visibility.Collapsed;
            }
        }

        private void PollItemInfoGrid_LoadingDataSource(object sender, Newegg.Oversea.Silverlight.Controls.Data.LoadingDataEventArgs e)
        {
            facade.GetPollItemList(SysNo, (obj, args) =>
            {
                if (args.FaultsHandle())
                    return;

                List<PollItemVM> list = new List<PollItemVM>();
                foreach (PollItem item in args.Result)
                {
                    list.Add(item.Convert<PollItem, PollItemVM>());
                }
                PollItemInfoGrid.ItemsSource = list;

                //PollItemInfoGrid.ItemsSource = DynamicConverter<PollItemVM>.ConvertToVMList<List<PollItemVM>>(args.Result);
                //
                facade.GetPollAnswer(SysNo, (obj2, args2) =>
                {
                    if (args2.FaultsHandle())
                        return;
                    QueryResultGrid.ItemsSource = args2.Result.ToList();
                });
            });
        }

        /// <summary>
        /// 保存投票选项
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            //if (!ValidationManager.Validate(this.PollItemInfo))
            if (string.IsNullOrEmpty(model.ItemName.Trim())) 
                CPApplication.Current.CurrentPage.Context.Window.Alert(ResNewsInfo.Information_PollItemNameIsNull, MessageType.Warning);
            else
            {
                PollItem item = EntityConvertorExtensions.ConvertVM<PollItemVM, PollItem>(model, (v, t) =>
                {
                    t.ItemName = new BizEntity.LanguageContent(ConstValue.BizLanguageCode, v.ItemName);
                });
                item.PollCount = 0;
                item.PollItemGroupSysNo = SysNo;
                facade.CreatePollItem(item, (obj, args) =>
                {
                    if (args.FaultsHandle())
                        return;
                    CPApplication.Current.CurrentPage.Context.Window.Alert(ResNewsInfo.Information_CreateSuccessful, Newegg.Oversea.Silverlight.Controls.Components.MessageType.Information);
                    PollItemInfoGrid.Bind();
                });
            }
        }

        /// <summary>
        /// 更新单个选项
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void hlUpdate_Click(object sender, RoutedEventArgs e)
        {
            PollItemVM vm = this.PollItemInfoGrid.SelectedItem as PollItemVM;
            if (string.IsNullOrEmpty(vm.ItemName))
                CPApplication.Current.CurrentPage.Context.Window.Alert(ResNewsInfo.Information_PollItemNameIsNull,MessageType.Warning);
            else
            {
                PollItem item = EntityConvertorExtensions.ConvertVM<PollItemVM, PollItem>(vm, (v, t) =>
                {
                    t.ItemName = new BizEntity.LanguageContent(ConstValue.BizLanguageCode, v.ItemName);
                });

                //PollItemGroup item = vm.ConvertVM<PollItemVM, PollItemGroup>();
                facade.UpdatePollItem(item, (obj, args) =>
                {
                    if (args.FaultsHandle())
                        return;
                    CPApplication.Current.CurrentPage.Context.Window.Alert(ResNewsInfo.Information_OperateSuccessful, Newegg.Oversea.Silverlight.Controls.Components.MessageType.Information);
                    PollItemInfoGrid.Bind();
                });
            }
        }

        /// <summary>
        /// 数据全部导出
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        //private void PollItemInfoGrid_ExportAllClick(object sender, EventArgs e)
        //{
        //    //ColumnSet col = new ColumnSet(this.DataGrid_Product_ResultList);
        //    //col.Insert(0, "ProductId", ResRMAReports.Excel_ProductID, 20)
        //    //    .SetWidth("ProductName", 30);
        //    //facade.ExportProductExcelFile(queryVM, new ColumnSet[] { col });
        //}
        /// <summary>
        /// 删除单个选项
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void hlDelete_Click(object sender, RoutedEventArgs e)
        {
            PollItemVM vm = this.PollItemInfoGrid.SelectedItem as PollItemVM;
            if (vm != null)
            {
                facade.DeletePollItem(vm.SysNo.Value, (obj, args) =>
                {
                    if (args.FaultsHandle())
                        return;

                    PollItemInfoGrid.Bind();
                });
            }
        }
    }

}
