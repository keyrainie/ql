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
using ECCentral.Portal.UI.MKT.Views;

namespace ECCentral.Portal.UI.MKT.UserControls
{
    public partial class UCECDynamicCategoryMaintain : UserControl
    {
        private ECDynamicCategoryVM selecteddVM;
        public ECDynamicCategoryVM SelectedVM
        {
            get
            {
                return selecteddVM;
            }
            set
            {
                selecteddVM = value;
                this.VM = value.DeepCopy();
            }
        }

        public event EventHandler<ECDynamicCategoryActionEventArgs> AddCompleted;
        public event EventHandler<ECDynamicCategoryActionEventArgs> EditCompleted;
        public event EventHandler CancelClick;
        public event EventHandler<ECDynamicCategoryActionEventArgs> DeleteCompleted;
        

        public ECDynamicCategoryVM VM
        {
            get
            {
                return this.gridMaintain.DataContext as ECDynamicCategoryVM;
            }
            set
            {
                this.gridMaintain.DataContext = value;
                value.ValidationErrors.Clear();
                this.ButtonPanel.DataContext = value;
            }
        }

        public IPage Page
        {
            get
            {
                return CPApplication.Current.CurrentPage;
            }
        }
      
        private IWindow CurrentWindow
        {
            get
            {
                return CPApplication.Current.CurrentPage.Context.Window;
            }
        }


        public UCECDynamicCategoryMaintain()
        {
            InitializeComponent();            
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

        private void btnAddProduct_Click(object sender, RoutedEventArgs e)
        {
            if ((this.VM.SysNo ?? 0) <= 0)
            {
                this.CurrentWindow.Alert("请选择要添加商品的类别！", MessageType.Warning);
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
                        list.ForEach(p=>{
                            sysNoList.Add(p.SysNo.Value);
                        });
                        new ECDynamicCategoryFacade(this.Page).CreateMapping(this.VM.SysNo.Value, sysNoList, (s, args) =>
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
            if (list.Count == 0)
            {
                CurrentWindow.Alert("请选择要删除的商品！", MessageType.Warning);
                return;
            }
            CurrentWindow.Confirm("你确定要删除选择的商品吗？", (o, a) =>
            {
                if (a.DialogResult == DialogResultType.OK)
                {
                    new ECDynamicCategoryFacade(this.Page).DeleteMapping(this.VM.SysNo.Value, list, (s, args) =>
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

        private void btnAddSubCategory_Click(object sender, RoutedEventArgs e)
        {
            if (this.SelectedVM.ParentSysNo == null)
            {
                this.CurrentWindow.Alert("请选择要添加子分类的上级类别！", MessageType.Warning);
                return;
            }
            if (this.SelectedVM.Level == 3)
            {
                this.CurrentWindow.Alert("最多只能添加三级类别！", MessageType.Warning);
                return;
            }            
            var cType = (this.Page as ECDynamicCategoryQuery).QueryVM.CategoryType;
            if (this.SelectedVM.Level == 2 && cType == DynamicCategoryType.WangJie)
            {
                this.CurrentWindow.Alert("网姐类别最多只能添加二级！", MessageType.Warning);
                return;
            }
            int level = (this.SelectedVM.SysNo ?? 0) == 0 ? 0 : this.SelectedVM.Level;
            this.VM = new ECDynamicCategoryVM() { ParentSysNo = this.SelectedVM.SysNo, CategoryType = cType, Level = level + 1 };
            this.dgProductMapping.ItemsSource = null;
            this.dgProductMapping.TotalCount = 0;
        }
        
        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            if ((this.VM.SysNo ?? 0) == 0)
            {
                this.CurrentWindow.Alert("请选择要删除的类别！", MessageType.Warning);
                return;
            }
            CurrentWindow.Confirm("你确定要删除选择该分类吗？", (o, a) =>
           {
               if (a.DialogResult == DialogResultType.OK)
               {
                   if (this.VM.SysNo.HasValue && this.VM.SysNo > 0)
                   {
                       new ECDynamicCategoryFacade(this.Page).Delete(this.VM.SysNo.Value, (s, args) =>
                       {
                           if (args.FaultsHandle())
                           {
                               return;
                           }                                                                                
                           this.CurrentWindow.Alert("提示", "删除成功！", MessageType.Information, (se, ar) =>
                           {
                               if (this.DeleteCompleted != null)
                               {
                                   this.DeleteCompleted(this, new ECDynamicCategoryActionEventArgs(this.VM));
                               }
                               this.VM.Status = DynamicCategoryStatus.Deactive;
                               this.dgProductMapping.ItemsSource = null;                        
                           });
                       });
                   }
               }
           });        
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (ValidationManager.Validate(this))
            {
                if (this.VM.Level > 3)
                {
                    this.CurrentWindow.Alert("最多只能建立三级分类！", MessageType.Warning);
                    return;
                }
                if (this.VM.SysNo.HasValue && this.VM.SysNo > 0)
                {
                    new ECDynamicCategoryFacade(this.Page).Update(this.VM, (o, a) =>
                    {
                        if (a.FaultsHandle())
                        {
                            return;
                        }
                        if (this.EditCompleted != null)
                        {
                            this.EditCompleted(this, new ECDynamicCategoryActionEventArgs(this.VM));
                        }
                        this.CurrentWindow.Alert("保存成功！", MessageType.Information);                        
                    });
                }
                else
                {
                    new ECDynamicCategoryFacade(this.Page).Create(this.VM, (o, a) =>
                    {
                        if (a.FaultsHandle())
                        {
                            return;
                        }
                        this.VM.SysNo = a.Result.SysNo;
                        if (this.AddCompleted != null)
                        {
                            this.AddCompleted(this, new ECDynamicCategoryActionEventArgs(this.VM));
                        }
                        this.CurrentWindow.Alert("保存成功！", MessageType.Information);
                    });
                }
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            if (this.CancelClick != null)
            {
                CancelClick(this, new EventArgs());
            }
        }

        private void dgProductMapping_LoadingDataSource(object sender, Newegg.Oversea.Silverlight.Controls.Data.LoadingDataEventArgs e)
        {
            new ECDynamicCategoryFacade(this.Page).QueryProductMapping(this.VM.SysNo.Value, e.PageIndex,e.PageSize,e.SortField, (o, a) =>
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

    public class ECDynamicCategoryActionEventArgs : EventArgs
    {
        private ECDynamicCategoryVM _data;

        public ECDynamicCategoryVM Data
        {
            get
            {
                return _data;
            }
        }
        public ECDynamicCategoryActionEventArgs(ECDynamicCategoryVM data)
        {
            _data = data;
        }
    }
}
