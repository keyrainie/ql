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
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.Portal.Basic;
using ECCentral.Portal.UI.SO.Facades;
using ECCentral.Portal.UI.SO.Models;
using ECCentral.Portal.UI.SO.Resources;
using ECCentral.BizEntity.SO;
using ECCentral.BizEntity.Common;
using Newegg.Oversea.Silverlight.Utilities.Validation;

namespace ECCentral.Portal.UI.SO.UserControls
{
    public partial class AddSOIntercept : UserControl
    {
        public IDialog Dialog
        {
            get;
            set;
        }

        private SOInterceptInfoVM m_SOInterceptInfoVM;
        public SOInterceptInfoVM CurrentSOInterceptInfoVM
        {
            get
            {
                return m_SOInterceptInfoVM;
            }
            private set
            {
                m_SOInterceptInfoVM = value;
                gdAddSOIntercept.DataContext = m_SOInterceptInfoVM;
            }
        }

        public AddSOIntercept()
        {
            InitializeComponent();
            CurrentSOInterceptInfoVM = new SOInterceptInfoVM();
            IniPageData();
        }

        private void IniPageData()
        {
            // 读取配置(ECCentral.Service.WebHost  --> SO.zh-cn.config) 初始化配送方式过滤 下拉列表
            CodeNamePairHelper.GetList(ConstValue.DomainName_SO
                            , new string[] { ConstValue.Key_ShipTypeFilter,ConstValue.Key_HasTrackingNumber, ConstValue.Key_ShipTimeType }
                            , CodeNamePairAppendItemType.None, (sender, e) =>
            {
                if (!e.FaultsHandle())
                {
                    cmbShipTypeEnum.ItemsSource = e.Result[ConstValue.Key_ShipTypeFilter];
                    cmbShipTypeEnum.SelectedIndex = 0;

                    cmbHasTrackingNumbe.ItemsSource = e.Result[ConstValue.Key_HasTrackingNumber];
                    cmbHasTrackingNumbe.SelectedIndex = 0;

                    cmbShipTimeType.ItemsSource = e.Result[ConstValue.Key_ShipTimeType];
                    cmbShipTimeType.SelectedIndex = 0;
                }
            });
        }

        /// <summary>
        /// 添加订单拦截设置
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            m_SOInterceptInfoVM = gdAddSOIntercept.DataContext as SOInterceptInfoVM;
            if (string.IsNullOrEmpty(m_SOInterceptInfoVM.ShippingTypeID)
                || string.IsNullOrEmpty(m_SOInterceptInfoVM.StockSysNo)
                || string.IsNullOrEmpty(m_SOInterceptInfoVM.HasTrackingNumber)
                || string.IsNullOrEmpty(m_SOInterceptInfoVM.ShipTimeType)
                || string.IsNullOrEmpty(m_SOInterceptInfoVM.EmailAddress)
                || string.IsNullOrEmpty(m_SOInterceptInfoVM.CCEmailAddress)
                || string.IsNullOrEmpty(m_SOInterceptInfoVM.FinanceEmailAddress)
                || string.IsNullOrEmpty(m_SOInterceptInfoVM.FinanceCCEmailAddress)
                )
            {
                CPApplication.Current.CurrentPage.Context.Window.Alert(ResSOIntercept.Info_SaveSOIntercept_Input_Error, MessageType.Error);
            }
            else 
            {
                ValidationManager.Validate(this.gdAddSOIntercept);
                if (m_SOInterceptInfoVM.HasValidationErrors && m_SOInterceptInfoVM.ValidationErrors.Count > 0)
                {
                    return;
                }
                m_SOInterceptInfoVM.ShipTypeSysNo = Convert.ToInt32(m_SOInterceptInfoVM.ShippingTypeID);
                SOInterceptInfo req = m_SOInterceptInfoVM.ConvertVM<SOInterceptInfoVM,SOInterceptInfo>();
                new SOInterceptFacade(CPApplication.Current.CurrentPage).AddSOInterceptInfo(req, (obj, args) =>
                {
                    if (!args.FaultsHandle())
                    {
                        CPApplication.Current.CurrentPage.Context.Window.Alert(ResSOIntercept.Info_AddSOIntercept_Success, MessageType.Information);
                        CloseDialog(new ResultEventArgs
                        {
                            DialogResult = DialogResultType.OK
                        });
                    }
                });
            }
        }

        private void cmbShipTypeEnum_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox cmbShipTypeEnum = (sender) as ComboBox;
            if (!string.IsNullOrEmpty(cmbShipTypeEnum.SelectedValue.ToString()))
            {
                string[] selectedEnum = new string[] { cmbShipTypeEnum.SelectedValue.ToString() };
                if (selectedEnum[0].Contains(","))
                {
                    selectedEnum = new string[2];
                    selectedEnum[0] = "1";
                    selectedEnum[1] = "2";
                }
                new SOQueryFacade().GetShipTypeList((obj, args) =>
                {
                    if (!args.FaultsHandle())
                    {
                        var list = args.Result.Where(item => selectedEnum.Contains(item.ShipTypeEnum.ToString()) && item.IsOnlineShow != HYNStatus.Hide);
                        if (list.Count() > 0)
                        {
                            cmbShipTypeSysNo.ItemsSource = list;
                            cmbShipTypeSysNo.SelectedIndex = 0;
                        }
                    }
                });
            }
            else
            {
                cmbShipTypeSysNo.ItemsSource = null;
            }
        }

        private void CloseDialog(ResultEventArgs args)
        {
            if (Dialog != null)
            {
                Dialog.ResultArgs = args;
                Dialog.Close();
            }
        }
    }
}
