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
using System.Windows.Navigation;
using Newegg.Oversea.Silverlight.Controls;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using ECCentral.Portal.UI.PO.Models.PurchaseOrder;
using ECCentral.BizEntity.PO.PurchaseOrder;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.BizEntity.PO;
using ECCentral.Portal.UI.PO.Resources;
using Newegg.Oversea.Silverlight.Controls.Components;

namespace ECCentral.Portal.UI.PO.Views
{
    /// <summary>
    /// 改类暂时没用了~
    /// </summary>
    [View(IsSingleton = true, NeedAccess = false, SingletonType = SingletonTypes.Url)]
    public partial class DeductMaintain : PageBase
    {
        private string getLoadDeductSysNo;
        public DeductFacade deductFacade;
        public DeductQueryVM deductVm;
        public DeductMaintain()
        {
            InitializeComponent();
        }

        public override void OnPageLoad(object sender, EventArgs e)
        {
            base.OnPageLoad(sender, e);
            deductVm = new DeductQueryVM();
            deductFacade = new DeductFacade(this);
            getLoadDeductSysNo = this.Request.Param;

            if (null != getLoadDeductSysNo)
            {
                LoadDeductInfo();
            }
        }

        //更新扣款项信息
        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            string name = this.txtName.Text.Trim();

            //扣款名称验证（必填，长度不超过200）
            if (string.IsNullOrEmpty(name))
            {
                Window.Alert(ResDeductQuery.AlertMsg_NameRequired);
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
                    deductVm.Name = name;
                }
            }

            deductVm.DeductType = this.rdbContract.IsChecked == true ? DeductType.Contract : DeductType.Temp;
            deductVm.AccountType = this.rdbFee.IsChecked == true ? AccountType.Fee : AccountType.Cost;
            deductVm.DeductMethod = this.rdbUnCash.IsChecked == true ? DeductMethod.UnCash : DeductMethod.Cash;


            Deduct oprDeductInfo = EntityConverter<DeductQueryVM, Deduct>.Convert(deductVm);

            if (null != getLoadDeductSysNo)
            {
                Window.Confirm(ResDeductQuery.InfoMsg_ConfirmModify, (obj, args) =>
                {
                    if (args.DialogResult == DialogResultType.OK)
                    {
                        //更新
                        deductFacade.UpdateDeductInfo(oprDeductInfo, (obj2, args2) =>
                        {
                            if (args2.FaultsHandle())
                            {
                                return;
                            }

                            Window.Alert(ResDeductQuery.AlertMsg_UpdateSuc);
                            Window.Navigate("/ECCentral.Portal.UI.PO/DeductQuery");
                            return;
                        });
                    }
                });
            }
            else
            {
                //新增
                deductFacade.AddDeductInfo(deductVm, (obj3, args3) =>
                {
                    if (args3.FaultsHandle())
                    {
                        return;
                    }

                    Window.Alert(ResDeductQuery.AlertMsg_CreateSuc);
                    Window.Navigate("/ECCentral.Portal.UI.PO/DeductQuery");
                    return;
                });
            }


        }

        /// <summary>
        /// 加载扣款项维护信息
        /// </summary>
        public void LoadDeductInfo()
        {
            deductFacade.GetSingleDeductBySysNo(getLoadDeductSysNo, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    Window.Alert(args.Error.Faults[0].ErrorDescription);
                    return;
                }

                deductVm = EntityConverter<Deduct, DeductQueryVM>.Convert(args.Result, (s, t) =>
                {
                    t = new DeductQueryVM
                    {
                        Name = s.Name,
                        DeductType = s.DeductType,
                        AccountType = s.AccountType,
                        DeductMethod = s.DeductMethod,
                    };
                });

                SetRdbtnIsSelected();
                this.DataContext = deductVm;
            });
        }

        //设置单选按钮
        public void SetRdbtnIsSelected()
        {
            //扣款项目类型
            if (deductVm.DeductType == DeductType.Contract)
            {
                this.rdbContract.IsChecked = true;
            }
            else
            {
                this.rdbTemp.IsChecked = true;
            }

            //记成本/费用
            if (deductVm.AccountType == AccountType.Fee)
            {
                this.rdbFee.IsChecked = true;
            }
            else
            {
                this.rdbCost.IsChecked = true;
            }

            //扣款方式
            if (deductVm.DeductMethod == DeductMethod.UnCash)
            {
                this.rdbUnCash.IsChecked = true;
            }
            else
            {
                this.rdbCash.IsChecked = true;
            }
        }
    }
}
