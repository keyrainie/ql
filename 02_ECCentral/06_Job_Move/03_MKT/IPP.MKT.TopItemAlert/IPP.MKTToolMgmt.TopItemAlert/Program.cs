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
                new TopItemBP().Process();
                logger.WriteLog("置顶商品缺货通知完成！");
            }
            catch (Exception ex)
            {
                JobHelper.SendExceptionMail(ex.ToString());
                logger.WriteLog(ex.ToString());
            }
        }
    }
}
