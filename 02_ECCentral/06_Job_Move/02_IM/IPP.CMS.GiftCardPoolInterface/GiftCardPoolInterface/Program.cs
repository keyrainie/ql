using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ContentMgmt.GiftCardPoolInterface.DA;
using ContentMgmt.GiftCardPoolInterface.Entities;

namespace ContentMgmt.GiftCardPoolInterface
{
    class Program
    {
        static void Main(string[] args)
        {

            BizProcess.BizLogFile = "Log\\biz2.log";
            //Singleton<BizProcess>.Instance.Process();
            new BizProcess().Process();

        }
    }
}
