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
using Newegg.Oversea.Silverlight.Controls;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using Newegg.Oversea.Silverlight.Utilities.Validation;

namespace ECCentral.Portal.Basic.Components.UserControls.LanguagesDescription
{
    public partial class UCLanguageDescripttionSetting : UserControl
    {
        public BizObjecLanguageDescVM viewModel
        {
            get
            {
                return this.DataContext as BizObjecLanguageDescVM;
            }
        }

        public BizObjecLanguageDescVM receive { get; set; }
        Facade facade;
        private IWindow CurrentWindow
        {
            get
            {
                return CPApplication.Current.CurrentPage.Context.Window;
            }
        }
        public UCLanguageDescripttionSetting()
        {

            facade = new Facade(CPApplication.Current.CurrentPage);
            InitializeComponent();
            this.Loaded+= new RoutedEventHandler(UCLanguageDescripttionSetting_Loaded);
        }


        void UCLanguageDescripttionSetting_Loaded(object sender, RoutedEventArgs e)
        {
            this.DataContext = viewModel;
            this.Loaded -= new RoutedEventHandler(UCLanguageDescripttionSetting_Loaded);
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
           if(!ValidationManager.Validate(LayoutRoot))
           {
               return;
           }
           if (!viewModel.SysNo.HasValue)
            {
                facade.Create(viewModel, (obj,args) =>
                    {
                        if(args.Result)
                        {
                            this.CurrentWindow.Alert("保存成功！");
                        }
                    });
            }
            else
            {
                facade.Update(viewModel, (args) =>
                {
                    if(args)
                    {
                        this.CurrentWindow.Alert("保存成功！");
                    }
                });
            }
        }

    }
}
