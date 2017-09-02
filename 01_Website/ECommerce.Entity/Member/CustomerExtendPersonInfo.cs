using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECommerce.Entity.Member
{

    /// <summary>
    /// 用户扩展信息
    /// </summary>
    public class CustomerExtendPersonInfo : EntityBase
    {
        #region [ properties ]


        /// <summary>
        /// 扩展表的CustomID
        /// </summary>
        public int CustomerSysNo { get; set; }

        public string CustomerNickName { get; set; }

        /// <summary>
        /// 居住现状
        /// </summary>
        public string LivingCondition { get; set; }

        /// <summary>
        /// 职业身份
        /// </summary>
        public string Occupation { get; set; }

        /// <summary>
        /// 婚姻状况
        /// </summary>
        public string Marriage { get; set; }

        /// <summary>
        /// 家乡
        /// </summary>
        public int HomeTownAreaSysNo { get; set; }

        /// <summary>
        /// 博客地址
        /// </summary>
        public string FavorURL { get; set; }

        /// <summary>
        /// 兴趣爱好
        /// </summary>
        public string PurchaseInterest { get; set; }

        /// <summary>
        /// 喜欢的明星
        /// </summary>
        public string FavorStar { get; set; }

        /// <summary>
        /// 喜欢的品牌
        /// </summary>
        public string PurchaseBrand { get; set; }
        #endregion
    }
}
