using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newegg.Oversea.Framework.Entity;
using System.Data;

namespace IPP.ECommerceMgmt.AutoInnerOnlineList.BusinessEntities
{
    [Serializable]
    public class Domain
    {
        [DataMapping("DomainCode", DbType.String)]
        public string DomainCode { get; set; }

        [DataMapping("DomainName", DbType.String)]
        public string DomainName { get; set; }

        /// <summary>
        /// pageid
        /// </summary>
        [DataMapping("C1List", DbType.String)]
        public string C1List { get; set; }

        [DataMapping("ExceptC3List", DbType.String)]
        public string ExceptC3List { get; set; }

        /// <summary>
        /// pagetype
        /// </summary>
        public int DomianValue
        {
            get
            {
                return Convert.ToInt32(DomainCode);
            }
        }
    }
}
