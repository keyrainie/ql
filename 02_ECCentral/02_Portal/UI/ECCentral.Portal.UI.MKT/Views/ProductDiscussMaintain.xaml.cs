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
using ECCentral.Portal.UI.MKT.Facades;
using ECCentral.Portal.UI.MKT.Models;
using ECCentral.QueryFilter.MKT;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.BizEntity.MKT;
using Newegg.Oversea.Silverlight.Controls.Components;
using ECCentral.Portal.UI.MKT.UserControls.Comment;
using ECCentral.Portal.UI.MKT.Resources;
using ECCentral.Portal.Basic;
using Newegg.Oversea.Silverlight.Utilities.Validation;

namespace ECCentral.Portal.UI.MKT.Views
{
    [View(IsSingleton = true, SingletonType = SingletonTypes.Url)]
    public partial class ProductDiscussMaintain : PageBase
    {
        private ProductDiscussQueryFilter filter;
        private ProductDiscussQueryFacade facade;
        private ProductDiscussQueryVM model;
        private List<ProductDiscussQueryVM> gridVM;

        private ProductDiscussReplyQueryFilter filterReply;
        private ProductDiscussReplyQueryVM modelReply;
        private List<ProductDiscussReplyQueryVM> gridVMReply;

        private ProductDiscussQueryFilter filterVM;
        private ProductDiscussReplyQueryFilter filterReplyVM;

        public ProductDiscussMaintain()
        {
            InitializeComponent();
        }

        public override void OnPageLoad(object sender, EventArgs e)
        {
            facade = new ProductDiscussQueryFacade(this);
            filter = new ProductDiscussQueryFilter();
            filterReply = new ProductDiscussReplyQueryFilter();
            model = new ProductDiscussQueryVM();
            modelReply = new ProductDiscussReplyQueryVM();
            model.ChannelID = "1";
            modelReply.ChannelID = "1";
            model.CompanyCode = Newegg.Oversea.Silverlight.ControlPanel.Core.CPApplication.Current.CompanyCode;
            modelReply.CompanyCode = Newegg.Oversea.Silverlight.ControlPanel.Core.CPApplication.Current.CompanyCode;
           
            //商品状态
            comProductStatus.ItemsSource = EnumConverter.GetKeyValuePairs<ECCentral.BizEntity.IM.ProductStatus>(EnumConverter.EnumAppendItemType.All);
            comReplyProductStatus.ItemsSource = EnumConverter.GetKeyValuePairs<ECCentral.BizEntity.IM.ProductStatus>(EnumConverter.EnumAppendItemType.All);


            CodeNamePairHelper.GetList("MKT", "ReplyStatus", CodeNamePairAppendItemType.All, (obj, args) =>
            {
                if (args.FaultsHandle()) return;
                comDiscussStatus.ItemsSource = args.Result;
                comReplyReviewStatus.ItemsSource = args.Result;
            });
            //顾客类型
            CodeNamePairHelper.GetList("MKT", "CustomerCategory", CodeNamePairAppendItemType.All, (obj, args) =>
            {
                if (args.FaultsHandle()) return;
                comCustomerCategory.ItemsSource = args.Result;
                comReplyCustomerCategory.ItemsSource = args.Result;
            });

            //回复类型
            //CodeNamePairHelper.GetList("MKT", "ReplySource", CodeNamePairAppendItemType.All, (obj, args) =>
            //{
            //    if (args.FaultsHandle()) return;
            //    comReplyStatus.ItemsSource = args.Result;
            //});


            ProductDiscussBaseInfo.DataContext = model;
            btnStackPanel.DataContext = model;

            replySection.DataContext = modelReply;
            btnReplyStackPanel.DataContext = modelReply;
            base.OnPageLoad(sender, e);
        }

        #region maintain

        private void QueryResultGrid_LoadingDataSource(object sender, Newegg.Oversea.Silverlight.Controls.Data.LoadingDataEventArgs e)
        {
            cbDemo.IsChecked = false;
            facade.QueryProductDiscuss(QueryResultGrid.QueryCriteria as ProductDiscussQueryFilter, e.PageSize, e.PageIndex, e.SortField, (s, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }
                gridVM = DynamicConverter<ProductDiscussQueryVM>.ConvertToVMList<List<ProductDiscussQueryVM>>(args.Result.Rows);
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
            filter = model.ConvertVM<ProductDiscussQueryVM, ProductDiscussQueryFilter>();
            filter.PageInfo = new ECCentral.QueryFilter.Common.PagingInfo()
            {
                PageSize = ConstValue.MaxRowCountLimit,
                PageIndex = 0,
                SortBy = string.Empty
            };
            facade.ExportExcelFile(filterVM, new ColumnSet[] { col });
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
                facade.BatchRefuseProductDiscuss(invalidSysNo, (obj, args) =>
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
                facade.BatchApproveProductDiscuss(invalidSysNo, (obj, args) =>
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
                facade.BatchReadProductDiscuss(invalidSysNo, (obj, args) =>
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
            ProductDiscussQueryVM item = this.QueryResultGrid.SelectedItem as ProductDiscussQueryVM;
            UCEditProductDiscuss usercontrol = new UCEditProductDiscuss();
            usercontrol.SysNo = item.SysNo.Value;
            usercontrol.Dialog = Window.ShowDialog(ResComment.Title_EditProductDiscuss, usercontrol, (obj, args) =>
            {
                QueryResultGrid.Bind();
            });
        }

        private void Button_Search_Click(object sender, RoutedEventArgs e)
        {
            if (ValidationManager.Validate(this.QuerySection))
            {
                filter = model.ConvertVM<ProductDiscussQueryVM, ProductDiscussQueryFilter>();
                filterVM = Newegg.Oversea.Silverlight.Utilities.UtilityHelper.DeepClone<ProductDiscussQueryFilter>(filter);
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


        /// <summary>
        /// 数据全部导出
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ProductDiscussReplyList_ExportAllClick(object sender, EventArgs e)
        {
            if (filterReplyVM == null || this.ProductDiscussReplyList.TotalCount < 1)
            {
                Window.Alert(ResComment.Information_ExportFailed);
                return;
            }
            ColumnSet col = new ColumnSet();
            col.Add("Title", "讨论标题", 15);
            col.Add("ProductID", "商品ID", 15);
            col.Add("Content", "回复内容", 20);
            col.Add("InUser", "创建人", 15);
            col.Add("InDate", "创建时间", 15);
            col.Add("EditUser", "更新人", 15);
            col.Add("EditDate", "更新时间", 15);

            filterReply = modelReply.ConvertVM<ProductDiscussReplyQueryVM, ProductDiscussReplyQueryFilter>();
            filterReply.PageInfo = new ECCentral.QueryFilter.Common.PagingInfo()
            {
                PageSize = ConstValue.MaxRowCountLimit,
                PageIndex = 0,
                SortBy = string.Empty
            };
            facade.ExportReplyExcelFile(filterReplyVM, new ColumnSet[] { col });
        }
        private void ProductDiscussReplyList_LoadingDataSource(object sender, Newegg.Oversea.Silverlight.Controls.Data.LoadingDataEventArgs e)
        {
            facade.QueryProductDiscussReply(ProductDiscussReplyList.QueryCriteria as ProductDiscussReplyQueryFilter, e.PageSize, e.PageIndex, e.SortField, (s, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }
                gridVMReply = DynamicConverter<ProductDiscussReplyQueryVM>.ConvertToVMList<List<ProductDiscussReplyQueryVM>>(args.Result.Rows);
                ProductDiscussReplyList.ItemsSource = gridVMReply;
                ProductDiscussReplyList.TotalCount = args.Result.TotalCount;

                if (gridVMReply != null)
                {
                    btnBatchInvalid2.Visibility = System.Windows.Visibility.Visible;
                    btnBatchVerify2.Visibility = System.Windows.Visibility.Visible;
                    btnBatchRead2.Visibility = System.Windows.Visibility.Visible;
                }
                else
                {
                    btnBatchInvalid2.Visibility = System.Windows.Visibility.Collapsed;
                    btnBatchVerify2.Visibility = System.Windows.Visibility.Collapsed;
                    btnBatchRead2.Visibility = System.Windows.Visibility.Collapsed;
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
                facade.BatchVoidProductDiscussReply(invalidSysNo, (obj, args) =>
                {
                    if (args.FaultsHandle())
                        return;

                    ProductDiscussReplyList.Bind();
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
                facade.BatchApproveProductDiscussReply(invalidSysNo, (obj, args) =>
                {
                    if (args.FaultsHandle())
                        return;

                    ProductDiscussReplyList.Bind();
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
                facade.BatchReadProductDiscussReply(invalidSysNo, (obj, args) =>
                {
                    if (args.FaultsHandle())
                        return;

                    ProductDiscussReplyList.Bind();
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
            ProductDiscussReplyQueryVM item = this.ProductDiscussReplyList.SelectedItem as ProductDiscussReplyQueryVM;
            UCEditProductDiscuss usercontrol = new UCEditProductDiscuss();
            usercontrol.SysNo = item.DiscussSysNo.Value;
            usercontrol.Dialog = Window.ShowDialog(ResComment.Title_EditProductDiscuss, usercontrol, (obj, args) =>
            {
                ProductDiscussReplyList.Bind();
            });
        }

        private void Button_Search2_Click(object sender, RoutedEventArgs e)
        {
            if (ValidationManager.Validate(this.QuerySection2))
            {
                filterReply = modelReply.ConvertVM<ProductDiscussReplyQueryVM, ProductDiscussReplyQueryFilter>();

                filterReplyVM = Newegg.Oversea.Silverlight.Utilities.UtilityHelper.DeepClone<ProductDiscussReplyQueryFilter>(filterReply);
                ProductDiscussReplyList.QueryCriteria = this.filterReply;
                ProductDiscussReplyList.Bind();
            }
        }


        private void ckbSelectRow2_Click(object sender, RoutedEventArgs e)
        {
            var checkBoxAll = sender as CheckBox;
            if (gridVMReply == null || checkBoxAll == null)
                return;
            gridVMReply.ForEach(item =>
            {
                item.IsChecked = checkBoxAll.IsChecked ?? false;
            });
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
