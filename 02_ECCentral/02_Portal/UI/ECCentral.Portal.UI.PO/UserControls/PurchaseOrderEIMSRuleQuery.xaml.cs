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
using ECCentral.Portal.UI.PO.Facades;
using ECCentral.BizEntity.PO;
using ECCentral.Portal.UI.PO.Models;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.Portal.UI.PO.Resources;

namespace ECCentral.Portal.UI.PO.UserControls
{
    public partial class PurchaseOrderEIMSRuleQuery : UserControl
    {
        public string EIMSNo;

        public IDialog Dialog { get; set; }
        public PurchaseOrderFacade serviceFacade;
        public PurchaseOrderEIMSRuleInfoVM viewVM;
        public IWindow CurrentWindow
        {
            get
            {
                return CPApplication.Current.CurrentPage.Context.Window;
            }
        }

        public IPage CurrentPage
        {
            get
            {
                return CPApplication.Current.CurrentPage;
            }
        }
        public PurchaseOrderEIMSRuleQuery(string eimsNo)
        {
            EIMSNo = eimsNo;
            viewVM = new PurchaseOrderEIMSRuleInfoVM();
            InitializeComponent();
            this.Loaded += new RoutedEventHandler(PurchaseOrderEIMSRuleQuery_Loaded);
        }

        /// <summary>
        /// 加载合同信息
        /// </summary>
        private void LoadEIMSInfo()
        {
            bool isRuleNumber = false;
            int ruleNumber = 0;
            if (int.TryParse(EIMSNo, out ruleNumber))
            {
                isRuleNumber = true;
            }

            if (isRuleNumber)
            {
                serviceFacade.GetEIMSRuleInfoBySysNo(ruleNumber.ToString(), (obj, args) =>
                {
                    if (args.FaultsHandle())
                    {
                        return;
                    }
                    PurchaseOrderEIMSRuleInfo ruleInfo = args.Result;
                    if (null != ruleInfo)
                    {
                        ruleInfo.RebateSchemeTransactions.ForEach(x =>
                        {
                            x.RebateBaseType = ruleInfo.RebateScheme.RebateBaseType;
                        });
                        viewVM = EntityConverter<PurchaseOrderEIMSRuleInfo, PurchaseOrderEIMSRuleInfoVM>.Convert(ruleInfo);
                        this.DataContext = viewVM;
                    }
                    else
                    {
                        this.lblAlterMemo.Text = string.Format(ResPurchaseOrderMaintain.Label_EIMSRule_NotExist, EIMSNo);
                    }
                });
            }
            else
            {
                serviceFacade.GetEIMSRuleInfoByAssignedCode(EIMSNo, (obj, args) =>
                {
                    if (args.FaultsHandle())
                    {
                        return;
                    }
                    PurchaseOrderEIMSRuleInfo ruleInfo = args.Result;
                    if (null != ruleInfo)
                    {
                        ruleInfo.RebateSchemeTransactions.ForEach(x =>
                        {
                            x.RebateBaseType = ruleInfo.RebateScheme.RebateBaseType;
                        });
                        viewVM = EntityConverter<PurchaseOrderEIMSRuleInfo, PurchaseOrderEIMSRuleInfoVM>.Convert(ruleInfo);
                        this.DataContext = viewVM;
                    }
                    else
                    {
                        this.lblAlterMemo.Text = string.Format(ResPurchaseOrderMaintain.Label_EIMSRule_NotExist, EIMSNo);
                    }
                });
            }
        }

        void PurchaseOrderEIMSRuleQuery_Loaded(object sender, RoutedEventArgs e)
        {
            this.Loaded -= PurchaseOrderEIMSRuleQuery_Loaded;
            this.serviceFacade = new PurchaseOrderFacade(CurrentPage);
            LoadEIMSInfo();
        }
    }
}
