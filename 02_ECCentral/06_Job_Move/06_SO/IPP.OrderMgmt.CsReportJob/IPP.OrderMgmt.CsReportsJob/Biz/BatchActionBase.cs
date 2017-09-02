using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newegg.Oversea.Framework.JobConsole.Client;

namespace IPP.ThirdPart.JobV31.Biz
{
    public abstract class BatchActionBase<T>
    {
        public abstract void DoAction(List<T> list);

        public JobContext Context;

        public void BatchRun(List<T> list, int countNum)
        {
            if (list == null || list.Count == 0)
            {
                return;
            }

            List<T> actionList;

            for (int indexNum = 0; indexNum < list.Count; indexNum += countNum)
            {
                actionList = list.Skip(indexNum).Take(countNum).ToList<T>();
                DoAction(actionList);
            }
        }
    }
}
