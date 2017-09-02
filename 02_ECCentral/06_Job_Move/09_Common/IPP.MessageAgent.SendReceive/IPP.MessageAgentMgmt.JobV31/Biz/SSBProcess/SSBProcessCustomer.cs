using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Newegg.Oversea.Framework.JobConsole.Client;
using IPP.MessageAgent.SendReceive.JobV31.Configuration;
using IPP.MessageAgent.SendReceive.JobV31.Utilities;

namespace IPP.MessageAgent.SendReceive.JobV31.Biz
{
    public class SSBProcessCustomer : SSBProcessBase
    {
        public SSBProcessCustomer(SSBChannel ssbChannel)
            : base(ssbChannel)
        { }

        public override string GetAcitonType(string message)
        {
            message=IPP.Framework.Utility.XmlUtils.RemoveNameSpace(message);

            string actionType = XMLHelper.SelectSingleNode(_ssbChannel.ActionTypeXpath, message);
            return actionType;
        }

        public override string GetReferenceKey(string message, SSBProcesser processer)
        {
            message = IPP.Framework.Utility.XmlUtils.RemoveNameSpace(message);

            string referenceKey = XMLHelper.SelectSingleNode(processer.ReferenceKeyXPath, message);
            return referenceKey;
        }
    }
}
