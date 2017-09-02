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
using ECCentral.BizEntity.MKT;
using ECCentral.Portal.UI.MKT.Models;
using ECCentral.Portal.UI.MKT.Facades;
using ECCentral.QueryFilter.MKT;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using Newegg.Oversea.Silverlight.Controls.Components;
using ECCentral.Portal.UI.MKT.Resources;
using ECCentral.Portal.Basic;
using Newegg.Oversea.Silverlight.Utilities.Validation;

namespace ECCentral.Portal.UI.MKT.Views
{
    [View(IsSingleton = true, SingletonType = SingletonTypes.Url)]
    public partial class CommentModeMaintain : PageBase
    {
        private RemarkModeQueryVM model;
        private RemarkModeQueryVM modelComment;
        private RemarkModeQueryVM modelConsult;
        private RemarkModeQueryVM modelDiscuss;
        private List<RemarkModeQueryVM> gridCommentVM;
        private List<RemarkModeQueryVM> gridConsultVM;
        private List<RemarkModeQueryVM> gridDiscussVM;
        private RemarkModeQueryFacade facade;

        private RemarkModeQueryFilter filter;
        private RemarkModeQueryFilter filterCommentVM;
        private RemarkModeQueryFilter filterConsultVM;
        private RemarkModeQueryFilter filterDiscussVM;

        public CommentModeMaintain()
        {
            InitializeComponent();
        }

        public override void OnPageLoad(object sender, EventArgs e)
        {
            model = new RemarkModeQueryVM();
            modelComment = new RemarkModeQueryVM();
            modelConsult = new RemarkModeQueryVM();
            modelDiscuss = new RemarkModeQueryVM();
            filter = new RemarkModeQueryFilter();
            model.CompanyCode = Newegg.Oversea.Silverlight.ControlPanel.Core.CPApplication.Current.CompanyCode;
            modelComment.CompanyCode = Newegg.Oversea.Silverlight.ControlPanel.Core.CPApplication.Current.CompanyCode;
            modelConsult.CompanyCode = Newegg.Oversea.Silverlight.ControlPanel.Core.CPApplication.Current.CompanyCode;
            modelDiscuss.CompanyCode = Newegg.Oversea.Silverlight.ControlPanel.Core.CPApplication.Current.CompanyCode;
            facade = new RemarkModeQueryFacade(this);

            commentModeSection.DataContext = modelComment;
            consultModeSection.DataContext = modelConsult;
            discussModeSection.DataContext = modelDiscuss; 

            facade.LoadRemarkMode(CPApplication.Current.CompanyCode, (obj, args) =>
            {
                if (args.FaultsHandle())
                    return;

                model = args.Result.Convert<RemarkMode, RemarkModeQueryVM>();
                remarkModeSection.DataContext = model;
            });
            base.OnPageLoad(sender, e);
        }

        #region 公告及促销评论管理
        
        /// <summary>
        /// 更新公告及促销评论
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnUpdatePromotionComment_Click(object sender, RoutedEventArgs e)
        {
            //RemarkModeQueryVM vm = remarkModeSection.DataContext as RemarkModeQueryVM;
            RemarkMode item = model.ConvertVM<RemarkModeQueryVM, RemarkMode>();
            item.RemarkType = RemarksType.Promotion;
            item.CompanyCode = Newegg.Oversea.Silverlight.ControlPanel.Core.CPApplication.Current.CompanyCode;
            facade.UpdateRemarkMode(item, (obj, args) =>
            {
                if (args.FaultsHandle())
                    return;

                CPApplication.Current.CurrentPage.Context.Window.Alert(ResComment.Information_UpdateSuccessful,MessageType.Information);
            });
        }
        #endregion

        #region 评论管理
        /// <summary>
        /// 查询
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnQueryCommentMode_Click(object sender, RoutedEventArgs e)
        {
            if (ValidationManager.Validate(this.QueryResultGrid1))
            {
                filter = modelComment.ConvertVM<RemarkModeQueryVM, RemarkModeQueryFilter>();
                filter.RemarkType = RemarksType.Comment;
            
                filterCommentVM = Newegg.Oversea.Silverlight.Utilities.UtilityHelper.DeepClone<RemarkModeQueryFilter>(filter);
                QueryResultGrid1.QueryCriteria = this.filter;
                QueryResultGrid1.Bind();
            }
        }

        /// <summary>
        /// 保存评论管理模式
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSaveCutOverMode_Click(object sender, RoutedEventArgs e)
        {
            if (radComment.IsChecked.Value)
            {
                RemarkMode item = modelComment.ConvertVM<RemarkModeQueryVM, RemarkMode>();
                item.RemarkType = RemarksType.Comment;
                item.CompanyCode = Newegg.Oversea.Silverlight.ControlPanel.Core.CPApplication.Current.CompanyCode;
                facade.UpdateRemarkMode(item, (obj, args) =>
                {
                    if (args.FaultsHandle())
                        return;
                    
                    CPApplication.Current.CurrentPage.Context.Window.Alert(ResComment.Information_UpdateSuccessful, MessageType.Information);
                });
            }
            else
                CPApplication.Current.CurrentPage.Context.Window.Alert(ResComment.Information_CannotSaveForChangeMode, MessageType.Warning);
        }

        /// <summary>
        /// 批量更新
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSaveCommentResult_Click(object sender, RoutedEventArgs e)
        {
            List<RemarkMode> updateModes = new List<RemarkMode>();

            if (gridCommentVM == null)
            {
                Window.Alert(ResComment.Information_MoreThanOneRecord, MessageType.Error);
                return;
            }

            gridCommentVM.ForEach(item =>
            {
                if (item.IsChecked == true)
                    updateModes.Add(item.ConvertVM<RemarkModeQueryVM, RemarkMode>());
            });
            if (updateModes.Count > 0)
                facade.UpdateOtherRemarkMode(updateModes, (obj, args) =>
                {
                    if (args.FaultsHandle())
                        return;

                    QueryResultGrid1.Bind();
                });
            else
                Window.Alert(ResComment.Information_MoreThanOneRecord, MessageType.Error);
        }

        /// <summary>
        /// 全选
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ckbSelectRow1_Click(object sender, RoutedEventArgs e)
        {
            var checkBoxAll = sender as CheckBox;
            if (gridCommentVM == null || checkBoxAll == null)
                return;
            gridCommentVM.ForEach(item =>
            {
                item.IsChecked = checkBoxAll.IsChecked ?? false;
            });
        }

        private void QueryResultGrid1_LoadingDataSource(object sender, Newegg.Oversea.Silverlight.Controls.Data.LoadingDataEventArgs e)
        {
            facade.QueryRemarkModeList(QueryResultGrid1.QueryCriteria as RemarkModeQueryFilter, e.PageSize, e.PageIndex, e.SortField, (obj, args) =>
            {
                if (args.FaultsHandle())
                    return;

                gridCommentVM = DynamicConverter<RemarkModeQueryVM>.ConvertToVMList<List<RemarkModeQueryVM>>(args.Result.Rows);
                QueryResultGrid1.ItemsSource = gridCommentVM;
                QueryResultGrid1.TotalCount = args.Result.TotalCount;
            });
        }

        #endregion

        #region 购物咨询
        /// <summary>
        /// 查询
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSearchConsultMode_Click(object sender, RoutedEventArgs e)
        {
            if (ValidationManager.Validate(this.QueryResultGrid2))
            {
                filter = modelConsult.ConvertVM<RemarkModeQueryVM, RemarkModeQueryFilter>();
                filter.RemarkType = RemarksType.Consult;
                filterConsultVM = Newegg.Oversea.Silverlight.Utilities.UtilityHelper.DeepClone<RemarkModeQueryFilter>(filter);
                QueryResultGrid2.QueryCriteria = this.filter;
                QueryResultGrid2.Bind();
            }
        }

        /// <summary>
        /// 保存购物咨询模式
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSaveConsultMode_Click(object sender, RoutedEventArgs e)
        {
            if (radConsult.IsChecked.Value)
            {
                RemarkMode item = modelConsult.ConvertVM<RemarkModeQueryVM, RemarkMode>();
                item.RemarkType = RemarksType.Consult;
                item.CompanyCode = Newegg.Oversea.Silverlight.ControlPanel.Core.CPApplication.Current.CompanyCode;
                facade.UpdateRemarkMode(item, (obj, args) =>
                {
                    if (args.FaultsHandle())
                        return;
                    CPApplication.Current.CurrentPage.Context.Window.Alert(ResComment.Information_UpdateSuccessful, MessageType.Information);
                });
            }
            else
                CPApplication.Current.CurrentPage.Context.Window.Alert(ResComment.Information_CannotSaveForChangeMode, MessageType.Warning);
        }

        private void btnSaveConsultResult_Click(object sender, RoutedEventArgs e)
        {
            List<RemarkMode> updateModes = new List<RemarkMode>();
            if (gridConsultVM == null)
            {
                Window.Alert(ResComment.Information_MoreThanOneRecord, MessageType.Error);
            }
            else
            {
                gridConsultVM.ForEach(item =>
                {
                    if (item.IsChecked == true)
                        updateModes.Add(item.ConvertVM<RemarkModeQueryVM, RemarkMode>());
                });
                if (updateModes.Count > 0)
                    facade.UpdateOtherRemarkMode(updateModes, (obj, args) =>
                    {
                        if (args.FaultsHandle())
                            return;

                        QueryResultGrid2.Bind();
                    });
                else
                    Window.Alert(ResComment.Information_MoreThanOneRecord, MessageType.Error);
            }
        }

        /// <summary>
        /// 全选
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ckbSelectRow2_Click(object sender, RoutedEventArgs e)
        {
            var checkBoxAll = sender as CheckBox;
            if (gridConsultVM == null || checkBoxAll == null)
                return;
            gridConsultVM.ForEach(item =>
            {
                item.IsChecked = checkBoxAll.IsChecked ?? false;
            });
        }

        private void QueryResultGrid2_LoadingDataSource(object sender, Newegg.Oversea.Silverlight.Controls.Data.LoadingDataEventArgs e)
        {
           
            facade.QueryRemarkModeList(QueryResultGrid2.QueryCriteria as RemarkModeQueryFilter, e.PageSize, e.PageIndex, e.SortField,(obj, args) =>
            {
                if (args.FaultsHandle())
                    return;

                gridConsultVM = DynamicConverter<RemarkModeQueryVM>.ConvertToVMList<List<RemarkModeQueryVM>>(args.Result.Rows);
                QueryResultGrid2.ItemsSource = gridConsultVM;
                QueryResultGrid2.TotalCount = args.Result.TotalCount;
            });
        }
        #endregion

        #region 网友讨论
        /// <summary>
        /// 查询
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSearchDiscussMode_Click(object sender, RoutedEventArgs e)
        {
            if (ValidationManager.Validate(this.QueryResultGrid3))
            {
                filter = modelDiscuss.ConvertVM<RemarkModeQueryVM, RemarkModeQueryFilter>();
                filter.RemarkType = RemarksType.Discuss;
                filterDiscussVM = Newegg.Oversea.Silverlight.Utilities.UtilityHelper.DeepClone<RemarkModeQueryFilter>(filter);
                QueryResultGrid3.QueryCriteria = this.filter;
                QueryResultGrid3.Bind();
            }
        }

        /// <summary>
        /// 保存
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSaveDiscussMode_Click(object sender, RoutedEventArgs e)
        {
            if (radDiscuss.IsChecked.Value)
            {
                RemarkMode item = modelDiscuss.ConvertVM<RemarkModeQueryVM, RemarkMode>();
                item.RemarkType = RemarksType.Discuss;
                item.CompanyCode = Newegg.Oversea.Silverlight.ControlPanel.Core.CPApplication.Current.CompanyCode;
                facade.UpdateRemarkMode(item, (obj, args) =>
                {
                    if (args.FaultsHandle())
                        return;
                    CPApplication.Current.CurrentPage.Context.Window.Alert(ResComment.Information_UpdateSuccessful, MessageType.Information);
                });
            }
            else
                CPApplication.Current.CurrentPage.Context.Window.Alert(ResComment.Information_CannotSaveForChangeMode,MessageType.Warning);
        }

        /// <summary>
        /// 批量更新
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSaveDiscussResult_Click(object sender, RoutedEventArgs e)
        {
            List<RemarkMode> updateModes = new List<RemarkMode>();
            if (gridDiscussVM == null)
            {
                Window.Alert(ResComment.Information_MoreThanOneRecord, MessageType.Error);
            }
            else
            {
                gridDiscussVM.ForEach(item =>
                {
                    if (item.IsChecked == true)
                        updateModes.Add(item.ConvertVM<RemarkModeQueryVM, RemarkMode>());
                });
                if (updateModes.Count > 0)
                    facade.UpdateOtherRemarkMode(updateModes, (obj, args) =>
                    {
                        if (args.FaultsHandle())
                            return;

                        QueryResultGrid3.Bind();
                    });
                else
                    Window.Alert(ResComment.Information_MoreThanOneRecord, MessageType.Error);
            }
        }

        /// <summary>
        /// 全选
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ckbSelectRow3_Click(object sender, RoutedEventArgs e)
        {
            var checkBoxAll = sender as CheckBox;
            if (gridDiscussVM == null || checkBoxAll == null)
                return;
            gridDiscussVM.ForEach(item =>
            {
                item.IsChecked = checkBoxAll.IsChecked ?? false;
            });
        }

        private void QueryResultGrid3_LoadingDataSource(object sender, Newegg.Oversea.Silverlight.Controls.Data.LoadingDataEventArgs e)
        {
            
            facade.QueryRemarkModeList(QueryResultGrid3.QueryCriteria as RemarkModeQueryFilter, e.PageSize, e.PageIndex, e.SortField, (obj, args) =>
            {
                if (args.FaultsHandle())
                    return;

                gridDiscussVM = DynamicConverter<RemarkModeQueryVM>.ConvertToVMList<List<RemarkModeQueryVM>>(args.Result.Rows);
                QueryResultGrid3.ItemsSource = gridDiscussVM;
                QueryResultGrid3.TotalCount = args.Result.TotalCount;
            });
        }

        #endregion


        /// <summary>
        /// 数据全部导出
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void QueryResultGrid1_ExportAllClick(object sender, EventArgs e)
        {
            if (filterCommentVM == null || this.QueryResultGrid1.TotalCount < 1)
            {
                Window.Alert(ResComment.Information_ExportFailed);
                return;
            }
            ColumnSet col = new ColumnSet(this.QueryResultGrid1);
            filter = modelComment.ConvertVM<RemarkModeQueryVM, RemarkModeQueryFilter>();
            filter.PageInfo = new ECCentral.QueryFilter.Common.PagingInfo()
            {
                PageSize = ConstValue.MaxRowCountLimit,
                PageIndex = 0,
                SortBy = string.Empty
            };
            facade.ExportRemarkModeExcelFile(filterCommentVM, new ColumnSet[] { col });
        }

        /// <summary>
        /// 数据全部导出
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void QueryResultGrid2_ExportAllClick(object sender, EventArgs e)
        {
            if (filterConsultVM == null || this.QueryResultGrid2.TotalCount < 1)
            {
                Window.Alert(ResComment.Information_ExportFailed);
                return;
            }
            ColumnSet col = new ColumnSet(this.QueryResultGrid2);
            filter = modelConsult.ConvertVM<RemarkModeQueryVM, RemarkModeQueryFilter>();
            filter.PageInfo = new ECCentral.QueryFilter.Common.PagingInfo()
            {
                PageSize = ConstValue.MaxRowCountLimit,
                PageIndex = 0,
                SortBy = string.Empty
            };
            facade.ExportRemarkModeExcelFile(filterConsultVM, new ColumnSet[] { col });
        }

        /// <summary>
        /// 数据全部导出
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void QueryResultGrid3_ExportAllClick(object sender, EventArgs e)
        {
            if (filterDiscussVM == null || this.QueryResultGrid3.TotalCount < 1)
            {
                Window.Alert(ResComment.Information_ExportFailed);
                return;
            }
            ColumnSet col = new ColumnSet(this.QueryResultGrid3);
            filter = modelDiscuss.ConvertVM<RemarkModeQueryVM, RemarkModeQueryFilter>();
            filter.PageInfo = new ECCentral.QueryFilter.Common.PagingInfo()
            {
                PageSize = ConstValue.MaxRowCountLimit,
                PageIndex = 0,
                SortBy = string.Empty
            };
            facade.ExportRemarkModeExcelFile(filterDiscussVM, new ColumnSet[] { col });
        }
    }

}
