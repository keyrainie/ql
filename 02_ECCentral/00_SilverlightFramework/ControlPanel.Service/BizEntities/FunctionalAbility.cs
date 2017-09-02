using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newegg.Oversea.Framework.Entity;
using System.Data;

namespace Newegg.Oversea.Silverlight.ControlPanel.Service.BizEntities
{
    public class FunctionalAbilityEntity
    {
        [DataMapping("Name", DbType.String)]
        public string Name { get; set; }

        [DataMapping("ApplicationId", DbType.Guid)]
        public string ApplicationId { get; set; }
    }
}
