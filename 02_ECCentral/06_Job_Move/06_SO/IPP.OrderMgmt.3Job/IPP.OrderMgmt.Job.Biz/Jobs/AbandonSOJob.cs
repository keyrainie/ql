using System;
using System.Configuration;
using System.Data;

using IPP.Oversea.CN.CustomerMgmt.ServiceInterfaces.DataContracts;
using IPP.Oversea.CN.ServiceCommon.ServiceInterfaces.DataContracts;

using Newegg.Oversea.Framework.DataAccess;
using Newegg.Oversea.Framework.JobConsole.Client;

using IPP.OrderMgmt.Job.Biz.Resource;
using System.Threading;

namespace IPP.OrderMgmt.Job.Biz
{
    /// <summary>
    /// 
    /// </summary>
    public class AbandonOCNSOJob : JobInstance
    {
        private static int success = 0;
        private static int failed = 0;

        /// <summary>
        /// 以旧换新作废订单Job
        /// </summary>
        public AbandonOCNSOJob()
        {
            JobName = "AbandonSOJob";
            Description = "AbandonSOJob";
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
            AbandonOCNSO(context);
        }

        private void AbandonOCNSO(JobContext context)
        {
            var command = DataCommandManager.GetDataCommand("AbandonOCNSOJob");
            command.SetParameterValue("CompanyCode", CompanyCode);

            var ds = command.ExecuteDataSet();
            var inform = string.Empty;

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
                        context.Message += "订单：" + dr["sysno"] + "作废失败，原因：" +
                            exp.Message.Replace("\r", "").Replace("\n", "") + "\r\n";
                        failed++;
                        continue;
                    }
                }
                context.Message += string.Format(StringResource.ExpiredSOEexcuteDone,
                    success, failed) + "\r\n";
            }
            else
            {
                inform = "<div align=center>无记录</div>";
                context.Message += StringResource.ExpiredSONoResult + "\r\n";
            }
        }
    }
}
