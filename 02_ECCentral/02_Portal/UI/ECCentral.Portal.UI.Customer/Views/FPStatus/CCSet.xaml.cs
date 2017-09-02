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
using ECCentral.Portal.UI.Customer.Models;
using ECCentral.Portal.UI.Customer.Facades;
using ECCentral.Portal.Basic.Utilities;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using ECCentral.Service.Customer.Restful.RequestMsg;
using ECCentral.Portal.UI.Customer.Resources;
using ECCentral.Portal.Basic;

namespace ECCentral.Portal.UI.Customer.Views.FPStatus
{
    [View]
    public partial class CCSet : PageBase
    {
        private CCSetVM viewModel;
        private FPCheckFacade fpCheckFacade;
        private string WebChannelID;
        private bool[] EnableArr = new bool[] { false, false, true, true, true, false };
        private List<CCSetVM> orginList = new List<CCSetVM>();
        public CCSet()
        {
            viewModel = new CCSetVM();
            InitializeComponent();
        }
        public override void OnPageLoad(object sender, EventArgs e)
        {
            fpCheckFacade = new FPCheckFacade(this);
            WebChannelID = Request.Param;
            gridCCList.Bind();
            base.OnPageLoad(sender, e);
            if (!AuthMgr.HasFunctionPoint(AuthKeyConst.Customer_CCSet_Save))
                this.ButtonSave.IsEnabled = false;
        }
        private void ButtonSave_Click(object sender, RoutedEventArgs e)
        {
            List<CCSetReq> list = new List<CCSetReq>();
            (gridCCList.ItemsSource as List<CCSetVM>).ForEach(item =>
            {
                if (orginList.Where(p => p.SysNo == item.SysNo && p.Params == item.Params && p.Status == item.Status).Count() <= 0)
                    list.Add(new CCSetReq() { SysNo = item.SysNo.Value, Params = item.Params, Status = item.Status.Value });
            });

            fpCheckFacade.UpdateETC(list, (obj, args) =>
           {
               if (args.FaultsHandle())
                   return;
               Window.Alert(ResFPCheck.msg_SaveSuccess);
               gridCCList.Bind();
           });
        }
        private void DataGrid_LoadingDataSource(object sender, Newegg.Oversea.Silverlight.Controls.Data.LoadingDataEventArgs e)
        {
            fpCheckFacade.QueryETC(WebChannelID, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }
                List<CCSetVM> list = DynamicConverter<CCSetVM>.ConvertToVMList(args.Result.Rows);
                for (int i = 0; i < list.Count; i++)
                {
                    list[i].Enable = EnableArr[i];
                }
                orginList = list.DeepCopy();
                gridCCList.ItemsSource = list;
                gridCCList.TotalCount = args.Result.TotalCount;
            });
        }


    }

}
