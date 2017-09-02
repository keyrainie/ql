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
using ECCentral.Portal.UI.MKT.Models;
using ECCentral.Portal.UI.MKT.Facades;
using ECCentral.BizEntity.MKT;
using ECCentral.Portal.Basic.Utilities;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using ECCentral.Portal.UI.MKT.Resources;
using Newegg.Oversea.Silverlight.Controls.Components;

namespace ECCentral.Portal.UI.MKT.UserControls
{
    public partial class UCAddAdvertisers : UserControl
    {
        public IDialog Dialog { get; set; }
        public int SysNo
        {
            get;
            set;
        }
        private bool isAdd = true;
        private AdvertisersVM vm;
        private AdvertiserFacade facade;
        public UCAddAdvertisers()
        {
            InitializeComponent();
            Loaded += new RoutedEventHandler(UCAddAdvertisers_Loaded);
        }

        private void UCAddAdvertisers_Loaded(object sender, RoutedEventArgs e)
        {
            Loaded -= new RoutedEventHandler(UCAddAdvertisers_Loaded);
            facade = new AdvertiserFacade(CPApplication.Current.CurrentPage);
            if (SysNo > 0)
            {
                isAdd = false;
                tbAdvUserName.IsReadOnly = true;
                tbMonitor.IsReadOnly = true;
                facade.LoadAdvertiser(SysNo, (obj, args) =>
                {
                    if (args.FaultsHandle())
                        return;

                    vm = args.Result.Convert<Advertisers, AdvertisersVM>();
                    LayoutRoot.DataContext = vm;
                });
            }
            else
            {
                vm = new AdvertisersVM();
                LayoutRoot.DataContext = vm;
            }

            List<ValidationEntity> validationCondition = new List<ValidationEntity>();
            validationCondition.Add(new ValidationEntity(ValidationEnum.IsNotEmpty, this.txtCookie.Text.Trim(), ResNewsInfo.Content_TheNumberIsNull));
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            if (Dialog != null)
            {
                Dialog.ResultArgs.Data = null;
                Dialog.ResultArgs.DialogResult = DialogResultType.OK;
                Dialog.Close();
            }
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidationManager.Validate(this.LayoutRoot))
                return;
            vm = LayoutRoot.DataContext as AdvertisersVM;
            Advertisers Adv = vm.ConvertVM<AdvertisersVM, Advertisers>();
            Adv.CompanyCode = Newegg.Oversea.Silverlight.ControlPanel.Core.CPApplication.Current.CompanyCode;

            if (isAdd)
            {
                facade.AddAdvertiser(Adv, (obj, args) =>
                {
                    if (args.FaultsHandle())
                        return;

                    CPApplication.Current.CurrentPage.Context.Window.Alert(ResNewsInfo.Information_CreateSuccessful,Newegg.Oversea.Silverlight.Controls.Components.MessageType.Information);
                });
            }
            else
            {
                Adv.SysNo = SysNo;
                facade.UpdateAdvertiser(Adv, (obj, args) =>
                {
                    if (args.FaultsHandle())
                        return;
                    CPApplication.Current.CurrentPage.Context.Window.Alert(ResNewsInfo.Information_UpdateSuccessful, Newegg.Oversea.Silverlight.Controls.Components.MessageType.Information);
                });

            }
        }
    }
}
