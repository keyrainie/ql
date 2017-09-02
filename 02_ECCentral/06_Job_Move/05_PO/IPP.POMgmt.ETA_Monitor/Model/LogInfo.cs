using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IPPOversea.POmgmt.Model;

namespace IPPOversea.POmgmt.ETA.Model
{
    public class LogInfo
    {
        public LogInfo(POEntity entity)
        {
            this.OptIP = "127.0.0.1";
            this.OptTime = DateTime.Now;
            this.OptUserSysNo = 493;
            this.CompanyCode = Settings.CompanyCode;
            this.TicketType = Convert.ToInt32(Settings.TicketType);
        }

        public string Note { get; set; }
        public string OptIP { get; set; }
        public DateTime OptTime { get; set; }
        public int OptUserSysNo { get; set; }
        public string TicketSysNo { get; set; }
        public int TicketType { get; set; }
        public string CompanyCode { get; set; }
    }
}
