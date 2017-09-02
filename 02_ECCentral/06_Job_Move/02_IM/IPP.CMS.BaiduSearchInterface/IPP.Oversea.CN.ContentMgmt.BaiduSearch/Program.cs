using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IPP.Oversea.CN.ContentManagement.BizProcess.Common;
using IPP.Oversea.CN.ContentMgmt.Baidu.BizProcess;
using IPP.Oversea.CN.ContentMgmt.Baidu.Utility;
using Newegg.Oversea.Framework.ExceptionBase;

namespace IPP.Oversea.CN.ContentMgmt.Baidu
{
    class Program
    {
        static void Main(string[] args)
        {
            TxtFileLogger logger = LoggerManager.GetLogger();
            try
            {
                logger.WriteLog("任务开始。");

                BaiduBPV2 bp = new BaiduBPV2(logger);
                bp.Process();

                logger.WriteLog("任务结束。");

                //BaiduBP processor = new BaiduBP();
                //processor.Init();
                //processor.Process();
            }
            catch (Exception ex)
            {
                logger.WriteLog(ex.ToString());
                JobHelper.SendMail(ex.ToString());
                logger.WriteLog("百度搜索Datafeed处理完成！");
            }
        }
    }
}
