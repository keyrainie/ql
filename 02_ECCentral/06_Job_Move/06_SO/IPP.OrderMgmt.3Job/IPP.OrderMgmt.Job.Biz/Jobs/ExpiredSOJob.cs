using System;
using System.Configuration;
using System.Data;

using Newegg.Oversea.Framework.DataAccess;
using Newegg.Oversea.Framework.JobConsole.Client;

using IPP.OrderMgmt.Job.Biz.Resource;
using System.Threading;
using ECCentral.BizEntity.Common;

namespace IPP.OrderMgmt.Job.Biz
{
    /// <summary>
    /// 
    /// </summary>
    public class ExpiredSOJob : JobInstance
    {
        private static int success = 0;
        private static int failed = 0;

        /// <summary>
        /// Descript the job
        /// </summary>
        public ExpiredSOJob()
        {
            JobName = "ExpiredSOJob";
            Description = "ExpiredSOJob";
            ExcuteTime = DateTime.Now.ToString();
        }

        /// <summary>
        /// Main job logic
        /// Pass local test in person
        /// </summary>
        public override void Run(JobContext context)
        {
            success = 0; //先清零
            failed = 0;

            base.Run(context);
            context.Message += StringResource.ExpiredSOJobStart + "\r\n";
            AbandonExpiredSO(context);
        }

        private void AbandonExpiredSO(JobContext context)
        {
            var command = DataCommandManager.GetDataCommand("ExpiredSO");
            command.SetParameterValue("CompanyCode", CompanyCode);

            var ds = command.ExecuteDataSet();
            var inform = string.Empty;
            // AZ 订单标示，使用不同的邮件内容
            var isAZSO = false;

            if (Util.HasMoreRow(ds))
            {
                context.Message += string.Format(StringResource.ExpiredSOWorkingCount,
                    ds.Tables[0].Rows.Count) + "\r\n";

                inform = @Resource.StringResource.InForm;
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    try
                    {
                        //abandon so here
                        Util.AbandonSO((int)dr["sysno"], context);
                        context.Message += string.Format(StringResource.ExpiredSOSuccessful,DateTime.Now.ToLongTimeString(),
                            (int)dr["sysno"]) + "\r\n";
                        success++;
                        Thread.Sleep(100);
                    }
                    catch (Exception exp)
                    {
                        inform += "<tr><td align=center><font color ='red'>" + dr["SOID"].ToString() + "</font></td>"
                                    + "<td align=center><font color ='red'>" + dr["orderdate"].ToString() + "</font></td>"
                                    + "<td align=center><font color ='red'>" + dr["UpdateTime"].ToString() + "&nbsp;</font></td>"
                                    + "<td align=center><font color ='red'>" + dr["paytypename"].ToString() + "</font></td>"
                                    + "<td align=center><font color ='red'>" + dr["email"].ToString() + "&nbsp; Error:" + exp.Message + /*exp.StackTrace + */"</font></td>"
                                + "</tr>";
                        context.Message += "订单：" + dr["sysno"] + "作废失败，原因：" +
                            exp.Message.Replace("\r", "").Replace("\n", "") + "\r\n";
                        failed++;
                        continue;
                    }

                    var checkShippingInfo = Util.GetSOCheckShipping((int)dr["sysno"], this.CompanyCode);

                    try
                    {
                        isAZSO = ((int)checkShippingInfo.Tables[0].Rows[0]["SpecialSOType"] ==
                            (int)CommonEnum.SpecialSOType.AZ);

                        //发mail给客户，通知订单已经作废
                        var csMailBody = string.Empty;
                        if (!isAZSO)
                        {
                            csMailBody = string.Format(
                                Resource.StringResource.SOEmailBody
                                , Util.TrimNull(dr["CustomerID"])
                                , Util.TrimNull(dr["SOID"])
                                , DateTime.Now.Date.ToString("yyyy.MM.dd"))
                                .Replace("$$Year",DateTime.Today.Year.ToString());
                        }
                        else // AZ订单作废使用AZ特定的内容
                        {
                            csMailBody = string.Format(
                                Resource.StringResource.AZSOEmailBody, Util.TrimNull(dr["SOID"]));
                        }
                        //Util.SendEmail(this.CompanyCode, new MailInfo
                        //{
                        //    ToName = dr["email"].ToString(),
                        //    Subject = string.Format(StringResource.SOCancal, dr["SOID"].ToString()),
                        //    Body = csMailBody,
                        //    IsAsync = true,
                        //    IsInternal = false
                        //});

                        context.Message += string.Format(StringResource.ExpiredSOMailToCustomer,
                            dr["SOID"]) + dr["email"].ToString() + "\r\n";

                    }
                    catch (Exception ex)
                    {
                        Util.WriteTheExcetpionLog(string.Format(StringResource.ExpiredSOMailException,
                            dr["SysNo"]) + "\r\n" + ex.ToString());
                        context.Message += string.Format
                            (StringResource.ExpiredSOMailException, dr["SysNo"]) + "\r\n"
                            + ex.Message;
                    }

                    inform += "<tr><td align=center>" + dr["SOID"].ToString() + "</td>"
                                + "<td align=center>" + dr["orderdate"].ToString() + "</td>"
                                + "<td align=center>" + dr["UpdateTime"].ToString() + "&nbsp;</td>"
                                + "<td align=center>" + dr["paytypename"].ToString() + "</td>"
                                + "<td align=center>" + dr["email"].ToString() + "&nbsp;</td>"
                            + "</tr>";
                }
                inform += "</table>";
                context.Message += string.Format(StringResource.ExpiredSOEexcuteDone,
                    success, failed) + "\r\n";
            }
            else
            {
                inform = "<div align=center>无记录</div>";
                context.Message += StringResource.ExpiredSONoResult + "\r\n";
            }

            Util.SendEmail(this.CompanyCode, new MailInfo
            {
                ToName = ConfigurationManager.AppSettings["ExpiredSOMailList"],
                Body = inform,
                Subject = StringResource.ExpiredSOCSerMailSubject +
                    DateTime.Now.ToString(AppConst.DateFormatLong),
                IsAsync = true,
                IsInternal = true
            });
        }
    }
}
