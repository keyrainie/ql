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
using Newegg.Oversea.Silverlight.Controls;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using ECCentral.QueryFilter.PO;
using ECCentral.Portal.UI.PO.Facades;
using ECCentral.Portal.UI.PO.Models;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.Service.PO.Restful.RequestMsg;

namespace ECCentral.Portal.UI.PO.UserControls
{
    public partial class VendorHoldPM : UserControl
    {
        public IDialog Dialog { get; set; }
        public VendorFacade serviceFacade;
        public List<VendorHoldPMInfoVM> CanHoldPMList;
        public List<VendorHoldPMInfoVM> HoldedPMList;
        public int VendorSysNo;
        public string Reason;
        public IWindow CurrentWindow
        {
            get
            {
                return CPApplication.Current.CurrentPage.Context.Window;
            }
        }
        public VendorHoldPM(int vendorSysNo, string reason)
        {
            InitializeComponent();
            VendorSysNo = vendorSysNo;
            Reason = reason;
            HoldedPMList = new List<VendorHoldPMInfoVM>();
            CanHoldPMList = new List<VendorHoldPMInfoVM>();
            this.Loaded += new RoutedEventHandler(VendorHoldPM_Loaded);
        }

        void VendorHoldPM_Loaded(object sender, RoutedEventArgs e)
        {
            this.Loaded -= VendorHoldPM_Loaded;
            serviceFacade = new VendorFacade(CPApplication.Current.CurrentPage);

            BindPMList();
        }

        private void BindPMList()
        {
            VendorQueryFilter filter = new VendorQueryFilter() { VendorSysNo = this.VendorSysNo };
            //1. 获取Vendor下可以锁定的PM列表
            serviceFacade.QueryCanLockPMListVendorSysNo(filter, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }
                CanHoldPMList = DynamicConverter<VendorHoldPMInfoVM>.ConvertToVMList(args.Result.Rows);
                this.gridPMList.Bind();

                //2.获取Vendor下已经锁定的 PM并勾选:
                serviceFacade.QueryVendorPMHoldInfoByVendorSysNo(filter, (obj2, args2) =>
                {
                    if (args2.FaultsHandle())
                    {
                        return;
                    }
                    HoldedPMList = DynamicConverter<VendorHoldPMInfoVM>.ConvertToVMList(args2.Result.Rows);
                    HoldedPMList = HoldedPMList.Where(x => x.IsChecked == true).ToList();
                    HoldedPMList.ForEach(x =>
                    {
                        VendorHoldPMInfoVM vm = CanHoldPMList.SingleOrDefault(g => g.PMSysNo == x.PMSysNo.Value);
                        if (null != vm)
                        {
                            vm.IsChecked = true;
                        }
                    });

                });
            });
        }

        private void ckbSelectRow_Click(object sender, RoutedEventArgs e)
        {
            CheckBox chk = sender as CheckBox;
            if (null != chk)
            {
                if (null != this.gridPMList.ItemsSource)
                {
                    foreach (var item in this.gridPMList.ItemsSource)
                    {
                        if (item is VendorHoldPMInfoVM)
                        {
                            if (chk.IsChecked == true)
                            {
                                if (!((VendorHoldPMInfoVM)item).IsChecked)
                                {
                                    ((VendorHoldPMInfoVM)item).IsChecked = true;
                                }
                            }
                            else
                            {
                                if (((VendorHoldPMInfoVM)item).IsChecked)
                                {
                                    ((VendorHoldPMInfoVM)item).IsChecked = false;
                                }
                            }

                        }
                    }
                }
            }
        }
        private void gridPMList_LoadingDataSource(object sender, Newegg.Oversea.Silverlight.Controls.Data.LoadingDataEventArgs e)
        {
            this.gridPMList.ItemsSource = CanHoldPMList;
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            //取消操作:
            Dialog.ResultArgs.Data = null;
            Dialog.ResultArgs.DialogResult = DialogResultType.Cancel;
            Dialog.Close(true);
        }

        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            //if (CanHoldPMList.Where(x => x.IsChecked == true).Count() <= 0)
            //{
            //    CurrentWindow.Alert("请先选择一个要锁定的PM！");
            //    return;
            //}
            if (CanHoldPMList.Count > 0)
            {
                VendorHoldPMReq requsetMsg = new VendorHoldPMReq();
                List<int> holdList = new List<int>();
                List<int> unHoldList = new List<int>();


                this.CanHoldPMList.ForEach(x =>
                {
                    if (x.IsChecked == true)
                    {
                        holdList.Add(x.PMSysNo.Value);
                    }
                    else
                    {
                        unHoldList.Add(x.PMSysNo.Value);
                    }

                });
                List<int> intersectList = holdList.Intersect(unHoldList).ToList();
                holdList = holdList.Except(intersectList).ToList();
                unHoldList = unHoldList.Except(intersectList).ToList();
                requsetMsg.HoldSysNoList = holdList;
                requsetMsg.UnHoldSysNoList = unHoldList;
                requsetMsg.VendorSysNo = VendorSysNo;
                requsetMsg.HoldUserSysNo = CPApplication.Current.LoginUser.UserSysNo.Value;
                requsetMsg.Reason = Reason;

                serviceFacade.HoldOrUnholdVendorPM(requsetMsg, (obj, args) =>
                {
                    if (args.FaultsHandle())
                    {
                        return;
                    }
                    //确认操作 ：
                    Dialog.ResultArgs.DialogResult = DialogResultType.OK;

                    string alertString = string.Empty;

                    if (holdList.Count > 0)
                    {
                        alertString += string.Format("成功锁定PM : {1}，并成功锁定付款单:{0}条.", args.Result[0], string.Join(",", holdList));
                    }
                    if (unHoldList.Count > 0)
                    {
                        if (!string.IsNullOrEmpty(alertString))
                        {
                            alertString += Environment.NewLine;
                        }
                        alertString += string.Format("成功解除锁定PM: {1}，并成功解除锁定付款单:{0}条.", args.Result[1], string.Join(",", unHoldList));

                    }


                    Dialog.ResultArgs.Data = alertString;
                    Dialog.Close(true);
                });
            }
            else
            {
                Dialog.ResultArgs.Data = null;
                Dialog.Close(true);
            }

        }
    }
}
