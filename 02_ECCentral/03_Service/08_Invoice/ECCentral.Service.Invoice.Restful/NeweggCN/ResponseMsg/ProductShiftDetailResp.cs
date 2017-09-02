using System;
using System.Collections.Generic;
using ECCentral.BizEntity.Inventory;

namespace ECCentral.Service.Invoice.Restful.NeweggCN.ResponseMsg
{
    public class ProductShiftDetailResp
    {
        public int TotalCount
        {
            get;
            set;
        }

        public List<ProductShiftDetail> Data
        {
            get;
            set;
        }

        public ProductShiftDetailAmtInfo OutAmt
        {
            get;
            set;
        }

        public ProductShiftDetailAmtInfo InAmt
        {
            get;
            set;
        }

        public ProductShiftDetail NeedManualItem
        {
            get;
            set;
        }
    }
}
