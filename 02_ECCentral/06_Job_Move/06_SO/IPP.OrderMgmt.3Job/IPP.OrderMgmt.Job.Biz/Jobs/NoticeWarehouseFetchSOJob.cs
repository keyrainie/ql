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
    public class NoticeWarehouseFetchSOJob:JobInstance
    {
        private static int success = 0;
        private static int failed = 0;

        /// <summary>
        /// Descript the job
        /// </summary>
        public NoticeWarehouseFetchSOJob()
        {
            JobName = "NoticeWarehouseFetchSOJob";
            Description = "NoticeWarehouseFetchSOJob";
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

            context.Message += StringResource.TimeFlagNoticeWHSelfFetchSOJobStart + "\r\n";
            NoticeWarehouseFetchSO(context);
        }

        private void NoticeWarehouseFetchSO(JobContext context)
        {
            var date = DateTime.Now.ToString("yyyy-MM-dd") + " 23:59:59";
            DataCommand command = DataCommandManager.GetDataCommand("NoticeWarehouse");
            command.SetParameterValue("ShipTypeEnum", (int)CommonEnum.ShipTypeEnum.WareHouseBySelf);
            command.SetParameterValue("date", date);
            command.SetParameterValue("CompanyCode", this.CompanyCode);
            var ds = command.ExecuteDataSet();
            if (!Util.HasMoreRow(ds))
            {
                context.Message += StringResource.NoticeWarehouseFetchSONoResut + "\r\n";
                return;
            }

            context.Message += string.Format
                (StringResource.NoticeWarehouseFetchSOWorkingCount,ds.Tables[0].Rows.Count) + "\r\n";

            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                try
                {
                    var cellPhone = dr["CellPhone"].ToString();
                    var soNumber = dr["SysNo"].ToString();
                    var time = "17:30";
                    var msg = string.Format(Resource.StringResource.NoticeWarehouseFetchSOMsgBody,
                        soNumber, time);

                    ////上海仓搬仓临时修改短信内容
                    //if (dr["ShipTypeSysNo"].ToString() == "13")
                    //    msg = string.Format(Resource.StringResource.NoticeWarehouseFetchSOMsgBodySH,
                    //        soNumber, time);

                    if (Util.IsCellNumber(cellPhone))
                    {
                        Util.SendTheShortMessage(new SMSInfo
                        {
                            CellPhoneNum = cellPhone,
                            Content = msg,
                            Priority = 1
                        }, context);
                        context.Message += string.Format
                            (StringResource.SMSSendSuccessful, (int)dr["sysno"]) + "\r\n";

                        success++;
                    }
                    else 
                    {
                        context.Message += "订单号:" + (int)dr["sysno"] + "发送短信失败，原因为手机号格式不正确\r\n";
                        failed++;
                    }
                }
                catch (Exception ex)
                {
                    context.Message += string.Format(StringResource.SMSSendFailed, dr["sysno"]) +
                            ex.Message.Replace("\r", "").Replace("\n", "") + "\r\n";
                    failed++;
                }
            }
            context.Message += string.Format(StringResource.SMSJobDone, success, failed) + "\r\n";   
        }
    }
}
