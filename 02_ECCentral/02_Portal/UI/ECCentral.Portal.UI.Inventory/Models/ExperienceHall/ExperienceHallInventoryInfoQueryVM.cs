using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
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

namespace ECCentral.Portal.UI.Inventory.Models
{
    public class ExperienceHallInventoryInfoQueryVM : ModelBase
    {
        /// <summary>
        /// 商品一级类别编号
        /// </summary>
        public int? C1SysNo { get; set; }
        /// <summary>
        /// 商品二级类别编号
        /// </summary>
        public int? C2SysNo { get; set; }
        /// <summary>
        /// 商品三级类别编号
        /// </summary>
        public int? C3SysNo { get; set; }


        private string productID;
        public string ProductID
        {
            get
            {
                return productID;
            }
            set
            {
                SetValue("ProductID", ref productID, value);
            }
        }

        private int? _ProductSysNo;
        /// <summary>
        ///  商品SysNo
        /// </summary>
        public int? ProductSysNo { get { return _ProductSysNo; } set { this.SetValue("ProductSysNo", ref _ProductSysNo, value); } }

        //商品名称
        public string ProductName { get; set; }


        public string CompanyCode { get; set; }
    }
}
