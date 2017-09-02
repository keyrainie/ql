using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newegg.Oversea.Framework.JobConsole.Client;
using IPP.MessageAgent.SendReceive.JobV31.Configuration;

namespace IPP.MessageAgent.SendReceive.JobV31.Biz
{
    public class SSBProcessV1 : SSBProcessBase
    {
        public SSBProcessV1(SSBChannel ssbChannel)
            : base(ssbChannel)
        { }

        public override string GetAcitonType(string message)
        {
            throw new NotImplementedException();
        }

        public override string GetReferenceKey(string message, IPP.MessageAgent.SendReceive.JobV31.Configuration.SSBProcesser processer)
        {
            throw new NotImplementedException();
        }
    }
}
