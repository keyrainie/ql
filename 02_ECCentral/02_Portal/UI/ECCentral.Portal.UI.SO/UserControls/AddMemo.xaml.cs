using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

using ECCentral.BizEntity.Common;
using ECCentral.BizEntity.SO;
using ECCentral.Portal.Basic;
using ECCentral.Portal.Basic.Components.UserControls.ReasonCodePicker;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.Portal.UI.SO.Facades;
using ECCentral.Portal.UI.SO.Models;
using ECCentral.Portal.UI.SO.Resources;

using Newegg.Oversea.Silverlight.ControlPanel.Core;
using Newegg.Oversea.Silverlight.Controls.Components;
using Newegg.Oversea.Silverlight.Utilities.Validation;

namespace ECCentral.Portal.UI.SO.UserControls
{
    public partial class AddMemo : UserControl
    {

        public int SOSysNo { get; set; }

        /// <summary>
        /// 刷新日志
        /// </summary>
        public Action RefreshLog;

        /// <summary>
        /// 刷新投诉
        /// </summary>
        public Action RefreshComplian;

        public AddMemo()
        {
            InitializeComponent();
            Loaded += new RoutedEventHandler(AddMemo_Loaded);
        }

        void AddMemo_Loaded(object sender, RoutedEventArgs e)
        {
            addContext.DataContext = new SOInternalMemoInfoVM() { SOSysNo = SOSysNo };
            Ini();
        }

        private void Ini()
        {
            cmbStatus.ItemsSource = EnumConverter.GetKeyValuePairs<SOInternalMemoStatus>();
            CodeNamePairHelper.GetList(ConstValue.DomainName_SO, new string[] { ConstValue.Key_CallBackDegree, ConstValue.Key_SOInernalMemoSource, ConstValue.Key_ComplainType }, (sender, e) =>
            {
                if (!e.FaultsHandle())
                {
                    cmbImportanceDegree.ItemsSource = e.Result[ConstValue.Key_CallBackDegree];
                    cmbComplainType.ItemsSource = e.Result[ConstValue.Key_ComplainType];
                    cmbSource.ItemsSource = e.Result[ConstValue.Key_SOInernalMemoSource];
                    cmbStatus.SelectedIndex = cmbImportanceDegree.SelectedIndex = cmbComplainType.SelectedIndex = cmbSource.SelectedIndex = 0;
                }
            });
        }

        private void SelectPath_Click(object sender, RoutedEventArgs e)
        {
            UCReasonCodePicker uc = new UCReasonCodePicker();
            uc.ReasonCodeType = ReasonCodeType.Order;
            uc.ShowType = ReasonCodeTreeShowType.Active;
            IDialog dialog = CPApplication.Current.CurrentPage.Context.Window.ShowDialog(ResSOInternalMemo.hlb_SlelectReasonCode, uc, (obj, args) =>
            {
                if (args.DialogResult == DialogResultType.OK)
                {
                    KeyValuePair<string, string> item = (KeyValuePair<string, string>)args.Data;
                    if (item.Key.Length > 0)
                    {
                        txtReasonCode.Text = item.Key;
                        txtReasonCodePath.Text = item.Value;
                    }
                }
            });
            uc.Dialog = dialog;
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            var vm = addContext.DataContext as SOInternalMemoInfoVM;
            //验证控件输入
            ValidationManager.Validate(addContext);
            if (vm.ValidationErrors.Count > 0) return;

            var req = vm.ConvertVM<SOInternalMemoInfoVM, SOInternalMemoInfo>();
            req.LogTime = DateTime.Now;
            req.RemindTime_Date = dpDate.SelectedDate;
            req.RemainTime_Time = dpTime.Value;

            (new SOInternalMemoFacade(CPApplication.Current.CurrentPage)).Create(req, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }
                else
                {
                    //判断是否添加投诉
                    //添加新的投诉单
                    if (cbAddComplain.IsChecked.Value)
                    {
                        //获取时间需要异步
                        DateTimeHelper.GetServerTimeNow(ConstValue.DomainName_SO, p => {
                            var newComplain = new SOComplaintCotentInfo()
                            {
                                SOSysNo = req.SOSysNo,
                                ComplainType = cmbComplainType.SelectedValue.ToString(),
                                //类型转换为电话(默认)
                                Subject = string.Format(ResSOInternalMemo.Msg_ComplainSubject, req.SOSysNo, p),
                                ComplainContent = txtNote.Text, //内容和跟进日志内容相同
                            };
                            (new SOComplainFacade(CPApplication.Current.CurrentPage)).Create(newComplain, (cobj, cargs) =>
                            {
                                if (cargs.FaultsHandle())
                                {
                                    //创建失败
                                    return;
                                }
                                if (RefreshComplian != null)
                                {
                                    RefreshComplian();
                                }
                            });
                        });
                    }

                    CPApplication.Current.CurrentPage.Context.Window.Alert(ResSOInternalMemo.Msg_publicMemoAddNewInfo);
                    if (RefreshLog != null)
                    {
                        RefreshLog();
                    }
                }
            });
        }
    }
}
