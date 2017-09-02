using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Newegg.Oversea.Framework.JobConsole.Client;
using IPP.MessageAgent.SendReceive.JobV31.Configuration;

namespace IPP.MessageAgent.SendReceive.JobV31.Biz
{
    public class SSBProcessFactory
    {
        public static SSBProcessBase Create(SSBChannel ssbChannel)
        {
            switch (ssbChannel.Version)
            {
                case SSBVersion.V1:
                    return new SSBProcessV1(ssbChannel);
                case SSBVersion.V2:
                    return new SSBProcessV2(ssbChannel);
                case SSBVersion.V3:
                    return new SSBProcessV3(ssbChannel);
                case SSBVersion.Customer:
                    return new SSBProcessCustomer(ssbChannel);
                default:
                    return new SSBProcessV2(ssbChannel);
            }
        }
    }
}
