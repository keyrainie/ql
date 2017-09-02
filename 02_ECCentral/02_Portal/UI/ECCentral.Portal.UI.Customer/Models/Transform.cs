using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using ECCentral.BizEntity.Customer;
using ECCentral.Portal.Basic;
using ECCentral.Portal.UI.Customer.Models;
using ECCentral.Portal.Basic.Utilities;

namespace ECCentral.Portal.UI.Customer.Models
{
    public static class Transform
    {
        public static CustomerVM EtoM(this CustomerInfo entity)
        {
            return entity.Convert<CustomerInfo, CustomerVM>();
        }
        public static ObservableCollection<CustomerVM> EtoM(this List<CustomerInfo> elist)
        {
            if (elist == null)
                return null;

            return elist.Convert<CustomerInfo, CustomerVM, ObservableCollection<CustomerVM>>();

        }
    }
}