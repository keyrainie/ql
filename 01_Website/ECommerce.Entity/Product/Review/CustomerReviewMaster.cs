using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace ECommerce.Entity.Product
{
    public class CustomerReviewMaster
    {
        public int SysNo
        {
            get;
            set;
        }


        public int CustomerSysNo
        {
            get;
            set;
        }


        public int ReviewCount
        {
            get;
            set;
        }




        public int ConsultCount
        {
            get;
            set;
        }



        public int RelpyCount
        {
            get;
            set;
        }



        public int AnswerConsultCount
        {
            get;
            set;
        }



        public int GuideCount
        {
            get;
            set;
        }



        public int DiscussCount
        {
            get;
            set;
        }


        public DateTime InDate
        {
            get;
            set;
        }



        public int FollowedCount
        {
            get;
            set;
        }


        public int IsFollow
        {
            get;
            set;
        }

    }
}
