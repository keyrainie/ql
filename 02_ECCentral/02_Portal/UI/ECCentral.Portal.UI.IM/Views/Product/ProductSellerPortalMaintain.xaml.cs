using System;
using System.Windows;
using ECCentral.Portal.Basic;
using ECCentral.Portal.UI.IM.Facades;
using ECCentral.Portal.UI.IM.Models;
using ECCentral.Portal.UI.IM.UserControls;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using Newegg.Oversea.Silverlight.Controls;
using Newegg.Oversea.Silverlight.Controls.Data;
using System.Windows.Controls;
using System.Collections.Generic;
using System.Linq;
using Newegg.Oversea.Silverlight.Controls.Components;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.BizEntity.IM;

namespace ECCentral.Portal.UI.IM.Views
{
    [View(IsSingleton = true, SingletonType = SingletonTypes.Url)]
    public partial class ProductSellerPortalMaintain : PageBase
    {
        public ProductSellerPortalMaintain()
        {
            InitializeComponent();
        }

        SellerProductRequestQueryVM model;

        public override void OnPageLoad(object sender, EventArgs e)
        {
            base.OnPageLoad(sender, e);
            model = new SellerProductRequestQueryVM();
            this.DataContext = model;
        }


        #region 查询绑定

        private void Button_Search_Click(object sender, RoutedEventArgs e)
        {
            this.dgProductSellerPortalQueryResult.Bind();
        }

        private void dgProductSellerPortalQueryResult_LoadingDataSource(object sender, LoadingDataEventArgs e)
        {
            SellerProductRequestQueryFacade facade = new SellerProductRequestQueryFacade(this);
            model = (SellerProductRequestQueryVM)this.DataContext;
            facade.QuerySellerProductRequest(model, e.PageSize, e.PageIndex, e.SortField, (obj, args) =>
            {
                var list = new List<dynamic>();
                foreach (var row in args.Result.Rows)
                {
                    list.Add(row);
                }

                this.dgProductSellerPortalQueryResult.ItemsSource = list;
                this.dgProductSellerPortalQueryResult.TotalCount = args.Result.TotalCount;
            });
            cbDemo.IsChecked = false;
        }

        private void ckbSelectAllRow_Click(object sender, RoutedEventArgs e)
        {
            var ckb = sender as CheckBox;
            if (ckb == null) return;
            var viewList = dgProductSellerPortalQueryResult.ItemsSource as dynamic;
            foreach (var view in viewList)
            {
                view.IsChecked = ckb.IsChecked != null && ckb.IsChecked.Value;
            }

            //this.dgProductSellerPortalQueryResult.ItemsSource = viewList;

        }

        #endregion

        #region 页面内按钮处理事件

        #region 界面事件

        private void hyperlinkEdit_Click(object sender, RoutedEventArgs e)
        {
            dynamic item = this.dgProductSellerPortalQueryResult.SelectedItem as dynamic;
            if (item == null) return;
            var sysNo = Convert.ToInt32(item.SysNo);

            if ((SellerProductRequestType)item.Type == SellerProductRequestType.NewCreated)
            {

                //创建ID编辑
                ProductSellerPortalMaintainDetail detail = new ProductSellerPortalMaintainDetail();
                detail.SysNo = sysNo;
                detail.Dialog = Window.ShowDialog(ECCentral.Portal.UI.IM.Resources.ResProductSellerPortalMaintainDetail.Dialog_AddProduct, detail, (s, args) =>
                {
                    if (args.DialogResult == DialogResultType.OK)
                    {
                        dgProductSellerPortalQueryResult.Bind();
                    }
                }, new Size(750, 600));
            }

            if((SellerProductRequestType)item.Type == SellerProductRequestType.ParameterUpdate)
            {
                ProductSellerPortalParameterDetail detail = new ProductSellerPortalParameterDetail();
                detail.SysNo = sysNo;
                detail.ProductID = item.ProductID;
                detail.Dialog = Window.ShowDialog(ECCentral.Portal.UI.IM.Resources.ResProductSellerPortalParameterDetail.Dialog_ParameterEdit, detail, (s, args) =>
                {
                    if (args.DialogResult == DialogResultType.OK)
                    {
                        dgProductSellerPortalQueryResult.Bind(); 
                    }
                }, new Size(750, 600));
            }

            if ((SellerProductRequestType)item.Type == SellerProductRequestType.ImageAndDescriptionUpdate)
            {
                ProductSellerPortalImageAndDescUpdate detail = new ProductSellerPortalImageAndDescUpdate();
                detail.SysNo = sysNo;
                detail.Dialog = Window.ShowDialog(ECCentral.Portal.UI.IM.Resources.ResProductSellerPortalMaintainDetail.Dialog_AddProduct, detail, (s, args) =>
                {
                    if (args.DialogResult == DialogResultType.OK)
                    {
                        dgProductSellerPortalQueryResult.Bind();
                    }
                }, new Size(750, 600));
            }

        }

        private void hyperlinkProductID_Click(object sender, RoutedEventArgs e)
        {
            dynamic item = this.dgProductSellerPortalQueryResult.SelectedItem as dynamic;
            if (item != null)
            {
                this.Window.Navigate(string.Format(ConstValue.IM_ProductMaintainUrlFormat, item.ProductSysno), null, true);
            }
        }

        private void btnBatchAudit_Click(object sender, RoutedEventArgs e)
        {
            Window.MessageBox.Show("批量审核时，只能对待审核状态进行操作，如勾选其他状态，系统会自动忽略!", MessageBoxType.Information);
            if (dgProductSellerPortalQueryResult.ItemsSource != null)
            {
                var viewList = dgProductSellerPortalQueryResult.ItemsSource as List<dynamic>;
                var selectSource = (from p in viewList where p.IsChecked && p.Status == SellerProductRequestStatus.WaitApproval select p).ToList();
                #region Jack.G.tang 2013-1-6 update Bug95307
                /*修改原因:批量审核时，只能对待审核状态进行操作
                 *修改类容:直接筛选掉其他状态，并给予提示
                 */
                #endregion
                if (selectSource == null || selectSource.Count == 0)
                {
                    Window.Alert("请选择一条记录！", MessageType.Error);
                    return;
                }

                var auditList = (from c in selectSource
                                 select
                                     new SellerProductRequestVM
                                     {
                                         SysNo = c.SysNo,
                                         Status = c.Status,
                                         ProductName = c.ProductName
                                     }).ToList();

                var facade = new SellerProductRequestQueryFacade(this);
                facade.BatchApproveProductRequest(auditList, (obj, arg) =>
                {
                    if (arg.FaultsHandle())
                    {
                        dgProductSellerPortalQueryResult.Bind();
                        return;
                    }
                  
                });
            }
        }

        private void btnBatchDeny_Click(object sender, RoutedEventArgs e)
        {
            Window.MessageBox.Show("批量退回时，只能对待审核或审核完成两个状态进行操作，如勾选其他状态，系统会自动忽略!", MessageBoxType.Information);
            if (dgProductSellerPortalQueryResult.ItemsSource != null)
            {
                var viewList = dgProductSellerPortalQueryResult.ItemsSource as List<dynamic>;
                #region Jack.G.tang 2013-1-6 update Bug95307
                /*修改原因:批量退回时会将勾选的所有状态都退回，实际上只有待审核和审核完成能退回
                 *修改类容:直接筛选掉其他状态，并给予提示
                 */
                #endregion
                var selectSource = (from p in viewList where p.IsChecked && 
                                        (p.Status == SellerProductRequestStatus.WaitApproval || p.Status == SellerProductRequestStatus.Approved) 
                                    select p).ToList();
                if (selectSource == null || selectSource.Count == 0)
                {
                    Window.Alert("请选择一条记录！", MessageType.Error);
                    return;
                }

                ProductSellerPortalBatchDenyDetail detail = new ProductSellerPortalBatchDenyDetail();

                detail.SelectRows = selectSource;

                detail.Dialog = Window.ShowDialog(ECCentral.Portal.UI.IM.Resources.ResProductSellerPortalMaintain.Btn_BatchDeny, detail, (s, args) =>
                {
                    dgProductSellerPortalQueryResult.Bind();
                }, new Size(300, 200));
               
            }
        }

        private void btnBatchCreateID_Click(object sender, RoutedEventArgs e)
        {
            if (dgProductSellerPortalQueryResult.ItemsSource != null)
            {
                var viewList = dgProductSellerPortalQueryResult.ItemsSource as List<dynamic>;
                var selectSource = viewList.Where(p => p.IsChecked).ToList();
                if (selectSource == null || selectSource.Count == 0)
                {
                    Window.Alert("请选择一条记录！", MessageType.Error);
                    return;
                }

                var auditList = (from c in selectSource
                                 select
                                     new SellerProductRequestVM
                                     {
                                         SysNo = c.SysNo,
                                         Status = c.Status,
                                         ProductName = c.ProductName
                                     }).ToList();

                var facade = new SellerProductRequestQueryFacade(this);
                facade.BatchCreateIDProductRequest(auditList, (obj, arg) =>
                {
                    if (arg.FaultsHandle())
                    {
                        return;
                    }
                });
            }
        }

        private void btnExcelOutput_Click(object sender, RoutedEventArgs e)
        {

            ColumnSet col = new ColumnSet();

            col.Insert(0, "CategoryInfo.CategoryName.Content", "类别", 15)
                .Insert(1, "Manufacturer.ManufacturerNameLocal.Content", "生产商", 15)
                .Insert(2, "Brand.BrandNameLocal.Content", "品牌", 20)
                .Insert(3, "ProductGroupMode", "型号系列", 25)
                .Insert(4, "ProductName", "商品名称", 25)
                .Insert(5, "ProductModel", "具体型号", 25)
                .Insert(6, "SellerName", "供应商", 25)
                .Insert(7, "PMUser.UserDisplayName", "PM", 25)
                .Insert(8, "SellerSite", "厂商链接", 25)
                .Insert(9, "VirtualPrice", "泰隆优选供价", 25)
                .Insert(10, "CurrentPrice", "泰隆优选售价", 25)
                .Insert(11, "Margin", "毛利率(不含优惠券赠品)", 25)
                .Insert(12, "BasicPrice", "线下市场价", 25)
                .Insert(13, "IsConsign", "代销属性", 25)
                .Insert(14, "IsTakePictures", "是否拍照", 25)
                .Insert(15, "Keywords", "搜索关键字", 25)
                .Insert(16, "Weight", "重量（单位：g）", 25)
                .Insert(17, "Length", "包装尺寸（长度，单位：mm）", 25)
                .Insert(18, "Width", "包装尺寸（宽度，单位：mm）", 25)
                .Insert(19, "PackageList", "包装清单", 25)
                .Insert(20, "HostWarrantyDay", "主件保修期（天）", 25)
                .Insert(21, "PartWarrantyDay", "附件保修期（天）", 25)
                .Insert(22, "Warranty", "保修细则", 25)
                .Insert(23, "ServicePhone", "厂商售后电话", 25)
                .Insert(24, "MinPackNumber", "最小包装数量", 25)
                .Insert(25, "Note", "备注", 25);
                        

            model = (SellerProductRequestQueryVM)this.DataContext;
            var facade = new SellerProductRequestQueryFacade(this);
            facade.ExportSellerProductRequestExcelFile(model, new ColumnSet[] { col });
        }

        #endregion

        #endregion


    }
}
