using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.Common;

namespace ECCentral.BizEntity.IM
{
    /// <summary>
    /// 商品退换货信息
    /// </summary>
    public class ProductRMAPolicyInfo:ICompany,ILanguage
    {

        
        /// <summary>
        /// 退换货日志编号
        /// </summary>
        public int? RMAPolicyChangeLogSysNo { get; set; }

        /// <summary>
        /// 退换货SysNo
        /// </summary>
        public int? RMAPolicyMasterSysNo { get; set; }

        /// <summary>
        /// 是否可以在线申请
        /// </summary>
        public string IsOnlineRequst { get; set; }
        /// <summary>
        /// 退货期
        /// </summary>
        public int? ReturnDate { get; set; }
        /// <summary>
        /// 换货期
        /// </summary>
        public int? ChangeDate { get; set; }
        /// <summary>
        /// 保修期
        /// </summary>
        public int? WarrantyDay { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
        public string WarrantyDesc { get; set; }

        public string IsBrandWarranty { get; set; } 
        /// <summary>
        /// 商品编号
        /// </summary>
        public int? ProductSysNo { get; set; }

        #region ICompany Members

        public string CompanyCode
        {
            get;
            set;
        }
        public UserInfo User { get; set; }

        #endregion

        #region ILanguage Members

        public string LanguageCode
        {
            get;
            set;
        }

        #endregion
    }
}
