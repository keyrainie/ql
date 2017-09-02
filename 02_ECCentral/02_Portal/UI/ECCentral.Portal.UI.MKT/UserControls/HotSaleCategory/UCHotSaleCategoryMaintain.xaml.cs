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
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using ECCentral.Portal.UI.MKT.Facades;
using ECCentral.BizEntity.MKT;
using ECCentral.Portal.UI.MKT.Models;
using ECCentral.Portal.Basic.Utilities;
using Newegg.Oversea.Silverlight.Controls.Components;
using Newegg.Oversea.Silverlight.Controls;
using Newegg.Oversea.Silverlight.Utilities.Validation;
using ECCentral.Portal.UI.MKT.Resources;
using ECCentral.BizEntity.Enum.Resources;

namespace ECCentral.Portal.UI.MKT.UserControls
{
    public partial class UCHotSaleCategoryMaintain : UserControl
    {
        private HotSaleCategoryVM _currentVM;
        /// <summary>
        /// 标识界面是否处于创建新记录模式
        /// </summary>
        private bool _isEditing;

        /// <summary>
        /// 当前打开的Tab页面
        /// </summary>
        private IWindow CurrentWindow
        {
            get
            {
                return CPApplication.Current.CurrentPage.Context.Window;
            }
        }

        /// <summary>
        /// 窗口句柄
        /// </summary>
        public IDialog DialogHandle { get; set; }

        public UCHotSaleCategoryMaintain(int currentSysNo)
        {
            InitializeComponent();

            this.lstChannelList.ItemsSource = CPApplication.Current.CurrentWebChannelList.ToList<UIWebChannel>();
            this.rbInvalid.Content = EnumConverter.GetDescription(ADStatus.Deactive, typeof(ADStatus));
            this.rbValid.Content = EnumConverter.GetDescription(ADStatus.Active, typeof(ADStatus));

            this.ucPageType.PageTypeLoadCompleted += new EventHandler(ucPageType_PageTypeLoadCompleted);
            this.ucPageType.PagePositionLoadCompleted += new EventHandler(ucPageType_PagePositionLoadCompleted);
            if (currentSysNo > 0)
            {
                //当前界面正在编辑模式,加载数据
                _isEditing = true;
                var hotSaleCategoryFacade = new HotSaleCategoryFacade(CPApplication.Current.CurrentPage);
                hotSaleCategoryFacade.Load(currentSysNo, (vm) =>
                    {
                        this.ucCategoeyPicker.LoadCategoryCompleted += new EventHandler<EventArgs>(ucCategoeyPicker_LoadCategoryCompleted);
                        _currentVM = vm;
                        this.DataContext = _currentVM;
                        //cbUpdateSameGroupAl.Visibility = System.Windows.Visibility.Visible;
                    });
                this.btnCreate.Visibility = System.Windows.Visibility.Visible;
            }
            else
            {
                _isEditing = false;
                _currentVM = new HotSaleCategoryVM();
                this.DataContext = _currentVM;
                this.lstChannelList.SelectedIndex = 0;
                this.btnCreate.Visibility = System.Windows.Visibility.Collapsed;
            }
        }

        void ucCategoeyPicker_LoadCategoryCompleted(object sender, EventArgs e)
        {
            this.ucCategoeyPicker.Category3SysNo = _currentVM.C3SysNo;
        }

        void ucPageType_PagePositionLoadCompleted(object sender, EventArgs e)
        {
            if (_currentVM.Position != null)
            {
                this.lstPosition.SelectedValue = _currentVM.Position;
            }
            else
            {
                this.lstPosition.SelectedIndex = 0;
            }
        }

        void ucPageType_PageTypeLoadCompleted(object sender, EventArgs e)
        {
            if (_currentVM.PageType != null)
            {
                this.ucPageType.SetPageType(_currentVM.PageType);
            }
            else
            {
                this.ucPageType.SetFirstPageTypeSelected();
            }
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            if (button != null && button.Name == "btnCreate")
            {
                _isEditing = false;
                _currentVM.SysNo = 0;
            }


            //输入验证不通过，则直接返回
            if (!ValidationManager.Validate(this)) return;
            if (this.ucPageType.PageType == null)
            {
                CurrentWindow.Alert(ResHotSaleCategory.Info_PleaseSelectPageType);
                return;
            }
            _currentVM.PageType = this.ucPageType.PageType.Value;
            if (this.lstPosition.SelectedValue == null)
            {
                CurrentWindow.Alert(ResHotSaleCategory.Info_PleaseSelectPosition);
                return;
            }
            _currentVM.Position = int.Parse(this.lstPosition.SelectedValue.ToString());
            if (this.ucCategoeyPicker.ChooseCategory3SysNo == null)
            {
                CurrentWindow.Alert(ResHotSaleCategory.Info_PleaseSelectC3);
                return;
            }
            _currentVM.C3SysNo = this.ucCategoeyPicker.ChooseCategory3SysNo;
            var hotSaleCategoryFacade = new HotSaleCategoryFacade(CPApplication.Current.CurrentPage);
            if (_isEditing)
            {
                //编辑
                hotSaleCategoryFacade.Update(_currentVM, this.cbUpdateSameGroupAl.IsChecked, () =>
                {
                    CurrentWindow.Alert(ResHotSaleCategory.Info_EditSuccess);
                    CloseDialog(DialogResultType.OK);
                });
            }
            else
            {
                //新建
                hotSaleCategoryFacade.Create(_currentVM, () =>
                {
                    CurrentWindow.Alert(ResHotSaleCategory.Info_AddSuccess);
                    CloseDialog(DialogResultType.OK);
                });
            }
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            CloseDialog(DialogResultType.Cancel);
        }

        private void CloseDialog(DialogResultType result)
        {
            if (DialogHandle != null)
            {
                DialogHandle.ResultArgs = new ResultEventArgs
                {
                    DialogResult = result
                };
                DialogHandle.Close();
            }
        }
        private void ucPageType_PageTypeSelectionChanged(object sender, PageTypeSelectionChangedEventArgs e)
        {
            if (cbUpdateSameGroupAl != null)
            {
                lstFlagshipList.Visibility = System.Windows.Visibility.Collapsed;
                if (e.PageType == 18)
                {
                    cbUpdateSameGroupAl.Visibility = System.Windows.Visibility.Visible;
                }
                else if (e.PageType == 9)
                {
                    //根据选定的页面类型加载页面列表
                    PageTypeFacade facade = new PageTypeFacade(CPApplication.Current.CurrentPage);
                    facade.GetPages(CPApplication.Current.CompanyCode, lstChannelList.SelectedValue == null ? "1" : lstChannelList.SelectedValue.ToString(), (int)ModuleType.Banner, "9", (s, args) =>
                    {
                        if (args.FaultsHandle())
                            return;
                        if (args.Result != null && args.Result.PageList != null)
                        {
                            args.Result.PageList.Insert(0, new WebPage { ID = null, PageName = ResCommonEnum.Enum_Select });
                            lstFlagshipList.ItemsSource = args.Result.PageList;
                            if (lstFlagshipList.Items.Count > 0 && (int)lstFlagshipList.SelectedValue == 0)
                            {
                                lstFlagshipList.SelectedIndex = 0;
                            }
                        }
                    });
                    lstFlagshipList.Visibility = System.Windows.Visibility.Visible;
                }
                else
                {
                    cbUpdateSameGroupAl.Visibility = System.Windows.Visibility.Collapsed;
                }
            }
        }
    }
}
