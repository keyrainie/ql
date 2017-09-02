using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.Utility;
using ECCentral.Service.Common.IDataAccess;

namespace ECCentral.Service.Common.BizProcessor
{
    [VersionExport(typeof(EmailProcessor))]
    public class EmailProcessor
    {
        public virtual void SendAsyncMail(MailMessage msg)
        {
            ObjectFactory<IEmailDA>.Instance.InsertMail(msg);
        }

        public virtual void SendAsyncMailInternal(MailMessage msg)
        {
            ObjectFactory<IEmailDA>.Instance.InsertInternalMail(msg);
        }


    }


}
