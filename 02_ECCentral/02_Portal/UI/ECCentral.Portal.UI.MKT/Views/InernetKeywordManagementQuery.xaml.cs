using System;
using System.Windows;
using ECCentral.BizEntity.Common;
using ECCentral.BizEntity.MKT;
using ECCentral.BizEntity.MKT.Keyword;
using ECCentral.Portal.UI.IM.Facades;
using ECCentral.Portal.UI.MKT.Facades;
using ECCentral.Portal.UI.MKT.Models;
using ECCentral.Portal.UI.MKT.UserControls;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using Newegg.Oversea.Silverlight.Controls;
using Newegg.Oversea.Silverlight.Controls.Data;
using System.Collections.Generic;
using System.Windows.Controls;
using Newegg.Oversea.Silverlight.Utilities.Validation;
using Newegg.Oversea.Silverlight.Controls.Components;

namespace ECCentral.Portal.UI.MKT.Views
{
    [View(IsSingleton = true, SingletonType = SingletonTypes.Url)]
    public partial class InernetKeywordManagementQuery : PageBase
    {
        #region 属性
        InternetKeywordQueryVM _model;
        private InernetKeywordQueryFacade _facade;
        #endregion

        #region 初始化加载

        public InernetKeywordManagementQuery()
        {
            InitializeComponent();
            Loaded += OpenAPIQuery_Loaded;
        }

        void OpenAPIQuery_Loaded(object sender, RoutedEventArgs e)
        {
            _model = new InternetKeywordQueryVM();
            DataContext = _model;
            _facade = new InernetKeywordQueryFacade(this);
        }

     

        #endregion

        #region 查询绑定

        /// <summary>
        /// 查询
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidationManager.Validate(this))
             {
               return;
            }
            if (_model.BeginDate != null && _model.EndDate != null)
            {
                if (_model.BeginDate.Value.CompareTo(_model.EndDate) == 0)
                {
                }
            }

            dgInernetKeywordQueryResult.Bind();
        }

        /// <summary>
        /// 绑定
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgInernetKeywordQueryResult_LoadingDataSource(object sender, LoadingDataEventArgs e)
        {
            _facade.QueryKeyword(_model, e.PageSize, e.PageIndex, e.SortField, (obj, args) =>
            {
                if(args.FaultsHandle()) return;
                dgInernetKeywordQueryResult.ItemsSource = args.Result.Rows.ToList("IsChecked", false);
                dgInernetKeywordQueryResult.TotalCount = args.Result.TotalCount;
            });
        }
        #endregion

        #region 页面内按钮处理事件

        #region 界面事件
        private void ckbSelectAllRow_Click(object sender, RoutedEventArgs e)
        {
            var cb = sender as CheckBox;
            if (cb != null)
            {
                var viewlist = dgInernetKeywordQueryResult.ItemsSource as dynamic;
                if (viewlist != null)
                {
                    foreach (var item in viewlist)
                    {
                        item.IsChecked = cb.IsChecked != null && cb.IsChecked.Value;
                    }
                }
            }
        }

        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            var ucInernetKeyword = new UCInernetKeywordEdit();
            ucInernetKeyword.Dialog = Window.ShowDialog("添加外网搜索关键词 ", ucInernetKeyword, (s, args) =>
            {
                if (args.DialogResult == DialogResultType.OK)
                {
                    dgInernetKeywordQueryResult.Bind();
                }
            }, new Size(750, 600));
        }

        /// <summary>
        /// 有效
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnEffective_Click(object sender, RoutedEventArgs e)
        {
            UpdateStatus(IsDefaultStatus.Active);
        }

        /// <summary>
        /// 无效
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnInvalid_Click(object sender, RoutedEventArgs e)
        {
            UpdateStatus(IsDefaultStatus.Deactive);
        }

        /// <summary>
        /// 编辑
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void hyperlinkStatus_Click(object sender, RoutedEventArgs e)
        {
            var item = dgInernetKeywordQueryResult.SelectedItem as dynamic;
            int sysNo = item.SysNo;
            if (sysNo > 0)
            {
                var opFacade = new InernetKeywordFacade();
                var status = item.SetStatus;
                IsDefaultStatus enumStatus;
                Enum.TryParse(status, out enumStatus);
                var list = new List<InternetKeywordInfo>();
                var entity = new InternetKeywordInfo { SysNo = sysNo, Status = enumStatus };
                list.Add(entity);
                opFacade.ModifyKeywordStatus(list, (obj, arg) =>
                {
                    if (arg.FaultsHandle())
                    {
                        return;
                    }

                    Window.Alert("操作成功！");
                    dgInernetKeywordQueryResult.Bind();
                });

            }
           
        }

        #endregion

        #endregion

        #region 跳转

        /// <summary>
        /// 修改状态
        /// </summary>
        /// <param name="status"></param>
        private void UpdateStatus(IsDefaultStatus status)
        {
            var opFacade = new InernetKeywordFacade();
            var list = new List<InternetKeywordInfo>();

            var viewlist = dgInernetKeywordQueryResult.ItemsSource as dynamic;
            if (viewlist != null)
            {
                foreach (var item in viewlist)
                {
                    if (item.IsChecked == true)
                    {
                        var entity = new InternetKeywordInfo
                        {
                            SysNo = item.SysNo,
                            Status = status,
                            OperateUser = new UserInfo
                            {
                                SysNo =
                                    CPApplication.Current.LoginUser.UserSysNo ?? 0,
                                UserName = CPApplication.Current.LoginUser.LoginName
                            }
                        };
                        list.Add(entity);
                    }
                }
            }
            if (list.Count > 0)
            {
                opFacade.ModifyKeywordStatus(list, (obj, arg) =>
                {
                    if (arg.FaultsHandle())
                    {
                        return;
                    }

                    Window.Alert("操作成功！");
                    dynamic d = this.dgInernetKeywordQueryResult.ItemsSource as dynamic;
                    if (viewlist != null)
                    {
                        foreach (var item in d)
                        {
                            item.IsChecked = false;
                        }
                    }
                    dgInernetKeywordQueryResult.Bind();
                });
            }
            else
            {
                Window.Alert("请先选择！");
            }
        }
        #endregion

      
      
    }
}
