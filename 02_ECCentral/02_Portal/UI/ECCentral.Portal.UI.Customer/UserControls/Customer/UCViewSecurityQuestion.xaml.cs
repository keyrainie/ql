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
using ECCentral.Portal.UI.Customer.Facades;
using ECCentral.Portal.UI.Customer.Models;
using ECCentral.Portal.Basic.Utilities;

namespace ECCentral.Portal.UI.Customer.UserControls.Customer
{
    public partial class UCViewSecurityQuestion : UserControl
    {
        #region 页面初始化
        public IDialog Dialog
        {
            get;
            set;
        }
        private IWindow CurrentWindow
        {
            get
            {
                return CPApplication.Current.CurrentPage.Context.Window;
            }
        }

        public int CustomerSysNo
        {
            get;
            set;
        }

        private CustomerQueryFacade facade;

        private List<SecurityQuestionViewModel> vmList;

        public UCViewSecurityQuestion()
        {
            InitializeComponent();
        }

        public UCViewSecurityQuestion(int customerSysNo)
        {
            InitializeComponent();
            this.CustomerSysNo = customerSysNo;
            facade = new CustomerQueryFacade();
            Loaded += new RoutedEventHandler(UCViewSecurityQuestion_Loaded);
        }

        #endregion

        #region 页面加载事件
        void UCViewSecurityQuestion_Loaded(object sender, RoutedEventArgs e)
        {
            facade.GetSecurityQuestion(this.CustomerSysNo, (obj, args) =>
            {
                if (args.FaultsHandle()) return;
                if (args.Result != null)
                {
                    vmList = DynamicConverter<SecurityQuestionViewModel>.ConvertToVMList(args.Result.Rows);
                    if (vmList.Count != 0)
                        this.gridInfo.DataContext = vmList[0];
                    else
                        this.gridInfo.DataContext = new SecurityQuestionViewModel();
                }

            });
        }
        #endregion

        #region 关闭按钮事件
        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Dialog.Close();
        }
        #endregion
    }
}
