using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newegg.Oversea.Framework.Entity;
using System.Data;

namespace IPP.Oversea.CN.ContentMgmt.Baidu.Entities
{
    public class CategoryManufacturerPathEntity
    {
      [DataMapping("PathSegment", DbType.String)]
      public string PathSegment
      {
          get;
          set;
      }

      [DataMapping("ManufacturerSegment", DbType.String)]
      public string ManufacturerSegment
      {
          get;
          set;
      }
    }
}
