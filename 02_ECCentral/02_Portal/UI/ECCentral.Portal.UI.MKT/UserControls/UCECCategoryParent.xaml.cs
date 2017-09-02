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
using Newegg.Oversea.Silverlight.Controls;
using Newegg.Oversea.Silverlight.Controls.Components;
using ECCentral.Portal.UI.MKT.Models;
using System.Collections.ObjectModel;

namespace ECCentral.Portal.UI.MKT.UserControls
{
    public partial class UCECCategoryParent : UserControl
    {
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

        public UCECCategoryParent()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 当前可用的父类列表
        /// </summary>
        private ObservableCollection<ECCategoryVM> _unassignedParent = new ObservableCollection<ECCategoryVM>();
        /// <summary>
        /// 已分配的父类列表
        /// </summary>
        private ObservableCollection<ECCategoryVM> _assignedParent = new ObservableCollection<ECCategoryVM>();
        public void BindData(ObservableCollection<ECCategoryVM> unassignedParent, ObservableCollection<ECCategoryVM> assignedParent)
        {
            _unassignedParent.Clear();
            if (unassignedParent != null)
            {
                foreach (var p in unassignedParent)
                {
                    _unassignedParent.Add(p);
                }
            }
            this.lstUnassignedParent.ItemsSource = _unassignedParent;
            _assignedParent.Clear();
            if (assignedParent != null)
            {
                foreach (var p in assignedParent)
                {
                    _assignedParent.Add(p);
                }
            }
            this.lstAssignedParent.ItemsSource = _assignedParent;
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            CloseDialog(null);
        }

        private void CloseDialog(ObservableCollection<ECCategoryVM> data)
        {
            if (DialogHandle != null)
            {
                DialogHandle.ResultArgs = new ResultEventArgs
                {
                    Data = data
                };
                DialogHandle.Close();
            }
        }

        //增加父类
        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            var checkedItems = _unassignedParent.Where(p => p.IsChecked == true).ToList();
            foreach (ECCategoryVM p in checkedItems)
            {
                p.IsChecked = false;
                _assignedParent.Add(p);
                _unassignedParent.Remove(p);
            }
        }

        //移除父类
        private void btnRemove_Click(object sender, RoutedEventArgs e)
        {
            var checkedItems = _assignedParent.Where(p => p.IsChecked == true).ToList();
            foreach (ECCategoryVM p in checkedItems)
            {
                p.IsChecked = false;
                _unassignedParent.Add(p);
                _assignedParent.Remove(p);
            }
        }

        private void btnConfirm_Click(object sender, RoutedEventArgs e)
        {
            CloseDialog(_assignedParent);
        }
    }
}
