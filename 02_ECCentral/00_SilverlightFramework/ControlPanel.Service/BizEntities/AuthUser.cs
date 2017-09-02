using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

using Newegg.Oversea.Framework.Entity;

namespace Newegg.Oversea.Silverlight.ControlPanel.Service.BizEntities
{
    public class AuthUser
    {
        [DataMapping("UniqueName", DbType.String)]
        public string UniqueName { get; set; }

        [DataMapping("FirstName", DbType.String)]
        public string FirstName { get; set; }

        [DataMapping("LastName", DbType.String)]
        public string LastName { get; set; }
    }
}
