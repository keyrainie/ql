using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newegg.Oversea.Framework.DataAccess;
using SendMKTPointEmail.Biz.Entities;
using SendMKTPointEmail.Biz.Common;

namespace SendMKTPointEmail.Biz.DataAccess
{
    public class SendMKTPointEmailDA
    {
        /// <summary>
        /// 获取积分账号的可用积分
        /// </summary>
        /// <param name="pmAccounts"></param>
        /// <returns></returns>
        public static List<CustomerPointInfoEntity> GetPMPointInfoEntityList(List<string> pmAccounts)
        {
            string pmAccountStr = string.Concat("'", string.Join("','", pmAccounts.ToArray()), "'");

            CustomDataCommand command = DataCommandManager.CreateCustomDataCommandFromConfig("GetPMAccountPointInfo");
            command.AddInputParameter("@CompanyCode", System.Data.DbType.AnsiStringFixedLength, JobConfig.CompanyCode);

            command.CommandText = command.CommandText.Replace("#PMList#", pmAccountStr);

            var result = command.ExecuteEntityList<CustomerPointInfoEntity>();

            return result;
        }


        /// <summary>
        /// 获取MKT账号在过去几天里所支出的积分统计列表
        /// </summary>
        /// <param name="mktAccounts"></param>
        /// <param name="passDays"></param>
        /// <returns></returns>
        public static List<CustomerPointInfoEntity> GetMKTPointInfoEntityList(List<string> mktAccounts, int passDays)
        {
            string pmAccountStr = string.Concat("'", string.Join("','", mktAccounts.ToArray()), "'");

            CustomDataCommand command = DataCommandManager.CreateCustomDataCommandFromConfig("GetMKTAccountPassDaysPointInfo");

            command.CommandText = command.CommandText.Replace("#PASSDAYS#", passDays.ToString());
            command.CommandText = command.CommandText.Replace("#PMList#", pmAccountStr);

            var result = command.ExecuteEntityList<CustomerPointInfoEntity>();

            return result;
        }
    }
}
