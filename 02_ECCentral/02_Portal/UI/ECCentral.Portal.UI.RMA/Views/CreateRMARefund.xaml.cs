using System.Linq;
using System.Collections.Generic;
using System.Windows;

using ECCentral.BizEntity.Enum.Resources;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.Portal.UI.RMA.Facades;
using ECCentral.Portal.UI.RMA.Models;
using ECCentral.Portal.UI.RMA.UserControls;
using ECCentral.Portal.UI.RMA.Resources;

using Newegg.Oversea.Silverlight.ControlPanel.Core;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using Newegg.Oversea.Silverlight.Controls;
using Newegg.Oversea.Silverlight.Controls.Components;
using ECCentral.Portal.Basic;


namespace ECCentral.Portal.UI.RMA.Views
{
    [View(IsSingleton = true, SingletonType = SingletonTypes.Url)]
    public partial class CreateRMARefund : PageBase
    {
        private int registerSysNo;

        public CreateRMARefund()
        {
            InitializeComponent();
        }

        public override void OnPageLoad(object sender, System.EventArgs e)
        {
            base.OnPageLoad(sender, e);

            string no = Request.Param;
            if (!string.IsNullOrEmpty(no))
            {
                int.TryParse(no, out registerSysNo);
            }

            new RefundFacade(this).GetWaitingSOForRefund((obj, args) =>
            {
                List<KeyValuePair<int?, string>> list = new List<KeyValuePair<int?, string>>();
                if (args.Result != null)
                {
                    args.Result.ForEach(p =>
                    {
                        var item = new KeyValuePair<int?, string>(p, p.ToString());
                        list.Add(item);
                    });
                    list.Insert(0, new KeyValuePair<int?, string>(null, ResCommonEnum.Enum_All));
                }
                this.cmbSOList.ItemsSource = list;
                this.cmbSOList.SelectedIndex = 0;
            });

            SearchWaitingRegisters(null);
            SetAccessControl();
        }

        //权限控制:
        private void SetAccessControl()
        {
            btnNew.IsEnabled = AuthMgr.HasFunctionPoint(AuthKeyConst.RMA_Refund_New);
        }

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            int? soSysNo = (int?)this.cmbSOList.SelectedValue;
            SearchWaitingRegisters(soSysNo);
        }

        private void SearchWaitingRegisters(int? soSysNo)
        {            
            new RefundFacade(this).GetWaitingRegisters(soSysNo, (obj, args) =>
            {
                List<RegisterBasicVM> list = DynamicConverter<RegisterBasicVM>.ConvertToVMList(args.Result.Rows);
                List<RegisterVM> registers = new List<RegisterVM>();
                list.ForEach(p =>
                {                   
                    registers.Add(new RegisterVM { BasicInfo = p });
                });

                registers.ForEach(p =>
                {
                    p.IsChecked = p.BasicInfo.SysNo == this.registerSysNo;
                });

                this.dataRegisterList.ItemsSource = registers;
                this.dataRegisterList.TotalCount = args.Result.TotalCount;
            });
        }

        private void btnNew_Click(object sender, RoutedEventArgs e)
        {
            List<RegisterVM> list = this.dataRegisterList.ItemsSource as List<RegisterVM>;
            if (list.Count(p => p.IsChecked) == 0)
            {
                Window.Alert(ResCreateRefund.Warning_NoItemSelected, MessageType.Warning);
                return;
            }
            UCNewRMARefund uc = new UCNewRMARefund();
            uc.Registers = list.Where(p => p.IsChecked).ToList();
            IDialog dialog = CPApplication.Current.CurrentPage.Context.Window.ShowDialog(ResCreateRefund.PopTitle_CreateRefund, uc, (obj, args) =>
            {
                if (args.Data != null)
                {
                    bool result = (bool)args.Data;
                    if (result)
                    {
                        btnSearch_Click(this, null);
                    }
                }
            });
            uc.Dialog = dialog;
        }
    }
}