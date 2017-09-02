using GetEPortSOJob.BusinessEntities;
using GetEPortSOJob.Dac.Common;
using GetEPortSOJob.Utilities;
using Newegg.Oversea.Framework.JobConsole.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GetEPortSOJob.Providers
{
    public class GetEPortSOJob : IJobAction
    {
        public void Run(JobContext context)
        {
            
            //宁波跨境贸易电子商务平台
            // 申报单状态查询（根据更新时间）
            //假设有5000页，从1开始计算，每页1000条纪录
            for (int i = 0; i < 5000; i++)
            {
                Message mess = new Message();
                mess = NingBoAPI.GetSOAndUpdateStatus(context, i + 1);
                if (mess.header.NextPage != "T")
                {
                    break;
                }
            }
            Console.WriteLine("跳出来啦");
        }
    }
}
