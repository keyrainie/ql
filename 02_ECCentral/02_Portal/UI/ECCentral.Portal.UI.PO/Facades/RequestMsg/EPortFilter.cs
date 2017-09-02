using ECCentral.BizEntity.PO;
using ECCentral.QueryFilter.Common;
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

namespace ECCentral.Portal.UI.PO
{
    public class EPortFilter
    {
        public EPortFilter()
        {
            PageInfo = new PagingInfo{ PageIndex=0,PageSize=25};
        }
        public PagingInfo PageInfo { get; set; }

        /// <summary>
        /// 系统编号
        /// </summary>
        public int? SysNo
        {
            get;
            set;
        }
        /// <summary>
        /// 名称
        /// </summary>
        public string Name
        {
            get;
            set;
        }
        /// <summary>
        /// 免税限额
        /// </summary>
        public int TaxFreeLimit
        {
            get;
            set;
        }
        /// <summary>
        /// 发货方式
        /// </summary>
        public EPortShippingTypeENUM ShippingType
        {
            get;
            set;
        }

        /// <summary>
        /// 支付方式
        /// </summary>
        public string PayType
        {
            get;
            set;
        }
        /// <summary>
        /// 状态
        /// </summary>
        public EPortStatusENUM? Status
        {
            get;
            set;
        }
        public DateTime? DateFrom
        {
            get;
            set;
        }

        public DateTime? DataTo
        {
            get;
            set;
        }
        /// <summary>
        /// 创建人SysNo
        /// </summary>
        public int? CreateUserSysNo
        {
            get;
            set;
        }
        /// <summary>
        /// 创建人
        /// </summary>
        public string CreateUser
        {
            get;
            set;
        }

    }
}
