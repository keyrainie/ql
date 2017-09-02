using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IPP.OrderMgmt.CsReportsJob.Biz.InternalMemo;
using IPP.ThirdPart.JobV31.BusinessEntities.IngramMicro;
using Newegg.Oversea.Framework.JobConsole.Client;

namespace IPP.OrderMgmt.Host
{
    class Program
    {
        static void Main(string[] args)
        {
            //List<InternalMemoEntity> Imemos = new List<InternalMemoEntity>();

            //Imemos = BuiltTestCase01();

            //InternalMemoBiz biz = new InternalMemoBiz();
            //List<InternalMemoReport> reports = biz.GetReports(Imemos);

            //biz.SendMail(reports);
            //foreach (var report in reports)
            //{
            //    Console.WriteLine(string.Format("{0}\t{1}\t{2}\t{3}\t{4}",
            //        report.UserName, 
            //        report.ResolvedCount, 
            //        report.UnResolvedCount, 
            //        report.Count, 
            //        report.ResolvedRate));
            //}

            InternalMemoBiz biz = new InternalMemoBiz();
            JobContext context = new JobContext();
            biz.Run(context);

            Console.WriteLine("sucessed!");
            Console.ReadLine();
        }

        public static  List<InternalMemoEntity> BuiltTestCase01()
        {
            List<InternalMemoEntity> Imemos = new List<InternalMemoEntity>();

            InternalMemoEntity memo = new InternalMemoEntity();
            memo.SOSysNo = 10001;
            memo.UserSysNo = 101;
            memo.UserName = "sherry";
            memo.InDate = DateTime.Parse("2011-6-20 13:00");
            Imemos.Add(memo);

            memo = new InternalMemoEntity();
            memo.SOSysNo = 10002;
            memo.UserSysNo = 101;
            memo.UserName = "sherry";
            memo.InDate = DateTime.Parse("2001-6-20  14:00:00");
            Imemos.Add(memo);

            memo = new InternalMemoEntity();
            memo.SOSysNo = 10002;
            memo.UserSysNo = 101;
            memo.UserName = "sherry";
            memo.InDate = DateTime.Parse("2001-6-21  14:00:00");
            Imemos.Add(memo);

            memo = new InternalMemoEntity();
            memo.SOSysNo = 10003;
            memo.UserSysNo = 101;
            memo.UserName = "sherry";
            memo.InDate = DateTime.Parse("2001-6-20  14:00:00");
            Imemos.Add(memo);

            memo = new InternalMemoEntity();
            memo.SOSysNo = 10003;
            memo.UserSysNo = 102;
            memo.UserName = "ida";
            memo.InDate = DateTime.Parse("2001-6-21  14:00:00");
            Imemos.Add(memo);

            return Imemos;
        }

        public static List<InternalMemoEntity> BuiltTestCase02()
        {
            List<InternalMemoEntity> Imemos = new List<InternalMemoEntity>();

            InternalMemoEntity memo = new InternalMemoEntity();
            memo.SOSysNo = 10001;
            memo.UserSysNo = 101;
            memo.UserName = "sherry";
            memo.InDate = DateTime.Parse("2011-6-20 13:00");
            Imemos.Add(memo);

            memo = new InternalMemoEntity();
            memo.SOSysNo = 10002;
            memo.UserSysNo = 101;
            memo.UserName = "sherry";
            memo.InDate = DateTime.Parse("2001-6-20  14:00:00");
            Imemos.Add(memo);

            memo = new InternalMemoEntity();
            memo.SOSysNo = 10002;
            memo.UserSysNo = 101;
            memo.UserName = "sherry";
            memo.InDate = DateTime.Parse("2001-6-21  14:00:00");
            Imemos.Add(memo);

            memo = new InternalMemoEntity();
            memo.SOSysNo = 10003;
            memo.UserSysNo = 101;
            memo.UserName = "sherry";
            memo.InDate = DateTime.Parse("2001-6-20  14:00:00");
            Imemos.Add(memo);

            memo = new InternalMemoEntity();
            memo.SOSysNo = 10003;
            memo.UserSysNo = 102;
            memo.UserName = "ida";
            memo.InDate = DateTime.Parse("2001-6-21  14:00:00");
            Imemos.Add(memo);

            memo = new InternalMemoEntity();
            memo.SOSysNo = 10003;
            memo.UserSysNo = 101;
            memo.UserName = "sherry";
            memo.InDate = DateTime.Parse("2001-6-23  14:00:00");
            Imemos.Add(memo);

            return Imemos;
        }
    }
}
