using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IPP.OrderMgmt.JobV31.AppEnum;

namespace IPP.OrderMgmt.JobV31.BusinessEntities
{
  public  class CustomerRankAmtLimitEntity
    {
      public int CustomerRank { get; set; }

      public decimal AmtLimit { get; set; }
    }
}
