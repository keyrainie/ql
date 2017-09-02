using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.Utility;

namespace ECCentral.Service.Common.IDataAccess
{
    public interface IEmailDA
    {
        MailMessage InsertMail(MailMessage mailInfo);

        MailMessage InsertInternalMail(MailMessage mailInfo);
    }
}
