using System.Data;
using Newegg.Oversea.Framework.Entity;
using System;
using System.Configuration;

namespace IPP.EcommerceMgmt.SendCustomerGuidePoints.Entities
{
    public class CustomerGuideEntity
    {
        private static int RankTScore = Convert.ToInt32(ConfigurationManager.AppSettings["RankTScore"]);
        private static int RankLScore = Convert.ToInt32(ConfigurationManager.AppSettings["RankLScore"]);
        private static int RankAScore = Convert.ToInt32(ConfigurationManager.AppSettings["RankAScore"]);
        private static int RankPScore = Convert.ToInt32(ConfigurationManager.AppSettings["RankPScore"]);

        [DataMapping("SysNo", DbType.Int32)]
        public int SysNo { get; set; }

        [DataMapping("CustomerSysNo", DbType.Int32)]
        public int CustomerSysNo { get; set; }

        private string contributeRank;

        [DataMapping("ContributeRank", DbType.AnsiStringFixedLength)]
        public string ContributeRank
        {
            get
            {
                return contributeRank;
            }
            set
            {
                contributeRank = value;
                switch (contributeRank)
                {
                    case "T":
                        CustomerPoint = RankTScore;
                        break;
                    case "L":
                        CustomerPoint = RankLScore;
                        break;
                    case "A":
                        CustomerPoint = RankAScore;
                        break;
                    case "P":
                        CustomerPoint = RankPScore;
                        break;
                }
            }
        }

        public int CustomerPoint { get; set; }
    }
}
