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
using ECCentral.Portal.Basic.Utilities;
using ECCentral.Portal.UI.MKT.Facades;
using ECCentral.Portal.UI.MKT.Models;
using ECCentral.QueryFilter.MKT;
using ECCentral.Portal.UI.MKT.UserControls.Comment;
using Newegg.Oversea.Silverlight.Controls.Components;
using ECCentral.BizEntity.MKT;
using ECCentral.Portal.Basic;
using ECCentral.Portal.UI.MKT.Resources;
using Newegg.Oversea.Silverlight.Utilities.Validation;

namespace ECCentral.Portal.UI.MKT.Views
{
    [View(IsSingleton = true, SingletonType = SingletonTypes.Url)]
    public partial class ProductConsultMaintain : PageBase
    {
        private ProductConsultQueryFilter filter;
        private ProductConsultQueryFacade facade;
        private ProductConsultQueryVM model;
        private List<ProductConsultQueryVM> gridVM;

        private ProductConsultReplyQueryFilter filterReply;
        private ProductConsultReplyQueryVM modelReply;
        private List<ProductConsultReplyQueryVM> gridVMReply;

        private ProductConsultQueryFilter filterVM;
        private ProductConsultReplyQueryFilter filterReplyVM;

        /// <summary>
        /// 是否可以全选
        /// </summary>
        private bool CanCheckAll = true;

        public ProductConsultMaintain()
        {
            InitializeComponent();
        }

        public override void OnPageLoad(object sender, EventArgs e)
        {
            facade = new ProductConsultQueryFacade(this);
            filter = new ProductConsultQueryFilter();
            filterReply = new ProductConsultReplyQueryFilter();
            model = new ProductConsultQueryVM();
            modelReply = new ProductConsultReplyQueryVM();
            modelReply.ChannelID = "1";
            model.ChannelID = "1";
            model.CompanyCode = Newegg.Oversea.Silverlight.ControlPanel.Core.CPApplication.Current.CompanyCode;
            modelReply.CompanyCode = Newegg.Oversea.Silverlight.ControlPanel.Core.CPApplication.Current.CompanyCode;

            #region 咨询
            //商品状态
            comProductStatus.ItemsSource = EnumConverter.GetKeyValuePairs<ECCentral.BizEntity.IM.ProductStatus>(EnumConverter.EnumAppendItemType.All);
		

            //咨询状态,就是相对应的回复状态
            CodeNamePairHelper.GetList("MKT", "ReplyStatus",CodeNamePairAppendItemType.All, (obj, args) =>
            {
                if (args.FaultsHandle()) return;
                comConsultStatus.ItemsSource = args.Result;
                comConsultStatusReply.ItemsSource = args.Result;//.Remove(;需要删除已经回复，需要确认
            });

            //标记类型，也就是咨询的类型
            CodeNamePairHelper.GetList("MKT", "ConsultCategory", CodeNamePairAppendItemType.All, (obj, args) =>
            {
                if (args.FaultsHandle()) return;
                comConsultCategory.ItemsSource = args.Result;
            });
            //顾客类型
            CodeNamePairHelper.GetList("MKT", "CustomerCategory", CodeNamePairAppendItemType.All, (obj, args) =>
            {
                if (args.FaultsHandle()) return;
                comCustomerCategory.ItemsSource = args.Result;
                comCustomerCategoryReply.ItemsSource = args.Result;
            });
            #endregion

            #region 回复
            comIsTopReply.ItemsSource = EnumConverter.GetKeyValuePairs<YNStatus>(EnumConverter.EnumAppendItemType.All);
            comProductStatusReply.ItemsSource = EnumConverter.GetKeyValuePairs<ECCentral.BizEntity.IM.ProductStatus>(EnumConverter.EnumAppendItemType.All);
           
            comIsType.ItemsSource = EnumConverter.GetKeyValuePairs<ReplyVendor>(EnumConverter.EnumAppendItemType.All);
            #endregion

            ProductConsultLayout.DataContext = model;
            btnStackPanel.DataContext = model;

            ProductConsultReplyLayout.DataContext = modelReply;
            btnReplyStackPanel.DataContext = modelReply;
            base.OnPageLoad(sender, e);
        }

        #region maintain
        /// <summary>
        /// 数据全部导出
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void QueryResultGrid_ExportAllClick(object sender, EventArgs e)
        {
            if (filterVM == null || this.QueryResultGrid.TotalCount < 1)
            {
                Window.Alert(ResComment.Information_ExportFailed);
                return;
            }
            ColumnSet col = new ColumnSet(this.QueryResultGrid);
            filter = model.ConvertVM<ProductConsultQueryVM, ProductConsultQueryFilter>();
            filter.PageInfo = new ECCentral.QueryFilter.Common.PagingInfo()
            {
                PageSize = ConstValue.MaxRowCountLimit,
                PageIndex = 0,
                SortBy = string.Empty
            };
            facade.ExportExcelFile(filterVM, new ColumnSet[] { col });
        }

        private void QueryResultGrid_LoadingDataSource(object sender, Newegg.Oversea.Silverlight.Controls.Data.LoadingDataEventArgs e)
        {
            facade.QueryProductConsult(QueryResultGrid.QueryCriteria as ProductConsultQueryFilter, e.PageSize, e.PageIndex, e.SortField, (s, args) =>
            {
                if (args.FaultsHandle())
                    return;

                gridVM = DynamicConverter<ProductConsultQueryVM>.ConvertToVMList<List<ProductConsultQueryVM>>(args.Result.Rows);
                QueryResultGrid.ItemsSource = gridVM;
                QueryResultGrid.TotalCount = args.Result.TotalCount;

                if (gridVM != null)
                {
                    btnBatchInvalid.Visibility = System.Windows.Visibility.Visible;
                    btnBatchVerify.Visibility = System.Windows.Visibility.Visible;
                    btnBatchRead.Visibility = System.Windows.Visibility.Visible;
                }
                else
                {
                    btnBatchInvalid.Visibility = System.Windows.Visibility.Collapsed;
                    btnBatchVerify.Visibility = System.Windows.Visibility.Collapsed;
                    btnBatchRead.Visibility = System.Windows.Visibility.Collapsed;
                }
            });	
        }

        /// <summary>
        /// 批量作废
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnBatchInvalid_Click(object sender, RoutedEventArgs e)
        {
            List<int> invalidSysNo = new List<int>();
            gridVM.ForEach(item =>
            {
                if (item.IsChecked == true)
                    invalidSysNo.Add(item.SysNo.Value);
            });
            if (invalidSysNo.Count > 0)
                facade.BatchSetProductConsultInvalid(invalidSysNo, (obj, args) =>
                {
                    if (args.FaultsHandle())
                        return;

                    QueryResultGrid.Bind();
                });
            else
                Window.Alert(ResComment.Information_MoreThanOneRecord, MessageType.Error);
        }
        
        /// <summary>
        /// 批量审核
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnBatchVerify_Click(object sender, RoutedEventArgs e)
        {
            List<int> invalidSysNo = new List<int>();
            gridVM.ForEach(item =>
            {
                if (item.IsChecked == true)
                    invalidSysNo.Add(item.SysNo.Value);
            });
            if (invalidSysNo.Count > 0)
                facade.BatchSetProductConsultValid(invalidSysNo, (obj, args) =>
                {
                    if (args.FaultsHandle())
                        return;

                    QueryResultGrid.Bind();
                });
            else
                Window.Alert(ResComment.Information_MoreThanOneRecord, MessageType.Error);
        }
        
        /// <summary>
        /// 批量阅读
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnBatchRead_Click(object sender, RoutedEventArgs e)
        {
            List<int> invalidSysNo = new List<int>();
            gridVM.ForEach(item =>
            {
                if (item.IsChecked == true)
                    invalidSysNo.Add(item.SysNo.Value);
            });
            if (invalidSysNo.Count > 0)
                facade.BatchSetProductConsultRead(invalidSysNo, (obj, args) =>
                {
                    if (args.FaultsHandle())
                        return;

                    QueryResultGrid.Bind();
                });
            else
                Window.Alert(ResComment.Information_MoreThanOneRecord, MessageType.Error);
        }

        /// <summary>
        /// 编辑
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void hlEdit_Click(object sender, RoutedEventArgs e)
        {
            ProductConsultQueryVM item = this.QueryResultGrid.SelectedItem as ProductConsultQueryVM;
            UCAddProductConsult usercontrol = new UCAddProductConsult();
            usercontrol.SysNo = item.SysNo.Value;
            usercontrol.Dialog = Window.ShowDialog(ResComment.Title_EditProductConsult, usercontrol, (obj, args) =>
            {
                QueryResultGrid.Bind();
            });
        }

        /// <summary>
        /// 链接到商品
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void hlbCheckProduct_Click(object sender, RoutedEventArgs e)
        {
            HyperlinkButton hb=sender as HyperlinkButton;
            //Ocean.20130514, Move to ControlPanelConfiguration
            string urlFormat = this.Window.Configuration.GetConfigValue(ConstValue.DomainName_ExternalSYS, ConstValue.ConfigKey_External_WebSiteProductPreviewUrl);
            UtilityHelper.OpenWebPage(string.Format(urlFormat, hb.Tag));
        }

        private void Button_Search_Click(object sender, RoutedEventArgs e)
        {
            if (ValidationManager.Validate(this.QuerySection))
            {
                filter = model.ConvertVM<ProductConsultQueryVM, ProductConsultQueryFilter>();
                filterVM = Newegg.Oversea.Silverlight.Utilities.UtilityHelper.DeepClone<ProductConsultQueryFilter>(filter);
                QueryResultGrid.QueryCriteria = this.filter;
                QueryResultGrid.Bind();
            }
        }

        private void ckbSelectRow_Click(object sender, RoutedEventArgs e)
        {
            var checkBoxAll = sender as CheckBox;
            if (gridVM == null || checkBoxAll == null)
                return;
            gridVM.ForEach(item =>
            {
                item.IsChecked = checkBoxAll.IsChecked ?? false;
            });
        }

        //更多条件
        private void ckb_MoreQueryBuilder_Click(object sender, RoutedEventArgs e)
        {
            if (ckb_MoreQueryBuilder.IsChecked.HasValue && ckb_MoreQueryBuilder.IsChecked.Value)
                spMoreQueryBuilder.Visibility = System.Windows.Visibility.Visible;
            else
                spMoreQueryBuilder.Visibility = System.Windows.Visibility.Collapsed;
        }


        #endregion


        #region reply

        private void QueryResultGrid2_LoadingDataSource(object sender, Newegg.Oversea.Silverlight.Controls.Data.LoadingDataEventArgs e)
        {
            facade.QueryProductConsultReply(QueryResultGrid2.QueryCriteria as ProductConsultReplyQueryFilter,e.PageSize, e.PageIndex, e.SortField, (s, args) =>
            {
                if (args.FaultsHandle())
                    return;

                gridVMReply = DynamicConverter<ProductConsultReplyQueryVM>.ConvertToVMList<List<ProductConsultReplyQueryVM>>(args.Result.Rows);
                QueryResultGrid2.ItemsSource = gridVMReply;
                QueryResultGrid2.TotalCount = args.Result.TotalCount;

                foreach (ProductConsultReplyQueryVM v in gridVMReply)
                {
                    if (v.Type == "M")
                        CanCheckAll = false;
                }
                if (gridVMReply != null)// && (comReplyStatus.SelectedValue == null || comReplyStatus.SelectedValue.ToString() != "M"))
                {
                    btnBatchInvalid2.Visibility = System.Windows.Visibility.Visible;
                    btnBatchVerify2.Visibility = System.Windows.Visibility.Visible;
                    btnBatchRead2.Visibility = System.Windows.Visibility.Visible;
                    btnBatchSetTop.Visibility = System.Windows.Visibility.Visible;
                    btnCancelSetTop.Visibility = System.Windows.Visibility.Visible;
                }
                else
                {
                    btnBatchInvalid2.Visibility = System.Windows.Visibility.Collapsed;
                    btnBatchVerify2.Visibility = System.Windows.Visibility.Collapsed;
                    btnBatchRead2.Visibility = System.Windows.Visibility.Collapsed;
                    btnBatchSetTop.Visibility = System.Windows.Visibility.Collapsed;
                    btnCancelSetTop.Visibility = System.Windows.Visibility.Collapsed;
                }
            });	
        }

        /// <summary>
        /// 批量作废
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnBatchInvalid2_Click(object sender, RoutedEventArgs e)
        {
            List<int> invalidSysNo = new List<int>();
            gridVMReply.ForEach(item =>
            {
                if (item.IsChecked == true)
                    invalidSysNo.Add(item.SysNo.Value);
            });
            if (invalidSysNo.Count > 0)
                facade.BatchSetProductConsultReplyInvalid(invalidSysNo, (obj, args) =>
                {
                    if (args.FaultsHandle())
                        return;

                    QueryResultGrid2.Bind();
                });
            else
                Window.Alert(ResComment.Information_MoreThanOneRecord, MessageType.Error);
        }
        
        /// <summary>
        /// 批量审核
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnBatchVerify2_Click(object sender, RoutedEventArgs e)
        {
            List<int> invalidSysNo = new List<int>();
            gridVMReply.ForEach(item =>
            {
                if (item.IsChecked == true)
                    invalidSysNo.Add(item.SysNo.Value);
            });
            if (invalidSysNo.Count > 0)
                facade.BatchSetProductConsultReplyValid(invalidSysNo, (obj, args) =>
                {
                    if (args.FaultsHandle())
                        return;

                    QueryResultGrid2.Bind();
                });
            else
                Window.Alert(ResComment.Information_MoreThanOneRecord, MessageType.Error);
        }
       
        /// <summary>
        /// 批量阅读
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnBatchRead2_Click(object sender, RoutedEventArgs e)
        {
            List<int> invalidSysNo = new List<int>();
            gridVMReply.ForEach(item =>
            {
                if (item.IsChecked == true)
                    invalidSysNo.Add(item.SysNo.Value);
            });
            if (invalidSysNo.Count > 0)
                facade.BatchSetProductConsultReplyRead(invalidSysNo, (obj, args) =>
                {
                    if (args.FaultsHandle())
                        return;

                    QueryResultGrid2.Bind();
                });
            else
                Window.Alert(ResComment.Information_MoreThanOneRecord, MessageType.Error);
        }

        /// <summary>
        /// 编辑
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void hlEditReply_Click(object sender, RoutedEventArgs e)
        {
            ProductConsultReplyQueryVM item = this.QueryResultGrid2.SelectedItem as ProductConsultReplyQueryVM;
            UCAddProductConsult usercontrol = new UCAddProductConsult();
            usercontrol.SysNo = item.ConsultSysNo.Value;
            usercontrol.Dialog = Window.ShowDialog(ResComment.Title_EditProductConsult, usercontrol, (obj, args) =>
            {
                QueryResultGrid2.Bind();
            });
        }

        private void Button_Search2_Click(object sender, RoutedEventArgs e)
        {
            if (ValidationManager.Validate(this.QuerySection2))
            {
                filterReply = modelReply.ConvertVM<ProductConsultReplyQueryVM, ProductConsultReplyQueryFilter>();
                filterReplyVM = Newegg.Oversea.Silverlight.Utilities.UtilityHelper.DeepClone<ProductConsultReplyQueryFilter>(filterReply);
                QueryResultGrid2.QueryCriteria = this.filterReply;
                QueryResultGrid2.Bind();
            }
        }

        private void ckbSelectRow2_Click(object sender, RoutedEventArgs e)
        {
            if (!CanCheckAll)
            {
                ((CheckBox)sender).IsChecked = false;
                return;
            }
            var checkBoxAll = sender as CheckBox;
            if (gridVMReply == null || checkBoxAll == null)
                return;
            gridVMReply.ForEach(item =>
            {
                item.IsChecked = checkBoxAll.IsChecked ?? false;
            });
        }

        private void btnBatchSetTop_Click(object sender, RoutedEventArgs e)
        {
            List<int> invalidSysNo = new List<int>();
            gridVMReply.ForEach(item =>
            {
                if (item.IsChecked == true)
                    invalidSysNo.Add(item.SysNo.Value);
            });
            if (invalidSysNo.Count > 0)
                facade.BatchSetProductConsultReplyTop(invalidSysNo, (obj, args) =>
                {
                    if (args.FaultsHandle())
                        return;

                    QueryResultGrid2.Bind();
                });
            else
                Window.Alert(ResComment.Information_MoreThanOneRecord, MessageType.Error);
        }
        /// <summary>
        /// 数据全部导出
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void QueryResultGrid2_ExportAllClick(object sender, EventArgs e)
        {
            if (filterReply == null || this.QueryResultGrid2.TotalCount < 1)
            {
                Window.Alert(ResComment.Information_ExportFailed);
                return;
            }
            
            ColumnSet col = new ColumnSet();
            col.Add("Status", "回复状态");
            col.Add("ReplyContent", "回复内容");
            col.Add("TopicContent", "回复主题");
            col.Add("VendorName", "商品所属商家");
            col.Add("IsTop", "是否置顶");
            col.Add("Type", "回复者类型");
            col.Add("InUser", "创建人");
            col.Add("InDate", "创建时间");
            col.Add("EditUser", "更新人");
            col.Add("EditDate", "更新时间");
            filterReply = modelReply.ConvertVM<ProductConsultReplyQueryVM, ProductConsultReplyQueryFilter>();
            filterReply.PageInfo = new ECCentral.QueryFilter.Common.PagingInfo()
            {
                PageSize = ConstValue.MaxRowCountLimit,
                PageIndex = 0,
                SortBy = string.Empty
            };
            facade.ExportReplyExcelFile(filterReply, new ColumnSet[] { col });
        }

        private void btnBatchCancelSetTop_Click(object sender, RoutedEventArgs e)
        {
            List<int> invalidSysNo = new List<int>();
            gridVMReply.ForEach(item =>
            {
                if (item.IsChecked == true)
                    invalidSysNo.Add(item.SysNo.Value);
            });
            if (invalidSysNo.Count > 0)
                facade.BatchCancelProductConsultReplyTop(invalidSysNo, (obj, args) =>
                {
                    if (args.FaultsHandle())
                        return;

                    QueryResultGrid2.Bind();
                });
            else
                Window.Alert(ResComment.Information_MoreThanOneRecord, MessageType.Error);
        }

        //更多条件
        private void ckb_MoreQueryBuilder2_Click(object sender, RoutedEventArgs e)
        {
            if (ckb_MoreQueryBuilder2.IsChecked.HasValue && ckb_MoreQueryBuilder2.IsChecked.Value)
                spMoreQueryBuilder2.Visibility = System.Windows.Visibility.Visible;
            else
                spMoreQueryBuilder2.Visibility = System.Windows.Visibility.Collapsed;
        }
        #endregion
    }
}
