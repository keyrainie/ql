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
using Newegg.Oversea.Silverlight.Controls.Components;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.BizEntity.MKT;
using ECCentral.Portal.UI.MKT.Models;
using Newegg.Oversea.Silverlight.Controls;
using Newegg.Oversea.Silverlight.Utilities.Validation;
using ECCentral.Portal.UI.MKT.Facades;
using System.Collections.ObjectModel;
using ECCentral.BizEntity.IM;
using ECCentral.Portal.UI.MKT.Resources;
using ECCentral.Portal.Basic.Components.UserControls.ProductPicker;

namespace ECCentral.Portal.UI.MKT.UserControls
{
    public partial class UCECCategoryMaintain : UserControl
    {
        private ECCategoryVM _currentVM;
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

       

        public UCECCategoryMaintain()
        {
            InitializeComponent();
            this.rbDeactive.Content = EnumConverter.GetDescription(ADStatus.Deactive, typeof(ADStatus));
            this.rbActive.Content = EnumConverter.GetDescription(ADStatus.Active, typeof(ADStatus));
            this.rbNone.Content = ResECCategory.TextBlock_None;
            this.rbNew.Content = EnumConverter.GetDescription(FeatureType.New, typeof(FeatureType));
            this.rbHot.Content = EnumConverter.GetDescription(FeatureType.Hot, typeof(FeatureType));
            this.lstChannelList.ItemsSource = CPApplication.Current.CurrentWebChannelList.ToList<UIWebChannel>();
            this.lstCategoryLevel.ItemsSource = EnumConverter.GetKeyValuePairs<ECCategoryLevel>();
        }

        public void ChangeToEditMode(ECCategoryVM selectedNode)
        {
            //当前界面正在编辑模式,加载数据
            _isEditing = true;
            this.txtTitle.Text = ResECCategory.Info_EditTitle;
            _currentVM = selectedNode;
            
            this.DataContext = _currentVM;
            this.btnDelete.Visibility = Visibility.Visible;

            this.txtPriority.IsReadOnly = _currentVM.Level != ECCategoryLevel.Category1;

            if (_currentVM.Level == ECCategoryLevel.Category2)
            {
                this.chbShowInParent.Content = "一级类页面显示";
            }

            if (_currentVM.Level == ECCategoryLevel.Category3)
            {
                this.chbShowInParent.Content = "二级类页面显示";
                LoadAllBackendC3(() =>
                {
                    this.lstBackendC3.SelectedValue = _currentVM.C3SysNo;
                });
                this.btnAddChild.Visibility = Visibility.Collapsed;             
            }
            else
            {
                this.btnAddChild.Visibility = Visibility.Visible;
            }
            //加载父类列表
            LoadParentView((unAssignedParent, assignedParent) =>
            {
                if (assignedParent != null)
                {
                    //先清空
                    _currentVM.Parents.Clear();
                    //再逐个加入
                    foreach (var p in assignedParent)
                    {
                        _currentVM.Parents.Add(p);
                    }
                    _currentVM.ParentCount = _currentVM.Parents.Count;
                }
            });
            LoadChildView((unAssignedChildren, assignedChildren) =>
                {
                    if (assignedChildren != null)
                    {

                        //先清空
                        _currentVM.Children.Clear();
                        //再逐个加入
                        foreach (var p in assignedChildren)
                        {
                            _currentVM.Children.Add(p);
                        }
                        _currentVM.ChildrenCount = _currentVM.Children.Count;
                    }
                });
        }

        public void ChangeToCreateMode(ECCategoryVM toCreateVM)
        {
            _isEditing = false;
            this.txtTitle.Text = ResECCategory.Info_AddTitle;
            this.btnAddChild.Visibility = Visibility.Collapsed;
            this.btnDelete.Visibility = Visibility.Collapsed;

            _currentVM = toCreateVM;
            this.DataContext = _currentVM;
            this.lstChannelList.SelectedIndex = 0;
            this.txtPriority.IsReadOnly = _currentVM.Level != ECCategoryLevel.Category1;

            if (_currentVM.Level == ECCategoryLevel.Category2)
            {
                this.chbShowInParent.Content = "一级类页面显示";
            }

            if (_currentVM.Level == ECCategoryLevel.Category3)
            {
                this.chbShowInParent.Content = "二级类页面显示";
                LoadAllBackendC3(() =>
                {
                    this.lstBackendC3.SelectedIndex = 0;
                });
            }
        }

        private List<BackendCategoryVM> _allValidBackendC3 = new List<BackendCategoryVM>();
        private void LoadAllBackendC3(Action callback)
        {
            if (_allValidBackendC3.Count == 0)
            {
                new ExternalServiceFacade(CPApplication.Current.CurrentPage).GetAllActiveBackendC3((s, args) =>
                {
                    if (args.FaultsHandle()) return;
                    if (args.Result != null)
                    {
                        foreach (var c in args.Result)
                        {
                            _allValidBackendC3.Add(new BackendCategoryVM
                            {
                                SysNo = c.SysNo,
                                CategoryName = c.CategoryName != null ? c.CategoryName.Content : ""
                            });
                        }
                    }
                    lstBackendC3.ItemsSource = _allValidBackendC3;
                    if (callback != null)
                    {
                        callback();
                    }
                });
            }
            else
            {
                if (callback != null)
                {
                    callback();
                }
            }
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            //输入验证不通过，则直接返回
            if (!ValidationManager.Validate(this)) return;

            var facade = new ECCategoryFacade(CPApplication.Current.CurrentPage);

            if (_currentVM.Level == ECCategoryLevel.Category3)
            {
                if (_currentVM.Name.Length < 1 || _currentVM.Name.Length > 10)
                {
                    CurrentWindow.Alert("三级类别最少1个字，最多10个字");
                    return;
                }
            }
            else if (_currentVM.Level == ECCategoryLevel.Category2)
            {
                if (_currentVM.Name.Length < 1 || _currentVM.Name.Length > 15)
                {
                    CurrentWindow.Alert("二级类别最少1个字，最多不能超过15个字");
                    return;
                }
            }
            else
            {
                if (_currentVM.Name.Length > 15)
                {
                    CurrentWindow.Alert("名称字数长度超范围，一级类别不可大于15个字");
                    return;
                }
            }

            if (_isEditing)
            {
                //编辑
                facade.Update(_currentVM, (s, args) =>
                {
                    if (args.FaultsHandle()) return;
                    CurrentWindow.Alert(ResECCategory.Info_EditSuccess);
                    OnEditCompleted(_currentVM);
                });
            }
            else
            {
                //新建
                facade.Create(_currentVM, (s, args) =>
                {
                    if (args.FaultsHandle()) return;
                    CurrentWindow.Alert(ResECCategory.Info_AddSuccess);

                    _currentVM = args.Result.Convert<ECCategory, ECCategoryVM>();

                    _currentVM.ChannelID = args.Result.WebChannel == null ? "1" : args.Result.WebChannel.ChannelID;
               
                    OnAddCompleted(_currentVM);

                    OnCancelClick(); 
                   
                });
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            OnCancelClick();
        }

        private void btnMaintainParent_Click(object sender, RoutedEventArgs e)
        {
            LoadParentView((unAssignedParent, assignedParent) =>
            {
                UCECCategoryParent ucParent = new UCECCategoryParent();
                ucParent.DialogHandle = CurrentWindow.ShowDialog(ResECCategory.Info_ParentMaintainTitle, ucParent, OnParentMaintainClosed);
                ucParent.BindData(unAssignedParent, assignedParent);
            });
        }

        private void LoadParentView(Action<ObservableCollection<ECCategoryVM>, ObservableCollection<ECCategoryVM>> callback)
        {
            var ecCategoryFacade = new ECCategoryFacade(CPApplication.Current.CurrentPage);
            ecCategoryFacade.LoadParentView(_currentVM.SysNo ?? 0, _currentVM.Level, (s, args) =>
            {
                if (args.FaultsHandle()) return;
                ObservableCollection<ECCategoryVM> unAssignedParent = new ObservableCollection<ECCategoryVM>();
                ObservableCollection<ECCategoryVM> assignedParent = new ObservableCollection<ECCategoryVM>();
                ECCategoryParentView parentView = args.Result;
                var parentVMList = parentView.ParentCategoryList.Convert<ECCategory, ECCategoryVM>();
                foreach (var p in parentVMList)
                {

                    if ((parentView.CurrentParentSysNoList != null && parentView.CurrentParentSysNoList.IndexOf(p.RSysNo) > -1)
                        || _currentVM.Parents.FirstOrDefault(i => i.RSysNo == p.RSysNo) != null)
                    {
                        assignedParent.Add(p);
                    }
                    else
                    {
                        unAssignedParent.Add(p);
                    }
                }
                if (callback != null)
                {
                    callback(unAssignedParent, assignedParent);
                }
            });
        }

        protected void OnParentMaintainClosed(object sender, ResultEventArgs e)
        {
            var assignedParent = e.Data as ObservableCollection<ECCategoryVM>;
            if (assignedParent != null)
            {
                _currentVM.Parents.Clear();
                foreach (var p in assignedParent)
                {
                    _currentVM.Parents.Add(p);
                }
                _currentVM.ParentCount = assignedParent.Count;
            }
        }

        private void btnAddChild_Click(object sender, RoutedEventArgs e)
        {
            if (_currentVM != null)
            {

                //当前节点做为要创建子节点的父级节点
                ECCategoryVM parent = _currentVM;
                _currentVM = new ECCategoryVM();
                //根据当前节点的Level确定其子级的level
                ECCategoryLevel childLevel = ECCategoryLevel.Category1;
                if (parent.Level == ECCategoryLevel.Category1)
                {
                    childLevel = ECCategoryLevel.Category2;
                }
                else if (parent.Level == ECCategoryLevel.Category2)
                {
                    childLevel = ECCategoryLevel.Category3;
                }
                _currentVM.Level = childLevel;
                _currentVM.ChannelID = parent.ChannelID;
                _currentVM.Parents.Add(parent);
                _currentVM.ParentCount = _currentVM.Parents.Count;

                ChangeToCreateMode(_currentVM);
            }
        }

        private void btnMaintainChild_Click(object sender, RoutedEventArgs e)
        {
            LoadChildView((unAssignedChild, assignedChild) =>
            {
                UCECCategoryParent ucParent = new UCECCategoryParent();
                ucParent.DialogHandle = CurrentWindow.ShowDialog(ResECCategory.Info_ChildMaintainTitle, ucParent, OnChildMaintainClosed);
                ucParent.BindData(unAssignedChild, assignedChild);
            });
        }

        private void LoadChildView(Action<ObservableCollection<ECCategoryVM>, ObservableCollection<ECCategoryVM>> callback)
        {
            var ecCategoryFacade = new ECCategoryFacade(CPApplication.Current.CurrentPage);
            ecCategoryFacade.LoadChildView(_currentVM.SysNo ?? 0, _currentVM.Level, _currentVM.RSysNo, (s, args) =>
            {
                if (args.FaultsHandle()) return;
                ObservableCollection<ECCategoryVM> unAssignedChild = new ObservableCollection<ECCategoryVM>();
                ObservableCollection<ECCategoryVM> assignedChild = new ObservableCollection<ECCategoryVM>();
                var childView = args.Result;
                var parentVMList = childView.ChildCategoryList.Convert<ECCategory, ECCategoryVM>();
                foreach (var p in parentVMList)
                {

                    if (childView.CurrentChildSysNoList != null && childView.CurrentChildSysNoList.IndexOf(p.SysNo.Value) > -1)
                    {
                        assignedChild.Add(p);
                    }
                    else
                    {
                        unAssignedChild.Add(p);
                    }
                }
                if (callback != null)
                {
                    callback(unAssignedChild, assignedChild);
                }
            });
        }

        protected void OnChildMaintainClosed(object sender, ResultEventArgs e)
        {
            var assignedChild = e.Data as ObservableCollection<ECCategoryVM>;
            if (assignedChild != null)
            {
                _currentVM.Children.Clear();
                foreach (var p in assignedChild)
                {
                    _currentVM.Children.Add(p);
                }
                _currentVM.ChildrenCount = assignedChild.Count;
            }
        }

        public event EventHandler<ECCategoryActionEventArgs> AddCompleted;
        public event EventHandler<ECCategoryActionEventArgs> EditCompleted;
        public event EventHandler CancelClick;
        public event EventHandler<ECCategoryActionEventArgs> DeleteCompleted;

        protected virtual void OnAddCompleted(ECCategoryVM vm)
        {
            var hander = AddCompleted;
            if (hander != null)
            {
                hander(this, new ECCategoryActionEventArgs(vm));
            }
        }

        protected virtual void OnEditCompleted(ECCategoryVM vm)
        {
            var hander = EditCompleted;
            if (hander != null)
            {
                hander(this, new ECCategoryActionEventArgs(vm));
            }
        }

        protected virtual void OnCancelClick()
        {
            var hander = CancelClick;
            if (hander != null)
            {
                hander(this, EventArgs.Empty);
            }
        }

        protected virtual void OnDeleteCompleted(ECCategoryVM vm)
        {
            var hander = DeleteCompleted;
            if (hander != null)
            {
                hander(this, new ECCategoryActionEventArgs(vm));
            }
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (_currentVM != null && _currentVM.SysNo.HasValue)
            {
                CurrentWindow.Confirm(ResECCategory.Confirm_Delete, (d, r) =>
                {
                    if (r.DialogResult == DialogResultType.OK)
                    {
                        var facade = new ECCategoryFacade(CPApplication.Current.CurrentPage);
                        facade.Delete(_currentVM.SysNo.Value, (s, args) =>
                        {
                            if (args.FaultsHandle()) return;

                            CurrentWindow.Alert(ResECCategory.Info_DeleteSuccess);
                            OnDeleteCompleted(_currentVM);
                        });
                    }
                });
            }
        }


        private void btnAddProduct_Click(object sender, RoutedEventArgs e)
        {
            if ((this._currentVM.SysNo ?? 0) <= 0)
            {
                this.CurrentWindow.Alert("请选择要添加商品的类别！", MessageType.Warning);
                return;
            }
            if (_currentVM.Level != ECCategoryLevel.Category3)
            {
                this.CurrentWindow.Alert("你当前选中的分类不是三级类别，不能添加商品！", MessageType.Warning);
                return;
            }

            UCProductSearch picker = new UCProductSearch();
            picker.SelectionMode = SelectionMode.Multiple;

            IDialog dialog = CurrentWindow.ShowDialog("选择商品", picker, (o, a) =>
            {
                if (a.DialogResult == DialogResultType.OK)
                {
                    var list = a.Data as List<ProductVM>;
                    if (list != null && list.Count > 0)
                    {
                        List<int> sysNoList = new List<int>();
                        list.ForEach(p =>
                        {
                            sysNoList.Add(p.SysNo.Value);
                        });
                        new ECCategoryFacade(CPApplication.Current.CurrentPage).CreateMapping(this._currentVM.SysNo.Value, sysNoList, (s, args) =>
                        {
                            if (args.FaultsHandle())
                            {
                                return;
                            }
                            this.CurrentWindow.Alert("提示", "添加商品成功！", MessageType.Information, (se, ar) =>
                            {
                                this.dgProductMapping.Bind();
                            });
                        });
                    }
                }
            });
            picker.DialogHandler = dialog;
        }

        private void ChbSelectAllProductGroupRowClick(object sender, RoutedEventArgs e)
        {
            var ckb = sender as CheckBox;
            if (ckb == null) return;
            var viewList = this.dgProductMapping.ItemsSource as dynamic;
            if (viewList == null) return;
            foreach (var view in viewList)
            {
                view.IsChecked = ckb.IsChecked != null && ckb.IsChecked.Value;
            }
        }

        private void btnDeleteProduct_Click(object sender, RoutedEventArgs e)
        {
            var source = this.dgProductMapping.ItemsSource as dynamic;
            if (source == null)
            {
                this.CurrentWindow.Alert("请类别下无商品！", MessageType.Warning);
            }
            List<int> list = new List<int>();
            foreach (var item in source)
            {
                if (item.IsChecked == true)
                {
                    list.Add(item.ProductSysNo);
                }
            }
            if (list.Count > 0)
            {
                CurrentWindow.Confirm("你确定要删除选择的商品吗？", (o1, a1) =>
                {
                    if (a1.DialogResult == DialogResultType.OK)
                    {
                        new ECCategoryFacade(CPApplication.Current.CurrentPage).DeleteMapping(this._currentVM.SysNo.Value, list, (s, args) =>
                        {
                            if (args.FaultsHandle())
                            {
                                return;
                            }
                            //重新绑定数据
                            this.dgProductMapping.Bind();
                        });
                    }
                });
            }
            else
            {
                UCProductSearch picker = new UCProductSearch();
                picker.SelectionMode = SelectionMode.Multiple;

                IDialog dialog = CurrentWindow.ShowDialog("删除商品", picker, (o, a) =>
                {
                    if (a.DialogResult == DialogResultType.OK)
                    {
                        var list2 = a.Data as List<ProductVM>;
                        if (list2 != null && list2.Count > 0)
                        {
                            List<int> sysNoList = new List<int>();
                            list2.ForEach(p =>
                            {
                                sysNoList.Add(p.SysNo.Value);
                            });

                            CurrentWindow.Confirm("你确定要删除选择的商品吗？", (o1, a1) =>
                            {
                                if (a1.DialogResult == DialogResultType.OK)
                                {
                                    new ECCategoryFacade(CPApplication.Current.CurrentPage).DeleteMapping(this._currentVM.SysNo.Value, sysNoList, (s, args) =>
                                    {
                                        if (args.FaultsHandle())
                                        {
                                            return;
                                        }
                                        //重新绑定数据
                                        this.dgProductMapping.Bind();
                                    });
                                }
                            });
                        }
                    }
                });
                picker.DialogHandler = dialog;
            }
        }

        private void dgProductMapping_LoadingDataSource(object sender, Newegg.Oversea.Silverlight.Controls.Data.LoadingDataEventArgs e)
        {
            new ECCategoryFacade(CPApplication.Current.CurrentPage).QueryProductMapping(this._currentVM.SysNo.Value, e.PageIndex, e.PageSize, e.SortField, (o, a) =>
            {
                if (a.FaultsHandle())
                {
                    return;
                }
                this.dgProductMapping.TotalCount = a.Result.TotalCount;
                this.dgProductMapping.ItemsSource = a.Result.Rows.ToList("IsChecked", false);
            });
        }

        public void BindDataGrid()
        {
            chkAll.IsChecked = false;
            this.dgProductMapping.Bind();
        }
    }

    public class ECCategoryActionEventArgs : EventArgs
    {
        private ECCategoryVM _data;

        public ECCategoryVM Data
        {
            get
            {
                return _data;
            }
        }
        public ECCategoryActionEventArgs(ECCategoryVM data)
        {
            _data = data;
        }
    }
}
