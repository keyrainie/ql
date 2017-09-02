using System;
using System.Collections.Generic;
using System.Json;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using ECCentral.BizEntity.IM;
using ECCentral.Portal.Basic;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.Portal.Basic.Components.UserControls.Language;
using ECCentral.Portal.UI.IM.Facades;
using ECCentral.Portal.UI.IM.Facades.Product;
using ECCentral.Portal.UI.IM.Models;
using ECCentral.Portal.UI.IM.Resources;
using ECCentral.Portal.UI.IM.UserControls;
using ECCentral.Portal.UI.IM.UserControls.Product;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using Newegg.Oversea.Silverlight.Controls;
using Newegg.Oversea.Silverlight.Controls.Components;
using Newegg.Oversea.Silverlight.Controls.Data;
using Newegg.Oversea.Silverlight.Utilities.Validation;
using ECCentral.Portal.UI.IM.Models.Product;

namespace ECCentral.Portal.UI.IM.Views
{
    [View(IsSingleton = true, SingletonType = SingletonTypes.Url)]
    public partial class ProductQuery : PageBase
    {
        #region 属性
        ProductQueryExVM model;
        #endregion

        #region 初始化加载

        public ProductQuery()
        {
            InitializeComponent();
        }

        public override void OnPageLoad(object sender, EventArgs e)
        {
            base.OnPageLoad(sender, e);
            model = new ProductQueryExVM();
            ucProductQueryCondition.ucCategoryPicker.LoadCategoryCompleted += CategoryUserControlLoaded;
            this.DataContext = model;
            ucProductQueryCondition.cbProductManagerList.PMLoaded += cbProductManagerList_PMLoaded;
            ucProductQueryCondition.OnStockListSelectionChanged += ucProductQueryCondition_OnStockListSelectionChanged;

        }

        //加载完成后赋值
        private void CategoryUserControlLoaded(object sender, EventArgs e)
        {
            model = new ProductQueryExVM();
            ProductQueryExVM.Category1List = ucProductQueryCondition.ucCategoryPicker.Category1List;
            ProductQueryExVM.Category2List = ucProductQueryCondition.ucCategoryPicker.Category2List;
            ProductQueryExVM.Category3List = ucProductQueryCondition.ucCategoryPicker.Category3List;
            ucProductQueryCondition.ucCategoryPicker.LoadCategoryCompleted -= CategoryUserControlLoaded;
        }

        private void cbProductManagerList_PMLoaded(object sender, EventArgs e)
        {
            model.PMUserCondition = 0;
            if (ProductQueryExVM.HasSeniorPMPermission)
            {
                model.PMUserCondition = 3;
            }
            else if (ProductQueryExVM.HasJuniorPMPermission)
            {
                model.PMUserCondition = 1;
            }
        }
        #endregion

        #region 查询绑定
        private void btnProductSearch_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidationManager.Validate(this))
            {
                return;
            }
            ucProductQueryResult.cbDemo.IsChecked = false;
            ucProductQueryResult.dgProductQueryResult.Bind();

        }

        private void dgProductQueryResult_LoadingDataSource(object sender, LoadingDataEventArgs e)
        {
            ProductQueryFacade facade = new ProductQueryFacade(this);
            var _vm = (ProductQueryExVM)this.DataContext;
            if (_vm != null && _vm.PMUserCondition <= 0)
            {
                setPMUserCondition(_vm);
            }

            facade.QueryProductEx(_vm, e.PageSize, e.PageIndex, e.SortField, (obj, args) =>
            {
                var list = new List<dynamic>();
                foreach (var row in args.Result.Rows)
                {
                    list.Add(row);
                }
                this.ucProductQueryResult.dgProductQueryResult.ItemsSource = list;
                this.ucProductQueryResult.dgProductQueryResult.TotalCount = args.Result.TotalCount;
            });
        }

        private void dgProductQueryResult_ExportAllDataSource(object sender, EventArgs e)
        {
            ProductQueryFacade facade = new ProductQueryFacade(this);
            var _vm = (ProductQueryExVM)this.DataContext;
            if (_vm != null && _vm.PMUserCondition <= 0)
            {
                setPMUserCondition(_vm);
            }
            ColumnSet columnSet = new ColumnSet(this.ucProductQueryResult.dgProductQueryResult);
            //SysNo GroupID ProductID ProductTitle
            columnSet.Insert(0, new ColumnData() { FieldName = "C3SysNo", Title = "三级类别" });
            columnSet.Insert(0, new ColumnData() { FieldName = "C2SysNo", Title = "二级类别" });
            columnSet.Insert(0, new ColumnData() { FieldName = "C1SysNo", Title = "一级类别" });
            if (columnSet["MerchantNameDisplay"] != null) { columnSet["MerchantNameDisplay"].FieldName = "MerchantName"; }
            facade.ExportAllProductToExcel(_vm, new ColumnSet[] { columnSet });
        }
        #endregion

        #region 页面内按钮处理事件

        #region 界面事件

        private void ucProductQueryCondition_GridView_LoadingDataSource(object sender, EventArgs e)
        {
            dgProductQueryResult_LoadingDataSource(sender, (LoadingDataEventArgs)e);
        }

        private void ucProductQueryResult_GridView_ExportAllDataSource(object sender, EventArgs e)
        {
            dgProductQueryResult_ExportAllDataSource(sender, e);
        }

        //模板选择事件发生改变
        private void ucProductQueryCondition_OnStockListSelectionChanged(object sender, StockListSelectionChangedEventArgs e)
        {
            var selectItem = ucProductQueryCondition.cbProfileTemplate.SelectedItem as ProductProfileTemplateInfo;
            if (selectItem == null) return;
            Button_DelProfileTemplate.Visibility = selectItem.SysNo > 0 ? Visibility.Visible : Visibility.Collapsed;

            if (selectItem.SysNo > 0)
            {
                DataContext = e.Data;
            }
            else
            {
                //重置查询条件
                setSearchDefault();
            }
        }

        //商品多语言编辑
        private void ucProductQueryCondition_GridView_HyperlinkMultiLanguageEdit_Click(object sender, RoutedEventArgs e)
        {
            if (!AuthMgr.HasFunctionPoint(AuthKeyConst.IM_ProductQuery_MultiLanguage))
            {
                CPApplication.Current.CurrentPage.Context.Window.Alert("你无此操作权限");
                return;
            }

            dynamic product = this.ucProductQueryResult.dgProductQueryResult.SelectedItem as dynamic;
            UCMultiLanguageMaintain item = new UCMultiLanguageMaintain(product.SysNo, "Product");

            item.Dialog = Window.ShowDialog("编辑商品多语言", item, (s, args) =>
            {

            }, new Size(750, 600));
        }
        //商品日志
        private void ucProductQueryCondition_GridView_HyperlinkProductKey_Click(object sender, RoutedEventArgs e)
        {

            dynamic product = this.ucProductQueryResult.dgProductQueryResult.SelectedItem as dynamic;
            if (product != null)
            {
                this.Window.Navigate(string.Format(ConstValue.IM_LogQueryUrlFormat, product.SysNo), null, true);
            }
        }
        //图片
        private void ucProductQueryCondition_GridView_HyperlinkImageId_Click(object sender, RoutedEventArgs e)
        {
            hyperlinkProductKey_Click(sender, e);
        }
        private void ucProductQueryCondition_GridView_HyperlinkProductNameKey_Click(object sender, RoutedEventArgs e)
        {
            dynamic product = this.ucProductQueryResult.dgProductQueryResult.SelectedItem as dynamic;
            if (product != null)
            {
                this.Window.Navigate(string.Format(ConstValue.IM_ProductMaintainUrlFormat, product.SysNo), null, true);
            }
        }
        private void ucProductQueryCondition_GridView_HyperlinkGroupID_Click(object sender, RoutedEventArgs e)
        {
            dynamic product = this.ucProductQueryResult.dgProductQueryResult.SelectedItem as dynamic;
            if (product != null)
            {
                var groupSysNo = product.GroupID;
                this.Window.Navigate(string.Format(ConstValue.IM_ProductGroupMaintainFormat, groupSysNo), null, true);
            }

        }
        private void ucProductQueryCondition_GridView_HyperlinkAccountQtyKey_Click(object sender, RoutedEventArgs e)
        {
            dynamic product = this.ucProductQueryResult.dgProductQueryResult.SelectedItem as dynamic;
            if (product != null)
            {
                var prarm = Convert.ToString(product.SysNo) + ",-999";
                this.Window.Navigate(string.Format(ConstValue.Inventory_ItemQueryFormat, prarm), null, true);
            }
        }

        private void ucProductQueryCondition_GridView_HyperlinkAvailableQtyKey_Click(object sender, RoutedEventArgs e)
        {
            dynamic product = this.ucProductQueryResult.dgProductQueryResult.SelectedItem as dynamic;
            if (product != null)
            {
                var prarm = Convert.ToString(product.SysNo) + ",-999";
                this.Window.Navigate(string.Format(ConstValue.Inventory_ItemAllocatedCardQueryFormat, prarm), null, true);
            }
        }

        private void ucProductQueryCondition_GridView_HyperlinkVirtualQtyKey_Click(object sender, RoutedEventArgs e)
        {
            dynamic product = this.ucProductQueryResult.dgProductQueryResult.SelectedItem as dynamic;
            if (product != null)
            {
                var prarm = Convert.ToString(product.SysNo);
                this.Window.Navigate(string.Format(ConstValue.Inventory_VirtualRequestMaintainFormat, prarm), null, true);
            }
        }

        private void ucProductQueryCondition_GridView_HyperlinkSaleDaysKey_Click(object sender, RoutedEventArgs e)
        {
            dynamic product = this.ucProductQueryResult.dgProductQueryResult.SelectedItem as dynamic;
            if (product != null)
            {
                var prarm = Convert.ToString(product.SysNo);
                this.Window.Navigate(string.Format(ConstValue.Inventory_InventoryQueryFormat, prarm), null, true);
            }
        }
        private void ucProductQueryCondition_GridView_HyperlinkDMSValueKey_Click(object sender, RoutedEventArgs e)
        {
            dynamic product = this.ucProductQueryResult.dgProductQueryResult.SelectedItem as dynamic;
            if (product != null)
            {
                var prarm = Convert.ToString(product.SysNo);
                this.Window.Navigate(string.Format(ConstValue.Inventory_InventoryQueryFormat, prarm), null, true);
            }
        }

        private void ucProductQueryCondition_GridView_HyperlinkPurchaseQtyKey_Click(object sender, RoutedEventArgs e)
        {
            dynamic product = this.ucProductQueryResult.dgProductQueryResult.SelectedItem as dynamic;
            if (product != null)
            {
                var prarm = Convert.ToString(product.SysNo);
                this.Window.Navigate(string.Format(ConstValue.PO_PurchaseOrderWaitingInStockQuery, prarm), null, true);
            }
        }

        private void ucProductQueryCondition_GridView_HyperlinkUnmarketableProductQuantityKey_Click(object sender, RoutedEventArgs e)
        {
            dynamic product = this.ucProductQueryResult.dgProductQueryResult.SelectedItem as dynamic;
            if (product != null)
            {
                var prarm = Convert.ToString(product.SysNo);
                this.Window.Navigate(string.Format(ConstValue.Inventory_UnmarketableInventoryQuery, prarm), null, true);
            }
        }

        private void ucProductQueryCondition_GridView_HyperlinkProductNotifyTimesKey_Click(object sender, RoutedEventArgs e)
        {
            dynamic product = this.ucProductQueryResult.dgProductQueryResult.SelectedItem as dynamic;
            if (product != null)
            {
                var prarm = Convert.ToString(product.SysNo);
                this.Window.Navigate(string.Format(ConstValue.IM_ProductNotifyFormat, prarm), null, true);
            }
        }
        private void ucProductQueryCondition_GridView_linkProducId_Click(object sender, RoutedEventArgs e)
        {
            dynamic product = this.ucProductQueryResult.dgProductQueryResult.SelectedItem as dynamic;
            if (product != null)
            {
                var productSysNo = Convert.ToString(product.SysNo);
                //Ocean.20130514, Move to ControlPanelConfiguration
                string urlFormat = this.Window.Configuration.GetConfigValue(ConstValue.DomainName_ExternalSYS, ConstValue.ConfigKey_External_WebSiteProductPreviewUrl);
                this.Window.Navigate(string.Format(urlFormat, productSysNo), null, true);
            }
        }
        private void ucProductQueryCondition_GridView_linkCurrentPrice_Click(object sender, RoutedEventArgs e)
        {
            dynamic product = this.ucProductQueryResult.dgProductQueryResult.SelectedItem as dynamic;
            if (product != null)
            {
                var prarm = Convert.ToString(product.SysNo);
                this.Window.Navigate(string.Format(ConstValue.IM_ProductlinkPriceUrlFormat, prarm), null, true);
            }
        }
        private void btnProductSet_Click(object sender, RoutedEventArgs e)
        {
            setSearchDefault();
        }

        /// <summary>
        /// 批量上架
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ProductBatchOnSale_Click(object sender, RoutedEventArgs e)
        {
            if (!AuthMgr.HasFunctionPoint(AuthKeyConst.IM_ProductQuery_BatchOnSale))
            {
                CPApplication.Current.CurrentPage.Context.Window.Alert("你无此操作权限");
                return;
            }

            var productfacade = new ProductFacade();
            if (ucProductQueryResult.dgProductQueryResult.ItemsSource != null)
            {
                var viewList = ucProductQueryResult.dgProductQueryResult.ItemsSource as List<dynamic>;
                var selectSource = viewList.Where(p => p.IsChecked).ToList();
                if (selectSource == null || selectSource.Count == 0)
                {
                    Window.Alert(ResProductQuery.SelectMessageInfo, MessageType.Error);
                    return;
                }
                var auditList = (from c in selectSource
                                 select
                                   (int)c.SysNo).ToList();

                productfacade.ProductBatchOnSale(auditList, (obj, args)
                =>
                {
                    if (!args.FaultsHandle())
                    {
                        var successCount = selectSource.Count;

                        var result = new StringBuilder();

                        if (args.Result.Any())
                        {
                            foreach (var r in args.Result)
                            {
                                result.AppendLine(r.Value);
                            }
                            successCount = successCount - args.Result.Count();
                        }

                        result.AppendLine("更新成功，影响记录数" + successCount + "条");
                        if (args.Result.Any())
                        {
                            CPApplication.Current.CurrentPage.Context.
                                Window.Alert(result.ToString());
                        }
                        else
                        {
                            Window.MessageBox.Show(result.ToString().Trim(), MessageBoxType.Success);
                        }
                    }
                });
            }
        }

        /// <summary>
        /// 库存同步
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void InventorySynchronization_Click(object sender, RoutedEventArgs e)
        {

        }

        /// <summary>
        /// 立即同步
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Synchronization_Click(object sender, RoutedEventArgs e)
        {

        }

        /// <summary>
        /// 保存模板
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnProfileTemplateSave_Click(object sender, RoutedEventArgs e)
        {
            var selectItem = ucProductQueryCondition.cbProfileTemplate.SelectedItem as ProductProfileTemplateInfo;
            if (selectItem == null) return;
            var facade = new ProductProfileTemplateFacade();
            var vm = (ProductQueryExVM)DataContext;
            var tempStr = JsonHelper.JsonSerializer(vm);
            tempStr = ucProductQueryCondition.ConvertToBase64String(tempStr);
            if (selectItem.SysNo > 0)
            {
                selectItem.TemplateValue = tempStr;
                facade.ModifyProductProfileTemplate(selectItem, (obj1, arg1) =>
                                                                    {
                                                                        selectItem.UserName =
                                                                            CPApplication.Current.LoginUser.DisplayName;

                                                                        if (arg1.FaultsHandle())
                                                                        {
                                                                            return;
                                                                        }
                                                                        Window.Alert(ResProductQuery.Operate_info);
                                                                    });
            }
            else
            {
                var detail = new ProductProfileTemplateMaintain { Heard = vm.QueryFilter };
                detail.Dialog = Window.ShowDialog(ResProductQuery.ModelTitle, detail, (s, args) =>
                {
                    if (args.DialogResult == DialogResultType.OK)
                    {
                        var entity = new ProductProfileTemplateInfo
                                         {
                                             TemplateName = detail.Text,
                                             TemplateType = ProductQueryCondition.TemplateType,
                                             UserName = CPApplication.Current.LoginUser.LoginName,
                                             UserId = CPApplication.Current.LoginUser.ID,
                                             TemplateValue = tempStr,
                                             CompanyCode = CPApplication.Current.CompanyCode
                                         };
                        facade.CreateProductProfileTemplate(entity, (obj1, arg1) =>
                        {
                            if (arg1.FaultsHandle())
                            {
                                return;
                            }
                            Window.Alert(ResProductQuery.Operate_info);
                            ucProductQueryCondition.BindcbProfileTemplate(1);
                        });

                    }
                }, new Size(300, 200));
            }

        }

        /// <summary>
        /// 删除模板
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnDelProfileTemplateSave_Click(object sender, RoutedEventArgs e)
        {
            var selectItem = ucProductQueryCondition.cbProfileTemplate.SelectedItem as ProductProfileTemplateInfo;
            if (selectItem == null || selectItem.SysNo <= 0) return;
            Window.Confirm(ResProductQuery.Delete_Info, (obj, arg) =>
            {
                if (arg.DialogResult == DialogResultType.OK)
                {
                    var facade = new ProductProfileTemplateFacade();
                    facade.DeleteProductProfileTemplate(selectItem, (obj1, arg1) =>
                                                        {
                                                            if (arg1.FaultsHandle())
                                                            {
                                                                return;
                                                            }
                                                            Window.Alert(ResProductQuery.Operate_info);
                                                            ucProductQueryCondition.BindcbProfileTemplate(2);
                                                        });
                }

            });

        }

        /// <summary>
        /// 导出商检申报文件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ExporterInspection_Click(object sender, RoutedEventArgs e)
        {
            //if (!AuthMgr.HasFunctionPoint(AuthKeyConst.IM_ProductQuery_ExporterTariffApply))
            //{
            //    CPApplication.Current.CurrentPage.Context.Window.Alert("你无此操作权限");
            //    return;
            //}
            var viewList = ucProductQueryResult.dgProductQueryResult.ItemsSource as List<dynamic>;
            var selectSource = viewList.Where(p => p.IsChecked).ToList();
            if (selectSource == null || selectSource.Count == 0)
            {
                Window.Alert(ResProductQuery.SelectMessageInfo, MessageType.Error);
                return;
            }
            var productSysnos = (from c in selectSource
                                 select
                                   (int)c.SysNo).ToList();
            ProductQueryFacade facade = new ProductQueryFacade(this);
            facade.ExportInspection(productSysnos);
        }

        /// <summary>
        /// 导出报关文件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ExporterTariffApply_Click(object sender, RoutedEventArgs e)
        {
            if (!AuthMgr.HasFunctionPoint(AuthKeyConst.IM_ProductQuery_ExporterTariffApply))
            {
                CPApplication.Current.CurrentPage.Context.Window.Alert("你无此操作权限");
                return;
            }
            var viewList = ucProductQueryResult.dgProductQueryResult.ItemsSource as List<dynamic>;
            var selectSource = viewList.Where(p => p.IsChecked).ToList();
            if (selectSource == null || selectSource.Count == 0)
            {
                Window.Alert(ResProductQuery.SelectMessageInfo, MessageType.Error);
                return;
            }
            var productSysnos = (from c in selectSource
                             select
                               (int)c.SysNo).ToList();
            ProductQueryFacade facade = new ProductQueryFacade(this);
            facade.ExportTariffApply(productSysnos);
        }
        #endregion

        #endregion

        #region 跳转

        //新建商品
        private void btnProductNew_Click(object sender, RoutedEventArgs e)
        {
            Window.Navigate(ConstValue.IM_ProductMaintainCreateFormat, null, true);
        }

        //编辑商品
        private void hyperlinkProductKey_Click(object sender, RoutedEventArgs e)
        {
            dynamic product = this.ucProductQueryResult.dgProductQueryResult.SelectedItem as dynamic;
            if (product != null)
            {
                //this.Window.Navigate(string.Format(ConstValue.IM_ProductMaintainUrlFormat, product.SysNo), null, true);
                this.Window.Navigate(string.Format(ConstValue.IM_ProductResourcesUrlFormat, product.SysNo), null, true);

            }
        }
        //日志商品
        private void hyperlinkProductLog_Click(object sender, RoutedEventArgs e)
        {
            dynamic product = this.ucProductQueryResult.dgProductQueryResult.SelectedItem as dynamic;
            this.Window.Navigate(string.Format(ConstValue.IM_ProductQueryPriceChangeLogUrlFormat, product.SysNo), null, true);
        }
        #endregion

        #region 业务方法

        /// <summary>
        /// 设置默认的查询条件
        /// </summary>
        private void setSearchDefault()
        {
            model = new ProductQueryExVM();
            DataContext = model;
            ucProductQueryCondition.cbProfileTemplate.SelectedValue = 0;
            ucProductQueryCondition.cbProductManagerList.SelectedPMSysNo = CPApplication.Current.LoginUser.UserSysNo;
            ucProductQueryCondition.cbProductManagerList.SelectedPMName = CPApplication.Current.LoginUser.DisplayName;
        }

        private void setPMUserCondition(ProductQueryExVM vm)
        {
            vm.PMUserCondition = 0;
            if (ProductQueryExVM.HasSeniorPMPermission)
            {
                vm.PMUserCondition = 3;
            }
            else if (ProductQueryExVM.HasJuniorPMPermission)
            {
                vm.PMUserCondition = 1;
            }
        }
        #endregion

        private void btnBatchAudit_Click(object sender, RoutedEventArgs e)
        {
            Window.Confirm("您确定要进行批量审核操作吗？", (s, args) =>
            {
                if (args.DialogResult == DialogResultType.OK)
                {
                    var viewList = ucProductQueryResult.dgProductQueryResult.ItemsSource as List<dynamic>;
                    List<int> productSysNos = viewList.Where(p => p.IsChecked).Select(c => (int)c.SysNo).ToList();
                    if (productSysNos == null || productSysNos.Count == 0)
                    {
                        Window.Alert(ResProductQuery.SelectMessageInfo, MessageType.Error);
                        return;
                    }

                    var productfacade = new ProductFacade();
                    productfacade.BatchAuditProduct(productSysNos, ProductStatus.InActive_Audited, (result) =>
                    {
                        if (string.IsNullOrWhiteSpace(result))
                        {
                            ucProductQueryResult.dgProductQueryResult.Bind();
                        }
                        else
                        {
                            if (result.Length > 100)
                            {
                                ProductTextboxAlert content = new ProductTextboxAlert(result);
                                content.Width = 550D;
                                content.Height = 350D;
                                IDialog dialog = this.Window.ShowDialog("操作提示", content, (obj, args1) =>
                                {
                                    ucProductQueryResult.dgProductQueryResult.Bind();
                                });
                            }
                            else
                            {
                                Window.Alert("操作提示", result, MessageType.Warning, (obj, args2) => { ucProductQueryResult.dgProductQueryResult.Bind(); });
                            }
                        }
                    });
                }
            });
            #region 原有进出关审核逻辑
            //Window.Confirm("您确定要进行批量审核操作吗？", (s, args) =>
            //{
            //    if (args.DialogResult == Newegg.Oversea.Silverlight.Controls.Components.DialogResultType.OK)
            //    {
            //        ProductBatchEntryVM productBatchEntryVM = new ProductBatchEntryVM();
            //        productBatchEntryVM.Title = "批量审核商品";
            //        productBatchEntryVM.BtnPassTitle = "审核通过";
            //        productBatchEntryVM.BtnRejectTitle = "审核不通过";
            //        ProductBatchEntryNote content = new ProductBatchEntryNote(this, productBatchEntryVM);
            //        content.Width = 550D;
            //        content.Height = 350D;
            //        IDialog dialog = this.Window.ShowDialog(productBatchEntryVM.Title, content, (obj, args1) =>
            //        {
            //            if (productBatchEntryVM.AuditPass.HasValue)
            //            {
            //                if (productBatchEntryVM.AuditPass == true)
            //                {
            //                    ProductBatchEntry(ProductEntryStatus.AuditSucess, ProductEntryStatusEx.Inspection, productBatchEntryVM.Note);
            //                }
            //                else
            //                {
            //                    ProductBatchEntry(ProductEntryStatus.AuditFail, ProductEntryStatusEx.Inspection, productBatchEntryVM.Note);
            //                }
            //            }
            //        });
            //        content.Dialog = dialog;
            //    }
            //});
            #endregion
        }

        private void btnBatchSubmitInspection_Click(object sender, RoutedEventArgs e)
        {
            
            Window.Confirm("您确定要进行批量提交商检操作吗？", (s, args) =>
            {
                if (args.DialogResult == Newegg.Oversea.Silverlight.Controls.Components.DialogResultType.OK)
                    ProductBatchEntry(ProductEntryStatus.Entry, ProductEntryStatusEx.Inspection, string.Empty);
            });
        }


        private void btnBatchInspection_Click(object sender, RoutedEventArgs e)
        {
            Window.Confirm("您确定要进行批量商检操作吗？", (s, args) =>
            {
                if (args.DialogResult == Newegg.Oversea.Silverlight.Controls.Components.DialogResultType.OK)
                {
                    ProductBatchEntryVM productBatchEntryVM = new ProductBatchEntryVM();
                    productBatchEntryVM.Title = "批量商检";
                    productBatchEntryVM.BtnPassTitle = "商检通过";
                    productBatchEntryVM.BtnRejectTitle = "商检不通过";
                    ProductBatchEntryNote content = new ProductBatchEntryNote(this, productBatchEntryVM);
                    content.Width = 550D;
                    content.Height = 350D;
                    IDialog dialog = this.Window.ShowDialog(productBatchEntryVM.Title, content, (obj, args1) =>
                    {
                        if (productBatchEntryVM.AuditPass.HasValue)
                        {
                            if (productBatchEntryVM.AuditPass == true)
                            {
                                ProductBatchEntry(ProductEntryStatus.Entry, ProductEntryStatusEx.InspectionSucess, productBatchEntryVM.Note);
                            }
                            else
                            {
                                ProductBatchEntry(ProductEntryStatus.EntryFail, ProductEntryStatusEx.InspectionFail, productBatchEntryVM.Note);
                            }
                        }
                    });
                    content.Dialog = dialog;
                }
            });
        }

        private void btnBatchSubmitCustoms_Click(object sender, RoutedEventArgs e)
        {
            Window.Confirm("您确定要进行批量提交报关操作吗？", (s, args) =>
            {
                if (args.DialogResult == Newegg.Oversea.Silverlight.Controls.Components.DialogResultType.OK)
                    ProductBatchEntry(ProductEntryStatus.Entry, ProductEntryStatusEx.Customs, string.Empty);
            });
        }

        private void btnBatchCustoms_Click(object sender, RoutedEventArgs e)
        {
            
            Window.Confirm("您确定要进行批量报关操作吗？", (s, args) =>
            {
                if (args.DialogResult == Newegg.Oversea.Silverlight.Controls.Components.DialogResultType.OK)
                {
                    ProductBatchEntryVM productBatchEntryVM = new ProductBatchEntryVM();
                    productBatchEntryVM.Title = "批量报关";
                    productBatchEntryVM.BtnPassTitle = "报关通过";
                    productBatchEntryVM.BtnRejectTitle = "报关不通过";
                    ProductBatchEntryNote content = new ProductBatchEntryNote(this, productBatchEntryVM);
                    content.Width = 550D;
                    content.Height = 350D;
                    IDialog dialog = this.Window.ShowDialog(productBatchEntryVM.Title, content, (obj, args1) =>
                    {
                        if (args1 != null && args1.Data !=null)
                        {
                            ProductBatchEntryVM argsData = args1.Data as ProductBatchEntryVM;
                            if (argsData != null)
                            {
                                if (argsData.AuditPass.HasValue)
                                {
                                    if (argsData.AuditPass == true)
                                    {
                                        ProductBatchEntry(ProductEntryStatus.EntrySuccess, ProductEntryStatusEx.CustomsSuccess, argsData.Note);
                                    }
                                    else
                                    {
                                        ProductBatchEntry(ProductEntryStatus.EntryFail, ProductEntryStatusEx.CustomsFail, argsData.Note);
                                    }
                                }
                            }
                        }
                    });
                    content.Dialog = dialog;
                }
            });
        }

        public void ProductBatchEntry(ProductEntryStatus entryStatus, ProductEntryStatusEx entryStatusEx, string note)
        {
            var productfacade = new ProductFacade();
            if (ucProductQueryResult.dgProductQueryResult.ItemsSource != null)
            {
                var viewList = ucProductQueryResult.dgProductQueryResult.ItemsSource as List<dynamic>;
                var selectSource = viewList.Where(p => p.IsChecked).ToList();
                if (selectSource == null || selectSource.Count == 0)
                {
                    Window.Alert(ResProductQuery.SelectMessageInfo, MessageType.Error);
                    return;
                }
                var auditList = (from c in selectSource
                                 select
                                   (int)c.SysNo).ToList();

                productfacade.ProductBatchEntry(auditList
                    , note
                    , entryStatus
                    , entryStatusEx
                    , (result) =>
                    {
                        if (string.IsNullOrWhiteSpace(result))
                        {
                            ucProductQueryResult.dgProductQueryResult.Bind();
                        }
                        else
                        {
                            if (result.Length > 100)
                            {
                                ProductTextboxAlert content = new ProductTextboxAlert(result);
                                content.Width = 550D;
                                content.Height = 350D;
                                IDialog dialog = this.Window.ShowDialog("操作提示", content, (obj, args) =>
                                {
                                    ucProductQueryResult.dgProductQueryResult.Bind();
                                });
                            }
                            else
                            {
                                Window.Alert("操作提示", result, MessageType.Warning, (obj, args) => { ucProductQueryResult.dgProductQueryResult.Bind(); });
                            }
                        }
                    });
            }
        }
    }
}
