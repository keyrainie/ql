using System;
using System.Linq;
using System.Text;
using System.Data;
using System.Collections.Generic;

using Newegg.Oversea.Framework.Entity;

namespace Newegg.Oversea.Silverlight.ControlPanel.Service.BizEntities
{
    public class ProfileItem
    {
        [DataMapping("ProfileId", DbType.Int32)]
        public int ProfileId { get; set; }

        [DataMapping("ApplicationId", DbType.AnsiStringFixedLength)]
        public string ApplicationId { get; set; }

        [DataMapping("ProfileType", DbType.AnsiString)]
        public string ProfileType { get; set; }

        [DataMapping("ProfileValue", DbType.Xml)]
        public string ProfileValue { get; set; }

        [DataMapping("ProfileItemGuid", DbType.AnsiStringFixedLength)]
        public string ProfileItemGuid { get; set; }

        [DataMapping("InDate", DbType.DateTime)]
        public DateTime InDate { get; set; }

        [DataMapping("InUser", DbType.String)]
        public string InUser { get; set; }
    }
}
