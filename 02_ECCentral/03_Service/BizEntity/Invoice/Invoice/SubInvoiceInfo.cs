using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.Common;

namespace ECCentral.BizEntity.Invoice
{
    /// <summary>
    /// 发票拆分信息
    /// </summary>
    public class SubInvoiceInfo : IIdentity, ICompany
    {
        /// <summary>
        /// 订单系统编号
        /// </summary>
        public int? SOSysNo
        {
            get;
            set;
        }

        /// <summary>
        /// 仓库编号
        /// </summary>
        public int? StockSysNo
        {
            get;
            set;
        }

        /// <summary>
        /// 发票自动编号
        /// </summary>
        public int? InvoiceSeq
        {
            get;
            set;
        }

        /// <summary>
        /// 拆分数量
        /// </summary>
        public int? SplitQty
        {
            get;
            set;
        }

        /// <summary>
        /// 商品系统编号
        /// </summary>
        public int? ProductSysNo
        {
            get;
            set;
        }

        private bool isExtendWarrantyItem = false;
        /// <summary>
        /// 是否是延保
        /// </summary>
        public bool? IsExtendWarrantyItem
        {
            get
            {
                return isExtendWarrantyItem;
            }
            set
            {
                isExtendWarrantyItem = value ?? false;
            }
        }
        /// <summary>
        /// 如果是延保，对应的主商品编号
        /// </summary>
        public List<int> MasterProductSysNo
        {
            get;
            set;
        }

        #region IIdentity Members

        /// <summary>
        /// 系统编号
        /// </summary>
        public int? SysNo
        {
            get;
            set;
        }

        #endregion IIdentity Members

        #region ICompany Members

        /// <summary>
        /// 公司代码
        /// </summary>
        public string CompanyCode
        {
            get;
            set;
        }

        #endregion ICompany Members
    }
}