using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newegg.Oversea.Framework.Entity;
using System.Data;

namespace Newegg.Oversea.Silverlight.ControlPanel.Service.BizEntities
{
    public class LocalizedResEntity
    {
        [DataMapping("TransactionNumber", DbType.Int32)]
        public int TransactionNumber { get; set; }

        [DataMapping("ReferenceMenuId", DbType.String)]
        public string ReferenceMenuId { get; set; }

        [DataMapping("DisplayName", DbType.String)]
        public string Name { get; set; }

        [DataMapping("MenuDescription", DbType.String)]
        public string Description { get; set; }

        [DataMapping("LanguageCode", DbType.AnsiStringFixedLength)]
        public string LanguageCode { get; set; }

        [DataMapping("IconStyle", DbType.String)]
        public string IconStyle { get; set; }

        [DataMapping("LinkPath", DbType.String)]
        public string LinkPath { get; set; }

        [DataMapping("InDate", DbType.DateTime)]
        public DateTime? InDate { get; set; }

        [DataMapping("InUser", DbType.String)]
        public string InUser { get; set; }

        [DataMapping("EditDate", DbType.DateTime)]
        public DateTime? EditDate { get; set; }

        [DataMapping("EditUser", DbType.String)]
        public string EditUser { get; set; }
    }
}
