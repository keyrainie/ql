using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using ECCentral.BizEntity.Invoice;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.Portal.UI.Invoice.Facades;
using ECCentral.Portal.UI.Invoice.Models;
using ECCentral.Portal.UI.Invoice.Resources;
using ECCentral.QueryFilter.Invoice;
using Newegg.Oversea.Silverlight.Controls.Components;
using Newegg.Oversea.Silverlight.Utilities.Validation;

namespace ECCentral.Portal.UI.Invoice.UserControls
{
    public partial class UCResponsibleUserCfg : PopWindow
    {
        public ResponsibleUserVM responsibleUser = new ResponsibleUserVM();
        private ARWindowFacade facade = null;
        private bool needClearData = false;

        public UCResponsibleUserCfg()
        {
            InitializeComponent();
            Loaded += new System.Windows.RoutedEventHandler(UCResponsibleUserCfg_Loaded);
        }

        public void UCResponsibleUserCfg_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            Loaded -= new System.Windows.RoutedEventHandler(UCResponsibleUserCfg_Loaded);

            needClearData = false;
            facade = new ARWindowFacade(this.CurrentPage);
            this.LayoutRoot.DataContext = responsibleUser;
            this.responsibleUser.ValidationErrors.Clear();

            needClearData = true;
        }

        public UCResponsibleUserCfg(ResponsibleUserVM userVM, ARWindowFacade facade)
            : this()
        {
            this.responsibleUser = userVM;
            this.facade = facade;
        }

        /// <summary>
        /// 配置
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnConfig_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            var flag = ValidationManager.Validate(this.LayoutRoot);
            if (flag)
            {
                if (!(!responsibleUser.ShipTypeSysNo.HasValue || string.IsNullOrEmpty(responsibleUser.CustomerSysNo)))
                {
                    responsibleUser.ValidationErrors.Add(new System.ComponentModel.DataAnnotations.ValidationResult(
                        ResUCResponsibleUserCfg.Message_ShipTypeOrCustomerSysNoOnlyOneDlgTitle
                        , new List<string>() { "ShipTypeSysNo", "CustomerSysNo" }));

                    return;
                }

                responsibleUser.CompanyCode = Newegg.Oversea.Silverlight.ControlPanel.Core.CPApplication.Current.CompanyCode;

                facade.ExistedResponsibleUser(responsibleUser, existed =>
                {
                    if (existed)
                    {
                        AlertConfirmDialog(ResUCResponsibleUserCfg.Message_ConfirmRecoverDlgTitle, (obj) =>
                        {
                            facade.CreateResponsibleUser(responsibleUser, true, () =>
                            {
                                CloseDialog(DialogResultType.OK);
                            });
                        });
                    }
                    else
                    {
                        facade.CreateResponsibleUser(responsibleUser, false, () =>
                        {
                            CloseDialog(DialogResultType.OK);
                        });
                    }
                });
            }
        }

        /// <summary>
        /// 重置
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnReset_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            responsibleUser = new ResponsibleUserVM()
            {
                CompanyCode = Newegg.Oversea.Silverlight.ControlPanel.Core.CPApplication.Current.CompanyCode
            };
            this.LayoutRoot.DataContext = responsibleUser;
        }

        private void Combox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (needClearData)
            {
                ClearData();
            }
            switch (responsibleUser.IncomeStyle)
            {
                case ResponseUserOrderStyle.Normal:
                    SetControl(false, true, false, true);
                    break;

                case ResponseUserOrderStyle.Advanced:
                    SetControl(true, false, true, false);
                    break;

                case ResponseUserOrderStyle.RefundException:
                    SetControl(false, false, false, false);
                    break;

                default:
                    SetControl(true, true, true, true);
                    break;
            }
        }

        private void ClearData()
        {
            responsibleUser.PayTypeSysNo = null;
            responsibleUser.ShipTypeSysNo = null;
            responsibleUser.SourceType = null;
            responsibleUser.CustomerSysNo = null;
        }

        private void SetControl(bool payTypeEnabled, bool shipTypeEnabled, bool sourceTypeEnabled, bool customerSysNoEnabled)
        {
            cmbPayType.IsEnabled = payTypeEnabled;
            cmbShipType.IsEnabled = shipTypeEnabled;
            cmbSourceType.IsEnabled = sourceTypeEnabled;
            tbCustomerSysNo.IsEnabled = customerSysNoEnabled;
        }
    }
}