using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Collections.Generic;

using ECCentral.Portal.Basic;
using ECCentral.Portal.UI.MKT.Facades;
using ECCentral.Portal.UI.MKT.Models;
using ECCentral.Portal.UI.MKT.Resources;

using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using Newegg.Oversea.Silverlight.Controls;
using Newegg.Oversea.Silverlight.Controls.Components;
using Newegg.Oversea.Silverlight.Utilities.Validation;
using System.Globalization;
using System.Threading;

namespace ECCentral.Portal.UI.MKT.Views.Promotion
{
    [View(IsSingleton = true, SingletonType = SingletonTypes.Url)]
    public partial class ComboSaleBatchMaintain : PageBase
    {
        private List<int> sysNoList;

        public ComboBatchVM VM
        {
            get
            {
                return this.gridInput.DataContext as ComboBatchVM;
            }
            set
            {
                this.gridInput.DataContext = value;
            }
        }

        public ComboSaleBatchMaintain()
        {
            InitializeComponent();
        }

        public override void OnPageLoad(object sender, EventArgs e)
        {
            base.OnPageLoad(sender, e);

            this.VM = new ComboBatchVM();
        }

        private void dataComboList_LoadingDataSource(object sender, Newegg.Oversea.Silverlight.Controls.Data.LoadingDataEventArgs e)
        {
            var comboFilter = new ComboQueryReqVM { SysNoList = this.sysNoList };
            
            this.dataComboList.QueryCriteria = comboFilter;
            new ComboFacade(this).Query(comboFilter, e.PageSize, e.PageIndex, e.SortField, (obj, args) =>
            {
                this.dataComboList.ItemsSource = args.Result.Rows.ToList();
                this.dataComboList.TotalCount = args.Result.TotalCount;
            });
        }

        /// <summary>
        /// UI上已经屏蔽了此按钮，如果客户实在需要，再开放
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnActive_Click(object sender, RoutedEventArgs e)
        {
            if (this.ucMasterItems.SelectedProductSysNoList.Count == 0)
            {
                Window.Alert(ResComboSaleBatchMaintain.Warning_NoMasterItems, MessageType.Warning);
                return;
            }
            if (this.ucItems.SelectedProductSysNoList.Count == 0)
            {
                Window.Alert(ResComboSaleBatchMaintain.Warning_NoItems, MessageType.Warning);
                return;
            }

            this.VM.MasterItems = this.ucMasterItems.SelectedProductSysNoList;
            this.VM.Items = this.ucItems.SelectedProductSysNoList;

            new ComboFacade(this).BatchUpdate(this.VM, (obj, args) =>
            {
                this.dataComboList.Bind();                
            });         
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {          
            if (ValidationManager.Validate(this.gridInput))
            {
                if (!string.IsNullOrEmpty(this.VM.Discount) && !string.IsNullOrEmpty(this.VM.DiscountRate))
                {
                    Window.Alert(ResComboSaleBatchMaintain.Warning_DiscountAndDiscountRate, MessageType.Warning);
                    return;
                }
                if (this.ucMasterItems.SelectedProductSysNoList.Count == 0)
                {
                    Window.Alert(ResComboSaleBatchMaintain.Warning_NoMasterItems, MessageType.Warning);
                    return;
                }
                if (this.ucItems.SelectedProductSysNoList.Count == 0)
                {
                    Window.Alert(ResComboSaleBatchMaintain.Warning_NoItems, MessageType.Warning);
                    return;
                }

                this.VM.MasterItems = this.ucMasterItems.SelectedProductSysNoList;
                this.VM.Items = this.ucItems.SelectedProductSysNoList;



                 //主商品和次商品不能有相同记录
                bool flag = true;
                foreach(var p in this.VM.MasterItems)                
                {
                    if (this.VM.Items.FirstOrDefault(q => q == p) != 0)
                    {
                        flag = false;
                        break;
                    }
                };
                if (!flag)
                {
                    //Window.Alert("主商品和次商品有相同记录，请重新选择商品！");
                    Window.Alert(ResComboSaleBatchMaintain.Button_Active);
                    return;
                }

                new ComboFacade(this).BatchCreate(this.VM, (obj, args) =>
                {
                    sysNoList = args.Result.Select(p => p.SysNo.Value).ToList();

                    this.dataComboList.Bind();

                    Window.Alert(ResComboSaleBatchMaintain.Info_SaveSuccessfully);
                });               
            }
        }

        private void btnEdit_Click(object sender, RoutedEventArgs e)
        {
            dynamic vm = (sender as HyperlinkButton).DataContext;
            this.Window.Navigate(string.Format(ConstValue.MKT_ComboSaleMaintainUrlFormat, vm.SysNo), null, true);
        }

        private void ckbSelectAllRow_Click(object sender, RoutedEventArgs e)
        {
            CheckBox chk = (CheckBox)sender;
            dynamic rows = this.dataComboList.ItemsSource;
            if (rows != null)
            {
                foreach (dynamic row in rows)
                {
                    row.IsChecked = chk.IsChecked.Value ? true : false;
                }
            }   
        }
    }
}