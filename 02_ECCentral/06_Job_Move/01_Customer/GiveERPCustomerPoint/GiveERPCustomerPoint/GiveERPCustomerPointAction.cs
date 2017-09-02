using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newegg.Oversea.Framework.JobConsole.Client;
using GiveERPCustomerPoint.Biz;

namespace GiveERPCustomerPoint
{
    public class GiveERPCustomerPointAction : IJobAction
    {
        public void Run(JobContext context)
        {
            GiveERPCustomerPointBiz biz = new GiveERPCustomerPointBiz(context);
            biz.GivePoint();
            ReturnCrmPointActionBiz retrunBiz = new ReturnCrmPointActionBiz();
            retrunBiz.ReturnPoint();
        }

        static void Main(string[] args)
        {
            //送积分:
            GiveERPCustomerPointBiz biz = new GiveERPCustomerPointBiz();
            biz.GivePoint();
            //退积分：
            ReturnCrmPointActionBiz retrunBiz = new ReturnCrmPointActionBiz();
            retrunBiz.ReturnPoint();
        }
    }
}
