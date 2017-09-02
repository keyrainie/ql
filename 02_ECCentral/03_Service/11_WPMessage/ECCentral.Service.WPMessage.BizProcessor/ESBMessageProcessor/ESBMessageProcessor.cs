using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.EventMessage.SO;
using ECCentral.Service.Utility;

namespace ECCentral.Service.WPMessage.BizProcessor
{
    public class Task
    {
        public int CategorySysNo { get; set; }
        public string BizSysNo { get; set; }
        public string UrlParameter { get; set; }
        public string Memo { get; set; }
        public int CurrentUserSysNo { get; set; }
    }

    public interface IESBMessageProcessor
    {
        void Process(ESBMessage message);
    }

    public abstract class ESBMessageProcessor<T> : IESBMessageProcessor
    {
        public void Process(ESBMessage message)
        {
            if (message == null)
            {
                return;
            }
            T msg = message.GetData<T>();
            if (msg == null)
            {
                return;
            }
            if (!Check(msg))
            {
                return;
            }
            if (NeedProcess(msg) == false)
            {
                return;
            }

            Task task = Convert(message.MessageID, message.Subject, msg);
            Save(task);
        }

        protected virtual bool Check(T msg)
        {
            int categorySysNo = GetCategorySysNo();
            string bizSysNo = GetBizSysNo(msg);
            List<ECCentral.WPMessage.BizEntity.WPMessage> list = ObjectFactory<WPMessage.BizProcessor.WPMessageProcessor>.Instance.GetWPMessage(categorySysNo, bizSysNo);
            if (list == null || list.Count == 0)
            {
                return true;
            }
            return !list.Exists(m => m.Status != ECCentral.WPMessage.BizEntity.WPMessageStatus.Completed);
        }

        protected virtual bool NeedProcess(T msg)
        {
            return true;
        }

        protected virtual Task Convert(string msgID, string subject, T msg)
        {
            return new Task
            {
                CategorySysNo = GetCategorySysNo(),
                BizSysNo = GetBizSysNo(msg),
                UrlParameter = GetUrlParameter(msg),
                Memo = GetMemo(msg),
                CurrentUserSysNo = GetCurrentUserSysNo(msg)
            };
        }

        protected abstract int GetCategorySysNo();

        protected abstract string GetBizSysNo(T msg);

        protected abstract string GetUrlParameter(T msg);

        protected abstract string GetMemo(T msg);

        protected abstract int GetCurrentUserSysNo(T msg);

        protected abstract void Save(Task task);
    }
    /// <summary>
    /// 生成待办事项
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class WPMessageCreator<T> : ESBMessageProcessor<T>
    {
        protected override void Save(Task task)
        {
            ObjectFactory<WPMessage.BizProcessor.WPMessageProcessor>.Instance.AddWPMessage(task.CategorySysNo,
                task.BizSysNo, task.UrlParameter, task.CurrentUserSysNo, task.Memo);
        }
    }

    /// <summary>
    /// 完成待办事项
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class WPMessageCompleter<T> : ESBMessageProcessor<T>
    {
        protected override bool Check(T msg)
        {
            return true;
        }

        protected override void Save(Task task)
        {
            ObjectFactory<WPMessage.BizProcessor.WPMessageProcessor>.Instance.CompleteWPMessage(task.CategorySysNo,
                task.BizSysNo, task.CurrentUserSysNo, task.Memo);
        }

        protected override string GetUrlParameter(T msg)
        {
            return null;
        }

        protected override string GetMemo(T msg)
        {
            return null;
        }
    }
}
