using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Newegg.Oversea.Framework.ExceptionHandler;
using IPP.Oversea.CN.InventoryMgmt.JobV31.Common;
using System.Threading;
using Newegg.Oversea.Framework.JobConsole.Client;
using System.Text.RegularExpressions;
using IPP.Oversea.CN.InventoryMgmt.JobV31.DataAccess;
namespace IPP.Oversea.CN.InventoryMgmt.JobV31.Biz
{
    public class ChannelInventoryBP
    {
        public static void Run(JobContext context)
        {
            string startJobString = "\r\n\r\n\r\n====================================================================\r\n";
            startJobString += string.Format("====================={0}=====================\r\n", DateTime.Now.ToString("yyyy年MM月dd日 HH:mm:ss:fffff"));
            startJobString += "=====================Job启动=====================\r\n";

            Util.WriteLog(startJobString, context, Config.Debug);

            Util.WriteLog("正在检索渠道商……");

            List<int> channelSysNos = CommonDA.GetAllChannel();
            int[] exists = Config.ExistsChannelSysNo;
            for (int i = 0; i < exists.Length; i++)
            {
                channelSysNos.Remove(exists[i]);
            }
            string[] arr = channelSysNos.Select(item => item.ToString()).ToArray();
            Util.WriteLog(string.Format("共检索渠道商{0}个({1})", channelSysNos.Count, string.Join(",", arr)));
            //ChannelInventoryBaseBP bp1 = new ChannelProductPercentBP(context);
            //ChannelInventoryRun run1 = new ChannelInventoryRun(bp1.Run);
            Queue<ChannelInventoryRun> bizQueue = new Queue<ChannelInventoryRun>();
            Queue<IAsyncResult> asyncResultQueue = new Queue<IAsyncResult>();

            Util.WriteLog("开始渠道库存同步……");
            foreach (int channel in channelSysNos)
            {
                ChannelInventoryBaseBP biz = new ChannelProductPercentBP(context, channel);
                ChannelInventoryRun run = new ChannelInventoryRun(biz.Run);
                IAsyncResult result = run.BeginInvoke(null, null);
                bizQueue.Enqueue(run);
                asyncResultQueue.Enqueue(result);
            }


            //ChannelInventoryBaseBP bpTaoBao = new TaoBaoChannelProductPercentBP(context);
            //ChannelInventoryRun runTaoBo = new ChannelInventoryRun(bpTaoBao.Run);
            //IAsyncResult resultTaoBo = runTaoBo.BeginInvoke(null, null);

            //ChannelInventoryBaseBP bpDF = new DFChannelProductPercentBP(context);
            //ChannelInventoryRun runDF = new ChannelInventoryRun(bpDF.Run);
            //IAsyncResult resultDF = runDF.BeginInvoke(null, null);

            //ChannelInventoryBaseBP bpCM = new CMChannelProductPercentBP(context);
            //ChannelInventoryRun runCM = new ChannelInventoryRun(bpCM.Run);
            //IAsyncResult resultCM = runCM.BeginInvoke(null, null);

            //ChannelInventoryBaseBP bpJSB = new JSBChannelProductPercentBP(context);
            //ChannelInventoryRun runJSB = new ChannelInventoryRun(bpJSB.Run);
            //IAsyncResult resultJSB = runJSB.BeginInvoke(null, null);

            ChannelInventoryBaseBP bpAppoint = new ChannelProductAppointBP(context);
            ChannelInventoryRun runAppoint = new ChannelInventoryRun(bpAppoint.Run);
            IAsyncResult resultAppoint = runAppoint.BeginInvoke(null, null);

            try
            {
                runAppoint.EndInvoke(resultAppoint);
            }
            catch (Exception ex)
            {
                ExceptionHelper.HandleException(ex);
                Util.WriteLog(ex.Message, context, Config.Debug);
                Util.WriteLog(ex.StackTrace, context, Config.Debug);
            }

            while (bizQueue.Count > 0)
            {
                ChannelInventoryRun run = bizQueue.Dequeue();
                IAsyncResult result = asyncResultQueue.Dequeue();
                try
                {
                    run.EndInvoke(result);
                }
                catch (Exception ex)
                {
                    ExceptionHelper.HandleException(ex);
                    Util.WriteLog(ex.Message, context, Config.Debug);
                    Util.WriteLog(ex.StackTrace, context, Config.Debug);
                }
            }

            //try
            //{
            //    runJSB.EndInvoke(resultJSB);
            //}
            //catch (Exception ex)
            //{
            //    ExceptionHelper.HandleException(ex);
            //    Util.WriteLog(ex.Message, context, Config.Debug);
            //    Util.WriteLog(ex.StackTrace, context, Config.Debug);
            //}
            //try
            //{
            //    runCM.EndInvoke(resultCM);
            //}
            //catch (Exception ex)
            //{
            //    ExceptionHelper.HandleException(ex);
            //    Util.WriteLog(ex.Message, context, Config.Debug);
            //    Util.WriteLog(ex.StackTrace, context, Config.Debug);
            //}
            //try
            //{
            //    runDF.EndInvoke(resultDF);
            //}
            //catch (Exception ex)
            //{
            //    ExceptionHelper.HandleException(ex);
            //    Util.WriteLog(ex.Message, context, Config.Debug);
            //    Util.WriteLog(ex.StackTrace, context, Config.Debug);
            //}
            //try
            //{
            //    runTaoBo.EndInvoke(resultTaoBo);
            //}
            //catch (Exception ex)
            //{
            //    ExceptionHelper.HandleException(ex);
            //    Util.WriteLog(ex.Message, context, Config.Debug);
            //    Util.WriteLog(ex.StackTrace, context, Config.Debug);
            //}
            Util.WriteLog("渠道库存同步完成", context, Config.Debug);

            Util.WriteLog("Job退出", context, Config.Debug);

            if (Config.Debug)
            {
                ThreadPool.QueueUserWorkItem(new WaitCallback(WriteFile), context.Message);

            }
        }

        private static void WriteFile(object obj)
        {
            if (obj == null)
            {
                return;
            }
            string message = obj.ToString();

            string path = AppDomain.CurrentDomain.BaseDirectory;
            path += @"\Log\JobLog.log";
            //path = Regex.Replace(path, @"\\\", @"\");
            Util.WriteFile(path, message);
        }
    }
}
