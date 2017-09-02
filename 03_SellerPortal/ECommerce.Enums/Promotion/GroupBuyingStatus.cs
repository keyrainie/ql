using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECommerce.Enums.Promotion
{
    /// <summary>
    /// 团购活动状态
    /// </summary>
    public class GroupBuyingStatus
    {
        private static Dictionary<string, string> _keyValues = new Dictionary<string, string>();

        static GroupBuyingStatus()
        {
            _keyValues.Add(GroupBuyingStatus.Init, "初始态");
            _keyValues.Add(GroupBuyingStatus.WaitingAudit, "待审核");
            _keyValues.Add(GroupBuyingStatus.VerifyFaild, "审核未通过");
            _keyValues.Add(GroupBuyingStatus.Pending, "就绪");
            _keyValues.Add(GroupBuyingStatus.Active, "运行");
            _keyValues.Add(GroupBuyingStatus.Deactive, "作废");
            _keyValues.Add(GroupBuyingStatus.Finished, "完成");
        }
        
        /// <summary>
        /// 初始
        /// </summary>
        public static string Init="O";
        /// <summary>
        /// 待审核
        /// </summary>
        public static string WaitingAudit="W";
        /// <summary>
        /// 审核未通过
        /// </summary>
        public static string VerifyFaild="N";
        /// <summary>
        /// 就绪
        /// </summary>
        public static string Pending="P";
        /// <summary>
        /// 待处理,用于商家商品
        /// </summary>
        //public static string WaitHandling="R";
        /// <summary>
        /// 运行
        /// </summary>
        public static string Active="A";
        /// <summary>
        /// 作废
        /// </summary>
        public static string Deactive="D";
        /// <summary>
        /// 完成
        /// </summary>
        public static string Finished = "F";

        public static Dictionary<string, string> GetKeyValues()
        {
            return _keyValues;
        }

        public static string GetValue(string key)
        {
            string result="";
            if (key != null)
            {
                _keyValues.TryGetValue(key, out result);
            }
            return result;
        }

    }
}
