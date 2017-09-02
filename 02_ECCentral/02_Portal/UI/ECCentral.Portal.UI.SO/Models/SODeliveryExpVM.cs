using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using ECCentral.Portal.Basic.Utilities;
using System.Collections.Generic;
using Newegg.Oversea.Silverlight.Utilities.Validation;
using System.Linq;
using System.ComponentModel;
using ECCentral.Portal.UI.SO.Resources;



namespace ECCentral.Portal.UI.SO.Models
{
    public class SODeliveryExpVM : ModelBase
    {
        

        private string message;
        public string Message
        {
            get { return message; }
            set { SetValue<string>("Message", ref message, value); }
        }

        private int orderType;
        public int OrderType
        {
            get { return orderType; }
            set { SetValue<int>("OrderType", ref orderType, value); }
        }

        private List<CodeNamePair> orderTypeList;
        public List<CodeNamePair> OrderTypeList
        {
            get { return orderTypeList; }
            set { SetValue<List<CodeNamePair>>("OrderTypeList", ref orderTypeList, value); }
        }

        private string orderSysNos;
        [Validate(ValidateType.Required)]
        public string OrderSysNos
        {
            get { return orderSysNos; }
            set {

               

                try
                {
                    List<string> orderList = value.Replace("；", ";").Trim()
                            .Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries)
                            .ToList<string>();

                    orderList.ForEach(x => {
                        int.Parse(x);
                    });

                    if (orderList == null || orderList.Count == 0)
                    {
                        this.ValidationErrors.Add(new System.ComponentModel.DataAnnotations.ValidationResult(ResDeliveryList.Msg_ErrorDataAnnotations));
                        throw new ArgumentException(ResDeliveryList.Msg_ErrorDataAnnotations);
                    }
                }
                catch
                {
                    this.ValidationErrors.Add(new System.ComponentModel.DataAnnotations.ValidationResult(ResDeliveryList.Msg_ErrorDataAnnotations));
                    throw new ArgumentException(ResDeliveryList.Msg_ErrorDataAnnotations);
                }

                this.ValidationErrors.Clear();
                

                SetValue<string>("OrderSysNos", ref orderSysNos, value); 
            }
        }

       

    }
}
