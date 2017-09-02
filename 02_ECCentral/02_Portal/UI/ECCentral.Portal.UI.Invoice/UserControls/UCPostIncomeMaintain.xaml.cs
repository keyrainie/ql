using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using ECCentral.BizEntity.Invoice;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.Portal.UI.Invoice.Facades;
using ECCentral.Portal.UI.Invoice.Models;
using ECCentral.Portal.UI.Invoice.Utility;
using Newegg.Oversea.Silverlight.Controls.Components;
using Newegg.Oversea.Silverlight.Utilities.Validation;

namespace ECCentral.Portal.UI.Invoice.UserControls
{
    public partial class UCPostIncomeMaintain : PopWindow
    {
        public enum MaintanMode
        {
            Create,
            Modify,
            View
        }

        private PostIncomeFacade postIncomeFacade;
        private OtherDomainDataFacade otherFacade;
        private PostIncomeVM currentPostIncome;
        private MaintanMode currentMode;

        public UCPostIncomeMaintain()
        {
            InitializeComponent();

            currentMode = MaintanMode.Create;
            Loaded += new RoutedEventHandler(UCPostIncomeMaintain_Loaded);
        }

        public UCPostIncomeMaintain(PostIncomeVM postIncomeVM, MaintanMode maintainMode)
            : this()
        {
            this.currentMode = maintainMode;
            this.currentPostIncome = postIncomeVM;
        }

        private void UCPostIncomeMaintain_Loaded(object sender, RoutedEventArgs e)
        {
            Loaded -= new RoutedEventHandler(UCPostIncomeMaintain_Loaded);
            InitData();
            SetControlAvailably();
            currentPostIncome.ValidationErrors.Clear();
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            var flag1 = ValidationManager.Validate(this.BaseInfo);
            if (flag1)
            {
                if (currentMode == MaintanMode.Create)
                {
                    postIncomeFacade.Create(currentPostIncome, () => CloseDialog(DialogResultType.OK));
                }
                else if (currentMode == MaintanMode.Modify)
                {
                    var soSysNoList = currentPostIncome.ConfirmedSOSysNoList
                        .Split(new char[] { '.' }, StringSplitOptions.RemoveEmptyEntries)
                        .ToList();
                    if (ValidConfirmOrders(soSysNoList) && flag1)
                    {
                        currentPostIncome.ValidationErrors.Clear();
                        postIncomeFacade.Update(currentPostIncome, () => CloseDialog(DialogResultType.OK));
                    }
                }
            }
        }

        private void InitData()
        {
            postIncomeFacade = new PostIncomeFacade(CurrentPage);
            otherFacade = new OtherDomainDataFacade(CurrentPage);

            if (currentMode == MaintanMode.Create)
            {
                currentPostIncome = new PostIncomeVM();
            }
            this.BaseInfo.DataContext = currentPostIncome;
        }

        private void SetControlAvailably()
        {
            switch (currentMode)
            {
                case MaintanMode.Create:
                    this.BaseInfo.SetChildControlAvailably(false, new List<UIElement>
                            {
                                tbSOSysNo,tbIncomeAmt,tbPayUser,dpIncomeDate,tbPayBank,tbIncomeBank
                            });
                    break;

                case MaintanMode.Modify:
                    //非已处理状态
                    if (currentPostIncome.HandleStatus == PostIncomeHandleStatus.WaitingHandle)
                    {
                        if (currentPostIncome.ConfirmStatus == PostIncomeStatus.Confirmed)
                        {
                            //待处理
                            this.BaseInfo.SetChildControlAvailably(false, new List<UIElement>
                                {
                                    tbCSNotes,tbCSConfirmedSO,cbIsHandled
                                });
                        }
                        else
                        {
                            //待确认
                            this.BaseInfo.SetChildControlAvailably(false, new List<UIElement>
                                {
                                    tbSOSysNo,tbIncomeAmt,tbPayUser,dpIncomeDate,tbPayBank,tbIncomeBank,tbBankNo,tbNotes
                                });
                        }
                    }
                    break;

                case MaintanMode.View:
                    this.BaseInfo.SetChildControlAvailably(false);
                    this.btnSave.Visibility = Visibility.Collapsed;
                    break;
            }
        }

        private bool ValidConfirmOrders(List<string> soSysNoList)
        {
            var duplicateSysNoS = soSysNoList.GroupBy(g => g)
                .Where(w => w.Count() > 1)
                .Select(s => s.Key);

            if (duplicateSysNoS.Count() > 0)
            {
                currentPostIncome.ValidationErrors.Add(new System.ComponentModel.DataAnnotations.ValidationResult(
                    string.Format(ECCentral.Portal.UI.Invoice.Resources.ResUCPostIncomeMaintain.Message_DuplicateSysNo,
                        string.Join(".", duplicateSysNoS.ToArray())), new string[] { "ConfirmedSOSysNoList" }));

                this.Dispatcher.BeginInvoke(() => tbCSConfirmedSO.Focus());
                return false;
            }
            return true;
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            CloseDialog(DialogResultType.Cancel);
        }
    }
}