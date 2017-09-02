using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using ECCentral.BizEntity.MKT;
using ECCentral.Portal.Basic;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.Portal.UI.MKT.Facades;
using ECCentral.Portal.UI.MKT.Models;
using ECCentral.Portal.UI.MKT.Resources;
using ECCentral.Portal.UI.MKT.UserControls.Keywords;
using ECCentral.QueryFilter.MKT;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using Newegg.Oversea.Silverlight.Controls;
using Newegg.Oversea.Silverlight.Controls.Components;
using Newegg.Oversea.Silverlight.Utilities.Validation;

namespace ECCentral.Portal.UI.MKT.Views
{
    [View(IsSingleton = true, SingletonType = SingletonTypes.Url)]
    public partial class SegmentMaintain : PageBase
    {
        private SegmentQueryFacade facade;
        private SegmentQueryVM model;
        private SegmentQueryFilter filter;
        private List<SegmentQueryVM> gridVM;
        private SegmentQueryFilter filterVM;

        public SegmentMaintain()
        {
            InitializeComponent();
        }

        public override void OnPageLoad(object sender, EventArgs e)
        {
            facade = new SegmentQueryFacade(this);
            model = new SegmentQueryVM();
            filter = new SegmentQueryFilter();
            cbKeywordStatus.ItemsSource = new List<KeyValuePair<KeywordsStatus?, string>>() 
            {
                new KeyValuePair<KeywordsStatus?, string>(null,"--所有--"),
                new KeyValuePair<KeywordsStatus?, string>(KeywordsStatus.Waiting,"待审核"),
                new KeyValuePair<KeywordsStatus?, string>(KeywordsStatus.Passed,"审核通过"),
                new KeyValuePair<KeywordsStatus?, string>(KeywordsStatus.Reject,"审核拒绝"),
             };
            model.ChannelID = "1";
            model.CompanyCode = Newegg.Oversea.Silverlight.ControlPanel.Core.CPApplication.Current.CompanyCode;
            QuerySection.DataContext = model;
            btnStackPanel.DataContext = model;
            base.OnPageLoad(sender, e);
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
                Window.Alert(ResKeywords.Information_ExportFailed);
                return;
            }
            ColumnSet col = new ColumnSet(this.QueryResultGrid);
            filter = model.ConvertVM<SegmentQueryVM, SegmentQueryFilter>();
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
            facade.QuerySegment(QueryResultGrid.QueryCriteria as SegmentQueryFilter, e.PageSize, e.PageIndex, e.SortField, (s, args) =>
            {
                if (args.FaultsHandle())
                    return;

                gridVM = DynamicConverter<SegmentQueryVM>.ConvertToVMList<List<SegmentQueryVM>>(args.Result.Rows);
                QueryResultGrid.ItemsSource = gridVM;
                QueryResultGrid.TotalCount = args.Result.TotalCount;
                if (gridVM != null)
                {
                    btnBatchDelete.Visibility = Visibility.Visible;
                    btnBatchVerify.Visibility = Visibility.Visible;
                    btnBatchReject.Visibility = Visibility.Visible;
                }
            });
        }

        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnAddItem_Click(object sender, RoutedEventArgs e)
        {
            UCAddSegment usercontrol = new UCAddSegment();
            usercontrol.Dialog = Window.ShowDialog(ResKeywords.Title_CreateSegment, usercontrol, (obj, args) =>
            {
                filter = model.ConvertVM<SegmentQueryVM, SegmentQueryFilter>();

                filterVM = Newegg.Oversea.Silverlight.Utilities.UtilityHelper.DeepClone<SegmentQueryFilter>(filter);
                QueryResultGrid.QueryCriteria = this.filter;
                QueryResultGrid.Bind();
            });
        }

         /// <summary>
         /// 编辑
         /// </summary>
         /// <param name="sender"></param>
         /// <param name="e"></param>
        private void hlEdit_Click(object sender, RoutedEventArgs e)
        {
            SegmentQueryVM item = this.QueryResultGrid.SelectedItem as SegmentQueryVM;
            if (item != null)
            {
                if (item.Status != KeywordsStatus.Waiting)
                {
                    UCAddSegment usercontrol = new UCAddSegment();
                    usercontrol.SysNo = item.SysNo;
                    usercontrol.Dialog = Window.ShowDialog(ResKeywords.Title_EditSegment, usercontrol, (obj, args) =>
                    {
                        QueryResultGrid.Bind();
                    });
                }
                else
                    Window.Alert(ResKeywords.Information_KeywordsDonontPassVerify, MessageType.Warning);
            }
            else
                Window.Alert(ResKeywords.Information_MoreThanOneRecord, MessageType.Warning);
        }

        ///// <summary>
        ///// 编辑该行
        ///// </summary>
        ///// <param name="sender"></param>
        ///// <param name="e"></param>
        //private void Hyperlink_Edit_Click(object sender, RoutedEventArgs e)
        //{
        //    dynamic item = this.QueryResultGrid.SelectedItem as dynamic;
        //    if (item != null)
        //    {
        //        //Window.Navigate(string.Format("/ECCentral.Portal.UI.MKT/UCAddAdvertisers/{0}", adv.SysNo), null, true);
        //        UCAddSegment usercontrol = new UCAddSegment();
        //        usercontrol.SysNo = item.SysNo;
        //        IDialog dialog = Window.ShowDialog("编辑关键词", usercontrol, (obj, args) =>
        //        {
        //            QueryResultGrid.Bind();
        //        });
        //    }
        //    else
        //    {
        //        Window.Alert("请选择一行数据进行编辑！", MessageType.Error);
        //    }
        //}

        /// <summary>
        /// 批量删除
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnBatchDelete_Click(object sender, RoutedEventArgs e)
        {
            Window.MessageBox.Show("注意:批量删除时,不能对待审核的记录做操作，如勾选了待审核记录，系统会自动忽略!", MessageBoxType.Information);
            List<int> sysNoList = new List<int>();
            int tips = 0;
            gridVM.ForEach(item =>
            {
                if (item.IsChecked == true && item.Status!=KeywordsStatus.Waiting)
                    sysNoList.Add(item.SysNo);
                if (item.IsChecked == true && item.Status == KeywordsStatus.Waiting)
                    tips++;
            });
            if (sysNoList.Count > 0)
            {
                Window.Confirm("确认删除?",(objs,arg)=>
                {
                    if (arg.DialogResult == DialogResultType.OK)
                    {
                        facade.DeleteSegmentInfos(sysNoList, (obj, args) =>
                        {
                            if (args.FaultsHandle())
                                return;
                            Window.Alert(string.Format("删除操作，成功{0}条，失败{1}条;", sysNoList.Count.ToString(), tips.ToString()), MessageType.Information);
                            QueryResultGrid.Bind();
                            this.cbotemp.IsChecked = false;
                        });
                    }
                });
              
            }else
            {
                Window.Alert(ResKeywords.Information_MoreThanOneRecord, MessageType.Warning);
            }
        }

        /// <summary>
        /// 批量审核通过
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnBatchVerify_Click(object sender, RoutedEventArgs e)
        {
            Window.MessageBox.Show("注意:批量审核通过时,只对待审核的记录做操作，如勾选了其它状态记录，系统会自动忽略!", MessageBoxType.Information);
            List<SegmentInfo> sysNoList = new List<SegmentInfo>();
            gridVM.ForEach(item =>
            {
                if (item.IsChecked&&item.Status == KeywordsStatus.Waiting)
                    sysNoList.Add(new SegmentInfo() {
                        SysNo = item.SysNo,
                        InUser = item.InUser
                        
                    });
            });
            if (sysNoList.Count > 0)
                facade.SetSegmentInfosValid(sysNoList, (obj, args) =>
                {
                    if (args.FaultsHandle())
                    {
                        QueryResultGrid.Bind();
                        this.cbotemp.IsChecked = false;
                    }
                });
            else
                Window.Alert(ResKeywords.Information_MoreThanOneRecord, MessageType.Warning);
        }

        /// <summary>
        /// 批量审核拒绝
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnBatchReject_Click(object sender, RoutedEventArgs e)
        {
            Window.MessageBox.Show("注意:批量审核拒绝时,只对待审核的记录做操作，如勾选了其它状态记录，系统会自动忽略!", MessageBoxType.Information);
            List<SegmentInfo> sysNoList = new List<SegmentInfo>();
            gridVM.ForEach(item =>
            {
                if (item.IsChecked&&item.Status == KeywordsStatus.Waiting)
                    sysNoList.Add(new SegmentInfo()
                    {
                        SysNo = item.SysNo,
                        InUser = item.InUser

                    });
            });
            if (sysNoList.Count > 0)
                facade.SetSegmentInfosInvalid(sysNoList, (obj, args) =>
                {
                    if (args.FaultsHandle())
                    {
                        QueryResultGrid.Bind();
                        this.cbotemp.IsChecked = false;
                    }
                });
            else
                Window.Alert(ResKeywords.Information_MoreThanOneRecord, MessageType.Warning);
        }

        /// <summary>
        /// 批量导入
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnBatchExport_Click(object sender, RoutedEventArgs e)
        {
            UCUploadSegment usercontrol = new UCUploadSegment();
            usercontrol.Dialog = Window.ShowDialog(ResKeywords.Title_BatchUploadSegment, usercontrol, (obj, args) =>
            {
                QueryResultGrid.Bind();
            });
            //只支持txt文件格式

            //var lines = new HashSet<string>();
            //while (!reader.EndOfStream)
            //{
            //    var value = reader.ReadLine().Trim().Replace("\t", " ");
            //    if (!string.IsNullOrEmpty(value))
            //    {
            //        lines.Add(value);
            //    }
            //}

            //if (!lines.Any())
            //{
            //    ShowMessage("导入的txt文件没有任何内容");
            //    return;
            //}

            //if (lines.Count > 2000)
            //{

            //    ShowMessage("导入的关键字不能超过2000个");
            //    return;
            //}

            //var result = SegmentService.ImportKeywords(lines,
            //    CPContext.Current.LoginUser.CompanyCode,
            //    CPContext.Current.LoginUser.LoginName,
            //    CPContext.Current.LanguageCode,
            //    CPContext.Current.StoreCompanyCode);

            //this.ShowMessage(string.Format("成功导入关键字{0}个，重复关键字{1}个，错误关键字{2}个",
            //    result.Body.ImportSuccessfullyCount,
            //    result.Body.HasSameKeywordsCount,
            //    result.Body.ImportFailedCount));
        }

        private void Button_Search_Click(object sender, RoutedEventArgs e)
        {
            if (ValidationManager.Validate(this.QuerySection))
            {
                filter = model.ConvertVM<SegmentQueryVM, SegmentQueryFilter>();
            
                filterVM = Newegg.Oversea.Silverlight.Utilities.UtilityHelper.DeepClone<SegmentQueryFilter>(filter);
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
        /// <summary>
        /// 单个删除
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void hlDelete_Click(object sender, RoutedEventArgs e)
        {
            SegmentQueryVM vm = this.QueryResultGrid.SelectedItem as SegmentQueryVM;
            Window.Confirm("确认删除?", (objs, args) =>
                {
                    if (args.DialogResult == DialogResultType.OK)
                    {
                        facade.DeleteSegmentInfos(new List<int>() { vm.SysNo }, (obj, arg) =>
                        {
                            if (arg.FaultsHandle())
                            {
                                return;
                            }
                            Window.Alert("删除成功!");
                            this.QueryResultGrid.Bind();
                        });
                    }
                });
        }

        /// <summary>
        /// 单个审核通过
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void hlVerify_Click(object sender, RoutedEventArgs e)
        {
            SegmentQueryVM vm = this.QueryResultGrid.SelectedItem as SegmentQueryVM;
            facade.SetSegmentInfosValid(new List<SegmentInfo>() { 
                new SegmentInfo()
                {
                    SysNo = vm.SysNo,
                    InUser =vm.InUser 
                    
                }
        }, (obj, arg) =>
            {
                if (arg.FaultsHandle())
                {
                    this.QueryResultGrid.Bind();
                }
            });
        }
        /// <summary>
        /// 单个审核拒绝
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void hlReject_Click(object sender, RoutedEventArgs e)
        {
            SegmentQueryVM vm = this.QueryResultGrid.SelectedItem as SegmentQueryVM;
            facade.SetSegmentInfosInvalid(
                new List<SegmentInfo>() { 
                    new SegmentInfo()
                    {
                        SysNo = vm.SysNo,
                        InUser =vm.InUser 
                    }
                }, (obj, arg) => 
                {
                    if (arg.FaultsHandle())
                    {
                        this.QueryResultGrid.Bind();
                    }
            });
        }
    }

   
}
