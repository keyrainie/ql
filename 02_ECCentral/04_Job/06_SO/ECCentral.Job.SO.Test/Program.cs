using Newegg.Oversea.Framework.JobConsole.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECCentral.Job.SO.Test
{
    class Program
    {
        static void Main(string[] args)
        {
            //订单申报
            //new ECCentral.Job.SO.EasiPaySODeclare.Processor().Run(new JobContext());
            //顺丰快递job测试
            //new ECCentral.Job.SO.SFExpress.Processor().Run(new JobContext());
            //圆通快递job测试
            //new ECCentral.Job.SO.YTExpress.Processor().Run(new JobContext());
            //快递100
            new ECCentral.Job.SO.KD100Express.Processor().Run(new JobContext());
        }
    }
}
