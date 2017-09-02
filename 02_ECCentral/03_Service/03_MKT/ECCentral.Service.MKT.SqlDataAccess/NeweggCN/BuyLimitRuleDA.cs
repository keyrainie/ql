using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.MKT;
using ECCentral.Service.MKT.IDataAccess;
using ECCentral.Service.Utility;
using ECCentral.Service.Utility.DataAccess;

namespace ECCentral.Service.MKT.SqlDataAccess
{
    [VersionExport(typeof(IBuyLimitRuleDA))]
    public class BuyLimitRuleDA : IBuyLimitRuleDA
    {
        #region IBuyLimitRuleDA Members

        public BuyLimitRule Load(int sysNo)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("NewPromotion_BuyLimitRule_Load");
            cmd.SetParameterValue("@SysNo", sysNo);

            return cmd.ExecuteEntity<BuyLimitRule>();
        }

        public void Insert(BuyLimitRule data)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("NewPromotion_BuyLimitRule_Insert");

            cmd.SetParameterValue(data);

            cmd.ExecuteNonQuery();
            data.SysNo = Convert.ToInt32(cmd.GetParameterValue("@SysNo"));
        }

        public void Update(BuyLimitRule data)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("NewPromotion_BuyLimitRule_Update");

            cmd.SetParameterValue(data);

            cmd.ExecuteNonQuery();
        }

        /// <summary>
        /// 验证商品/套餐是否已存在规则设置
        /// </summary>
        /// <param name="limitType">规则类型</param>
        /// <param name="excludeSysNo">排除规则系统编号</param>
        /// <param name="itemSysNos">商品或套餐系统编号</param>
        public bool CheckExistsRule(LimitType limitType, int excludeSysNo, params int[] itemSysNos)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("NewPromotion_BuyLimitRule_CheckExistsRule");
            cmd.SetParameterValue("@ExcludeSysNo", excludeSysNo);
            cmd.SetParameterValue("@LimitType", (int)limitType);
            cmd.ReplaceParameterValue("#ItemSysNos#", string.Join(",", itemSysNos));
            return cmd.ExecuteScalar<bool>();
        }

        #endregion
    }
}
