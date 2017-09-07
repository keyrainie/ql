using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECommerce.Entity.Member
{
    public class Society
    {

        public int Sys_No { get; set; }
        public string SocietyID { get; set; }
        public int OrganizationID { get; set; }
        public string SocietyName { get; set; }
        public string Pwd { get; set; }
        public string Phone { get; set; }
        public string Description { get; set; }
        public string Status { get; set; }
        public DateTime InDate { get; set; }
        public string InUser { get; set; }
        public DateTime EditDate { get; set; }
        public string EditUser { get; set; }
    }
}
