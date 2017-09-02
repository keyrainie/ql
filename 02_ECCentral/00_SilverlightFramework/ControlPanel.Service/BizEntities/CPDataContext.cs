using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newegg.Oversea.Framework.DataConfiguration;

namespace Newegg.Oversea.Silverlight.ControlPanel.Service.BizEntities
{
    public class CPDataContext : ControlPanelDataContext
    {
        public CPDataContext(OperationType type)
            : base(DbInstanceManager.GetConnectionString(type == OperationType.Query ? "QueryFrameworkDB" : "OverseaLocalControlPanel"))
        {
            this.ExecuteCommand("SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED", new object[] { });
            this.CommandTimeout = 60;
        }
    }
    public enum OperationType
    {
        Query,
        Action
    }
}