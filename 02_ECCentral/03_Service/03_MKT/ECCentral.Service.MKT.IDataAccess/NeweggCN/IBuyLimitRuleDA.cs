using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.MKT;

namespace ECCentral.Service.MKT.IDataAccess
{
    /// <summary>
    /// 单品/套餐限购规则DA
    /// </summary>
    public interface IBuyLimitRuleDA
    {
        BuyLimitRule Load(int sysNo);

        void Insert(BuyLimitRule data);

        void Update(BuyLimitRule data);

        /// <summary>
        /// 验证商品/套餐是否已存在规则设置
        /// </summary>
        /// <param name="limitType">规则类型</param>
        /// <param name="excludeSysNo">排除规则系统编号</param>
        /// <param name="itemSysNos">商品或套餐系统编号</param>
        bool CheckExistsRule(LimitType limitType, int excludeSysNo,params int[] itemSysNos);
    }
}
