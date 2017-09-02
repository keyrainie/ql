using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ECCentral.BizEntity;
using ECCentral.BizEntity.Invoice;
using ECCentral.Service.Utility;

namespace ECCentral.Service.Customer.AppService
{
    /// <summary>
    /// 批量操作管理，提供简单的多任务处理
    /// </summary>
    internal class BatchActionManager
    {
        /// <summary>
        /// 执行批量操作
        /// </summary>
        /// <typeparam name="T">批量操作项类型</typeparam>
        /// <param name="request">执行批量操作请求数据列表</param>
        /// <param name="doAction">需要对每个数据项执行的操作</param>
        /// <returns>批量操作结果</returns>
        internal static BatchActionResult<T> DoBatchAction<T>(List<BatchActionItem<T>> request, Action<T> doAction)
        {
            if (request == null || request.Count == 0)
            {
                return new BatchActionResult<T>(0);
            }
            BizException unknowException = new BizException(ResouceManager.GetMessageString("SO.SOInfo", "SO_Action_Unknow"));
            var result = new BatchActionResult<T>(request.Count);
            string cultureName = Thread.CurrentThread.CurrentCulture.Name;
            List<Task> tasks = new List<Task>();
            for (int i = 0; i < request.Count; i++)
            {
                int index = i;
                var task = Task.Factory.StartNew(() =>
                {
                    try
                    {
                        Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo(cultureName);
                        doAction(request[index].Data);
                        result.SuccessList.Add(request[index]);
                    }
                    catch (BizException exp)
                    {
                        result.FaultList.Add(new FaultTask<BatchActionItem<T>>(request[index], exp));
                    }
                    catch (Exception ex)
                    {
                        ExceptionHelper.HandleException(ex);
                        result.FaultList.Add(new FaultTask<BatchActionItem<T>>(request[index], unknowException));
                    }
                });
                tasks.Add(task);
            }
            //阻塞，直到所有任务完成
            Task.WaitAll(tasks.ToArray());

            return result;
        }
    }

    /// <summary>
    /// 批量操作数据项,这是数据值和数据标识的组合
    /// </summary>
    /// <typeparam name="T"></typeparam>
    internal class BatchActionItem<T>
    {
        /// <summary>
        /// 数据项标识
        /// </summary>
        internal string ID
        {
            get;
            set;
        }

        /// <summary>
        /// 数据项数据
        /// </summary>
        internal T Data
        {
            get;
            set;
        }
    }

    /// <summary>
    /// 批量处理返回结果
    /// </summary>
    /// <typeparam name="T"></typeparam>
    internal class BatchActionResult<T>
    {
        internal BatchActionResult(int totalCount)
        {
            TotalCount = totalCount;
            SuccessList = new List<BatchActionItem<T>>();
            FaultList = new List<FaultTask<BatchActionItem<T>>>();
        }

        /// <summary>
        /// 总记录数
        /// </summary>
        internal int TotalCount
        {
            get;
            private set;
        }

        /// <summary>
        /// 执行成功列表
        /// </summary>
        internal List<BatchActionItem<T>> SuccessList
        {
            get;
            private set;
        }

        /// <summary>
        /// 执行失败列表
        /// </summary>
        internal List<FaultTask<BatchActionItem<T>>> FaultList
        {
            get;
            private set;
        }
        private const string MessagePath_SOInfo = "SO.SOInfo";
        internal void ThrowErrorException()
        {
            if (FaultList != null && FaultList.Count > 0)
            {
                StringBuilder msgBuilder = new StringBuilder();
                msgBuilder.AppendLine(String.Format(ResouceManager.GetMessageString(MessagePath_SOInfo, "SO_Action_Batch_Result"), TotalCount, SuccessList == null ? 0 : SuccessList.Count, FaultList.Count));
                string failedMessageTemplate = ResouceManager.GetMessageString(MessagePath_SOInfo, "SO_Action_FailedMsg");
                StringBuilder failedMessageBuilder = new StringBuilder();
                failedMessageBuilder.AppendLine();
                foreach (FaultTask<BatchActionItem<T>> failedSO in FaultList)
                {
                    failedMessageBuilder.AppendLine(String.Format(failedMessageTemplate, failedSO.FaultItem.ID, failedSO.FaultException.Message));
                }
                msgBuilder.AppendLine(String.Format(ResouceManager.GetMessageString(MessagePath_SOInfo, "SO_Action_Batch_Failed"), failedMessageBuilder));
                throw new BizException(msgBuilder.ToString());
            }
        }
    }

    /// <summary>
    /// 失败任务
    /// </summary>
    /// <typeparam name="T"></typeparam>
    internal class FaultTask<T>
    {
        internal FaultTask(T item, Exception exp)
        {
            FaultItem = item;
            FaultException = exp;
        }

        /// <summary>
        /// 失败数据项
        /// </summary>
        internal T FaultItem
        {
            get;
            private set;
        }

        /// <summary>
        /// 异常信息
        /// </summary>
        internal Exception FaultException
        {
            get;
            private set;
        }
    }
}