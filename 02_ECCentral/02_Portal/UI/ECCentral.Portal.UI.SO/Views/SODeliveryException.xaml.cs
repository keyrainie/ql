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
using System.Windows.Navigation;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using ECCentral.Portal.UI.SO.Models;
using ECCentral.Portal.UI.SO.Facades;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using ECCentral.BizEntity.SO;
using Newegg.Oversea.Silverlight.Controls;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.Portal.Basic;
using Newegg.Oversea.Silverlight.Utilities.Validation;


namespace ECCentral.Portal.UI.SO.Views
{
    [View]
    public partial class SODeliveryException : PageBase
    {
        SODeliveryExpVM PageView;
        SOLogisticsFacade Facade;


        public SODeliveryException()
        {

            InitializeComponent();
        }

        public override void OnPageLoad(object sender, EventArgs e)
        {
            base.OnPageLoad(sender, e);

            Facade = new SOLogisticsFacade(this);
            PageView = new SODeliveryExpVM();
            //this.tipMsgBox.DataContext = PageView.Message;

            BindData();

            this.spConditions.DataContext = PageView;
            this.ddlOrderType.SelectedIndex = 0;

        }


        private void BindData()
        {


            //CodePair
            CodeNamePairHelper.GetList(ConstValue.DomainName_SO
                ,  ConstValue.Key_DeliveryExpOrderType 
                , CodeNamePairAppendItemType.None, (o, p) =>
                {

                    PageView.OrderTypeList = p.Result;
                });


 
        }

        private void MarkExp_Click(object sender, RoutedEventArgs e)
        {
            ValidationManager.Validate(this.spConditions);

            if (PageView.ValidationErrors.Count == 0)
            {
                
                string orders = PageView.OrderSysNos;

                List<string> orderList = orders.Replace("；", ";").Trim()
                        .Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries)
                        .ToList<string>();

                DeliveryExpMarkEntity entity = new DeliveryExpMarkEntity() { OrderSysNos = new List<int?>() };

                foreach (string order in orderList)
                {
                    int orderInt32;
                    int.TryParse(order, out orderInt32);

                    if (orderInt32 == 0)
                    {
                        PageView.Message += order;
                        return;
                    }
                    entity.OrderSysNos.Add(orderInt32);
                }

                entity.CompanyCode = CPApplication.Current.CompanyCode;
                entity.OrderType = PageView.OrderType;

                Facade.MarkDeliveryExp(entity, (obj, args) =>
                {
                    if (args.FaultsHandle())
                        return;
                });
                
            }
        }

    }
}
