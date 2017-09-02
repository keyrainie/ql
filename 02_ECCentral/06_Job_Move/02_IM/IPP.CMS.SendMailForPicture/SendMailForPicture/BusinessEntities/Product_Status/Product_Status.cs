using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

using Newegg.Oversea.Framework.Entity;

namespace IPP.ContentMgmt.SendMailForPicture.BusinessEntities
{
    [Serializable]
    public class ProductList
    {
        [DataMapping("c3name", DbType.String)]
        public string c3name
        {
            get;
            set;
        }
        [DataMapping("ProductID", DbType.String)]
        public string ProductID
        {
            get;
            set;
        }
        [DataMapping("ProductName", DbType.String)]
        public string ProductName
        {
            get;
            set;
        }
    }
}
