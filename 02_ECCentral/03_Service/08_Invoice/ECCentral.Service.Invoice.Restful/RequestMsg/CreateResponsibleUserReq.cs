using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.Invoice;

namespace ECCentral.Service.Invoice.Restful.RequestMsg
{
    public class CreateResponsibleUserReq
    {
        public ResponsibleUserInfo ResponsibleUser
        {
            get;
            set;
        }

        public bool? OverrideWhenCreate
        {
            get;
            set;
        }
    }
}