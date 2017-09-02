using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using ECCentral.BizEntity.Common;

namespace ECCentral.BizEntity.Inventory
{
    /// <summary>
    /// 商品库存单据-移仓篮
    /// </summary>
    public class ShiftRequestItemBasket : IIdentity, ICompany
    {
        #region IIdentity Members

        /// <summary>
        /// 系统编号
        /// </summary>
        public int? SysNo { get; set; }

        #endregion IIdentity Members

        #region ICompany Members

        /// <summary>
        /// 所属公司
        /// </summary>
        public string CompanyCode
        {
            get;
            set;
        }

        #endregion ICompany Members

        /// <summary>
        /// 创建人
        /// </summary>
        public UserInfo CreateUser { get; set; }

        /// <summary>
        /// 创建日期
        /// </summary>
        public DateTime? CreateDate { get; set; }

        /// <summary>
        /// 更新人
        /// </summary>
        public UserInfo EditUser { get; set; }

        /// <summary>
        /// 更新日期
        /// </summary>
        public DateTime? EditDate { get; set; }

        /// <summary>
        /// 移仓商品列表
        /// </summary>
        public List<ShiftRequestItemInfo> ShiftItemInfoList { get; set; }

    }
}
