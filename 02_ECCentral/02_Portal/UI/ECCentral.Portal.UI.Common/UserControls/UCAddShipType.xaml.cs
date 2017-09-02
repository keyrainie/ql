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
using Newegg.Oversea.Silverlight.Utilities.Validation;
using Newegg.Oversea.Silverlight.Controls.Components;
using ECCentral.Portal.UI.Common.Models;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.BizEntity.Common;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using ECCentral.Portal.UI.Common.Facades;
using Newegg.Oversea.Silverlight.Controls;

namespace ECCentral.Portal.UI.Common.UserControls
{
    public partial class UCAddShipType : UserControl
    {
        public IDialog Dialog { get; set; }
        public IPage Page { get; set; } 
        private ShipTypeInfoVM VM;
        private ShipTypeFacade facade;
        public int? _sysNo;
        public string _channelId = string.Empty;

        public UCAddShipType(int? sysNo,string channelId)
        {
            InitializeComponent();
            if (sysNo.HasValue&&sysNo.Value>0)
            {
                _sysNo = sysNo;
                _channelId = channelId;
            }
            Loaded += new RoutedEventHandler(UCAddShipType_Loaded);
        }

        private void UCAddShipType_Loaded(object sender, RoutedEventArgs e)
        {
            Loaded -= new RoutedEventHandler(UCAddShipType_Loaded);
            facade = new ShipTypeFacade(CPApplication.Current.CurrentPage);
            if (_sysNo.HasValue)
            {
                facade.LoadShipType(_sysNo, (obj, args) =>
                    {
                        VM = args.Result;
                        VM.ChannelID = _channelId;
                        if (!VM.DsSysNo.HasValue)
                        {
                            VM.DsSysNo = 1;
                        }
                        LayoutRoot.DataContext = VM;

                    });
            }
            else
            {
                VM = new ShipTypeInfoVM();
                LayoutRoot.DataContext = VM;
                comIsOnlineShow.IsEnabled = false;
                comDeliveryPromise.SelectedIndex = 0;
                comIsWithPackFee.SelectedIndex = 0;
                comIsOnlineShow.SelectedIndex = 0;
                comShipTypeEnum.SelectedIndex = 0;
                comShippingPackStyle.SelectedIndex = 0;
                cbDeliveryType.SelectedIndex = 0;
                comIsSpecified.SelectedIndex = 0;
                comStoreType.SelectedIndex = 0;
                VM.DsSysNo = 1;
            }
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (ValidationManager.Validate(this.LayoutRoot))
            {
                if (!_sysNo.HasValue)
                {
                    facade.CreateShipType(VM, (s, args) =>
                        {
                            if (args.FaultsHandle())
                            {
                                return;
                            }
                            Message("操作已成功！");
                            CloseDialog(new ResultEventArgs() { DialogResult = DialogResultType.OK });
                        });
                }
                else
                {
                    facade.UpdateShipType(VM, (s, args) =>
                        {
                            if (args.FaultsHandle())
                            {
                                return;
                            }
                            Message("操作已成功！");
                            CloseDialog(new ResultEventArgs(){ DialogResult=DialogResultType.OK});
                        });
                }
            }
        }


        private void btnReset_Click(object sender, RoutedEventArgs e)
        {
            VM = new ShipTypeInfoVM();
            LayoutRoot.DataContext = VM;
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            if (Dialog != null)
            {
                Dialog.ResultArgs.Data = null;
                Dialog.ResultArgs.DialogResult = DialogResultType.Cancel;
                Dialog.Close();
            }
        }

        #region UI事件
        private void comShipTypeEnum_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.VM.ShipTypeEnum == ShippingTypeEnum.SelfGetInCity)
            {
                shipType_Ex.Visibility = Visibility.Visible;
            }
            else
            {
                shipType_Ex.Visibility = Visibility.Collapsed;
            }
            txtPhone.Visibility = this.VM.ShipType_ExVisibility;
            txtEmail.Visibility = this.VM.ShipType_ExVisibility;
            UCArea.Visibility = this.VM.ShipType_ExVisibility; 
        }

        private void cbDeliveryType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            switch (this.VM.DeliveryType)
            {
                
                case ShipDeliveryType.NoAppointed:
                    DeliveryType.Visibility = Visibility.Visible; 
                    Availsection.IsEnabled = false;
                    IntervalDays.IsEnabled = false;
                    comAppointment4CombineSO.Visibility = Visibility.Collapsed;
                    break;
                case ShipDeliveryType.OneDayOnce:
                    DeliveryType.Visibility = Visibility.Visible; 
                    Availsection.IsEnabled = true;
                    Availsection.Text = "7";
                    IntervalDays.IsEnabled = true;
                    IntervalDays.Text = "0";
                    comAppointment4CombineSO.Visibility = Visibility.Visible;
                    break;
                case ShipDeliveryType.OneDayTwice:
                    DeliveryType.Visibility = Visibility.Visible; 
                    Availsection.IsEnabled = true;
                    Availsection.Text = "3";
                    IntervalDays.IsEnabled = false;
                    IntervalDays.Text = "";
                    comAppointment4CombineSO.Visibility = Visibility.Visible;
                    break;
                case ShipDeliveryType.EveryDay:
                    DeliveryType.Visibility = Visibility.Visible; 
                    Availsection.IsEnabled = true;
                    Availsection.Text = "";
                    IntervalDays.IsEnabled = false;
                    IntervalDays.Text = "";
                    comAppointment4CombineSO.Visibility = Visibility.Collapsed;
                    break;
                case ShipDeliveryType.OneDaySix:
                    DeliveryType.Visibility = Visibility.Visible; 
                    Availsection.IsEnabled = true;
                    Availsection.Text = "";
                    IntervalDays.IsEnabled = false;
                    IntervalDays.Text = "";
                    comAppointment4CombineSO.Visibility = Visibility.Collapsed;
                    break;

            }
                   

        }
        #endregion

        #region 辅助方法
        private void Message(string msg)
        {
            Page.Context.Window.Alert(msg, Newegg.Oversea.Silverlight.Controls.Components.MessageType.Information);
        }

        private void CloseDialog(ResultEventArgs args)
        {
            if (Dialog != null)
            {
                Dialog.ResultArgs = args;
                Dialog.Close();
            }
        }
        #endregion


    }
}
