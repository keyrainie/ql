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
using ECCentral.BizEntity.SO;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;

namespace ECCentral.Portal.UI.SO.Models
{
    public class MasterInfoVM : ModelBase
    {
        /// <summary>
        /// 主商品编号
        /// </summary>
        private int m_productSysNo;
        public int ProductSysNo 
        {
            get { return m_productSysNo; }
            set {
                this.SetValue("ProductSysNo", ref m_productSysNo, value); 
            }
        }

        /// <summary>
        /// 主商品数量
        /// </summary>
        private int m_quantity;
        public int Quantity
        {
            get { return m_quantity; }
            set
            {
                this.SetValue("Quantity", ref m_quantity, value);
            }
        }
        /// <summary>
        /// 主商品类型
        /// </summary>
        
        public SOProductType? ProductType { get; set; }


        /// <summary>
        /// 单次活动主商品数量
        /// </summary>
        
        public int QtyPreTime { get; set; }

    }
}
