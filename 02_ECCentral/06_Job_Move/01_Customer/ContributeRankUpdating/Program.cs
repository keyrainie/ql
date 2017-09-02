using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ContributeRankUpdating
{
    class Program
    {
        static void Main(string[] args)
        {
            Biz biz = new Biz();
            biz.Run(new Newegg.Oversea.Framework.JobConsole.Client.JobContext());
        }
    }
}
