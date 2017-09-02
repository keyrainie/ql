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
using ECCentral.Portal.UI.MKT.Models;
using ECCentral.BizEntity.MKT;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using ECCentral.Portal.UI.MKT.Facades.Promotion;
using Newegg.Oversea.Silverlight.Controls;
using Newegg.Oversea.Silverlight.Utilities.Validation;

namespace ECCentral.Portal.UI.MKT.UserControls.Coupon
{
    public partial class UCCouponCodeEdit : UserControl
    {
        public IDialog Dialog { get; set; }

        private CouponCodeSettingViewModel _commonCodeViewModel = new CouponCodeSettingViewModel();
        private CouponCodeSettingViewModel _throwInCodeViewModel = new CouponCodeSettingViewModel();
        private bool isLoaded = false;

        private CouponsFacade _facade;

        public int CouponsSysNo { get; set; }

        public string CCCustomerMaxFrequency { get; set; }

        public string CCMaxFrequency { get; set; }

        private IWindow CurrentWindow
        {
            get
            {
                return CPApplication.Current.CurrentPage.Context.Window;
            }
        }

        public UCCouponCodeEdit()
        {
            InitializeComponent();
            this.Loaded += new RoutedEventHandler(UCCouponCodeEdit_Loaded);
        }

        void UCCouponCodeEdit_Loaded(object sender, RoutedEventArgs e)
        {
            if (isLoaded)
            {
                return;
            }

            _commonCodeViewModel.CouponsSysNo = CouponsSysNo;
            _commonCodeViewModel.CouponCodeType = CouponCodeType.Common;
            gridCommonCode.DataContext = _commonCodeViewModel; 

            _throwInCodeViewModel.CouponsSysNo = CouponsSysNo;
            _throwInCodeViewModel.CouponCodeType = CouponCodeType.ThrowIn; 

            gridThrowInCode.DataContext = _throwInCodeViewModel;

            _facade = new CouponsFacade(CPApplication.Current.CurrentPage);
            
            isLoaded = true;
        }

        private void btnSaveCommonCode_Click(object sender, RoutedEventArgs e)
        {
            ValidationManager.Validate(this.gridCommonCode);
            if (_commonCodeViewModel.HasValidationErrors) return;

            if (string.IsNullOrEmpty(_commonCodeViewModel.CouponCode) || string.IsNullOrEmpty(_commonCodeViewModel.CouponCode.Trim()))
            {
                CurrentWindow.Alert("优惠券代码不能为空！");
                return;
            }
            if (string.IsNullOrEmpty(_commonCodeViewModel.CCCustomerMaxFrequency))
            {
                CurrentWindow.Alert("每个ID限用次数必须输入！");
                return;
            }


            _facade.CreateCouponCode(_commonCodeViewModel, () =>
                {
                    CurrentWindow.Alert("通用型优惠券创建成功！", MessageType.Information);

                    this.Dialog.ResultArgs.DialogResult = DialogResultType.OK;
                    this.Dialog.ResultArgs.Data = null;
                    this.Dialog.Close(true);
                });
        }


        private void btnSaveThrowInCode_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidationManager.Validate(this.gridThrowInCode))
            {
                return;
            }
            if (_throwInCodeViewModel.HasValidationErrors) return;

            if (string.IsNullOrEmpty(_throwInCodeViewModel.ThrowInCodeCount))
            {
                CurrentWindow.Alert("优惠券张数必须输入！");
                return;
            }
            if (string.IsNullOrEmpty(_throwInCodeViewModel.CCCustomerMaxFrequency))
            {
                CurrentWindow.Alert("每个ID限用次数必须输入！");
                return;
            }
            

            _facade.CreateCouponCode(_throwInCodeViewModel, () =>
            {
                CurrentWindow.Alert("所有投放型优惠券创建成功！", MessageType.Information);

                this.Dialog.ResultArgs.DialogResult = DialogResultType.OK;
                this.Dialog.ResultArgs.Data = null;
                this.Dialog.Close(true);
            });

        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Dialog.ResultArgs.DialogResult = DialogResultType.Cancel;
            this.Dialog.ResultArgs.Data = null;
            this.Dialog.Close(true);
        }


    }
}
