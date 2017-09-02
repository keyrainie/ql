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
using ECCentral.BizEntity.Inventory;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.Portal.UI.Inventory.Facades;
using ECCentral.Portal.UI.Inventory.Models;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using Newegg.Oversea.Silverlight.Controls;
using Newegg.Oversea.Silverlight.Controls.Components;
using Newegg.Oversea.Silverlight.Utilities.Validation;

namespace ECCentral.Portal.UI.Inventory.UserControls.BatchMgmt
{
    public partial class UCAdventProductsEdit : UserControl
    {
        public IDialog Dialog { get; set; }

        public IWindow CurrentWindow
        {
            get
            {
                return CPApplication.Current.CurrentPage.Context.Window;
            }
        }

        public IPage CurrentPage
        {
            get
            {
                return CPApplication.Current.CurrentPage;
            }
        }

        private AdventProductsInfoVM _vm;
        private bool isAddAction;
        private AdventProductsFacade _facade;

        public UCAdventProductsEdit()
        {
            _vm = new AdventProductsInfoVM();
            InitializeComponent();
            isAddAction = true;
            this.Loaded += UCAdventProductsEdit_Loaded;
        }

        public UCAdventProductsEdit(dynamic editData)
        {
            InitializeComponent();
            isAddAction = false;
            _vm = new AdventProductsInfoVM()
            {
                BrandSysNo = editData["BrandSysNo"].ToString(),
                C3SysNo = editData["C3SysNo"].ToString(),
                BrandName = editData["BrandName"],
                Email = editData["Email"],
                RingDay = editData["RingDay"].ToString(),
                SysNo = editData["SysNo"],
                EditUser = editData["EditUser"]
            };
            this.Loaded += UCAdventProductsEdit_Loaded;
        }

        void UCAdventProductsEdit_Loaded(object sender, RoutedEventArgs e)
        {
            this.Loaded -= UCAdventProductsEdit_Loaded;
            _facade = new AdventProductsFacade(this.CurrentPage);
            if (_vm.SysNo.HasValue)
            {
                //this.ucCategory.Category3SysNo = _vm.C3SysNo;
                //this.ucBrand.SelectedBrandSysNo = _vm.BrandSysNo.ToString();
                //this.ucBrand.SelectedBrandName = _vm.BrandName.ToString();
                //this.txtEmail.Text = _vm.Email;
                //this.txtRingDay.Text = _vm.RingDay.ToString();
            }
            this.ucCategory.LoadCategoryCompleted += (s, args) =>
            {
                this.DataContext = _vm;
            };
        }
        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidationManager.Validate(this.LayoutRoot))
            {
                return;
            }
            if (string.IsNullOrEmpty(_vm.BrandSysNo))
            {
                CurrentWindow.Alert("错误", "请选择一个品牌!", MessageType.Error);
                return;
            }
            if (string.IsNullOrEmpty(_vm.C3SysNo))
            {
                CurrentWindow.Alert("错误", "请选择一个3级类别!", MessageType.Error);
                return;
            }
            CurrentWindow.Confirm("提示", "确定要进行保存操作吗?", (obj, args) =>
            {
                if (args.DialogResult == DialogResultType.OK)
                {
                    ProductRingDayInfo entity = EntityConverter<AdventProductsInfoVM, ProductRingDayInfo>.Convert(_vm);
                    if (isAddAction)
                    {
                        _facade.InsertProductRingInfo(entity, (obj2, args2) =>
                        {
                            if (args2.FaultsHandle())
                            {
                                return;
                            }
                            CurrentWindow.Alert("提示", "创建成功!", MessageType.Information, (obj3, args3) =>
                           {
                               this.Dialog.ResultArgs.DialogResult = DialogResultType.OK;
                               this.Dialog.Close(true);
                           });
                        });
                    }
                    else
                    {
                        _facade.UpdateProductRingInfo(entity, (obj2, args2) =>
                        {
                            if (args2.FaultsHandle())
                            {
                                return;
                            }
                            CurrentWindow.Alert("提示", "保存成功!", MessageType.Information, (obj3, args3) =>
                            {
                                this.Dialog.ResultArgs.DialogResult = DialogResultType.OK;
                                this.Dialog.Close(true);
                            });
                        });
                    }
                }
            });
        }
        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Dialog.ResultArgs.DialogResult = DialogResultType.Cancel;
            this.Dialog.Close(true);
        }
    }
}
