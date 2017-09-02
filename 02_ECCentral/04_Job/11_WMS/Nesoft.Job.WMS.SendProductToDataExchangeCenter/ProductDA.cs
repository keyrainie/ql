using Nesoft.Job.WMS.Common.Entity;
using Nesoft.Utility;
using Nesoft.Utility.DataAccess;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using Nesoft.Utility;
using System.IO;

namespace Nesoft.Job.WMS.SendProductToDataExchangeCenter
{
    public static class ProductDA
    {
        static ProductDA()
        {
            if (!File.Exists(s_SendSuccessProductDataFile))
            {
                SerializationUtility.SaveToXml(s_SendSuccessProductDataFile, new List<SuccessLog>());
            }
        }
        /// <summary>
        /// 查询商品信息
        /// </summary>
        /// <returns></returns>
        public static List<ProductInfo> QueryProduct()
        {
            var successSysNos = SerializationUtility.LoadFromXml<List<SuccessLog>>(s_SendSuccessProductDataFile);
            CustomDataCommand cmd = DataCommandManager.CreateCustomDataCommandFromConfig("QueryProduct");
            using (var sqlBuilder = new DynamicQuerySqlBuilder(cmd))
            {
                if (successSysNos.Count > 0)
                {
                    var sysNos = string.Format("({0})", string.Join(",", successSysNos.Select(t => t.ProductSysNo.ToString()).ToArray()));
                    sqlBuilder.ConditionConstructor.AddCustomCondition(QueryConditionRelationType.AND, string.Format(@"p.SysNo NOT IN " + sysNos));
                }
                //增加报关成功条件
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "pcei.EntryStatusEx", DbType.Int32, "@EntryStatusEx", QueryConditionOperatorType.Equal, 3);
                //增加备案成功条件
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "pcei.EntryStatus", DbType.Int32, "@EntryStatus", QueryConditionOperatorType.Equal, 4);
                //添加过滤直邮商品
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "pe.ProductTradeType", DbType.Int32, "@ProductTradeType", QueryConditionOperatorType.Equal, 1);
                cmd.CommandText = sqlBuilder.BuildQuerySql();
            }
            return cmd.ExecuteEntityList<ProductInfo>();
        }
        /// <summary>
        /// 发送成功商品日志记录
        /// </summary>
        public readonly static string s_SendSuccessProductDataFile = Path.Combine(AppDomain.CurrentDomain.SetupInformation.ApplicationBase, "SendSuccessProduct.config");
        /// <summary>
        /// 更新商品发送状态
        /// </summary>
        /// <param name="sysNo"></param>
        /// <returns></returns>
        public static bool UpdateProductStatus(int sysNo)
        {
            var successSysNos = SerializationUtility.LoadFromXml<List<SuccessLog>>(s_SendSuccessProductDataFile);
            if (successSysNos.FirstOrDefault(t => t.ProductSysNo == sysNo) == null)
            {
                successSysNos.Add(new SuccessLog() { ProductSysNo = sysNo, CreateTime = DateTime.Now });
                SerializationUtility.SaveToXml(s_SendSuccessProductDataFile, successSysNos);
            }
            return true;
            //DataCommand cmd = DataCommandManager.GetDataCommand("UpdateProductStatus");
            //cmd.SetParameterValue("@SysNo", sysNo);
            //return cmd.ExecuteNonQuery() > 0;
        }
    }
    /// <summary>
    /// 发送成功日志记录信息
    /// </summary>
    public class SuccessLog
    {
        public int ProductSysNo { get; set; }
        public DateTime CreateTime { get; set; }
    }
}
