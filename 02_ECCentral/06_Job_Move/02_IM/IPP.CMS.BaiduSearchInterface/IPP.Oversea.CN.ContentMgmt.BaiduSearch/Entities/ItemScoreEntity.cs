using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using Newegg.Oversea.Framework.Entity;

namespace IPP.Oversea.CN.ContentMgmt.Baidu.Entities
{
    public class ItemScoreEntity
    {
        [DataMapping("ReviewCount", DbType.Int32)]
        public int? ReviewCount { get; set; }

        [DataMapping("RatingScore", DbType.Int32)]
        public int? RatingScore { get; set; }

        public string ScoreString
        {
            get
            {
                string strScore = string.Empty;

                if (!ReviewCount.HasValue || !RatingScore.HasValue)
                {
                    strScore = "0";
                }
                else
                {
                    decimal totalScore = RatingScore.Value;
                    decimal totalCount = ReviewCount.Value;
                    strScore = (ReviewCount == 0) ? "0" : (totalScore / totalCount).ToString("f1");
                }
                return strScore;
            }
        }
    }
}
