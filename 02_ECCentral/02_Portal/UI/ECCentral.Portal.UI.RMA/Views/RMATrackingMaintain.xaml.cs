using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

using ECCentral.Portal.Basic;
using ECCentral.Portal.Basic.Components.Facades;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.Portal.UI.RMA.Facades;
using ECCentral.Portal.UI.RMA.Models;
using ECCentral.Portal.UI.RMA.Resources;
using ECCentral.Portal.UI.RMA.UserControls;

using Newegg.Oversea.Silverlight.ControlPanel.Core;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using Newegg.Oversea.Silverlight.Controls;
using Newegg.Oversea.Silverlight.Controls.Components;
using Newegg.Oversea.Silverlight.Utilities.Validation;

namespace ECCentral.Portal.UI.RMA.Views
{
    [View(IsSingleton = true, NeedAccess = false, SingletonType = SingletonTypes.Url)]
    public partial class RMATrackingMaintain : PageBase
    {
        private RMATrackingFacade facade;
        private List<RMATrackingVM> list;
        List<ValidationEntity> validationCondition;
        private RMATrackingQueryVM queryVM;
        private RMATrackingQueryVM lastQueryVM;
        public RMATrackingMaintain()
        {
            InitializeComponent();
        }
        public override void OnPageLoad(object sender, EventArgs e)
        {
            base.OnPageLoad(sender, e);
            facade = new RMATrackingFacade(this);
            this.QueryFilter.DataContext = queryVM = new RMATrackingQueryVM();

            int registerSysNo;
            string RegisterSysNo = this.Request.Param;
            if (!string.IsNullOrEmpty(RegisterSysNo)
                && int.TryParse(RegisterSysNo, out registerSysNo))
            {
                TextBox_RegisterSysNo.Text = RegisterSysNo;
                queryVM.RegisterSysNo = RegisterSysNo;
                lastQueryVM = Newegg.Oversea.Silverlight.Utilities.UtilityHelper.DeepClone<RMATrackingQueryVM>(queryVM);

                this.DataGrid_Query_ResultList.Bind();
            }
            else
            {
                if (RegisterSysNo != null)
                    Window.Alert(ResRMATracking.Msg_RegisterSysNoError, ResRMATracking.Msg_RegisterSysNoError, MessageType.Warning, (obj, args) =>
                    {
                        Window.Close();
                    });

            }
            BuildValidateCondition();
            SetAccessControl();
        }

        private void SetAccessControl()
        {
            //权限控制:
            Button_Creat.IsEnabled = AuthMgr.HasFunctionPoint(AuthKeyConst.RMA_RMATracking_Edit_CanAdd);
            Button_Close.IsEnabled = AuthMgr.HasFunctionPoint(AuthKeyConst.RMA_RMATracking_Edit_CanClose);
        }

        private void BuildValidateCondition()
        {
            validationCondition = new List<ValidationEntity>();
            validationCondition.Add(new ValidationEntity(ValidationEnum.IsNotEmpty, this.TextBox_RegisterSysNo.Text.Trim(), ResRMATracking.Msg_NoCondition));
            validationCondition.Add(new ValidationEntity(ValidationEnum.IsInteger, this.TextBox_RegisterSysNo.Text.Trim(), ResRMATracking.Msg_IsInteger));
        }

        private void DataGrid_Query_ResultList_LoadingDataSource(object sender, Newegg.Oversea.Silverlight.Controls.Data.LoadingDataEventArgs e)
        {
            facade.Query(lastQueryVM, e.PageSize, e.PageIndex, e.SortField, (obj, args) =>
            {
                list = DynamicConverter<RMATrackingVM>.ConvertToVMList(args.Result.Rows);
                this.DataGrid_Query_ResultList.ItemsSource = list;
                this.DataGrid_Query_ResultList.TotalCount = args.Result.TotalCount;
            });
        }

        private void Button_Creat_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(this.Request.Param))
            {
                this.Window.Alert(ResRMATracking.Msg_NoData);
                return;
            }
            UCCreateRMATracking uc = new UCCreateRMATracking();
            CodeNamePairHelper.GetList(ConstValue.DomainName_RMA, "RMAInternalMemoSourceType", (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }
                RMATrackingVM vm = new RMATrackingVM();
                vm.RegisterSysNo = int.Parse(this.Request.Param);
                vm.publicMemoSourceTypes = args.Result;
                uc.DataContext = vm;

            });
            IDialog dialog = CPApplication.Current.CurrentPage.Context.Window.ShowDialog(ResRMATracking.Dialog_CreateRMATracking, uc, (obj, args) =>
            {
                if (args.DialogResult == DialogResultType.OK)
                {
                    this.DataGrid_Query_ResultList.Bind();
                }
            });
            uc.Dialog = dialog;
        }

        private void Button_Close_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidateSelectRow())
            {
                this.Window.Alert(ResRMATracking.Msg_SelectItem);
                return;
            }
            RMATrackingVM vm = this.DataGrid_Query_ResultList.SelectedItem as RMATrackingVM;
            UCCloseRMATracking uctlClose = new UCCloseRMATracking();
            uctlClose.DataContext = vm;
            uctlClose.Dialog = Window.ShowDialog(ResRMATracking.Dialog_CloseRMATracking, uctlClose, (obj, args) =>
            {
                if (args.DialogResult == DialogResultType.OK)
                {
                    this.DataGrid_Query_ResultList.Bind();
                }
            });
        }

        private void Button_Search_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidateCondition())
            {
                return;
            }
                lastQueryVM = Newegg.Oversea.Silverlight.Utilities.UtilityHelper.DeepClone<RMATrackingQueryVM>(queryVM);

                string url = string.Format(ConstValue.RMA_TrackingMaintainUrl, TextBox_RegisterSysNo.Text.Trim());
                Window.Navigate(url, null, false);
        }

        private bool ValidateCondition()
        {
            bool ret = true;

            if (!ValidationHelper.Validation(this.TextBox_RegisterSysNo, validationCondition))
            {
                ret = false;
            }
            return ret;
        }

        private bool ValidateSelectRow()
        {
            bool ret = true;
            if (list == null)
            {
                ret = false;
            }
            else
            {
                var selectedList = list.Where(item => item.IsChecked).ToList();
                if (selectedList.Count == 0)
                {
                    ret = false;
                }
            }
            return ret;
        }
        private void Grid_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Enter)
                return;
            var textBox = e.OriginalSource as TextBox;
            if (textBox != null)
            {
                var txtBinding = textBox.GetBindingExpression(TextBox.TextProperty);
                if (txtBinding != null)
                {
                    txtBinding.UpdateSource();
                    this.DataGrid_Query_ResultList.Bind();
                }
            }
        }

    }
}
