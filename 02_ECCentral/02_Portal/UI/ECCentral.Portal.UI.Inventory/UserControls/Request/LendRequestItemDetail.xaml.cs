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
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

#region ECCentral Libs

using ECCentral.Portal.UI.Inventory.Facades;
using ECCentral.Portal.UI.Inventory.Models;
using ECCentral.Portal.UI.Inventory.Resources;

#endregion ECCentral Libs

#region Newegg.Oversea.Oversea Libs

using Newegg.Oversea.Silverlight.ControlPanel.Core;
using Newegg.Oversea.Silverlight.Controls;
using Newegg.Oversea.Silverlight.Controls.Components;
using Newegg.Oversea.Silverlight.Utilities;
using Newegg.Oversea.Silverlight.Utilities.Validation;

#endregion Newegg.Oversea.Oversea Libs

namespace ECCentral.Portal.UI.Inventory.UserControls
{
    public partial class LendRequestItemDetail : UserControl
    {
        public IDialog Dialog
        {
            get;
            set;
        }

        private IWindow CurrentWindow
        {
            get
            {
                return CPApplication.Current.CurrentPage.Context.Window;
            }
        }

        private LendRequestItemVM m_lendItem;
        public LendRequestItemVM CurrentLendItemInfoVM
        {
            get
            {
                return m_lendItem;
            }
            private set
            {
                m_lendItem = value;
                LayoutRoot.DataContext = value;
            }
        }

        public LendRequestMaintainFacade LendRequestFacade
        {
            get;
            set;
        }

        public LendRequestItemDetail()
        {
            InitializeComponent();
            NewRequestItemContext();
            Loaded += new RoutedEventHandler(LendRequestItemDetail_Loaded);
        }

        public LendRequestItemDetail(LendRequestItemVM requestItem)
            : this()
        {
            CurrentLendItemInfoVM = UtilityHelper.DeepClone(requestItem);
        }

        #region Event Handler

        private void LendRequestItemDetail_Loaded(object sender, RoutedEventArgs e)
        {
            Loaded -= new RoutedEventHandler(LendRequestItemDetail_Loaded);
            LendRequestFacade = new LendRequestMaintainFacade(CPApplication.Current.CurrentPage);
        }

        private void NewRequestItemContext()
        {
            LendRequestItemVM model = new LendRequestItemVM()
            {
                //
            };
            CurrentLendItemInfoVM = model;
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            LendRequestItemVM model = LayoutRoot.DataContext as LendRequestItemVM;
            bool flag = ValidationManager.Validate(LayoutRoot);

            if (!model.HasValidationErrors && LendRequestFacade != null)
            {
                if (model.SysNo != null)
                {
                   //TODO:更新内存中LendItem Info
                   //由父窗口执行真正的保存操作
                    CloseDialog(new ResultEventArgs
                    {
                        DialogResult = DialogResultType.OK,
                        Data = model
                    });
                }
                else
                {
                    //新增的时候由父窗口执行真正的保存操作
                    LendRequestItemVM copy = UtilityHelper.DeepClone(model);
                    if (this.ucProductPicker.SelectedProductInfo != null)
                    {
                        copy.ProductName = this.ucProductPicker.SelectedProductInfo.ProductName;
                        copy.PMUserName = "";// this.ucProductPicker.SelectedProductInfo.PMUserName;                        
                    }
                    CurrentLendItemInfoVM = copy;
                    CloseDialog(new ResultEventArgs
                    {
                        DialogResult = DialogResultType.OK,
                        Data = copy
                    });
                }
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            CloseDialog(new ResultEventArgs
            {
                DialogResult = DialogResultType.Cancel
            });
        }

        private void txtLendQuantity_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox txt = sender as TextBox;
            txt.Text = txt.Text.Trim();
            ConvertRequestItemVM vm = txt.DataContext as ConvertRequestItemVM;
            if (!Regex.IsMatch(txt.Text, @"^-?[1-9](\d{0,5})$"))
            {
                if (Regex.IsMatch(txt.Text, @"^-?\d+$"))
                {
                    txt.Text = txt.Text.Length > 6 ? txt.Text.Substring(0, 6) : txt.Text;
                }
                else
                {
                    txt.Text = "0";
                }
            }
        }        
       

        #endregion Event Handler

        #region Helper Methods

        private void CloseDialog(ResultEventArgs args)
        {
            if (Dialog != null)
            {
                Dialog.ResultArgs = args;
                Dialog.Close();
            }
        }

        #endregion Helper Methods
    }
}
