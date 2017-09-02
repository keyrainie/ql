using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.QueryFilter.Common;
using ECCentral.BizEntity.ExternalSYS;

namespace ECCentral.QueryFilter.ExternalSYS
{
    public class AdvertisingQueryFilter
    {
       public PagingInfo PageInfo { get; set; }

       public int? ProductLineCategorySysNo { get; set; }

       public int? ProductLineSysNo { get; set; }

       public int? ImageWidth { get; set; }

       public int? ImageHeight { get; set; }

       public AdvertisingType? Type { get; set; }

       public DateTime? OperateDateFrom { get; set; }

       public DateTime? OperateDateTo { get; set; }

       public string InUser { get; set; }

       public string ImageSize
       {
           get
           {
               return this.ImageWidth + "*" + this.ImageHeight;
           }
           set
           {
               if (!string.IsNullOrEmpty(value) && value != "*")
               {
                   string[] dimission = value.Split('*');
                   this.ImageWidth = int.Parse(dimission[0]);
                   this.ImageHeight = int.Parse(dimission[1]);
               }
           }
       }
    }
}
