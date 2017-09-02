using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Nesoft.ECWeb.MobileService.Models.Order;

namespace Nesoft.ECWeb.MobileService.Models.Cart
{
    public class CartResultModel
    {
        public bool HasSucceed { get; set; }

        public List<string> ErrorMessages { get; set; }

        public OrderInfoModel ReturnData { get; set; }

    }
}