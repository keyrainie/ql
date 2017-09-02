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
using Newegg.Oversea.Silverlight.Controls;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using ECCentral.Portal.UI.MKT.Models;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.Portal.UI.MKT.Facades;
using ECCentral.QueryFilter.MKT;
using ECCentral.BizEntity.MKT;
using ECCentral.Portal.UI.MKT.UserControls.Comment;
using Newegg.Oversea.Silverlight.Controls.Components;
using ECCentral.Portal.UI.MKT.Resources;
using ECCentral.Portal.Basic;
using Newegg.Oversea.Silverlight.Utilities.Validation;
using ECCentral.Portal.UI.MKT.UserControls;

namespace ECCentral.Portal.UI.MKT.Views
{
    [View(IsSingleton = true, SingletonType = SingletonTypes.Url)]
    public partial class ProductReviewMaintain : PageBase
    {
        private ProductReviewQueryFilter filter;
        private ProductReviewQueryFacade facade;
        private ProductReviewQueryVM model;
        private List<ProductReviewQueryVM> gridVM;

        private ProductReviewReplyQueryFilter filterReply;
        private ProductReviewReplyQueryVM modelReply;
        private List<ProductReviewReplyQueryVM> gridVMReply;

        private ProductReviewQueryFilter filterVM;
        private ProductReviewReplyQueryFilter filterReplyVM;

        /// <summary>
        /// 是否可以全选
        /// </summary>
        private bool CanCheckAll = true;
        public ProductReviewMaintain()
        {
            InitializeComponent();
        }


        public override void OnPageLoad(object sender, EventArgs e)
        {
            facade = new ProductReviewQueryFacade(this);
            filter = new ProductReviewQueryFilter();
            filterReply = new ProductReviewReplyQueryFilter();
            model = new ProductReviewQueryVM();
            modelReply = new ProductReviewReplyQueryVM();
            modelReply.ChannelID = "1";
            model.ChannelID = "1";
            model.CompanyCode = Newegg.Oversea.Silverlight.ControlPanel.Core.CPApplication.Current.CompanyCode;
            modelReply.CompanyCode = Newegg.Oversea.Silverlight.ControlPanel.Core.CPApplication.Current.CompanyCode;
            //商品状态
            comProductStatus.ItemsSource = EnumConverter.GetKeyValuePairs<ECCentral.BizEntity.IM.ProductStatus>(EnumConverter.EnumAppendItemType.All);
            comReplyProductStatus.ItemsSource = EnumConverter.GetKeyValuePairs<ECCentral.BizEntity.IM.ProductStatus>(EnumConverter.EnumAppendItemType.All);

            List<KeyValuePair<YNStatus?, string>> ynSource = EnumConverter.GetKeyValuePairs<YNStatus>(EnumConverter.EnumAppendItemType.All);
            //List<KeyValuePair<NYNStatus?, string>> nynSource = EnumConverter.GetKeyValuePairs<NYNStatus>(EnumConverter.EnumAppendItemType.All);
            //首页热评
            comIndexHotReview.ItemsSource = ynSource;
            //首页服务热评
            comIndexServiceHotReview.ItemsSource = ynSource;
            //是否精华
            comEssence.ItemsSource = ynSource;
            //是否置顶
            comIsTop.ItemsSource = ynSource;
            //置底
            comIsBottom.ItemsSource = ynSource;
            //是否有用候选
            comUseful.ItemsSource = ynSource;
            //是否有用候选
            comIsCandidate.ItemsSource = ynSource;
            //CS处理状态，
            comCSProcessStatus.ItemsSource = EnumConverter.GetKeyValuePairs<ReviewProcessStatus>(EnumConverter.EnumAppendItemType.All);
            //评论状态,就是相对应的回复状态
            CodeNamePairHelper.GetList("MKT", "ReplyStatus", CodeNamePairAppendItemType.All, (obj, args) =>
            {
                if (args.FaultsHandle()) return;
                comReviewStatus.ItemsSource = args.Result;
                comReplyReviewStatus.ItemsSource = args.Result;
            });
            CodeNamePairHelper.GetList("MKT", "Scores", (obj, args) =>
            {
                if (args.FaultsHandle()) return;
                comScores.ItemsSource = args.Result;
                comScores.SelectedIndex = 0;
            });
            //顾客类型
            CodeNamePairHelper.GetList("MKT", "CustomerCategory", CodeNamePairAppendItemType.All, (obj, args) =>
            {
                if (args.FaultsHandle()) return;
                comCustomerCategory.ItemsSource = args.Result;
                comReplyCustomerCategory.ItemsSource = args.Result;
            });

            //回复类型
            CodeNamePairHelper.GetList("MKT", "ReplySource", CodeNamePairAppendItemType.All, (obj, args) =>
            {
                if (args.FaultsHandle()) return;
                comReplyStatus.ItemsSource = args.Result;
            });

            //评论类型 普通/晒单
            List<KeyValuePair<ReviewType?, string>> reviewOperate = EnumConverter.GetKeyValuePairs<ReviewType>(EnumConverter.EnumAppendItemType.All);
            comReviewType.ItemsSource = reviewOperate; 

            List<CodeNamePair> operate = new List<CodeNamePair>();
            operate.Add(new CodeNamePair() { Code = "0", Name = ResComment.Option_ALL });
            operate.Add(new CodeNamePair() { Code = ">", Name = ">" });
            operate.Add(new CodeNamePair() { Code = ">=", Name = ">=" });
            operate.Add(new CodeNamePair() { Code = "=", Name = "=" });
            operate.Add(new CodeNamePair() { Code = "<", Name = "<" });
            operate.Add(new CodeNamePair() { Code = "<=", Name = "<=" });
            comOperation.ItemsSource = operate;

            productReviewBaseInfo.DataContext = model;
            btnStackPanel.DataContext = model;
            productReviewReply.DataContext = modelReply;
            btnReplyStackPanel.DataContext = modelReply;

            SearchResult.DataContext = model;
            SearchResult2.DataContext = modelReply;
            base.OnPageLoad(sender, e);
            comOperation.SelectedIndex = 0;
        }

        #region maintain

        private void QueryResultGrid_LoadingDataSource(object sender, Newegg.Oversea.Silverlight.Controls.Data.LoadingDataEventArgs e)
        {
            var data = QueryResultGrid.QueryCriteria as ProductReviewQueryFilter;
            data.IsIndexPageServiceHotComment = comIndexServiceHotReview.SelectedValue as YNStatus?;
            data.IsIndexPageHotComment = comIndexHotReview.SelectedValue as YNStatus?;

            facade.QueryProductReview(data, e.PageSize, e.PageIndex, e.SortField, (s, args) =>
            {
                if (args.FaultsHandle())
                    return;

                gridVM = DynamicConverter<ProductReviewQueryVM>.ConvertToVMList<List<ProductReviewQueryVM>>(args.Result.Rows);
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
                facade.BatchSetProductReviewInvalid(invalidSysNo, (obj, args) =>
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
                facade.BatchSetProductReviewValid(invalidSysNo, (obj, args) =>
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
                facade.BatchSetProductReviewRead(invalidSysNo, (obj, args) =>
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
            ProductReviewQueryVM item = this.QueryResultGrid.SelectedItem as ProductReviewQueryVM;
            UCEditProductReview usercontrol = new UCEditProductReview();
            usercontrol.SysNo = item.SysNo.Value;
            usercontrol.ReplyMode = 1;
            usercontrol.ComplainSysNo = item.ComplainSysNo.HasValue ? item.ComplainSysNo.Value : 0;
            usercontrol.Dialog = Window.ShowDialog(ResComment.Title_ServiceReply, usercontrol, (obj, args) =>
            {
                QueryResultGrid.Bind();
            });
        }

        /// <summary>
        /// 邮件回复
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void hlEmailReply_Click(object sender, RoutedEventArgs e)
        {
            ProductReviewQueryVM item = this.QueryResultGrid.SelectedItem as ProductReviewQueryVM;
            UCEditProductReview usercontrol = new UCEditProductReview();
            usercontrol.SysNo = item.SysNo.Value;
            usercontrol.ReplyMode = 2;
            usercontrol.ComplainSysNo = item.ComplainSysNo.HasValue ? item.ComplainSysNo.Value : 0;
            usercontrol.Dialog = Window.ShowDialog(ResComment.Title_EmailReply, usercontrol, (obj, args) =>
            {
                QueryResultGrid.Bind();
            });
        }
        
        /// <summary>
        /// 厂商回复
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void hlVendorReply_Click(object sender, RoutedEventArgs e)
        {
            ProductReviewQueryVM item = this.QueryResultGrid.SelectedItem as ProductReviewQueryVM;
            UCEditProductReview usercontrol = new UCEditProductReview();
            usercontrol.SysNo = item.SysNo.Value;
            usercontrol.ReplyMode = 3;
            usercontrol.ComplainSysNo = item.ComplainSysNo.HasValue ? item.ComplainSysNo.Value : 0;
            usercontrol.Dialog = Window.ShowDialog(ResComment.Title_VendorReply, usercontrol, (obj, args) =>
            {
                QueryResultGrid.Bind();
            });
        }

        private void Button_Search_Click(object sender, RoutedEventArgs e)
        {
            if (ValidationManager.Validate(this.QuerySection))
            {
                filter = model.ConvertVM<ProductReviewQueryVM, ProductReviewQueryFilter>();
                filterVM = Newegg.Oversea.Silverlight.Utilities.UtilityHelper.DeepClone<ProductReviewQueryFilter>(filter);
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
        /// <summary>
        /// 数据全部导出
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void QueryResultGrid_ExportAllClick(object sender, EventArgs e)
        {
            if (filterVM == null || this.QueryResultGrid.TotalCount < 1)
            {
                Window.Alert(ResComment.Msg_ExportError);
                return;
            }

            if (this.QueryResultGrid.TotalCount > 10000)
            {
                Window.Alert(ResComment.Msg_ExportExceedsLimitCount);
                return;
            }

            ColumnSet col = new ColumnSet();

            col.Add("Title", "评论标题");
            col.Add("Prons", "优点");
            col.Add("Cons", "缺点");
            col.Add("Service", "其他");
            col.Add("Score", "总评分数");
            col.Add("ProductID", "商品ID");
            col.Add("InUser", "创建人");
            col.Add("InDate", "创建时间");
            col.Add("EditUser", "更新人");
            col.Add("EditDate", "更新时间");

            //filter = model.ConvertVM<ProductReviewQueryVM, ProductReviewQueryFilter>();
            filter.PageInfo = new ECCentral.QueryFilter.Common.PagingInfo()
            {
                PageSize = ConstValue.MaxRowCountLimit,
                PageIndex = 0,
                SortBy = string.Empty
            };
            facade.ExportExcelFile(filter, new ColumnSet[] { col });
        }

        #endregion
        /// <summary>
        /// 新建
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCreate_Click(object sender, RoutedEventArgs e)
        {

            UCProductReview usercontrol = new UCProductReview();            
            usercontrol.Dialog = Window.ShowDialog("创建评论", usercontrol, (obj, args) =>
            {
                if (args.Data != null && args.DialogResult == DialogResultType.OK)
                {
                    Window.Alert("评论创建成功");                 
                }               
            });
        }

        #region reply


        private void ProductReviewReplyList_LoadingDataSource(object sender, Newegg.Oversea.Silverlight.Controls.Data.LoadingDataEventArgs e)
        {
            
            facade.QueryProductReviewReply(ProductReviewReplyList.QueryCriteria as ProductReviewReplyQueryFilter, e.PageSize, e.PageIndex, e.SortField, (s, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }
                gridVMReply = DynamicConverter<ProductReviewReplyQueryVM>.ConvertToVMList<List<ProductReviewReplyQueryVM>>(args.Result.Rows);
                ProductReviewReplyList.ItemsSource = gridVMReply;
                ProductReviewReplyList.TotalCount = args.Result.TotalCount;

                //foreach (ProductReviewReplyQueryVM v in gridVMReply)
                //{
                //    if (v.TypeCategory == "M") 
                //        CanCheckAll = false;
                //}
                    
                if (gridVMReply != null && (comReplyStatus.SelectedValue == null || comReplyStatus.SelectedValue.ToString() != "M"))
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
                facade.BatchSetProductReviewReplyInvalid(invalidSysNo, (obj, args) =>
                {
                    if (args.FaultsHandle())
                        return;

                    ProductReviewReplyList.Bind();
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
                facade.BatchSetProductReviewReplyValid(invalidSysNo, (obj, args) =>
                {
                    if (args.FaultsHandle())
                        return;

                    ProductReviewReplyList.Bind();
                });
            else
                Window.Alert(ResComment.Information_MoreThanOneRecord, MessageType.Error);
        }

        /// <summary>
        /// 数据全部导出
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ProductReviewReplyList_ExportAllClick(object sender, EventArgs e)
        {
            if (filterReplyVM == null || this.ProductReviewReplyList.TotalCount < 1)
            {
                Window.Alert(ResComment.Information_ExportFailed);
                return;
            }
            ColumnSet col = new ColumnSet();
            col.Add("Title", "讨论标题");
            col.Add("ProductID", "商品ID");
            col.Add("Content", "回复内容");
            col.Add("InUser", "创建人");
            col.Add("InDate", "创建时间");
            col.Add("EditUser", "更新人");
            col.Add("EditDate", "更新时间");


            filterReply = modelReply.ConvertVM<ProductReviewReplyQueryVM, ProductReviewReplyQueryFilter>();
            filterReply.PageInfo = new ECCentral.QueryFilter.Common.PagingInfo()
            {
                PageSize = ConstValue.MaxRowCountLimit,
                PageIndex = 0,
                SortBy = string.Empty
            };
            facade.ExportReplyExcelFile(filterReply, new ColumnSet[] { col });
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
                facade.BatchSetProductReviewReplyRead(invalidSysNo, (obj, args) =>
                {
                    if (args.FaultsHandle())
                        return;

                    ProductReviewReplyList.Bind();
                });
            else
                Window.Alert(ResComment.Information_MoreThanOneRecord, MessageType.Error);
        }

        /// <summary>
        /// 编辑用户评论
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void hlEditReply_Click(object sender, RoutedEventArgs e)
        {
            ProductReviewReplyQueryVM item = this.ProductReviewReplyList.SelectedItem as ProductReviewReplyQueryVM;
            UCEditProductReview usercontrol = new UCEditProductReview();
            usercontrol.SysNo = item.ReviewSysNo.Value;
            usercontrol.ReplyMode = 4;
            usercontrol.ComplainSysNo = item.ComplainSysNo.HasValue?item.ComplainSysNo.Value:0;
            usercontrol.Dialog = Window.ShowDialog(ResComment.Title_EditProductReview, usercontrol, (obj, args) =>
            {
                ProductReviewReplyList.Bind();
            });
        }

        private void Button_Search2_Click(object sender, RoutedEventArgs e)
        {
            if (ValidationManager.Validate(this.QuerySection2))
            {
                filterReply = modelReply.ConvertVM<ProductReviewReplyQueryVM, ProductReviewReplyQueryFilter>();
                filterReplyVM = Newegg.Oversea.Silverlight.Utilities.UtilityHelper.DeepClone<ProductReviewReplyQueryFilter>(filterReply);
                ProductReviewReplyList.QueryCriteria = this.filterReply;
                ProductReviewReplyList.Bind();
            }
        }

        private void ckbSelectRow2_Click(object sender, RoutedEventArgs e)
        {
            //if (!CanCheckAll)
            //{
            //    ((CheckBox)sender).IsChecked = false;
            //    return;
            //}
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
