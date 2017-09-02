using System;
using System.Data;
using System.Linq;
using System.Text;
using System.Collections.Generic;

using Newegg.Oversea.Framework.Entity;

namespace Newegg.Oversea.Silverlight.ControlPanel.Service.BizEntities
{
    public class RoleEntity
    {
        [DataMapping("RoleID", DbType.String)]
        public string RoleID { get; set; }

        [DataMapping("RoleName", DbType.String)]
        public string RoleName { get; set; }

        [DataMapping("ApplicationID", DbType.String)]
        public string ApplicationID { get; set; }
    }
}
