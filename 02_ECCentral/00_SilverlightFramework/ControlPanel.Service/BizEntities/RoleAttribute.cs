using System;
using System.Linq;
using System.Text;
using System.Data;
using System.Collections.Generic;
using Newegg.Oversea.Framework.Entity;


namespace Newegg.Oversea.Silverlight.ControlPanel.Service.BizEntities
{
    public class RoleAttributeEntity
    {
        [DataMapping("RoleName", DbType.String)]
        public string RoleName { get; set; }

        [DataMapping("Type", DbType.String)]
        public string Type { get; set; }

        [DataMapping("Name", DbType.String)]
        public string Name { get; set; }

        [DataMapping("Value", DbType.String)]
        public string Value { get; set; }

        [DataMapping("ApplicationId", DbType.String)]
        public string ApplicationId { get; set; }
    }
}
