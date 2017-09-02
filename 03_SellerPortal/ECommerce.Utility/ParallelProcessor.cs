using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ECommerce.Utility
{
    public class ParallelProcessError
    {
        internal ParallelProcessError(int index, Exception ex)
        {
            Exception = ex;
            Index = index;
        }

        public Exception Exception { get; private set; }
        public int Index { get; private set; }
    }

    public class ParallelProcessResult<R>
    {
        internal ParallelProcessResult(R[] resultList, List<ParallelProcessError> errorList)
        {
            FuncResultList = resultList;
            Errors = errorList;
        }

        public R[] FuncResultList { get; private set; }
        public List<ParallelProcessError> Errors { get; private set; }
    }

    public static class ParallelProcessor
    {
        #region Private Member

        private class TransferObject<T>
        {
            public IContext Context;
            public Action<T> ActionHandler;
            public IQueue<T> Queue;
            public Exception[] ExceptionArray;
            public int AliveThreadCount;
            public bool Blocking;
            public Action<List<ParallelProcessError>> Callback;
        }

        private static void ProcessTask<T>(TransferObject<T> obj)
        {
            //将主线程的ServiceContext附加到当前的ServiceContext
            ContextManager.Current.Attach(obj.Context);
            T data;
            int index = obj.Queue.Dequeue(out data);
            while (index >= 0)
            {
                try
                {
                    obj.ActionHandler(data);
                }
                catch (Exception ex)
                {
                    obj.ExceptionArray[index] = ex;
                }
                index = obj.Queue.Dequeue(out data);
            }
            if (obj.Blocking == false && obj.Callback != null && Interlocked.Decrement(ref obj.AliveThreadCount) == 0)
            {
                obj.Callback(BuildErrorInfoList(obj.ExceptionArray));
            }
        }

        private class TransferObject<T, R>
        {
            public IContext Context;
            public Func<T, R> FuncHandler;
            public IQueue<T> Queue;
            public Exception[] ExceptionArray;
            public R[] ResultList;
            public int AliveThreadCount;
            public bool Blocking;
            public Action<ParallelProcessResult<R>> Callback;
        }

        private static void ProcessTask<T, R>(TransferObject<T, R> obj)
        {
            //将主线程的ServiceContext附加到当前的ServiceContext
            ContextManager.Current.Attach(obj.Context);
            T data;
            int index = obj.Queue.Dequeue(out data);
            while (index >= 0)
            {
                try
                {
                    obj.ResultList[index] = obj.FuncHandler(data);
                }
                catch (Exception ex)
                {
                    obj.ExceptionArray[index] = ex;
                }
                index = obj.Queue.Dequeue(out data);
            }
            if (obj.Blocking == false && obj.Callback != null && Interlocked.Decrement(ref obj.AliveThreadCount) == 0)
            {
                obj.Callback(new ParallelProcessResult<R>(obj.ResultList, BuildErrorInfoList(obj.ExceptionArray)));
            }
        }

        private static IQueue<T> CreateQueue<T>(T[] data)
        {
            LocalMemoryQueue<T> queue = new LocalMemoryQueue<T>(data.Length);
            for (int i = 0; i < data.Length; i++)
            {
                queue.Enqueue(data[i]);
            }
            return queue;
        }

        private static List<ParallelProcessError> BuildErrorInfoList(Exception[] array)
        {
            if (array == null || array.Length <= 0)
            {
                return new List<ParallelProcessError>(0);
            }
            List<ParallelProcessError> list = new List<ParallelProcessError>(array.Length);
            for (int i = 0; i < array.Length; i++)
            {
                if (array[i] != null)
                {
                    list.Add(new ParallelProcessError(i, array[i]));
                }
            }
            return list;
        }

        #endregion

        public static List<ParallelProcessError> Process<T>(T[] list, Action<T> actionHandler)
        {
            return Process<T>(list, actionHandler, Environment.ProcessorCount);
        }

        public static List<ParallelProcessError> Process<T>(T[] list, Action<T> actionHandler, int threadCount)
        {
            if (actionHandler == null)
            {
                throw new ArgumentNullException("funcHandler");
            }
            if (threadCount <= 0)
            {
                throw new ArgumentException("Error with the threadcount '" + threadCount + "'. The thread count must be larger than 0.", "threadCount");
            }
            if (list == null || list.Length <= 0)
            {
                return new List<ParallelProcessError>(0);
            }
            TransferObject<T> obj = new TransferObject<T>()
            {
                Context = ContextManager.Current,
                ActionHandler = actionHandler,
                Queue = CreateQueue(list),
                ExceptionArray = new Exception[list.Length],
                AliveThreadCount = threadCount,
                Blocking = true,
                Callback = null
            };
            Task[] tasks = new Task[threadCount - 1];
            for (int i = 0; i < threadCount - 1; i++)
            {
                tasks[i] = Task.Factory.StartNew(c => ProcessTask<T>((TransferObject<T>)c), obj);
            }
            ProcessTask<T>(obj);
            if (tasks.Length > 0)
            {
                Task.WaitAll(tasks);
            }
            return BuildErrorInfoList(obj.ExceptionArray);
        }

        public static ParallelProcessResult<R> Process<T, R>(T[] list, Func<T, R> funcHandler)
        {
            return Process<T, R>(list, funcHandler, Environment.ProcessorCount);
        }

        public static ParallelProcessResult<R> Process<T, R>(T[] list, Func<T, R> funcHandler, int threadCount)
        {
            if (funcHandler == null)
            {
                throw new ArgumentNullException("funcHandler");
            }
            if (threadCount <= 0)
            {
                throw new ArgumentException("Error with the threadcount '" + threadCount + "'. The thread count must be larger than 0.", "threadCount");
            }
            if (list == null)
            {
                return null;
            }
            if (list.Length <= 0)
            {
                return new ParallelProcessResult<R>(new R[0], new List<ParallelProcessError>(0));
            }
            TransferObject<T, R> obj = new TransferObject<T, R>()
            {
                Context = ContextManager.Current,
                FuncHandler = funcHandler,
                Queue = CreateQueue(list),
                ResultList = new R[list.Length],
                ExceptionArray = new Exception[list.Length],
                AliveThreadCount = threadCount,
                Blocking = true,
                Callback = null
            };
            Task[] tasks = new Task[threadCount - 1];
            for (int i = 0; i < threadCount - 1; i++)
            {
                tasks[i] = Task.Factory.StartNew(c => ProcessTask<T, R>((TransferObject<T, R>)c), obj);
            }
            ProcessTask<T, R>(obj);
            if (tasks.Length > 0)
            {
                Task.WaitAll(tasks);
            }
            return new ParallelProcessResult<R>(obj.ResultList, BuildErrorInfoList(obj.ExceptionArray));
        }

        public static void NonblockingProcess<T, R>(T[] list, Func<T, R> funcHandler, Action<ParallelProcessResult<R>> callback)
        {
            NonblockingProcess(list, funcHandler, Environment.ProcessorCount, callback);
        }

        public static void NonblockingProcess<T, R>(T[] list, Func<T, R> funcHandler, int threadCount, Action<ParallelProcessResult<R>> callback)
        {
            if (funcHandler == null)
            {
                throw new ArgumentNullException("funcHandler");
            }
            if (threadCount <= 0)
            {
                throw new ArgumentException("Error with the threadcount '" + threadCount + "'. The thread count must be larger than 0.", "threadCount");
            }
            if (list == null)
            {
                if (callback != null)
                {
                    callback(new ParallelProcessResult<R>(null, new List<ParallelProcessError>(0)));
                }
                return;
            }
            if (list.Length <= 0)
            {
                if (callback != null)
                {
                    callback(new ParallelProcessResult<R>(new R[0], new List<ParallelProcessError>(0)));
                }
                return;
            }
            TransferObject<T, R> obj = new TransferObject<T, R>()
            {
                Context = ContextManager.Current,
                FuncHandler = funcHandler,
                Queue = CreateQueue(list),
                ResultList = new R[list.Length],
                ExceptionArray = new Exception[list.Length],
                AliveThreadCount = threadCount,
                Blocking = false,
                Callback = callback
            };
            for (int i = 0; i < threadCount; i++)
            {
                Task.Factory.StartNew(c => ProcessTask<T, R>((TransferObject<T, R>)c), obj);
            }
        }

        public static void NonblockingProcess<T>(T[] list, Action<T> actionHandler, Action<List<ParallelProcessError>> callback = null)
        {
            NonblockingProcess(list, actionHandler, Environment.ProcessorCount, callback);
        }

        public static void NonblockingProcess<T>(T[] list, Action<T> actionHandler, int threadCount, Action<List<ParallelProcessError>> callback = null)
        {
            if (actionHandler == null)
            {
                throw new ArgumentNullException("funcHandler");
            }
            if (threadCount <= 0)
            {
                throw new ArgumentException("Error with the threadcount '" + threadCount + "'. The thread count must be larger than 0.", "threadCount");
            }
            if (list == null || list.Length <= 0)
            {
                if (callback != null)
                {
                    callback(new List<ParallelProcessError>(0));
                }
                return;
            }
            TransferObject<T> obj = new TransferObject<T>()
            {
                Context = ContextManager.Current,
                ActionHandler = actionHandler,
                Queue = CreateQueue(list),
                ExceptionArray = new Exception[list.Length],
                AliveThreadCount = threadCount,
                Blocking = false,
                Callback = callback
            };
            for (int i = 0; i < threadCount; i++)
            {
                Task.Factory.StartNew(c => ProcessTask<T>((TransferObject<T>)c), obj);
            }
        }
    }
}
