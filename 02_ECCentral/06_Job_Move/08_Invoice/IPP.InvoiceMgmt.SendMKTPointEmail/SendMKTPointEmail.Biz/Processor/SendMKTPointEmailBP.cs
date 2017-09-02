using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newegg.Oversea.Framework.JobConsole.Client;
using SendMKTPointEmail.Biz.Entities;
using System.Configuration;
using Newegg.Oversea.Framework.ExceptionBase;
using SendMKTPointEmail.Biz.Common;
using SendMKTPointEmail.Biz.ServiceAdapter;
using SendMKTPointEmail.Biz.DataAccess;
using ECCentral.BizEntity.Common;

namespace SendMKTPointEmail.Biz.Processor
{
    public class SendMKTPointEmailBP
    {
        /// <summary>
        /// JOB Start point
        /// </summary>
        /// <param name="jobContext"></param>
        public static void Start(JobContext jobContext)
        {
            //初始化数据
            List<MKTAccountEntity> mktAccountList = CreateMKTAccountList();
            List<PMAccountEntity> pmAccountList = CreatePMAccountList();

            //获取并填充每个账号可用分的具体值
             FillAvailablePointForMKTAccountList(mktAccountList);
             FillAvailablePointForPMAccountList(pmAccountList);

            //验证数据并填充结果
            CheckMKTAccountList(mktAccountList);
            CheckPMAccountList(pmAccountList);

            //处理结果
            SendMail(mktAccountList, pmAccountList);
        }

        /// <summary>
        /// 从配置项中构建MKT账号信息列表
        /// </summary>
        /// <returns></returns>
        private static List<MKTAccountEntity> CreateMKTAccountList()
        {
            List<MKTAccountEntity> result = new List<MKTAccountEntity>();

            string mktAccountstr = JobConfig.MKTAccount;
            string mktAccountMailRecvstr = JobConfig.MKTAccountMailRecv;

            string[] mktAccounts = mktAccountstr.Split(";".ToCharArray());
            mktAccounts.ToList().ForEach(x =>
            {
                result.Add(new MKTAccountEntity()
                {
                    Account = x,
                    RecvMailList = mktAccountMailRecvstr,
                    Status = 'A'
                }
                    );
            });

            return result;
        }

        /// <summary>
        /// 从配置项中构建PM账号信息列表
        /// </summary>
        /// <returns></returns>
        private static List<PMAccountEntity> CreatePMAccountList()
        {
            List<PMAccountEntity> result = new List<PMAccountEntity>();

            var accountInfoList = JobConfig.AccountPoitInfoList;

            for (int i = 0; i < accountInfoList.Count; i++)
            {
                var item = accountInfoList[i];

                result.Add(new PMAccountEntity()
                {
                    Account = item.Account,
                    PointLowerLimit = item.PointLimt,
                    RecvMailList = item.MailTo,
                    Status = 'A'
                });
            }


            return result;
        }


        /// <summary>
        /// 填充MKT账号在过去几天内的积分使用总值，以及当前可用积分值
        /// </summary>
        /// <param name="mktAccountList"></param>
        private static void FillAvailablePointForMKTAccountList(List<MKTAccountEntity> mktAccountList)
        {
            List<CustomerPointInfoEntity> queryResult = SendMKTPointEmailDA.GetPMPointInfoEntityList(mktAccountList.Select(x => x.Account).ToList());
            List<CustomerPointInfoEntity> queryResult4PassDays = SendMKTPointEmailDA.GetMKTPointInfoEntityList(mktAccountList.Select(x => x.Account).ToList(), JobConfig.MKTAccountPassDays);

            var result = (from x in mktAccountList
                          join q in queryResult
                              on x.Account equals q.CustomerID
                          join p in queryResult4PassDays
                              on x.Account equals p.CustomerID
                          select new MKTAccountEntity()
                          {
                              Account = x.Account,
                              PointAvailable = q.ValidScore,
                              PointLowerLimit = p.PointLowerLimit,
                              RecvMailList = x.RecvMailList,
                              Status = x.Status
                          }
                        ).ToList();

            mktAccountList.Clear();

            mktAccountList.AddRange(result.ToArray());
        }

        /// <summary>
        /// 填充MKT账号当前可用积分值
        /// </summary>
        /// <param name="pmAccountList"></param>
        private static void FillAvailablePointForPMAccountList(List<PMAccountEntity> pmAccountList)
        {
            List<CustomerPointInfoEntity> queryResult = SendMKTPointEmailDA.GetPMPointInfoEntityList(pmAccountList.Select(x => x.Account).ToList());

            var result = (from x in pmAccountList
                          join q in queryResult
                              on x.Account equals q.CustomerID
                          select new PMAccountEntity()
                          {
                              Account = x.Account,
                              PointAvailable = q.ValidScore,
                              PointLowerLimit = x.PointLowerLimit,
                              RecvMailList = x.RecvMailList,
                              Status = x.Status
                          }
                        ).ToList<PMAccountEntity>();

            pmAccountList.Clear();

            pmAccountList.AddRange(result.ToArray());
        }

        /// <summary>
        /// 检查MKT账号积分值及下限
        /// </summary>
        /// <param name="mktAccountList"></param>
        private static void CheckMKTAccountList(List<MKTAccountEntity> mktAccountList)
        {
            string mktAccountRevcMailSubjectTemplet = JobConfig.MKTAccountRevcMailSubjectTemplet;
            string mktAccountRevcMailBodyTemplet = JobConfig.MKTAccountRevcMailBodyTemplet;

            mktAccountList.ForEach(x =>
            {
                if (x.PointLowerLimit >= x.PointAvailable)
                {
                    x.MailSubject = mktAccountRevcMailSubjectTemplet;
                    x.MailBody = string.Format(mktAccountRevcMailBodyTemplet, x.Account, x.PointLowerLimit, x.PointAvailable);
                    x.Memo = x.MailBody;
                    x.Status = 'U';
                }
            });
        }

        /// <summary>
        /// 检查PM账号积分值及下限
        /// </summary>
        /// <param name="pmAccountList"></param>
        private static void CheckPMAccountList(List<PMAccountEntity> pmAccountList)
        {
            string pmAccountRevcMailSubjectTemplet = JobConfig.PMAccountRevcMailSubjectTemplet;
            string pmAccountRevcMailBodyTemplet = JobConfig.PMAccountRevcMailBodyTemplet;

            pmAccountList.ForEach(x =>
            {
                if (x.PointLowerLimit >= x.PointAvailable)
                {
                    x.MailSubject = string.Format(pmAccountRevcMailSubjectTemplet,x.Account,x.PointLowerLimit);
                    x.MailBody = string.Format(pmAccountRevcMailBodyTemplet, x.Account, x.PointLowerLimit, x.PointAvailable);
                    x.Memo = x.MailBody;
                    x.Status = 'U';
                }
            });
        }

        /// <summary>
        /// 依照查结果发送报警邮件
        /// </summary>
        /// <param name="mktAccountList"></param>
        /// <param name="pmAccountList"></param>
        private static void SendMail(List<MKTAccountEntity> mktAccountList, List<PMAccountEntity> pmAccountList)
        {
            //List<EmailEntity> sendList = new List<EmailEntity>();
            List<MailInfo> sendList = new List<MailInfo>();

            mktAccountList.ForEach(x =>
            {
                if (x.Status == 'U')
                {
                    sendList.Add(new MailInfo()
                    {
                        ToName = x.RecvMailList,
                        Subject = x.MailSubject,
                        Body = x.MailBody
                    });
                }
            });

            pmAccountList.ForEach(x =>
            {
                if (x.Status == 'U')
                {
                    sendList.Add(new MailInfo()
                    {
                        ToName = x.RecvMailList,
                        Subject = x.MailSubject,
                        Body = x.MailBody
                    });
                }
            });

            if (JobConfig.SendMailMethodSwitch.Equals("DB", StringComparison.OrdinalIgnoreCase))
            {
                sendList.ForEach(x =>
                {
                    EMailDA.SendEmail(x.ToName, x.Subject, x.Body);
                });
            }
            else if (JobConfig.SendMailMethodSwitch.Equals("RestfulService", StringComparison.OrdinalIgnoreCase))
            {
                CommonServiceAdapter.SendMail(sendList);
            }
            else
            {
                throw (new BusinessException("App.config=>SendMailMethodSwitch is not correct."));
            }
        }

    }
}
