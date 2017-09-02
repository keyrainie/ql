using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newegg.Oversea.Framework.Entity;
using System.Data;

namespace IPP.ThirdPart.JobV31.BusinessEntities.IngramMicro
{
    public class InternalMemoReport
    {
        public int UserSysNo { get; set; }

        public string UserName { get; set; }

        public int ResolvedCount { get; set; }

        public int UnResolvedCount { get; set; }

        public int Count 
        {
            get
            {
                return this.ResolvedCount + this.UnResolvedCount;
            }
        }

        public string ResolvedRate
        {
            get
            { 
                double rate = ResolvedCount* 1.0 / this.Count;

                return rate.ToString("P");
            }
        }

    }
}
