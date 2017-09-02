using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

using Newegg.Oversea.Framework.JobConsole.Client;
using IPP.MessageAgent.SendReceive.JobV31.Configuration;
using IPP.MessageAgent.SendReceive.JobV31.Utilities;

namespace IPP.MessageAgent.SendReceive.JobV31.Biz
{
    public class SSBProcessV3 : SSBProcessBase
    {
        private const string ActionTypeXPath = "//MessageHead/Type/text()";

        public SSBProcessV3(SSBChannel ssbChannel)
            : base(ssbChannel)
        { }

        public override string GetAcitonType(string message)
        {
            string actionType = XMLHelper.SelectSingleNode(ActionTypeXPath, message);
            return actionType;
        }

        public override string GetReferenceKey(string message, SSBProcesser processer)
        {
            string referenceKey = XMLHelper.SelectSingleNode(processer.ReferenceKeyXPath, message);
            return referenceKey;
        }
    }
}
