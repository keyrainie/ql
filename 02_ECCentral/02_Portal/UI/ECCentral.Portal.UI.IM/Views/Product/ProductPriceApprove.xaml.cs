using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using ECCentral.BizEntity.IM;
using ECCentral.Portal.Basic;
using ECCentral.Portal.UI.IM.Facades;
using ECCentral.Portal.UI.IM.Models;
using ECCentral.Portal.UI.IM.Resources;
using ECCentral.Portal.UI.IM.UserControls;
using ECCentral.Portal.UI.IM.UserControls.Product;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using Newegg.Oversea.Silverlight.Controls;
using Newegg.Oversea.Silverlight.Controls.Components;
using ECCentral.Portal.Basic.Utilities;

namespace ECCentral.Portal.UI.IM.Views
{
    [View(IsSingleton = true, SingletonType = SingletonTypes.Url)]
    public partial class ProductPriceApprove : PageBase
    {
        #region 构造函数以及字段
        ProductPriceRequestQueryVM model;
        ProductPriceRequestQueryFacade facade;
        List<dynamic> Lists = new List<dynamic>();

        public ProductPriceApprove()
        {
            InitializeComponent();
            this.QueryResultGrid.IsShowExcelExporter = false;
            this.QueryResultGrid.IsShowAllExcelExporter = false;
        }
        #endregion

        #region 初始化加载
        public override void OnPageLoad(object sender, EventArgs e)
        {
            base.OnPageLoad(sender, e);
            model = new ProductPriceRequestQueryVM();
            this.DataContext = model;
        }
        #endregion

        #region 按钮事件
        private void btnBatchPass_Click(object sender, RoutedEventArgs e)
        {

            if (QueryResultGrid.ItemsSource != null)
            {
                var viewList = QueryResultGrid.ItemsSource as List<dynamic>;
                var selectSource = viewList.Where(p => p.IsChecked).ToList();
                if (selectSource == null || selectSource.Count == 0)
                {
                    Window.Alert(ResProductPriceApprove.SelectMessageInfo, MessageType.Error);
                    return;
                }
                var detail = new ProductPriceRequestDemo();
                detail.Dialog = Window.ShowDialog(IM.Resources.ResProductPriceApprove.Dialog_AuditProductPriceRequest, detail, (s, args) =>
                {
                    if (args.DialogResult == DialogResultType.OK)
                    {
                        var auditList = (from c in selectSource
                                         select
                                             new ProductPriceRequestInfo
                                             {
                                                 SysNo = c.SysNo,
                                                 RequestStatus = GetProductPriceRequestPassStatus(c.AuditType, c.Status),
                                                 TLMemo = GetProductPriceRequestTLDemo(c.Status, detail.Text),
                                                 PMDMemo = GetProductPriceRequestPMDDemo(c.Status, detail.Text),
                                                 HasAdvancedAuditPricePermission = model.HasAdvancedAuditPricePermission,
                                                 HasPrimaryAuditPricePermission = model.HasPrimaryAuditPricePermission
                                             }).ToList();
                        var facade = new ProductPriceRequestQueryFacade(this);
                        facade.AuditProductPriceRequest(auditList, (obj, arg) =>
                        {
                            if (arg.FaultsHandle())
                            {
                                return;
                            }
                            Window.Alert(ResProductPriceApprove.AuditMessageInfo, MessageType.Error);
                            QueryResultGrid.Bind();
                        });
                        
                    }
                }, new Size(550, 300));

            }
        }

        private void btnBatchDeny_Click(object sender, RoutedEventArgs e)
        {
            if (QueryResultGrid.ItemsSource != null)
            {
                var viewList = QueryResultGrid.ItemsSource as List<dynamic>;
                var selectSource = viewList.Where(p => p.IsChecked).ToList();
                if (selectSource == null || selectSource.Count == 0)
                {
                    Window.Alert(ResProductPriceApprove.SelectMessageInfo, MessageType.Error);
                    return;
                }
                var detail = new ProductPriceRequestDemo();
                detail.Dialog = Window.ShowDialog(IM.Resources.ResProductPriceApprove.Dialog_AuditProductPriceRequest, detail, (s, args) =>
                {
                    if (args.DialogResult == DialogResultType.OK)
                    {
                        var auditList = (from c in selectSource
                                         select
                                             new ProductPriceRequestInfo
                                             {
                                                 SysNo = c.SysNo,
                                                 RequestStatus = ProductPriceRequestStatus.Deny,
                                                 TLMemo = GetProductPriceRequestTLDemo(c.Status, detail.Text),
                                                 PMDMemo = GetProductPriceRequestPMDDemo(c.Status, detail.Text),
                                                 HasAdvancedAuditPricePermission = model.HasAdvancedAuditPricePermission,
                                                 HasPrimaryAuditPricePermission = model.HasPrimaryAuditPricePermission
                                             }).ToList();
                        var facade = new ProductPriceRequestQueryFacade(this);
                        facade.AuditProductPriceRequest(auditList, (obj, arg) =>
                        {
                            if (arg.FaultsHandle())
                            {
                                return;
                            }
                            Window.Alert(ResProductPriceApprove.AuditMessageInfo, MessageType.Error);
                            QueryResultGrid.Bind();
                        });
                       
                    }
                }, new Size(550, 300));

            }
        }

        private void Button_Search_Click(object sender, RoutedEventArgs e)
        {
            QueryResultGrid.Bind();
        }

        private void QueryResultGrid_LoadingDataSource(object sender, Newegg.Oversea.Silverlight.Controls.Data.LoadingDataEventArgs e)
        {
            facade = new ProductPriceRequestQueryFacade(this);

            facade.QueryProductPriceRequestList(model, e.PageSize, e.PageIndex, e.SortField, (obj, args) =>
            {
                var list = new List<dynamic>();
                foreach (var row in args.Result.Rows)
                {
                    list.Add(row);
                }
                Lists = list;
                this.QueryResultGrid.ItemsSource = list;
                this.QueryResultGrid.TotalCount = args.Result.TotalCount;
            });
            cbDemo.IsChecked = false;

        }

        private void ckbSelectAllRow_Click(object sender, RoutedEventArgs e)
        {
            var ckb = sender as CheckBox;
            if (ckb == null) return;
            var viewList = QueryResultGrid.ItemsSource as dynamic;
            if (viewList == null) return;
            foreach (var view in viewList)
            {
                view.IsChecked = ckb.IsChecked != null && ckb.IsChecked.Value;
            }
            //QueryResultGrid.ItemsSource = viewList;
        }

        private void hyperlinkSysNo_Click(object sender, RoutedEventArgs e)
        {
            dynamic property = this.QueryResultGrid.SelectedItem as dynamic;

            if (property != null)
            {
                ProductPriceApproveEdit productPriceApproveMainUC = new ProductPriceApproveEdit();

                var sysNo = Convert.ToInt32(property.SysNo);
                productPriceApproveMainUC.SysNo = sysNo;
                productPriceApproveMainUC.ProductSysNo = property.ProductSysNo;
                productPriceApproveMainUC.ProductID = property.ProductID;
                productPriceApproveMainUC.PassStatus = GetProductPriceRequestPassStatus(property.AuditType, property.Status);
                productPriceApproveMainUC.Dialog = Window.ShowDialog("编辑价格审批", productPriceApproveMainUC, (s, args) =>
                {
                    if (args.DialogResult == DialogResultType.OK)
                    {
                        QueryResultGrid.Bind();
                    }
                }, new Size(800, 560));
            }
            else
            {
                Window.Alert(ResProductPriceApprove.Msg_OnSelectProperty, MessageType.Error);
            }
        }
        #endregion

        #region 方法
        /// <summary>
        /// 获得审核通过状态
        /// </summary>
        /// <param name="auditType"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        private ProductPriceRequestStatus GetProductPriceRequestPassStatus(ProductPriceRequestAuditType auditType, ProductPriceRequestStatus status)
        {
            var returnstatus = ProductPriceRequestStatus.Approved;
            if(auditType==ProductPriceRequestAuditType.SeniorAudit&&status==ProductPriceRequestStatus.Origin)
            {
                returnstatus = ProductPriceRequestStatus.NeedSeniorApprove;
            }
            return returnstatus;
        }

        /// <summary>
        /// 获得TL审核理由
        /// </summary>
        /// <param name="status"></param>
        /// <param name="demo"></param>
        /// <returns></returns>
        private string GetProductPriceRequestTLDemo(ProductPriceRequestStatus status,string demo)
        {
            var returnValue = "";
            if (status == ProductPriceRequestStatus.Origin)
            {
                returnValue = demo;
            }
            return returnValue;
        }

        /// <summary>
        /// 获得PMD审核理由
        /// </summary>
        /// <param name="status"></param>
        /// <param name="demo"></param>
        /// <returns></returns>
        private string GetProductPriceRequestPMDDemo(ProductPriceRequestStatus status, string demo)
        {
            var returnValue = "";
            if (status == ProductPriceRequestStatus.NeedSeniorApprove)
            {
                returnValue = demo;
            }
            return returnValue;
        }
        #endregion

       

    }

}
