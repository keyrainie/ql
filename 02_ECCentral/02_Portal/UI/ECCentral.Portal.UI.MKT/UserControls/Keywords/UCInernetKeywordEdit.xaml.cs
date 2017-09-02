using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.Portal.UI.IM.Facades;
using ECCentral.Portal.UI.IM.Models;
using ECCentral.Portal.UI.MKT.Models;
using ECCentral.Portal.UI.MKT.Resources;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using ECCentral.BizEntity.MKT;
using Newegg.Oversea.Silverlight.Controls.Components;
using Newegg.Oversea.Silverlight.Utilities.Validation;

namespace ECCentral.Portal.UI.MKT.UserControls
{

    public partial class UCInernetKeywordEdit : UserControl
    {
        public IDialog Dialog { get; set; }
        private InternetKeywordVM _vm;
        private InernetKeywordFacade _facade;

        public UCInernetKeywordEdit()
        {
            InitializeComponent();
            Loaded += OnUCOpenAPIEditLoad;
        }

        public void OnUCOpenAPIEditLoad(object sender, EventArgs e)
        {
            _facade=new InernetKeywordFacade();
            Bind();
        }

        private void Bind()
        {
            _vm = new InternetKeywordVM ();
            DataContext = _vm;
        }


        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            CloseDialog(DialogResultType.OK);
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidationManager.Validate(this))
                return;
            _facade.CreateKeyword(_vm, (s, args) =>
            {
                if (args.FaultsHandle())
                    return;
                CPApplication.Current.CurrentPage.Context.Window.Alert(ResComputerConfig.Info_AddSuccess);
                CloseDialog(DialogResultType.OK);
            });
        }

        private void CloseDialog(DialogResultType dialogResult)
        {
            if (Dialog != null)
            {
                Dialog.ResultArgs.DialogResult = dialogResult;
                Dialog.Close();
            }
        }
    }

}
