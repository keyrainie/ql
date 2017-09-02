using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IPP.InventoryMgmt.JobV31.BusinessEntities
{
    public class MailDataEntity
    {
        public List<TaobaoProduct> ThirdPartMappingNotExists { get; set; }

        public List<ThirdPartInventoryEntity> TaobaoProductNotExists { get; set; }

        public Dictionary<ThirdPartInventoryEntity, TaobaoProduct> InventoryQtyNotEquels { get; set; }
    }

    public class MailEntity
    {
        public string From { get; set; }

        public string CC { get; set; }

        public string To { get; set; }

        public string Subject { get; set; }

        public string Body { get; set; }

    }
}
