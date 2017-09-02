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
using ECCentral.BizEntity.PO;
using ECCentral.BizEntity.PO.PurchaseOrder;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.Portal.UI.PO.Models.PurchaseOrder;
using ECCentral.Portal.UI.PO.Resources;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using Newegg.Oversea.Silverlight.Controls;
using Newegg.Oversea.Silverlight.Controls.Components;

namespace ECCentral.Portal.UI.PO.UserControls
{
    public partial class UCDeductMaintain : UserControl
    {
        public IDialog Dialog { get; set; }
        public DeductFacade serviceFacade;
        public DeductQueryVM deductVM;
        private string getLoadDeductSysNo;
        private object p;
        public IWindow CurrentWindow
        {
            get
            {
                return CPApplication.Current.CurrentPage.Context.Window;
            }
        }
        public UCDeductMaintain(string deductSysNo)
        {
            this.getLoadDeductSysNo = deductSysNo;
            InitializeComponent();
            deductVM = new DeductQueryVM();
            serviceFacade = new DeductFacade(CPApplication.Current.CurrentPage);
            this.Loaded += new RoutedEventHandler(UCDeductMaintain_Loaded);
        }

        void UCDeductMaintain_Loaded(object sender, RoutedEventArgs e)
        {
            if (null != getLoadDeductSysNo)
            {
                serviceFacade.GetSingleDeductBySysNo(getLoadDeductSysNo, (obj, args) =>
                {
                    if (args.FaultsHandle())
                    {
                        // Window.Alert(args.Error.Faults[0].ErrorDescription);
                        return;
                    }

                    deductVM = EntityConverter<Deduct, DeductQueryVM>.Convert(args.Result, (s, t) =>
                    {
                        t = new DeductQueryVM
                        {
                            Name = s.Name,
                            DeductType = s.DeductType,
                            AccountType = s.AccountType,
                            DeductMethod = s.DeductMethod,
                        };
                    });

                    if (this.deductVM.Status == Status.Invalid)
                    {
                        this.btnSave.IsEnabled = false;
                    }
                    else
                    {
                        this.btnSave.IsEnabled = true;
                    }

                    SetRdbtnIsSelected();

                    this.DataContext = deductVM;
                });
            }
        }

        //设置单选按钮
        public void SetRdbtnIsSelected()
        {
            //扣款项目类型
            if (deductVM.DeductType == DeductType.Contract)
            {
                this.rdbContract.IsChecked = true;
            }
            else
            {
                this.rdbTemp.IsChecked = true;
            }

            //记成本/费用
            if (deductVM.AccountType == AccountType.Fee)
            {
                this.rdbFee.IsChecked = true;
            }
            else
            {
                this.rdbCost.IsChecked = true;
            }

            //扣款方式
            if (deductVM.DeductMethod == DeductMethod.UnCash)
            {
                this.rdbUnCash.IsChecked = true;
            }
            else
            {
                this.rdbCash.IsChecked = true;
            }
        }

        //编辑&新增扣款项目
        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            string name = this.txtName.Text.Trim();

            //扣款名称验证（必填，长度不超过200）
            if (string.IsNullOrEmpty(name))
            {
                CurrentWindow.Alert(ResDeductQuery.AlertMsg_NameRequired);
                return;
            }
            else
            {
                if (name.Length > 200)
                {
                    return;
                }
                else
                {
                    deductVM.Name = name;
                }
            }

            deductVM.DeductType = this.rdbContract.IsChecked == true ? DeductType.Contract : DeductType.Temp;
            deductVM.AccountType = this.rdbFee.IsChecked == true ? AccountType.Fee : AccountType.Cost;
            deductVM.DeductMethod = this.rdbUnCash.IsChecked == true ? DeductMethod.UnCash : DeductMethod.Cash;


            Deduct oprDeductInfo = EntityConverter<DeductQueryVM, Deduct>.Convert(deductVM);

            if (null != getLoadDeductSysNo)
            {
                //CurrentWindow.Alert("温馨提示：",ResDeductQuery.InfoMsg_ConfirmModify,MessageType.Information,(obj4, args4) =>
                //{
                //    if (args4.DialogResult == DialogResultType.OK)
                //    {
                //更新
                serviceFacade.UpdateDeductInfo(oprDeductInfo, (obj2, args2) =>
                {
                    if (args2.FaultsHandle())
                    {
                        return;
                    }

                    CurrentWindow.Alert(ResDeductQuery.AlertMsg_UpdateSuc);
                    Dialog.ResultArgs.Data = null;
                    Dialog.ResultArgs.DialogResult = DialogResultType.OK;
                    Dialog.Close(true);
                });
                //    }
                //});
            }
            else
            {
                //新增
                serviceFacade.AddDeductInfo(deductVM, (obj3, args3) =>
                {
                    if (args3.FaultsHandle())
                    {
                        return;
                    }

                    CurrentWindow.Alert(ResDeductQuery.AlertMsg_CreateSuc);
                    Dialog.ResultArgs.Data = null;
                    Dialog.ResultArgs.DialogResult = DialogResultType.OK;
                    Dialog.Close(true);
                    //return;
                });
            }
        }
    }
}
