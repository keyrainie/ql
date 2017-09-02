using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECommerce.Entity.GiftCard
{
    public class MyGiftCardListView
    {
        public QueryResult<GiftCardInfo> MyList { get; set; }

        public QueryResult<GiftCardInfo> MyBindingList { get; set; }
    }
}
