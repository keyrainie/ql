using ECCentral.QueryFilter.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECCentral.QueryFilter.Inventory
{
   public class ExperienceHallInventoryInfoQueryFilter
    {
       public PagingInfo PagingInfo { get; set; }
       /// <summary>
       /// 商品一级类别编号
       /// </summary>
       public int? C1SysNo { get; set; }
       /// <summary>
       /// 商品二级类别编号
       /// </summary>
       public int? C2SysNo { get; set; }
       /// <summary>
       /// 商品三级类别编号
       /// </summary>
       public int? C3SysNo { get; set; }

       /// <summary>
       ///  商品SysNo
       /// </summary>
       public int? ProductSysNo { get; set; }

       //商品名称
       public string ProductName { get; set; }


       public string CompanyCode { get; set; }

    }
}
