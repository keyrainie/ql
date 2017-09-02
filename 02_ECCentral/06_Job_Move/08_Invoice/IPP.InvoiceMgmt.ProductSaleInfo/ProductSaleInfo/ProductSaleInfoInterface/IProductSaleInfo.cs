using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProductSaleInfoInterface
{
    public delegate void ShowMsg(string info);
    public interface IProductSaleInfo
    {
        void SendProductSaleInfoReport();
        ShowMsg ShowInfo { get; set; }
    }
}
