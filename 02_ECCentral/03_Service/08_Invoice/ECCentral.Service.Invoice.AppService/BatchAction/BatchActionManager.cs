using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ECCentral.Service.Utility;

namespace ECCentral.Service.Invoice.AppService
{
    /// <summary>
    /// 批量操作管理，提供简单的多任务处理
    /// </summary>
    public class BatchActionManager
    {
        /// <summary>
        /// 执行批量操作
        /// </summary>
        /// <typeparam name="T">批量操作项类型</typeparam>
        /// <param name="request">执行批量操作请求数据列表</param>
        /// <param name="doAction">需要对每个数据项执行的操作</param>
        /// <returns>批量操作结果</returns>
        public static BatchActionResult<T> DoBatchAction<T>(List<BatchActionItem<T>> request, Action<T> doAction)
        {
            if (request == null || request.Count == 0)
            {
                return new BatchActionResult<T>(0);
            }
            var result = new BatchActionResult<T>(request.Count);
            string cultureName = Thread.CurrentThread.CurrentCulture.Name;
            List<Task> tasks = new List<Task>();
            request.ForEach(r =>
            {
                var item = r;
                IContext c = ServiceContext.Current;
                var task = Task.Factory.StartNew(() =>
                {
                    try
                    {                      
                        ServiceContext.Current.Attach(c);                     
                        Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo(cultureName);
                        doAction(item.Data);
                        result.SuccessList.Add(item);
                    }
                    catch (ECCentral.BizEntity.BizException exp)
                    {
                        lock (result.FaultList)
                        {
                            result.FaultList.Add(new FaultTask<BatchActionItem<T>>(item, exp));
                        }
                    }
                    catch (Exception e)
                    {
                        lock (result.FaultList)
                        {
                            //记录异常信息
                            ECCentral.Service.Utility.ExceptionHelper.HandleException(e);
                            result.FaultList.Add(new FaultTask<BatchActionItem<T>>(item, e));
                        }
                    }
                });
                tasks.Add(task);
            });
            //阻塞，直到所有任务完成
            Task.WaitAll(tasks.ToArray());

            return result;
        }
    }
}