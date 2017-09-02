using System;
using System.Data;
using IPP.OrderMgmt.Job.Biz.Resource;
using Newegg.Oversea.Framework.DataAccess;
using Newegg.Oversea.Framework.JobConsole.Client;

namespace IPP.OrderMgmt.Job.Biz
{
    /// <summary>
    /// 每天对3天前出库的订单发送评论送积分提醒邮件
    /// </summary>
    public class SendAddPointMailJob : JobInstance
    {
        private static DateTime TimeAddPoint;
        private static int successed = 0;
        private static int failed = 0;

        /// <summary>
        /// descript the job
        /// </summary>
        public SendAddPointMailJob()
        {
            this.JobName = "SendAddPointEmail";
            this.Description = "Send the Email notice the customer comment ";
            this.Description += " the product if the order is after three days";
            this.ExcuteTime = DateTime.Now.ToString();
        }

        /// <summary>
        /// Prepare the order which through give point away 
        /// through add the product comment 
        /// Send the notice Email
        /// </summary>
        public override void Run(JobContext context)
        {
            successed = 0;
            failed = 0;
            if (DateTime.Now.Hour >= 4 && DateTime.Now.Hour < 5)
            {
                if (TimeAddPoint.Date < DateTime.Now.Date)
                {
                    context.Message += StringResource.SendAddPoindMailJobStartMsg + "\r\n";
                    SendAddPointEmail(context);
                    TimeAddPoint = DateTime.Now;
                    context.Message += StringResource.JobCompletedMsg;
                }
            }
            else
                context.Message += StringResource.SendAddPoindMailJobStartTimeNote + "\r\n";
        }

        /// <summary>
        /// Mapping old IPP system method:SendAddPointEmail
        /// </summary>
        private void SendAddPointEmail(JobContext context)
        {
            // 对3天前出库的订单发送评论送积分的邮件，
            DataCommand command = DataCommandManager.GetDataCommand("AddPointSO");
            command.SetParameterValue("SpecialSOType", ((int)CommonEnum.SpecialSOType.AZ).ToString());
            command.SetParameterValue("Status", ((int)CommonEnum.SOStatus.OutStock).ToString());
            command.SetParameterValue("CompanyCode", CompanyCode);
            var ds = command.ExecuteDataSet();

            if (!Util.HasMoreRow(ds))
            {
                context.Message += StringResource.SendAddPoindMailJobNoResultMsg + "\r\n";
                return;
            }

            context.Message += string.Format(StringResource.SendAddPointMailJobWrokingCount,
                ds.Tables[0].Rows.Count) + "\r\n";
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                try
                {
                    var soSysNo = Convert.ToInt32(dr["SysNo"]);
                    Util.SendTheAddPointEmail(soSysNo, context);
                    context.Message += string.Format(StringResource.SendAddPointMailJobMailSendSuccessful, soSysNo)
                        + "\r\n";

                    successed++;
                }
                catch (Exception ex)
                {
                    Util.WriteTheExcetpionLog(string.Format(StringResource.SendAddPointMailJobExceptionMsg, dr["SysNo"])
                        + "：\r\n" + ex.ToString());

                    context.Message += string.Format(StringResource.SendAddPointMailJobExceptionMsg, dr["SysNo"]) + ex.ToString() + "\r\n";
                    failed++;
                }
            }
            context.Message += string.Format(StringResource.JobExecuteDone + "\r\n",
                successed, failed);
        }

        /// <summary>
        /// send email action
        /// </summary>
        /// <param name="soInfo"></param>
        #region deleted code
        private void SendEmail()
        {
            
        }
        #endregion
    }
}
