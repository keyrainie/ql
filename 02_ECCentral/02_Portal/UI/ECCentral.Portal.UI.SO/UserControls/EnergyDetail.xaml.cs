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
using ECCentral.Portal.UI.SO.Models;
using Newegg.Oversea.Silverlight.Controls.Components;
using Newegg.Oversea.Silverlight.Controls;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using ECCentral.Portal.UI.SO.Facades;
using Newegg.Oversea.Silverlight.Utilities;
using Newegg.Oversea.Silverlight.Utilities.Validation;
using ECCentral.Portal.UI.SO.Resources;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.Portal.Basic;
using ECCentral.BizEntity.SO;

namespace ECCentral.Portal.UI.SO.UserControls
{
    public partial class EnergyDetail : UserControl
    {
        public IDialog Dialog
        {
            get;
            set;
        }

        private SOEnergyVM m_SOEnergyVM;
        public SOEnergyVM SOEnergyVM
        {
            get { return m_SOEnergyVM; }
            private set
            {
                m_SOEnergyVM = value;
                gdEnergyInfo.DataContext = value;
            }
        }

        public EnergyDetail(int soSysNo)
        {
            InitializeComponent();
            new SOQueryFacade().GetSOEnergyBySOSysNo(soSysNo, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }
                SOEnergyVM = args.Result.Convert<SOEnergyEntity, SOEnergyVM>();
                ECCentral.Portal.Basic.Utilities.UtilityHelper.ReadOnlyControl(gdEnergyInfo, gdEnergyInfo.Children.Count, true);
                if (SOEnergyVM.CertificateType.HasValue)
                {
                    if (SOEnergyVM.CertificateType.Value == 0)
                    {
                        labPerson.Visibility = Visibility.Visible;
                        labCompany.Visibility = Visibility.Collapsed;

                        labCertificateNo.Visibility = Visibility.Visible;
                        txtCertificateNo.Visibility = Visibility.Visible;
                    }
                    else if (SOEnergyVM.CertificateType == 1)
                    {
                        labPerson.Visibility = Visibility.Collapsed;
                        labCompany.Visibility = Visibility.Visible;

                        labCertificateNo.Visibility = Visibility.Collapsed;
                        txtCertificateNo.Visibility = Visibility.Collapsed;
                    }
                }
            });
        }

        private void Button_Close_Click(object sender, RoutedEventArgs e)
        {
            Dialog.Close();
        }
    }
}
