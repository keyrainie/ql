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
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using Newegg.Oversea.Silverlight.Controls;
using ECCentral.Portal.UI.PO.UserControls;
using Newegg.Oversea.Silverlight.Controls.Components;
using Newegg.Oversea.Silverlight.Utilities.Validation;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.Portal.UI.PO.Models.EPort;
using ECCentral.Portal.UI.PO.Facades;
using ECCentral.BizEntity.PO;

namespace ECCentral.Portal.UI.PO.Views
{
    [View(IsSingleton=true)]
    public partial class EportMainPage : PageBase
    {
        public EPortVM eportVM;
        public EPortFacade facade;
        public EPortFilter queryRequest;
        public EportMainPage()
        {
            InitializeComponent();
        }
        public override void OnPageLoad(object sender, EventArgs e)
        {
            base.OnPageLoad(sender, e);

            eportVM = new EPortVM();
            BindComboBoxData();
            facade = new EPortFacade(this);
            queryRequest = new EPortFilter();
            this.DataContext = queryRequest;
        }
        private void BindComboBoxData()
        {
            this.cmbStatus.ItemsSource = EnumConverter.GetKeyValuePairs<EPortStatusENUM>(EnumConverter.EnumAppendItemType.All);
        }
        // Executes when the user navigates to this page.
        /// <summary>
        /// &
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCreateNewEport_Click(object sender, RoutedEventArgs e)
        {
            Dialog(null);
        }
        private void QueryResultGrid_LoadingDataSource(object sender, Newegg.Oversea.Silverlight.Controls.Data.LoadingDataEventArgs e)
        {
            if(string.IsNullOrWhiteSpace(this.texEportSysNo.Text))
            {
                this.queryRequest.SysNo=null;
            }
            this.queryRequest.PageInfo.PageIndex = e.PageIndex;
            this.queryRequest.PageInfo.PageSize = e.PageSize;
            this.queryRequest.PageInfo.SortBy = e.SortField;

            new EPortFacade(this).QueryEPortList(this.queryRequest, (obj, args) =>
            {
                this.QueryResultGrid.ItemsSource = args.Result.Rows;
                this.QueryResultGrid.TotalCount = args.Result.TotalCount;
            });
        }
        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            QueryResultGrid.Bind();
            //if (ValidationManager.Validate(this.StackPanelCondition))
            //{
            //    this.queryRequest = EntityConverter<EPortVM, EPortFilter>.Convert(eportVM);
            //    QueryResultGrid.Bind();
            //}
        }

        #region 私有方法
        private void Dialog(int? sysNo)
        {
            UCAddEPort uc = new UCAddEPort(sysNo);
            uc.Dialog = Window.ShowDialog(sysNo.HasValue ? "编辑电子口岸" : "新建电子口岸", uc, (obj, args) =>
            {
                if (args != null)
                {
                    if (args.DialogResult == DialogResultType.OK)
                        QueryResultGrid.Bind();
                }
            });
        }
        #endregion

        private void QueryResultGrid_ExportAllClick(object sender, EventArgs e)
        {
            
        }
        /// <summary>
        /// 编辑
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Hyperlink_EditVendor_Click(object sender, RoutedEventArgs e)
        {
            dynamic item = this.QueryResultGrid.SelectedItem as dynamic;
            Dialog(item.SysNo);
        }
        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Hyperlink_DeleteVendor_Click(object sender, RoutedEventArgs e)
        {
            dynamic item = this.QueryResultGrid.SelectedItem as dynamic;
            int sysNo = item.SysNo;
            new EPortFacade(this).DeleteEPort(sysNo, (obj, args) =>
                {
                    QueryResultGrid.Bind();
                });
        }

    }
}
