using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.Invoice;

namespace ECCentral.Service.Invoice.Restful.ResponseMsg
{
    public class ImportPostIncomeResp
    {
        public List<PostIncomeInfo> SuccessList;

        public List<ImportPostIncome> FaultList;

        public string Message;
    }
}
