using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using ECCentral.Portal.UI.Invoice.Facades;
using ECCentral.Portal.UI.Invoice.Models;
using ECCentral.Portal.UI.Invoice.Utility;
using Newegg.Oversea.Silverlight.Controls.Components;
using Newegg.Oversea.Silverlight.Utilities.Validation;
using ECCentral.Portal.Basic;
using Newegg.Oversea.Silverlight.Controls;
using ECCentral.Portal.UI.Invoice.Resources;

namespace ECCentral.Portal.UI.Invoice.UserControls
{
    /// <summary>
    /// 付款单设置凭据号（ReferenceID）
    /// </summary>
    public partial class UCPayItemMaintan : PopWindow
    {
        private PayItemVM payItemVM;
        private ActionType curActionType;
        private PayItemFacade payItemFacade;

        public enum ActionType
        {
            New,
            View,
            Update,
            SetReferenceID
        }

        public UCPayItemMaintan()
        {
            InitializeComponent();
            
            curActionType = ActionType.New;
            Loaded += new RoutedEventHandler(UCPayItemEdit_Loaded);

        }

        private void UCPayItemEdit_Loaded(object sender, RoutedEventArgs e)
        {
            Loaded -= new RoutedEventHandler(UCPayItemEdit_Loaded);
            InitData();
            SetControlAvailably();
        }

        private void SetControlAvailably()
        {
            switch (curActionType)
            {
                case ActionType.View:
                    this.BaseInfo.SetChildControlAvailably(false);
                    btnSave.Visibility = Visibility.Collapsed;
                    break;

                case ActionType.New:
                case ActionType.Update:
                    this.BaseInfo.SetChildControlAvailably(false);
                    this.BaseInfo.SetChildControlAvailably(true, new List<UIElement>() { tbSysNo });
                    btnSave.Visibility = Visibility.Visible;
                    break;

                case ActionType.SetReferenceID:
                    this.BaseInfo.SetChildControlAvailably(false, new List<UIElement>() { tbReferenceID });
                    btnSave.Visibility = Visibility.Visible;
                    break;
            }
        }

        private void InitData()
        {
            payItemFacade = new PayItemFacade(CurrentPage);

            this.BaseInfo.DataContext = payItemVM;
            payItemVM.ValidationErrors.Clear();
        }

        public UCPayItemMaintan(PayItemVM model, ActionType actionType)
            : this()
        {
            payItemVM = model;
            curActionType = actionType;
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            var flag = ValidationManager.Validate(this.BaseInfo);
            if (!flag)
            {
                return;
            }

            switch (curActionType)
            {
                case ActionType.SetReferenceID:
                    SetReferenceID();
                    break;

                case ActionType.New:
                    Create();
                    break;

                case ActionType.Update:
                    Update();
                    break;
            }
        }

        private void Update()
        {
            var flag = ValidationManager.Validate(this.BaseInfo);
            if (!flag)
                return;

            payItemFacade.Update(payItemVM, () => CloseDialog(DialogResultType.OK));
        }

        private void Create()
        {
            var flag = ValidationManager.Validate(this.BaseInfo);
            if (!flag)
                return;

            payItemFacade.Create(payItemVM, () => CloseDialog(DialogResultType.OK));
        }

        

        private void SetReferenceID()
        {
            if (!AuthMgr.HasFunctionAbsolute(AuthKeyConst.Invoice_PayItemQuery_SetReferenceID_AllPath))
            {
                
                this.AlertInformationDialog(ResCommon.Message_NoAuthorize);
                return;
 
            }


            var flag = ValidationManager.Validate(this.BaseInfo);
            if (!flag)
                return;

            payItemFacade.BatchSetReferenceID(new List<PayItemVM>() { payItemVM }, (msg) => CloseDialog(DialogResultType.OK));
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            CloseDialog(DialogResultType.Cancel);
        }
    }
}